using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("directories")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        private readonly IDirectoryService directoryService;
        public DirectoryController(IDirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }
        [HttpGet("getDirectories/{dirId}")]
        public ActionResult GetAllDirectories(string dirId)
        {
            var data = directoryService.GetDirectories(dirId);
            return Ok(data);
        }
        [HttpPost("{dirId}/{dirName}")]
        public ActionResult AddDirectory(string dirId, string dirName)
        {
            directoryService.AddDirectory(dirId, dirName);
            return Ok(true);
        }
    }
}
