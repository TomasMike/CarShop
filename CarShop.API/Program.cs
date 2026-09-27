using CarShop.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AppDomain.CurrentDomain.SetData("DataDirectory", AppContext.BaseDirectory);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "CarShop API",
                Version = "v1",
                Description = "REST API pre správu a rezerváciu vozidiel v autosalóne."
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Zadaj JWT token v tvare: Bearer [tvoj_token]",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
           options.UseSqlite(connectionString));

        builder.Services.AddIdentityApiEndpoints<IdentityUser>()
          .AddEntityFrameworkStores<AppDbContext>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "CarShop API v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.MapIdentityApi<IdentityUser>();
        app.UseHttpsRedirection();
        app.MapControllers();

        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var endpointDataSources = app.Services.GetServices<EndpointDataSource>();

            foreach (var source in endpointDataSources)
            {
                foreach (var endpoint in source.Endpoints)
                {
                    // Vytiahneme informácie o routovaní (Route Pattern)
                    if (endpoint is RouteEndpoint routeEndpoint)
                    {
                        var httpMethods = endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods;
                        var methods = httpMethods != null ? string.Join(", ", httpMethods) : "ANY";

                        Console.WriteLine($"[{methods}] {routeEndpoint.RoutePattern.RawText}");
                    }
                }
            }
        });

        app.Run();
    }
}
