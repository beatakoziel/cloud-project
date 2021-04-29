using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Route("files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService fileService;
        public FileController(IFileService fileService)
        {
            this.fileService = fileService;
        }
        [HttpGet]
        public ActionResult GetCurrentFiles()
        {
            var data = fileService.GetCurrentFiles();
            return Ok(data);
        }
        [HttpGet("{fileId}")]
        public ActionResult GetFileById(string fileId)
        {
            var data = fileService.GetFileById(fileId);
            return Ok(data);
        }
        [HttpGet("test")]
        public string Test()
        {
            return "hello";
        }
        [HttpPost]
        public ActionResult Add(File file)
        {
            fileService.AddFile(file);
            return Ok(true);
        }
        [HttpPut]
        public ActionResult Edit(File file)
        {
            fileService.EditFile(file);
            return Ok(true);
        }
        [HttpDelete("{fileId}")]
        public ActionResult Delete(string fileId)
        {
            fileService.DeleteFile(fileId);
            return Ok(true);
        }
        [HttpGet("getFileSource/{fileId}")]
        public ActionResult GetFileSource(string fileId)
        {
            var data = fileService.GetFileSource(fileId);
            return Ok(data);
        }
        [HttpPost("deleteByName")]
        public ActionResult DeleteByName(FileNameParameter file)
        {
            fileService.DeleteFileByName(file.Name);
            return Ok(true);
        }
    }
}