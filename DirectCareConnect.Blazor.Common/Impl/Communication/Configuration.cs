using DirectCareConnect.Common.Interfaces.Communication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DirectCareConnect.Common.Impl.Communication
{
    public class Configuration : IConfiguration
    {
        [JsonConstructor]
        public Configuration()
        {
            
        }

        public void Init()
        {
            var config = GetConfiguration();
            this.ApiBaseAddress = config.ApiBaseAddress;
            this.MockedRest = config.MockedRest;
            this.SendBirdAppId = config.SendBirdAppId;
        }

        public string ApiBaseAddress { get; set; }
        public bool MockedRest { get; set; }
        public string SendBirdAppId { get; set; }

        private static IConfiguration GetConfiguration()
        {
            var embeddedResourceStream = Assembly.GetAssembly(typeof(IConfiguration)).GetManifestResourceStream("DirectCareConnect.Common.Resources.config.json");
            if (embeddedResourceStream == null)
                return null;

            using (var streamReader = new StreamReader(embeddedResourceStream))
            {
                var jsonString = streamReader.ReadToEnd();
                var configuration = JsonConvert.DeserializeObject<Configuration>(jsonString);

                if (configuration == null)
                    return null;

                return configuration as IConfiguration;
            }
        }
    }
}
