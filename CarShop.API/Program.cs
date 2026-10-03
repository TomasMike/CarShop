using System.Threading.Tasks;
using CarShop.Context;
using CarShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Reflection.Metadata;

public partial class Program
{
    private static async Task Main(string[] args)
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
          .AddRoles<IdentityRole>()
          .AddEntityFrameworkStores<AppDbContext>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await context.Database.MigrateAsync();

            // Seed cars
            if (!context.Cars.Any())
            {
                var toyota = new CarBrand { Id = 1, Name = "Toyota" };
                var tesla = new CarBrand { Id = 2, Name = "Tesla" };

                context.Cars.AddRange(
                    new Car { CarBrand = toyota, Model = "RAV4", Year = 2023, Price = 32500m, Color = "Gray", IsAvailable = true },
                    new Car { CarBrand = tesla, Model = "Model 3", Year = 2024, Price = 39990m, Color = "White", IsAvailable = true }
                );

                await context.SaveChangesAsync();
            }



            var roleExists = await roleManager.RoleExistsAsync("Customer");
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }

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


        //add customer role to new registered users
        app.MapIdentityApi<IdentityUser>().AddEndpointFilter(async (context, next) =>
        {
            var request = context.Arguments.OfType<RegisterRequest>().FirstOrDefault();

            var result = await next(context);

            if (request is not null && context.HttpContext.Response.StatusCode == StatusCodes.Status200OK)
            {
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                var user = await userManager.FindByEmailAsync(request.Email);
                if (user != null)
                    await userManager.AddToRoleAsync(user, "Customer");
            }

            return result;
        });
        app.UseHttpsRedirection();
        app.MapControllers();


        app.Run();
    }
}
