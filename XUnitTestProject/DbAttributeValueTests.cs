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
            mockSet.Verify(m => m.Add(It.IsAny<TblCollectionEntry>()), Times.Once);
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
            mockSet.Verify(m => m.AddRange(It.IsAny<TblAttributeValue[]>()), Times.Once);
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    }
}