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
            __Blazor.Test.TestComponent.TypeInference.CreateMyComponent_0(builder, 0, 1, Microsoft.AspNetCore.Components.BindMethods.GetValue(
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
                              ParentValue

#line default
#line hidden
#nullable disable
            ), 2, Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, Microsoft.AspNetCore.Components.EventCallback.Factory.CreateInferred(this, __value => ParentValue = __value, ParentValue)), 3, () => ParentValue);
        }
        #pragma warning restore 1998
#nullable restore
#line 2 "x:\dir\subdir\Test\TestComponent.cshtml"
       
    public DateTime ParentValue { get; set; } = DateTime.Now;

#line default
#line hidden
#nullable disable
    }
}
namespace __Blazor.Test.TestComponent
{
    #line hidden
    internal static class TypeInference
    {
        public static void CreateMyComponent_0<T>(global::Microsoft.AspNetCore.Components.RenderTree.RenderTreeBuilder builder, int seq, int __seq0, T __arg0, int __seq1, global::Microsoft.AspNetCore.Components.EventCallback<T> __arg1, int __seq2, global::System.Linq.Expressions.Expression<global::System.Func<T>> __arg2)
        {
        builder.OpenComponent<global::Test.MyComponent<T>>(seq);
        builder.AddAttribute(__seq0, "SomeParam", __arg0);
        builder.AddAttribute(__seq1, "SomeParamChanged", __arg1);
        builder.AddAttribute(__seq2, "SomeParamExpression", __arg2);
        builder.CloseComponent();
        }
    }
}
#pragma warning restore 1591
