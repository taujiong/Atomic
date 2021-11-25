var builder = WebApplication.CreateBuilder(args);

#region Add services to the container

var mvcBuilder = builder.Services.AddControllers();
builder.AddAtomicLocalization(mvcBuilder);
builder.ConfigureApiController();

#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline.

app.UseAtomicControllerPreset();

app.UseHttpsRedirection();

app.UseAtomicLocalization();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();