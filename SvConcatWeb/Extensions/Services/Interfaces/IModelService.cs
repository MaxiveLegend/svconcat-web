using SvConcatWeb.Extensions.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace SvConcatWeb.Extensions.Services.Interfaces;

public interface IModelService
{
    IMasterModel CreateMasterModel(IPublishedContent content, Type contentType = null);
}