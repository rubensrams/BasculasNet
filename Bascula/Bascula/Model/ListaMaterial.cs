using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bascula.Model
{
    class ListaMaterial
    {


        [JsonProperty("idMaterial")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; } = String.Empty;

        [JsonProperty("description")]
        public string? Descripcion { get; set; }

        [JsonProperty("price")]
        public double Precio { get; set; }

    }
}
