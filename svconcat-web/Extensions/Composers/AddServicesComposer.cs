using svconcat_web.Extensions.Services;
using svconcat_web.Extensions.Services.Interfaces;
using Umbraco.Cms.Core.Composing;

namespace svconcat_web.Extensions.Composers;

[ComposeAfter(typeof(ViewModelFactoryComposer))]
public class AddServicesComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IModelService, ModelService>();
    }
}