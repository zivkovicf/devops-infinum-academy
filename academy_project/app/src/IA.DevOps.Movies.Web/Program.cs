using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using IA.DevOps.Movies.Common;
using IA.DevOps.Movies.Contracts.Models;
using IA.DevOps.Movies.Data.AwsS3;
using IA.DevOps.Movies.Data.Db;
using IA.DevOps.Movies.Data.Db.SeedData;
using IA.DevOps.Movies.Data.LocalBLOB;
using IA.DevOps.Movies.Services;
using IA.DevOps.Movies.Web;
using IA.DevOps.Movies.Web.Hubs;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

#region Web Application Configuration

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorise()
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

builder.Services.RegisterFluentValidators();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

#endregion

#region Configurations and Services

builder.Services.AddMoviesDbServices(builder.Configuration.GetConnectionString("MoviesDB"));

var databaseConnectionSettings = builder.Configuration.GetSection("ConnectionStrings");
builder.Services.Configure<DatabaseConnectionSettings>(databaseConnectionSettings);

var awsS3Settings = builder.Configuration.GetSection("AWS:S3");

if (awsS3Settings.Exists())
{
    builder.Services.Configure<AwsS3Settings>(awsS3Settings);
    builder.Services.AddAwsS3StorageServices();
}
else
{
    builder.Services.AddLocalBlobStorageServices();
}

builder.Services.AddServices();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
#endregion

#region API Configuration

builder.Services.AddControllers();
builder.Services.AddRouting(
    options =>
    {
        options.LowercaseUrls = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

builder.Services.AddHostedService<HealthMonitor>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpLogging();

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true
});
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapHub<ChartHub>("/chart-hub");
app.MapFallbackToPage("/_Host");

app.MapControllers();

app.Run();
