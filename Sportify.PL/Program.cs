using Sportify;
using Sportify.Models;
using Sportify.BLL.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────

builder.Services.AddControllersWithViews();

// Register BLL Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IGymAdService, GymAdService>();

// Register AppDbContext — move connection string to appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session support (needed for login/logout in AccountController)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── Pipeline ──────────────────────────────────────────────────────────────────

var app = builder.Build();

// Migrate/Alter Database Columns on Startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.ExecuteSqlRaw("ALTER TABLE Products ALTER COLUMN ImageURL nvarchar(max) NULL;");
        dbContext.Database.ExecuteSqlRaw("ALTER TABLE ProductVariants ALTER COLUMN ImageURL nvarchar(max) NULL;");
        dbContext.Database.ExecuteSqlRaw("ALTER TABLE Orders ALTER COLUMN PaidAt DATETIME NULL;");
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserMessages' AND xtype='U')
            BEGIN
                CREATE TABLE UserMessages (
                    UserMessageID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT NOT NULL,
                    MessageText NVARCHAR(2000) NOT NULL,
                    CreatedAt DATETIME NOT NULL,
                    CONSTRAINT FK_UserMessages_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END
        ");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization warning: {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();        // ← must come before UseAuthorization
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
