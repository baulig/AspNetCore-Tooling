// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Components;

namespace Microsoft.CodeAnalysis.Razor
{
    internal class EventHandlerTagHelperDescriptorProvider : ITagHelperDescriptorProvider
    {
        public int Order { get; set; }

        public RazorEngine Engine { get; set; }

        public void Execute(TagHelperDescriptorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var compilation = context.GetCompilation();
            if (compilation == null)
            {
                return;
            }

            var bindMethods = compilation.GetTypeByMetadataName(ComponentsApi.BindMethods.FullTypeName);
            if (bindMethods == null)
            {
                // If we can't find BindMethods, then just bail. We won't be able to compile the
                // generated code anyway.
                return;
            }

            var eventHandlerData = GetEventHandlerData(compilation);

            foreach (var tagHelper in CreateEventHandlerTagHelpers(eventHandlerData))
            {
                context.Results.Add(tagHelper);
            }
        }

        private List<EventHandlerData> GetEventHandlerData(Compilation compilation)
        {
            var eventHandlerAttribute = compilation.GetTypeByMetadataName(ComponentsApi.EventHandlerAttribute.FullTypeName);
            if (eventHandlerAttribute == null)
            {
                // This won't likely happen, but just in case.
                return new List<EventHandlerData>();
            }

            var types = new List<INamedTypeSymbol>();
            var visitor = new EventHandlerDataVisitor(types);

            // Visit the primary output of this compilation, as well as all references.
            visitor.Visit(compilation.Assembly);
            foreach (var reference in compilation.References)
            {
                // We ignore .netmodules here - there really isn't a case where they are used by user code
                // even though the Roslyn APIs all support them.
                if (compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol assembly)
                {
                    visitor.Visit(assembly);
                }
            }

            var results = new List<EventHandlerData>();

            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var attributes = type.GetAttributes();

                // Not handling duplicates here for now since we're the primary ones extending this.
                // If we see users adding to the set of event handler constructs we will want to add deduplication
                // and potentially diagnostics.
                for (var j = 0; j < attributes.Length; j++)
                {
                    var attribute = attributes[j];

                    if (attribute.AttributeClass == eventHandlerAttribute)
                    {
                        results.Add(new EventHandlerData(
                            type.ContainingAssembly.Name,
                            type.ToDisplayString(),
                            (string)attribute.ConstructorArguments[0].Value,
                            (INamedTypeSymbol)attribute.ConstructorArguments[1].Value));
                    }
                }
            }

            return results;
        }

        private List<TagHelperDescriptor> CreateEventHandlerTagHelpers(List<EventHandlerData> data)
        {
            var results = new List<TagHelperDescriptor>();

            for (var i = 0; i < data.Count; i++)
            {
                var entry = data[i];
                var attributeName = "@" + entry.Attribute;
                var eventArgType = entry.EventArgsType.ToDisplayString();

                var builder = TagHelperDescriptorBuilder.Create(ComponentMetadata.EventHandler.TagHelperKind, entry.Attribute, ComponentsApi.AssemblyName);
                builder.CaseSensitive = true;
                builder.Documentation = string.Format(
                    ComponentResources.EventHandlerTagHelper_Documentation,
                    attributeName,
                    eventArgType);

                builder.Metadata.Add(ComponentMetadata.SpecialKindKey, ComponentMetadata.EventHandler.TagHelperKind);
                builder.Metadata.Add(ComponentMetadata.EventHandler.EventArgsType, eventArgType);
                builder.Metadata.Add(TagHelperMetadata.Common.ClassifyAttributesOnly, bool.TrueString);
                builder.Metadata[TagHelperMetadata.Runtime.Name] = ComponentMetadata.EventHandler.RuntimeName;

                // WTE has a bug in 15.7p1 where a Tag Helper without a display-name that looks like
                // a C# property will crash trying to create the tooltips.
                builder.SetTypeName(entry.TypeName);

                builder.TagMatchingRule(rule =>
                {
                    rule.TagName = "*";

                    rule.Attribute(a =>
                    {
                        a.Name = attributeName;
                        a.NameComparisonMode = RequiredAttributeDescriptor.NameComparisonMode.FullMatch;
                        a.Metadata[ComponentMetadata.Common.DirectiveAttribute] = bool.TrueString;
                    });
                });

                builder.BindAttribute(a =>
                {
                    a.Documentation = string.Format(
                        ComponentResources.EventHandlerTagHelper_Documentation,
                        attributeName,
                        eventArgType);

                    a.Name = attributeName;

                    // We want event handler directive attributes to default to C# context.
                    a.TypeName = $"Microsoft.AspNetCore.Components.EventCallback<{eventArgType}>";

                    // But make this weakly typed (don't type check) - delegates have their own type-checking
                    // logic that we don't want to interfere with.
                    a.Metadata.Add(ComponentMetadata.Component.WeaklyTypedKey, bool.TrueString);

                    a.Metadata[ComponentMetadata.Common.DirectiveAttribute] = bool.TrueString;

                    // WTE has a bug 15.7p1 where a Tag Helper without a display-name that looks like
                    // a C# property will crash trying to create the tooltips.
                    a.SetPropertyName(entry.Attribute);
                });

                results.Add(builder.Build());
            }

            return results;
        }

        private struct EventHandlerData
        {
            public EventHandlerData(
                string assembly,
                string typeName,
                string element,
                INamedTypeSymbol eventArgsType)
            {
                Assembly = assembly;
                TypeName = typeName;
                Attribute = element;
                EventArgsType = eventArgsType;
            }

            public string Assembly { get; }

            public string TypeName { get; }

            public string Attribute { get; }

            public INamedTypeSymbol EventArgsType { get; }
        }

        private class EventHandlerDataVisitor : SymbolVisitor
        {
            private List<INamedTypeSymbol> _results;

            public EventHandlerDataVisitor(List<INamedTypeSymbol> results)
            {
                _results = results;
            }

            public override void VisitNamedType(INamedTypeSymbol symbol)
            {
                if (symbol.Name == "EventHandlers" && symbol.DeclaredAccessibility == Accessibility.Public)
                {
                    _results.Add(symbol);
                }
            }

            public override void VisitNamespace(INamespaceSymbol symbol)
            {
                foreach (var member in symbol.GetMembers())
                {
                    Visit(member);
                }
            }

            public override void VisitAssembly(IAssemblySymbol symbol)
            {
                // This as a simple yet high-value optimization that excludes the vast majority of
                // assemblies that (by definition) can't contain a component.
                if (symbol.Name != null && !symbol.Name.StartsWith("System.", StringComparison.Ordinal))
                {
                    Visit(symbol.GlobalNamespace);
                }
            }
        }
    }
}
