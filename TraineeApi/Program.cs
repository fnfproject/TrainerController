using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestionBankApi.Trainee.Interfaces;

using QuestionBankApi.Trainee.Services;
using TraineeApi.Models;
using TraineeApi.Services;
using UserService = TraineeApi.Services.UserService;

namespace TraineeApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure CORS
            builder.Services.AddCors(setUp =>
            {
                setUp.AddPolicy("cors", setUp =>
                {
                    setUp.AllowAnyHeader();
                    setUp.AllowAnyMethod();
                    setUp.AllowAnyOrigin();
                });
            });

            // Register ApplicationDbContext with the dependency injection container
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register JwtService
            builder.Services.AddSingleton<JwtService>(sp =>
                new JwtService(builder.Configuration));

            // Register TwoFactorAuthService
            builder.Services.AddSingleton<TwoFactorAuthService>();

            // Register IUserService
            builder.Services.AddScoped<IUserService, UserService>();

            

            // Add JWT authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
                    };
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("cors");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
