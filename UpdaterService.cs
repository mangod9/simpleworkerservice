using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService
{
    public class UpdaterService
    {

       
        IMapper _mapper;

        ILogger _logger;
        public UpdaterService( ILogger<UpdaterService> logger)
        {
         
            _logger = logger;
        }


        public async Task<bool> Run()
        {
         
            try
            {
                _logger.LogInformation("Updating Sales Information");
               
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex, "ERROR");
            }
            return true;
        }


     
    }
}
