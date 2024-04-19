using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Bascula.Model
{
    class Material
    {
        [JsonProperty("idProducto")]
        public int Id { get; set; }

        [JsonProperty("nombreProducto")]
        public string Nombre { get; set; }

        [JsonProperty("pesoBruto")]
        public double PesoBruto { get; set; }

        [JsonProperty("pesoNeto")]
        public double PesoNeto { get; set; }

        [JsonProperty("tara")]
        public double Tara { get; set; }

        [JsonProperty("precioNegociado")]
        public double CostoKilo { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonIgnore]
        public int AutoIncrement { get; set; }

    }
}
