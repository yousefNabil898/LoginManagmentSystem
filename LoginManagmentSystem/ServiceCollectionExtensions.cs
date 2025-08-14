using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.BusinessLogic.Profiles;
using System.BusinessLogic.Services.Attachmentservices;
using System.BusinessLogic.Services.CompanyService;
using System.BusinessLogic.Services.EmailSevice;
using System.BusinessLogic.Services.Servicesmanager;
using System.DataAcesses.Data.Context;
using System.DataAcesses.Data.Repositories.Company;
using System.DataAcesses.Exceptions;
using System.DataAcesses.Models;
using System.Text;

namespace LoginManagmentSystem.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<SystemContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Identity
            services.AddIdentity<Company, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<SystemContext>()
            .AddDefaultTokenProviders();

            // JWT authentication
            var key = configuration["Jwt:Key"];
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
                };
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            // Application services & repositories
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IServiceManager, ServiceManager>();

            // AutoMapper
            services.AddAutoMapper(cfg => { }, typeof(CompanyProfile).Assembly);

            // Validation responses
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(m => m.Value.Errors.Any())
                        .Select(m => new ValidationError
                        {
                            Field = m.Key,
                            Errors = m.Value.Errors.Select(e => e.ErrorMessage)
                        });

                    var response = new ValidationErrorToReturn { ValidationErrors = errors };
                    return new BadRequestObjectResult(response);
                };
            });
            services.Configure<SmtpSettings>(
             configuration.GetSection("SmtpSettings"));
            return services;
        }
    }
}
