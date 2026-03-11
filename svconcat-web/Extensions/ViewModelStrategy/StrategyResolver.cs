using svconcat_web.Extensions.ViewModelStrategy.Interfaces;

namespace svconcat_web.Extensions.ViewModelStrategy;

public class StrategyResolver(IServiceProvider serviceProvider) : IStrategyResolver
{
    public IMappingStrategy<TSource, TTarget> GetFor<TSource, TTarget>(TSource source)
    {
        var strategies = serviceProvider.GetServices<IMappingStrategy<TSource, TTarget>>();

        if (!strategies.Any()) throw new Exception(
            $"No mapping strategy defined for source type {typeof(TSource).FullName} to target type {typeof(TTarget).FullName}");
        if (strategies.Count() > 1)
            throw new Exception(
                $"More than one mapping strategy defined for source type {typeof(TSource).FullName} to target type {typeof(TTarget).FullName}");
        
        return strategies.First();
    }
}