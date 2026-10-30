
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using RepoDb;
using Registro_de_Notas.Clases;

namespace Registro_de_Notas.Repositorios
{
    public class RegistroNotaRepositorio
    {
        public List<RegistroNota> ObtenerTodos()
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                return conexion.QueryAll<RegistroNota>().ToList();
            }
        }

        public RegistroNota ObtenerPorId(int id)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                return conexion.Query<RegistroNota>(
                    registro => registro.Id == id
                ).FirstOrDefault();
            }
        }

        public int Insertar(RegistroNota registro)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                return conexion.Insert<RegistroNota, int>(registro);
            }
        }

        public int Actualizar(RegistroNota registro)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                return conexion.Update<RegistroNota>(registro, registro.Id);
            }
        }

        public int Eliminar(int id)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                conexion.Open();
                return conexion.Delete<RegistroNota>(id);
            }
        }
    }
}
