using TouristAgencyAPI.Data;
using TouristAgencyAPI.Repositories;
using TouristAgencyShared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tourist Agency API",
        Version = "v1",
        Description = "A .NET Core 8 Minimal API for tourist agency management with CRUD operations for destinations, customers, guides, tours, and bookings.",
        Contact = new OpenApiContact
        {
            Name = "Tourist Agency API",
            Email = "support@touristagency.com"
        }
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register database context
builder.Services.AddSingleton<DatabaseContext>();

// Register repositories
builder.Services.AddScoped<IRepository<Destination>, DestinationRepository>();
builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();
builder.Services.AddScoped<IRepository<Guide>, GuideRepository>();
builder.Services.AddScoped<IRepository<Tour>, TourRepository>();
builder.Services.AddScoped<IRepository<Booking>, BookingRepository>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tourist Agency API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at root URL
    });
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// API Endpoints

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "API Gateway", Timestamp = DateTime.UtcNow }))
.WithName("HealthCheck")
.WithOpenApi();

// Destinations endpoints
app.MapGet("/api/destinations", async (IRepository<Destination> repository) =>
{
    var destinations = await repository.GetAllAsync();
    return Results.Ok(destinations);
})
.WithName("GetDestinations")
.WithOpenApi();

app.MapGet("/api/destinations/{id}", async (int id, IRepository<Destination> repository) =>
{
    var destination = await repository.GetByIdAsync(id);
    return destination is null ? Results.NotFound() : Results.Ok(destination);
})
.WithName("GetDestination")
.WithOpenApi();

app.MapPost("/api/destinations", async (Destination destination, IRepository<Destination> repository) =>
{
    var id = await repository.CreateAsync(destination);
    destination.Id = id;
    return Results.Created($"/api/destinations/{id}", destination);
})
.WithName("CreateDestination")
.WithOpenApi();

app.MapPut("/api/destinations/{id}", async (int id, Destination destination, IRepository<Destination> repository) =>
{
    destination.Id = id;
    var success = await repository.UpdateAsync(destination);
    return success ? Results.Ok(destination) : Results.NotFound();
})
.WithName("UpdateDestination")
.WithOpenApi();

app.MapDelete("/api/destinations/{id}", async (int id, IRepository<Destination> repository) =>
{
    var success = await repository.DeleteAsync(id);
    return success ? Results.Ok() : Results.NotFound();
})
.WithName("DeleteDestination")
.WithOpenApi();

// Customers endpoints
app.MapGet("/api/customers", async (IRepository<Customer> repository) =>
{
    var customers = await repository.GetAllAsync();
    return Results.Ok(customers);
})
.WithName("GetCustomers")
.WithOpenApi();

app.MapGet("/api/customers/{id}", async (int id, IRepository<Customer> repository) =>
{
    var customer = await repository.GetByIdAsync(id);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
})
.WithName("GetCustomer")
.WithOpenApi();

app.MapPost("/api/customers", async (Customer customer, IRepository<Customer> repository) =>
{
    var id = await repository.CreateAsync(customer);
    customer.Id = id;
    return Results.Created($"/api/customers/{id}", customer);
})
.WithName("CreateCustomer")
.WithOpenApi();

app.MapPut("/api/customers/{id}", async (int id, Customer customer, IRepository<Customer> repository) =>
{
    customer.Id = id;
    var success = await repository.UpdateAsync(customer);
    return success ? Results.Ok(customer) : Results.NotFound();
})
.WithName("UpdateCustomer")
.WithOpenApi();

app.MapDelete("/api/customers/{id}", async (int id, IRepository<Customer> repository) =>
{
    var success = await repository.DeleteAsync(id);
    return success ? Results.Ok() : Results.NotFound();
})
.WithName("DeleteCustomer")
.WithOpenApi();

// Guides endpoints
app.MapGet("/api/guides", async (IRepository<Guide> repository) =>
{
    var guides = await repository.GetAllAsync();
    return Results.Ok(guides);
})
.WithName("GetGuides")
.WithOpenApi();

