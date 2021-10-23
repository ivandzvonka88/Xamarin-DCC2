using DirectCareConnect.Common.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Services
{
    public class CleanService : ICleanService
    {
        async public Task<bool> Exists()
        {
            Console.Write("TESTING VICTOR");
            return await Task.FromResult(true);
        }
    }
}
