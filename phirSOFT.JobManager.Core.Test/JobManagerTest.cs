using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace phirSOFT.JobManager.Core.Test
{
    [TestFixture]
    class JobManagerTest
    {

        [Test]
        public void TestOverAllStatus()
        {
            var manager = new JobManager();
            
            Assert.AreEqual(false, manager.CanDisplayOverallProgress);
            Assert.AreEqual(JobStatus.Succeded, manager.OverallStatus);
        }
    }
}
