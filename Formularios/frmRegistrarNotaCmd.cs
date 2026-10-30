
using System;
using System.Globalization;
using System.Windows.Forms;
using Registro_de_Notas.Clases;
using Registro_de_Notas.Repositorios;

namespace Registro_de_Notas.Formularios
{
    public partial class frmRegistrarNotaCmd : Form
    {
        private readonly RegistroNotaRepositorio repositorio =
            new RegistroNotaRepositorio();
        private Label lblTitulo;
        private Label label2;
        private Label lblNombre;
        private Label lblApellido;
        private Label lblCodigo;
        private Label label6;
        private Label lblLaboratorio;
        private Label lblParcial;
        private Label lblAsistencia;
        private Button btnGuardar;
        private Button btnCancelar;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtCodigo;
        private TextBox txtLaboratorio;
        private TextBox txtParcial;
        private TextBox txtAsistencia;
        private Label label11;
        private Panel pnlNotaFinal;
        private Label lblTituloNota;
        private Label lblNotaFinal;
        private readonly RegistroNota registro;

        public frmRegistrarNotaCmd(RegistroNota estudiante)
        {
            InitializeComponent();
            ConfigurarValidaciones();

            txtLaboratorio.TextChanged += (s, e) => CalcularNotaFinal();
            txtParcial.TextChanged += (s, e) => CalcularNotaFinal();
            txtAsistencia.TextChanged += (s, e) => CalcularNotaFinal();

            registro = estudiante;

            CargarDatos();

            CalcularNotaFinal();

            btnGuardar.Click += btnGuardar_Click;
            btnCancelar.Click += btnCancelar_Click;
        }

        private void CargarDatos()
        {
            txtNombre.Text = registro.Nombre;
            txtApellido.Text = registro.Apellido;
            txtCodigo.Text = registro.Codigo;

            txtLaboratorio.Text =
                registro.Laboratorio.ToString(CultureInfo.CurrentCulture);

            txtParcial.Text =
                registro.Parcial.ToString(CultureInfo.CurrentCulture);

            txtAsistencia.Text =
                registro.Asistencia.ToString(CultureInfo.CurrentCulture);
        }

