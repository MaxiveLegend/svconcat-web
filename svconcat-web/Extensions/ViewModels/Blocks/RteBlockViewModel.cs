using Umbraco.Cms.Core.Strings;

namespace svconcat_web.Extensions.ViewModels.Blocks;

public class RteBlockViewModel
{
    public IHtmlEncodedString Content { get; set; }
    
    public bool HasContent => !string.IsNullOrEmpty(Content?.ToString());
}