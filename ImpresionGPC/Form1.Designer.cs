namespace ImpresionGPC
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSeleccionarExcel = new System.Windows.Forms.Button();
            this.btnEstablecerVencimiento = new System.Windows.Forms.Button();
            this.dtpVencimiento = new System.Windows.Forms.DateTimePicker();
            this.btnImprimirEtiqueta = new System.Windows.Forms.Button();
            this.lblFechaVencimientoSeleccionada = new System.Windows.Forms.Label();
            this.lblEstadoTitulo = new System.Windows.Forms.Label();
            this.lblEstadoMensaje = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cmbImpresoras = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSeleccionarExcel
            // 
            this.btnSeleccionarExcel.Location = new System.Drawing.Point(10, 4);
            this.btnSeleccionarExcel.Name = "btnSeleccionarExcel";
            this.btnSeleccionarExcel.Size = new System.Drawing.Size(112, 49);
            this.btnSeleccionarExcel.TabIndex = 0;
            this.btnSeleccionarExcel.Text = "Seleccionar Archivo";
            this.btnSeleccionarExcel.UseVisualStyleBackColor = true;
            this.btnSeleccionarExcel.Click += new System.EventHandler(this.btnSeleccionarExcel_Click);
            // 
            // btnEstablecerVencimiento
            // 
            this.btnEstablecerVencimiento.Location = new System.Drawing.Point(8, 3);
            this.btnEstablecerVencimiento.Name = "btnEstablecerVencimiento";
            this.btnEstablecerVencimiento.Size = new System.Drawing.Size(122, 50);
            this.btnEstablecerVencimiento.TabIndex = 3;
            this.btnEstablecerVencimiento.Text = "Ingresa el Vencimiento";
            this.btnEstablecerVencimiento.UseVisualStyleBackColor = true;
            this.btnEstablecerVencimiento.Click += new System.EventHandler(this.btnEstablecerVencimiento_Click);
            // 
            // dtpVencimiento
            // 
            this.dtpVencimiento.Location = new System.Drawing.Point(12, 15);
            this.dtpVencimiento.Name = "dtpVencimiento";
            this.dtpVencimiento.Size = new System.Drawing.Size(122, 20);
            this.dtpVencimiento.TabIndex = 4;
            this.dtpVencimiento.Visible = false;
            this.dtpVencimiento.ValueChanged += new System.EventHandler(this.dtpVencimiento_ValueChanged);
            // 
            // btnImprimirEtiqueta
            // 
            this.btnImprimirEtiqueta.Location = new System.Drawing.Point(332, 371);
            this.btnImprimirEtiqueta.Name = "btnImprimirEtiqueta";
            this.btnImprimirEtiqueta.Size = new System.Drawing.Size(91, 50);
            this.btnImprimirEtiqueta.TabIndex = 5;
            this.btnImprimirEtiqueta.Text = "Imprimir";
            this.btnImprimirEtiqueta.UseVisualStyleBackColor = true;
            this.btnImprimirEtiqueta.Click += new System.EventHandler(this.btnImprimirEtiqueta_Click);
            // 
            // lblFechaVencimientoSeleccionada
            // 
            this.lblFechaVencimientoSeleccionada.AutoSize = true;
            this.lblFechaVencimientoSeleccionada.Location = new System.Drawing.Point(614, 354);
            this.lblFechaVencimientoSeleccionada.Name = "lblFechaVencimientoSeleccionada";
            this.lblFechaVencimientoSeleccionada.Size = new System.Drawing.Size(0, 13);
            this.lblFechaVencimientoSeleccionada.TabIndex = 6;
            // 
            // lblEstadoTitulo
            // 
            this.lblEstadoTitulo.AutoSize = true;
            this.lblEstadoTitulo.Location = new System.Drawing.Point(29, 252);
            this.lblEstadoTitulo.Name = "lblEstadoTitulo";
            this.lblEstadoTitulo.Size = new System.Drawing.Size(0, 13);
            this.lblEstadoTitulo.TabIndex = 1;
            // 
            // lblEstadoMensaje
            // 
            this.lblEstadoMensaje.AutoSize = true;
            this.lblEstadoMensaje.Location = new System.Drawing.Point(34, 86);
            this.lblEstadoMensaje.Name = "lblEstadoMensaje";
            this.lblEstadoMensaje.Size = new System.Drawing.Size(0, 13);
            this.lblEstadoMensaje.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSeleccionarExcel);
            this.panel1.Location = new System.Drawing.Point(632, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(125, 53);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnEstablecerVencimiento);
            this.panel2.Location = new System.Drawing.Point(624, 146);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(133, 60);
            this.panel2.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dtpVencimiento);
            this.panel3.Location = new System.Drawing.Point(617, 314);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(137, 53);
            this.panel3.TabIndex = 9;
            // 
            // cmbImpresoras
            // 
            this.cmbImpresoras.FormattingEnabled = true;
            this.cmbImpresoras.Location = new System.Drawing.Point(315, 86);
            this.cmbImpresoras.Name = "cmbImpresoras";
            this.cmbImpresoras.Size = new System.Drawing.Size(121, 21);
            this.cmbImpresoras.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cmbImpresoras);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblFechaVencimientoSeleccionada);
            this.Controls.Add(this.btnImprimirEtiqueta);
            this.Controls.Add(this.lblEstadoMensaje);
            this.Controls.Add(this.lblEstadoTitulo);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "Form1";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Impresion de Estiquetas";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSeleccionarExcel;
        private System.Windows.Forms.Button btnEstablecerVencimiento;
        private System.Windows.Forms.DateTimePicker dtpVencimiento;
        private System.Windows.Forms.Button btnImprimirEtiqueta;
        private System.Windows.Forms.Label lblFechaVencimientoSeleccionada;
        private System.Windows.Forms.Label lblEstadoTitulo;
        private System.Windows.Forms.Label lblEstadoMensaje;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox cmbImpresoras;
    }
}

