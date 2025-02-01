using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySuperDuperPetProject.Middle;
using MySuperDuperPetProject.Middlewares;
using MySuperDuperPetProject.TransferDatabaseContext;

namespace MySuperDuperPetProject
{
    public class Startup(IConfiguration config)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITransferLogic, TransferLogic>();
            services.AddScoped<UsersLogic>();
            services.AddDbContext<TransferDbContext>(d => d.UseNpgsql(config.GetConnectionString("MyConnection")), ServiceLifetime.Scoped, ServiceLifetime.Scoped);

            services.AddSingleton<CheckUserSessionMiddleware>();
            services.AddSingleton<ClaimsCheckMiddleware>();

            services.AddMemoryCache();

            services.AddCors();
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = config["JwtIssuer"],
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(SecretKeyStorage.SecretKey)),
                    ClockSkew = TimeSpan.Zero // remove delay of token when expire
                };
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Scheme = "bearer",
                    BearerFormat = "jwt-bearer",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                 });
            });
            services.AddRouting(urls => urls.LowercaseUrls = true);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseSwagger();
            app.UseSwaggerUI();

            app.AddClaimsCheckMiddleware();
            app.AddCheckUserSessionMiddleware();
            


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}