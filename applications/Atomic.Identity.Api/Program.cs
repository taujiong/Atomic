var builder = WebApplication.CreateBuilder(args);

#region Add services to the container

builder.Services.AddControllers();
builder.ConfigureApiController();

#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline.

app.UseAtomicControllerPreset();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();