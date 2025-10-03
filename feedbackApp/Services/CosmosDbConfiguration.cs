using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace feedbackApp.Services
{
    public class CosmosDbConfiguration
    {
        public string Endpoint { get; set; }
        public string Database { get; set; }

        public string Container { get; set; }
        public string AdminContainer { get; set; }


    }
}