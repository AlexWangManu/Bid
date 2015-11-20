using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SHbidTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TimeCompareTest()
        {
            DateTime now = DateTime.Now;
            DateTime expectedTime = DateTime.Parse("11:29:50");
            bool flag = now > expectedTime ? true : false;
            Assert.AreEqual(flag, true);
        }
    }
}
