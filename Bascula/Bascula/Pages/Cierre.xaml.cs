using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Bascula.Model;
using Bascula.Service;

namespace Bascula.Pages
{
    /// <summary>
    /// Lógica de interacción para Cierre.xaml
    /// </summary>
    public partial class Cierre : Window
    {
        private int Id_user;
        public bool Flag { get; set; }

        private double entrada;

        private double salida;

        public Cierre(int id_user)
        {
            InitializeComponent();
            this.Id_user = id_user;
            Flag = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            string resultado = new ApiService().GuardarCierre(Id_user, entrada, salida);
            if(resultado == "OK")
            {
                new ApiService().CloseTurn(Id_user);
            }
            else
            {
                MessageBox.Show("Ocurrio un error al guardar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Flag = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            entrada = new ApiService().Efectivo_total(Id_user);
            Apertura.Text = string.Format(new CultureInfo("en-US"), "{0:C}", entrada);
            salida = new ApiService().Total_Compras(Id_user);
            Compras.Text = string.Format(new CultureInfo("en-US"), "{0:C}", salida);
            Diferencia.Text = string.Format(new CultureInfo("en-US"), "{0:C}", entrada - salida);
            Restante.Text = string.Format(new CultureInfo("en-US"), "{0:C}", entrada - salida < 0 ? 0 : entrada - salida);
            Diferencia.Foreground = entrada - salida < 0 ? Brushes.Crimson : Brushes.Black;
        }
    }
}
