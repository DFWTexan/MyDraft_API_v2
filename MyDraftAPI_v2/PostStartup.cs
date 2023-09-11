namespace MyDraftAPI_v2
{
    public class PostStartup : IHostedService
    {
        private readonly ILogger<PostStartup> _logger;
        private readonly DraftEngine_v2 _draftEngine;

        public PostStartup(ILogger<PostStartup> logger, DraftEngine_v2 draftEngine)
        {
            _logger = logger;
            _draftEngine = draftEngine;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PostStartup: StartAsync");

            //var vInput = new Database.Model.UserLeague
            //{
            //    ID = 1
            //};

            await _draftEngine.InitializeLeagueData_v2();

            //return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PostStartup: StopAsync");

            return Task.CompletedTask;
        }
    }   
}
