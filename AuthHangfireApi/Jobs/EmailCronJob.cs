using Hangfire;

namespace AuthHangfireApi.Jobs
{
    public class EmailCronJob
    {
        private readonly SampleJobs _jobs;
        private readonly ILogger<EmailCronJob> _logger;

        public EmailCronJob(SampleJobs jobs, ILogger<EmailCronJob> logger)
        {
            _jobs = jobs;
            _logger = logger;
        }

        [DisableConcurrentExecution(60)]
        public async Task SendEmailEvery10Seconds(string to, string message)
        {
            _logger.LogInformation("▶ 10-sec email batch started");
            JobControl.EmailJobEnabled = true;

            for (int i = 0; i < 6; i++) // 6 × 10s = 1 minute
            {
                if (!JobControl.EmailJobEnabled)
                {
                    _logger.LogInformation("🛑 Email job stopped mid-run");
                    break;
                }

                if (JobControl.Email10SecPaused)
                {
                    _logger.LogInformation("🛑 Email job pass");
                    break;
                }

                _jobs.SendEmail(to, $"{message} - {DateTime.Now}");

                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            _logger.LogInformation("⏹ 10-sec email batch finished");
        }
    }

}
