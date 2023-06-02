using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Castle.Core.Resource;
using CollectionKeepersAPIV1.Controllers;
using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.Functions;
using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XUnitTestProject
{
    public class DbUserTests : IDisposable
    {
        public DbUserTests(ITestOutputHelper output) 
        {
            Log.Logger = new LoggerConfiguration() //new
            .WriteTo.Console()
            .WriteTo.File("./Serilogs/logs.txt")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://10.176.88.60:9200"))
            {
                AutoRegisterTemplate = true,
                BatchPostingLimit = 1,
            })
            .CreateLogger();
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }

        [Fact]
        public void AddUsersTest()
        {
            Log.Information(" Carrying out Add Users test");
            var mockSet = new Mock<DbSet<TblUser>>();

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(m => m.TblUsers).Returns(mockSet.Object);

            var service = new UserServices(mockContext.Object);
            service.AddUserToDB(new TblUser
            {
                FldUserId= 1,
                FldUsername = "Test1",
                FldPassword = "TestPassword",
                FldEmail = "Test@Test.com"
            });
            try
            {
                mockSet.Verify(m => m.Add(It.IsAny<TblUser>()), Times.Once);
                mockContext.Verify(m => m.SaveChanges(), Times.Once());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
           
        }
        [Fact]
        public void ModifyUserTest() //Look into this one later
        {
            Log.Information(" Carrying out Modify user test");
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

            try
            {
                mockSet.Verify(m => m.Add(It.IsAny<TblUser>()), Times.AtLeastOnce());
                mockContext.Verify(m => m.SaveChanges(), Times.AtLeastOnce());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void GetAllUsersTest()
        {
            Log.Information(" Carrying out get all users test");
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
            try
            {
                Assert.Equal(3, QueriedUsers.Count);
                Assert.Equal("Test3", QueriedUsers[2].FldUsername);
                Assert.Equal("TestPassword2", QueriedUsers[1].FldPassword);
                Assert.Equal("Test@Test.com", QueriedUsers[0].FldEmail);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void GetUsersOnUserIDTest()
        {
            Log.Information(" Carrying out get user on userID test");
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

            try
            {
                Assert.Equal(1, QueriedUsers.Count);
                Assert.Equal("Test3", QueriedUsers[0].FldUsername);
                Assert.Equal("TestPassword3", QueriedUsers[0].FldPassword);
                Assert.Equal("Test3@Test.com", QueriedUsers[0].FldEmail);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void GetUsersOnUserEmailTest()
        {
            Log.Information(" Carrying out get users on emailID test");
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

            try
            {
                Assert.Equal(1, QueriedUsers.Count);
                Assert.Equal("Test2", QueriedUsers[0].FldUsername);
                Assert.Equal("TestPassword2", QueriedUsers[0].FldPassword);
                Assert.Equal("Test2@Test.com", QueriedUsers[0].FldEmail);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

       
    }
}