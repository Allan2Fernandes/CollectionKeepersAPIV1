using Serilog;
using Xunit.Abstractions;

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
    
    
}