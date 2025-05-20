using System;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using AspNetCoreHero.ToastNotification;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Models;
using MyPhamSoul.Repository;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using MyPhamSoul.Services;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MyPhamSoul.Hubs;
using Microsoft.AspNetCore.HttpsPolicy;
using MyPhamSoul.Helpper;







var builder = WebApplication.CreateBuilder(args);

// Add services to the container.




var connectionString = builder.Configuration.GetConnectionString("_2025MyPhamContext");
builder.Services.AddDbContext<_2025MyPhamContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });
builder.Services.AddSignalR();
builder.Services.AddScoped<ILoaiRepository, LoaiRepository>();
builder.Services.AddScoped<IHangRepository, HangRepository>();
builder.Services.AddSession(options =>
{
   
    options.Cookie.Name = ".MyPhamSoul.Session"; // Tên của Cookie Session
    options.Cookie.IsEssential = true; // Đảm bảo rằng Cookie này cần thiết
});
builder.Services.AddScoped<ICTLoaiRepository, CTLoaiRepository>();

builder.Services.AddSingleton<IVnPayService, VnPayService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddTransient<IMailService, MailService>();

builder.Services.AddSingleton(x =>
    new PaypalClient(
        builder.Configuration["PayPalOptions:AppId"],
        builder.Configuration["PayPalOptions:AppSecret"],
        builder.Configuration["PayPalOptions:Mode"]

    )
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    
    options.ReturnUrlParameter = "http://localhost:5096/signin-google";
}
    )
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
});

// Thêm dịch vụ Identity

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();


builder.Services.AddRazorPages().AddViewOptions(options =>
{
    options.HtmlHelperOptions.ClientValidationEnabled = true;
});

builder.Services.AddHttpClient<MyPhamSoul.Services.GhnShippingService>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Ghn:ApiBaseUrl"]);
    c.DefaultRequestHeaders.Add("Token", builder.Configuration["Ghn:ApiKey"]);
});

builder.Services.AddScoped<MyPhamSoul.Services.IShippingService, MyPhamSoul.Services.GhnShippingService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapHub<ConnectedHub>("/ConnectedHub");
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapControllerRoute(
    name: "sanphamtheodanhmuc",
    pattern: "sanpham/sanphamtheodanhmuc/{maDanhMuc}",
    defaults: new { controller = "SanPham", action = "SanPhamTheoDanhMuc" });

app.MapControllerRoute(
    name: "filter",
    pattern: "home/filter",
    defaults: new { controller = "Home", action = "Filter" });

app.Run();
