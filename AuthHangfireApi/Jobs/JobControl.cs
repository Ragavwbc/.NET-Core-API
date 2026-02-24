namespace AuthHangfireApi.Jobs
{
    public static class JobControl
    {
        public static volatile bool EmailJobEnabled = true;

        public static volatile bool Email10SecPaused = false;
    }
}
