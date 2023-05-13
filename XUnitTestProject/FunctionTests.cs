using CollectionKeepersAPIV1.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestProject
{
    public class FunctionTests
    {
        [Fact]
        public void EmailValidationTest()
        {
            string ExpectedValidEmail = "User@mail.com";
            string ExpectedInvalidEmail1 = "User@mailcom";
            string ExpectedInvalidEmail2 = "UserMail.com";

            Assert.Equal(true, Functions.CheckIfValidEmail(ExpectedValidEmail));
            Assert.Equal(false, Functions.CheckIfValidEmail(ExpectedInvalidEmail1));
            Assert.Equal(false, Functions.CheckIfValidEmail(ExpectedInvalidEmail2));
        }
    }
}
