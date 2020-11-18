using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private static byte[] ParseIntToByteArray(uint value)
        {
            var result = new byte[4];

            var counter = 3;

            while (value > 0)
            {
                result[counter] = (byte)value;
                value >>= 8;
                counter--;
            }

            return result;
        }

        [TestMethod]

        public void TestMethod1()
        {
            int counter;

            byte[] array;

            for (uint i = 0; i < int.MaxValue; i++)
            {
                array = ParseIntToByteArray(i);
                counter = 0;
                for (var j = 0; j < array.Length; j++)
                {
                    counter += array[j] << (8 * (3 - j));
                }
                Assert.IsTrue(i == counter,$"{i}/{counter}");
            }
        }



    }
}
