namespace Test
{
    public class TestClass
    {
        public int TestMethod() { return 1; }
    }

    public class Program
    {
        public void Main()
        {
            var instance = new TestClass();
            var value = instance.TestMethod();
        }
    }
}