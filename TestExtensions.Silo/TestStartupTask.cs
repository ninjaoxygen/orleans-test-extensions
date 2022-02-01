using Orleans.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestExtensions.StandaloneSilo
{
    public class TestStartupTask : IStartupTask
    {
        private readonly ITestService _testService;

        public TestStartupTask(ITestService testService)
        {
            _testService = testService;
        }

        public async Task Execute(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("TestExtensionsStartupTask.Execute >");

                await _testService.RunTest();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TestExtensionsStartupTask.Execute Exception: {ex}");
            }
            finally
            {
                Console.WriteLine("TestExtensionsStartupTask.Execute <");
            }
        }
    }
}
