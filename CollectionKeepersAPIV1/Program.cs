using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
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

        // Database context
        builder.Services.AddDbContext<CollectionsDbContext>(options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        // ----- CORS -----
        
        string policyName = "ANGRYGORILLA";
        builder.Services.AddCors(policy => policy.AddPolicy(policyName, corsPolicy => {
                corsPolicy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            })
        );
        /*
        builder.Services.AddCors(policyBuilder => policyBuilder.AddDefaultPolicy(policy => {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            })
        );
        */

        // Serilog
        using var log = new LoggerConfiguration() //new
            .WriteTo.Console()
            .WriteTo.File("./Serilogs/logs.txt")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            {
                DetectElasticsearchVersion = false,
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
            })
            .CreateLogger();
        Log.Logger = log; //new 
        log.Information("Done setting up serilog!"); //new

        // Heroku wants to specify the port, se we gotta get the port from environment varibles when running on heroku
        var port = Environment.GetEnvironmentVariable("PORT");
        builder.WebHost.UseUrls("http://*:" + port);
    
        var app = builder.Build();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();
        app.UseCors(policyName);
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}


