using POOI_EF_SEBASTIANCASTILLO.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POOI_EF_SEBASTIANCASTILLO.Controllers
{
public class ConsumidorController : Controller
  {
        string cadenaConexion =
            ConfigurationManager.ConnectionStrings["ExamenFinal"].ConnectionString;

        List<Consumidor> listarConsumidores()
        {
            List<Consumidor> lista = new List<Consumidor>();

            SqlConnection cn = new SqlConnection(cadenaConexion);
            cn.Open();

            SqlCommand cmd = new SqlCommand("usp_listar_consumidor", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Consumidor
                {
                    dnicont = dr.GetString(0),
                    nomcont = dr.GetString(1),
                    apecont = dr.GetString(2),
                    emailcont = dr.IsDBNull(3) ? null : dr.GetString(3),
                    idpais = dr.IsDBNull(4) ? null : dr.GetString(4)
                });
            }

            dr.Close();
            cn.Close();

            return lista;
        }

        public ActionResult Index()
        {
            return View(listarConsumidores());
        }
        string agregarConsumidor(Consumidor consumidor)
        {
            SqlConnection cn = new SqlConnection(cadenaConexion);
            cn.Open();

            SqlCommand cmd = new SqlCommand("usp_agrega_consumidor", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@dnicont", consumidor.dnicont);
            cmd.Parameters.AddWithValue("@nomcont", consumidor.nomcont ?? "");
            cmd.Parameters.AddWithValue("@apecont", consumidor.apecont ?? "");
            cmd.Parameters.AddWithValue("@emailcont",(object)consumidor.emailcont ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idpais",(object)consumidor.idpais ?? DBNull.Value);

            cmd.Parameters.Add("@mensaje", System.Data.SqlDbType.VarChar, 250);
            cmd.Parameters["@mensaje"].Direction =
                System.Data.ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            string mensaje = cmd.Parameters["@mensaje"].Value.ToString();

            cn.Close();

            return mensaje;
        }

        public ActionResult Create()
        {
            return View(new Consumidor());
        }

        [HttpPost]
        public ActionResult Create(Consumidor consumidor)
        {
            ViewBag.Mensaje = agregarConsumidor(consumidor);
            return View(consumidor);
        }
        string eliminarConsumidor(string dni)
        {
            SqlConnection cn = new SqlConnection(cadenaConexion);
            cn.Open();

            SqlCommand cmd = new SqlCommand("usp_eliminar_consumidor", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@dnicont", dni);

            cmd.Parameters.Add("@mensaje", System.Data.SqlDbType.VarChar, 250);
            cmd.Parameters["@mensaje"].Direction =
                System.Data.ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            string mensaje = cmd.Parameters["@mensaje"].Value.ToString();

            cn.Close();

            return mensaje;
        }

        public ActionResult Delete(string dni)
        {
            Consumidor consumidor = listarConsumidores()
                .Find(c => c.dnicont == dni);

            if (consumidor == null)
                return HttpNotFound();

            return View(consumidor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Consumidor consumidor)
        {
            TempData["Mensaje"] = eliminarConsumidor(consumidor.dnicont);

            return RedirectToAction("Index");
        }
        string actualizarConsumidor(Consumidor consumidor)
        {
            SqlConnection cn = new SqlConnection(cadenaConexion);
            cn.Open();

            SqlCommand cmd = new SqlCommand("usp_actualizar_consumidor", cn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@dnicont", consumidor.dnicont ?? "");
            cmd.Parameters.AddWithValue("@nomcont", consumidor.nomcont ?? "");
            cmd.Parameters.AddWithValue("@apecont", consumidor.apecont ?? "");

            cmd.Parameters.AddWithValue("@emailcont",
                (object)consumidor.emailcont ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@idpais",
                (object)consumidor.idpais ?? DBNull.Value);

            cmd.Parameters.Add("@mensaje", System.Data.SqlDbType.VarChar, 250);
            cmd.Parameters["@mensaje"].Direction =
                System.Data.ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            string mensaje = cmd.Parameters["@mensaje"].Value.ToString();

            cn.Close();

            return mensaje;
        }

        public ActionResult Update(string dni)
        {
            Consumidor consumidor = listarConsumidores()
                .Find(c => c.dnicont == dni);

            if (consumidor == null)
                return HttpNotFound();

            return View(consumidor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Consumidor consumidor)
        {
            ViewBag.Mensaje = actualizarConsumidor(consumidor);

            return View(consumidor);
        }
    }
}