﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleArchitecture.Swagger.Filters;

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var allowAnonymous = actionMetadata.Any(metadataItem => metadataItem is AllowAnonymousAttribute);

        if (allowAnonymous)
        {
            return;
        }
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement() {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            }
        };

    }
}