using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    public class EntregaMedicamentoController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: EntregaMedicamento
        public ActionResult Index(string paciente = "")
        {
            List<tbSalida> entregas = null;
            if (paciente == "")
            {
                entregas = (from t in db.tbSalida where t.tipoSalida == false orderby t.fechaSalida descending select t).Take(15).ToList();
            }
            else
            {
                entregas = (from t in db.tbSalida where t.tbPaciente.nombre.Contains(paciente) & t.tipoSalida == false orderby t.fechaSalida descending select t).Take(15).ToList();
            }
            ViewBag.paciente = paciente;
            return View(entregas);
        }

        // GET: EntregaMedicamento/Entregar
        public ActionResult Entregar()
        {
            return View();
        }

        public ActionResult Pacientes(string filtro = "")
        {
            //este es el codigo que se ejecuta cuando se habre el modal apachando el boton buscar, tiene un filtro como cualquier otro
            //pero al final, dice return PartialView("_Productos", lista), osea que llama a la vista parcial _Productos y le pasa la lista
            //como el modelo
            List<tbPaciente> lista = null;
            if (filtro == "")
            {
                lista = (from t in db.tbPaciente
                         orderby t.nombre
                         select t).Take(15).ToList();
            }
            else
            {
                lista = (from t in db.tbPaciente
                         where t.nombre.Contains(filtro)
                         orderby t.codPaciente
                         select t).Take(15).ToList();
            }
            return PartialView("_Pacientes", lista);
        }

        public ActionResult Filtrar(string filtro = "")
        {
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
                             dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml"),
                         }).Take(10).ToList();
            }
            else
            {
                lista = (from t in db.tbProducto
                         where t.producto.Contains(filtro) && t.estado == true//cambiado de & a && (no evalua la segunda expr si la 1era ea false)
                         orderby t.producto
                         select new RegistroProducto
                         {
                             codProducto = t.codProducto,
                             nombre = t.producto,
                             categoria = t.tbCategoria.categoria,
                             presentacion = t.tbPresentacion.presentacion,
                             dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml")
                         }).Take(10).ToList();
            }
            return PartialView("_Medicamentos", lista);
        }

        public ActionResult MostrarDetalle(IEnumerable<Item> detalle)
        {
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

        public string Crear(EntregaMedicamento entrega)
        {
            try
            {
                tbSalida Salida = new tbSalida
                {
                    codPaciente = entrega.codPaciente,
                    fechaSalida = DateTime.Now,
                    tipoSalida = false
                };
                Salida.tbDetalleSalida.AddRange(entrega.detalle);
                db.tbSalida.InsertOnSubmit(Salida);
                db.SubmitChanges();
                return Url.Action("Index");
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
        // GET: EntregaMedicamento/Detalles/5
        public ActionResult Detalles(int codSalida)
        {
            DetalleEntrega modelo = new DetalleEntrega();
            modelo.entrega = (from t in db.tbSalida where t.codSalida == codSalida select t).SingleOrDefault();
            modelo.paciente = (from t in db.tbPaciente where t.codPaciente == modelo.entrega.codPaciente select t).SingleOrDefault();
            modelo.detalle = (from det in db.tbDetalleSalida
                                            join pro in db.tbProducto on det.codProducto equals pro.codProducto
                                            where det.codSalida == codSalida
                                            select new RegistroProducto
                                            {
                                                codProducto = det.codProducto,
                                                nombre = pro.producto,
                                                categoria = pro.tbCategoria.categoria,
                                                presentacion = pro.tbPresentacion.presentacion,
                                                dosis = pro.dosis.ToString() + ((pro.codVolumen == 1) ? " mg" : " ml"),
                                                cantidad = det.cantidad.Value
                                            }).ToList();
            return View(modelo);
        }

        // GET: EntregaMedicamento/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EntregaMedicamento/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EntregaMedicamento/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EntregaMedicamento/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EntregaMedicamento/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EntregaMedicamento/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
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
    }
}
