// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Razor.Language.Components;

namespace Microsoft.AspNetCore.Razor.Language.Intermediate
{
    public sealed class ReferenceCaptureIntermediateNode : IntermediateNode
    {
        public ReferenceCaptureIntermediateNode(IntermediateToken identifierToken)
        {
            IdentifierToken = identifierToken ?? throw new ArgumentNullException(nameof(identifierToken));
            Source = IdentifierToken.Source;
        }

        public ReferenceCaptureIntermediateNode(IntermediateToken identifierToken, string componentCaptureTypeName)
            : this(identifierToken)
        {
            if (string.IsNullOrEmpty(componentCaptureTypeName))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(componentCaptureTypeName));
            }

            IsComponentCapture = true;
            ComponentCaptureTypeName = componentCaptureTypeName;
        }

        public override IntermediateNodeCollection Children => IntermediateNodeCollection.ReadOnly;

        public IntermediateToken IdentifierToken { get; }

        public bool IsComponentCapture { get; }

        public string ComponentCaptureTypeName { get; set; }

        public string FieldTypeName => IsComponentCapture ? ComponentCaptureTypeName : "global::" + ComponentsApi.ElementRef.FullTypeName;

        public string TypeName => $"global::System.Action<{FieldTypeName}>";

        public override void Accept(IntermediateNodeVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.VisitReferenceCapture(this);
        }

        public override void FormatNode(IntermediateNodeFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            
            formatter.WriteContent(IdentifierToken?.Content);
            
            formatter.WriteProperty(nameof(IdentifierToken), IdentifierToken?.Content);
            formatter.WriteProperty(nameof(ComponentCaptureTypeName), ComponentCaptureTypeName);
        }
    }
}
