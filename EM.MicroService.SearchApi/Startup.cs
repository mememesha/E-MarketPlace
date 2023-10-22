using EM.MicroService.SearchApi.Abstractions;
using EM.MicroService.SearchApi.Extensions;
using EM.MicroService.SearchApi.Options;
using Microsoft.Extensions.Caching.Distributed;

namespace EM.MicroService.SearchApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection("RedisSettings")["RedisApiUrl"];
                options.InstanceName = "SampleInstance";
            });

            // services.AddControllers().AddMvcOptions(x =>
            //     x.SuppressAsyncSuffixInActionNames = false);

            // services.AddHttpClient<IDistributedCache, DistributedCache>(client =>
            // {
            //     client.BaseAddress = new Uri(Configuration["RedisSettings:RedisApiUrl"]);
            //     client.Timeout = TimeSpan.FromMinutes(3);
            // }).ConfigurePrimaryHttpMessageHandler(() =>
            // {
            //     var clientHandler = new HttpClientHandler();
            //     clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            //     return clientHandler;
            // });

            services.AddControllers().AddNewtonsoftJson();
            services.AddOpenApiDocument(options =>
            {
                options.Title = "EM API Doc";
                options.Version = "1.0";
            });

            services.Configure<ElasticSearchOptions>(Configuration.GetSection("ElasticSearchOptions"));
            /*var is4Options = Configuration.GetSection("IS4").Get<IS4Options>();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                        options =>
                        {
                            options.Authority = is4Options.Uri;
                            options.Audience = is4Options.Audience;
                            
                            options.BackchannelHttpHandler = new HttpClientHandler
                            {
                                ServerCertificateCustomValidationCallback = delegate { return true; }
                            };
                            
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false
                            };
                        });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("write-access", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "webapi.write");
                });
            });*/

            services.AddCors(
                options => options.AddPolicy("AllowAll",
                    p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAll");

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseSessionCookie();
            app.UseCache();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}