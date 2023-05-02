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
            // Arrange
            int a = 5;
            int b = 7;

            // Act
            int result = a + b;

            // Assert
            Assert.NotEqual(12, result);
        }
    }
}