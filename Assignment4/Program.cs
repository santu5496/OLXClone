using DbOperation.Interface;
using DbOperation.Implementation;
//using UserManagement.Lib;
using DbOperation.Models;


var builder = WebApplication.CreateBuilder(args);

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
   // pattern: "{controller=Configuration1}/{action=Configuration1}/{id?}");
    pattern: "{controller=SearchView}/{action=SearchView}/{id?}");


//  pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
