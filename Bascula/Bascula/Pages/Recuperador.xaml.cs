using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
using Bascula.Model;
using Bascula.Service;

namespace Bascula.Pages
{
    /// <summary>
    /// Lógica de interacción para Recuperador.xaml
    /// </summary>
    public partial class Recuperador : Window
    {

        private bool _isNew;
        internal int RecuperadorId { get; set; }
        internal String NombreRecuperador { get; set; }

        public Recuperador()
        {
            InitializeComponent();
            _isNew = false;
            RecuperadorId = 0;
            NombreRecuperador = "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SocioBusqueda.IsEnabled = false;
            NombreBusqueda.IsEnabled = false;
            ApPaternoBusqueda.IsEnabled = false;
            ApMaternoBusqueda.IsEnabled = false;
            SocioBusqueda.Text = "";
            NombreBusqueda.Text = "";
            ApPaternoBusqueda.Text = "";
            ApMaternoBusqueda.Text = "";

            panelRegistro.Visibility = Visibility.Visible;
            _isNew = true;
            Nombre.Text = "";
            APPaterno.Text = "";
            APMaterno.Text = "";
            Email.Text = "";
            Telefono.Text = "";
            Nombre.IsEnabled = true;
            APPaterno.IsEnabled = true;
            APMaterno.IsEnabled = true;
            Email.IsEnabled = true;
            Telefono.IsEnabled = true;
            Buscar.IsEnabled = false;
            RecuperadorId = 0;
            NombreRecuperador = "";

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (_isNew)
            {
                if(Nombre.Text.Trim() =="" || APPaterno.Text.Trim() == "" || APMaterno.Text.Trim() == "")
                {
                    MessageBox.Show("Los campos de nombre no pueden quedar vacios.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //agrega y selecciona
               int nuevo = new ApiService().saveRecuperador(Nombre.Text, APPaterno.Text, APMaterno.Text, Email.Text, Telefono.Text);
               if(nuevo > 0)
                {
                    RecuperadorId = nuevo;
                    NombreRecuperador = Nombre.Text + " " + APPaterno.Text + " " + APMaterno.Text;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el nuevo socio. ","Advertencia",MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                //selecciona
                this.Close();
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
  

            List<SocioRecuperador> recuperador = new ApiService().Recuperador(SocioBusqueda.Text.Trim(),
                string.IsNullOrEmpty(NombreBusqueda.Text.Trim()) ? null: NombreBusqueda.Text.Trim(),
                string.IsNullOrEmpty(ApPaternoBusqueda.Text.Trim()) ? null : ApPaternoBusqueda.Text.Trim(),
                string.IsNullOrEmpty(ApMaternoBusqueda.Text.Trim()) ? null : ApMaternoBusqueda.Text.Trim());


            if (recuperador.Count == 0)
            {
                MessageBox.Show("Socio no encontrado", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (recuperador.Count == 1)
            {
                panelRegistro.Visibility = Visibility.Visible;
                Nombre.IsEnabled = false;
                APPaterno.IsEnabled = false;
                APMaterno.IsEnabled = false;
                Email.IsEnabled = false;
                Telefono.IsEnabled = false;
                //si encontrado
                Nombre.Text = recuperador[0].Nombre;
                APPaterno.Text = recuperador[0].Apellido_paterno;
                APMaterno.Text = recuperador[0].Apellido_materno;
                Email.Text = recuperador[0].Email;
                Telefono.Text = recuperador[0].Telefono;
                RecuperadorId = recuperador[0].Id_recuperador;
                NombreRecuperador = Nombre.Text + " " + APPaterno.Text + " " + APMaterno.Text;
            }
            else
            {
                MessageBox.Show("La búsqueda trae más de un socio, la búsqueda debe ser más especifica", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }

        private void SocioBusqueda_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
        private void Valida_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            RecuperadorId = 0;
            NombreRecuperador = "";
            this.Close();
        }

    }
}
