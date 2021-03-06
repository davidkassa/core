﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bit.Api.Utilities
{
    public static class MultipartFormDataHelper
    {
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public static async Task GetFileAsync(this HttpRequest request, Func<Stream, string, Task> callback)
        {
            await request.GetFilesAsync(1, callback);
        }

        private static async Task GetFilesAsync(this HttpRequest request, int? fileCount, Func<Stream, string, Task> callback)
        {
            var boundary = GetBoundary(MediaTypeHeaderValue.Parse(request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync();
            var fileNumber = 1;
            while(section != null && fileNumber <= fileCount)
            {
                ContentDispositionHeaderValue content;
                if(ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out content) &&
                    HasFileContentDisposition(content))
                {
                    var fileName = HeaderUtilities.RemoveQuotes(content.FileName) ?? string.Empty;
                    await callback(section.Body, fileName);
                }

                if(fileNumber >= fileCount)
                {
                    section = null;
                }
                else
                {
                    section = await reader.ReadNextSectionAsync();
                    fileNumber++;
                }
            }
        }

        private static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
            if(string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if(boundary.Length > lengthLimit)
            {
                throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary;
        }

        private static bool HasFileContentDisposition(ContentDispositionHeaderValue content)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return content != null && content.DispositionType.Equals("form-data") &&
                (!string.IsNullOrEmpty(content.FileName) || !string.IsNullOrEmpty(content.FileNameStar));
        }
    }
}
