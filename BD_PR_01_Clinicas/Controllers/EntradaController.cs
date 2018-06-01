﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    public class EntradaController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Entrada
        public ActionResult Index()
        {
            List<tbEntrada> lista = (from t in db.tbEntrada orderby t.fechaEntrada select t).ToList();
            return View(lista);
        }

        // GET: Entrada/Crear
        public ActionResult Crear()
        {
            return View(new Entrada());
        }

        // POST: Entrada/Crear
        [HttpPost]
        public ActionResult Crear(Datos datos)
        {
            //aqui creo el objeto tbEntrada y todo lo del detalle, esto tu me lo enseñaste no voy a explicarlo jajajajajajajajaslkdjfa;sldkfja;lskdjf;
            tbEntrada entrada = new tbEntrada
            {
                descripcion = datos.descripcion,
                fechaEntrada = DateTime.Now
            };
            foreach (Item item in datos.detalle)
            {
                entrada.tbDetalleEntrada.Add(new tbDetalleEntrada
                {
                    codProducto = item.codProducto,
                    cantidad = item.cantidad
                });
            }
            db.tbEntrada.InsertOnSubmit(entrada);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        // GET: Entrada/Productos
        public ActionResult Productos(string filtro = "")
        {
            //este es el codigo que se ejecuta cuando se habre el modal apachando el boton buscar, tiene un filtro como cualquier otro
            //pero al final, dice return PartialView("_Productos", lista), osea que llama a la vista parcial _Productos y le pasa la lista
            //como el modelo
            List<RegistroProducto> lista = null;
            if (filtro == "")
            {
                lista = (from t in db.tbProducto
                            where t.estado == true
                            orderby t.producto
                            select new RegistroProducto
                            {
                                codProducto = t.codProducto,
                                nombre = t.producto,
                                categoria = t.tbCategoria.categoria,
                                presentacion = t.tbPresentacion.presentacion,
                                dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml")
                            }).ToList();
            }
            else
            {
                lista = (from t in db.tbProducto
                            where t.producto.Contains(filtro) & t.estado == true
                            orderby t.producto
                            select new RegistroProducto
                            {
                                codProducto = t.codProducto,
                                nombre = t.producto,
                                categoria = t.tbCategoria.categoria,
                                presentacion = t.tbPresentacion.presentacion,
                                dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml")
                            }).ToList();
            }
            return PartialView("_Productos", lista);
        }

        public ActionResult MostrarDetalle(IEnumerable<Item> detalle)
        {
            //este action recibe como parametro la lista de productos Item, tenes que verla en los modelos, esta dentro del modelo Entrada,
            //es un objeto con un int codProducto y un int cantidad, que son los dos campos del array en javascript
            //creo una lista
            List<RegistroProducto> lista = new List<RegistroProducto>();
            if (detalle != null)
            {
                foreach (Item item in detalle)
                {
                    //por cada item dentro de la lista detalle creo un registroProducto
                    lista.Add((from t in db.tbProducto
                               where t.codProducto == item.codProducto
                               orderby t.producto
                               select new RegistroProducto
                               {
                                   codProducto = t.codProducto,
                                   nombre = t.producto,
                                   categoria = t.tbCategoria.categoria,
                                   presentacion = t.tbPresentacion.presentacion,
                                   dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml"),
                                   cantidad = item.cantidad
                               }).SingleOrDefault());
                }
            }
            //el retorno sera el html creado en la vista parcial _detalle pasandole como modelo la lista
            return PartialView("_Detalle", lista);
        }

        // GET: Entrada/Detalles/5
        public ActionResult Detalles(int codEntrada)
        {
            List<tbDetalleEntrada> lista = (from t in db.tbDetalleEntrada where t.codEntrada == codEntrada select t).ToList();
            return View(lista);
        }

        

        // GET: Entrada/ListaProductos/5
        public ActionResult ListaProductos(int codEntrada)
        {
            return View();
        }

        // POST: Entrada/ListaProductos/5
        [HttpPost]
        public ActionResult ListaProductos(int codEntrada, bool accion, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Entrada/AgregarProducto/5
        public ActionResult AgregarProducto(int codEntrada)
        {
            return View();
        }

        // GET: Entrada/cantidad/5
        public ActionResult cantidad(int codEntrada, int codProducto)
        {
            return View();
        }

        // POST: Entrada/cantidad/5
        [HttpPost]
        public ActionResult cantidad(int codEntrada, int codProducto, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("ListaProductos", new { codEntrada = codEntrada });
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // GET: Entrada/NuevoProducto/5
        public ActionResult NuevoProducto(int codEntrada)
        {
            List<Volumen> edades = new List<Volumen> { (new Volumen { codVolumen = 1, volumen = "Niños" }),
                                             (new Volumen { codVolumen = 2, volumen = "Adultos" }) };
            ViewBag.codPresentacion = new SelectList(db.tbPresentacion, "codPresentacion", "presentacion");
            ViewBag.codCategoria = new SelectList(db.tbCategoria, "codCategoria", "categoria");
            ViewBag.codVolumen = new SelectList(edades, "codVolumen", "volumen");
            ViewBag.codEntrada = codEntrada;
            return View();
        }

        // POST: Entrada/NuevoProducto/5
        [HttpPost]
        public ActionResult NuevoProducto(int codEntrada, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbProducto nuevo = new tbProducto
                {
                    producto = collection["producto"],
                    codCategoria = int.Parse(collection["codCategoria"]),
                    codPresentacion = int.Parse(collection["codPresentacion"]),
                    codVolumen = int.Parse(collection["codVolumen"]),
                    estado = true
                };
                db.tbProducto.InsertOnSubmit(nuevo);
                db.SubmitChanges();
                int codProducto = (from t in db.tbProducto orderby t.codProducto descending select t.codProducto).First();
                return RedirectToAction("Cantidad", new { codEntrada = codEntrada, codProducto = codProducto });
            }
            catch
            {
                return View();
            }
        }

       
    }
}
