using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Bascula.Service;
using Microsoft.Win32;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace Bascula.Pages
{
    /// <summary>
    /// Lógica de interacción para Consulta.xaml
    /// </summary>
    public partial class Consulta : Window
    {
        private int IdUser;
        private int IdSucursal;
        private string nombreUsuario;

        public Consulta(int IdUser, int IdSucursal, string nombreUsuario)
        {
            this.IdUser = IdUser;
            this.IdSucursal = IdSucursal;
            this.nombreUsuario = nombreUsuario;
            InitializeComponent();
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            busqueda();
        }

        private void Exportar_Click_PDF(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialogPDF = new SaveFileDialog();
            saveFileDialogPDF.Filter = "PDF file (*.pdf |*.pdf";
            if (saveFileDialogPDF.ShowDialog() == true)

            {
                List<ReporteCompraNuevo> compras = null;
                if (FechaInicial.SelectedDate != null && FechaFinal.SelectedDate != null)
                {
                   compras = new ApiService().ComprasNuevo(IdSucursal, FechaInicial.SelectedDate.Value.Date, FechaFinal.SelectedDate.Value.Date.AddDays(1).AddTicks(-1), nombreUsuario);
                }

                if(compras == null || compras.Count == 0)
                {
                    MessageBox.Show("No se encontraron datos","Aviso.",MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using FileStream stream = new(saveFileDialogPDF.FileName, FileMode.Create);
                string html = "<table style='width:100%; font-size:12px;'><tr style=\"font-weight: bold; background-color:#99CC00; margin:0\"><td>Fecha</td><td>Sucursal</td><td>Recuperador</td><td>Usuario</td><td>Estatus</td><td>Peso Neto</td><td>Precio</td><td>Total</td></tr>";
                foreach(ReporteCompraNuevo compra in compras)
                {
                    html += "<tr>";
                    html += "<td> "+ compra.fecha + "</td>";
                    html += "<td>" + compra.sucursal + "</td>";
                    html += "<td>" + compra.recuperador + "</td>";
                    html += "<td>" + compra.usuario + "</td>";
                    html += "<td>" + compra.estatus + "</td>";
                    html += "<td>" + compra.pesoNeto + "</td>";
                    html += "<td>" + compra.precio + "</td>";
                    html += "<td>" + compra.total + "</td>";
                    html += "</tr>";
                }
                html += "</table>";

                Document document = new Document(PageSize.A4, 5, 5, 5, 5);
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);

                document.Open();
                using (StringReader reader = new StringReader(html))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(pdfWriter, document, reader);
                }
                document.Close();
                stream.Close();

            }
        }

        private void Exportar_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {

                List<ReporteCompraNuevo> compras = null;
                if (FechaInicial.SelectedDate != null && FechaFinal.SelectedDate != null)
                {
                    compras = new ApiService().ComprasNuevo(IdSucursal, FechaInicial.SelectedDate.Value.Date, FechaFinal.SelectedDate.Value.Date.AddDays(1).AddTicks(-1), nombreUsuario);
                }

                if (compras == null || compras.Count == 0)
                {
                    MessageBox.Show("No se encontraron datos", "Aviso.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                string body = "FECHA,SUCURSAL,RECUPERADOR,USUARIO,ESTATUS,PESO NETO,PRECIO,TOTAL" + System.Environment.NewLine;
                foreach (ReporteCompraNuevo compra in compras)
                {
                    body += compra.fecha + ",";
                    body += compra.sucursal + ",";
                    body += compra.recuperador + ",";
                    body += compra.usuario + ",";
                    body += compra.estatus + ",";
                    body += compra.pesoNeto + ",";
                    body += compra.precio + ",";
                    body += compra.total + System.Environment.NewLine;
                }
                File.WriteAllText(saveFileDialog.FileName, body);
            }
            
        }

       private void SendCancel(object sender, RoutedEventArgs e)
        {
            var row = GetParent<DataGridRow>((Button)sender);
            ReporteCompra reporteCompra = (ReporteCompra)row.Item;

            if(reporteCompra.Estatus == "Vigente")
            {
                var resultado = MessageBox.Show("¿Realmente desea enviar a cancelación la compra seleccionada?", "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if(resultado == MessageBoxResult.No)
                {
                    return;
                }
            }
            
            if (reporteCompra.Estatus == "Pendiente")
            {
                MessageBox.Show("Esta compra ya ha sido enviada para su cancelación", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (reporteCompra.Estatus == "Cancelado")
            {
                MessageBox.Show("Esta compra ya ha sido cancelada", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                new ApiService().CancelarCompra(reporteCompra.IdCompra);
                busqueda();
            }



        }

        private TargetType GetParent<TargetType>(DependencyObject o)
            where TargetType : DependencyObject
        {
            if (o == null || o is TargetType) return (TargetType)o;
            return GetParent<TargetType>(VisualTreeHelper.GetParent(o));
        }

        private void busqueda()
        {
            if (FechaInicial.SelectedDate != null && FechaFinal.SelectedDate != null)
            {
                Grid.ItemsSource = new ApiService().Compras(IdUser, FechaInicial.SelectedDate.Value.Date, FechaFinal.SelectedDate.Value.Date.AddDays(1).AddTicks(-1));
            }
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReporteCompra item = (ReporteCompra)Grid.SelectedItem;
            if(item == null) return;
            ObservableCollection<Material> materials = new ApiService().Detalle(item.IdCompra);
            Detalle.ItemsSource = materials;
        }

    }
}
