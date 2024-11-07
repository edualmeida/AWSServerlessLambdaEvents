using Amazon.CloudWatchLogs;
using Amazon.DynamoDBv2;
using AWSServerless2.Application.Services;
using AWSServerless2.Configuration;
using AWSServerless2.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AWSServerless2;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var appSettings = new AppSettings();
        Configuration.Bind(appSettings);
        services.AddSingleton(appSettings);

        services.AddTransient<IEventService, EventService>();

        var awsOptions = Configuration.GetAWSOptions();

        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddControllers();

        //if (Environment.IsDevelopment())
        //{
        //    //services.AddSingleton<IAmazonS3>(provider => {
        //    //    var settings = provider.GetService<AppSettings>();
        //    //    return new AmazonS3Client(npbnpinnew AmazonS3Config
        //    //    {
        //    //        UseHttp = true,
        //    //        ServiceURL = settings.Aws.S3.ServiceUrl,
        //    //        ForcePathStyle = true
        //    //    });
        //    //});
        //    services.AddSingleton<IAmazonCloudWatchLogs>(provider => {
        //        var settings = provider.GetService<AppSettings>();
        //        return new AmazonCloudWatchLogsClient(new AmazonCloudWatchLogsConfig
        //        {
        //            UseHttp = true,
        //            ServiceURL = settings.Aws.CloudWatch.ServiceUrl
        //        });
        //    });
        //}
        //else
        //{
        //    services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
        //    services.AddAWSService<IAmazonCloudWatchLogs>();
        //}
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.ConfigureCustomExceptionMiddleware();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}