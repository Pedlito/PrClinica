using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class DetalleEntrega
    {
        public tbPaciente paciente { get; set; }
        public tbSalida entrega { get; set; }
        public List<RegistroProducto> detalle { get; set; }
    }
}