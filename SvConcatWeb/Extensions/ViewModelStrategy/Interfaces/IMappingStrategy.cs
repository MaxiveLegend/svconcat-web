namespace SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;

public interface IMappingStrategy<in TSource, out TTarget>
{
    TTarget Execute(TSource source);
}