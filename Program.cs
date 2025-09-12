using System.Text;
using ApiEcommerce.Models;
using ApiEcommerce1.Constants;
using ApiEcommerce1.Repository;
using ApiEcommerce1.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));

builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1mb
    options.UseCaseSensitivePaths = true;
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(cfg =>
{
  cfg.AddMaps(typeof(Program).Assembly);
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey)) {
    throw new InvalidOperationException("SecretKey not configured");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // for production must be true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // validate token sign with our key
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddControllers(option =>
{
  option.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);
  option.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
}
  
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
     options =>
  {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
      Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                    "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                    "Ejemplo: \"12345abcdef\"",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.Http,
      Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          },
          Scheme = "oauth2",
          Name = "Bearer",
          In = ParameterLocation.Header
        },
        new List<string>()
      }
    });
    options.SwaggerDoc("v1", new OpenApiInfo // documentation for v1
    {
      Version = "v1",
      Title = "API Ecommerce",
      Description = "API for Ecommerce",
      TermsOfService = new Uri("http://example.com/terms"),
      Contact = new OpenApiContact //optional
      {
        Name = "Guido",
        Url = new Uri("https://guido.com")
      },
      License = new OpenApiLicense //optional
      {
        Name = "Use license",
        Url = new Uri("https://guido.com/license")
      }
    });
    options.SwaggerDoc("v2", new OpenApiInfo // documentation for v2
    {
      Version = "v2",
      Title = "API Ecommerce v2",
      Description = "API for Ecommerce",
      TermsOfService = new Uri("http://example.com/terms"),
      Contact = new OpenApiContact //optional
      {
        Name = "Guido",
        Url = new Uri("https://guido.com")
      },
      License = new OpenApiLicense //optional
      {
        Name = "Use license",
        Url = new Uri("https://guido.com/license")
      }
    });
  }
);

var apiVersioninBuilder = builder.Services.AddApiVersioning(option =>
{
  option.AssumeDefaultVersionWhenUnspecified = true;
  option.DefaultApiVersion = new ApiVersion(1, 0);
  option.ReportApiVersions = true;
  // option.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version"));
});
apiVersioninBuilder.AddApiExplorer(option =>
{
  option.GroupNameFormat = "'v'VVV"; //v1, v2, v3, etc
  option.SubstituteApiVersionInUrl = true; // api/v{version}/products
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(PolicyNames.AllowSpecificOrigin,
    builder =>
    {
        // "*" allows any url
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
      options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    });
}

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin); // applying cors

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
