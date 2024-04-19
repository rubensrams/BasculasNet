using System;   
using System.Windows.Media;
using System.Windows;
using System.Drawing.Printing;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Printing;
using System.Linq;
using Bascula.Pages;
using System.Reflection;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Text;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Collections.ObjectModel;
using Bascula.Model;

namespace Bascula.Components
{
    internal class Impresion
    {

        public bool Logo { get; set; }

        public bool ShowPrecio { get; set; }

        public string Folio { get; set; } = string.Empty;

        public string Sucursal { get; set; } = string.Empty;

        public int Socio { get; set; }

        public string Flete { get; set; } = string.Empty;

        public double Total { get; set; }

        public string Atiende { get; set; } = string.Empty;

        public string NombreSocio { get; set; } = string.Empty;

        public ObservableCollection<Material>? MaterialesSeleccionados { get; set; }

        public void Print(string impresora)
        {

            PrintDialog pDialog = new()
            {
                PageRangeSelection = PageRangeSelection.AllPages,
                UserPageRangeEnabled = true,
                PrintQueue = new PrintServer().GetPrintQueue(impresora)
            };

            IDocumentPaginatorSource idpSource = CreateFlowDocument();
            pDialog.PrintDocument(idpSource.DocumentPaginator, String.Empty);

        }

        private FlowDocument CreateFlowDocument()
        {

            string separator = "========================================";
            string simpleSeparator = "****************************************";
            string firma = "----------------------------------------";

            // Create a FlowDocument  
            FlowDocument doc = new FlowDocument();

            doc.FontFamily = new FontFamily("MS Gothic");
            doc.FontSize = 11.0;
            // Create a Section  
            Section sec = new Section();

            // Create first Paragraph  
            Paragraph p1 = new Paragraph();
            p1.LineHeight = 14;

            string ticket = Environment.NewLine;
            ticket +=Environment.NewLine;
            ticket += DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + Environment.NewLine;
            ticket += "Sucursal: " + Sucursal + Environment.NewLine;
            ticket += "Folio: " + Folio.PadLeft(6,'0') +"     Socio: "+ Socio.ToString().PadLeft(6,'0') + Environment.NewLine;
            ticket += "Flete: "+ Flete + Environment.NewLine;

            ticket += separator + Environment.NewLine;
            if (ShowPrecio)
            {
                ticket += "Clave         Precio" + Environment.NewLine;
                ticket += "Peso Neto     Total" + Environment.NewLine;
            }
            else
            {
                ticket += "Clave" + Environment.NewLine;
                ticket += "Peso Neto" + Environment.NewLine;
            }

            ticket += separator + Environment.NewLine;

            if (ShowPrecio)
            {
                foreach (Material material in MaterialesSeleccionados)
                {
                    ticket += material.Nombre + "      " + string.Format(new CultureInfo("en-US"), "{0:C}", material.CostoKilo) + Environment.NewLine;
                    ticket += Math.Round(material.PesoNeto,2) + " KG        " + string.Format(new CultureInfo("en-US"), "{0:C}", material.Total) + Environment.NewLine;
                }
                ticket += simpleSeparator + Environment.NewLine;
                ticket += "TOTAL: " + string.Format(new CultureInfo("en-US"), "{0:C}", Total) + " MXN" + Environment.NewLine;
            }
            else
            {
                foreach(Material material in MaterialesSeleccionados)
                {
                    ticket += material.Nombre + Environment.NewLine;
                    ticket += Math.Round(material.PesoNeto, 2) + " KG" + Environment.NewLine;
                }
            }

            
            ticket += separator + Environment.NewLine;

            ticket += Environment.NewLine;
            ticket += Environment.NewLine;
            ticket += Environment.NewLine;
            ticket += Environment.NewLine;

            ticket += firma + Environment.NewLine;
            ticket += "Atendió: " +Atiende + Environment.NewLine;


            ticket += Environment.NewLine;
            ticket += Environment.NewLine;
            ticket += Environment.NewLine;
            ticket += Environment.NewLine;

            ticket += firma + Environment.NewLine;
            ticket += "Socio: " + NombreSocio + Environment.NewLine;

            ticket += Environment.NewLine;
            ticket += Environment.NewLine;
            ticket += Environment.NewLine;
            ticket += Environment.NewLine;

            ticket += ".";

            Image img = new Image();
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\logo.png");
            bi3.EndInit();
            img.Width = 170;
            img.Source = bi3;

            if(Logo)
                p1.Inlines.Add(img);
            
            p1.Inlines.Add(ticket);
            sec.Blocks.Add(p1);
 
            doc.Blocks.Add(sec);

            return doc;
        }

    }
}