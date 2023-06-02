using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Xunit.Abstractions;
using Serilog.Sinks.Elasticsearch;

namespace XUnitTestProject
{
    public class DbAttributeTests : IDisposable 
    {
        public DbAttributeTests(ITestOutputHelper output)
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
        public void AddNewAttributeTest()
        {
            Log.Information("Carrying out Add New AttributeTest");
            
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

            try
            {
                mockSet.Verify(m => m.Add(It.IsAny<TblAttribute>()), Times.Once);
                mockContext.Verify(m => m.SaveChanges(), Times.Once());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void UpdateAttributeTest()
        {
            Log.Information("Carrying out Update Attribute test");
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
            try
            {
                mockSet.Verify(m => m.Add(It.IsAny<TblAttribute>()), Times.AtLeastOnce());
                mockContext.Verify(m => m.SaveChanges(), Times.AtLeastOnce());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
            
        }

        [Fact]
        public void CheckIfAttributeExistsInCollectionTest()
        {
            Log.Information("Carrying out test to check if attribute exists in the collection");
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

            try
            {
                Assert.Equal(true, AttributeExistsInCollection1);
                Assert.Equal(false, AttributeExistsInCollection2);
                Assert.Equal(false, AttributeExistsInCollection3);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void GetAttributesOnAttributeIDTest()
        {
            Log.Information("Carrying out test to get attribute on attributeID");
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

            try
            {
                Assert.Equal(1, QueriedAttributes.Count);
                Assert.Equal(5, QueriedAttributes.First().FldCollectionId);
                Assert.Equal("TestAttributeName3", QueriedAttributes.First().FldAttributeName);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

        [Fact]
        public void GetAllAttributesOnCollectionIDTest()
        {
            Log.Information("Carrying out test to get all attributes in a collection using CollectionID");
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

            try
            {
                Assert.Equal(2, QueriedAttributes.Count);
                Assert.Equal(1, QueriedAttributes[0].FldCollectionId);
                Assert.Equal("TestAttributeName1", QueriedAttributes[0].FldAttributeName);
                Assert.Equal(1, QueriedAttributes[1].FldCollectionId);
                Assert.Equal("TestAttributeName2", QueriedAttributes[1].FldAttributeName);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw;
            }
        }

    }
}
