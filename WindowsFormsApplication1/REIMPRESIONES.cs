using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAPA_ENTIDAD;
using CAPA_NEGOCIO;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Configuration;

namespace WindowsFormsApplication1
{
    public partial class REIMPRESIONES : Form
    {
        public string bc_id_caja;
        public string bc_id_puntoventa;
        public string bc_id_empleado;
        public string bc_id_empresa;
        public string bc_nombre_empleado;
        public string bc_tipo_cambio;
        public string bc_sede;
        public string bc_fchapertura;
        public string bc_fchacierre;
        public string bc_saldo_ini;
        public string bc_saldo_fin;

        public string[] bcvalor = new string[20];
        public string[] bcidbien = new string[20];
        public string[] bcPRECIO_BIEN = new string[20];
        public DataTable detalle = new DataTable();



        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
        public string bc_id_cliente;
        string vFILTRO = "";
        public REIMPRESIONES()
        {

            InitializeComponent();
        }

        private void BUSCAR_CLIENTE_Load(object sender, EventArgs e)
        {
            
            vFILTRO = " ESTADO = 1";

            //crea botones en el gridview
            DataGridViewButtonColumn colBotones = new DataGridViewButtonColumn();
            colBotones.Name = "colBotones";
            colBotones.HeaderText = "Seleccionar";
            colBotones.Text = "Seleccionar";
            colBotones.UseColumnTextForButtonValue = true;
            this.dgvClientes.Columns.Add(colBotones);
            //------------------------------------------------------------///
           // CARGAR_DATOS();
            
            //DataTable detalle = (DataTable)OBJINT.vPdt_detBien;

            

        }

        #region OBJETOS
        E_MANT_CLIENTE E_OBJCLIENTE = new E_MANT_CLIENTE();
        N_VENTA N_OBJCLIENTE = new N_VENTA();

       



        #endregion
 

        void autocompletar_DESCRIPCION()
        {
            try
            {
                txtNOMCLIENTE.AutoCompleteMode = AutoCompleteMode.Suggest;
                txtNOMCLIENTE.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtRUCDNI.AutoCompleteMode = AutoCompleteMode.Suggest;
                txtRUCDNI.AutoCompleteSource = AutoCompleteSource.CustomSource;
                AutoCompleteStringCollection col = new AutoCompleteStringCollection();
                AutoCompleteStringCollection ruc = new AutoCompleteStringCollection();

                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT DESCRIPCION FROM CLIENTE", con);
                /* DataTable dt = new DataTable();
                 SqlDataAdapter da = new SqlDataAdapter(cmd);
                 da.Fill(dt);*/
                SqlDataReader dr = null;

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    col.Add(dr["DESCRIPCION"].ToString());
                }
                dr.Close();
                txtNOMCLIENTE.AutoCompleteCustomSource = col;
                con.Close();
                con.Open();
                if (txtNOMCLIENTE.Text.Length >= 6)
                {
                    SqlCommand cmv = new SqlCommand("SELECT RUC_DNI FROM CLIENTE where DESCRIPCION = '" + txtNOMCLIENTE.Text + "'", con);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmv);
                    da.Fill(dt);
                    
                    txtRUCDNI.Text = dt.Rows[0][0].ToString();
                    
                    con.Close();
                }
                else { con.Close(); }
            }