app.MapGet("/api/guides/{id}", async (int id, IRepository<Guide> repository) =>
{
    var guide = await repository.GetByIdAsync(id);
    return guide is null ? Results.NotFound() : Results.Ok(guide);
})
.WithName("GetGuide")
.WithOpenApi();

app.MapPost("/api/guides", async (Guide guide, IRepository<Guide> repository) =>
{
    var id = await repository.CreateAsync(guide);
    guide.Id = id;
    return Results.Created($"/api/guides/{id}", guide);
})
.WithName("CreateGuide")
.WithOpenApi();

app.MapPut("/api/guides/{id}", async (int id, Guide guide, IRepository<Guide> repository) =>
{
    guide.Id = id;
    var success = await repository.UpdateAsync(guide);
    return success ? Results.Ok(guide) : Results.NotFound();
})
.WithName("UpdateGuide")
.WithOpenApi();

app.MapDelete("/api/guides/{id}", async (int id, IRepository<Guide> repository) =>
{
    var success = await repository.DeleteAsync(id);
    return success ? Results.Ok() : Results.NotFound();
})
.WithName("DeleteGuide")
.WithOpenApi();

// Tours endpoints
app.MapGet("/api/tours", async (IRepository<Tour> repository) =>
{
    var tours = await repository.GetAllAsync();
    return Results.Ok(tours);
})
.WithName("GetTours")
.WithOpenApi();

app.MapGet("/api/tours/{id}", async (int id, IRepository<Tour> repository) =>
{
    var tour = await repository.GetByIdAsync(id);
    return tour is null ? Results.NotFound() : Results.Ok(tour);
})
.WithName("GetTour")
.WithOpenApi();

app.MapPost("/api/tours", async (Tour tour, IRepository<Tour> repository) =>
{
    var id = await repository.CreateAsync(tour);
    tour.Id = id;
    return Results.Created($"/api/tours/{id}", tour);
})
.WithName("CreateTour")
.WithOpenApi();

app.MapPut("/api/tours/{id}", async (int id, Tour tour, IRepository<Tour> repository) =>
{
    tour.Id = id;
    var success = await repository.UpdateAsync(tour);
    return success ? Results.Ok(tour) : Results.NotFound();
})
.WithName("UpdateTour")
.WithOpenApi();

app.MapDelete("/api/tours/{id}", async (int id, IRepository<Tour> repository) =>
{
    var success = await repository.DeleteAsync(id);
    return success ? Results.Ok() : Results.NotFound();
})
.WithName("DeleteTour")
.WithOpenApi();

// Bookings endpoints
app.MapGet("/api/bookings", async (IRepository<Booking> repository) =>
{
    var bookings = await repository.GetAllAsync();
    return Results.Ok(bookings);
})
.WithName("GetBookings")
.WithOpenApi();

app.MapGet("/api/bookings/{id}", async (int id, IRepository<Booking> repository) =>
{
    var booking = await repository.GetByIdAsync(id);
    return booking is null ? Results.NotFound() : Results.Ok(booking);
})
.WithName("GetBooking")
.WithOpenApi();

app.MapPost("/api/bookings", async (Booking booking, IRepository<Booking> repository) =>
{
    var id = await repository.CreateAsync(booking);
    booking.Id = id;
    return Results.Created($"/api/bookings/{id}", booking);
})
.WithName("CreateBooking")
.WithOpenApi();

app.MapPut("/api/bookings/{id}", async (int id, Booking booking, IRepository<Booking> repository) =>
{
    booking.Id = id;
    var success = await repository.UpdateAsync(booking);
    return success ? Results.Ok(booking) : Results.NotFound();
})
.WithName("UpdateBooking")
.WithOpenApi();

app.MapDelete("/api/bookings/{id}", async (int id, IRepository<Booking> repository) =>
{
    var success = await repository.DeleteAsync(id);
    return success ? Results.Ok() : Results.NotFound();
})
.WithName("DeleteBooking")
.WithOpenApi();

app.Run();
