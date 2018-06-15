using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class Entrada
    {
    
        
        public List<RegistroProducto> detalle { get; set; }
        public Entrada()
        {
            detalle = new List<RegistroProducto>();
        }


    }


    //modelo de un registro de producto
    public class RegistroProducto
    {
        public int codProducto { get; set; }
        public string nombre { get; set; }
        public string categoria { get; set; }
        public string presentacion { get; set; }
        public string dosis { get; set; }
        public int cantidad { get; set; }
        public int existencia { get; set; }
    }
        public class Item
    {
        public int codProducto { get; set; }
        public int cantidad { get; set; }
    }

    public class Datos
    {
        public string descripcion { get; set; }
        public List<Item> detalle { get; set; }
    }
}

