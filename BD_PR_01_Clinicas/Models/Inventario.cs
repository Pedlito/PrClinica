using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class Inventario
    {
        public int codProducto { get; set; }
        public string producto { get; set; }
        public string categoria { get; set; }
        public string presentacion { get; set; }
        public decimal dosis { get; set; }
        public int codVolumen { get; set; }
        public int existencia { get; set; }
    }




}