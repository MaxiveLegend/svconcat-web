namespace svconcat_web.Extensions.ViewModelStrategy.Interfaces;

public interface IViewmodelFactory
{
    TTarget CreateViewModel<TSource, TTarget>(TSource source) where TTarget : class, new();
}