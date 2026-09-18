using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Myschools.Api.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

[CompilerGenerated]
internal class Program
{
	private static async Task Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(delegate(SwaggerGenOptions options)
		{
			options.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Myschools API",
				Version = "v1",
				Description = "HTTP API for the Myschools school management system."
			});
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "Enter the JWT returned by POST /api/auth/login."
			});
			options.AddSecurityRequirement(new OpenApiSecurityRequirement { [new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			}] = Array.Empty<string>() });
		});
		builder.Services.AddSingleton<SqlDatabase>();
		builder.Services.AddSingleton<LegacyPasswordCipher>();
		builder.Services.AddHttpClient();
		builder.Services.AddCors(delegate(CorsOptions options)
		{
			options.AddPolicy("Frontend", delegate(CorsPolicyBuilder policy)
			{
				policy.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? new string[1] { "http://localhost:3000" }).AllowAnyHeader().AllowAnyMethod();
			});
		});
		string jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
		builder.Services.AddAuthentication("Bearer").AddJwtBearer(delegate(JwtBearerOptions options)
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.FromMinutes(1.0)
			};
		});
		builder.Services.AddAuthorization();
		WebApplication app = builder.Build();
		await app.Services.GetRequiredService<SqlDatabase>().EnsurePasswordResetTokensTableAsync();
		await app.Services.GetRequiredService<SqlDatabase>().EnsureFailedLoginAttemptsTableAsync();
		app.UseSwagger();
		app.UseSwaggerUI(delegate(SwaggerUIOptions options)
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "Myschools API v1");
			options.RoutePrefix = "swagger";
		});
		app.UseHttpsRedirection();
		app.UseCors("Frontend");
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
		app.Run();
	}
}
