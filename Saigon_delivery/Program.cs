using Microsoft.EntityFrameworkCore;
using Saigon_delivery.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SaigonDeliveryContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SaigonDeliveryDB")));
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SaigonDeliveryContext>();

    var pendingUsers = db.Users.Where(u => u.PasswordHash == "pending_hash").ToList();
    if (pendingUsers.Any())
    {
        var passwordMap = new Dictionary<string, string>
        {
            { "admin@saigon.vn",      "Admin@123" },
            { "khachhang1@saigon.vn", "Customer@123" },
            { "khachhang2@saigon.vn", "Customer@123" },
            { "shipper1@saigon.vn",   "Shipper@123" },
            { "shipper2@saigon.vn",   "Shipper@123" },
        };

        foreach (var user in pendingUsers)
        {
            if (passwordMap.TryGetValue(user.Email, out var rawPassword))
            {
                var bytes = System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(rawPassword));
                user.PasswordHash = Convert.ToHexString(bytes).ToLower();
            }
        }

        db.SaveChanges();
    }
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
