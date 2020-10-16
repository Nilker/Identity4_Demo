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
           * add by lhl 2020��10��10��18:08:27
           * Authentication ��֤
           * Authorization ��Ȩ
           */
            //��֤��������ַ  �����֤����ע���DI
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:6006"; // IdentityServer��������ַ
                    //options.ApiName = "swagger_api"; // ������Խ��������֤��API��Դ������
                    options.RequireHttpsMetadata = true; // ָ���Ƿ�ΪHTTPS
                });

            //����Token�� �������ˣ�ID4) ��������ͬһ̨��������
            services.AddIdentityServer()
                .AddTestUsers(TestUsers.Users)
               // in-memory, code config
               .AddInMemoryIdentityResources(Config.IdentityResources)
               .AddInMemoryApiScopes(Config.ApiScopes)
               .AddInMemoryClients(Config.Clients)
            // not recommended for production - you need to store your key material somewhere secure
            .AddDeveloperSigningCredential();


            //��Swagger ע�뵽 DI ������ȥ����������������������������������������������������������������������
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "����API", Version = "v1" });
                // ��ȡxml�ļ���
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // ��ȡxml�ļ�·��
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ��ӿ�������ע�ͣ�true��ʾ��ʾ������ע��
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
                                { "swagger_api", "swagger�ܱ�����api" },//ָ���ͻ��������api������ ���Ϊ�գ���ͻ����޷�����
                                { "swagger_api2", "swagger�ܱ�����api2" }//ָ���ͻ��������api������ ���Ϊ�գ���ͻ����޷�����
                            }
                        }
                    }
                });

                //ÿ�����������Ƿ� ������������ϣ����еķ���������
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
            //��Swagger ע�뵽 DI ������ȥ����������������������������������������������������������������������
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
                c.OAuthAppName("swagger_api ����");

                //c.OAuthClientSecret("secret");
                //c.OAuthRealm("test-realm");
                //c.OAuthScopeSeparator(" ");
                //c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "foo", "bar" } });
                //c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });

            app.UseRouting();

            app.UseIdentityServer();

            //��Ȩ
            app.UseAuthorization();
            //��֤
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