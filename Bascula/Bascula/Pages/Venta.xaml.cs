using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Bascula.Components;
using Bascula.Model;
using Bascula.Service;


namespace Bascula.Pages
{
    /// <summary>
    /// Lógica de interacción para Venta.xaml
    /// </summary>
    public partial class Venta : Page
    {

        private readonly List<Material> ListaMateriales = new();
        private readonly ObservableCollection<Material> MaterialesSeleccionados = new();
        private User user;
        private int RecuperadorId;
        private string RecuperadorNombre;
        private string impresora;
        private int puerto;
        private string pesoLocal = "";

        public Venta()
        {
            InitializeComponent();
            DatosIniciales();
        }

        private void DatosIniciales()
        {
            Dictionary<string, string> dic =
                File.ReadLines(
                    System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ini.properties")
                    ).Select(s => s.Split('=')).Select(s => new {
                        key = s[0],
                        value = string.Join("=", s.Select((o, n) => new {
                            n,
                            o
                        }).Where(o => o.n > 0).Select(o => o.o))
                    }).ToDictionary(o => o.key, o => o.value);

            impresora = dic["IMPRESORA"];
            puerto = Int32.Parse(dic["PUERTO"]);

        }

        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Tickevent;
            timer.Start();
        }

        private void Tickevent(object? sender, EventArgs e)
        {
            Fecha.Content = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataCombos();
            user = ((Bascula.MainWindow)Application.Current.MainWindow).user;
            Fullname.Content = (user.Name + " " + user.LastName).ToUpper();
            Sucursal.Content = user.Sucursal;
            RecuperadorId = 0;

          //  using (WebClient cliente = new())
          //  {
                try
                {
                    //string nombreArchivo = user.Avatar.Split("/avatar/")[1];
                    //if (!File.Exists(@"C:\temp\" + nombreArchivo))
                    //{
                       // cliente.DownloadFile(new Uri(user.Avatar), @"C:\temp\" + nombreArchivo);

                    //}
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri(user.Avatar);
                    //logo.UriSource = new Uri(@"C:\temp\" + nombreArchivo);
                    logo.EndInit();
                    Avatar.Source = logo;

                }
                catch (Exception) { }
          //  }

            if (!new ApiService().ChecarTurno(user.Id)) //Servicio Regresa verdadero, existe turno abierto
            {
                new ApiService().OpenTurn(user.Id);
                MontoInicial m = new MontoInicial(user.Id, 1);
                m.ShowDialog();
            }

            Folio.Content = "FOLIO DE COMPRA:" + new ApiService().Folio(user.Id);
            StartClock();
            conectSerB1(); //CONECTAR CON BASCULAS
        }


        private void Pesar_Click(object sender, RoutedEventArgs e)
        {
            //Peso.Text = new Random().Next(80, 250) + ".00";
            //LoadPeso("peso");
            Peso.Text = pesoLocal;
        }

