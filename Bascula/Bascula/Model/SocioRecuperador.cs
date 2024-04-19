using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bascula.Model
{
    class SocioRecuperador
    {
        [JsonProperty("idRecuperador")]
        public int Id_recuperador { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("apPaterno")]
        public string Apellido_paterno { get; set; }

        [JsonProperty("apMaterno")]
        public string Apellido_materno { get; set; }

        [JsonProperty("direccion")]
        public string Direccion { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("telefono")]
        public string Telefono { get; set; }

    }
}
