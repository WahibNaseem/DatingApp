using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace DatingApp.API
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
            //Connection string for sqllite
            //services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            //We can simply use AddIdentity but that helps in razor view which we are not using so thats
            // why I'll use AddIdentityCore and that will require some configuration
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>{
                    // Right now we want to use or own password which we were having that's
                    // why making these password configuration but it is not recommended
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;  
            });


            //This is all about builder related code which is required because above we used AddIdentityCore
            builder = new IdentityBuilder(builder.UserType, typeof(Role),builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            //Code to Authorize the User through policy rather the action method filter
            services.AddAuthorization(options => {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratorPhotoRole", policy => policy.RequireRole("Admin","Moderator"));
                options.AddPolicy("VIPOnly", policy => policy.RequireRole("VIP"));

            });


           
            services.AddMvc(options => {
                //Added this configuration to authorize user globally
                var policy= new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();

                    options.Filters.Add(new AuthorizeFilter(policy)); 
            }
            )
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                             .AddJsonOptions(opt =>
                             {
                                 opt.SerializerSettings.
                                 ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                             });
            services.AddCors();
            services.Configure<CloudinarySettings>(config: Configuration.GetSection("CloudinarySettings"));
            
            Mapper.Reset();
            services.AddAutoMapper();
            services.AddTransient<Seed>();
            // services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();
          
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                // app.UseHsts();
            }

            // app.UseHttpsRedirection();
            seeder.SeedUsers();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc(routes =>
                  routes.MapSpaFallbackRoute(
                      name:"spa-fallback",
                      defaults: new { controller = "Fallback", action = "Index"}
                  ));
        }
    }
}
