using AgriStream.Web.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();  

builder.Services.AddDbContext<AgriStreamDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

// Azure Blob client
builder.Services.AddSingleton(x =>
{
    var conn = builder.Configuration.GetConnectionString("Storage")
               ?? throw new InvalidOperationException("Storage connection string missing");
    return new Azure.Storage.Blobs.BlobServiceClient(conn);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();