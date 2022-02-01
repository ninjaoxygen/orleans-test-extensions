using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;

namespace TestExtensions
{
    public interface IExampleGrain : IGrainWithStringKey
    {
    }

    // an example grain where we do BindExtension calls inside the grain
    public class ExampleGrain : Grain, IExampleGrain
    {
        readonly ILogger _logger;
        readonly IGrainActivationContext _grainActivationContext;
        readonly IProviderRuntime _providerRuntime;

        public ExampleGrain(ILogger<IExampleGrain> logger, IGrainActivationContext grainActivationContext, IProviderRuntime providerRuntime)
        {
            _logger = logger;
            _grainActivationContext = grainActivationContext;
            _providerRuntime = providerRuntime;

            _logger.LogInformation($"{nameof(ExampleGrain)} Constructor");
        }

        public override Task OnActivateAsync()
        {
            // register our bindings now
            try
            {
                _logger.LogInformation($"{nameof(ExampleGrain)} OnActivateAsync ->");

                // register a test non-generic extension, this alwats works fine
                _logger.LogInformation($"{nameof(ExampleGrain)} calling DoBindingSpecific");
                DoBindingSpecific();
                _logger.LogInformation($"{nameof(ExampleGrain)} finishing DoBindingSpecific");

                // register first generic extension
                _logger.LogInformation($"{nameof(ExampleGrain)} calling DoBinding Generic string");
                DoBinding<string>();
                _logger.LogInformation($"{nameof(ExampleGrain)} finishing DoBinding Generic string");

                // register second generic extensions
                _logger.LogInformation($"{nameof(ExampleGrain)} calling DoBinding Generic int");
                DoBinding<int>();
                _logger.LogInformation($"{nameof(ExampleGrain)} finishing DoBinding Generic int");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{nameof(ExampleGrain)} OnActivateAsync failed Exception: {ex}");
            }
            finally
            {
                _logger.LogInformation($"{nameof(ExampleGrain)} OnActivateAsync <-");
            }

            return base.OnActivateAsync();
        }

        private Task DoBindingSpecific()
        {
            _logger.LogInformation($"{nameof(ExampleGrain)} {nameof(DoBindingSpecific)} ->");

            _providerRuntime.BindExtension<TestSpecificExtension, ITestSpecificExtension>(() => {
                _logger.LogInformation($"inside the {nameof(DoBindingSpecific)} BindExtension factory method");
                return new TestSpecificExtension(_logger, _grainActivationContext);
            });
            
            _logger.LogInformation($"{nameof(ExampleGrain)} {nameof(DoBindingSpecific)} <-");
            
            return Task.CompletedTask;
        }

        private Task DoBinding<T>()
        {
            try
            {
                _logger.LogInformation($"{nameof(ExampleGrain)}.DoBinding<T> ->");

                _providerRuntime.BindExtension<TestGenericExtension<T>, ITestGenericExtension<T>>(() =>
                {
                    _logger.LogInformation($"{nameof(ExampleGrain)} inside the {nameof(DoBinding)}<{typeof(T).FullName}> BindExtension factory method");
                    return new TestGenericExtension<T>(_logger, _grainActivationContext);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"DoBinding<T> (T's generic arguments are {string.Join(", ", typeof(T).GenericTypeArguments.Select(x => x.Name))}) exception: {ex}");
            }
            finally
            {
                _logger.LogInformation($"{nameof(ExampleGrain)}.DoBinding<T> <-");
            }

            return Task.CompletedTask;
        }
    }
}
