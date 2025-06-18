using APIlady.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(
    options => options.SuppressModelStateInvalidFilter = true);


builder.Services.AddDbContext<LadyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Lady")));

builder.Services.AddCors(options => options.AddPolicy("LadyPolicy",
    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LadyContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = "aboellil",
        ValidateAudience = true,
        ValidAudience = "mohamed",
        IssuerSigningKey = 
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("asldfjbaskjfvb@#$4584545kasjfbfvdfslkasfl")),
        ClockSkew = TimeSpan.Zero,
    };

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors("LadyPolicy");


app.UseHttpsRedirection();


app.MapControllers();

app.Run();
