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
}