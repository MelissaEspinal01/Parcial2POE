
using Microsoft.Data.SqlClient;
using RepoDb;
using System;
using System.Configuration;

namespace Registro_de_Notas.Clases
{
    public static class ConexionBD
    {
        private static readonly string cadenaConexion =
            ConfigurationManager.ConnectionStrings["conexionBD"]?.ConnectionString
            ?? throw new InvalidOperationException(
                "No se encontró la conexión 'conexionBD' en App.config.");

        public static SqlConnection ObtenerConexion()
        {
            GlobalConfiguration
                .Setup()
                .UseSqlServer();

            return new SqlConnection(cadenaConexion);
        }
    }
}
