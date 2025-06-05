using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add CORS policy for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // React default port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Hello, World!");
    Console.WriteLine("Running!");
    Console.WriteLine(app);
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
});