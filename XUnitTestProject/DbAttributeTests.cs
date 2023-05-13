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
    public class DbAttributeTests
    {
        [Fact]
        public void AddNewAttributeTest()
        {
            var mockSet = new Mock<DbSet<TblAttribute>>();

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(m => m.TblAttributes).Returns(mockSet.Object);

            var service = new AttributeServices(mockContext.Object);
            service.AddNewAttribute(new TblAttribute
            {
                FldAttributeId = 1,
                FldCollectionId = 1,
                FldAttributeName = "TestAttributeName"
            });

            mockSet.Verify(m => m.Add(It.IsAny<TblAttribute>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void UpdateAttributeTest()
        {
            var OriginalAttribute = new TblAttribute
            {
                FldAttributeId = 1,
                FldCollectionId = 1,
                FldAttributeName = "TestAttributeName1"
            };

            var UpdatedAttributeDetails = new TblAttribute
            {
                FldAttributeId = 1,
                FldCollectionId = 2,
                FldAttributeName = "NewAttributeName"
            };
            var mockSet = new Mock<DbSet<TblAttribute>>();

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(m => m.TblAttributes).Returns(mockSet.Object);

            var service = new AttributeServices(mockContext.Object);
            service.AddNewAttribute(OriginalAttribute);
            service.UpdateAttribute(OriginalAttribute, UpdatedAttributeDetails);
            mockSet.Verify(m => m.Add(It.IsAny<TblAttribute>()), Times.AtLeastOnce());
            mockContext.Verify(m => m.SaveChanges(), Times.AtLeastOnce());
        }

        [Fact]
        public void CheckIfAttributeExistsInCollectionTest()
        {
            var data = new List<TblAttribute>
            {
                new TblAttribute
                {
                    FldAttributeId = 1,
                    FldCollectionId = 1,
                    FldAttributeName = "TestAttributeName1"
                },
                new TblAttribute
                {
                    FldAttributeId = 2,
                    FldCollectionId = 1,
                    FldAttributeName = "TestAttributeName2"
                },
                new TblAttribute
                {
                    FldAttributeId = 3,
                    FldCollectionId = 1,
                    FldAttributeName = "TestAttributeName3"
                },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblAttribute>>();
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblAttributes).Returns(mockSet.Object);

            var service = new AttributeServices(mockContext.Object);
            var AttributeExistsInCollection1 = service.CheckIfAttributeExistsInCollection(new TblAttribute
            {
                FldCollectionId = 1,
                FldAttributeName = "TestAttributeName2"
            });

            var AttributeExistsInCollection2 = service.CheckIfAttributeExistsInCollection(new TblAttribute
            {
                FldCollectionId = 1,
                FldAttributeName = "TestAttributeName4"
            });

            var AttributeExistsInCollection3 = service.CheckIfAttributeExistsInCollection(new TblAttribute
            {
                FldCollectionId = 2,
                FldAttributeName = "TestAttributeName1"
            });

            Assert.Equal(true, AttributeExistsInCollection1);
            Assert.Equal(false, AttributeExistsInCollection2);
            Assert.Equal(false, AttributeExistsInCollection3);
        }

        [Fact]
        public void GetAttributesOnAttributeIDTest()
        {
            var data = new List<TblAttribute>
            {
                new TblAttribute
                {
                    FldAttributeId = 1,
                    FldCollectionId = 1,
                    FldAttributeName = "TestAttributeName1"
                },
                new TblAttribute
                {
                    FldAttributeId = 2,
                    FldCollectionId = 1,
                    FldAttributeName = "TestAttributeName2"
                },
                new TblAttribute
                {
                    FldAttributeId = 3,
                    FldCollectionId = 5,
                    FldAttributeName = "TestAttributeName3"
                },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblAttribute>>();
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblAttributes).Returns(mockSet.Object);

            var service = new AttributeServices(mockContext.Object);
            var QueriedAttributes = service.GetAttributesOnAttributeID(3);

            Assert.Equal(1, QueriedAttributes.Count);
            Assert.Equal(5, QueriedAttributes.First().FldCollectionId);
            Assert.Equal("TestAttributeName3", QueriedAttributes.First().FldAttributeName);
        }

        [Fact]
        public void GetAllAttributesOnCollectionIDTest()
        {
            var data = new List<TblAttribute>
            {
                new TblAttribute
                {
                    FldAttributeId = 1,
                    FldCollectionId = 1,
                    FldAttributeName = "TestAttributeName1"
                },
                new TblAttribute
                {
                    FldAttributeId = 2,
                    FldCollectionId = 1,
                    FldAttributeName = "TestAttributeName2"
                },
                new TblAttribute
                {
                    FldAttributeId = 3,
                    FldCollectionId = 5,
                    FldAttributeName = "TestAttributeName3"
                },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<TblAttribute>>();
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TblAttribute>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<CollectionsDbContext>();
            mockContext.Setup(c => c.TblAttributes).Returns(mockSet.Object);

            var service = new AttributeServices(mockContext.Object);
            var QueriedAttributes = service.GetAllAttributesOnCollectionID(1);

            Assert.Equal(2, QueriedAttributes.Count);
            Assert.Equal(1, QueriedAttributes[0].FldCollectionId);
            Assert.Equal("TestAttributeName1", QueriedAttributes[0].FldAttributeName);
            Assert.Equal(1, QueriedAttributes[1].FldCollectionId);
            Assert.Equal("TestAttributeName2", QueriedAttributes[1].FldAttributeName);
        }

    }
}
