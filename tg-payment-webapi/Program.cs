using Core.Services.Payment;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// CORS
var PaymentAllowSpecificOrigins = "_paymentAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(PaymentAllowSpecificOrigins,
        builder =>
        {
            builder
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Services
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Repos
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Add Mapster Mapping
var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
typeAdapterConfig.Scan(Assembly.GetAssembly(typeof(PaymentToPaymentDtoRegister)));
// register the mapper as Singleton service for my application
var mapperConfig = new Mapper(typeAdapterConfig);
builder.Services.AddSingleton<IMapper>(mapperConfig);

// //DbContext
// var sqlstring = "";
// if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DBSTRING")))
//     sqlstring = builder.Configuration.GetConnectionString("SqlServer");
// else
//     sqlstring = Environment.GetEnvironmentVariable("DBSTRING");
// ;
// builder.Services.AddDbContext<PaymentDbContext>(options =>
//     options.UseSqlServer(sqlstring)
// );

// DbContext
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// Add Controllers
builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // ignore omitted parameters on models to enable optional params (e.g. User update)
    //x.JsonSerializerOptions.IgnoreNullValues = true;
});
;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
app.UseCors(PaymentAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// // check connection
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetService<PaymentDbContext>();
//
//     SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(sqlstring);
//     sqlConnectionStringBuilder.InitialCatalog = "master";
//
//     context.Database.SetConnectionString(sqlConnectionStringBuilder.ConnectionString);
//
//
//     Console.WriteLine("Waiting for DB connection...");
//
//     while (!context.Database.CanConnect())
//     {
//         int milliseconds = 2000;
//         Thread.Sleep(milliseconds);
//         // we need to wait, since we need to run migrations
//     }
//
//     Console.WriteLine("DB connected");
//
//     context.Database.SetConnectionString(sqlstring);
// }

app.Run();