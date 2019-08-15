using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Test
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestM()
        {
            int x = 10;
            int y = 20;
            Assert.AreEqual(30, x + y);
        }
    }
}
