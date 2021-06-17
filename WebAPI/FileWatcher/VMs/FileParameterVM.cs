using System;
using System.Collections.Generic;
using System.Text;

namespace FileWatcher.VMs
{
    public class FileParameterVM
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public byte[] Source { get; set; }
        public string Directory { get; set; }
    }
}
