using Microsoft.Extensions.Logging;
using Orleans;
using System.Threading.Tasks;

namespace TestExtensions
{
    public interface ITestService
    {
        Task RunTest();
    }

    public class TestService : ITestService
    {
        readonly ILogger _logger;
        readonly IGrainFactory _grainFactory;

        public TestService(ILogger<ITestService> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            _grainFactory = grainFactory;

            _logger.LogInformation($"{nameof(TestService)} Constructor");
        }

        public async Task RunTest()
        {
            string key = "foo";

            var _grain = _grainFactory.GetGrain<IExampleGrain>(key);

            var extensionSpecific = _grain.AsReference<ITestSpecificExtension>();
            // this one always works, which shows that BindExtension<> is working
            await extensionSpecific.PrintSpecific("test specific");

            var extensionString = _grain.AsReference<ITestGenericExtension<string>>();
            // without the Orleans patches this one fails, which shows that BindExtension<> with generics is not working
            await extensionString.PrintGeneric("XOXOX1 PrintGeneric with string", "bar");

            var extensionInt = _grain.AsReference<ITestGenericExtension<int>>();
            await extensionInt.PrintGeneric("XOXOX2 PrintGeneric with int", 321);

            // this should fail, we have not called BindExtension<> with this type
            var extensionBool = _grain.AsReference<ITestGenericExtension<bool>>();
            await extensionBool.PrintGeneric("XOXOX3 PrintGeneric with bool", true);
        }
    }
}
