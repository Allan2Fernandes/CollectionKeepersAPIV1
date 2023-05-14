using CollectionKeepersAPIV1.Controllers.ControllerLogic;
using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Xunit.Abstractions;
using Moq;

namespace XUnitTestProject;

public class DbAttributeValueTests : IDisposable
{
    public DbAttributeValueTests(ITestOutputHelper output) 
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.TestOutput(output)
            .WriteTo.File("./TestSerilogs/DbCollectionTestLogs.txt")
            .CreateLogger();
            
    }

    public void Dispose()
    {
        Log.CloseAndFlush();
    }
    
    [Fact]
    public void AddNewCollectionEntryTest()
    {
        Log.Information("Carrying out Add New Collection Entry test");
            
        var mockSet = new Mock<DbSet<TblCollectionEntry>>();
        var mockContext = new Mock<CollectionsDbContext>();
        mockContext.Setup(m => m.TblCollectionEntries).Returns(mockSet.Object);

        var service = new AttributeValueServices(mockContext.Object);
        service.CreatenewCollectionEntry();
        try
        {
            mockSet.Verify(m => m.Add(It.IsAny<TblCollectionEntry>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }
    
    [Fact]
    public void AddListOfAttributeValues()
    {
        Log.Information("Carrying out Add New Collection Entry test");
            
        var mockSet = new Mock<DbSet<TblAttributeValue>>();
        var mockContext = new Mock<CollectionsDbContext>();
        mockContext.Setup(m => m.TblAttributeValues).Returns(mockSet.Object);

        var service = new AttributeValueServices(mockContext.Object);
        service.AddListofAttributeValues(new TblAttributeValue[]
        {
            new TblAttributeValue(),
            new TblAttributeValue(),
            new TblAttributeValue()
        });
        try
        {
            mockSet.Verify(m => m.AddRange(It.IsAny<TblAttributeValue[]>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }
    
    [Fact]
    public void UpdateAttributeValueTest()
    {
        Log.Information("Carrying out test to update an attribute value");

        TblAttributeValue OriginalAttributeValue = new TblAttributeValue
        {
            FldAttributeId = 5,
            FldCollectionEntryId = 1,
            FldValue = "Iphone"
        };
        
        TblAttributeValue UpdatedAttributeValueDetails = new TblAttributeValue
        {
            FldAttributeId = 5,
            FldCollectionEntryId = 1,
            FldValue = "Nokia"
        };
        
        var mockSet = new Mock<DbSet<TblAttributeValue>>();
        var mockContext = new Mock<CollectionsDbContext>();
        mockContext.Setup(m => m.TblAttributeValues).Returns(mockSet.Object);

        var service = new AttributeValueServices(mockContext.Object);
        service.UpdateAttributeValue(OriginalAttributeValue, UpdatedAttributeValueDetails);
        try
        {
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }
    
    [Fact]
    public void GetAttributeValuesOnAttributeValueIDTest()
    {
        Log.Information("Carrying out test to get attribute on attributeID");
        var data = new List<TblAttributeValue>
        {
            new TblAttributeValue
            {
                FldAttributeValueId = 1,
                FldAttributeId = 5,
                FldCollectionEntryId = 1,
                FldValue = "Nokia"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 2,
                FldAttributeId = 5,
                FldCollectionEntryId = 1,
                FldValue = "Samsung"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 3,
                FldAttributeId = 5,
                FldCollectionEntryId = 1,
                FldValue = "OnePlus"
            },
        }.AsQueryable();

        var mockSet = new Mock<DbSet<TblAttributeValue>>();
        mockSet.As<IQueryable<TblAttributeValue>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<TblAttributeValue>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<TblAttributeValue>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<TblAttributeValue>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

        var mockContext = new Mock<CollectionsDbContext>();
        mockContext.Setup(c => c.TblAttributeValues).Returns(mockSet.Object);

        var service = new AttributeValueServices(mockContext.Object);
        var QueriedAttributes = service.GetAttributeValueOnID(3);

        try
        {
            Assert.Equal(1, QueriedAttributes.Count);
            Assert.Equal(5, QueriedAttributes.First().FldAttributeId);
            Assert.Equal("OnePlus", QueriedAttributes.First().FldValue);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }
    
    [Fact]
    public void GetCollectionEntryOnIDTest()
    {
        Log.Information("Carrying out test to get all attributes in a collection entry");

        var CollectionsData = new List<TblCollection>
        {
            new TblCollection
            {
                FldCollectionId = 1,
                FldUserId = 1,
                FldCollectionName = "Pokemon Cards",
                FldCollectionDescription = "A card game",
                FldCollectionThumbnail = "PkCards.png",
                FldIsPrivate = false
            }
        }.AsQueryable();

        var AttributeData = new List<TblAttribute>
        {
            new TblAttribute
            {
                FldAttributeId = 1,
                FldCollectionId = 1,
                FldAttributeName = "Name"
            },
            new TblAttribute
            {
                FldAttributeId = 2,
                FldCollectionId = 1,
                FldAttributeName = "Type"
            },
            new TblAttribute
            {
                FldAttributeId = 3,
                FldCollectionId = 1,
                FldAttributeName = "IMage"
            },
        }.AsQueryable();

        var CollectionEntryData = new List<TblCollectionEntry>
        {
            new TblCollectionEntry
            {
                FldCollectionEntryId = 1
            },
            new TblCollectionEntry
            {
                FldCollectionEntryId = 2
            },
            new TblCollectionEntry
            {
                FldCollectionEntryId = 3
            }
        }.AsQueryable();

        var AttributeValueData = new List<TblAttributeValue>
        {
            new TblAttributeValue
            {
                FldAttributeValueId = 1,
                FldAttributeId = 1,
                FldCollectionEntryId = 1,
                FldValue = "Pikachu"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 2,
                FldAttributeId = 2,
                FldCollectionEntryId = 1,
                FldValue = "Lightning"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 3,
                FldAttributeId = 3,
                FldCollectionEntryId = 1,
                FldValue = "pikachu.png"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 4,
                FldAttributeId = 1,
                FldCollectionEntryId = 2,
                FldValue = "Charmander"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 5,
                FldAttributeId = 2,
                FldCollectionEntryId = 2,
                FldValue = "Fire"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 6,
                FldAttributeId = 3,
                FldCollectionEntryId = 2,
                FldValue = "Charmander.png"
            },
        }.AsQueryable();
        
        var mockSetCollection = new Mock<DbSet<TblCollection>>();
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.Provider).Returns(CollectionsData.Provider);
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.Expression).Returns(CollectionsData.Expression);
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.ElementType).Returns(CollectionsData.ElementType);
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.GetEnumerator()).Returns(() => CollectionsData.GetEnumerator());

        var mockSetAttribute = new Mock<DbSet<TblAttribute>>();
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.Provider).Returns(AttributeData.Provider);
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.Expression).Returns(AttributeData.Expression);
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.ElementType).Returns(AttributeData.ElementType);
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.GetEnumerator()).Returns(() => AttributeData.GetEnumerator());
        
        var mockSetCollectionEntry = new Mock<DbSet<TblCollectionEntry>>();
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.Provider).Returns(CollectionEntryData.Provider);
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.Expression).Returns(CollectionEntryData.Expression);
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.ElementType).Returns(CollectionEntryData.ElementType);
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.GetEnumerator()).Returns(() => CollectionEntryData.GetEnumerator());
        
        var mockSetAttributeValue = new Mock<DbSet<TblAttributeValue>>();
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.Provider).Returns(AttributeValueData.Provider);
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.Expression).Returns(AttributeValueData.Expression);
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.ElementType).Returns(AttributeValueData.ElementType);
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.GetEnumerator()).Returns(() => AttributeValueData.GetEnumerator());

        var mockContext = new Mock<CollectionsDbContext>();
        mockContext.Setup(c => c.TblCollections).Returns(mockSetCollection.Object);
        mockContext.Setup(c => c.TblAttributes).Returns(mockSetAttribute.Object);
        mockContext.Setup(c => c.TblCollectionEntries).Returns(mockSetCollectionEntry.Object);
        mockContext.Setup(c => c.TblAttributeValues).Returns(mockSetAttributeValue.Object);

        var service = new AttributeValueServices(mockContext.Object);
        var QueriedAttributes = service.GetCollectionEntryOnID(1);

        try
        {
            Assert.Equal(3, QueriedAttributes.Count);
            Assert.Equal(1, QueriedAttributes[1].FldCollectionEntryId);
            Assert.Equal(2, QueriedAttributes[1].FldAttributeId);
            Assert.Equal("pikachu.png", QueriedAttributes[2].FldValue);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }

    [Fact]
    public void GetDisctinctCollectionEntryIDsOnCollectionIDTest()
    {
         Log.Information("Carrying out test to get all unique CollectionEntryIDs on CollectionID");

        var CollectionsData = new List<TblCollection>
        {
            new TblCollection
            {
                FldCollectionId = 1,
                FldUserId = 1,
                FldCollectionName = "Pokemon Cards",
                FldCollectionDescription = "A card game",
                FldCollectionThumbnail = "PkCards.png",
                FldIsPrivate = false
            },
            new TblCollection
            {
                FldCollectionId = 2,
                FldUserId = 1,
                FldCollectionName = "Cars",
                FldCollectionDescription = "A car game",
                FldCollectionThumbnail = "Cars.png",
                FldIsPrivate = false
            }
        }.AsQueryable();

        var AttributeData = new List<TblAttribute>
        {
            new TblAttribute
            {
                FldAttributeId = 1,
                FldCollectionId = 1,
                FldAttributeName = "Name"
            },
            new TblAttribute
            {
                FldAttributeId = 2,
                FldCollectionId = 1,
                FldAttributeName = "Type"
            },
            new TblAttribute
            {
                FldAttributeId = 3,
                FldCollectionId = 1,
                FldAttributeName = "IMage"
            },
        }.AsQueryable();

        var CollectionEntryData = new List<TblCollectionEntry>
        {
            new TblCollectionEntry
            {
                FldCollectionEntryId = 1
            },
            new TblCollectionEntry
            {
                FldCollectionEntryId = 2
            },
            new TblCollectionEntry
            {
                FldCollectionEntryId = 3
            }
        }.AsQueryable();

        var AttributeValueData = new List<TblAttributeValue>
        {
            new TblAttributeValue
            {
                FldAttributeValueId = 1,
                FldAttributeId = 1,
                FldCollectionEntryId = 1,
                FldValue = "Pikachu"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 2,
                FldAttributeId = 2,
                FldCollectionEntryId = 1,
                FldValue = "Lightning"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 3,
                FldAttributeId = 3,
                FldCollectionEntryId = 1,
                FldValue = "pikachu.png"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 4,
                FldAttributeId = 1,
                FldCollectionEntryId = 2,
                FldValue = "Charmander"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 5,
                FldAttributeId = 2,
                FldCollectionEntryId = 2,
                FldValue = "Fire"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 6,
                FldAttributeId = 3,
                FldCollectionEntryId = 2,
                FldValue = "Charmander.png"
            },
        }.AsQueryable();
        
         var mockSetCollection = new Mock<DbSet<TblCollection>>();
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.Provider).Returns(CollectionsData.Provider);
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.Expression).Returns(CollectionsData.Expression);
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.ElementType).Returns(CollectionsData.ElementType);
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.GetEnumerator()).Returns(() => CollectionsData.GetEnumerator());

        var mockSetAttribute = new Mock<DbSet<TblAttribute>>();
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.Provider).Returns(AttributeData.Provider);
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.Expression).Returns(AttributeData.Expression);
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.ElementType).Returns(AttributeData.ElementType);
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.GetEnumerator()).Returns(() => AttributeData.GetEnumerator());
        
        var mockSetCollectionEntry = new Mock<DbSet<TblCollectionEntry>>();
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.Provider).Returns(CollectionEntryData.Provider);
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.Expression).Returns(CollectionEntryData.Expression);
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.ElementType).Returns(CollectionEntryData.ElementType);
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.GetEnumerator()).Returns(() => CollectionEntryData.GetEnumerator());
        
        var mockSetAttributeValue = new Mock<DbSet<TblAttributeValue>>();
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.Provider).Returns(AttributeValueData.Provider);
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.Expression).Returns(AttributeValueData.Expression);
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.ElementType).Returns(AttributeValueData.ElementType);
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.GetEnumerator()).Returns(() => AttributeValueData.GetEnumerator());

        var mockContext = new Mock<CollectionsDbContext>();
        mockContext.Setup(c => c.TblCollections).Returns(mockSetCollection.Object);
        mockContext.Setup(c => c.TblAttributes).Returns(mockSetAttribute.Object);
        mockContext.Setup(c => c.TblCollectionEntries).Returns(mockSetCollectionEntry.Object);
        mockContext.Setup(c => c.TblAttributeValues).Returns(mockSetAttributeValue.Object);
        
        var service = new AttributeValueServices(mockContext.Object);
        var ListOfCollectionEntryIDs = service.GetDisctinctCollectionEntryIDsOnCollectionID(1);
        
        try
        {
            Assert.Equal(2, ListOfCollectionEntryIDs.Count);
            Assert.Equal(1, ListOfCollectionEntryIDs[0]);
            Assert.Equal(2, ListOfCollectionEntryIDs[1]);
        }  catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }
    
    [Fact]
    public void GetAllAttributeValuesForACollectionTest()
    {
         Log.Information("Carrying out test to get all attribute values in a collection on CollectionID");

        var CollectionsData = new List<TblCollection>
        {
            new TblCollection
            {
                FldCollectionId = 1,
                FldUserId = 1,
                FldCollectionName = "Pokemon Cards",
                FldCollectionDescription = "A card game",
                FldCollectionThumbnail = "PkCards.png",
                FldIsPrivate = false
            },
            new TblCollection
            {
                FldCollectionId = 2,
                FldUserId = 1,
                FldCollectionName = "Cars",
                FldCollectionDescription = "A car game",
                FldCollectionThumbnail = "Cars.png",
                FldIsPrivate = false
            }
        }.AsQueryable();

        var AttributeData = new List<TblAttribute>
        {
            new TblAttribute
            {
                FldAttributeId = 1,
                FldCollectionId = 1,
                FldAttributeName = "Name"
            },
            new TblAttribute
            {
                FldAttributeId = 2,
                FldCollectionId = 1,
                FldAttributeName = "Type"
            },
            new TblAttribute
            {
                FldAttributeId = 3,
                FldCollectionId = 1,
                FldAttributeName = "IMage"
            },
        }.AsQueryable();

        var CollectionEntryData = new List<TblCollectionEntry>
        {
            new TblCollectionEntry
            {
                FldCollectionEntryId = 1
            },
            new TblCollectionEntry
            {
                FldCollectionEntryId = 2
            },
            new TblCollectionEntry
            {
                FldCollectionEntryId = 3
            }
        }.AsQueryable();

        var AttributeValueData = new List<TblAttributeValue>
        {
            new TblAttributeValue
            {
                FldAttributeValueId = 1,
                FldAttributeId = 1,
                FldCollectionEntryId = 1,
                FldValue = "Pikachu"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 2,
                FldAttributeId = 2,
                FldCollectionEntryId = 1,
                FldValue = "Lightning"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 3,
                FldAttributeId = 3,
                FldCollectionEntryId = 1,
                FldValue = "Pikachu.png"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 4,
                FldAttributeId = 1,
                FldCollectionEntryId = 2,
                FldValue = "Charmander"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 5,
                FldAttributeId = 2,
                FldCollectionEntryId = 2,
                FldValue = "Fire"
            },
            new TblAttributeValue
            {
                FldAttributeValueId = 6,
                FldAttributeId = 3,
                FldCollectionEntryId = 2,
                FldValue = "Charmander.png"
            },
        }.AsQueryable();
        
         var mockSetCollection = new Mock<DbSet<TblCollection>>();
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.Provider).Returns(CollectionsData.Provider);
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.Expression).Returns(CollectionsData.Expression);
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.ElementType).Returns(CollectionsData.ElementType);
        mockSetCollection.As<IQueryable<TblCollection>>().Setup(m => m.GetEnumerator()).Returns(() => CollectionsData.GetEnumerator());

        var mockSetAttribute = new Mock<DbSet<TblAttribute>>();
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.Provider).Returns(AttributeData.Provider);
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.Expression).Returns(AttributeData.Expression);
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.ElementType).Returns(AttributeData.ElementType);
        mockSetAttribute.As<IQueryable<TblAttribute>>().Setup(m => m.GetEnumerator()).Returns(() => AttributeData.GetEnumerator());
        
        var mockSetCollectionEntry = new Mock<DbSet<TblCollectionEntry>>();
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.Provider).Returns(CollectionEntryData.Provider);
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.Expression).Returns(CollectionEntryData.Expression);
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.ElementType).Returns(CollectionEntryData.ElementType);
        mockSetCollectionEntry.As<IQueryable<TblCollectionEntry>>().Setup(m => m.GetEnumerator()).Returns(() => CollectionEntryData.GetEnumerator());
        
        var mockSetAttributeValue = new Mock<DbSet<TblAttributeValue>>();
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.Provider).Returns(AttributeValueData.Provider);
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.Expression).Returns(AttributeValueData.Expression);
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.ElementType).Returns(AttributeValueData.ElementType);
        mockSetAttributeValue.As<IQueryable<TblAttributeValue>>().Setup(m => m.GetEnumerator()).Returns(() => AttributeValueData.GetEnumerator());

        var mockContext = new Mock<CollectionsDbContext>();
        mockContext.Setup(c => c.TblCollections).Returns(mockSetCollection.Object);
        mockContext.Setup(c => c.TblAttributes).Returns(mockSetAttribute.Object);
        mockContext.Setup(c => c.TblCollectionEntries).Returns(mockSetCollectionEntry.Object);
        mockContext.Setup(c => c.TblAttributeValues).Returns(mockSetAttributeValue.Object);
        
        var service = new AttributeValueServices(mockContext.Object);
        var ListOfCollectionEntryIDs1 = service.GetAllAttributeValuesForACollection(1);
        var ListOfCollectionEntryIDs2 = service.GetAllAttributeValuesForACollection(2);
        
        try
        {
            Assert.Equal(0, ListOfCollectionEntryIDs2.Count);
            Assert.Equal(6, ListOfCollectionEntryIDs1.Count);
            Assert.Equal("Charmander.png", ListOfCollectionEntryIDs1[5].FldValue);
            Assert.Equal("Pikachu.png", ListOfCollectionEntryIDs1[4].FldValue);
            Assert.Equal(3, ListOfCollectionEntryIDs1[4].FldAttributeId);
        }  catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }
}