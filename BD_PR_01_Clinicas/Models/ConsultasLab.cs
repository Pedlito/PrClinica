using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class ConsultasLab
    {
        public int codLaboratorio { get; set; }
        public string laboratorio { get; set; }
        public string resultado { get; set; }
        public string rango { get; set; }
    }
}