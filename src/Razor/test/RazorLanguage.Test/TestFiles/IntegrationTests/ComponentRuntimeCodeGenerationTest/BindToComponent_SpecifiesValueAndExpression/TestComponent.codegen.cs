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
            builder.AddAttribute(1, "Value", Microsoft.AspNetCore.Components.RuntimeHelpers.TypeCheck<System.Int32>(Microsoft.AspNetCore.Components.BindMethods.GetValue(
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
                          ParentValue

#line default
#line hidden
#nullable disable
            )));
            builder.AddAttribute(2, "ValueChanged", new System.Action<System.Int32>(__value => ParentValue = __value));
            builder.AddAttribute(3, "ValueExpression", Microsoft.AspNetCore.Components.RuntimeHelpers.TypeCheck<System.Linq.Expressions.Expression<System.Func<System.Int32>>>(() => ParentValue));
            builder.CloseComponent();
        }
        #pragma warning restore 1998
#nullable restore
#line 2 "x:\dir\subdir\Test\TestComponent.cshtml"
       
    public int ParentValue { get; set; } = 42;

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
