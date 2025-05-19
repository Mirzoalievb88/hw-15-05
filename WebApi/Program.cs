using Infrastructure.Services;
using Infrastructure.Interfaces;
using Domain.Entities;
using Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IBooksService, BooksService>();

// Add services to the container.

builder.Services.AddScoped<DataContext>();

builder.Services.AddScoped<BooksService>();      
builder.Services.AddScoped<BorrowingsService>();      
builder.Services.AddScoped<MemberService>();      


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My App"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();