﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Categoria
        public ActionResult Index()
        {
            List<tbCategoria> lista = (from t in db.tbCategoria orderby t.estado descending, t.categoria select t).ToList();
            return View(lista);
        }

        // GET: Categoria/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: Categoria/Create
        [HttpPost]
        public ActionResult Crear(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                tbCategoria nueva = new tbCategoria
                {
                    categoria = collection["categoria"],
                    estado = true
                };
                db.tbCategoria.InsertOnSubmit(nueva);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Categoria/Edit/5
        public ActionResult Edit(int id)
        {
            tbCategoria editar = (from t in db.tbCategoria where t.codCategoria == id select t).SingleOrDefault();
            return View(editar);
        }

        // POST: Categoria/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                tbCategoria editar = (from t in db.tbCategoria where t.codCategoria == id select t).SingleOrDefault();
                editar.categoria = collection["categoria"];
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Categoria/Delete/5
        public ActionResult CambiarEstado(int codCategoria, bool estado)
        {
            tbCategoria cambio = (from t in db.tbCategoria where t.codCategoria == codCategoria select t).SingleOrDefault();
            return View(cambio);
        }

        // POST: Categoria/Delete/5
        [HttpPost]
        public ActionResult CambiarEstado(int codCategoria, bool estado, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbCategoria cambio = (from t in db.tbCategoria where t.codCategoria == codCategoria select t).SingleOrDefault();
                cambio.estado = estado;
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}