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
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@%!¡¿?";
    options.User.RequireUniqueEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// Configuración de cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Home/Index";
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Repositories and Services
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Owner configuration

if (!app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    db.Database.Migrate();

    //Credentials
    var ownerEmail = builder.Configuration["OwnerCredentials:Email"]
        ?? Environment.GetEnvironmentVariable("OWNER_EMAIL");
    var ownerPassword = builder.Configuration["OwnerCredentials:Password"]
        ?? Environment.GetEnvironmentVariable("OWNER_PASSWORD");

    if (ownerEmail != null && ownerPassword != null)
    {
        if (!await roleManager.RoleExistsAsync("Owner"))
        {
            var role = new UserRole("Owner") // Usa el nuevo constructor
            {
                NormalizedName = "OWNER",
                AccessLevel = 10,
                Description = "God access level for the owner of the system"
            };
            await roleManager.CreateAsync(role);

        }
        if (await userManager.FindByEmailAsync(ownerEmail) == null)
        {
            var owner = new User
            {
                UserName = ownerEmail,
                Email = ownerEmail,
                NormalizedUserName = ownerEmail.ToUpper(),
                NormalizedEmail = ownerEmail.ToUpper(),
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(owner, ownerPassword);
            if (!result.Succeeded)
            {
                // Log de errores (útil para depuración)
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }
                throw new Exception("Error al crear el usuario propietario");
            }
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
    // add payment methods
    if(!await db.PaymentMethods.AnyAsync())
    {
        var paymentMethods = new List<PaymentMethod>
        {
            new PaymentMethod { Name = "Cash"},
            new PaymentMethod { Name = "Credit Card"},
            new PaymentMethod { Name = "Debit Card"},
            new PaymentMethod { Name = "Bank Transfer"},
            new PaymentMethod { Name = "Check"},
        };
        await db.PaymentMethods.AddRangeAsync(paymentMethods);
        await db.SaveChangesAsync();
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
