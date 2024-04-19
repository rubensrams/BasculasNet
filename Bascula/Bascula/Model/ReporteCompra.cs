using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Bascula.Model
{
    internal class ReporteCompra
    {
        public int IdCompra { get; set; }

        public int Folio { get; set; }

        public string Recuperador { get; set; }

        public double Total { get; set; }

        public string Sucursal { get; set; }

        public string User { get; set; }

        public string Estatus { get; set; }

        public string Fecha { get; set; }
    }
}
