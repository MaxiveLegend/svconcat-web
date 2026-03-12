using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SvConcatWeb.Extensions.Services.Interfaces;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace SvConcatWeb.Extensions.Controllers;

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