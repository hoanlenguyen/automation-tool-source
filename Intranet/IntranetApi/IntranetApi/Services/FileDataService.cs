using Dapper;
using IntranetApi.DbContext;
using IntranetApi.Enum;
using IntranetApi.Helper;
using IntranetApi.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using System.Data;
using System.Security.Claims;

namespace IntranetApi.Services
{
    [Authorize]
    public static class FileDataService
    {
        public static void AddFileDataService(this WebApplication app)
        {
            app.MapPost("File/Upload", [Authorize]
            async Task<IResult> (
                [FromServices] IFileStorageService fileService, 
                [FromQuery] string? folderName, 
                HttpRequest request) =>
            {
                if(folderName.IsNullOrEmpty())
                    folderName = request.Headers["folderName"].ToString();

                var result = new List<string>();
                foreach (var file in request.Form.Files)
                {
                    if (file is null || file.Length == 0)
                        continue;

                    using var fileStream = file.OpenReadStream();
                    byte[] bytes = new byte[file.Length];
                    fileStream.Read(bytes, 0, (int)file.Length);
                    result.Add(await fileService.SaveAndGetShortUrl(bytes, file.FileName, folderName));
                }
                return Results.Ok(result);
            });
        }
    }
}