using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using ALLInOne.Filter;
using ALLInOne.TestConfig;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ALLInOne
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
            services.AddControllersWithViews();

            services.AddMvcCore()
                .AddApiExplorer();

            /*
           * add by lhl 2020年10月10日18:08:27
           * Authentication 认证
           * Authorization 授权
           */
            //认证服务器地址  身份认证服务注册的DI
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:6006"; // IdentityServer服务器地址
                    //options.ApiName = "swagger_api"; // 用于针对进行身份验证的API资源的名称
                    options.RequireHttpsMetadata = true; // 指定是否为HTTPS
                });

            //发放Token的 服务器端（ID4) 本例：在同一台服务器上
            services.AddIdentityServer()
                .AddTestUsers(TestUsers.Users)
               // in-memory, code config
               .AddInMemoryIdentityResources(Config.IdentityResources)
               .AddInMemoryApiScopes(Config.ApiScopes)
               .AddInMemoryClients(Config.Clients)
            // not recommended for production - you need to store your key material somewhere secure
            .AddDeveloperSigningCredential();


            //将Swagger 注入到 DI 容器中去↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "俺的API", Version = "v1" });
                // 获取xml文件名
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // 获取xml文件路径
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 添加控制器层注释，true表示显示控制器注释
                c.IncludeXmlComments(xmlPath, true);

                c.OperationFilter<AddAuthHeaderOperationFilter>();
                //c.OperationFilter<AuthResponsesOperationFilter>(); 
                //c.OperationFilter<SwaggerHttpHeaderFilter>();


                // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
                            Scopes = new Dictionary<string, string> {
                                { "swagger_api", "swagger受保护的api" },//指定客户端请求的api作用域。 如果为空，则客户端无法访问
                                { "swagger_api2", "swagger受保护的api2" }//指定客户端请求的api作用域。 如果为空，则客户端无法访问
                            }
                        }
                    }
                });

                //每个方法后面是否 带锁，如果加上，所有的方法都有锁
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                //        },
                //        new[] { "swagger_api" }
                //    }
                //});

            });
            //将Swagger 注入到 DI 容器中去↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

                c.OAuthClientId("swagger_client");
                c.OAuthAppName("swagger_api 描述");

                //c.OAuthClientSecret("secret");
                //c.OAuthRealm("test-realm");
                //c.OAuthScopeSeparator(" ");
                //c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "foo", "bar" } });
                //c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });

            app.UseRouting();

            app.UseIdentityServer();

            //授权
            app.UseAuthorization();
            //认证
            app.UseAuthentication();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}