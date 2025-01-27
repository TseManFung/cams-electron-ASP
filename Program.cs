using Campus_Asset_Management_System;
using ElectronNET.API;
using ElectronNET.API.Entities;
using RFIDReaderAPI;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseElectron(args);

// Optional: Use the Electron.NET API classes directly with Dependency Injection
builder.Services.AddElectron();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/RfidReader/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// TODO: make it to real url
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=RfidReader}/{action=Index}/{id?}");

await app.StartAsync(); // Start the application asynchronously

// Initialize Electron
await ElectronBootstrap.InitAsync(); // 確保這裡調用 InitAsync
app.WaitForShutdown(); // Wait for the application to shut down