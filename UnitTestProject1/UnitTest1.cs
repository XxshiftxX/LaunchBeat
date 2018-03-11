using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    enum ee
    {
        a,b,c
    }
    [TestClass]
    public class UnitTest1
    {
        ee[] arr = new ee[3] { ee.a, ee.b, ee.c };
        Dictionary<ee, int> dic = new Dictionary<ee, int>() { { ee.a, 0 }, { ee.b, 1 }, { ee.c, 2 } };

        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine((int)ee.b);
        }
        [TestMethod]
        public void TestMethod2()
        {
            Console.WriteLine(dic[ee.b]);
        }
    }
}
