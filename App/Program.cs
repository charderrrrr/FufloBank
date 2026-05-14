using App.UI.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=localhost;Database=fuflobank;Username=postgres;Password=pass123";

builder.Services.AddSingleton(new SessionManager(connectionString));
builder.Services.AddControllers();

var app = builder.Build();

app.Urls.Add("http://localhost:5002");

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();