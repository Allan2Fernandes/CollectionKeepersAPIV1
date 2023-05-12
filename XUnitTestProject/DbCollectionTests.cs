using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestProject
{
    public class DbCollectionTests
    {
        [Fact]
        public void AddCollectionTest()
        {
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

            mockSet.Verify(m => m.Add(It.IsAny<TblCollection>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void UpdatCollectionTest()
        {
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
            mockSet.Verify(m => m.Add(It.IsAny<TblCollection>()), Times.AtLeastOnce());
            mockContext.Verify(m => m.SaveChanges(), Times.AtLeastOnce());
        }

        [Fact]
        public void GetAllCollectionsOnUserIDTest()
        {
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
            var QueriedUsers = service.GetAllCollectionsOnUserID(1);

            Assert.Equal(3, QueriedUsers.Count);
            Assert.Equal("TestCollectionName3", QueriedUsers[2].FldCollectionName);
        }

        [Fact]
        public void GetAllOfAUsersPublicCollectionsTest()
        {
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
            var QueriedUsers = service.GetAllUsersPublicCollections(1);

            Assert.Equal(2, QueriedUsers.Count);
            Assert.Equal("TestCollectionName3", QueriedUsers[1].FldCollectionName);
        }

        [Fact]
        public void GetAllPublicCollectionsContainingKeywordTest()
        {
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
            var QueriedUsers = service.GetAllCollectionsContainingKeyword("caps");

            Assert.Equal(2, QueriedUsers.Count);
            Assert.Equal("Football caps", QueriedUsers[1].FldCollectionName);
            Assert.Equal(1, QueriedUsers[0].FldUserId);
            Assert.Equal(2, QueriedUsers[0].FldCollectionId);
        }

        [Fact]
        public void GetCollectionOnCollectionIDTest()
        {
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
            var QueriedUsers = service.GetCollectionsOnCollectionID(4);

            Assert.Equal(1, QueriedUsers.Count);
            Assert.Equal("caps", QueriedUsers[0].FldCollectionName);
            Assert.Equal(2, QueriedUsers[0].FldUserId);
            Assert.Equal(4, QueriedUsers[0].FldCollectionId);
            Assert.Equal(true, QueriedUsers[0].FldIsPrivate);
            Assert.Equal("Description of the test collection4", QueriedUsers[0].FldCollectionDescription);
        }

    }  
}
