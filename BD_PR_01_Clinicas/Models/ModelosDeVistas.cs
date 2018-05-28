using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD_PR_01_Clinicas.Models
{
    public class ModeloDevistas { }
    public class SalidaModel
    {
        public List<productoModelo> ProductosAMostrar { get; set; }
        public SalidaModel()
        {
            ProductosAMostrar = new List<productoModelo>();
           // inicializarVariables();
        }
        public string descripcionSalida { get; set; }

        public int codigoProducto { get; set; }

        public string nombreProducto { get; set; }
        public string presentacionProducto { get; set; }
        public string volumenProducto { get; set; }
        public int cantidadProducto { get; set; }
        public int existenciaProducto { get; set; }
        public void inicializarVariables()                                                                       
        {
            codigoProducto = 0;
            nombreProducto = "";
            presentacionProducto = "";
            volumenProducto = "";
            cantidadProducto = 1;
        }
        public bool SeAgregoUnProductoValido()
        {
            return !(codigoProducto == 0 || string.IsNullOrEmpty(nombreProducto) || cantidadProducto <= 0);
        }
        public bool ExisteEnDetalle(int ProdId)
        {
            return ProductosAMostrar.Any(x => x.productoCod == ProdId);
        }

        public void RetirarItemDeDetalle()
        {
            if (ProductosAMostrar.Count > 0)
            {
                var detalleARetirar = ProductosAMostrar.Where(x => x.productoQuitar)
                                                        .SingleOrDefault();

                ProductosAMostrar.Remove(detalleARetirar);
            }
        }

        public void AgregarProducto()
        {
            ProductosAMostrar.Add(new productoModelo
            {
                productoCod = codigoProducto,
                productoNom = nombreProducto,
                productoPresent = presentacionProducto,
                productoVol = volumenProducto,
                productoCant = cantidadProducto,
                productoQuitar = false
            });

            inicializarVariables();//probablemente no necesario porque se sobre escriben
        }

        public tbSalida agregarAMOdelo()
        {   //codigo de Salida por ser incremental se agrega solo
            tbSalida regSalida = new tbSalida();
            regSalida.descripcion = this.descripcionSalida;
            regSalida.fechaSalida = DateTime.Now;
            foreach (var d in ProductosAMostrar)
            {

                regSalida.tbDetalleSalida.Add(new tbDetalleSalida
                {
                    //codigo de Salida se agrega solo
                    codProducto = d.productoCod,
                    cantidad = d.productoCant

                });
            }
            return regSalida;
        }

    }

//modelo de un registro de producto
    public class productoModelo
    {
        public int productoCod { get; set; }
        public string productoNom { get; set; }
        public string productoPresent { get; set; }
        public string productoVol { get; set; }
        public int productoCant { get; set; }
        public int productoExist { get; set; }
        public bool productoQuitar { get; set; }
        
    
    }

}