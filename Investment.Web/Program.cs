using AutoMapper;
using Investment.BLL.Services;
using Investment.Core.Interfaces;
using Investment.Core.Mappings;
using Investment.DAL.Data;
using Investment.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()));

// Configures the InvestmentDbContext to use either an in-memory database or SQL Server based on the "UseInMemoryDatabase" setting in appsettings.json.
// By default, this setting is enabled (true), which initializes the context with an in-memory database suitable for testing or development. 
// To switch to SQL Server, change "UseInMemoryDatabase" to false in appsettings.json and ensure the "DefaultConnection" string is configured in the connection strings.
// Before running the application with SQL Server, make sure to execute the migrations to set up the required database schema.
bool useInMemoryDatabase = builder.Configuration.GetValue<bool>("DatabaseConfig:UseInMemoryDatabase");
builder.Services.AddDbContext<InvestmentDbContext>(options =>
{
    if (useInMemoryDatabase)
    {
        options.UseInMemoryDatabase("InvestmentInMemoryDb");
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Add Services
builder.Services.AddScoped<IStockLotRepository, StockLotRepository>();
builder.Services.AddScoped<ISaleRecordRepository, SaleRecordRepository>();
builder.Services.AddScoped<IInvestmentService, InvestmentService>();

// Mapper
var config = new MapperConfiguration(cfg => { cfg.AddProfile(new ApplicationProfile()); });
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

// Build app
var app = builder.Build();

// Create a database and the data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InvestmentDbContext>();
    dbContext.Database.EnsureCreated();
}

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
