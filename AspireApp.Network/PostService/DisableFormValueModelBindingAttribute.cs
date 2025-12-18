using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PostService
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingAttribute : Attribute, IAsyncResourceFilter
    {
        //IResourceFilter
        //public void OnResourceExecuting(ResourceExecutingContext context)
        //{
        //    var factories = context.ValueProviderFactories;
        //    factories.RemoveType<FormValueProviderFactory>();
        //    factories.RemoveType<FormFileValueProviderFactory>();
        //    factories.RemoveType<JQueryFormValueProviderFactory>();
        //}

        //public void OnResourceExecuted(ResourceExecutedContext context)
        //{
        //}

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var factories = context.ValueProviderFactories;
            factories.RemoveType<FormValueProviderFactory>();
            factories.RemoveType<FormFileValueProviderFactory>();
            factories.RemoveType<JQueryFormValueProviderFactory>();
            ResourceExecutedContext executedContext = await next();
        }
    }
}
