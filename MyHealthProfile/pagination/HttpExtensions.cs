using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tawtheiq.Application.Cores.General.Dtos;

namespace Tawtheiq.Application.Cores.General.extension
{
    public static class HttpExtensions
    
        {
            public static void AddPaginationHeader(this HttpResponse response, MetaData metaData)
            {
                var option = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData, option));
                response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

            }
        
    }
}
