using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Xunit.Abstractions;
using Serilog.Sinks.Elasticsearch;

namespace XUnitTestProject
{
    public class DbCollectionTests : IDisposable 
    {
        public DbCollectionTests(ITestOutputHelper output) 
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
        public void AddCollectionTest()
        {
            Log.Information("Carrying out Add Collection Test");
            var mockSet = new Mock<DbSet<TblCollection>>();

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(m => m.TblCollections).Returns(mockSet.Object);

            var service = new CollectionsServices(mockContext.Object);
            service.AddCollectionToDB(new TblCollection
            {
                FldCollectionId = 1,
                FldUserId = 1,
                FldCollectionName = "TestCollectionName",
                FldCollectionDescription = "Description of the test collection",
                FldCollectionThumbnail = "TestCollection.png",
                FldIsPrivate = false
            });

            try
            {
                mockSet.Verify(m => m.Add(It.IsAny<TblCollection>()), Times.Once);
                mockContext.Verify(m => m.SaveChanges(), Times.Once());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void UpdateCollectionTest()
        {
            Log.Information("Carrying out Update Collection Test");
            var OriginalCollection = new TblCollection
            {
                FldCollectionId = 1,
                FldUserId = 1,
                FldCollectionName = "TestCollectionName",
                FldCollectionDescription = "Description of the test collection",
                FldCollectionThumbnail = "TestCollection.png",
                FldIsPrivate = false
            };

            var UpdatedCollection = new TblCollection
            {
                FldCollectionId = 1,
                FldUserId = 1,
                FldCollectionName = "NewCollectionName",
                FldCollectionDescription = "New Collection Description",
                FldCollectionThumbnail = "UpdatedImage.png",
                FldIsPrivate = false
            };
            var mockSet = new Mock<DbSet<TblCollection>>();

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(m => m.TblCollections).Returns(mockSet.Object);

            var service = new CollectionsServices(mockContext.Object);
            service.AddCollectionToDB(OriginalCollection);
            service.UpdateCollection(OriginalCollection, UpdatedCollection);

            try
            {
                mockSet.Verify(m => m.Add(It.IsAny<TblCollection>()), Times.AtLeastOnce());
                mockContext.Verify(m => m.SaveChanges(), Times.AtLeastOnce());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void GetAllCollectionsOnUserIDTest()
        {
            Log.Information("Carrying out get all collections on UserID tests");
            var data = new List<TblCollection>
            {
                new TblCollection
                {
                    FldCollectionId = 1,
                    FldUserId = 1,
                    FldCollectionName = "TestCollectionName",
                    FldCollectionDescription = "Description of the test collection",
                    FldCollectionThumbnail = "TestCollection.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 2,
                    FldUserId = 1,
                    FldCollectionName = "TestCollectionName2",
                    FldCollectionDescription = "Description of the test collection2",
                    FldCollectionThumbnail = "TestCollection2.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 3,
                    FldUserId = 1,
                    FldCollectionName = "TestCollectionName3",
                    FldCollectionDescription = "Description of the test collection3",
                    FldCollectionThumbnail = "TestCollection3.png",
                    FldIsPrivate = true
                },
                new TblCollection
                {
                    FldCollectionId = 4,
                    FldUserId = 2,
                    FldCollectionName = "TestCollectionName4",
                    FldCollectionDescription = "Description of the test collection4",
                    FldCollectionThumbnail = "TestCollection4.png",
                    FldIsPrivate = false
                }

            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblCollection>>();
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblCollections).Returns(mockSet.Object);

            var service = new CollectionsServices(mockContext.Object);
            var QueriedCollections = service.GetAllCollectionsOnUserID(1);

            try
            {
                Assert.Equal(3, QueriedCollections.Count);
                Assert.Equal("TestCollectionName3", QueriedCollections[2].FldCollectionName);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void GetAllOfAUsersPublicCollectionsTest()
        {
            Log.Information("Carrying out get all of a user's public collections test");
            var data = new List<TblCollection>
            {
                new TblCollection
                {
                    FldCollectionId = 1,
                    FldUserId = 1,
                    FldCollectionName = "TestCollectionName",
                    FldCollectionDescription = "Description of the test collection",
                    FldCollectionThumbnail = "TestCollection.png",
                    FldIsPrivate = true
                },
                new TblCollection
                {
                    FldCollectionId = 2,
                    FldUserId = 1,
                    FldCollectionName = "TestCollectionName2",
                    FldCollectionDescription = "Description of the test collection2",
                    FldCollectionThumbnail = "TestCollection2.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 3,
                    FldUserId = 1,
                    FldCollectionName = "TestCollectionName3",
                    FldCollectionDescription = "Description of the test collection3",
                    FldCollectionThumbnail = "TestCollection3.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 4,
                    FldUserId = 2,
                    FldCollectionName = "TestCollectionName4",
                    FldCollectionDescription = "Description of the test collection4",
                    FldCollectionThumbnail = "TestCollection4.png",
                    FldIsPrivate = false
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblCollection>>();
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblCollections).Returns(mockSet.Object);

            var service = new CollectionsServices(mockContext.Object);
            var QueriedCollections = service.GetAllUsersPublicCollections(1);

            try
            {
                Assert.Equal(2, QueriedCollections.Count);
                Assert.Equal("TestCollectionName3", QueriedCollections[1].FldCollectionName);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void GetAllPublicCollectionsContainingKeywordTest()
        {
            Log.Information("Carrying out get all public collections on keyword test");
            var data = new List<TblCollection>
            {
                new TblCollection
                {
                    FldCollectionId = 1,
                    FldUserId = 1,
                    FldCollectionName = "Pokemons",
                    FldCollectionDescription = "Description of the test collection",
                    FldCollectionThumbnail = "TestCollection.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 2,
                    FldUserId = 1,
                    FldCollectionName = "Bottle caps",
                    FldCollectionDescription = "Description of the test collection2",
                    FldCollectionThumbnail = "TestCollection2.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 3,
                    FldUserId = 1,
                    FldCollectionName = "Football caps",
                    FldCollectionDescription = "Description of the test collection3",
                    FldCollectionThumbnail = "TestCollection3.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 4,
                    FldUserId = 2,
                    FldCollectionName = "caps",
                    FldCollectionDescription = "Description of the test collection4",
                    FldCollectionThumbnail = "TestCollection4.png",
                    FldIsPrivate = true
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblCollection>>();
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblCollections).Returns(mockSet.Object);

            var service = new CollectionsServices(mockContext.Object);
            var QueriedCollections = service.GetAllCollectionsContainingKeyword("caps");

            try
            {
                Assert.Equal(2, QueriedCollections.Count);
                Assert.Equal("Football caps", QueriedCollections[1].FldCollectionName);
                Assert.Equal(1, QueriedCollections[0].FldUserId);
                Assert.Equal(2, QueriedCollections[0].FldCollectionId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void GetCollectionOnCollectionIDTest()
        {
            Log.Information("Carrying out get Collection on CollectionID test");
            var data = new List<TblCollection>
            {
                new TblCollection
                {
                    FldCollectionId = 1,
                    FldUserId = 1,
                    FldCollectionName = "Pokemons",
                    FldCollectionDescription = "Description of the test collection",
                    FldCollectionThumbnail = "TestCollection.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 2,
                    FldUserId = 1,
                    FldCollectionName = "Bottle caps",
                    FldCollectionDescription = "Description of the test collection2",
                    FldCollectionThumbnail = "TestCollection2.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 3,
                    FldUserId = 1,
                    FldCollectionName = "Football caps",
                    FldCollectionDescription = "Description of the test collection3",
                    FldCollectionThumbnail = "TestCollection3.png",
                    FldIsPrivate = false
                },
                new TblCollection
                {
                    FldCollectionId = 4,
                    FldUserId = 2,
                    FldCollectionName = "caps",
                    FldCollectionDescription = "Description of the test collection4",
                    FldCollectionThumbnail = "TestCollection4.png",
                    FldIsPrivate = true
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblCollection>>();
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblCollection>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblCollections).Returns(mockSet.Object);

            var service = new CollectionsServices(mockContext.Object);
            var QueriedCollections = service.GetCollectionsOnCollectionID(4);

            try
            {
                Assert.Equal(1, QueriedCollections.Count);
                Assert.Equal("caps", QueriedCollections[0].FldCollectionName);
                Assert.Equal(2, QueriedCollections[0].FldUserId);
                Assert.Equal(4, QueriedCollections[0].FldCollectionId);
                Assert.Equal(true, QueriedCollections[0].FldIsPrivate);
                Assert.Equal("Description of the test collection4", QueriedCollections[0].FldCollectionDescription);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

    }  
}
