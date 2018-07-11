using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;
using PagedList;

namespace BD_PR_01_Clinicas.Controllers
{
    [AutenticadoAttribute]
  //  [PermisoAttribute(Permiso = RolesPermisos.administrar_roles)]
    public class RolesController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Roles
        public ActionResult Index(int? page)
        {
            List<tbRol> roles = db.tbRol.ToList();
          
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (roles == null) { return View(); }
            return View(roles.ToPagedList(pageNumber, pageSize));
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
        public ActionResult CrearRol(tbRol rl)
        {

            try
            {

                tbRol rol = new tbRol()
                {
                    Rol = rl.Rol,
                    descripcion = rl.descripcion,
                    estado = true
                };
                db.tbRol.InsertOnSubmit(rol);
                db.SubmitChanges();
                return RedirectToAction("Index");

             }
            catch
             {
                ModelState.AddModelError("Rol", "no se pudo crear el rol");
                return View(rl);
            }
}

        // GET: Roles/Edit/5
        public ActionResult EditarRol(int? id)
        {
           tbRol Rl = (from r in db.tbRol where r.codTipoUsuario == id select r).SingleOrDefault();
            return View(Rl);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        public ActionResult EditarRol(FormCollection coleccion)
        {
   
         
            try
            {

                tbRol rl = db.tbRol.Where(x => x.codTipoUsuario == int.Parse(coleccion["codTipoUsuario"])).SingleOrDefault();
                rl.Rol = coleccion["Rol"];
                rl.descripcion = coleccion["descripcion"];

                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.errores = "No se pudo realizar la operacion";
                return View("VistaDeErrores");
            }
        }

        // GET: Roles/Delete/5
        public ActionResult CambiarEstadoRol(int id)
        {
            var Rl = (from r in db.tbRol where r.codTipoUsuario == id select r).SingleOrDefault();
         
            return View(Rl);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        public ActionResult CambiarEstadoRol(FormCollection collection)
        {
            int cod = int.Parse(collection["codTipoUsuario"]);

            try
            {
                var Rl = (from r in db.tbRol where r.codTipoUsuario == cod select r).SingleOrDefault();

                string str = Request.Params["btn"];

                if (str == "Archivar")
                {
                    Rl.estado = false;
                }
                else
                {
                    Rl.estado  = true;
                }   
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.errores = "No se pudo realizar la operacion";
                return View("VistaDeErrores");
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
        
            return RedirectToAction("Index");
        }

  
    }
}
