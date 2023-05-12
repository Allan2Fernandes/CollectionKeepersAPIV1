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
using Serilog;

namespace XUnitTestProject
{
    public class DbUserTests
    {  

        [Fact]
        public void AddUsersTest()
        {
            var mockSet = new Mock<DbSet<TblUser>>();

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(m => m.TblUsers).Returns(mockSet.Object);

            var service = new UserServices(mockContext.Object);
            service.AddUserToDB(new TblUser
            {
                /*
                    public int FldUserId { get; set; }
                    public string? FldUsername { get; set; }
                    public string? FldPassword { get; set; }
                    public string? FldEmail { get; set; }
                */
                FldUserId= 1,
                FldUsername = "Test1",
                FldPassword = "TestPassword",
                FldEmail = "Test@Test.com"
            });

            mockSet.Verify(m => m.Add(It.IsAny<TblUser>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void ModifyUserTest() //Look into this one later
        {
            var Originaluser = new TblUser
            {

                FldUserId = 1,
                FldUsername = "Test1",
                FldPassword = "TestPassword",
                FldEmail = "Test@Test.com"
            };

            var NewUser = new TblUser
            {

                FldUserId = 1,
                FldUsername = "Test2",
                FldPassword = "TestPassword2",
                FldEmail = "Test2@Test.com"
            };
            var mockSet = new Mock<DbSet<TblUser>>();

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(m => m.TblUsers).Returns(mockSet.Object);

            var service = new UserServices(mockContext.Object);
            service.AddUserToDB(Originaluser);
            service.UpdateUserDetails(Originaluser, NewUser);
            mockSet.Verify(m => m.Add(It.IsAny<TblUser>()), Times.AtLeastOnce());
            mockContext.Verify(m => m.SaveChanges(), Times.AtLeastOnce());
        }

        [Fact]
        public void GetAllUsersTest()
        {
            var data = new List<TblUser>
            {
                new TblUser
                {
                    FldUserId= 1,
                    FldUsername = "Test1",
                    FldPassword = "TestPassword1",
                    FldEmail = "Test@Test.com"
                },
                new TblUser
                {
                    FldUserId= 2,
                    FldUsername = "Test2",
                    FldPassword = "TestPassword2",
                    FldEmail = "Test2@Test.com"
                },
                new TblUser
                {
                    FldUserId= 3,
                    FldUsername = "Test3",
                    FldPassword = "TestPassword3",
                    FldEmail = "Test3@Test.com"
                },

            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblUser>>();
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblUsers).Returns(mockSet.Object);

            var service = new UserServices(mockContext.Object);
            var QueriedUsers = service.GetAllUsers();

            Assert.Equal(3, QueriedUsers.Count);
            Assert.Equal("Test3", QueriedUsers[2].FldUsername);
            Assert.Equal("TestPassword2", QueriedUsers[1].FldPassword);
            Assert.Equal("Test@Test.com", QueriedUsers[0].FldEmail);
        }

        [Fact]
        public void GetUsersOnUserIDTest()
        {
            var data = new List<TblUser>
            {
                new TblUser
                {
                    FldUserId= 1,
                    FldUsername = "Test1",
                    FldPassword = "TestPassword1",
                    FldEmail = "Test@Test.com"
                },
                new TblUser
                {
                    FldUserId= 2,
                    FldUsername = "Test2",
                    FldPassword = "TestPassword2",
                    FldEmail = "Test2@Test.com"
                },
                new TblUser
                {
                    FldUserId= 3,
                    FldUsername = "Test3",
                    FldPassword = "TestPassword3",
                    FldEmail = "Test3@Test.com"
                },

            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblUser>>();
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblUsers).Returns(mockSet.Object);

            var service = new UserServices(mockContext.Object);
            var QueriedUsers = service.GetUsersOnID(3);

            Assert.Equal(1, QueriedUsers.Count);
            Assert.Equal("Test3", QueriedUsers[0].FldUsername);
            Assert.Equal("TestPassword3", QueriedUsers[0].FldPassword);
            Assert.Equal("Test3@Test.com", QueriedUsers[0].FldEmail);
        }

        [Fact]
        public void GetUsersOnUserEmailTest()
        {
            var data = new List<TblUser>
            {
                new TblUser
                {
                    FldUserId= 1,
                    FldUsername = "Test1",
                    FldPassword = "TestPassword1",
                    FldEmail = "Test@Test.com"
                },
                new TblUser
                {
                    FldUserId= 2,
                    FldUsername = "Test2",
                    FldPassword = "TestPassword2",
                    FldEmail = "Test2@Test.com"
                },
                new TblUser
                {
                    FldUserId= 3,
                    FldUsername = "Test3",
                    FldPassword = "TestPassword3",
                    FldEmail = "Test3@Test.com"
                },

            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblUser>>();
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblUsers).Returns(mockSet.Object);

            var service = new UserServices(mockContext.Object);
            var QueriedUsers = service.GetUsersOnEmail("Test2@Test.com");

            Assert.Equal(1, QueriedUsers.Count);
            Assert.Equal("Test2", QueriedUsers[0].FldUsername);
            Assert.Equal("TestPassword2", QueriedUsers[0].FldPassword);
            Assert.Equal("Test2@Test.com", QueriedUsers[0].FldEmail);
        }
    }
}