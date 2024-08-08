namespace app_tests
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.IsTrue(2 + 2 == 4);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Assert.IsTrue(5 > 10);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Assert.IsNotNull(new object());
        }

        [TestMethod]
        public void TestMethod5()
        {
            string str = "hello";
            Assert.AreEqual("hello", str);
        }

        [TestMethod]
        public void TestMethod6()
        {
            int[] numbers = { 1, 2, 3 };
            CollectionAssert.Contains(numbers, 2);
        }

        [TestMethod]
        public void TestMethod7()
        {
            int x = 10;
            Assert.AreEqual(10, x);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Assert.IsTrue("Test".StartsWith("T"));
        }

        [TestMethod]
        public void TestMethod9()
        {
            int a = 5, b = 5;
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void TestMethod10()
        {
            double pi = 3.14;
            Assert.AreEqual(3.14, pi, 0.001);
        }
    }
}