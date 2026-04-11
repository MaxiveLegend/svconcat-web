using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class CtaBlockToCtaBlockViewModel(IViewmodelFactory viewmodelFactory) : IMappingStrategy<CtaBlock, CtaBlockViewModel>
{
    public CtaBlockViewModel Execute(CtaBlock source)
    {
        var vm = new CtaBlockViewModel();

        if (source == null) return vm;

        vm.Title = source.Title ?? string.Empty;
        vm.Text = source.Text ?? string.Empty;
        vm.Links = source.Cta?.Select(viewmodelFactory.CreateViewModel<Link, LinkViewModel>) ?? [];

        return vm;
    }
}