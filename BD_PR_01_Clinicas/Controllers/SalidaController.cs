using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;
using PagedList;

namespace BD_PR_01_Clinicas.Controllers
{
    public class SalidaController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Salida
        public ActionResult Index(int? page, string fechaIn, string fechaFin,string fec1,string fec2)
        {
            List<tbSalida> lista = null;
            if (!string.IsNullOrEmpty(fechaIn) && !string.IsNullOrEmpty(fechaFin))
            {
                page = 1;
            }
            else
            {
                fechaIn = fec1;
                fechaFin = fec2;
            }

            ViewBag.fec1 = fechaIn;
            ViewBag.fec2 = fechaFin;

            if (string.IsNullOrEmpty(fechaIn) || string.IsNullOrEmpty(fechaFin))
            {
                lista = (from t in db.tbSalida  orderby t.fechaSalida select t).Take(20).ToList();
            }
            else
            {
                lista = (from t in db.tbSalida
                         where t.fechaSalida >= DateTime.Parse(fechaIn) && t.fechaSalida <= DateTime.Parse(fechaFin)
                         select t).ToList();
            }


            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (lista == null) { return View(); }
            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        // GET: Salida/Detalles/5
        public ActionResult Detalles(int codSalida)
        {
            List<tbDetalleSalida> lista = (from t in db.tbDetalleSalida where t.codSalida == codSalida select t).ToList();
            return View(lista);
        }
        // GET: Salida/Crear
        public ActionResult Crear()
        {
            return View(new Salida());
        }

        // POST: Salida/Crear
        [HttpPost]
        public ActionResult Crear(Datos datos)
        {
            //aqui creo el objeto tbEntrada y todo lo del detalle, esto tu me lo enseñaste no voy a explicarlo jajajajajajajajaslkdjfa;sldkfja;lskdjf;
            try
            {
                tbSalida Salida = new tbSalida
                {
                    descripcion = datos.descripcion,
                    fechaSalida = DateTime.Now,
                    tipoSalida = true
                };
                foreach (Item item in datos.detalle)
                {
                    Salida.tbDetalleSalida.Add(new tbDetalleSalida
                    {
                        codProducto = item.codProducto,
                        cantidad = item.cantidad
                    });
                }
                db.tbSalida.InsertOnSubmit(Salida);
                db.SubmitChanges();
            }
            catch (Exception e)
            {

                return  View(e.Message);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Productos(string filtro = "") {

            List<RegistroProducto> lista = null;

            if (filtro == "")
            {
                lista = (from e in db.tbDetalleEntrada
                         join p in db.tbProducto on e.codProducto equals p.codProducto
                         where db.existencias(p.codProducto) > 0
                         select new RegistroProducto
                         {
                             codProducto = p.codProducto,
                             nombre = p.producto,
                             categoria = p.tbCategoria.categoria,
                             presentacion = p.tbPresentacion.presentacion,
                             dosis = p.dosis.ToString() + ((p.codVolumen == 1) ? " mg" : " ml")
                         }).Take(10).Distinct().ToList();
            }
            else
            {
                lista = (from e in db.tbDetalleEntrada
                         join p in db.tbProducto on e.codProducto equals p.codProducto
                         where p.producto.Contains(filtro) && db.existencias(p.codProducto) > 0
                         select new RegistroProducto
                         {
                             codProducto = p.codProducto,
                             nombre = p.producto,
                             categoria = p.tbCategoria.categoria,
                             presentacion = p.tbPresentacion.presentacion,
                             dosis = p.dosis.ToString() + ((p.codVolumen == 1) ? " mg" : " ml")
                            
                         }).Distinct().ToList();
            }
            return PartialView("_Productos", lista);  
        }

        public ActionResult MostrarDetalle(IEnumerable<Item> detalle) {

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
                                   cantidad = item.cantidad,
                                   existencia = db.existencias(t.codProducto).Value
                               }).SingleOrDefault());
                }
            }
            //el retorno sera el html creado en la vista parcial _detalle pasandole como modelo la lista
            return PartialView("_Detalle", lista);
        }
        // GET: Salida/ListaProductos/5
        public ActionResult ListaProductos(int codSalida)
        {
            List<tbDetalleSalida> lista = (from t in db.tbDetalleSalida where t.codSalida == codSalida select t).ToList();
            ViewBag.codSalida = codSalida;
            return View(lista);
        }

        // POST: Salida/ListaProductos/5
        [HttpPost]
        public ActionResult ListaProductos(int codSalida, bool accion, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                if (!accion)
                {
                    // TODO: Add cancel logic here
                    tbSalida eliminar = (from t in db.tbSalida where t.codSalida == codSalida select t).SingleOrDefault();
                    db.tbSalida.DeleteOnSubmit(eliminar);
                    db.SubmitChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Salida/AgregarProducto/5
        public ActionResult AgregarProducto(int codSalida)
        {
            List<tbProducto> productos = (from t in db.tbProducto where t.estado == true && db.existencias(t.codProducto) > 0 orderby t.producto select t).ToList();
            ViewBag.codSalida = codSalida;
            return View(productos);
        }

        // GET: Salida/cantidad/5
        public ActionResult cantidad(int codSalida, int codProducto)
        {
            tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
            ViewBag.codProducto = codProducto;
            ViewBag.codSalida = codSalida;
            ViewBag.producto = producto.producto;
            ViewBag.categoria = producto.tbCategoria.categoria;
            ViewBag.presentacion = producto.tbPresentacion.presentacion;
            ViewBag.codVolumen = producto.codVolumen;
            return View();
        }

        // POST: Salida/cantidad/5
        [HttpPost]
        public ActionResult cantidad(int codSalida, int codProducto, FormCollection collection)
        {
            try
            {
                tbDetalleSalida existente = (from t in db.tbDetalleSalida where t.codSalida == codSalida && t.codProducto == codProducto select t).SingleOrDefault();
                if (existente == null)
                {
                    tbDetalleSalida nueva = new tbDetalleSalida
                    {
                        codSalida = codSalida,
                        codProducto = codProducto,
                        cantidad = int.Parse(collection["cantidad"])
                    };
                    db.tbDetalleSalida.InsertOnSubmit(nueva);
                }
                else
                {
                    existente.cantidad += int.Parse(collection["cantidad"]);
                }
                db.SubmitChanges();
                return RedirectToAction("ListaProductos", new { codSalida = codSalida });
            }
            catch
            {
                return View();
            }
        }

        // GET: Salida/QuitarProducto/5
        public ActionResult QuitarProducto(int codSalida, int codProducto)
        {
            tbDetalleSalida quitar = (from t in db.tbDetalleSalida where t.codSalida == codSalida && t.codProducto == codProducto select t).SingleOrDefault();
            db.tbDetalleSalida.DeleteOnSubmit(quitar);
            db.SubmitChanges();
            return RedirectToAction("ListaProductos", new { codSalida = codSalida });
        }

        // GET: Salida/EditarCantidad/5
        public ActionResult EditarCantidad(int codSalida, int codProducto)
        {
            tbDetalleSalida editar = (from t in db.tbDetalleSalida where t.codSalida == codSalida && t.codProducto == codProducto select t).SingleOrDefault();
            return View(editar);
        }

        // POST: Salida/EditarCantidad/5
        [HttpPost]
        public ActionResult EditarCantidad(int codSalida, int codProducto, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbDetalleSalida editar = (from t in db.tbDetalleSalida where t.codSalida == codSalida && t.codProducto == codProducto select t).SingleOrDefault();
                editar.cantidad = int.Parse(collection["cantidad"]);
                db.SubmitChanges();
                return RedirectToAction("ListaProductos", new { codSalida = codSalida });
            }
            catch
            {
                return View();
            }
        }
    }
}
