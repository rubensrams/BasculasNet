using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using Bascula.Model;
using Newtonsoft.Json;

namespace Bascula.Service
{
    class ApiService
    {
        private readonly string URL;
        private readonly string KEY;

        public ApiService()
        {
            Dictionary<string, string> dicitionario =
                File.ReadLines(
                    System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ini.properties")
                    ).Select(s => s.Split('=')).Select(s => new {
                        key = s[0],
                        value = string.Join("=", s.Select((o, n) => new {
                            n,
                            o
                        }).Where(o => o.n > 0).Select(o => o.o))
                    }).ToDictionary(o => o.key, o => o.value);

            URL = dicitionario["URL"];
            KEY = dicitionario["KEY"];
        }

        internal User Enabled(string username, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            try
            {
                var response = client.PostAsync("/basculas/api/user/get/", new FormUrlEncodedContent(pairs)).Result;
                if (response.IsSuccessStatusCode)
                {
                    string resultContent = response.Content.ReadAsStringAsync().Result;
                    if (resultContent != null)
                        return JsonConvert.DeserializeObject<User>(resultContent);

                }
            }catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        internal List<ListaMaterial> Materiales()
        {
            var pairs = new List<KeyValuePair<string, string>> { };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/materials/get/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                    return JsonConvert.DeserializeObject<List<ListaMaterial>>(resultContent);
            }
            return null;
        }

        internal string Dinero(int id_user, int id_sucursal, decimal dinero)
        {
            var pairs = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("idUser", id_user.ToString()),
            new KeyValuePair<string, string>("idSucursal", id_sucursal.ToString()),
            new KeyValuePair<string, string>("cantidad", dinero.ToString())
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/efectivo/add/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                    return resultContent;
            }
            return null;

        }

