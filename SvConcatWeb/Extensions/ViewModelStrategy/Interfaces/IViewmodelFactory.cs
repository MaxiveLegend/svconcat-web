namespace SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;

public interface IViewmodelFactory
{
    TTarget CreateViewModel<TSource, TTarget>(TSource source) where TTarget : class, new();
}