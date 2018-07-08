using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BD_PR_01_Clinicas.Models;
namespace BD_PR_01_Clinicas.Models
{
    public class TipoUsuario
    {
        public int codTipoUsuario { get; set; }
        public string tipoUsuario { get; set; }
    }

    public class ModeloPermisosRol {
        public int codRol { get; set; }
        public string NombreRol { get; set;}
   

        public List<CheckBoxPermiso> permisos { get; set; }

        public ModeloPermisosRol()
        {   //evita errores al llamar permisos.Count
            permisos = new List<CheckBoxPermiso>();
        }
    }

    public class CheckBoxPermiso{
    public int codPemiso { get; set; }
    public string nombrePermiso { get; set; }
   public bool IsChecked { get; set; }
    }


}