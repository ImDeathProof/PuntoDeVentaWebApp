using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de DbContext
builder.Services.AddDbContext<DataContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionSQLServer")));

builder.Services.AddIdentity<User, UserRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// Configuración de cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Owner configuration

if (!app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    //Credentials
    var ownerEmail = builder.Configuration["OwnerCredentials:Email"] 
        ?? Environment.GetEnvironmentVariable("OWNER_EMAIL");
    var ownerPassword = builder.Configuration["OwnerCredentials:Password"]
        ?? Environment.GetEnvironmentVariable("OWNER_PASSWORD");
    
    if(ownerEmail != null && ownerPassword != null)
    {
        if(!await roleManager.RoleExistsAsync("Owner"))
        {
            var role = new UserRole
            {
                Name = "Owner",
                NormalizedName = "OWNER",
                AccessLevel = 10,
                Description = "God access level for the owner of the system"
            };
            await roleManager.CreateAsync(role);
        }
        if(await userManager.FindByEmailAsync(ownerEmail) == null)
        {
            var owner = new User {
                UserName = ownerEmail,
                Email = ownerEmail,
                NormalizedUserName = ownerEmail.ToUpper(),
                NormalizedEmail = ownerEmail.ToUpper(),
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(owner, ownerPassword);
            await userManager.AddToRoleAsync(owner, "Owner");
        }
    }
    // Configure roles in db for first time
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var role = new UserRole
        {
            Name = "Admin",
            NormalizedName = "ADMIN",
            AccessLevel = 8,
            Description = "Administrator access level"
        };
        await roleManager.CreateAsync(role);
    }
    if (!await roleManager.RoleExistsAsync("Seller"))
    {
        var role = new UserRole
        {
            Name = "Seller",
            NormalizedName = "SELLER",
            AccessLevel = 3,
            Description = "Seller access level"
        };
        await roleManager.CreateAsync(role);
    }
    if (!await roleManager.RoleExistsAsync("Manager"))
    {
        var role = new UserRole
        {
            Name = "Manager",
            NormalizedName = "MANAGER",
            AccessLevel = 7,
            Description = "Manager access level"
        };
        await roleManager.CreateAsync(role);
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
