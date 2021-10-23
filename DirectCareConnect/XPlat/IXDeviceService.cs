using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.XPlat
{
    public interface IXDeviceService
    {
        string AppVersionName { get; }
        long AppVersionCode { get; }

        string VersionName { get; }
        int VersionCode { get; }

    }
}
