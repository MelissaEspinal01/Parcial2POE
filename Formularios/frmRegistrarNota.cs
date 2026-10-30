
using Registro_de_Notas.Clases;
using Registro_de_Notas.Formularios;
using Registro_de_Notas.Repositorios;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Registro_de_Notas
{
    public partial class frmRegistrarNota : Form
    {
        private readonly RegistroNotaRepositorio repositorio =
            new RegistroNotaRepositorio();

        public frmRegistrarNota()
        {
            InitializeComponent();

            ConfigurarValidaciones();
            ConfigurarTabla();

            // Conectar los eventos
            this.Load += frmRegistrarNota_Load;
            btnAgregar.Click += btnAgregar_Click;
            dgvRegistro.CellDoubleClick += dgvRegistro_CellDoubleClick;
        }

        // Configurar validaciones de los campos
        private void ConfigurarValidaciones()
        {
            // Nombre y apellido
            txtNombre.KeyPress += (s, e) => ValidarSoloLetras(e);
            txtApellido.KeyPress += (s, e) => ValidarSoloLetras(e);

            // Código
            txtCodigo.KeyPress += (s, e) => ValidarCodigo(e);

            // Calificaciones
            txtLaboratorio.KeyPress += (s, e) =>
                ValidarSoloNumeros(e, txtLaboratorio);

            txtParcial.KeyPress += (s, e) =>
                ValidarSoloNumeros(e, txtParcial);

            txtAsistencia.KeyPress += (s, e) =>
                ValidarSoloNumeros(e, txtAsistencia);
        }

        // Permitir únicamente letras, espacios y teclas de control
        private void ValidarSoloLetras(KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) &&
                !char.IsWhiteSpace(e.KeyChar) &&
                !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
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

        // Validar números decimales
        private void ValidarSoloNumeros(KeyPressEventArgs e, TextBox txt)
        {
            char separador = Convert.ToChar(
                System.Globalization.CultureInfo.CurrentCulture
                .NumberFormat.NumberDecimalSeparator
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
        private void frmRegistrarNota_Load(object sender, EventArgs e)
        {
            CargarRegistros();
        }

        private void ConfigurarTabla()
        {
            dgvRegistro.AutoGenerateColumns = true;
            dgvRegistro.Columns.Clear();

            dgvRegistro.AllowUserToAddRows = false;
            dgvRegistro.ReadOnly = true;
            dgvRegistro.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvRegistro.MultiSelect = false;

            dgvRegistro.DefaultCellStyle.ForeColor = Color.Black;
            dgvRegistro.DefaultCellStyle.BackColor = Color.White;
            dgvRegistro.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvRegistro.DefaultCellStyle.SelectionBackColor = Color.DarkBlue;

            dgvRegistro.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvRegistro.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvRegistro.EnableHeadersVisualStyles = false;
        }

        private void CargarRegistros()
        {
            try
            {
                dgvRegistro.DataSource = repositorio.ObtenerTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los registros: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private bool ValidarNota(
            string texto,
            string campo,
            out decimal nota)
        {
            nota = 0;

            if (!decimal.TryParse(texto, out nota))
            {
                MessageBox.Show(
                    "Ingresa una nota válida para " + campo + ".",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            if (nota < 0 || nota > 10)
            {
                MessageBox.Show(
                    "La nota de " + campo +
                    " debe estar entre 0 y 10.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            return true;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show(
                    "Completa el nombre, apellido y código del estudiante.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            decimal laboratorio;
            decimal parcial;
            decimal asistencia;

            if (!ValidarNota(
                txtLaboratorio.Text, "Laboratorio", out laboratorio))
                return;

            if (!ValidarNota(
                txtParcial.Text, "Parcial", out parcial))
                return;

            if (!ValidarNota(
                txtAsistencia.Text, "Asistencia", out asistencia))
                return;

            decimal notaFinal =
                laboratorio * 0.50m +
                parcial * 0.40m +
                asistencia * 0.10m;

            RegistroNota registro = new RegistroNota
            {
                Nombre = txtNombre.Text.Trim().ToUpper(),
                Apellido = txtApellido.Text.Trim().ToUpper(),
                Codigo = txtCodigo.Text.Trim().ToUpper(),
                Laboratorio = laboratorio,
                Parcial = parcial,
                Asistencia = asistencia,
                NotaFinal = Math.Round(notaFinal, 2)
            };

            try
            {
                repositorio.Insertar(registro);

                MessageBox.Show(
                    "Estudiante registrado correctamente.\n" +
                    "Nota final: " + registro.NotaFinal.ToString("0.00"),
                    "Registro exitoso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LimpiarCampos();
                CargarRegistros();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar el estudiante.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar que exista un registro seleccionado
            if (dgvRegistro.CurrentRow == null)
            {
                MessageBox.Show(
                    "Selecciona un estudiante para eliminar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Obtener el ID del estudiante seleccionado
            int id = Convert.ToInt32(
                dgvRegistro.CurrentRow.Cells["Id"].Value
            );

            // Confirmar antes de eliminar
            DialogResult respuesta = MessageBox.Show(
                "¿Estás seguro de que deseas eliminar este estudiante?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    int resultado = repositorio.Eliminar(id);

                    if (resultado > 0)
                    {
                        MessageBox.Show(
                            "Estudiante eliminado correctamente.",
                            "Eliminación exitosa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        CargarRegistros();
                    }
                    else
                    {
                        MessageBox.Show(
                            "No se encontró el registro para eliminar.",
                            "Aviso",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al eliminar el estudiante: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void dgvRegistro_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(
                    dgvRegistro.Rows[e.RowIndex].Cells["Id"].Value
                );

                RegistroNota estudiante = repositorio.ObtenerPorId(id);

                if (estudiante == null)
                {
                    MessageBox.Show("No se encontró el estudiante.");
                    return;
                }

                using (frmRegistrarNotaCmd formulario =
                    new frmRegistrarNotaCmd(estudiante))
                {
                    if (formulario.ShowDialog() == DialogResult.OK)
                    {
                        CargarRegistros();

                        MessageBox.Show(
                            "Los datos del estudiante se actualizaron correctamente.",
                            "Actualización exitosa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al abrir el registro: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtCodigo.Clear();
            txtLaboratorio.Clear();
            txtParcial.Clear();
            txtAsistencia.Clear();

            txtNombre.Focus();
        }

       
    }
}
