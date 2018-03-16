using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    public class obj
    {
        public int a;
        public int b;
        public obj(int i, int j)
        {
            a = i;
            b = j;
        }
    }
    public struct obj2
    {
        public int a;
        public int b;
        public obj2(int i, int j)
        {
            a = i;
            b = j;
        }
    }

    [TestClass]
    public class UnitTest1
    {
        obj a = new obj(10, 10);
        obj2 b = new obj2(10, 10);
        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine(a.a);
            Console.WriteLine(a.b);
        }
        [TestMethod]
        public void TestMethod2()
        {
            Console.WriteLine(b.a);
            Console.WriteLine(b.b);
        }
    }
}
