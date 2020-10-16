﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "lhl",
                    Password = "lhl"
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
        /// <summary>
        /// API信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new[]
            {
                new ApiResource("swagger_api", "swagger_api 描述")
            };
        }

        public static IEnumerable<ApiScope> ApiScopes()
        {
           return new ApiScope[]
            {
                new ApiScope("scope1"),
                new ApiScope("scope2"),
                new ApiScope("api1"),
                new ApiScope("swagger_api"),
            };
        }
           

        /// <summary>
        /// 客服端信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "swagger_client",//客服端名称
                    ClientName = "Swagger UI client",//描述
                    AllowedGrantTypes = GrantTypes.Implicit,//Implicit 方式
                    AllowAccessTokensViaBrowser = true,//是否通过浏览器为此客户端传输访问令牌
                    RedirectUris =
                    {
                        "https://localhost:8001/swagger/oauth2-redirect.html"
                    },
                    AllowedScopes = { "swagger_api" }
                },

            };
        }
    }
}
