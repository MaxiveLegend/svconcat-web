using svconcat_web.Extensions.ViewModelStrategy.Interfaces;

namespace svconcat_web.Extensions.ViewModelStrategy;

public class ViewModelFactory(IStrategyResolver strategyResolver) : IViewmodelFactory
{
    public TTarget CreateViewModel<TSource, TTarget>(TSource source) where TTarget : class, new()
    {
        if (source == null) return null;

        var strategy = strategyResolver.GetFor<TSource, TTarget>(source);

        var result = strategy.Execute(source);
        return result;
    }
}