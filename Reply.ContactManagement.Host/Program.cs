using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reply.ContactManagement.API.Controllers;
using Reply.ContactManagement.API.Middleware;
using Reply.ContactManagement.Application.Extensions;
using Reply.ContactManagement.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddDbContext<ContactManagementDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionString")));
builder.Services.AddControllers()
    .AddApplicationPart(typeof(ContactsController).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<ContactsController>();
builder.Services.AddContactManagementApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
