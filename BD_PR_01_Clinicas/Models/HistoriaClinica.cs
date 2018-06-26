﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class HistoriaClinica
    {
        public int codConsulta { get; set; }
        public tbPaciente paciente { get; set; }
        public tbAntecedentesPatologicos patologicos { get; set; }
        public tbAntecedentesNoPatologicos noPatologicos { get; set; }
        public tbDesarrollo desarrollo { get; set; }
        public tbMujeres mujeres { get; set; }
        public tbPerfilSocial perfilSocial { get; set; }
        public tbRevisionSistemas revision { get; set; }
        public tbSignosVitales signos { get; set; }
        public List<tbProblema> problemas { get; set; }
        public tbPlanes planes { get; set; }
        public tbPlanTerapeutico terapeutico { get; set; }
        public List<tbReceta> receta { get; set; }
        public tbDiagnostico diagnostico { get; set; }
    }
}