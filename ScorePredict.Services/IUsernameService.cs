﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScorePredict.Services
{
    public interface IGetUsernameService
    {
        Task<string> GetUsernameAsync(string s);
    }
}
