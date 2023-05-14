using CollectionKeepersAPIV1.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionKeepersAPIV1.DataTransferObjects;

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

        [Fact]
        public void CheckEmptyListTest()
        {
            List<GetCollectionEntryOnIDDTO> List1 = new List<GetCollectionEntryOnIDDTO>();
            List<GetAllAttributeValuesForACollectionDTO> List2 = new List<GetAllAttributeValuesForACollectionDTO>
            {
                new GetAllAttributeValuesForACollectionDTO()
            };
            
            Assert.True(Functions.IsListEmpty(List1));
            Assert.False(Functions.IsListEmpty(List2));
        }
        
    }
}
