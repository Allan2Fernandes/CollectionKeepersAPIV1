using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Castle.Core.Resource;
using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.Functions;
using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;

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
            Assert.Equal(a, Functions.TestFunction0());
            Assert.Equal(a, Functions.TestFunction1());
            Assert.Equal(a, Functions.TestFunction2());
            Assert.Equal(a, Functions.TestFunction3());
            Assert.Equal(a, Functions.TestFunction4());
            Assert.Equal(a, Functions.TestFunction5());
            Assert.Equal(a, Functions.TestFunction6());
            Assert.Equal(a, Functions.TestFunction7());
            Assert.Equal(a, Functions.TestFunction8());
            Assert.Equal(a, Functions.TestFunction9());
            Assert.Equal(a, Functions.TestFunction10());
            Assert.Equal(a, Functions.TestFunction11());
            Assert.Equal(a, Functions.TestFunction12());
            Assert.Equal(a, Functions.TestFunction13());
            Assert.Equal(a, Functions.TestFunction14());
            Assert.Equal(a, Functions.TestFunction15());
            Assert.Equal(a, Functions.TestFunction16());
            Assert.Equal(a, Functions.TestFunction17());
            Assert.Equal(a, Functions.TestFunction18());
            Assert.Equal(a, Functions.TestFunction19());
            Assert.Equal(a, Functions.TestFunction20());
            Assert.Equal(a, Functions.TestFunction21());
            Assert.Equal(a, Functions.TestFunction22());
            Assert.Equal(a, Functions.TestFunction23());
            Assert.Equal(a, Functions.TestFunction24());
            Assert.Equal(a, Functions.TestFunction25());
            Assert.Equal(a, Functions.TestFunction26());
            Assert.Equal(a, Functions.TestFunction27());
            Assert.Equal(a, Functions.TestFunction28());
            Assert.Equal(a, Functions.TestFunction29());
            Assert.Equal(a, Functions.TestFunction30());
            Assert.Equal(a, Functions.TestFunction31());
            Assert.Equal(a, Functions.TestFunction32());
            Assert.Equal(a, Functions.TestFunction33());
            Assert.Equal(a, Functions.TestFunction34());
            Assert.Equal(a, Functions.TestFunction35());
            Assert.Equal(a, Functions.TestFunction36());
            Assert.Equal(a, Functions.TestFunction37());
            Assert.Equal(a, Functions.TestFunction38());
            Assert.Equal(a, Functions.TestFunction39());
            Assert.Equal(a, Functions.TestFunction40());
            Assert.Equal(a, Functions.TestFunction41());
            Assert.Equal(a, Functions.TestFunction42());
            Assert.Equal(a, Functions.TestFunction43());
            Assert.Equal(a, Functions.TestFunction44());
            Assert.Equal(a, Functions.TestFunction45());
            Assert.Equal(a, Functions.TestFunction46());
            Assert.Equal(a, Functions.TestFunction47());
            Assert.Equal(a, Functions.TestFunction48());
            Assert.Equal(a, Functions.TestFunction49());
        }

        
        [Fact]
        public void dbTest1()
        {
            var MockSet = new Mock<DbSet<TblUser>>();
            var MockContext = new Mock<CollectionsDbContext>();

            var data = new List<TblUser>
            {
                new TblUser
                {
                    FldUsername = "Test",
                    FldPassword = "123456",
                    FldEmail = "Test@mail.com",
                }
            }.AsQueryable();

            MockContext.Setup(m => m.TblUsers).Returns(MockSet.Object);           

            MockSet.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(data.Provider);
            MockSet.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(data.Expression);
            MockSet.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(data.ElementType);
            MockSet.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var service = new UserControllerLogic(MockContext.Object);
            List<TblUser> AllUsers = service.GetAllUsers();
            Assert.Equal(1, AllUsers.Count);
            Assert.Equal("123456", AllUsers[0].FldPassword);         

            Assert.Equal(6, 6);
        }



    }
}