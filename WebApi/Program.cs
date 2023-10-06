using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebAPi.DAL;
using WebAPi.DAL.Repositories.Implemetions;
using WebAPi.DAL.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();


var connection = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection, builder => builder.MigrationsAssembly("WebApi")));
var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
 
}



using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
    dbContext.EnsureRolesCreated();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
