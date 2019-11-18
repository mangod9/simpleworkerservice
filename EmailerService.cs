
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService
{
    public class EmailerService
    {
        ILogger _logger;
        public EmailerService(ILogger<EmailerService> logger)
        {
            _logger = logger;
        }


        public async Task<bool> Run()
        {
            try
            {
                _logger.LogInformation("Running Email Service");
            }

            catch (Exception ex)
            {
                _logger.LogCritical(ex, "error");
            }


            return false;
        }


    }
}