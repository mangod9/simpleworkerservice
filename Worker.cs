using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        TimeSpan _emailTimeUTC;
        int _updaterIntervalHours;
        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            _emailTimeUTC = new TimeSpan(15,0,0);
            _updaterIntervalHours = 4;

        }

        const int oneDayMilliseconds = 60 * 60 * 1000 * 24; // milliseconds to six hours

        private static UpdaterService _updaterService;
        private static Timer _EmailTimer;

        private static Timer _UpdaterTimer;

        private static EmailerService _emailerService;

        private static object _lockerObject = new object();
        private void EmailTimerEvent(object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            autoEvent.Set();

            lock (_lockerObject)
            {
                _logger.LogInformation("Email Service running at: {time}", DateTimeOffset.Now);

                _emailerService.Run().Wait();
            }
        }

        private void CreateEmailTimer()
        {

            var currentTime = DateTime.UtcNow.TimeOfDay;

            var timeDifference = _emailTimeUTC - currentTime;


            timeDifference = new TimeSpan(0, 0, 10);

            var autoEvent = new AutoResetEvent(false);
            _EmailTimer = new Timer(EmailTimerEvent, autoEvent, (int)timeDifference.TotalMilliseconds, 10*1000);

        }

        private void UpdaterTimerEvent(object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            autoEvent.Set();


            lock (_lockerObject)
            {
                _logger.LogInformation("Update Service running at: {time}", DateTimeOffset.Now);

                _updaterService.Run().Wait();
            }

        }


        private void CreateUpdaterTimer()
        {
            var autoEvent = new AutoResetEvent(false);
            _UpdaterTimer = new Timer(UpdaterTimerEvent, autoEvent, 0, _updaterIntervalHours * 60 * 60 * 1000);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _updaterService = _scopeFactory.CreateScope().ServiceProvider.GetService<UpdaterService>();

            _emailerService = _scopeFactory.CreateScope().ServiceProvider.GetService<EmailerService>();

            CreateEmailTimer();
            CreateUpdaterTimer();

            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(1000, stoppingToken);

            }
            _EmailTimer.Dispose();
            _UpdaterTimer.Dispose();
        }
    }
}
