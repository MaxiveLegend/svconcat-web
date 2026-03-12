using SvConcatWeb.Extensions.Services;
using SvConcatWeb.Extensions.Services.Interfaces;
using Umbraco.Cms.Core.Composing;

namespace SvConcatWeb.Extensions.Composers;

[ComposeAfter(typeof(ViewModelFactoryComposer))]
public class AddServicesComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IModelService, ModelService>();
    }
}