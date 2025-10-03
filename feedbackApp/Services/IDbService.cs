using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using feedbackApp.Models;

namespace feedbackApp.Services
{
    public interface IDbService<T>
    {
        public Task<HttpStatusCode> SaveAsync(T entity);
        
        public Task<IEnumerable<T>> FetchAsync(string id);


    }
}