using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);


#region Add services to the container

var mvcBuilder = builder.Services.AddRazorPages()
    .AddViewLocalization();
builder.AddAtomicLocalization(mvcBuilder);

var daprClient = new DaprClientBuilder().Build();
builder.Services.AddSingleton(daprClient);

#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

#endregion

app.Run();