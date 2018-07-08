using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class Rotaciones
    {
        public int rotacionId { get; set; }
        public string fechaIni { get; set; }
        public string fechaFin { get; set; }
        public List<ItemUser> integrantes { get; set; }
        public Rotaciones()
        {
            integrantes = new List<ItemUser>();
        }
    }


    public class ItemUser
    {
        public int codUser { get; set; }
   
    }

}

