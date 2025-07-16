using DbOperation.Interface;
using DbOperation.Implementation;
//using UserManagement.Lib;
using DbOperation.Models;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Enable detailed logging for debugging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IConfigurationService, ConfigurationService>(provider =>
{
    return new ConfigurationService(builder.Configuration.GetConnectionString("Assignment4"));
});

builder.Services.AddSingleton<IConfigurationService1, ConfigurationService1>(provider =>
{
    return new ConfigurationService1(builder.Configuration.GetConnectionString("Assignment4"));
});

builder.Services.AddSingleton<IVehicleManagementSerivice, VehicleManagementService>(provider =>
{
    return new VehicleManagementService(builder.Configuration.GetConnectionString("Assignment4"));
});

builder.Services.AddSingleton<IImageServices, ImageServices>(provider =>
{
    return new ImageServices(builder.Configuration.GetConnectionString("Assignment4"));
});

builder.Services.AddSingleton<ISearchViewService, SearchViewService>(provider =>
{
    return new SearchViewService(builder.Configuration.GetConnectionString("Assignment4"));
});

builder.Services.AddSingleton<IUserService, UsersService>(provider =>
{
    return new UsersService(builder.Configuration.GetConnectionString("Assignment4"));
});

//builder.Services.AddSingleton<IInventoryMaterialLog, InventoryMaterialLogService>(provider =>
//{
//    return new InventoryMaterialLogService(builder.Configuration.GetConnectionString("Assignment4"));
//});
//builder.Services.AddSingleton<IFinishedGoodsService, FinishedGoodsService>(provider =>
//{
//    return new FinishedGoodsService(builder.Configuration.GetConnectionString("Assignment4"));
//});
//builder.Services.AddSingleton<IBillingService, BillingService>(provider =>
//{
//    return new BillingService(builder.Configuration.GetConnectionString("Assignment4"));
//});
//builder.Services.AddSingleton<IRecipeService,RecipeService>(provider =>
//{
//    return new RecipeService(builder.Configuration.GetConnectionString("Assignment4"));
//});
//builder.Services.AddSingleton<IReturnmanagementService, ReturnManagementService>(provider =>
//{
//    return new ReturnManagementService(builder.Configuration.GetConnectionString("Assignment4"));
//});
//builder.Services.AddSingleton<IOrderManagement, OrderManagementSerivice>(provider =>
//{
//    return new OrderManagementSerivice(builder.Configuration.GetConnectionString("Assignment4"));
//});
//builder.Services.AddSingleton<IReportService, ReportService>(provider =>
//{
//    return new ReportService(builder.Configuration.GetConnectionString("Assignment4"),builder.Configuration.GetConnectionString("Assignment4"));
//});
//builder.Services.AddSingleton<IDailyExpenseService, DailyExpenseService>(provider =>
//{
//    return new DailyExpenseService(builder.Configuration.GetConnectionString("Assignment4"));
//});
//builder.Services.AddUserManagementServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

// TEMPORARILY DISABLED for debugging - uncomment after fixing the issue
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     app.UseHsts();
// }

// TEMPORARY: Enable detailed exception page for debugging
app.UseDeveloperExceptionPage();

// Alternative: Custom exception handler that shows details
// Uncomment this and comment out UseDeveloperExceptionPage() if you prefer custom error handling
/*
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        
        // Log the exception details
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "An unhandled exception occurred");
        
        // Return detailed error response
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/html";
        
        await context.Response.WriteAsync($@"
            <html>
            <head><title>Application Error</title></head>
            <body>
                <h1>Application Error</h1>
                <h2>Exception Details:</h2>
                <p><strong>Message:</strong> {exception?.Message}</p>
                <p><strong>Type:</strong> {exception?.GetType().Name}</p>
                <h3>Stack Trace:</h3>
                <pre>{exception?.StackTrace}</pre>
                <h3>Inner Exception:</h3>
                <pre>{exception?.InnerException?.Message}</pre>
                <pre>{exception?.InnerException?.StackTrace}</pre>
            </body>
            </html>
        ");
    });
});
*/

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
     pattern: "{controller=Configuration1}/{action=Configuration1}/{id?}");
    //pattern: "{controller=SearchView}/{action=SearchView}/{id?}");
//  pattern: "{controller=Login}/{action=Login}/{id?}");

// Log the connection string for debugging (remove this after fixing)
var connectionString = builder.Configuration.GetConnectionString("Assignment4");
Console.WriteLine($"Connection String: {connectionString}");

app.Run();