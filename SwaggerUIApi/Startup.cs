using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SwaggerUIApi.Filter;

namespace SwaggerUIApi
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

            /*
             * Authentication �����֤��ȷ���û���ݵĹ���
             * 
             */

            //��֤��������ַ  �����֤����ע���DI
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:8000"; // IdentityServer��������ַ
                    //options.ApiName = "swagger_api"; // ������Խ��������֤��API��Դ������
                    options.RequireHttpsMetadata = true; // ָ���Ƿ�ΪHTTPS
                });


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


                // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:8000/connect/authorize"),
                            Scopes = new Dictionary<string, string> {
                                { "swagger_api", "swagger�ܱ�����api" }//ָ���ͻ��������api������ ���Ϊ�գ���ͻ����޷�����
                            }
                        }
                    }
                });

                c.OperationFilter<AddAuthHeaderOperationFilter>();

                //ÿ�����������Ƿ� ������������ϣ����еķ���������
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                //        },
                //        new[] { "readAccess", "writeAccess" }
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

            //��Ȩ
            app.UseAuthorization();
            //��֤
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
