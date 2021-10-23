using DirectCareConnect.Common.Impl.Communication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DirectCareConnect.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ValidEmailLoggedIn()
        {
            DddezRestClient client = new DddezRestClient();
            var status = client.TryLogin("provider@thpl.com", "Abc123").GetAwaiter().GetResult();
            Assert.IsTrue(status.LoggedIn);
        }

        [TestMethod]
        public void NonValidEmail_NotLoggedIn()
        {
            DddezRestClient client = new DddezRestClient();
            var status = client.TryLogin("provider@thpl.com", "Abc1234").GetAwaiter().GetResult();
            Assert.IsFalse(status.LoggedIn);
        }

        [TestMethod]
        public void GetInitialData_Clients()
        {
            DddezRestClient client = new DddezRestClient();
            var status = client.TryLogin("provider@thpl.com", "Abc123").GetAwaiter().GetResult();
            if (status.LoggedIn)
            {
                var data = client.GetInitialData(status.Token).GetAwaiter().GetResult();
                Assert.IsTrue(data.Clients.Count > 0);
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void GetInitialData_Locations()
        {
            DddezRestClient client = new DddezRestClient();
            var status = client.TryLogin("provider@thpl.com", "Abc123").GetAwaiter().GetResult();
            if (status.LoggedIn)
            {
                var data = client.GetInitialData(status.Token).GetAwaiter().GetResult();
                Assert.IsTrue(data.Locations.Count > 0);
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void GetInitialData_Services()
        {
            DddezRestClient client = new DddezRestClient();
            var status = client.TryLogin("provider@thpl.com", "Abc123").GetAwaiter().GetResult();
            if (status.LoggedIn)
            {
                var data = client.GetInitialData(status.Token).GetAwaiter().GetResult();
                Assert.IsTrue(data.ClientServices.Count > 0);
                return;
            }
            Assert.Fail();
        }
    }
}
