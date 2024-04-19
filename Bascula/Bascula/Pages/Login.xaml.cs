using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bascula.Components;
using Bascula.Model;
using Bascula.Service;

namespace Bascula
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Usuario.Focus();
            
        }

        private void Acceso_Click(object sender, RoutedEventArgs e)
        {

            User user = new ApiService().Enabled(Usuario.Text, Password.Password);
            if(user.Id > 0 & user.Enabled)//significa que si existe el usuario
            {   
                Bascula.MainWindow window = (MainWindow)Application.Current.MainWindow;
                window.user = user;
                window.Flag_close = true;
                window.Height = 608;
                window.Width = 1034;
                window.MinHeight = 608;
                window.MinWidth = 1034;
                window.MaxHeight = 100000;
                window.FrameBase.Source = new Uri("/Pages/Venta.xaml", UriKind.Relative);
                window.MaxWidth = 100000;
                window.FrameBase.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                //Center Window
                window.Left = (SystemParameters.PrimaryScreenWidth / 2) - (window.Width / 2);
                window.Top = (SystemParameters.PrimaryScreenHeight / 2) - (window.Height / 2);
                NavigationCommands.BrowseBack.InputGestures.Clear();
                NavigationCommands.BrowseForward.InputGestures.Clear();

            }
            else
            {
                MessageBox.Show("Usuario incorrecto o inactivo.","Aviso",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
            
        }

    }
}
