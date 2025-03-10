// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace Microsoft.AspNetCore.Razor.Language.Components
{
    // Based on the DesignTimeNodeWriter from Razor repo.
    internal class ComponentDesignTimeNodeWriter : ComponentNodeWriter
    {
        private readonly ScopeStack _scopeStack = new ScopeStack();

        private static readonly string DesignTimeVariable = "__o";

        public override void WriteMarkupBlock(CodeRenderingContext context, MarkupBlockIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Do nothing
        }

        public override void WriteMarkupElement(CodeRenderingContext context, MarkupElementIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            context.RenderChildren(node);
        }

        public override void WriteUsingDirective(CodeRenderingContext context, UsingDirectiveIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Source.HasValue)
            {
                using (context.CodeWriter.BuildLinePragma(node.Source.Value, context))
                {
                    context.AddSourceMappingFor(node);
                    context.CodeWriter.WriteUsing(node.Content);
                }
            }
            else
            {
                context.CodeWriter.WriteUsing(node.Content);
            }
        }

        public override void WriteCSharpExpression(CodeRenderingContext context, CSharpExpressionIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Children.Count == 0)
            {
                return;
            }

            if (node.Source != null)
            {
                using (context.CodeWriter.BuildLinePragma(node.Source.Value, context))
                {
                    var offset = DesignTimeVariable.Length + " = ".Length;
                    context.CodeWriter.WritePadding(offset, node.Source, context);
                    context.CodeWriter.WriteStartAssignment(DesignTimeVariable);

                    for (var i = 0; i < node.Children.Count; i++)
                    {
                        if (node.Children[i] is IntermediateToken token && token.IsCSharp)
                        {
                            context.AddSourceMappingFor(token);
                            context.CodeWriter.Write(token.Content);
                        }
                        else
                        {
                            // There may be something else inside the expression like a Template or another extension node.
                            context.RenderNode(node.Children[i]);
                        }
                    }

                    context.CodeWriter.WriteLine(";");
                }
            }
            else
            {
                context.CodeWriter.WriteStartAssignment(DesignTimeVariable);
                for (var i = 0; i < node.Children.Count; i++)
                {
                    if (node.Children[i] is IntermediateToken token && token.IsCSharp)
                    {
                        context.CodeWriter.Write(token.Content);
                    }
                    else
                    {
                        // There may be something else inside the expression like a Template or another extension node.
                        context.RenderNode(node.Children[i]);
                    }
                }
                context.CodeWriter.WriteLine(";");
            }
        }

        public override void WriteCSharpCode(CodeRenderingContext context, CSharpCodeIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var isWhitespaceStatement = true;
            for (var i = 0; i < node.Children.Count; i++)
            {
                var token = node.Children[i] as IntermediateToken;
                if (token == null || !string.IsNullOrWhiteSpace(token.Content))
                {
                    isWhitespaceStatement = false;
                    break;
                }
            }

            IDisposable linePragmaScope = null;
            if (node.Source != null)
            {
                if (!isWhitespaceStatement)
                {
                    linePragmaScope = context.CodeWriter.BuildLinePragma(node.Source.Value, context);
                }

                context.CodeWriter.WritePadding(0, node.Source.Value, context);
            }
            else if (isWhitespaceStatement)
            {
                // Don't write whitespace if there is no line mapping for it.
                return;
            }

            for (var i = 0; i < node.Children.Count; i++)
            {
                if (node.Children[i] is IntermediateToken token && token.IsCSharp)
                {
                    context.AddSourceMappingFor(token);
                    context.CodeWriter.Write(token.Content);
                }
                else
                {
                    // There may be something else inside the statement like an extension node.
                    context.RenderNode(node.Children[i]);
                }
            }

            if (linePragmaScope != null)
            {
                linePragmaScope.Dispose();
            }
            else
            {
                context.CodeWriter.WriteLine();
            }
        }

        public override void WriteHtmlAttribute(CodeRenderingContext context, HtmlAttributeIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            context.RenderChildren(node);
        }

        public override void WriteHtmlAttributeValue(CodeRenderingContext context, HtmlAttributeValueIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Do nothing, this can't contain code.
        }

        public override void WriteCSharpExpressionAttributeValue(CodeRenderingContext context, CSharpExpressionAttributeValueIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Children.Count == 0)
            {
                return;
            }

            context.CodeWriter.WriteStartAssignment(DesignTimeVariable);
            for (var i = 0; i < node.Children.Count; i++)
            {
                if (node.Children[i] is IntermediateToken token && token.IsCSharp)
                {
                    WriteCSharpToken(context, token);
                }
                else
                {
                    // There may be something else inside the expression like a Template or another extension node.
                    context.RenderNode(node.Children[i]);
                }
            }
            context.CodeWriter.WriteLine(";");
        }

        public override void WriteHtmlContent(CodeRenderingContext context, HtmlContentIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Do nothing
        }

        protected override void BeginWriteAttribute(CodeWriter codeWriter, string key)
        {
            if (codeWriter == null)
            {
                throw new ArgumentNullException(nameof(codeWriter));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            codeWriter
                .WriteStartMethodInvocation($"{_scopeStack.BuilderVarName}.{nameof(ComponentsApi.RenderTreeBuilder.AddAttribute)}")
                .Write("-1")
                .WriteParameterSeparator()
                .WriteStringLiteral(key)
                .WriteParameterSeparator();
        }

        public override void WriteComponent(CodeRenderingContext context, ComponentIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.TypeInferenceNode == null)
            {
                // Writes something like:
                //
                // builder.OpenComponent<MyComponent>(0);
                // builder.AddAttribute(1, "Foo", ...);
                // builder.AddAttribute(2, "ChildContent", ...);
                // builder.SetKey(someValue);
                // builder.AddElementCapture(3, (__value) => _field = __value);
                // builder.CloseComponent();

                foreach (var typeArgument in node.TypeArguments)
                {
                    context.RenderNode(typeArgument);
                }

                // We need to preserve order for attibutes and attribute splats since the ordering
                // has a semantic effect.

                foreach (var child in node.Children)
                {
                    if (child is ComponentAttributeIntermediateNode attribute)
                    {
                        context.RenderNode(attribute);
                    }
                    else if (child is SplatIntermediateNode splat)
                    {
                        context.RenderNode(splat);
                    }
                }

                if (node.ChildContents.Any())
                {
                    foreach (var childContent in node.ChildContents)
                    {
                        context.RenderNode(childContent);
                    }
                }
                else
                {
                    // We eliminate 'empty' child content when building the tree so that usage like
                    // '<MyComponent>\r\n</MyComponent>' doesn't create a child content.
                    //
                    // Consider what would happen if the user's cursor was inside the element. At
                    // design -time we want to render an empty lambda to provide proper scoping
                    // for any code that the user types.
                    context.RenderNode(new ComponentChildContentIntermediateNode()
                    {
                        TypeName = ComponentsApi.RenderFragment.FullTypeName,
                    });
                }

                foreach (var setKey in node.SetKeys)
                {
                    context.RenderNode(setKey);
                }

                foreach (var capture in node.Captures)
                {
                    context.RenderNode(capture);
                }
            }
            else
            {
                // When we're doing type inference, we can't write all of the code inline to initialize
                // the component on the builder. We generate a method elsewhere, and then pass all of the information
                // to that method. We pass in all of the attribute values + the sequence numbers.
                //
                // __Blazor.MyComponent.TypeInference.CreateMyComponent_0(builder, 0, 1, ..., 2, ..., 3, ....);

                // Preserve order of attributes + splats
                var attributes = node.Children.Where(s =>
                {
                    return s is ComponentAttributeIntermediateNode || s is SplatIntermediateNode;
                }).ToList();
                var childContents = node.ChildContents.ToList();
                var captures = node.Captures.ToList();
                var setKeys = node.SetKeys.ToList();
                var remaining = attributes.Count + childContents.Count + captures.Count + setKeys.Count;

                context.CodeWriter.Write(node.TypeInferenceNode.FullTypeName);
                context.CodeWriter.Write(".");
                context.CodeWriter.Write(node.TypeInferenceNode.MethodName);
                context.CodeWriter.Write("(");

                context.CodeWriter.Write(_scopeStack.BuilderVarName);
                context.CodeWriter.Write(", ");

                context.CodeWriter.Write("-1");
                context.CodeWriter.Write(", ");

                for (var i = 0; i < attributes.Count; i++)
                {
                    context.CodeWriter.Write("-1");
                    context.CodeWriter.Write(", ");

                    // Don't type check generics, since we can't actually write the type name.
                    // The type checking with happen anyway since we defined a method and we're generating
                    // a call to it.
                    if (attributes[i] is ComponentAttributeIntermediateNode attribute)
                    {
                        WriteComponentAttributeInnards(context, attribute, canTypeCheck: false);
                    }
                    else if (attributes[i] is SplatIntermediateNode splat)
                    {
                        WriteSplatInnards(context, splat, canTypeCheck: false);
                    }

                    remaining--;
                    if (remaining > 0)
                    {
                        context.CodeWriter.Write(", ");
                    }
                }

                for (var i = 0; i < childContents.Count; i++)
                {
                    context.CodeWriter.Write("-1");
                    context.CodeWriter.Write(", ");

                    WriteComponentChildContentInnards(context, childContents[i]);

                    remaining--;
                    if (remaining > 0)
                    {
                        context.CodeWriter.Write(", ");
                    }
                }

                for (var i = 0; i < setKeys.Count; i++)
                {
                    context.CodeWriter.Write("-1");
                    context.CodeWriter.Write(", ");

                    WriteSetKeyInnards(context, setKeys[i]);

                    remaining--;
                    if (remaining > 0)
                    {
                        context.CodeWriter.Write(", ");
                    }
                }

                for (var i = 0; i < captures.Count; i++)
                {
                    context.CodeWriter.Write("-1");
                    context.CodeWriter.Write(", ");

                    WriteReferenceCaptureInnards(context, captures[i], shouldTypeCheck: false);

                    remaining--;
                    if (remaining > 0)
                    {
                        context.CodeWriter.Write(", ");
                    }
                }

                context.CodeWriter.Write(");");
                context.CodeWriter.WriteLine();
            }

            // We want to generate something that references the Component type to avoid
            // the "usings directive is unnecessary" message.
            // Looks like:
            // __o = typeof(SomeNamespace.SomeComponent);
            using (context.CodeWriter.BuildLinePragma(node.Source.Value, context))
            {
                context.CodeWriter.Write(DesignTimeVariable);
                context.CodeWriter.Write(" = ");
                context.CodeWriter.Write("typeof(");
                context.CodeWriter.Write(node.TagName);
                if (node.Component.IsGenericTypedComponent())
                {
                    context.CodeWriter.Write("<");
                    var typeArgumentCount = node.Component.GetTypeParameters().Count();
                    for (var i = 1; i < typeArgumentCount; i++)
                    {
                        context.CodeWriter.Write(",");
                    }
                    context.CodeWriter.Write(">");
                }
                context.CodeWriter.Write(");");
                context.CodeWriter.WriteLine();
            }
        }

        public override void WriteComponentAttribute(CodeRenderingContext context, ComponentAttributeIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Looks like:
            // __o = 17;
            context.CodeWriter.Write(DesignTimeVariable);
            context.CodeWriter.Write(" = ");

            // Following the same design pattern as the runtime codegen
            WriteComponentAttributeInnards(context, node, canTypeCheck: true);

            context.CodeWriter.Write(";");
            context.CodeWriter.WriteLine();
        }

        private void WriteComponentAttributeInnards(CodeRenderingContext context, ComponentAttributeIntermediateNode node, bool canTypeCheck)
        {
            // We limit component attributes to simple cases. However there is still a lot of complexity
            // to handle here, since there are a few different cases for how an attribute might be structured.
            //
            // This roughly follows the design of the runtime writer for simplicity.
            if (node.AttributeStructure == AttributeStructure.Minimized)
            {
                // Minimized attributes always map to 'true'
                context.CodeWriter.Write("true");
            }
            else if (node.Children.Count > 1)
            {
                // We don't expect this to happen, we just want to know if it can.
                throw new InvalidOperationException("Attribute nodes should either be minimized or a single type of content." + string.Join(", ", node.Children));
            }
            else if (node.Children.Count == 1 && node.Children[0] is HtmlContentIntermediateNode)
            {
                // We don't actually need the content at designtime, an empty string will do.
                context.CodeWriter.Write("\"\"");
            }
            else
            {
                // There are a few different forms that could be used to contain all of the tokens, but we don't really care
                // exactly what it looks like - we just want all of the content.
                //
                // This can include an empty list in some cases like the following (sic):
                //      <MyComponent Value="
                //
                // Or a CSharpExpressionIntermediateNode when the attribute has an explicit transition like:
                //      <MyComponent Value="@value" />
                //
                // Of a list of tokens directly in the attribute.
                var tokens = GetCSharpTokens(node);

                if ((node.BoundAttribute?.IsDelegateProperty() ?? false) ||
                    (node.BoundAttribute?.IsChildContentProperty() ?? false))
                {
                    // We always surround the expression with the delegate constructor. This makes type
                    // inference inside lambdas, and method group conversion do the right thing.
                    if (canTypeCheck)
                    {
                        context.CodeWriter.Write("new ");
                        context.CodeWriter.Write(node.TypeName);
                        context.CodeWriter.Write("(");
                    }
                    context.CodeWriter.WriteLine();

                    for (var i = 0; i < tokens.Count; i++)
                    {
                        WriteCSharpToken(context, tokens[i]);
                    }

                    if (canTypeCheck)
                    {
                        context.CodeWriter.Write(")");
                    }
                }
                else if (node.BoundAttribute?.IsEventCallbackProperty() ?? false)
                {
                    // This is the case where we are writing an EventCallback (a delegate with super-powers).
                    //
                    // An event callback can either be passed verbatim, or it can be created by the EventCallbackFactory.
                    // Since we don't look at the code the user typed inside the attribute value, this is always
                    // resolved via overloading.

                    if (canTypeCheck && NeedsTypeCheck(node))
                    {
                        context.CodeWriter.Write(ComponentsApi.RuntimeHelpers.TypeCheck);
                        context.CodeWriter.Write("<");
                        context.CodeWriter.Write(node.TypeName);
                        context.CodeWriter.Write(">");
                        context.CodeWriter.Write("(");
                    }

                    // Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, ...) OR
                    // Microsoft.AspNetCore.Components.EventCallback.Factory.Create<T>(this, ...)

                    context.CodeWriter.Write(ComponentsApi.EventCallback.FactoryAccessor);
                    context.CodeWriter.Write(".");
                    context.CodeWriter.Write(ComponentsApi.EventCallbackFactory.CreateMethod);

                    if (node.TryParseEventCallbackTypeArgument(out var argument))
                    {
                        context.CodeWriter.Write("<");
                        context.CodeWriter.Write(argument);
                        context.CodeWriter.Write(">");
                    }

                    context.CodeWriter.Write("(");
                    context.CodeWriter.Write("this");
                    context.CodeWriter.Write(", ");

                    context.CodeWriter.WriteLine();

                    for (var i = 0; i < tokens.Count; i++)
                    {
                        WriteCSharpToken(context, tokens[i]);
                    }

                    context.CodeWriter.Write(")");

                    if (canTypeCheck && NeedsTypeCheck(node))
                    {
                        context.CodeWriter.Write(")");
                    }
                }
                else
                {
                    // This is the case when an attribute contains C# code
                    //
                    // If we have a parameter type, then add a type check.
                    if (canTypeCheck && NeedsTypeCheck(node))
                    {
                        context.CodeWriter.Write(ComponentsApi.RuntimeHelpers.TypeCheck);
                        context.CodeWriter.Write("<");
                        context.CodeWriter.Write(node.TypeName);
                        context.CodeWriter.Write(">");
                        context.CodeWriter.Write("(");
                    }

                    for (var i = 0; i < tokens.Count; i++)
                    {
                        WriteCSharpToken(context, tokens[i]);
                    }

                    if (canTypeCheck && NeedsTypeCheck(node))
                    {
                        context.CodeWriter.Write(")");
                    }
                }
            }

            bool NeedsTypeCheck(ComponentAttributeIntermediateNode n)
            {
                return n.BoundAttribute != null && !n.BoundAttribute.IsWeaklyTyped();
            }   
        }

        private IReadOnlyList<IntermediateToken> GetCSharpTokens(IntermediateNode node)
        {
            // We generally expect all children to be CSharp, this is here just in case.
            return node.FindDescendantNodes<IntermediateToken>().Where(t => t.IsCSharp).ToArray();
        }

        public override void WriteComponentChildContent(CodeRenderingContext context, ComponentChildContentIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Writes something like:
            //
            // builder.AddAttribute(1, "ChildContent", (RenderFragment)((__builder73) => { ... }));
            // OR
            // builder.AddAttribute(1, "ChildContent", (RenderFragment<Person>)((person) => (__builder73) => { ... }));
            BeginWriteAttribute(context.CodeWriter, node.AttributeName);
            context.CodeWriter.Write($"({node.TypeName})(");

            WriteComponentChildContentInnards(context, node);

            context.CodeWriter.Write(")");
            context.CodeWriter.WriteEndMethodInvocation();
        }

        private void WriteComponentChildContentInnards(CodeRenderingContext context, ComponentChildContentIntermediateNode node)
        {
            // Writes something like:
            //
            // ((__builder73) => { ... })
            // OR
            // ((person) => (__builder73) => { })
            _scopeStack.OpenComponentScope(
                context,
                node.AttributeName,
                node.IsParameterized ? node.ParameterName : null);
            for (var i = 0; i < node.Children.Count; i++)
            {
                context.RenderNode(node.Children[i]);
            }
            _scopeStack.CloseScope(context);
        }

        public override void WriteComponentTypeArgument(CodeRenderingContext context, ComponentTypeArgumentIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // At design type we want write the equivalent of:
            //
            // __o = typeof(TItem);
            context.CodeWriter.Write(DesignTimeVariable);
            context.CodeWriter.Write(" = ");
            context.CodeWriter.Write("typeof(");

            var tokens = GetCSharpTokens(node);
            for (var i = 0; i < tokens.Count; i++)
            {
                WriteCSharpToken(context, tokens[i]);
            }

            context.CodeWriter.Write(");");
            context.CodeWriter.WriteLine();

            IReadOnlyList<IntermediateToken> GetCSharpTokens(ComponentTypeArgumentIntermediateNode arg)
            {
                // We generally expect all children to be CSharp, this is here just in case.
                return arg.FindDescendantNodes<IntermediateToken>().Where(t => t.IsCSharp).ToArray();
            }
        }

        public override void WriteTemplate(CodeRenderingContext context, TemplateIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Looks like:
            //
            // (__builder73) => { ... }
            _scopeStack.OpenTemplateScope(context);
            context.RenderChildren(node);
            _scopeStack.CloseScope(context);
        }

        public override void WriteSetKey(CodeRenderingContext context, SetKeyIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Looks like:
            //
            // builder.SetKey(_keyValue);

            var codeWriter = context.CodeWriter;

            codeWriter
                .WriteStartMethodInvocation($"{_scopeStack.BuilderVarName}.{ComponentsApi.RenderTreeBuilder.SetKey}");
            WriteSetKeyInnards(context, node);
            codeWriter.WriteEndMethodInvocation();
        }

        private void WriteSetKeyInnards(CodeRenderingContext context, SetKeyIntermediateNode node)
        {
            WriteCSharpCode(context, new CSharpCodeIntermediateNode
            {
                Source = node.Source,
                Children =
                    {
                        node.KeyValueToken
                    }
            });
        }

        public override void WriteSplat(CodeRenderingContext context, SplatIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Looks like:
            //
            // builder.AddMultipleAttributes(2, ...);
            context.CodeWriter.WriteStartMethodInvocation($"{_scopeStack.BuilderVarName}.{ComponentsApi.RenderTreeBuilder.AddMultipleAttributes}");
            context.CodeWriter.Write("-1");
            context.CodeWriter.WriteParameterSeparator();

            WriteSplatInnards(context, node, canTypeCheck: true);

            context.CodeWriter.WriteEndMethodInvocation();
        }

        private void WriteSplatInnards(CodeRenderingContext context, SplatIntermediateNode node, bool canTypeCheck)
        {
            var tokens = GetCSharpTokens(node);

            if (canTypeCheck)
            {
                context.CodeWriter.Write(ComponentsApi.RuntimeHelpers.TypeCheck);
                context.CodeWriter.Write("<");
                context.CodeWriter.Write(ComponentsApi.AddMultipleAttributesTypeFullName);
                context.CodeWriter.Write(">");
                context.CodeWriter.Write("(");
            }

            for (var i = 0; i < tokens.Count; i++)
            {
                WriteCSharpToken(context, tokens[i]);
            }

            if (canTypeCheck)
            {
                context.CodeWriter.Write(")");
            }
        }

        public override void WriteReferenceCapture(CodeRenderingContext context, ReferenceCaptureIntermediateNode node)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Looks like:
            //
            // __field = default(MyComponent);
            WriteReferenceCaptureInnards(context, node, shouldTypeCheck: true);
        }

        protected override void WriteReferenceCaptureInnards(CodeRenderingContext context, ReferenceCaptureIntermediateNode node, bool shouldTypeCheck)
        {
            // We specialize this code based on whether or not we can type check. When we're calling into
            // a type-inferenced component, we can't do the type check. See the comments in WriteTypeInferenceMethod.
            if (shouldTypeCheck)
            {
                // The runtime node writer moves the call elsewhere. At design time we
                // just want sufficiently similar code that any unknown-identifier or type
                // errors will be equivalent
                var captureTypeName = node.IsComponentCapture
                    ? node.ComponentCaptureTypeName
                    : ComponentsApi.ElementRef.FullTypeName;
                WriteCSharpCode(context, new CSharpCodeIntermediateNode
                {
                    Source = node.Source,
                    Children =
                    {
                        node.IdentifierToken,
                        new IntermediateToken
                        {
                            Kind = TokenKind.CSharp,
                            Content = $" = default({captureTypeName});"
                        }
                    }
                });
            }
            else
            {
                // Looks like:
                //
                // (__value) = { _field = (MyComponent)__value; }
                // OR
                // (__value) = { _field = (ElementRef)__value; }
                const string refCaptureParamName = "__value";
                using (var lambdaScope = context.CodeWriter.BuildLambda(refCaptureParamName))
                {
                    WriteCSharpCode(context, new CSharpCodeIntermediateNode
                    {
                        Source = node.Source,
                        Children =
                        {
                            node.IdentifierToken,
                            new IntermediateToken
                            {
                                Kind = TokenKind.CSharp,
                                Content = $" = {refCaptureParamName};"
                            }
                        }
                    });
                }
            }
        }

        private void WriteCSharpToken(CodeRenderingContext context, IntermediateToken token)
        {
            if (string.IsNullOrWhiteSpace(token.Content))
            {
                return;
            }

            if (token.Source?.FilePath == null)
            {
                context.CodeWriter.Write(token.Content);
                return;
            }

            using (context.CodeWriter.BuildLinePragma(token.Source, context))
            {
                context.CodeWriter.WritePadding(0, token.Source.Value, context);
                context.AddSourceMappingFor(token);
                context.CodeWriter.Write(token.Content);
            }
        }
    }
}
