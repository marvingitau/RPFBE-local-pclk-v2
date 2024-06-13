using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RPFBE.Auth;
using RPFBE.Model;
using RPFBE.Model.Repository;
using RPFBE.Service.DataUpload;
using RPFBE.Service.ErevukaAPI;
using RPFBE.Service.ExtServs;
using RPFBE.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFBE
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors();
          services.AddCors(options => options.AddPolicy("CorsPolicy",
          builder =>
          {
              builder
              .WithOrigins(new[] { "http://localhost:3000", "https://www.jobsite.com:3001/" })
              .AllowAnyMethod()
              .AllowAnyHeader();
                   //.AllowCredentials();
               }));

            


            services.AddControllers();
            // For Entity Framework  
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RPFBE")));

            // For Identity  
            //services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            //{
            //    // Default Password settings.
            //    options.Password.RequireDigit = false;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequiredUniqueChars = 1;
            //    options.User.RequireUniqueEmail = true;
            //})
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            //Custome Identity Error Describer
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
                //Token shorter one
                //options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.PasswordResetTokenProvider = ResetPasswordTokenProvider.ProviderKey;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<ResetPasswordTokenProvider>(ResetPasswordTokenProvider.ProviderKey)
                .AddErrorDescriber<CustomIdentityErrorDescriber>();

            // Adding Authentication  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
                options.Events = new JwtBearerEvents()
                {
                    //on any incoming messages check for custom prefix
                    OnMessageReceived = context =>
                    {
                        //grab current auhorization header from the current httpcontext
                        string authorization = context.Request.Headers["Authorization"];

                        // If no authorization header found, nothing to process further
                        if (string.IsNullOrEmpty(authorization))
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }

                        //check if it starts with prefix "Ss" 
                        if (authorization.StartsWith("Bearer Ss", StringComparison.OrdinalIgnoreCase))
                        {
                            
                            context.Token = DecodeToken(authorization.Substring("Bearer Ss".Length).Trim());
                        }


                        // If no token found, authentication failed.
                        if (string.IsNullOrEmpty(context.Token))
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
                    }
                };
            });


            //Nav Service options
            services.Configure<WebserviceCreds>(Configuration.GetSection("NAVDetails"));
           
            services.AddScoped<ICodeUnitWebService, CodeUnitWebService>();


            //Mail runtime setting instance
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();

            services.AddScoped<ICSVService, CSVService>();
            services.AddScoped<IAESc, AESc>();

            services.AddSingleton<ILMSService, LMSService>();
            //Adding instance of httclient
            services.AddHttpClient("ErevukaLMSApi", c => c.BaseAddress = new Uri(Configuration["LMS:BaseUrl"]));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            //app.UseCors(options => options.AllowCredentials().AllowAnyHeader().AllowAnyMethod().WithOrigins(new[] { "http://localhost:3000" }));
            //app.UseCors(options => options.AllowAnyOrigin()
            //                                .AllowAnyMethod()
            //                                .AllowAnyHeader()); 
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

         

            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            loggerFactory.AddFile($@"{Directory.GetCurrentDirectory()}\Logs\Log.txt");
        }

        public string DecodeToken(string token)
        {

            int pPstn = token.IndexOf('.') + 3;
            return token.Remove(pPstn, 1);
        }
    }
}
