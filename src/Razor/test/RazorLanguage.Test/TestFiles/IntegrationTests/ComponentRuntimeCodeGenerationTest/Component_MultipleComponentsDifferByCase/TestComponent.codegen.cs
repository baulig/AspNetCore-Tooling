// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    public class TestComponent : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.RenderTree.RenderTreeBuilder builder)
        {
            builder.OpenComponent<Test.MyComponent>(0);
            builder.AddAttribute(1, "IntProperty", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Int32>(
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
                          1

#line default
#line hidden
#nullable disable
            ));
            builder.CloseComponent();
            builder.AddMarkupContent(2, "\r\n");
            builder.OpenComponent<Test.Mycomponent>(3);
            builder.AddAttribute(4, "IntProperty", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Int32>(
#nullable restore
#line 2 "x:\dir\subdir\Test\TestComponent.cshtml"
                          2

#line default
#line hidden
#nullable disable
            ));
            builder.CloseComponent();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
