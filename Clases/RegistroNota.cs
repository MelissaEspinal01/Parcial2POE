using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registro_de_Notas.Clases
{
    public class RegistroNota
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Codigo { get; set; }

        public decimal Laboratorio { get; set; }

        public decimal Parcial { get; set; }

        public decimal Asistencia { get; set; }

        public decimal NotaFinal { get; set; }
    }
}
