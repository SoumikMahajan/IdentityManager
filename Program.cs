using IdentityManager;
using IdentityManager.Authorize;
using IdentityManager.Data;
using IdentityManager.Models;
using IdentityManager.Services;
using IdentityManager.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Config Dbcontext class
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Add Identity service to this project
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<INumberOfDaysForAccount, NumberOfDaysForAccount>();
builder.Services.AddScoped<IAuthorizationHandler, AdminWithOver1000DaysHandler>();
builder.Services.AddScoped<IAuthorizationHandler, FirstNameAuthHandler>();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.AccessDeniedPath = new PathString("/Account/NoAccess");
});

//change the default password requirment on register page
builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Lockout.MaxFailedAccessAttempts = 3;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(10000);
    opt.SignIn.RequireConfirmedEmail = false;
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin", policy => policy.RequireRole(SD.Admin));
    opt.AddPolicy("AdminAndUser", policy => policy.RequireRole(SD.Admin).RequireRole(SD.User));
    opt.AddPolicy("AdminRole_CreateClaim", policy => policy.RequireRole(SD.Admin).RequireClaim("create", "True"));
    opt.AddPolicy("AdminRole_CreateEditDeleteClaim", policy => policy
                    .RequireRole(SD.Admin)
                    .RequireClaim("create", "True")
                    .RequireClaim("edit", "True")
                    .RequireClaim("delete", "True")
                 );

    opt.AddPolicy("AdminRole_CreateEditDeleteClaim_ORSuperAdminRole", policy => policy.RequireAssertion(context =>
        AdminRole_CreateEditDeleteClaim_ORSuperAdminRole(context)



        ));
    opt.AddPolicy("OnlySuperAdminChecker", p => p.Requirements.Add(new OnlySuperAdminChecker()));

    opt.AddPolicy("AdminWithMoreThan1000Days", p => p.Requirements.Add(new AdminWithMoreThan1000DaysRequirement(1000)));
    opt.AddPolicy("FirstNameAuth", p => p.Requirements.Add(new FirstNameAuthRequirement("test")));
});

//Microsoft login
//6H~8Q~g__z7L6qKX4LgmIM0vTADpAGjguBl4Sa2m
//c99a1b88-78cb-4ebd-8af8-bfffe332c1ab
//5e671212-a215-4ba5-a744-acc1d4fef1a9
builder.Services.AddAuthentication().AddMicrosoftAccount(opt =>
{
    opt.ClientId = "5e671212-a215-4ba5-a744-acc1d4fef1a9";
    opt.ClientSecret = "6H~8Q~g__z7L6qKX4LgmIM0vTADpAGjguBl4Sa2m";
});
builder.Services.AddAuthentication().AddFacebook(opt =>
{
    opt.ClientId = "1095266281920012";
    opt.ClientSecret = "4a24fa632188baf5bc9a31617b87ee58";
});

var app = builder.Build();

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

//Add Authentication
app.UseAuthorization();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

bool AdminRole_CreateEditDeleteClaim_ORSuperAdminRole(AuthorizationHandlerContext context)
{
    return (
        context.User.IsInRole(SD.Admin) && context.User.HasClaim(c => c.Type == "Create" && c.Value == "True")
        && context.User.HasClaim(c => c.Type == "Edit" && c.Value == "True")
        && context.User.HasClaim(c => c.Type == "Delete" && c.Value == "True")
    )
    || context.User.IsInRole(SD.SuperAdmin);
}
