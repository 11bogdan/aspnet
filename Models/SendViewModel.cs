using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckDocument.Models
{
    public class SendViewModel
    {
        public string UserName { get; set; }
        public List<string> Files { get; set; }
        public string Comment { get; set; }
    }
}