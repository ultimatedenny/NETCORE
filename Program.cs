using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Services;
using NETCORE.Classes;
using IconCaptcha;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
{
    var services = builder.Services;
    services.AddCors();
    services.AddControllers();
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<CookieHelper>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddHttpContextAccessor();
    services.AddSingleton<AppSettings>();
    services.AddTransient<CookieHelper>();
    services.AddScoped<Class>();
    services.AddIconCaptcha(builder.Configuration.GetSection("IconCaptcha"));
    services.Configure<IconCaptchaOptions>(options =>
    {
        // options.IconPath = "assets/icons";
    });
}
var app = builder.Build();
{
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
    app.UseMiddleware<JwtMiddleware>();
    app.MapControllers();
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(name: "default",pattern: "{controller=Home}/{action=Index}");
app.MapIconCaptcha("/iconcaptcha");
app.Run();
