using EM.WebApi.Core.Extensions;
using EM.WebApi.Core.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using EM.Shared.Connections.Broker.RabbitMQ.Abstract;
using EM.Shared.Connections.Broker.RabbitMQ;
using EM.Shared.Connections.Broker.RabbitMQ.Model;

namespace EM.WebApi.Core
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
            services.AddControllers().AddMvcOptions(x=> 
                x.SuppressAsyncSuffixInActionNames = false);
            services.AddOpenApiDocument(options =>
            {
                options.Title = "EM API Doc";
                options.Version = "1.0";
            });
            
            services.Configure<ElasticsearchOptions>(Configuration.GetSection("ElasticsearchOptions"));
            var is4Options = Configuration.GetSection("IS4").Get<IS4Options>();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                        options =>
                        {
                            options.Authority = is4Options.Uri;
                            options.Audience = is4Options.Audience;
                            
                            // TODO AAAAAAAAAAAAAAAA будь проклят docker
                            // дает возможность web api воспринимать self-signed сертификаты
                            options.BackchannelHttpHandler = new HttpClientHandler
                            {
                                ServerCertificateCustomValidationCallback = delegate { return true; }
                            };
                            
                            // // длинная история - в общем токен от IS4 в Blazor приходят подписанными от localhost:10001
                            // // а web api стучится в IS4 по имени is4:10001
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
            });

            services.AddCors(
                options => options.AddPolicy("AllowAll", 
                    p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            
            services.Configure<IRabbitMqOptions>(Configuration.GetSection("RabbitMqOptions"));

            services.AddSingleton<IRabbitMqService, RabbitMqService>();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSessionCookie();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}