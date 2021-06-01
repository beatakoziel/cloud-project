using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.ViewModels
{
    public class FileParameterVM
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public byte[] Source { get; set; }
        public string Directory { get; set; }
    }
}
