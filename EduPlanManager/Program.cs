using EduPlanManager.Data;
using EduPlanManager.Extensions;
using EduPlanManager.Mappings;
using EduPlanManager.Models.Entities;
using EduPlanManager.Services;
using EduPlanManager.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EduPlanManager.UnitOfWork;
using EduPlanManager.Filters;
using CloudinaryDotNet;
using EduPlanManager.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login"; 
    options.AccessDeniedPath = "/auth/access-denied";
    options.LogoutPath = "/auth/logout";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; 
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});
//Cloudinary 
var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
var account = new Account(
       cloudinarySettings?.CloudName,
       cloudinarySettings?.ApiKey,
       cloudinarySettings?.ApiSecret
   );
Cloudinary cloudinary = new Cloudinary(account);
builder.Services.AddSingleton(cloudinary);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubjectCategoryService, SubjectCategoryService>();
builder.Services.AddScoped<IAcademicTermService, AcademicTermService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddScoped<MyAuthenFilter>();
var app = builder.Build();
app.SeedAdminUser();
if (!app.Environment.IsDevelopment())
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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
