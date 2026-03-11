using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using svconcat_web.Extensions.Services.Interfaces;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace svconcat_web.Extensions.Controllers;

public class DefaultController(
    ICompositeViewEngine viewEngine,
    ILogger<RenderController> logger,
    IUmbracoContextAccessor umbracoContextAccessor,
    IModelService modelService) : RenderController(logger, viewEngine, umbracoContextAccessor)
{
    public override IActionResult Index()
    {
        return CurrentTemplate(modelService.CreateMasterModel(CurrentPage));
    }
}