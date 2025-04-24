using Microsoft.Extensions.DependencyInjection;
using Quantum.Configurator;
using Quantum.Reng;

namespace Quantum.Logging.Configure
{
    public static class ConfigQuantumRengExtensions
    {
        public static ConfigQuantumRengBuilder ConfigQuantumReng(this QuantumServiceCollection collection)
        {
            return new ConfigQuantumRengBuilder(collection);
        }
    }

    public class ConfigQuantumRengBuilder
    {
        private readonly QuantumServiceCollection _quantumServiceCollection;
        RendConfig _config;

        public ConfigQuantumRengBuilder(QuantumServiceCollection collection)
        {
            _quantumServiceCollection = collection;
        }


        public ConfigQuantumRengBuilder AddConfigurator(RendConfig config)
        {
            _config = config;

            _quantumServiceCollection.Collection              .AddSingleton(config);

            return this;
        }

        public ConfigQuantumRengBuilder RegisterRengAs(ServiceLifetime serviceLifetime)
        {
            _quantumServiceCollection.Collection
                .Add(new ServiceDescriptor(typeof(IRengApi), typeof(Reng.RengApi), serviceLifetime));

            return this;
        }
        public QuantumServiceCollection and()
        {
            return _quantumServiceCollection;
        }

    }
}