            catch
            {
            }
        }
        void autocompletar_dni_ruc()
        {
            try
            {


                txtRUCDNI.AutoCompleteMode = AutoCompleteMode.Suggest;
                txtRUCDNI.AutoCompleteSource = AutoCompleteSource.CustomSource;
                AutoCompleteStringCollection col = new AutoCompleteStringCollection();

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT RUC_DNI FROM CLIENTE", con);

                SqlDataReader dr = null;

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    col.Add(dr["RUC_DNI"].ToString());
                }
                dr.Close();
                txtRUCDNI.AutoCompleteCustomSource = col;
                con.Close();
                con.Open();
                if (txtRUCDNI.Text.Length >= 4)
                {
                    SqlCommand cmv = new SqlCommand("SELECT DESCRIPCION FROM CLIENTE where RUC_DNI = '" + txtRUCDNI.Text + "'", con);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmv);
                    da.Fill(dt);
                    txtNOMCLIENTE.Text = dt.Rows[0][0].ToString();
                    con.Close();
                }
                else { con.Close(); }
            }

            catch
            {
            }
        }
        
        public void CARGAR_DATOS()
        {

            con.Open();
            SqlCommand cmv = new SqlCommand("SELECT DESCRIPCION FROM CLIENTE where RUC_DNI = '" + txtRUCDNI.Text + "'", con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmv);
            da.Fill(dt);
            txtNOMCLIENTE.Text = dt.Rows[0][0].ToString();
            con.Close();
            
        }
        

        private void button1_Click(object sender, EventArgs e)
        {/*
            vFILTRO = " ESTADO = 1";
            CARGAR_DATOS(CONCATENAR_CONDICION());
            */
        }

        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvClientes.Columns[e.ColumnIndex].Name == "colBotones")
            {
               // T_DETALLE detalle = new T_DETALLE();
                InterfazVenta OBJINT = new InterfazVenta();
                string id_CLIENTE_dgv = dgvClientes.Rows[e.RowIndex].Cells["ID_CLIENTE"].Value.ToString();
                string id_DNIRUC_dgv = dgvClientes.Rows[e.RowIndex].Cells["RUC_DNI"].Value.ToString();
                string id_DESCRIPCION_dgv = dgvClientes.Rows[e.RowIndex].Cells["DESCRIPCION"].Value.ToString();
              
                
                OBJINT.txtCLIENTE_VENTA.Text = dgvClientes.Rows[e.RowIndex].Cells["DESCRIPCION"].Value.ToString();

                OBJINT.txtCLIENTE_RUC.Text = dgvClientes.Rows[e.RowIndex].Cells["RUC_DNI"].Value.ToString();

                OBJINT.txtCLIENTE_ID.Text = dgvClientes.Rows[e.RowIndex].Cells["ID_CLIENTE"].Value.ToString();
                
               
                OBJINT.v_id_caja = Program.id_caja;
                OBJINT.lblCajaIDVentas.Text = Program.id_caja;
                OBJINT.v_id_puntoventa = bc_id_puntoventa;
                OBJINT.v_id_empleado = bc_id_empleado;
                OBJINT.v_id_empresa = bc_id_empresa;
                OBJINT.v_nombre_empleado = bc_nombre_empleado;
                OBJINT.v_tipo_cambio = bc_tipo_cambio;
                OBJINT.v_sede = bc_sede;
                OBJINT.v_fchapertura = bc_fchapertura;
                OBJINT.v_fchacierre = bc_fchacierre;
                OBJINT.v_saldo_ini = bc_saldo_ini;
                OBJINT.v_saldo_fin = bc_saldo_fin;
                //OBJINT.Visible = true;/*REVISAR HACER QUE MANDE VALORES SIN LODEAR
                this.Close();
            }
        
        }

        private void button1_DragOver(object sender, DragEventArgs e)
        {
            button1.BackColor= Color.DeepSkyBlue;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CAJA OBJCAJA = new CAJA();

            OBJCAJA.txtIDcaja.Text = Properties.Settings.Default.id_caja;
            OBJCAJA.Visible = true;
            this.Close();

        }

        private void txtNOMCLIENTE_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

        private void txtRUCDNI_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtNOMCLIENTE_TextChanged(object sender, EventArgs e)
        {

            autocompletar_DESCRIPCION();
        }

        private void txtRUCDNI_TextChanged(object sender, EventArgs e)
        {
            autocompletar_dni_ruc();
        }
    }
}
