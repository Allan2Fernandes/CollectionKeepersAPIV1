using System.Reflection.Metadata.Ecma335;
using CollectionKeepersAPIV1.Functions;

namespace XUnitTestProject
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            int a = 5;
            int b = 7;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(12, result);
        }

        [Fact]
        public void Test2()
        {
            int a = 0;
            int b = Functions.TestFunction();
            Assert.Equal(a, b);
        }
    }
}