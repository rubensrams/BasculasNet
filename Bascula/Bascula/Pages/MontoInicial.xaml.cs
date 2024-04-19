using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Bascula.Service;

namespace Bascula.Pages
{
    /// <summary>
    /// Lógica de interacción para MontoInicial.xaml
    /// </summary>
    public partial class MontoInicial : Window
    {
        public bool ExitForm { get; set; }
        private int id_user;
        private int id_sucursal;

        public MontoInicial(int id_user, int id_sucursal)
        {
            InitializeComponent();
            this.id_user = id_user;
            this.id_sucursal = id_sucursal;
            this.ExitForm = false;
        }

        private void Cantidad_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^^0-9.]+");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Cantidad.Text.Trim().Length > 0)
            {
                var result = MessageBox.Show("¿Desea registrar la cantidad de: " + Convert.ToDecimal(Cantidad.Text.Trim()).ToString("C2", new CultureInfo("es-MX")) + "?", "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ExitForm = true;
                    new ApiService().Dinero(id_user, id_sucursal, Convert.ToDecimal(Cantidad.Text.Trim()));
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Ingrese una cantidad", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!ExitForm)
                e.Cancel = true;
        }
    }
}
