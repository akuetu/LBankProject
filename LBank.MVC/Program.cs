using LBank.Business;
using LBank.Domain;
using LBank.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ServerConfigSettings>(builder.Configuration.GetSection("ServerConfig"));

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IFileRepository, FileRepository>();
builder.Services.AddTransient<IServerConfigParser, ServerConfigParser>();
builder.Services.AddScoped<IServerConfigService, ServerConfigService>();


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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
