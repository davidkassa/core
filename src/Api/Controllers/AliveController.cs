﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;

namespace Bit.Api.Controllers
{
    [Route("alive")]
    public class AliveController : Controller
    {
        [HttpGet("")]
        public DateTime Get()
        {
            return DateTime.UtcNow;
        }

        [HttpGet("claims")]
        public IActionResult Claims()
        {
            return new JsonResult(User?.Claims?.Select(c => new { c.Type, c.Value }));
        }

        [HttpGet("version")]
        public IActionResult Version()
        {
            var version = Assembly.GetEntryAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return new JsonResult(version);
        }
    }
}
