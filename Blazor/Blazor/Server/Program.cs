using Blazor.Server.Configuration;
using Blazor.Server.DatabaseContext;
using Blazor.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddSingleton(builder.Configuration.Get<BlazorConfiguration>());
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<BlazorContext>();
builder.Services.AddSingleton<WalletBuffer>();
builder.Services.AddScoped<NodeService>();
builder.Services.AddScoped<DatabaseManager>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
