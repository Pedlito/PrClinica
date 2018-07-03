using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class inventa
    {
        public string Producto { get; set; }
        public string Categoria { get; set; }
        public string Presentacion { get; set; }
        public int Existencia { get; set; }
    }
}