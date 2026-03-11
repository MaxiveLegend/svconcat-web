namespace svconcat_web.Extensions.ViewModelStrategy.Interfaces;

public interface IStrategyResolver
{
    IMappingStrategy<TSource, TTarget> GetFor<TSource, TTarget>(TSource source);
}