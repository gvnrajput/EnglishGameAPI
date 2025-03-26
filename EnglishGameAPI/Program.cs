using EnglishGameAPI.Services;

var builder = WebApplication.CreateBuilder(args);

//// Configure logging: Clear all logging providers and add only supported ones
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole(); // Enable console logging (adjust as needed)

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<IProfileDataService, JsonProfileDataService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Replace with your frontend origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
