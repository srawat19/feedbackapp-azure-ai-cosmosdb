using System;
using System.Collections.Generic;
using feedbackApp.Models;

namespace feedbackApp.Services
{
    public interface IAdminDbService
    {
        public Task<AdminEvents> FetchAsyncByEventShortName(string eventShortName);
        public Task<string> FetchAsyncByEventId(string eventId);
    }
}