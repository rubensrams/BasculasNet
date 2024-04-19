using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bascula.Model
{
    class Compra
    {
        [JsonProperty("idUsuario")]
        public int IdUsuario { get; set; }

        [JsonProperty("idRecuperador")]
        public int IdRecuperador  { get; set; }

        [JsonProperty("idSucursal")]
        public int IdSucursal  { get; set; }

        [JsonProperty("idFlete")]
        public int IdFlete { get; set; }

        [JsonProperty("total")]
        public double Total  { get; set; }

        [JsonProperty("detalle")]
        public ObservableCollection<Material> Detalle { get; set; }
    }
}
