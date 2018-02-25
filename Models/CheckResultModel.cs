using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckDocument.Models
{
    public class CheckResultModel
    {
        [System.ComponentModel.DisplayName("Проверяемый файл")]
        public string DocxFile { get; set; }

        [System.ComponentModel.DisplayName("Файл с требованиями")]
        public string ReqFile { get; set; }

        [System.ComponentModel.DisplayName("Результат")]
        public List<string> Message { get; set; }
    }
}