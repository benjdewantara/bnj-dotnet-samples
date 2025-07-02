using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using net8AspNetIdentity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString =
    builder.Configuration
        .GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    );
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            //    
        }
    )
    .AddEntityFrameworkStores<ApplicationDbContext>()
    ;
builder.Services
    .AddControllersWithViews();

var app = builder.Build();

{
    using var serviceScope = app.Services.CreateScope();
    var applicationDbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>()!;
    bool pendingMigrationsExist = applicationDbContext.Database.GetPendingMigrations().Any();
    if (pendingMigrationsExist)
    {
        applicationDbContext.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages();

app.Run();