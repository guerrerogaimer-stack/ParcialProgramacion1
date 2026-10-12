using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient; // Si usas .NET Framework: using System.Data.SqlClient;

namespace ParcialProgramacion1
{
    public partial class Form1 : Form
    {
        // Ajusta el Server si usas SQLEXPRESS: ".\\SQLEXPRESS"
        private const string CadenaConexion =
            "Server=.;Database=ParcialProgramacion1;Integrated Security=True;TrustServerCertificate=True;";

        private int idSeleccionado = 0;

        public Form1()
        {
            InitializeComponent();
            Load += (s, e) => CargarProductos();
        }

        // ---------------- ACTIVIDAD 4: VALIDACIONES ----------------
        private bool ValidarCampos(out decimal precio, out int existencias)
        {
            precio = 0;
            existencias = 0;

            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                MessageBox.Show("El nombre y la categoría son obligatorios.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtNombre.Text.Trim().Length > 100 || txtCategoria.Text.Trim().Length > 50)
            {
                MessageBox.Show("El nombre admite máximo 100 caracteres y la categoría 50.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text.Trim(), out precio)
                || precio <= 0)
            {
                MessageBox.Show("El precio debe ser numérico y mayor que cero.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtExistencias.Text.Trim(), out existencias) || existencias < 0)
            {
                MessageBox.Show("Las existencias deben ser un número entero igual o mayor que cero.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ---------------- ACTIVIDAD 3: OPERACIONES ----------------

        // CONSULTAR
        private void CargarProductos()
        {
            try
            {
                using (var cn = new SqlConnection(CadenaConexion))
                using (var da = new SqlDataAdapter(
                    "SELECT IdProducto, Nombre, Categoria, Precio, Existencias FROM dbo.Productos ORDER BY IdProducto", cn))
                {
                    var tabla = new DataTable();
                    da.Fill(tabla);
                    dgvProductos.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar los productos: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }

        // GUARDAR
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos(out decimal precio, out int existencias)) return;

            try
            {
                using (var cn = new SqlConnection(CadenaConexion))
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.Productos (Nombre, Categoria, Precio, Existencias) " +
                    "VALUES (@Nombre, @Categoria, @Precio, @Existencias)", cn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Categoria", txtCategoria.Text.Trim());
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Existencias", existencias);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Producto guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el producto: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // SELECCIONAR de la tabla
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];
            idSeleccionado = Convert.ToInt32(fila.Cells["IdProducto"].Value);
            txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
            txtCategoria.Text = fila.Cells["Categoria"].Value.ToString();
            txtPrecio.Text = fila.Cells["Precio"].Value.ToString();
            txtExistencias.Text = fila.Cells["Existencias"].Value.ToString();
        }

        // ACTUALIZAR
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la tabla para actualizar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCampos(out decimal precio, out int existencias)) return;

            try
            {
                using (var cn = new SqlConnection(CadenaConexion))
                using (var cmd = new SqlCommand(
                    "UPDATE dbo.Productos SET Nombre = @Nombre, Categoria = @Categoria, " +
                    "Precio = @Precio, Existencias = @Existencias WHERE IdProducto = @Id", cn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Categoria", txtCategoria.Text.Trim());
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Existencias", existencias);
                    cmd.Parameters.AddWithValue("@Id", idSeleccionado);

                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0)
                        MessageBox.Show("Producto actualizado correctamente.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No se encontró el producto.", "Aviso",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                LimpiarCampos();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el producto: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ELIMINAR
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la tabla para eliminar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar el producto seleccionado?", "Confirmar eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            try
            {
                using (var cn = new SqlConnection(CadenaConexion))
                using (var cmd = new SqlCommand("DELETE FROM dbo.Productos WHERE IdProducto = @Id", cn))
                {
                    cmd.Parameters.AddWithValue("@Id", idSeleccionado);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Producto eliminado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el producto: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // LIMPIAR
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            idSeleccionado = 0;
            txtNombre.Clear();
            txtCategoria.Clear();
            txtPrecio.Clear();
            txtExistencias.Clear();
            dgvProductos.ClearSelection();
            txtNombre.Focus();
        }
    }
}
