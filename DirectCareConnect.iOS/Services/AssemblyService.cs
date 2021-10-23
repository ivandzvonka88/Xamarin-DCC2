
using DirectCareConnect.AppPackageProject.Types;
using DirectCareConnect.Services;
using System.Reflection;

namespace DirectCareConnect.iOS.Services
{
    public class AssemblyService : IAssemblyService
    {
        public Assembly GetAppPackageAssembly()
        {
            return typeof(AppPackageProjectType).Assembly;
        }
    }
}
