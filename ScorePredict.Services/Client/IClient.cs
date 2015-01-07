﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScorePredict.Data;

namespace ScorePredict.Services.Client
{
    public interface IClient
    {
        Task<IDictionary<string, string>> PostApiAsync(string apiName, IDictionary<string, string> parameters = null);
        Task<IDictionary<string, string>> LoginFacebookAsync();
    }
}

