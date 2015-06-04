using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FileRepository.Models
{
    public class DisplayModel
    {
        
        public Dictionary<Storage, List<Storage>> data { get; set; }
    }
}