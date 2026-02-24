using Microsoft.Extensions.Logging;

namespace AuthHangfireApi.Jobs
{
    public class TestCronJobs
    {
        private readonly SampleJobs _jobs;
        private readonly ILogger<TestCronJobs> _logger;

        public TestCronJobs(SampleJobs jobs, ILogger<TestCronJobs> logger)
        {
            _jobs = jobs;
            _logger = logger;
        }

        public void RunEveryMinute()
        {
            _jobs.SendEmail(
                "ragav@wbcsoftwarelab.com",
                $"Hello! {DateTime.Now}"
            );

            _logger.LogInformation("✅ Cron executed at {time}", DateTime.Now);
        }
    }

}
