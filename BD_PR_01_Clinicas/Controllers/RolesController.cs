using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;
namespace BD_PR_01_Clinicas.Controllers
{
    public class RolesController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Roles
        public ActionResult Index()
        {
            
            return View(db.tbRol.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult DetallesRol(int id)
        {
            var Rl = (from r in db.tbRol where r.codTipoUsuario == id select r).SingleOrDefault();
            ViewBag.permisos  = (from p in Rl.tbRolPermiso where p.codTipoUsuario == id select p.tbPermiso).ToList();

            return View(Rl);
        }

        // GET: Roles/Create
        public ActionResult CrearRol()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        public ActionResult CrearRol(FormCollection collection)
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

        // GET: Roles/Edit/5
        public ActionResult EditarRol(int id)
        {
            return View();
        }

        // POST: Roles/Edit/5
        [HttpPost]
        public ActionResult EditarRol(tbRol rol)
        {
            try
            {
                db.tbRol.InsertOnSubmit(rol);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Roles/Delete/5
        public ActionResult ArchivarRol(int id)
        {
            var Rl = (from r in db.tbRol where r.codTipoUsuario == id select r).SingleOrDefault();
          
            return View(Rl);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        public ActionResult ArchivarRol(int id, FormCollection collection)
        {
            try
            {
                var Rl = (from r in db.tbRol where r.codTipoUsuario == id select r).SingleOrDefault();
                Rl.estado = false;
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AgregarPermisos(int? id) {
            //Se obtiene el rol
            tbRol rol = db.tbRol.Where(x => x.codTipoUsuario == id).SingleOrDefault();
            //se asigna rol al modelo
            ModeloPermisosRol modeloPermisoRol = new ModeloPermisosRol()
            {
                codRol = rol.codTipoUsuario,
                NombreRol = rol.Rol

             };

            //todos los permisos que existen
            List<tbPermiso> todosLosPermisos = (from p in db.tbPermiso select p).ToList();

            //se obtienen los permisos del rol codigo y permiso
            var permisosDelRol = rol.tbRolPermiso.Select(p => p.tbPermiso);

            //se indican cuales son los permisos que se tienen actualmente
            var CheckBoxPermisos = new List<CheckBoxPermiso>();

            foreach (var p in todosLosPermisos)
            {
                CheckBoxPermisos.Add(new CheckBoxPermiso()
                {
                    codPemiso = p.idPermiso,
                    nombrePermiso = p.Permiso,
                    //seleccionar los que existen
                    IsChecked = permisosDelRol.Where(x => x.idPermiso == p.idPermiso).Any()
                });
            }  
            //se asigna al modelo
            modeloPermisoRol.permisos = CheckBoxPermisos;

            return View(modeloPermisoRol);
        }
        [HttpPost]
        public ActionResult AgregarPermisos(ModeloPermisosRol modelo)
        {
            //var permisosSeleccionados = modelo.permisos.Where(x => x.IsChecked).Select(x => x.codPemiso).ToList();
            //tbRol rol = db.tbRol.Where(x => x.codTipoUsuario == modelo.codRol).SingleOrDefault();
            //rol.tbRolPermiso.Clear();

       
      


            tbRol Rol = db.tbRol.Where(x => x.codTipoUsuario == modelo.codRol).SingleOrDefault();

    

            List<tbRolPermiso> tbRolPermisosAnt = Rol.tbRolPermiso.ToList();

            foreach (var p in modelo.permisos)
            {
                if (p.IsChecked)
                {       //si no esta en la lista antigua de la base de datos
                    if (!tbRolPermisosAnt.Any(x => x.idPermiso == p.codPemiso))
                    {
                        //el permiso no estaba y fue agregado
                        db.tbRolPermiso.InsertOnSubmit(new tbRolPermiso { idPermiso = p.codPemiso, codTipoUsuario = modelo.codRol});

                    }//else: se encuentra en la base y no se quitó el permiso, no se modificoo

                }
                else
                {
                    //si esta en la lista antigua de la base de datos es por que el permiso esta asignado y debe removerse
                    if (tbRolPermisosAnt.Any(x => x.idPermiso == p.codPemiso))
                    {
                        //tbRolPermiso rp = (from r in db.tbRolPermiso where (r.idPermiso == p.idPermiso && r.codTipoUsuario==modelo.rol.codTipoUsuario) select r).SingleOrDefault();
                        tbRolPermiso rp = db.tbRolPermiso.Where(x => x.codTipoUsuario == modelo.codRol && x.idPermiso == p.codPemiso).SingleOrDefault();
                        db.tbRolPermiso.DeleteOnSubmit(rp);

                    }//else: no estaba y no ase agregaraa
                }
            }

            db.SubmitChanges();
            //ViewBag.errores =cod+" - "+tdo1+" - "+tdo3;
            //return View();
            return RedirectToAction("Index");
        }

  
    }
}
