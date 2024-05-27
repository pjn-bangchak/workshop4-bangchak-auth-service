using Microsoft.EntityFrameworkCore;
using BangchakAuthService.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BangchakAuthService.Services.RabbitMQ;
using BGTAuthService.Consumers;
var builder = WebApplication.CreateBuilder(args);

// Add Cors Service
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin(); // policy.WithOrigins("https://codingthailand.com")
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
    });
});

var connectionString = builder.Configuration.GetConnectionString("BangchakAuthServiceIdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'BangchakAuthServiceIdentityDbContextConnection' not found.");
builder.Services.AddDbContext<BangchakAuthServiceIdentityDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
.AddEntityFrameworkStores<BangchakAuthServiceIdentityDbContext>();

// Add services to the container.
// add Jwt service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                config =>
                {
                    //config.SaveToken = true;

                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT_KEY").Value!)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero
                    };

                    
                }
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//custom service
builder.Services.AddSingleton<IRabbitMQConnectionManager, RabbitMQConnectionManager>();

// add rabbitmq consumers
builder.Services.AddHostedService<ErrorConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
