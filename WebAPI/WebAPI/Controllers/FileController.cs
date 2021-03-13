using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService fileService;
        public FileController(IFileService fileService)
        {
            this.fileService = fileService;
        }
        [HttpGet("getAllFiles")]
        public ActionResult GetAllFiles() {
            var data = fileService.GetAllFiles();
            return Ok(data);
        }
        [HttpGet("getFileById/{fileId}")]
        public ActionResult GetFileById(string fileId)
        {
            var data = fileService.GetFileById(fileId);
            return Ok(data);
        }
        [HttpPost("add")]
        public ActionResult Add(File file)
        {
            fileService.AddFile(file);
            return Ok(true);
        }
        [HttpPut("edit")]
        public ActionResult Edit(File file)
        {
            fileService.EditFile(file);
            return Ok(true);
        }
        [HttpDelete("delete/{fileId}")]
        public ActionResult delete(string fileId)
        {
            fileService.DeleteFile(fileId);
            return Ok(true);
        }
    }
}
