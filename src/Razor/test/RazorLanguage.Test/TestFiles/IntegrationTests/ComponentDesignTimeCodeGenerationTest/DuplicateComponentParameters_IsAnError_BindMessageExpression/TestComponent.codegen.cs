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
        #pragma warning disable 219
        private void __RazorDirectiveTokenHelpers__() {
        }
        #pragma warning restore 219
        #pragma warning disable 0414
        private static System.Object __o = null;
        #pragma warning restore 0414
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.RenderTree.RenderTreeBuilder builder)
        {
            __o = Microsoft.AspNetCore.Components.RuntimeHelpers.TypeCheck<System.Linq.Expressions.Expression<System.Action<System.String>>>(
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
                                                           (s) => {}

#line default
#line hidden
#nullable disable
            );
            __o = Microsoft.AspNetCore.Components.RuntimeHelpers.TypeCheck<System.String>(Microsoft.AspNetCore.Components.BindMethods.GetValue(
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
                             message

#line default
#line hidden
#nullable disable
            ));
            __o = Microsoft.AspNetCore.Components.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.String>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.String>(this, 
            Microsoft.AspNetCore.Components.EventCallback.Factory.CreateInferred(this, __value => message = __value, message)));
            __o = Microsoft.AspNetCore.Components.RuntimeHelpers.TypeCheck<System.Linq.Expressions.Expression<System.Action<System.String>>>(() => message);
            builder.AddAttribute(-1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((builder2) => {
            }
            ));
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
__o = typeof(MyComponent);

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
#nullable restore
#line 2 "x:\dir\subdir\Test\TestComponent.cshtml"
            
    string message = "hi";

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
