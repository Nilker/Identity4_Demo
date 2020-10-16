using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ALLInOne.Filter
{
    public class SwaggerHttpHeaderFilter : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();
            var filterPipeline = context.ApiDescription.ActionDescriptor.EndpointMetadata; //判断是否添加权限过滤器
            var isAuthorized = filterPipeline.Any(s => s.GetType() == typeof(AuthorizeAttribute));//判断是否允许匿名方法 
            var allowAnonymous = filterPipeline.Any(s => s.GetType() == typeof(AllowAnonymousAttribute));//判断是否允许匿名方法 

            //如果Api方法是允许匿名方法，Token不是必填的
            if (isAuthorized && !allowAnonymous)
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "Token",
                    Required = true,
                });
            }
        }
    }
}
