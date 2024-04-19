using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bascula.Model
{
    internal class ReporteCompraNuevo
    {
        public string fecha { get;set; }

        public string sucursal { get; set; }

        public string recuperador  { get; set; }

        public string usuario  { get; set; }

        public string estatus  { get; set; }

        public string pesoNeto  { get; set; }

        public string precio  { get; set; }

        public string total  { get; set; }

    }
}
