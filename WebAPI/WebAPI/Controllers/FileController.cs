using Microsoft.AspNetCore.Http;
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
        [HttpGet("getFiles/{dirId}")]
        public ActionResult GetCurrentFiles(string dirId)
        {
            var data = fileService.GetCurrentFiles(dirId);
            return Ok(data);
        }
        [HttpGet("{name}")]
        public ActionResult GetCurrent(string name)
        {
            var data = fileService.GetCurrentFile(name);
            return Ok(data);
        }
        [HttpPost("addFile/{dirId}")]
        [RequestSizeLimit(long.MaxValue)]
        public ActionResult Add(IFormFile file, string dirId)
        {
            fileService.AddFile(file, dirId);
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
        [HttpGet("{fileId}/source")]
        public ActionResult GetFileSource(string fileId)
        {
            FileSourceVM file = fileService.GetFileSource(fileId);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = file.Name,
                Inline = true,
            };

            Response.Headers.Add("Content-Disposition", cd.ToString());

            return File(file.Source, file.ContentType);
        }
        
        [HttpDelete]
        public ActionResult DeleteByName(FileNameParameter file)
        {
            fileService.DeleteFileByName(file.Name);
            return Ok(true);
        }
    }
}