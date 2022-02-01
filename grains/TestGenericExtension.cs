using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Providers;
using Orleans.Runtime;

namespace TestExtensions
{
    // this one fails, it's generic
    //[TypeCodeOverride(unchecked((int)0xFFC0E296))]
    public interface ITestGenericExtension<T> : IGrainExtension
    {
        [Transaction(TransactionOption.Suppress)]
        Task PrintGeneric(string message, T value);
    }

    public class TestGenericExtension<T> : ITestGenericExtension<T>
    {
        readonly ILogger _logger;
        readonly IGrainActivationContext _context;

        public TestGenericExtension(ILogger logger, IGrainActivationContext context)
        {
            _logger = logger;
            _context = context;

            _logger.LogInformation($"{nameof(TestGenericExtension<T>)} Constructor PrimaryKeyString: {context.GrainInstance.GetPrimaryKeyString()} where T is {typeof(T).FullName}");
        }

        public Task PrintGeneric(string message, T value)
        {
            _logger.LogInformation($">>>>> {nameof(TestGenericExtension<T>)}.{nameof(PrintGeneric)} message = {message}, value = {value} where T is {typeof(T).FullName}");

            return Task.CompletedTask;
        }
    }
}
