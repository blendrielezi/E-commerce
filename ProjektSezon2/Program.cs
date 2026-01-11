using Microsoft.AspNetCore.Authentication.Google;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektSezon2.Data;
using ProjektSezon2.Models;
using ProjektSezon2.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// 1) Configure EF Core & Identity DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    )
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2) Register IHttpContextAccessor for session access
builder.Services.AddHttpContextAccessor();

// 3) Identity + Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 4) External authentication providers
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
        options.Scope.Add("user:email");
    });

// 5) Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 6) Cookie events for session logging
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events = new CookieAuthenticationEvents
    {
        OnSignedIn = async ctx =>
        {
            var svc = ctx.HttpContext.RequestServices.GetRequiredService<ISessionService>();
            await svc.CreateSessionAsync(ctx.Principal);
        },
        OnSigningOut = async ctx =>
        {
            var svc = ctx.HttpContext.RequestServices.GetRequiredService<ISessionService>();
            await svc.EndSessionAsync(ctx.HttpContext.User);
        }
    };
});

// 7) Admin filter
builder.Services.AddScoped<ProjektSezon2.Filters.AdminFilter>();

// 8) Cookie consent
builder.Services.AddScoped<ICookieConsentService, CookieConsentService>();

// 9) Controllers and views
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 10) Stripe API key
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// 11) Ensure DB is migrated & seed roles
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var rm = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await db.Database.MigrateAsync();

    var roles = new[] { "User", "Admin" };
    foreach (var role in roles)
    {
        if (!await rm.RoleExistsAsync(role))
            await rm.CreateAsync(new IdentityRole(role));
    }
}

// 12) Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// 13) Map API controllers
app.MapControllers();


// 14) Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Clients}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
