using Microsoft.EntityFrameworkCore;
using Reply.ContactManagement.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddDbContext<ContactManagementDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionString")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
