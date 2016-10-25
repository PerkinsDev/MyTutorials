using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld
{
    // Now on Github
    // Test from Laptop --> Guthub --> Desktop
    public class Startup
    {
        // Store field at class level, otherwise can't use in ConfigureServices
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        // Startup can also have injected parameters in constructer (like controllers)
        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)   // Path of actual project - not webroot
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();  // This allows you to override config files based on environment

            // Store at class level in case needed in other parts of Startup. Let VS auto generate fields
            _config = builder.Build();     // Returms type IConfigurationRoot
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Figures out that _config is IApplicationRoot and this single instance (Singleton) will be supplied everywhere it is being used
            services.AddSingleton(_config); 
            // Check for environment to switch between a test method and a real method (Project props ->Debug -> Environment Variables
            if (_env.IsEnvironment("Development") || _env.IsEnvironment("Testing"))
            {
                // Add our own Services. AddScoped means we supply and interface and what class can fulfill it.
                // Scoped means it will create an instance of the service as it actually needs it and reuse for a single request
                services.AddScoped<IMailService, DebugMailSevice>();
            }
            else
            {
                    // Implement a real Mail Service Later
            }

            // Special Extension method to wire Context class and EF Interfaces
            // Context is now injectable in different parts of the project
            services.AddDbContext<WorldContext>();

            // Add the repository service (Scoped because it may be expensive to create context class so limit to one per request cycle)
            // Takes the interface and the class to use
            services.AddScoped<IWorldRepository, WorldRepository>();
            // Could have different classes for different scenarios   -- //services.AddScoped<IWorldRepository, MockWorldRepository>();

            // Add our geoLocation service as transient obj because it doesn't have any of it's own state
            // When we ask for it later we\'ll usually get our own copy of the service
            services.AddTransient<GeoCoordsService>();

            // Transient creates this everytime we need it. To inject it below in the Configure Method
            services.AddTransient<WorldContextSeedData>();

            // Where you configure options for passwords stronger/weaker/special chars etc
            services.AddIdentity<WorldUser, IdentityRole>(config =>
                {
                    config.User.RequireUniqueEmail = true;
                    config.Password.RequiredLength = 8;
                    config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";  // Send user to "" when not authenticated. can also use external or 2 factor
                    config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents  // event prop is a set of callbacks that we can handle while auth. is happening
                    {
                        OnRedirectToLogin = async ctx =>    // set to return a status code when api, instead of actual redirection
                        {
                            if (ctx.Request.Path.StartsWithSegments("/api") && //does uri start with /api?
                            ctx.Response.StatusCode == 200)  // only when code is 200 )ok)
                            {
                                ctx.Response.StatusCode = 401;
                            }
                            else
                            {
                                ctx.Response.Redirect(ctx.RedirectUri);
                            }
                            await Task.Yield();
                        }
                    };
                }).AddEntityFrameworkStores<WorldContext>();  // An object is returned and you can Configure Where the Identities are stored

            services.AddLogging();

            // Register all the Mvc services so the Configure method below enabling Mvc (app.UseMvc) can work. It needs services to run
            // Enable configuration of Json options. Lambda to change config objects properties. Serializer to ContractResolver use a Json method to adjust the case
            // Mvc has support for configuring itself
            services.AddMvc(config =>
                {
                    if (_env.IsProduction()) // Use ASPNETCORE_ENVIRONMENT=Production to set tast machines, staging, dev
                    { 
                    // adds filter. if you attempt to access through HTTP it will attempt to direct you to HTTPS
                    config.Filters.Add(new RequireHttpsAttribute());
                    }
                })
                .AddJsonOptions(config =>
                {
                    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // This code gets written and called every time a request comes in. Aka Middleware in other frameworks
        // Configure sets up what sort of code is going to handle requests as they come in
        // Every part of the web server is optional. Including simple cases like serving static files
        // Entire set of operations. Nothing is hidden or implicit. Replaces the global.asax. ORDER IS IMPORTANT
        // Interface IHostingEnvironment is provided by the system. Works more broadly than C#: #if DEBUG

        // List of things (middlewares) that are going to look at requests and possibly return responses
        // Must enable the dependancy. Enabled in project.json. 
        // MUST be in correct order (ie default file before static files)
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory factory,
            WorldContextSeedData seeder)
        {
            // lets errors go to output console
            factory.AddConsole();

            // protect exceptions details from non-dev machines. Knows a Dev machine by project properties. 
            // to change environment -- Props --> Debug --> Environment variables --> Value
            if (env.IsEnvironment("Development"))   // also set w/ env.IsEnvironment("Development")...
 
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);  // this log level is more verbose
            }
            else
            {
                factory.AddDebug(LogLevel.Error);
            }

            app.UseStaticFiles();

            // Tell our app to use EF Identity. This turns it "on" in our system
            app.UseIdentity();

            // Initialize Tries to match all field name of a source(model) and destination(return real object). creatmap does this on our config obj
            Mapper.Initialize(config =>
            {
                // Bi-directional mapping Reverse to do Entity to Model for safe return of ViewModel to API (after DB calls)
                config.CreateMap<TripViewModel, Trip>().ReverseMap();
                config.CreateMap<StopViewModel, Stop>().ReverseMap();
            });


            // Enable MVC 6: Opt into MVC 6. Need middleware to listen for specific routes that I am going to implement
            // MVC requires a numer of services, classes etc., so you must add services.AddMvc() to ConfigureServices
            // Need a way to tell which controller. Using a lambda
            app.UseMvc(config =>
            {
                // MapRoute method takes a pattern of URL witrh different options and map them to specific controllers
                config.MapRoute(
                    // Default Fallback route when no specific route is being used for a specific method
                    // here using a C# construct to see the different parts ie name: ckjgshjkg
                    name: "Default",
                    template: "{controller}/{action}/{id?}", // assume parts of the route. ? at end = optional part
                                                             // Default: Root path mapped dire3ctly to the Index method on the AppController in case nothing supplied
                    defaults: new { controller = "App", action = "Index" }  // defaults if nothing supplied Look for App and Index if not specified
                    );
            });

            // Can't make Congigure() an async call - becomes a synchronous
            // Trick is to use wait
            seeder.EnsureSeedData().Wait();
        }
    }
}




// don't need when using MVC6 to create our views (cshtml) only for regular html
//app.UseDefaultFiles();  //when looking in a dir, (root) it will look for index.html etc files

//app.Run(async (context) =>
//{
//    await context.Response.WriteAsync("Hello World!");
//});