        private void CalculaTara_Click(object sender, RoutedEventArgs e)
        {
            //Tara.Text = new Random().Next(15, 50) + ".00";
            // LoadPeso("tara");
            Tara.Text = pesoLocal;
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {

            if (Producto.SelectedIndex != -1 && Cliente.Content != null && Cliente.Content.ToString() != "")
            {

                Material material = ListaMateriales.First(material => material.AutoIncrement == Producto.SelectedIndex);
                MaterialesSeleccionados.Add(new Material() { Id = material.Id, Nombre = material.Nombre, CostoKilo = material.CostoKilo, PesoBruto = Double.Parse(Peso.Text), PesoNeto = (Double.Parse(Peso.Text) - Double.Parse(Tara.Text)), Tara = Double.Parse(Tara.Text), Total = (Double.Parse(Peso.Text) - Double.Parse(Tara.Text)) * material.CostoKilo, });
                datos.ItemsSource = MaterialesSeleccionados;
                calculaSubtotales();

            }
            else
            {
                MessageBox.Show("Favor de llenar todos los campos.", "Error al agregar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Venta_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialesSeleccionados.Count == 0 || RecuperadorId == 0)
            {
                return;
            }

            if(Flete.SelectedIndex == -1)
            {
                MessageBox.Show("Favor de llenar todos los campos.", "Error al agregar", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("¿Desea Generar la Compra/Venta?", "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                bool impresionLogo = new ApiService().Impresion();
                string folioActual = new ApiService().Folio(user.Id);
                bool resultado = new ApiService().GuardarCompra(user.Id, RecuperadorId,user.IdSucursal, MaterialesSeleccionados,Flete.SelectedIndex);
                
                if (resultado)
                {
                    Impresion impresion = new()
                    {
                        ShowPrecio = impresionLogo,
                        Logo = impresionLogo,
                        Folio = folioActual,
                        Socio = RecuperadorId,
                        Flete = Flete.SelectedValue.ToString(),
                        Total = MaterialesSeleccionados.Sum(material => material.Total),
                        Atiende = (user.Name + " " + user.LastName).ToUpper(),
                        NombreSocio = RecuperadorNombre,
                        MaterialesSeleccionados = MaterialesSeleccionados,
                        Sucursal = user.Sucursal
                    };
                    try
                    {
                        impresion.Print(impresora);
                    }catch(Exception ex)
                    {
                        MessageBox.Show("Error en la impresora", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    var result =  MessageBox.Show("¿Desea volver a imprimir el ticket?","Aviso",MessageBoxButton.YesNo,MessageBoxImage.Question);
                    if(result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            impresion.Print(impresora);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error en la impresora", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    Limpiar();
                }
                else
                {
                    MessageBox.Show("Error al momento de guardar la compra.", "Error Compra", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private void Producto_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Producto.SelectedIndex != -1)
            {
                Producto.Text = Producto.SelectedValue.ToString();
                Material material = ListaMateriales.First(material => material.AutoIncrement == Producto.SelectedIndex);
                LeyendaKilo.Text = "PRECIO " + material.Nombre + "/" + string.Format(new CultureInfo("en-US"), "{0:C}", material.CostoKilo) + " X KILO";
            }
            else
            {
                LeyendaKilo.Text = "";
            }
        }

        private void Producto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Producto.SelectedIndex != -1)
            {
                Producto.Text = Producto.SelectedValue.ToString();
                Material material = ListaMateriales.First(material => material.AutoIncrement == Producto.SelectedIndex);
                LeyendaKilo.Text = "PRECIO " + material.Nombre + "/" + string.Format(new CultureInfo("en-US"), "{0:C}", material.CostoKilo) + " X KILO";
            }
            else
            {
                LeyendaKilo.Text = "";
            }
        }
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            int id_sucursal = 1;
            MontoInicial m = new MontoInicial(user.Id, id_sucursal);
            m.ExitForm = true;
            m.ShowDialog();
        }

        private void Cierre_Click(object sender, RoutedEventArgs e)
        {
            Cierre c = new Cierre(user.Id);
            c.ShowDialog();
            if (c.Flag)
            {
                Bascula.MainWindow window = (MainWindow)Application.Current.MainWindow;
                window.Flag_close = false;
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void AddSocio_Click(object sender, RoutedEventArgs e)
        {
            Recuperador recuperador = new Recuperador();
            recuperador.ShowDialog();
            RecuperadorId = recuperador.RecuperadorId;
            RecuperadorNombre = recuperador.NombreRecuperador.ToUpper();
            Cliente.Content = RecuperadorNombre;
        }

        private void calculaSubtotales()
        {
            Total.Content = string.Format(new CultureInfo("en-US"), "{0:C}", MaterialesSeleccionados.Sum(material => material.Total));
        }

        private void Consulta_Click(object sender, RoutedEventArgs e)
        {
            Consulta c = new Consulta(user.Id,user.IdSucursal, (user.Name + " " + user.LastName).ToUpper());
            c.ShowDialog();
        }

        private void DataCombos()
        {

            ListaMateriales.Clear();

            List<ListaMaterial> materiales = new ApiService().Materiales();
            for (int i = 0; i < materiales.Count; i++)
            {

                ListaMaterial material = materiales[i];
                ListaMateriales.Add(new Material() { AutoIncrement = i, Id = material.Id, Nombre = material.Code, CostoKilo = material.Precio });
            }

            foreach (Material m in ListaMateriales)
            {
                Producto.Items.Insert(m.AutoIncrement, m.Nombre);
            }

            Flete.Items.Add("ORIGEN");
            Flete.Items.Add("BODEGA");
        }

        private void Limpiar()
        {
            //cargar de nuevo la lista de materiales.
            Peso.Text = "0.00";
            Tara.Text = "0.00";
            Producto.Items.Clear();
            Cliente.Content = "";
            Flete.Items.Clear();
            MaterialesSeleccionados.Clear();
            datos.ItemsSource = MaterialesSeleccionados;
            Folio.Content = "FOLIO DE COMPRA:" + new ApiService().Folio(user.Id);
            LeyendaKilo.Text = "";
            DataCombos();
            calculaSubtotales();
        }

        /// <summary>
        /// Method for get id from press button
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">event</param>
        private void ShowHideDetails(object sender, RoutedEventArgs e)
        {
            var row = GetParent<DataGridRow>((Button)sender);
            MaterialesSeleccionados.RemoveAt(datos.Items.IndexOf(row.Item));
            calculaSubtotales();
        }

        private TargetType GetParent<TargetType>(DependencyObject o)
            where TargetType : DependencyObject
        {
            if (o == null || o is TargetType) return (TargetType)o;
            return GetParent<TargetType>(VisualTreeHelper.GetParent(o));
        }

        /*
         *NUEVAS FUNCIONES para la lectura de basculas 
         *
         */

        #region conexion_ethernet

        private Socket socketB1;
        private Socket sockSerB1;
        public AsyncCallback m_pfnCallBackB1;
        private byte[] BufferB1 = new byte[512];
        private delegate void delegado(string accion);

        private void conectSerB1()
        {
            try
            {
                this.sockSerB1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.sockSerB1.Bind((EndPoint)new IPEndPoint(IPAddress.Any, puerto));
                //Cozumel -> 8004
                //Holbox-> 8209
                //Isla mujeres -> 8209
                //cozumel impresora -> EC-PM-5895X 
                this.sockSerB1.Listen(5);
                this.sockSerB1.BeginAccept(new AsyncCallback(this.OnClientConnectB1), null);
            }
            catch (SocketException ex)
            {
                //MessageBox.Show("error en Declaracion: " + ex.Message);
            }
        }


        public void OnClientConnectB1(IAsyncResult asyn)
        {
            try
            {
                this.socketB1 = this.sockSerB1.EndAccept(asyn);
                Dispatcher.Invoke((Delegate)new delegado(this.MensajeB1), "u");
                this.sockSerB1.Close();
                this.conectSerB1();
                this.EsperaDatosB1();
            }
            catch (ObjectDisposedException ex)
            {
                //MessageBox.Show("error en Conexion 1: " + "OnClientConnection: Socket has been closed");
            }
            catch (SocketException ex)
            {
                //MessageBox.Show("error en Conexion: " + ex.Message);
            }
        }

        private void EsperaDatosB1()
        {
            try
            {
                if (this.m_pfnCallBackB1 == null)
                    this.m_pfnCallBackB1 = new AsyncCallback(this.ReceivedDataB1);
                this.socketB1.BeginReceive(this.BufferB1, 0, this.BufferB1.Length, SocketFlags.None, new AsyncCallback(this.ReceivedDataB1), null);
            }
            catch (SocketException ex)
            {
                //MessageBox.Show("error en Datos: " + ex.Message);
            }
        }

        public void ReceivedDataB1(IAsyncResult ar)
        {
            int count;
            try
            {
                lock (this.socketB1)
                    count = this.socketB1.EndReceive(ar);
            }
            catch
            {
                return;
            }
            if (count == 0)
                return;
            try
            {
                new delegado(this.MostrarB1)(Encoding.ASCII.GetString(this.BufferB1, 0, count));
            }
            catch
            {
                return;
            }
            this.socketB1.BeginReceive(this.BufferB1, 0, this.BufferB1.Length, SocketFlags.None, new AsyncCallback(this.ReceivedDataB1), null);
        }


        private void MostrarB1(string d)
        {
            //En esta variable viene el valor del peso en una cadena de caracteres.
            string temporal = d;
            try
            {
               pesoLocal = temporal.Split("kg")[0].Split("+")[1].Trim();
            }
            catch (Exception ex)
            {
                pesoLocal = "0.00";
            }

        }

        private void MensajeB1(string d)
        {
            this.labelPeso.Content = "Báscula 1 en línea";
        }

        #endregion

        #region pruebas_serialPort
        /*PRUEBAS DE SERIAL PORT*/
        /*
       
        private SerialPort serialPort1;
        private delegate void DelegadoAcceso(string accion);
        
        private void LoadPeso(string tipo) {

            try
            {
                serialPort1 = new SerialPort(puerto, 9600, Parity.None, 8, StopBits.One);
                serialPort1.Handshake = Handshake.None;

                serialPort1.WriteTimeout = 200;
                serialPort1.ReadTimeout = 200;
                if(tipo == "peso")
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(Sp_dataPesoRecibe);
                else
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(Sp_dataTaraRecibe);
                serialPort1.Open();
                serialPort1.Write("P");
                Thread.Sleep(400);
                serialPort1.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        } 

        public void Sp_dataTaraRecibe(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort1.ReadExisting();
            data = DoFormat(Double.Parse(data.Replace("kg", "").Trim())).ToString();
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DelegadoAcceso(WriteDataTara), data);
        }

        private void WriteDataTara(string data)
        {
            Tara.Text = data;
        }


        public void Sp_dataPesoRecibe(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort1.ReadExisting();
            data = DoFormat(Double.Parse(data.Replace("kg", "").Trim())).ToString();
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DelegadoAcceso(WriteDataPeso),data);
        }
        

        private void WriteDataPeso(string data)
        {
           Peso.Text = data;
        }
        
       

        public static string DoFormat(double myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int)myNumber).ToString();
            }
            else
            {
                return s;
            }
        } */

        #endregion

    }
}