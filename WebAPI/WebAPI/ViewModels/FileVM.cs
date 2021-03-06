using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.ViewModels
{
    public class FileVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public List<FileVM> PreviousVersions { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