        // Validar que solamente se ingresen letras
        private void ValidarSoloLetras(KeyPressEventArgs e)
        {
            // Permitir letras, espacios, tildes y teclas de control
            if (!char.IsLetter(e.KeyChar) &&
                !char.IsWhiteSpace(e.KeyChar) &&
                !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Validar que solamente se ingresen números decimales
        private void ValidarSoloNumeros(KeyPressEventArgs e, TextBox txt)
        {
            char separador = Convert.ToChar(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
            );

            if (char.IsControl(e.KeyChar))
                return;

            if (char.IsDigit(e.KeyChar))
                return;

            if (e.KeyChar == separador &&
                !txt.Text.Contains(separador.ToString()))
                return;

            e.Handled = true;
        }

        // Validar el código del estudiante
        private void ValidarCodigo(KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) &&
                !char.IsControl(e.KeyChar) &&
                e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void ConfigurarValidaciones()
        {
            txtNombre.KeyPress += (s, e) =>
                ValidarSoloLetras(e);

            txtApellido.KeyPress += (s, e) =>
                ValidarSoloLetras(e);

            txtCodigo.KeyPress += (s, e) =>
                ValidarCodigo(e);

            txtLaboratorio.KeyPress += (s, e) =>
                ValidarSoloNumeros(e, txtLaboratorio);

            txtParcial.KeyPress += (s, e) =>
                ValidarSoloNumeros(e, txtParcial);

            txtAsistencia.KeyPress += (s, e) =>
                ValidarSoloNumeros(e, txtAsistencia);
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtLaboratorio.Text) ||
                string.IsNullOrWhiteSpace(txtParcial.Text) ||
                string.IsNullOrWhiteSpace(txtAsistencia.Text))
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            decimal laboratorio;
            decimal parcial;
            decimal asistencia;

            if (!decimal.TryParse(txtLaboratorio.Text, NumberStyles.Number,
                    CultureInfo.CurrentCulture, out laboratorio) ||
                !decimal.TryParse(txtParcial.Text, NumberStyles.Number,
                    CultureInfo.CurrentCulture, out parcial) ||
                !decimal.TryParse(txtAsistencia.Text, NumberStyles.Number,
                    CultureInfo.CurrentCulture, out asistencia))
            {
                MessageBox.Show("Ingrese notas numéricas válidas.");
                return;
            }

            if (laboratorio < 0 || laboratorio > 10 ||
                parcial < 0 || parcial > 10 ||
                asistencia < 0 || asistencia > 10)
            {
                MessageBox.Show("Las notas deben estar entre 0 y 10.");
                return;
            }

            registro.Nombre = txtNombre.Text.Trim().ToUpper();
            registro.Apellido = txtApellido.Text.Trim().ToUpper();
            registro.Codigo = txtCodigo.Text.Trim().ToUpper();

            registro.Laboratorio = laboratorio;
            registro.Parcial = parcial;
            registro.Asistencia = asistencia;

            registro.NotaFinal =
                (laboratorio * 0.50m) +
                (parcial * 0.40m) +
                (asistencia * 0.10m);

            try
            {
                repositorio.Actualizar(registro);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al actualizar: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CalcularNotaFinal()
        {
            decimal laboratorio;
            decimal parcial;
            decimal asistencia;

            if (decimal.TryParse(txtLaboratorio.Text, out laboratorio) &&
                decimal.TryParse(txtParcial.Text, out parcial) &&
                decimal.TryParse(txtAsistencia.Text, out asistencia))
            {
                decimal notaFinal =
                    (laboratorio * 0.50m) +
                    (parcial * 0.40m) +
                    (asistencia * 0.10m);

                lblNotaFinal.Text = notaFinal.ToString("0.00");
            }
            else
            {
                lblNotaFinal.Text = "0.00";
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblApellido = new System.Windows.Forms.Label();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblLaboratorio = new System.Windows.Forms.Label();
            this.lblParcial = new System.Windows.Forms.Label();
            this.lblAsistencia = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.txtLaboratorio = new System.Windows.Forms.TextBox();
            this.txtParcial = new System.Windows.Forms.TextBox();
            this.txtAsistencia = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.pnlNotaFinal = new System.Windows.Forms.Panel();
            this.lblTituloNota = new System.Windows.Forms.Label();
            this.lblNotaFinal = new System.Windows.Forms.Label();
            this.pnlNotaFinal.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(166, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(350, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "📝EDITAR ESTUDIANTE";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(195, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(261, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Sistema de Registro de Notas";
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombre.Location = new System.Drawing.Point(22, 163);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(109, 25);
            this.lblNombre.TabIndex = 2;
            this.lblNombre.Text = "👤Nombre";
            // 
            // lblApellido
            // 
            this.lblApellido.AutoSize = true;
            this.lblApellido.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblApellido.Location = new System.Drawing.Point(343, 163);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(112, 25);
            this.lblApellido.TabIndex = 3;
            this.lblApellido.Text = "👤Apellido";
            // 
            // lblCodigo
            // 
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodigo.Location = new System.Drawing.Point(22, 261);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.Size = new System.Drawing.Size(93, 25);
            this.lblCodigo.TabIndex = 4;
            this.lblCodigo.Text = "#Codigo";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(193, 379);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(260, 32);
            this.label6.TabIndex = 5;
            this.label6.Text = "CALIFICACIONES";
            // 
            // lblLaboratorio
            // 
            this.lblLaboratorio.AutoSize = true;
            this.lblLaboratorio.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLaboratorio.Location = new System.Drawing.Point(12, 478);
            this.lblLaboratorio.Name = "lblLaboratorio";
            this.lblLaboratorio.Size = new System.Drawing.Size(186, 25);
            this.lblLaboratorio.TabIndex = 6;
            this.lblLaboratorio.Text = "Laboratorio (50%)";
            // 
            // lblParcial
            // 
            this.lblParcial.AutoSize = true;
            this.lblParcial.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParcial.Location = new System.Drawing.Point(234, 478);
            this.lblParcial.Name = "lblParcial";
            this.lblParcial.Size = new System.Drawing.Size(143, 25);
            this.lblParcial.TabIndex = 7;
            this.lblParcial.Text = "Parcial (40%)";
            // 
            // lblAsistencia
            // 
            this.lblAsistencia.AutoSize = true;
            this.lblAsistencia.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAsistencia.Location = new System.Drawing.Point(439, 478);
            this.lblAsistencia.Name = "lblAsistencia";
            this.lblAsistencia.Size = new System.Drawing.Size(177, 25);
            this.lblAsistencia.TabIndex = 8;
            this.lblAsistencia.Text = "Asistencia (10%)";
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.DarkGreen;
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(447, 723);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(189, 58);
            this.btnGuardar.TabIndex = 10;
            this.btnGuardar.Text = "Guardar Cambios";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.Gray;
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(305, 723);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(136, 58);
            this.btnCancelar.TabIndex = 11;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // txtNombre
            // 
            this.txtNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombre.Location = new System.Drawing.Point(25, 203);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(193, 30);
            this.txtNombre.TabIndex = 12;
            // 
            // txtApellido
            // 
            this.txtApellido.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtApellido.Location = new System.Drawing.Point(348, 203);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(193, 30);
            this.txtApellido.TabIndex = 13;
            // 
            // txtCodigo
            // 
            this.txtCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigo.Location = new System.Drawing.Point(26, 298);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(515, 30);
            this.txtCodigo.TabIndex = 14;
            // 
            // txtLaboratorio
            // 
            this.txtLaboratorio.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLaboratorio.Location = new System.Drawing.Point(17, 520);
            this.txtLaboratorio.Name = "txtLaboratorio";
            this.txtLaboratorio.Size = new System.Drawing.Size(176, 30);
            this.txtLaboratorio.TabIndex = 15;
            // 
            // txtParcial
            // 
            this.txtParcial.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtParcial.Location = new System.Drawing.Point(239, 520);
            this.txtParcial.Name = "txtParcial";
            this.txtParcial.Size = new System.Drawing.Size(173, 30);
            this.txtParcial.TabIndex = 16;
            this.txtParcial.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // txtAsistencia
            // 
            this.txtAsistencia.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAsistencia.Location = new System.Drawing.Point(444, 520);
            this.txtAsistencia.Name = "txtAsistencia";
            this.txtAsistencia.Size = new System.Drawing.Size(175, 30);
            this.txtAsistencia.TabIndex = 17;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(21, 98);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(321, 29);
            this.label11.TabIndex = 18;
            this.label11.Text = "Información del estudiante";
            // 
            // pnlNotaFinal
            // 
            this.pnlNotaFinal.Controls.Add(this.lblNotaFinal);
            this.pnlNotaFinal.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlNotaFinal.Location = new System.Drawing.Point(22, 622);
            this.pnlNotaFinal.Name = "pnlNotaFinal";
            this.pnlNotaFinal.Size = new System.Drawing.Size(176, 72);
            this.pnlNotaFinal.TabIndex = 19;
            // 
            // lblTituloNota
            // 
            this.lblTituloNota.AutoSize = true;
            this.lblTituloNota.BackColor = System.Drawing.Color.DarkOliveGreen;
            this.lblTituloNota.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloNota.ForeColor = System.Drawing.Color.White;
            this.lblTituloNota.Location = new System.Drawing.Point(22, 590);
            this.lblTituloNota.Name = "lblTituloNota";
            this.lblTituloNota.Size = new System.Drawing.Size(164, 29);
            this.lblTituloNota.TabIndex = 20;
            this.lblTituloNota.Text = "NOTA FINAL";
            // 
            // lblNotaFinal
            // 
            this.lblNotaFinal.AutoSize = true;
            this.lblNotaFinal.BackColor = System.Drawing.Color.DarkGreen;
            this.lblNotaFinal.Font = new System.Drawing.Font("Cambria", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotaFinal.ForeColor = System.Drawing.Color.White;
            this.lblNotaFinal.Location = new System.Drawing.Point(34, 0);
            this.lblNotaFinal.Name = "lblNotaFinal";
            this.lblNotaFinal.Size = new System.Drawing.Size(75, 33);
            this.lblNotaFinal.TabIndex = 0;
            this.lblNotaFinal.Text = "Nota";
            // 
            // frmRegistrarNotaCmd
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(648, 792);
            this.Controls.Add(this.lblTituloNota);
            this.Controls.Add(this.pnlNotaFinal);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtAsistencia);
            this.Controls.Add(this.txtParcial);
            this.Controls.Add(this.txtLaboratorio);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.txtApellido);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.lblAsistencia);
            this.Controls.Add(this.lblParcial);
            this.Controls.Add(this.lblLaboratorio);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblCodigo);
            this.Controls.Add(this.lblApellido);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblTitulo);
            this.Name = "frmRegistrarNotaCmd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editar Estudiante";
            this.pnlNotaFinal.ResumeLayout(false);
            this.pnlNotaFinal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
