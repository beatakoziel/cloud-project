using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.ViewModels
{
    public class FileSourceVM
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Source { get; set; }
    }
}
