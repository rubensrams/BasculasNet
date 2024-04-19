using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using Bascula.Model;
using Bascula.Pages;
using Bascula.Service;

namespace Bascula
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal User user;
        internal bool Flag_close;

        public MainWindow()
        {
            InitializeComponent();
            Flag_close = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Flag_close)
            {
                var messageBoxResult = MessageBox.Show("¿Esta seguro de cerrar su turno?", "Peligro", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Cierre c = new Cierre(user.Id);
                    c.ShowDialog();
                    if (c.Flag)
                    {
                        System.Windows.Application.Current.Shutdown();
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
