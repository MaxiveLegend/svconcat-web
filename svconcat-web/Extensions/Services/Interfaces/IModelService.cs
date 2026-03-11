using svconcat_web.Extensions.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace svconcat_web.Extensions.Services.Interfaces;

public interface IModelService
{
    IMasterModel CreateMasterModel(IPublishedContent content, Type contentType = null);
}