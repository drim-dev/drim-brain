using ResourcesLab.Features;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();
app.UseUrlProcessing();
app.UseMimeTypes();

app.Run();