        internal bool ChecarTurno(int id_user)
        {
            var pairs = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("idUser", id_user.ToString())
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/turno/get/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    dynamic element = JsonConvert.DeserializeObject(resultContent);
                    return (bool)element.openTurn;
                }
            }
            return false;
        }

        internal void OpenTurn(int id)
        {
            var pairs = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("idUser", id.ToString())
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/turno/open/", new FormUrlEncodedContent(pairs)).Result;
        }

        internal void CloseTurn(object id)
        {
            var pairs = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("idUser", id.ToString())
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/turno/close/", new FormUrlEncodedContent(pairs)).Result;
        }

        internal string Folio(int id_user)
        {
            var pairs = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("idUser", id_user.ToString())
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/compras/folio/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    dynamic element = JsonConvert.DeserializeObject(resultContent);
                    return (string)element.folio;
                }
            }
            return "N";
        }

        internal List<SocioRecuperador> Recuperador(string id_recuperador, string nombre, string apellido_paterno, string apellido_materno)
        {
            if (id_recuperador.Trim().Length == 0)
            {
                id_recuperador = "0";
            }
            var pairs = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("idRecuperador", id_recuperador),
                new KeyValuePair<string, string>("nombre", nombre),
                new KeyValuePair<string, string>("apPaterno", apellido_paterno),
                new KeyValuePair<string, string>("apMaterno", apellido_materno)
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/recuperador/search/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null && resultContent != "null")

                    return JsonConvert.DeserializeObject<List<SocioRecuperador>>(resultContent);
            }
            return null;
        }


        internal int saveRecuperador(string nombre, string apPaterno, string apMaterno, string correo, string telefono)
        {
            var pairs = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("nombre",nombre),
            new KeyValuePair<string, string>("apPaterno",apPaterno),
            new KeyValuePair<string, string>("apMaterno",apMaterno),
            new KeyValuePair<string, string>("correo", correo),
            new KeyValuePair<string, string>("telefono",telefono)
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/recuperador/add/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)

            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    dynamic element = JsonConvert.DeserializeObject(resultContent);
                    return (int)element.id;
                }
            }
            return -1;
        }

        internal bool GuardarCompra(int id, int recuperadorId, int idSucursal, ObservableCollection<Material> materialesSeleccionados, int idFlete)
        {
            Compra c = new()
            {
                IdSucursal = idSucursal,
                IdUsuario = id,
                IdRecuperador = recuperadorId,
                IdFlete = idFlete,
                Detalle = materialesSeleccionados
            };
            double total = 0;
            foreach (Material m in materialesSeleccionados)
            {
                total += m.Total;
            }
            c.Total = total;

            using var client = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, new Uri(URL + "/basculas/api/compra/add/"));
            client.DefaultRequestHeaders.Add("key", KEY);
            message.Content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
            var response = client.SendAsync(message).Result;
            return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
        }

        internal double Total_Compras(int id_user)
        {

            var pairs = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("idUser",id_user.ToString()),
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/compras/cierre/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)

            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    dynamic element = JsonConvert.DeserializeObject(resultContent);
                    return (double)element.total;
                }
            }
            return -1;
        }

        internal double Efectivo_total(int id_user)
        {

            var pairs = new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("idUser",id_user.ToString()),
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/efectivo/total/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    dynamic element = JsonConvert.DeserializeObject(resultContent);
                    return (double)element.efectivo;
                }
            }
            return -1;
        }
        internal string GuardarCierre(int id_user, double entrada, double salida)
        {
            var pairs = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("idUser",id_user.ToString()),
                new KeyValuePair<string, string>("entrada",entrada.ToString()),
                new KeyValuePair<string, string>("salida",salida.ToString()),
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/cierre/save/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    dynamic element = JsonConvert.DeserializeObject(resultContent);
                    return (string)element.response;
                }
            }
            return "NO";
        }

        internal List<ReporteCompraNuevo> ComprasNuevo(int sucursal, DateTime fechaInicio, DateTime fechaFinal,string nombre)
        {
            var pairs = new List<KeyValuePair<string, string>> {
             new KeyValuePair<string, string>("sucursal", sucursal.ToString()),
             new KeyValuePair<string, string>("fechaInicio", fechaInicio.ToUniversalTime().ToString("yyyy'-'MM'-'dd")),
             new KeyValuePair<string, string>("fechaFinal", fechaFinal.ToUniversalTime().ToString("yyyy'-'MM'-'dd")),
              new KeyValuePair<string, string>("nombre", nombre),
            };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/report/compra/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    return JsonConvert.DeserializeObject<List<ReporteCompraNuevo>>(resultContent);
                }
            }
            return null;
        }

        internal List<ReporteCompra> Compras(int id, DateTime fechaIni, DateTime fechaFin)
        {
            var pairs = new List<KeyValuePair<string, string>> {
             new KeyValuePair<string, string>("idUser", id.ToString()),
             new KeyValuePair<string, string>("fechaIni", fechaIni.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK")),
             new KeyValuePair<string, string>("fechaFin", fechaFin.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK")),
            };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/compras/get/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                   return JsonConvert.DeserializeObject<List<ReporteCompra>>(resultContent);
                }
            }
            return null;
        }

        internal string CancelarCompra(int idCompra)
        {
            var pairs = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("idCompra",idCompra.ToString())
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/compras/cancel/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    dynamic element = JsonConvert.DeserializeObject(resultContent);
                    return (string)element.response;
                }
            }
            return "NO";
        }

        internal ObservableCollection<Material> Detalle(int idCompra)
        {
            var pairs = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("idCompra",idCompra.ToString())
           };

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/details/get/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    return JsonConvert.DeserializeObject<ObservableCollection<Material>>(resultContent);
                }
            }
            return null;
        }

        internal bool Impresion()
        {
            var pairs = new List<KeyValuePair<string, string>> {};

            var client = new HttpClient { BaseAddress = new Uri(URL) };
            client.DefaultRequestHeaders.Add("key", KEY);
            var response = client.PostAsync("/basculas/api/options/impresion/", new FormUrlEncodedContent(pairs)).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultContent = response.Content.ReadAsStringAsync().Result;
                if (resultContent != null)
                {
                    dynamic element = JsonConvert.DeserializeObject(resultContent);
                    return (bool)element.printLogo;
                }
            }
            return false;
        }
    }

}
