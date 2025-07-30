using System;
using System.IO;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Drawing.Printing;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Management;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;


namespace ImpresionGPC
{
    public partial class Form1 : Form
    {
        private string nombreImpresora;
        private List<Pedido> listaPedidos;
        private Panel pnlContenedor;
        private string nombreArchivoSeleccionado = "";
        private Label lblNombreArchivo;
        private Button btnEliminarArchivo;

        public Form1()
        {
            InitializeComponent();
            listaPedidos = new List<Pedido>();
            LlenarComboBoxImpresoras();
            DiseñarInterfaz();
        }

        private class Pedido
        {
            public string CodigoMenu { get; set; }
            public string NombreMenu { get; set; }
            public string LugarEntrega { get; set; }
            public string NombreEmpleado { get; set; }
            public DateTime FechaEnvasado { get; set; }
        }

        private void btnSeleccionarExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos Excel (*.xlsx;*.xls)|*.xlsx;*.xls|Todos los archivos (*.*)|*.*";
            openFileDialog.Title = "Seleccionar Archivo Excel";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string rutaArchivo = openFileDialog.FileName;
                nombreArchivoSeleccionado = Path.GetFileName(rutaArchivo);
                lblNombreArchivo.Text = "Archivo: " + nombreArchivoSeleccionado;
                lblNombreArchivo.Visible = true;
                btnEliminarArchivo.Visible = true;
                LeerDatosExcel(rutaArchivo);
                btnEstablecerVencimiento.Visible = true;
            }
        }

        private void btnEliminarArchivo_Click(object sender, EventArgs e)
        {
            nombreArchivoSeleccionado = "";
            lblNombreArchivo.Text = "";
            lblNombreArchivo.Visible = false;
            btnEliminarArchivo.Visible = false;
            btnEstablecerVencimiento.Visible = false;
            dtpVencimiento.Visible = false;
            btnImprimirEtiqueta.Visible = false;
            lblFechaVencimientoSeleccionada.Visible = false;
            listaPedidos.Clear();
        }

        private void LeerDatosExcel(string rutaArchivo)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            Excel.Worksheet xlWorksheet = null;
            Excel.Range xlRange = null;

            try
            {
                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Open(rutaArchivo);
                xlWorksheet = xlWorkbook.Sheets[1];
                xlRange = xlWorksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;


                listaPedidos.Clear();

                for (int i = 3; i <= rowCount; i++)
                {
                    string codigoMenu = xlRange.Cells[i, 1]?.Value2?.ToString();
                    string nombreMenu = xlRange.Cells[i, 2]?.Value2?.ToString();
                    string nombreEmpleado = xlRange.Cells[i, 3]?.Value2?.ToString();
                    string lugarEntrega = xlRange.Cells[i, 4]?.Value2?.ToString();
                    DateTime fechaEnvasado = DateTime.Now;

                    if (!string.IsNullOrEmpty(codigoMenu) && !string.IsNullOrEmpty(nombreMenu) && !string.IsNullOrEmpty(lugarEntrega))
                    {
                        listaPedidos.Add(new Pedido
                        {
                            CodigoMenu = codigoMenu,
                            NombreMenu = nombreMenu,
                            LugarEntrega = lugarEntrega,
                            NombreEmpleado = nombreEmpleado,
                            FechaEnvasado = fechaEnvasado
                        });

                    }
                    else
                    {

                    }
                }

                if (listaPedidos.Count == 0)
                {
                    MessageBox.Show("No se encontraron datos de pedidos en el archivo Excel.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnEstablecerVencimiento.Visible = false;
                }
                else
                {
                    btnEstablecerVencimiento.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer el archivo Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (xlRange != null) Marshal.ReleaseComObject(xlRange);
                if (xlWorksheet != null) xlWorkbook.Close();
                if (xlApp != null) xlApp.Quit();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void btnEstablecerVencimiento_Click(object sender, EventArgs e)
        {
            dtpVencimiento.Visible = true;
            btnImprimirEtiqueta.Visible = true;
            lblFechaVencimientoSeleccionada.Text = "Fecha Vencimiento:";
        }

        private void btnImprimirEtiqueta_Click(object sender, EventArgs e)
        {
            DateTime fechaVencimientoSeleccionada = dtpVencimiento.Value.Date;

            if (listaPedidos == null || listaPedidos.Count == 0)
            {
                MessageBox.Show("No hay pedidos para imprimir. Por favor, selecciona un archivo Excel con pedidos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            nombreImpresora = cmbImpresoras.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(nombreImpresora))
            {
                MessageBox.Show("No se ha seleccionado ninguna impresora.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Variables para el contador de etiquetas
            int totalEtiquetasEnExcel = listaPedidos.Count;
            int etiquetasImpresasExitosamente = 0;
            StringBuilder erroresImpresion = new StringBuilder();

            foreach (Pedido pedido in listaPedidos)
            {
                string zplCommands = "";
                try
                {
                    zplCommands = GenerarZpl(pedido.NombreMenu, pedido.FechaEnvasado, fechaVencimientoSeleccionada, pedido.LugarEntrega, pedido.CodigoMenu, pedido.NombreEmpleado);

                    if (string.IsNullOrEmpty(zplCommands) || zplCommands.Contains("ERROR: INVALID BC DATA"))
                    {
                        erroresImpresion.AppendLine($"No se pudo generar un ZPL válido para el pedido: '{pedido.NombreMenu}' (Código: '{pedido.CodigoMenu}'). Saltando este pedido.");
                        continue;
                    }

                    bool impresionActualExitosa = false;

                    if (nombreImpresora.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
                    {
                        impresionActualExitosa = SendZplToSerialPrinter(zplCommands, nombreImpresora);
                    }
                    else
                    {
                        impresionActualExitosa = RawPrinterHelper.SendStringToPrinter(nombreImpresora, zplCommands);
                    }

                    if (impresionActualExitosa)
                    {
                        etiquetasImpresasExitosamente++; // Incrementa el contador de éxito
                    }
                    else
                    {
                        erroresImpresion.AppendLine($"Fallo al imprimir la etiqueta para: '{pedido.NombreMenu}'.");
                    }
                }
                catch (Exception ex)
                {
                    erroresImpresion.AppendLine($"Error inesperado al procesar el pedido '{pedido.NombreMenu}': {ex.Message}");
                }
            }

            // --- Lógica del mensaje final actualizada ---
            int etiquetasFallidas = totalEtiquetasEnExcel - etiquetasImpresasExitosamente;
            string mensajeFinal = $"Se intentaron procesar {totalEtiquetasEnExcel} etiquetas del archivo Excel.\n";

            if (etiquetasImpresasExitosamente > 0)
            {
                mensajeFinal += $"Se imprimieron {etiquetasImpresasExitosamente} etiquetas exitosamente.\n";
            }

            if (erroresImpresion.Length > 0)
            {
                mensajeFinal += $"Ocurrieron {etiquetasFallidas} fallos de impresión. Detalles:\n\n{erroresImpresion.ToString()}";
                MessageBox.Show(mensajeFinal, "Resultado de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (etiquetasImpresasExitosamente > 0)
            {
                MessageBox.Show(mensajeFinal + "Todas las etiquetas se imprimieron exitosamente.", "Impresión Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else // No hubo impresiones exitosas y no hay errores detallados (ej. lista vacía al principio)
            {
                MessageBox.Show("No se pudo imprimir ninguna etiqueta. Revise la configuración y los datos.", "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // --- Fin de la lógica del mensaje final ---

            // Ocultar los controles después de intentar imprimir
            btnEstablecerVencimiento.Visible = false;
            dtpVencimiento.Visible = false;
            btnImprimirEtiqueta.Visible = false;
            lblFechaVencimientoSeleccionada.Text = "";
            // ** LÍNEAS PARA LIMPIAR EL "SELECCIONAR ARCHIVO" **

            lblNombreArchivo.Text = string.Empty; 
            btnEliminarArchivo.Visible = false;      
            listaPedidos.Clear();                    
             
            btnSeleccionarExcel.Enabled = true;            
            btnSeleccionarExcel.Visible = true;
        }

        private string GenerarZpl(string nombreProducto, DateTime fechaEnvasado, DateTime fechaVencimiento, string lugarEntrega, string codigoBarra, string nombreEmpleado)
        {
            string zpl = "^XA\n";
            // Ancho de la etiqueta
            zpl += "^PW400\n";
            // Origen de la etiqueta
            zpl += "^LH20,20\n";

            // LUGAR DE ENTREGA (ARRIBA Y CENTRADO, AJUSTADO )
            if (!string.IsNullOrEmpty(lugarEntrega))
            {
                // Tamaño de fuente
                zpl += "^CF0,30\n";
                // ^FO0,5. Mantenemos 5 para que no se salga del margen superior.
                zpl += "^FO0,5^FB360,40,1,C^FDLUGAR: " + lugarEntrega + "^FS\n";
            }
            // Fuente por defecto, altura 25
            zpl += "^CF0,25\n";
            // ^FO10,45 (55 - 10)
            zpl += "^A0,30,30^FO10,45^FD" + nombreEmpleado + "^FS\n";


            zpl += "^CF0,25\n";
            //  ^FO10,80 (90 - 10)
            zpl += "^FO10,80^FDMenu: " + nombreProducto + "^FS\n";

            // Fecha de Elaboración
            zpl += "^CF0,25\n";
            //  ^FO10,115 (125 - 10)
            zpl += "^FO10,115^FDELAB: " + fechaEnvasado.ToString("dd/MM/yyyy") + "^FS\n";

            // Fecha de Vencimiento
            zpl += "^CF0,25\n";
            // ^FO10,150 (160 - 10)
            zpl += "^FO10,150^FDVENC: " + fechaVencimiento.ToString("dd/MM/yyyy") + "^FS\n";


            // CÓDIGO DE BARRAS EAN-13

            // Limpiar el string para asegurar que solo contenga dígitos.
            string cleanedCodigoBarra = new string(codigoBarra.Where(char.IsDigit).ToArray());

            string baseCodigoBarra;
            if (cleanedCodigoBarra.Length >= 12)
            {
                baseCodigoBarra = cleanedCodigoBarra.Substring(0, 12);
            }
            else
            {
                baseCodigoBarra = cleanedCodigoBarra.PadLeft(12, '0');
            }

            if (string.IsNullOrEmpty(baseCodigoBarra) || baseCodigoBarra.Length != 12)
            {
                //  ^FO0,270 (280 - 10)
                zpl += "^FO0,270^FDERROR: DATOS DE CODIGO DE BARRA INVALIDOS^FS\n";
                zpl += "^XZ\n";
                return zpl;
            }

            char checkDigit;
            try
            {
                checkDigit = CalculateEan13Checksum(baseCodigoBarra);
            }
            catch (ArgumentException ex)
            {
                //  ^FO0,270 (280 - 10)
                zpl += $"^FO0,270^FDERROR BC CHKSUM: {ex.Message.Replace("\n", " ")}^FS\n";
                zpl += "^XZ\n";
                return zpl;
            }
            catch (FormatException ex)
            {
                //  ^FO0,270 (280 - 10) 
                zpl += $"^FO0,270^FDERROR BC FORMAT: {ex.Message.Replace("\n", " ")}^FS\n";
                zpl += "^XZ\n";
                return zpl;
            }

            string fullEan13 = baseCodigoBarra + checkDigit;

            // Reducimos el module width a 2. La altura la mantenemos en 120.
            zpl += "^BY2,3.0,120^FS\n";

            //  ^FO40,175 (185 - 10)
            zpl += "^FO80,175^BE,Y,Y\n";  //FO80 centra el código de barras en la etiqueta de la etiqueta
            zpl += "^FD" + fullEan13 + "^FS\n";

            zpl += "^XZ\n";
            return zpl;
            // Finaliza el formato ZPL
        }

        public static char CalculateEan13Checksum(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length != 12)
            {
                throw new ArgumentException($"EAN-13 checksum calculation requires exactly 12 digits. Received length: {data?.Length ?? 0}, data: \"{data}\"");
            }

            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit;
                if (!int.TryParse(data[i].ToString(), out digit))
                {
                    throw new FormatException($"Carácter no numérico o inválido para EAN-13 encontrado en la posición {i}: '{data[i]}'. Cadena completa: \"{data}\"");
                }

                if ((i + 1) % 2 == 1)
                {
                    sum += digit;
                }
                else
                {
                    sum += digit * 3;
                }
            }

            int totalSumModulo10 = sum % 10;
            int checksum = (10 - totalSumModulo10) % 10;

            return checksum.ToString()[0];
        }

        private void dtpVencimiento_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void PrintRawZpl(string zplCommands, string printerName)
        {
            using (PrintDocument pd = new PrintDocument())
            {
                pd.PrinterSettings.PrinterName = printerName;
                pd.PrintPage += (sender, ev) =>
                {
                    // ¡Advertencia! Esto NO envía ZPL "raw". Intentará dibujar el texto ZPL como gráficos.                   
                    ev.Graphics.DrawString(zplCommands, new Font("Arial", 10), Brushes.Black, 0, 0);
                };
                try
                {
                    pd.Print();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al imprimir la etiqueta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SendZplToNetworkPrinter(string zplCommands, string printerIP, int printerPort)
        {
            try
            {
                TcpClient client = new TcpClient(printerIP, printerPort);
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(zplCommands);
                stream.Write(data, 0, data.Length);
                stream.Close();
                client.Close();

            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Error de socket al imprimir: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar a la impresora de red: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool SendZplToSerialPrinter(string zplCommands, string portName)
        {
            SerialPort serialPort = null;
            try
            {
                // Ajusta los parámetros según la configuración de tu impresora serial
                serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                serialPort.Open();
                serialPort.Write(zplCommands);
                return true; // Retorna true si todo fue bien
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar ZPL a la impresora serial ({portName}): {ex.Message}", "Error de Impresión Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Retorna false si hubo un error
            }
            finally
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            LlenarComboBoxImpresoras();
        }

        private void LlenarComboBoxImpresoras()
        {
            cmbImpresoras.Items.Clear();
            try
            {
                foreach (string printerName in PrinterSettings.InstalledPrinters)
                {
                    cmbImpresoras.Items.Add(printerName);
                }
                if (cmbImpresoras.Items.Count > 0)
                {
                    cmbImpresoras.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener la lista de impresoras: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendZplToPrinter(string zplCommands, string printerName)
        {
            try
            {
                using (PrintDocument printDoc = new PrintDocument())
                {
                    printDoc.PrinterSettings.PrinterName = printerName;
                    printDoc.PrintController = new StandardPrintController();

                    printDoc.PrintPage += (sender, e) =>
                    {
                        Font font = new Font("Arial", 10);
                        Graphics graphics = e.Graphics;

                        RawPrinterHelper.SendStringToPrinter(printDoc.PrinterSettings.PrinterName, zplCommands);

                        e.HasMorePages = false;
                    };

                    printDoc.Print();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir la etiqueta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DiseñarInterfaz()
        {
            // Formulario
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("Segoe UI", 9);
            this.Padding = new Padding(15);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Impresión de Etiquetas GPC";

            // Crear y configurar el FlowLayoutPanel principal
            FlowLayoutPanel flpPrincipal = new FlowLayoutPanel();
            flpPrincipal.Dock = DockStyle.Fill;
            flpPrincipal.FlowDirection = FlowDirection.TopDown;
            flpPrincipal.AutoSize = true;
            flpPrincipal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flpPrincipal.Padding = new Padding(20);
            this.Controls.Add(flpPrincipal);

            // Tamaño estándar para los botones
            Size botonSize = new Size(150, 35);

            // Botón Seleccionar Archivo
            EstilizarBoton(btnSeleccionarExcel, Color.FromArgb(70, 130, 180), Color.White, botonSize);
            flpPrincipal.Controls.Add(btnSeleccionarExcel);

            // Panel para el nombre del archivo y el botón eliminar
            FlowLayoutPanel pnlArchivo = new FlowLayoutPanel();
            pnlArchivo.AutoSize = true;
            pnlArchivo.FlowDirection = FlowDirection.LeftToRight;
            pnlArchivo.Margin = new Padding(0, 5, 0, 0);
            flpPrincipal.Controls.Add(pnlArchivo);

            lblNombreArchivo = new Label();
            lblNombreArchivo.Text = "";
            EstilizarEtiqueta(lblNombreArchivo, Color.Black, FontStyle.Regular);
            lblNombreArchivo.BackColor = Color.Transparent;
            lblNombreArchivo.AutoSize = true;
            lblNombreArchivo.Visible = false;
            pnlArchivo.Controls.Add(lblNombreArchivo);

            btnEliminarArchivo = new Button();
            btnEliminarArchivo.Text = "Eliminar";
            btnEliminarArchivo.BackColor = Color.FromArgb(220, 53, 69);
            btnEliminarArchivo.ForeColor = Color.White;
            EstilizarBoton(btnEliminarArchivo, Color.FromArgb(220, 53, 69), Color.White, new Size(100, 35));
            btnEliminarArchivo.Visible = false;
            btnEliminarArchivo.Click += btnEliminarArchivo_Click;
            pnlArchivo.Controls.Add(btnEliminarArchivo);

            // Label e ComboBox para la impresora
            Label lblImpresora = new Label();
            lblImpresora.Text = "Impresora:";
            EstilizarEtiqueta(lblImpresora, Color.Black, FontStyle.Regular);
            lblImpresora.AutoSize = true;
            lblImpresora.Margin = new Padding(0, 10, 0, 0);
            flpPrincipal.Controls.Add(lblImpresora);

            cmbImpresoras.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImpresoras.BackColor = Color.White;
            cmbImpresoras.ForeColor = Color.Black;
            cmbImpresoras.FlatStyle = FlatStyle.Flat;
            cmbImpresoras.Size = new Size(200, 25);
            cmbImpresoras.Margin = new Padding(0, 5, 0, 0);
            flpPrincipal.Controls.Add(cmbImpresoras);

            // Botón Ingresar Vencimiento
            EstilizarBoton(btnEstablecerVencimiento, Color.FromArgb(70, 130, 180), Color.White, botonSize);
            btnEstablecerVencimiento.Margin = new Padding(0, 10, 0, 0);
            flpPrincipal.Controls.Add(btnEstablecerVencimiento);

            // Panel para Label y DateTimePicker de Vencimiento
            FlowLayoutPanel pnlVencimiento = new FlowLayoutPanel();
            pnlVencimiento.AutoSize = true;
            pnlVencimiento.FlowDirection = FlowDirection.LeftToRight;
            pnlVencimiento.Margin = new Padding(0, 5, 0, 0);
            flpPrincipal.Controls.Add(pnlVencimiento);

            lblFechaVencimientoSeleccionada = new Label();
            lblFechaVencimientoSeleccionada.Text = "Fecha de Vencimiento:";
            EstilizarEtiqueta(lblFechaVencimientoSeleccionada, Color.Black, FontStyle.Regular);
            lblFechaVencimientoSeleccionada.AutoSize = true;
            lblFechaVencimientoSeleccionada.Visible = false;
            pnlVencimiento.Controls.Add(lblFechaVencimientoSeleccionada);

            dtpVencimiento.BackColor = Color.White;
            dtpVencimiento.ForeColor = Color.Black;
            dtpVencimiento.Format = DateTimePickerFormat.Short;
            dtpVencimiento.Size = new Size(120, 25);
            dtpVencimiento.Margin = new Padding(5, 0, 0, 0);
            dtpVencimiento.Visible = false;
            pnlVencimiento.Controls.Add(dtpVencimiento);

            // Botón Imprimir
            EstilizarBoton(btnImprimirEtiqueta, Color.FromArgb(34, 139, 34), Color.White, botonSize);
            btnImprimirEtiqueta.Margin = new Padding(0, 15, 0, 0);
            flpPrincipal.Controls.Add(btnImprimirEtiqueta);

            // Centrar los controles en el FlowLayoutPanel principal
            flpPrincipal.FlowDirection = FlowDirection.TopDown;
            flpPrincipal.WrapContents = false;
            flpPrincipal.AutoSize = true;
            flpPrincipal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flpPrincipal.Dock = DockStyle.Fill;
            flpPrincipal.Padding = new Padding(20);

            // Ajustar el tamaño del formulario al contenido
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.ClientSize = flpPrincipal.Size;
            this.MinimumSize = this.ClientSize;
        }

        private void AjustarControles()
        {
            //Ver
        }

        private void EstilizarBoton(Button boton, Color backColor, Color foreColor, Size size)
        {
            boton.BackColor = backColor;
            boton.ForeColor = foreColor;
            boton.FlatStyle = FlatStyle.Flat;
            boton.Padding = new Padding(8, 4, 8, 4);
            boton.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(Math.Max(0, backColor.R - 20), Math.Max(0, backColor.G - 20), Math.Max(0, backColor.B - 20));
            boton.FlatAppearance.MouseDownBackColor = Color.FromArgb(Math.Max(0, backColor.R - 40), Math.Max(0, backColor.G - 40), Math.Max(0, backColor.B - 40));
            boton.UseVisualStyleBackColor = true;
            boton.Cursor = Cursors.Hand;
            boton.Margin = new Padding(5);
            boton.Size = size;
            boton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        }

        private void EstilizarEtiqueta(Label etiqueta, Color foreColor, FontStyle fontStyle)
        {
            etiqueta.ForeColor = foreColor;
            etiqueta.Font = new Font("Segoe UI", 9, fontStyle);
            etiqueta.BackColor = Color.Transparent;
            etiqueta.Padding = new Padding(0);
            etiqueta.Margin = new Padding(5);
            etiqueta.AutoSize = true;
        }
    }

    public static class RawPrinterHelper
    {
        // Declaraciones DllImport       
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);


        [DllImport("winspool.drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, IntPtr pDocInfo);

        [DllImport("winspool.drv", EntryPoint = "EndDocPrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "StartPagePrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "EndPagePrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "WritePrinter", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        //Clase DOCINFOA 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr hPrinter = IntPtr.Zero;
            bool bSuccess = false;
            IntPtr pBytes = IntPtr.Zero;
            IntPtr pDocInfo = IntPtr.Zero;
            int dwCount = 0;

            try
            {
                if (!OpenPrinter(szPrinterName, out hPrinter, IntPtr.Zero))
                {
                    int lastError = Marshal.GetLastWin32Error();
                    MessageBox.Show($"Error Win32 {lastError} al abrir la impresora.", "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                DOCINFOA di = new DOCINFOA();
                di.pDocName = "Etiqueta ZPL";
                di.pDataType = "RAW";
                di.pOutputFile = null;


                // Asignar memoria no administrada para la estructura
                pDocInfo = Marshal.AllocCoTaskMem(Marshal.SizeOf(di));
                // Copiar la estructura administrada a la memoria no administrada
                Marshal.StructureToPtr(di, pDocInfo, false);

                if (!StartDocPrinter(hPrinter, 1, pDocInfo))
                {
                    int lastError = Marshal.GetLastWin32Error();
                    MessageBox.Show($"Error Win32 {lastError} al iniciar el documento de impresión.", "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!StartPagePrinter(hPrinter))
                {
                    int lastError = Marshal.GetLastWin32Error();
                    MessageBox.Show($"Error Win32 {lastError} al iniciar la página de impresión.", "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                byte[] bytes = Encoding.ASCII.GetBytes(szString);
                dwCount = bytes.Length;
                pBytes = Marshal.AllocCoTaskMem(dwCount);
                Marshal.Copy(bytes, 0, pBytes, dwCount);

                int dwWritten = 0;
                if (!WritePrinter(hPrinter, pBytes, dwCount, out dwWritten))
                {
                    int lastError = Marshal.GetLastWin32Error();
                    MessageBox.Show($"Error Win32 {lastError} al escribir en la impresora.", "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bSuccess = false;
                }
                else
                {
                    bSuccess = true;
                }

                if (!EndPagePrinter(hPrinter))
                {
                    int lastError = Marshal.GetLastWin32Error();
                    MessageBox.Show($"Error Win32 {lastError} al finalizar la página de impresión.", "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!EndDocPrinter(hPrinter))
                {
                    int lastError = Marshal.GetLastWin32Error();
                    MessageBox.Show($"Error Win32 {lastError} al finalizar el documento de impresión.", "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            finally
            {
                // Libera la memoria de la estructura DOCINFOA también
                if (pDocInfo != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pDocInfo);
                }
                if (pBytes != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pBytes);
                }
                if (hPrinter != IntPtr.Zero)
                {
                    ClosePrinter(hPrinter);
                }
            }
            return bSuccess;
        }
    }
}