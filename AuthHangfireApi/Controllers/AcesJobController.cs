using AuthHangfireApi.Authentication;
using AuthHangfireApi.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/aces")]
[Authorize(Roles = Roles.Admin)]
public class AcesJobController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJob;

    public AcesJobController(IBackgroundJobClient backgroundJob)
    {
        _backgroundJob = backgroundJob;
    }

    [HttpPost("cron/start")]
    public IActionResult StartAcesCron()
    {
        RecurringJob.AddOrUpdate<AcesFileJob>(
            "aces-app-cron",
            job => job.AddAppTag(),
            Cron.Minutely
        );

        return Ok("▶ ACES cron started (adds <App> every 1 minute)");
    }

    [HttpDelete("cron/stop")]
    public IActionResult StopAcesCron()
    {
        RecurringJob.RemoveIfExists("aces-app-cron");
        return Ok("⏹ ACES cron stopped");
    }

    [HttpGet("open/latest")]
    [Produces("application/xml")]
    public IActionResult OpenLatest()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "ACES_LIVE.xml");

        if (!System.IO.File.Exists(path))
            return NotFound("ACES file not found");

        return Content(System.IO.File.ReadAllText(path), "application/xml");
    }
}
