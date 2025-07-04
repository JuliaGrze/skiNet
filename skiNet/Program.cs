using API.Middleware;
using API.SignalR;
using CORE.Entities;
using CORE.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddControllers(); //Register Controllers
//Connection with database
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Reposiotry Services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
//Generic Reposiotry Service
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//Unit of work inject
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Cors
builder.Services.AddCors();

//Connection with Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var connString = builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("Cannot get redis connection string");
    var configuration = ConfigurationOptions.Parse(connString, true);
    return ConnectionMultiplexer.Connect(configuration);
});
//Cart Service
builder.Services.AddSingleton<ICartService, CartService>();

//Identity
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<StoreContext>();

//Payment Service
builder.Services.AddScoped<IPaymentService, PaymentService>();

//SignalR - WebSignal to DI
builder.Services.AddSignalR();

var app = builder.Build();

//rejestruje Twoj middleware obslugujacy bledy w potoku HTTP
app.UseMiddleware<ExceptionMiddleware>();

//Cors
app.UseCors(x => x.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("http://localhost:4200", "https://localhost:4200")
                .AllowCredentials()); //allow to cookies from client

//Midleware
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

//Identity
app.MapGroup("api").MapIdentityApi<AppUser>(); //api/login

//Map endpoint for Hub
app.MapHub<NotificationHub>("/hub/notifications");

//Seed data
try
{
    // Tworzy nowy "zakres us³ug" – czyli coœ w rodzaju sztucznego ¿¹dania HTTP
    // Dlaczego to wa¿ne?
    // StoreContext ma lifetime Scoped, czyli dzia³a per request — wiêc nie mo¿esz u¿yæ go bezpoœrednio poza kontrolerem
    using var scopre = app.Services.CreateScope();

    // Pobierasz dostêp do wszystkich zarejestrowanych us³ug w kontenerze DI w tym zakresie. To z tego services bêdziesz braæ np.StoreContext
    var services = scopre.ServiceProvider;

    // Wstrzykujesz sobie StoreContext z kontenera DI
    var context = services.GetRequiredService<StoreContext>();

    // Automatycznie tworzy lub aktualizuje bazê danych na podstawie migracji.
    await context.Database.MigrateAsync();

    //Uruchamia Twój kod seeduj¹cy
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

app.Run();
