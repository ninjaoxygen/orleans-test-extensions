using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace TestExtensions
{
    // this one always works, it's not generic
    public interface ITestSpecificExtension : IGrainExtension
    {
        [Transaction(TransactionOption.Suppress)]
        Task PrintSpecific(string message);
    }

    public class TestSpecificExtension : ITestSpecificExtension
    {
        readonly ILogger _logger;
        readonly IGrainActivationContext _context;

        public TestSpecificExtension(ILogger logger, IGrainActivationContext context)
        {
            _logger = logger;
            _context = context;

            _logger.LogInformation($"{nameof(TestSpecificExtension)} Constructor PrimaryKeyString: {context.GrainInstance.GetPrimaryKeyString()}");
        }

        public Task PrintSpecific(string message)
        {
            _logger.LogInformation($">>>>> {nameof(TestSpecificExtension)}.{nameof(PrintSpecific)} message = {message}");

            return Task.CompletedTask;
        }
    }
}
