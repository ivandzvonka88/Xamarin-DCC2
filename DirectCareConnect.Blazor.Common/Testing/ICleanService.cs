using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Testing
{
    public interface ICleanService
    {
        Task<bool> Exists();
    }
}
