using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class ConsultasFechas
    {
        public string paciente { get; set; }
        public DateTime fecha { get; set; }
        public string medico { get; set; }
        public string estudiante { get; set; }
    }
}