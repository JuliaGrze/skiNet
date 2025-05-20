using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddControllers(); //Register Controllers
//Connection with database
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

//Midleware

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "Hello World! eee");

app.Run();
