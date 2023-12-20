using MessagesLab.Features;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseShowHeaders();
app.UseShowBody();

app.Run();
