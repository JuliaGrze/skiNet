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
    // Tworzy nowy "zakres us�ug" � czyli co� w rodzaju sztucznego ��dania HTTP
    // Dlaczego to wa�ne?
    // StoreContext ma lifetime Scoped, czyli dzia�a per request � wi�c nie mo�esz u�y� go bezpo�rednio poza kontrolerem
    using var scopre = app.Services.CreateScope();

    // Pobierasz dost�p do wszystkich zarejestrowanych us�ug w kontenerze DI w tym zakresie. To z tego services b�dziesz bra� np.StoreContext
    var services = scopre.ServiceProvider;

    // Wstrzykujesz sobie StoreContext z kontenera DI
    var context = services.GetRequiredService<StoreContext>();

    // Automatycznie tworzy lub aktualizuje baz� danych na podstawie migracji.
    await context.Database.MigrateAsync();

    //Uruchamia Tw�j kod seeduj�cy
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

app.Run();
