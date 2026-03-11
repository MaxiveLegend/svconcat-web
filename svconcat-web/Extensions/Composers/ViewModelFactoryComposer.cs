using System.Reflection;
using svconcat_web.Extensions.ViewModelStrategy;
using svconcat_web.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Composing;

namespace svconcat_web.Extensions.Composers;

public class ViewModelFactoryComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        var MappingStrategies = DiscoverInternalStrategies();

        if (!MappingStrategies.Equals(default(IEnumerable<Type>)))
        {
            RegisterStrategies(builder, MappingStrategies);
        }

        builder.Services.AddScoped<IStrategyResolver, StrategyResolver>();
        builder.Services.AddScoped<IViewmodelFactory, ViewModelFactory>();
    }

    private IEnumerable<Type> DiscoverInternalStrategies()
    {
        return Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.GetInterfaces().Any(DetectMappingStrategyInterface));
    }

    private void RegisterStrategies(IUmbracoBuilder builder, IEnumerable<Type> strategies)
    {
        foreach (var strategy in strategies)
        {
            var interfaceDescriptor = strategy.GetInterfaces()
                .FirstOrDefault(DetectMappingStrategyInterface);
            
            if (interfaceDescriptor == null) continue;
            
            builder.Services.AddScoped(interfaceDescriptor, strategy);
        }
    }
    
    private Func<Type, bool> DetectMappingStrategyInterface =>
        _interface => _interface.IsGenericType && _interface.GetGenericTypeDefinition() == typeof(IMappingStrategy<,>);
}