using AuthHangfireApi.Authentication;
using AuthHangfireApi.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthHangfireApi.Controllers
{
    [ApiController]
    [Route("api/admin/jobs")]
    [Authorize(Roles = Roles.Admin)]
    public class JobAdminController : ControllerBase
    {
        private readonly SampleJobs _jobs;

        public JobAdminController(SampleJobs jobs)
        {
            _jobs = jobs;
        }

        // =========================
        // 🔹 EMAIL – ONE TIME
        // =========================
        [HttpPost("emails/send")]
        public IActionResult SendEmail(string email, string message)
        {
            BackgroundJob.Enqueue(() => _jobs.SendEmail(email, message));
            return Ok("📬 Email queued");
        }

        // =========================
        // 🔹 EMAIL – RECURRING (10 sec)
        // =========================
        [HttpPost("emails/recurring_10sec/start")]
        public IActionResult StartRecurringEmail(string email, string message)
        {
            RecurringJob.AddOrUpdate<EmailCronJob>(
                "email-recurring-10sec",
                j => j.SendEmailEvery10Seconds(email, message),
                Cron.Minutely // Hangfire minimum
            );

            return Ok("📧 Recurring email started");
        }

        [HttpDelete("emails/recurring_10sec/stop")]
        public IActionResult StopRecurringEmail()
        {
            JobControl.EmailJobEnabled = false;
            RecurringJob.RemoveIfExists("email-recurring-10sec");

            return Ok("🛑 Recurring email stopped");
        }

        [HttpPost("email/pause_10sec_email")]
        public IActionResult PauseEmailJob()
        {
            JobControl.Email10SecPaused = true;
            return Ok("⏸ Email job paused");
        }

        [HttpPost("email/resume_10sec_email")]
        public IActionResult ResumeEmailJob()
        {
            JobControl.Email10SecPaused = false;
            return Ok("▶ Email job resumed");
        }


        // =========================
        // 🔹 CLEANUP JOB
        // =========================
        //[HttpPost("cleanup/schedule")]
        //public IActionResult ScheduleCleanup()
        //{
        //    RecurringJob.AddOrUpdate(
        //        "cleanup-logs",
        //        () => _jobs.CleanLogs(),
        //        "0 1 * * *"
        //    );

        //    return Ok("🧹 Cleanup scheduled");
        //}

        //[HttpDelete("cleanup/remove")]
        //public IActionResult RemoveCleanup()
        //{
        //    RecurringJob.RemoveIfExists("cleanup-logs");
        //    return Ok("🗑 Cleanup removed");
        //}

        // =========================
        // 🔹 TEST JOB
        // =========================
        //[HttpPost("test/minute/start")]
        //public IActionResult StartMinuteTest()
        //{
        //    RecurringJob.AddOrUpdate<TestCronJobs>(
        //        "test-minute-job",
        //        j => j.RunEveryMinute(),
        //        Cron.Minutely
        //    );

        //    return Ok("▶ Test job started");
        //}

        //[HttpDelete("test/minute/stop")]
        //public IActionResult StopMinuteTest()
        //{
        //    RecurringJob.RemoveIfExists("test-minute-job");
        //    return Ok("⏹ Test job stopped");
        //}
    }

}