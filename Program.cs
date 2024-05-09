using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BangchakAuthService.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BangchakAuthServiceIdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'BangchakAuthServiceIdentityDbContextConnection' not found.");

builder.Services.AddDbContext<BangchakAuthServiceIdentityDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
.AddEntityFrameworkStores<BangchakAuthServiceIdentityDbContext>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
