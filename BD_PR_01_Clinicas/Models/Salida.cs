using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class Salida
    {

        public List<RegistroProducto> detalle { get; set; }
        public Salida()
        {
            detalle = new List<RegistroProducto>();
        }

    }

    public class EntregaMedicamento
    {
        public int codPaciente { get; set; }
        public List<tbDetalleSalida> detalle { get; set; }
    }
}