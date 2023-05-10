using CollectionKeepersAPIV1.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;

public class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.

    

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddDbContext<CollectionsDbContext>(options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddCors(policyBuilder =>
                   policyBuilder.AddDefaultPolicy(policy =>
                   {
                       policy.AllowAnyOrigin();
                       policy.AllowAnyHeader();
                       policy.AllowAnyMethod();
                   }
                   )
               );
        using var log = new LoggerConfiguration() //new
            .WriteTo.Console()
            .WriteTo.File("./Serilogs/logs.txt")
            .CreateLogger();

        Log.Logger = log; //new 
        log.Information("Done setting up serilog!"); //new


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }


}


