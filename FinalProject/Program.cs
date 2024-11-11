using FinalProject.Controllers.API;
using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Services;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddControllers();


builder.Services.AddDbContext<QlptContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhongTro"));
});


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IEmailSender, EmailService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Customer/SignIn";
    options.AccessDeniedPath = "/AccessDenied";
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<FavoriteListService>();
builder.Services.AddScoped<RoomService>();

builder.Services.AddScoped<RegisterService>();
builder.Services.AddScoped<BillService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddHttpClient<GeocodeController>();

//đăng ký PaypalClient dạng Singleton() - chỉ có 1 instance duy nhất trong toàn ứng dụng
builder.Services.AddSingleton(x => new PaypalClient(
    builder.Configuration["PaypalOptions:AppId"],
    builder.Configuration["PaypalOptions:AppSecret"],
    builder.Configuration["PaypalOptions:Mode"]
    ));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseCors("MyAllowSpecificOrigins");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
