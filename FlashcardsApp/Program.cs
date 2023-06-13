using FlashcardsApp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllersWithViews();

// Service CORS
string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
                        policy =>
                        {
                            policy.WithOrigins("https://localhost:44447")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .SetIsOriginAllowed(origin => true)
                                .AllowCredentials()
                                .Build();
                        });
});

// Configure Database
builder.Services.AddDbContext<FlashcardsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

// Configure JWS authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.
            GetBytes(builder.Configuration.GetSection("Authentication:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience= false,
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
