using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileRepository.Models
{
    public class MultiFileContent
    {
        public int filenumber { get; set; }
        public List<string> fileList { get; set; }
        public Dictionary<string, string> fileContent { get; set; }
    }
}