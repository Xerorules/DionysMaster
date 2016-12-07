using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Runtime.InteropServices;

using CAPA_ENTIDAD;
using CAPA_NEGOCIO;

namespace WindowsFormsApplication1
{
    public partial class MOVIMIENTOS : Form
    {
        public string m_id_caja;
        public string m_id_empleado;
        public string m_id_puntoventa;
        public string m_id_empresa;
        public string m_nombre_empleado;
        public string m_tipo_cambio;
        public string m_sede;
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
        


        public MOVIMIENTOS()
        {
            InitializeComponent();
        }

        private void MOVIMIENTOS_Load(object sender, EventArgs e)
        {
                

            //CON ESTO PUEDO VERIFICO SI TENGO UNA CAJA ABIERTA  Y SINO ES ASI, ABRIR UNA CAJA NUEVA
            if (Properties.Settings.Default.id_caja == string.Empty)
                {
                    CAJA objcaja = new CAJA();
                    objcaja.ShowDialog();
                    this.Hide();
                }

                
                lblEmpresa.Text = Properties.Settings.Default.nomempresa;
                lblUsuario.Text = Properties.Settings.Default.nomempleado;
                lblSede.Text = Properties.Settings.Default.nomsede;
                lblFecha.Text = DateTime.Today.ToShortDateString();
                rdbSOLES.Checked = true;
                rdbTICKET.Checked = true;
               // rdbTODOS.Checked = true;
                FILTRAR_CAJA_KARDEX(0, "3", "");
                ESTADO_TRANSACCION(1);
                LLENAR_COMBO_TIPOMOV();
                LLENAR_COMBO_TIPOPAGO();
                SELECCIONAR_REGISTRO_CARGADATA(); //AQUI CARGO POR PRIMERA VEZ TODOS LOS CAMPOS SELECIONADOS DE LA GRILLA
                ESTADO_TEXBOX_VENTA(2); //PARA PONER EN ESTADO DE BLOQUEADO A LOS TEXBOX DE LA VENTA
            
        }

        #region OBJETOS
        N_VENTA N_OBJVENTAS = new N_VENTA();
        N_LOGUEO N_OBJEMPRESA = new N_LOGUEO();
        E_CAJA_KARDEX E_OBJCAJA_KARDEX = new E_CAJA_KARDEX();

        #endregion


        #region FUNCIONES


        private bool VALIDAR_DATOS_CAJA_KARDEX()
        {

            bool RESULTADO = false;
            //ESTE SEGMENTO HAY QUE MODIFICAR POSTERIORMENTE EL COMBO DE ID_TIPO_MOV  
            try
            {
                if (cboTIPO_MOV.SelectedIndex != -1)
                {
                    if (cboTIPO_PAGO.SelectedItem.ToString() != string.Empty)
                    {
                        if (txtMONTO.Text != string.Empty)
                        {
                            if (rdbSOLES.Checked == true)
                            {
                                if (txtDESCRIPCION.Text != string.Empty)
                                {
                                    if (cboTIPO_MOV.SelectedValue.ToString() == "IPV" || cboTIPO_MOV.SelectedValue.ToString() == "EPC")
                                    {
                                        if (txtID_DOC.Text != string.Empty)
                                        {
                                            if (txtSALDO.Text != string.Empty)
                                            {
                                                RESULTADO = true;
                                            }
                                            else
                                            {
                                                RESULTADO = false;
                                            }

                                        }
                                        else
                                        {
                                            RESULTADO = false;
                                        }

                                    }
                                    else
                                    {
                                        RESULTADO = true;
                                    }
                                }
                                else
                                {
                                    RESULTADO = false;
                                }

                            }
                            else
                            {
                                RESULTADO = false;
                            }
                        }
                        else
                        {
                            RESULTADO = false;
                        }
                    }
                    else
                    {
                        RESULTADO = false;
                    }
                }
                else
                {
                    RESULTADO = false;
                }
            }
            catch (Exception)
            {

                MessageBox.Show("LOS DATOS ESTAN INCOMPLETOS");
            }


            return RESULTADO;
        }


        #endregion

        #region PROCEDIMIENTOS
        void FILTRAR_CAJA_KARDEX(int OPCION, string VER, string DESCRIPCION)
        {
            double TOTAL_CAJA, TOTAL_SOLES, TOTAL_DOLARES;
            TOTAL_CAJA = 0.00;
            TOTAL_SOLES = 0.00;
            TOTAL_DOLARES = 0.00;

            DataTable dt = new DataTable();
            string ID_MOVIMIENTO = string.Empty;
            string ID_CAJA = Properties.Settings.Default.id_caja;
            string TIPO_PAGO = string.Empty;
            string ID_TIPOMOV = string.Empty;
            string OPCION_USUARIO = string.Empty;
            
            dt = N_OBJVENTAS.FILTRAR_CAJA_KARDEX(ID_MOVIMIENTO, ID_CAJA, DESCRIPCION, TIPO_PAGO, ID_TIPOMOV, OPCION, VER);
            
            dgvMOV_CAJAKARDEX.DataSource = dt;
            dgvMOV_CAJAKARDEX.Columns[10].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[0].HeaderText = "MOVIMIENTO";
            dgvMOV_CAJAKARDEX.Columns[1].HeaderText = "DESCRIPCION";
            dgvMOV_CAJAKARDEX.Columns[2].HeaderText = "T. PAGO";
            dgvMOV_CAJAKARDEX.Columns[3].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[4].HeaderText = "T. MOV";
            dgvMOV_CAJAKARDEX.Columns[5].HeaderText = "IMPORTE";
            dgvMOV_CAJAKARDEX.Columns[6].HeaderText = "MON";
            dgvMOV_CAJAKARDEX.Columns[7].HeaderText = "TC";
            dgvMOV_CAJAKARDEX.Columns[8].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[9].HeaderText = "IMPORTE SOLES";
            dgvMOV_CAJAKARDEX.Columns[10].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[11].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[12].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[13].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[14].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[15].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[16].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[17].Visible = false;
            dgvMOV_CAJAKARDEX.Columns[18].HeaderText = "ANULADO";
            dgvMOV_CAJAKARDEX.Columns[19].HeaderText = "FECHA";
            dgvMOV_CAJAKARDEX.Columns[20].HeaderText = "COMPRA/VENTA";




            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TOTAL_CAJA = TOTAL_CAJA + Convert.ToDouble(dt.Rows[i]["IMPORTE_CAJA"]);
                if (dt.Rows[i]["MONEDA"].ToString() == "S")
                {
                    if (dt.Rows[i]["ID_TIPOMOV"].ToString().Substring(0, 1) == "I")
                    {
                        TOTAL_SOLES = TOTAL_SOLES + Convert.ToDouble(dt.Rows[i]["IMPORTE"]);
                    }
                    else
                    {
                        TOTAL_SOLES = TOTAL_SOLES - Convert.ToDouble(dt.Rows[i]["IMPORTE"]);
                    }


                }
                else
                {
                    if (dt.Rows[i]["ID_TIPOMOV"].ToString().Substring(0, 1) == "I")
                    {
                        TOTAL_DOLARES = TOTAL_DOLARES + Convert.ToDouble(dt.Rows[i]["IMPORTE"]);
                    }
                    else
                    {

                        TOTAL_DOLARES = TOTAL_DOLARES - Convert.ToDouble(dt.Rows[i]["IMPORTE"]);
                    }
                }
            }

            txtTOTALSOLES.Text = TOTAL_SOLES.ToString("N2");
            txtTOTALDOLARES.Text = TOTAL_DOLARES.ToString("N2");
            txtTOTALCAJA.Text = TOTAL_CAJA.ToString("N2");

        }


        void LLENAR_COMBO_TIPOMOV()
        {

            cboTIPO_MOV.ValueMember = "ID_TIPOMOV";
            cboTIPO_MOV.DisplayMember = "DESCRIPCION";
            cboTIPO_MOV.DataSource = N_OBJVENTAS.LISTAR_TIPO_MOVIMIENTO();
            


        }

        void LLENAR_COMBO_TIPOPAGO() { 

            cboTIPO_PAGO.ValueMember = "ID_TIPOPAGO";
            cboTIPO_PAGO.DisplayMember = "DESCRIPCION";
            cboTIPO_PAGO.DataSource = N_OBJVENTAS.LISTAR_TIPO_PAGO();
        }



        void ESTADO_TRANSACCION(int ESTADO)
        {
            if (ESTADO == 1) //ESTADO CONSULTA
            {
                txtID_MOVIMIENTO.ReadOnly = true;
                txtFECHA.ReadOnly = true;
                txtFECHA_ANULADO.ReadOnly = true;
                cboTIPO_MOV.Enabled = false;
                cboTIPO_PAGO.Enabled = false;
                txtMONTO.ReadOnly = true;
                rdbSOLES.Enabled = false;
                rdbDOLARES.Enabled = false;
                txtDESCRIPCION.ReadOnly = true;
                btnNUEVO.Enabled = true;
                btnGRABAR.Enabled = false;
                btnCANCELAR.Enabled = false;
                btnANULAR.Enabled = true;
                btnIMPRIMIR.Enabled = true;
                cboTIPO_BUSQUEDA.Enabled = true;
                txtDATA_BUSQUEDA.ReadOnly = true;
                rdbACTIVOS.Enabled = true;
                rdbANULADOS.Enabled = true;
                rdbTODOS.Enabled = true;
                dgvMOV_CAJAKARDEX.Enabled = true;
                btnBUSCAR.Enabled = true;
                txtDATA_BUSQUEDA.ReadOnly = false;

            }
            if (ESTADO == 2) //ESTADO NUEVO
            {
                //LIMPIARDO CONTROLES
                txtID_MOVIMIENTO.Text = string.Empty;
                txtFECHA.Text = DateTime.Now.ToShortDateString();
                txtFECHA_ANULADO.Text = string.Empty;
                cboTIPO_MOV.SelectedIndex = 0;
                cboTIPO_PAGO.SelectedIndex = 0;
                txtMONTO.Text = string.Empty;
                txtDESCRIPCION.Text = string.Empty;
                rdbSOLES.Checked = false;
                rdbDOLARES.Checked = false;

                //===================

                txtID_MOVIMIENTO.ReadOnly = true;
                txtFECHA.ReadOnly = true;
                txtFECHA_ANULADO.ReadOnly = true;
                cboTIPO_MOV.Enabled = true;
                cboTIPO_PAGO.Enabled = true;
                txtMONTO.ReadOnly = false;
                rdbSOLES.Checked = true;
                rdbSOLES.Enabled = false;
                rdbDOLARES.Enabled = false;
                txtDESCRIPCION.ReadOnly = false;
                btnNUEVO.Enabled = false;
                btnGRABAR.Enabled = true;
                btnCANCELAR.Enabled = true;
                btnANULAR.Enabled = false;
                btnIMPRIMIR.Enabled = false;
                cboTIPO_BUSQUEDA.Enabled = false;
                txtDATA_BUSQUEDA.ReadOnly = false;
                rdbACTIVOS.Enabled = false;
                rdbANULADOS.Enabled = false;
                rdbTODOS.Enabled = false;
                dgvMOV_CAJAKARDEX.Enabled = false;
                btnBUSCAR.Enabled = false;
                txtDATA_BUSQUEDA.ReadOnly = true;
            }
        }

        public void ANULAR_CAJA_KARDEX_REGISTRO()
        {
            E_OBJCAJA_KARDEX.ID_MOVIMIENTO  = dgvMOV_CAJAKARDEX.CurrentRow.Cells[0].Value.ToString();
            E_OBJCAJA_KARDEX.DESCRIPCION = string.Empty;
            E_OBJCAJA_KARDEX.ID_COMPVENT = string.Empty;
            E_OBJCAJA_KARDEX.ID_TIPOMOV = string.Empty;
            E_OBJCAJA_KARDEX.ID_TIPOPAGO = string.Empty;
            E_OBJCAJA_KARDEX.IMPORTE = 0.00;
            E_OBJCAJA_KARDEX.MONEDA = string.Empty;
            E_OBJCAJA_KARDEX.TIPO_CAMBIO = 0.00;
            E_OBJCAJA_KARDEX.AMORTIZADO = 0.00;
            E_OBJCAJA_KARDEX.ID_CAJA = string.Empty;
            E_OBJCAJA_KARDEX.IMPORTE_CAJA = 0.00;
            E_OBJCAJA_KARDEX.OPCION = 2; //ESTA OPCION 2 ANULA AMORTIZACION

            N_OBJVENTAS.CAJA_KARDEX_MANTENIMIENTO(E_OBJCAJA_KARDEX);
        }

        private void GRABAR_CAJA_KARDEX()
        {
            try
            {
                E_OBJCAJA_KARDEX.ID_MOVIMIENTO = string.Empty;
                E_OBJCAJA_KARDEX.DESCRIPCION = txtDESCRIPCION.Text.ToString();
                if (txtID_DOC.Text != string.Empty)
                {
                    E_OBJCAJA_KARDEX.ID_COMPVENT = txtID_DOC.Text;
                }
                else
                {
                    E_OBJCAJA_KARDEX.ID_COMPVENT = string.Empty;
                }

                E_OBJCAJA_KARDEX.ID_TIPOMOV = cboTIPO_MOV.SelectedValue.ToString();
                E_OBJCAJA_KARDEX.ID_TIPOPAGO = cboTIPO_PAGO.SelectedValue.ToString();

                E_OBJCAJA_KARDEX.IMPORTE = Convert.ToDouble(txtMONTO.Text.ToString());

                if (rdbSOLES.Checked == true)
                {
                    E_OBJCAJA_KARDEX.MONEDA = "S";
                }
                else
                {
                    if (rdbDOLARES.Checked == true)
                    {
                        E_OBJCAJA_KARDEX.MONEDA = "D";
                    }
                }
                E_OBJCAJA_KARDEX.TIPO_CAMBIO = Convert.ToDouble(Properties.Settings.Default.tipo_cambio);

                if (txtMONEDA.Text == "S" && rdbSOLES.Checked == true)
                {
                    E_OBJCAJA_KARDEX.AMORTIZADO = Convert.ToDouble(txtMONTO.Text.ToString());
                }
                if (txtMONEDA.Text == "S" && rdbDOLARES.Checked == true)
                {
                    E_OBJCAJA_KARDEX.AMORTIZADO = Convert.ToDouble(txtMONTO.Text.ToString()) * Convert.ToDouble(Properties.Settings.Default.tipo_cambio);
                }
                if (txtMONEDA.Text == "D" && rdbSOLES.Checked == true)
                {
                    E_OBJCAJA_KARDEX.AMORTIZADO = Convert.ToDouble(txtMONTO.Text.ToString()) / Convert.ToDouble(Properties.Settings.Default.tipo_cambio);
                }
                if (txtMONEDA.Text == "D" && rdbDOLARES.Checked == true)
                {
                    E_OBJCAJA_KARDEX.AMORTIZADO = Convert.ToDouble(txtMONTO.Text.ToString());
                }


                E_OBJCAJA_KARDEX.ID_CAJA = Properties.Settings.Default.id_caja;

                string var = cboTIPO_MOV.SelectedValue.ToString().Substring(0,1);
                if (var == "I") //es un ingreso
                {

                    if (rdbSOLES.Checked == true) // esta en soles
                    {
                        E_OBJCAJA_KARDEX.IMPORTE_CAJA = Convert.ToDouble(txtMONTO.Text.ToString());
                    }
                    else //sino es dolares y mi importe caja siempre es soles
                    {
                        E_OBJCAJA_KARDEX.IMPORTE_CAJA = Math.Round(Convert.ToDouble(txtMONTO.Text.ToString()) * Convert.ToDouble(Properties.Settings.Default.tipo_cambio), 2);
                    }
                }
                else //entonces es un egreso y registro mi importe caja en negativo
                {
                    if (rdbSOLES.Checked == true) // esta en soles
                    {
                        E_OBJCAJA_KARDEX.IMPORTE_CAJA = (-1) * Convert.ToDouble(txtMONTO.Text.ToString());
                    }
                    else //sino es dolares y mi importe caja siempre es soles
                    {
                        E_OBJCAJA_KARDEX.IMPORTE_CAJA = (-1) * Math.Round(Convert.ToDouble(txtMONTO.Text.ToString()) * Convert.ToDouble(Properties.Settings.Default.tipo_cambio), 2);
                    }
                }

                E_OBJCAJA_KARDEX.OPCION = 1; //ESTA OPCION 1 INSERTA EL NUEVO REGISTRO

                N_OBJVENTAS.CAJA_KARDEX_MANTENIMIENTO(E_OBJCAJA_KARDEX);
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion


        public void CONSULTAR_VENTAS(string OPCION)
        {

            if (cboTIPO_MOV.SelectedValue.ToString() == "IPV")
            {

                DataTable dt = new DataTable();
                dt = N_OBJVENTAS.CAPTURAR_TABLA_VENTA(OPCION,Properties.Settings.Default.id_sede.ToString());
                if (dt.Rows.Count > 0)
                {
                    txtPERSONA.Text = dt.Rows[0]["C_DESCRIPCION"].ToString();
                    txtNUM_DOCUMENTO.Text = dt.Rows[0]["V_TIPO_DOC"].ToString() + dt.Rows[0]["V_SERIE"].ToString() + dt.Rows[0]["V_NUMERO"].ToString();
                    txtMONEDA.Text = dt.Rows[0]["V_MONEDA"].ToString();
                    txtSALDO.Text = dt.Rows[0]["V_SALDO"].ToString();
                }
                else
                {
                    MessageBox.Show("ERROR, VERIFICAR SI EL NUMERO DE VENTA ES CORRECTO");
                }
            }
            if (cboTIPO_MOV.SelectedValue.ToString() == "EPC")
            {
                DataTable dt = new DataTable();
                dt = N_OBJVENTAS.CAPTURAR_TABLA_COMPRA(OPCION);
                if (dt.Rows.Count > 0)
                {
                    txtPERSONA.Text = dt.Rows[0]["P_DESCRIPCION"].ToString();
                    txtNUM_DOCUMENTO.Text = dt.Rows[0]["C_TIPO_DOC"].ToString() + dt.Rows[0]["C_SERIE"].ToString() + dt.Rows[0]["C_NUMERO"].ToString();
                    txtMONEDA.Text = dt.Rows[0]["C_MONEDA"].ToString();
                    txtSALDO.Text = dt.Rows[0]["C_SALDO"].ToString();
                }
                else
                {
                    MessageBox.Show("ERROR, VERIFICAR SI EL NUMERO DE COMPRA ES CORRECTO");
                }
            }

        }


        public void ESTADO_TEXBOX_VENTA(int ESTADO)
        {
            txtID_DOC.Text = string.Empty;
            txtPERSONA.Text = string.Empty;
            txtNUM_DOCUMENTO.Text = string.Empty;
            txtMONEDA.Text = string.Empty;
            txtSALDO.Text = string.Empty;
            if (ESTADO == 1)//ESTADO DE INGRESO POR VENTA
            {
                txtID_DOC.ReadOnly = false;
                txtPERSONA.ReadOnly = true;
                txtNUM_DOCUMENTO.ReadOnly = true;
                txtMONEDA.ReadOnly = true;
                txtSALDO.ReadOnly = true;
            }
            if (ESTADO == 2)//ESTADOO DE BLOQUEADO
            {
                txtID_DOC.ReadOnly = true;
                txtPERSONA.ReadOnly = true;
                txtNUM_DOCUMENTO.ReadOnly = true;
                txtMONEDA.ReadOnly = true;
                txtSALDO.ReadOnly = true;
            }
            
        }
        

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCANCELAR_Click(object sender, EventArgs e)
        {

            ESTADO_TRANSACCION(1); //CON ESTO CONTROLAMOS LA ACTIVIDAD O INACTIVIDAD DE LOS CONTROLES
            FILTRAR_CAJA_KARDEX(0, "1", ""); //AQUI ACTUALIZO Y AGO QUE EL FILTRO SEA POR TODOS LO ACTIVOS 
            rdbACTIVOS.Checked = false;
            rdbANULADOS.Checked = false;
            rdbTODOS.Checked = true;
            txtDATA_BUSQUEDA.Text = string.Empty; //LIMPIAR EL CAMPO DE BUSQUEDA
            dgvMOV_CAJAKARDEX.CurrentCell.Selected = false; //SELECCIONA EL PPRIMER REGISTRO
            SELECCIONAR_REGISTRO_CARGADATA(); //AQUI CARGO POR PRIMERA VEZ TODOS LOS CAMPOS SELECIONADOS DE LA GRILLA
            ESTADO_TEXBOX_VENTA(2);

        }

        private void btnNUEVO_Click(object sender, EventArgs e)
        {
            rdbSOLES.Checked = true;
            ESTADO_TRANSACCION(2); //CON ESTO CONTROLAMOS LA ACTIVIDAD O INACTIVIDAD DE LOS CONTROLES
            dgvMOV_CAJAKARDEX.CurrentCell.Selected = false; //ESTO NO SELECCIONA NINGUN REGISTRO
            LLENAR_COMBO_TIPOMOV();
            LLENAR_COMBO_TIPOPAGO();
            ESTADO_TEXBOX_VENTA(1);
        }

        private void btnGRABAR_Click(object sender, EventArgs e)
        {
            if (VALIDAR_DATOS_CAJA_KARDEX())
            {
                GRABAR_CAJA_KARDEX();
                ESTADO_TRANSACCION(1);//CON ESTO CONTROLAMOS LA ACTIVIDAD O INACTIVIDAD DE LOS CONTROLES
                FILTRAR_CAJA_KARDEX(0, "1", ""); //AQUI ACTUALIZO Y AGO QUE EL FILTRO SEA POR TODOS LO ACTIVOS 
                rdbACTIVOS.Checked = false;
                rdbANULADOS.Checked = false;
                rdbANULADOS.Checked = false;
                txtDATA_BUSQUEDA.Text = string.Empty; //LIMPIAR EL CAMPO DE BUSQUEDA
                dgvMOV_CAJAKARDEX.Rows[0].Selected = true; //SELECCIONA EL PPRIMER REGISTRO
                SELECCIONAR_REGISTRO_CARGADATA(); //AQUI CARGO POR PRIMERA VEZ TODOS LOS CAMPOS SELECIONADOS DE LA GRILLA
                
            }
            else
            {
                MessageBox.Show("ERROR, FALTAN DATOS NECESARIOS POR INGRESAR");
            }
            ESTADO_TEXBOX_VENTA(2);
        }

        private void btnIMPRIMIR_Click(object sender, EventArgs e)
        {
            if (dgvMOV_CAJAKARDEX.Rows.Count != 0 && rdbACTIVOS.Checked == false && rdbTODOS.Checked == false && rdbANULADOS.Checked == false ) //AQUI VALIDO QUE EXISTAN DATOS EN MI GRIDVIEW PARA PODER IMPRIIMIR MIS DATOS
            {

                if (dgvMOV_CAJAKARDEX.CurrentRow.Cells[3].Value.ToString() != "IPV") //AQUI GENERO EL ARCHIVO PDF PARA SU IMPRESION
                {

                    P_IMPRIMIR_GRABAR();

                }
                else //SINO GENERO LA IMPRESION DE LOS TICKET BOLETA
                {
                  
                    
                   
                }
            }
        }

        private void btnBUSCAR_Click(object sender, EventArgs e)
        {
            if (rdbACTIVOS.Checked == true)//ACTIVOS
            {
                FILTRAR_CAJA_KARDEX(cboTIPO_BUSQUEDA.SelectedIndex, "1", txtDATA_BUSQUEDA.Text.ToString());
                dgvMOV_CAJAKARDEX.CurrentCell.Selected = false; //DESELECCIONO LA FILA SELECIONADA DEL GRIDVIEW
            }
            if (rdbANULADOS.Checked == true)//ANULADOS
            {
                FILTRAR_CAJA_KARDEX(cboTIPO_BUSQUEDA.SelectedIndex, "2", txtDATA_BUSQUEDA.Text.ToString());
            }
            if (rdbTODOS.Checked == true)//TODOS
            {
                FILTRAR_CAJA_KARDEX(cboTIPO_BUSQUEDA.SelectedIndex, "3", txtDATA_BUSQUEDA.Text.ToString());
            }
        }

        private void rdbACTIVOS_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbACTIVOS.Checked == true)
            {
                FILTRAR_CAJA_KARDEX(0, "1", "");
                SELECCIONAR_REGISTRO_CARGADATA();
                btnANULAR.Enabled = true;
            }
        }

        private void rdbANULADOS_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbANULADOS.Checked == true)
            {
                FILTRAR_CAJA_KARDEX(0, "2", "");
                SELECCIONAR_REGISTRO_CARGADATA();
                btnANULAR.Enabled = false;
            }
        }

        private void rdbTODOS_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbTODOS.Checked == true)
            {
                FILTRAR_CAJA_KARDEX(0, "3", "");
                SELECCIONAR_REGISTRO_CARGADATA();
                btnANULAR.Enabled = true;
            }
        }

        private void SELECCIONAR_REGISTRO_CARGADATA()
                {
            try
            {
                //========================================================================
                if (dgvMOV_CAJAKARDEX.Rows.Count != 0)
                {
                    //PLANTILLA: dgvMOV_CAJAKARDEX.CurrentRow.Cells[1].ToString();

                    //txtID_MOVIMIENTO.Text = dgvMOV_CAJAKARDEX.SelectedRow.Cells[1].Text;
                    txtID_MOVIMIENTO.Text = dgvMOV_CAJAKARDEX.CurrentRow.Cells[0].Value.ToString();
                    txtFECHA.Text = dgvMOV_CAJAKARDEX.CurrentRow.Cells[19].Value.ToString();
                    //txtFECHA.Text = Convert.ToDateTime(dgvMOV_CAJAKARDEX.CurrentRow.Cells[19].Value).ToString("dd/MM/yyyy");
                    
                    if (dgvMOV_CAJAKARDEX.CurrentRow.Cells[18].Value.ToString() != "&nbsp;")
                    {
                        txtFECHA_ANULADO.Text = dgvMOV_CAJAKARDEX.CurrentRow.Cells[18].Value.ToString();
                    }
                    else
                    {
                        txtFECHA_ANULADO.Text = string.Empty;
                    }
                    cboTIPO_PAGO.SelectedItem = dgvMOV_CAJAKARDEX.CurrentRow.Cells[3].Value.ToString();
                    cboTIPO_MOV.Text = dgvMOV_CAJAKARDEX.CurrentRow.Cells[4].Value.ToString();
                    if (dgvMOV_CAJAKARDEX.CurrentRow.Cells[6].Value.ToString() == "S")
                    {
                        rdbSOLES.Checked = true;
                    }
                    /*if (dgvMOV_CAJAKARDEX.Rows[Int32.Parse(dgvMOV_CAJAKARDEX.SelectedRows.ToString())].Cells[6].Value.ToString() == "D")
                    {
                        rdbDOLARES.Checked = true;
                    }*/

                    txtMONTO.Text = dgvMOV_CAJAKARDEX.CurrentRow.Cells[8].Value.ToString();
                    txtDESCRIPCION.Text = dgvMOV_CAJAKARDEX.CurrentRow.Cells[1].Value.ToString();
                }
                else
                {
                    txtID_MOVIMIENTO.Text = string.Empty;
                    txtFECHA.Text = string.Empty;
                    txtFECHA_ANULADO.Text = string.Empty;
                    cboTIPO_MOV.SelectedIndex = 0;
                    cboTIPO_PAGO.SelectedIndex = 0;
                    txtMONTO.Text = string.Empty;
                    rdbSOLES.Checked = true;
                    txtDESCRIPCION.Text = string.Empty;
                }
                //========================================================================
            }
            catch (Exception)
            {

                MessageBox.Show("ERROR VERIFIQUE SUS DATOS");
            }

        }

        private void dgvMOV_CAJAKARDEX_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           // SELECCIONAR_REGISTRO_CARGADATA();
        }

        private void btnANULAR_Click(object sender, EventArgs e)
        {
            if (txtCODANULACION.Text.ToString() == "CTDIONYS2016")
            {
                ANULAR_CAJA_KARDEX_REGISTRO();
                FILTRAR_CAJA_KARDEX(0, "2", ""); //FILTRO TODOS Y POR ANULADOS
                rdbANULADOS.Checked = true;
                dgvMOV_CAJAKARDEX.SelectedRows.Equals(0); //SELECCIONA EL PPRIMER REGISTRO  /*REVISAR QUE CUMPLA LA ACCION*/<<<------------
                SELECCIONAR_REGISTRO_CARGADATA(); //AQUI CARGO POR PRIMERA VEZ TODOS LOS CAMPOS SELECIONADOS DE LA GRILLA


                if (dgvMOV_CAJAKARDEX.SelectedCells.Count != 0)
                {
                    

                    string id_movent = dgvMOV_CAJAKARDEX.CurrentRow.Cells["ID_COMPVENT"].Value.ToString();
                    string fecha_anulado = Convert.ToDateTime(dgvMOV_CAJAKARDEX.CurrentRow.Cells["FECHA_ANULADO"].Value).ToString("yyyyMMdd hh:mm:ss");
                    
                    con.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE VENTA SET FECHA_ANULADO = '"+ fecha_anulado + "' where ID_VENTA = '"+id_movent+"'", con);
                    cmd.ExecuteNonQuery();
                   // DataTable dt = new DataTable();
                   // SqlDataAdapter da = new SqlDataAdapter(cmd);
                   // da.Fill(dt);
                    con.Close();
                }
                
            }
            else
            {
                MessageBox.Show("ERROR INGRESAR CLAVE DE AUTORIZACION");
            }
            txtCODANULACION.Text = string.Empty;
        }


        // IMPRESIONES SPOOL >>> REVISAR QUE FUNCIONE CORRECTAMENTE<<<<
        // =============================================================================================================================================== 
        public void IMPRIMIR_SPOOL()
        {
            DataTable DATOS_EMPRESA = new DataTable();
            DATOS_EMPRESA = N_OBJEMPRESA.CONSULTAR_VISTA_EMPRESA(Properties.Settings.Default.id_empresa); //AQUI CARGO LOS DATOS DE MI VISTA V_EMPRESA

            DataTable DATOS_SEDE = new DataTable();
            DATOS_SEDE = N_OBJEMPRESA.CONSULTAR_VISTA_SEDE(Program.id_sede); //AQUI CARGO LOS DATOS DE MI VISTA V_SEDE 

            DataTable DATOS_CAJA_KARDEX = new DataTable();                         //ESTO ME PERMITE CREAR EL DATATABLE PARA LLAMAR A LOS DATOS DE MI CAJA KARDEX
            string ID_MOVIMIENTO = txtID_MOVIMIENTO.Text;                          //ESTO PERMITE GENERAR LA VARIABLE DEL ID_MOVIMIENTO

            DATOS_CAJA_KARDEX = N_OBJVENTAS.LISTA_REGISTRO_CAJA_KARDEX(ID_MOVIMIENTO);        //ESTO ME PERMITE ALMACENAR TODOS LOS DATOS EN UN DATATABLE DE MI 
                                                                                              //CAJA_KARDEX PARA PODER ACCEDER A ELLO EN TODO MOMENTO

            //LIMPIANDO MI SPOOL SI ESQUE UBIERA IMPRESIONES PENDIENTES
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, string.Empty, "2");
            // ========================================================================================


            //AQUI ESTOY OBTENIENDO TODOS LOS DATOS DE LA EMPRESA
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, DATOS_EMPRESA.Rows[0]["DESCRIPCION"].ToString(), "1");      //aqui va el nombre de la empresa
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "RUC: " + DATOS_EMPRESA.Rows[0]["RUC"].ToString(), "1");    //aqui va el ruc de la empresa
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "DIRECCION: " + DATOS_EMPRESA.Rows[0]["DIRECCION"].ToString(), "1");
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, DATOS_EMPRESA.Rows[0]["UBIDSN"].ToString() + "-" + DATOS_EMPRESA.Rows[0]["UBIPRN"].ToString() + "-" + DATOS_EMPRESA.Rows[0]["UBIDEN"].ToString(), "1"); //DISTRITO PROVINCIA Y DEPARTAMENTO
            //AQUI ESTOY OBTENIENDO TODOS LOS DATOS DE LA SEDE
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "-", "1");                                        //imprime una linea de guiones
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "SEDE: " + DATOS_SEDE.Rows[0]["ID_SEDE"].ToString() + " " + DATOS_SEDE.Rows[0]["DESCRIPCION"].ToString(), "1"); //aqui va el codigo y el nombre de la sede de la empresa 
            //AQUI ESTOY OBTENIENDO TODOS LOS DATOS DEL PUNTO DE VENTA
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "PV: " + Properties.Settings.Default.punto_venta + " " + Properties.Settings.Default.punto_venta, "1");                 //aqui va el codigo y el nombre del punto de venta
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "-", "1");
            //N_OBJVENTAS.SPOOL_ETIQUETERA(Session["ID_PUNTOVENTA"].ToString(), "MAQ REG : " + DATOS_VENTA.Rows[0][48].ToString(), "1");          //AQUI SE COLOCA EL NOMBRE DE LA MAQUINA REGISTRADORA
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, DATOS_CAJA_KARDEX.Rows[0]["FECHA"].ToString(), "1");   //aqui va la fecha

            // AQUI VA EL NOMBRE  DEL MOVIMIENTO DE LA VENTA O COMPRA
            string TIP_MOV;
            TIP_MOV = DATOS_CAJA_KARDEX.Rows[0]["TM_DESCRIPCION"].ToString();
            //AQUI ESTOY OBTENIENDO EL MOTIVO DE MOVIMIENTO
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "RECIBO: " + TIP_MOV, "1");
            //AQUI ESTOY OBTENIENDO EL ID_MOVIMIENTO Y EL IMPORTE TOTAL DEL MOVIMIENTO
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "#MOV : " + DATOS_CAJA_KARDEX.Rows[0]["ID_MOVIMIENTO"].ToString(), "1");


            if (DATOS_CAJA_KARDEX.Rows[0]["MONEDA"].ToString() == "S")
            {
                N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "IMPORTE : " + " S/. " + Convert.ToDouble(DATOS_CAJA_KARDEX.Rows[0]["IMPORTE"]).ToString("N2"), "1");
            }
            else
            {
                N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "IMPORTE : " + " US$. " + Convert.ToDouble(DATOS_CAJA_KARDEX.Rows[0]["IMPORTE"]).ToString("N2"), "1");
            }


            //AQUI ESTOY OBTENIENDO GUIONES PARA GENERAR UNA LINEA
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "-", "1");

            //AQUI ESTOY OBTENIENDO LA DESCRIPCION DEL MOVIMIENTO DE LA CAJA_KARDEX
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, DATOS_CAJA_KARDEX.Rows[0]["DESCRIPCION"].ToString(), "1");

            //AQUI ESTOY OBTENIENDO GUIONES PARA GENERAR UNA LINEA
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "-", "1");

            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "USUARIO: " + DATOS_CAJA_KARDEX.Rows[0]["EMPLEADO"].ToString(), "1"); //obtenemos la descripcion del cajero o empleado

            //AQUI ESTOY OBTENIENDO GUIONES PARA GENERAR UNA LINEA
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "-", "1");

            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "RECEPTOR: ", "1");

            //AQUI ESTOY OBTENIENDO GUIONES PARA GENERAR UNA LINEA
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "NOMBRE: ____________________________", "1");

            //AQUI ESTOY OBTENIENDO GUIONES PARA GENERAR UNA LINEA
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "DNI: ___________________________", "1");


            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, "CORTATICKET", "1");
            
        }

        private void btnIMPRIMIR_REPORTCAJA_Click(object sender, EventArgs e)
        {
            if (rdbTICKET.Checked == true)
            {
                /*P_IMPRIMIR_MOV_CAJA();*/
                IMPRIMIR_SPOOL_TODOS_MOVCAJA(); //AQUI IMPRIMIMOS TODO LOS MOVIMIENTOS EN UN SOLO REPORTE O IMPRESION EN LA IMPRESORA ETICKETERA
            }
            else
            {
                string ID_EMPRESA = Properties.Settings.Default.id_empresa;
                  
            }
        }

        // IMPRESION TICKET
       
        void P_IMPRIMIR_GRABAR()
        {
            string DIRECCION = "";
            string RUC = "";
            //string ID_VENTA = "";
            string WEB = "";
            con.Open();
            SqlCommand cmv = new SqlCommand("SELECT DIRECCION,RUC,WEB_SITE FROM EMPRESA WHERE DESCRIPCION='" + Properties.Settings.Default.nomempresa + "'", con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmv);
            da.Fill(dt);
            DIRECCION = dt.Rows[0][0].ToString();
            RUC = dt.Rows[0][1].ToString();
            WEB = dt.Rows[0][2].ToString();
            con.Close();

            

            string MAQREG = "";
            string puntoventadesc = "";
            con.Open();
            SqlCommand cmd2 = new SqlCommand("SELECT SERIE_MAQREG,DESCRIPCION FROM PUNTO_VENTA WHERE ID_PUNTOVENTA='" + Properties.Settings.Default.punto_venta + "'", con);
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            da2.Fill(dt2);
            MAQREG = dt2.Rows[0][0].ToString();
            puntoventadesc = dt2.Rows[0][1].ToString();
            con.Close();


            CreaTicket Ticket1 = new CreaTicket();
            Ticket1.impresora = "BIXOLON SRP-270";

            Ticket1.TextoCentro(Properties.Settings.Default.nomempresa);
            Ticket1.TextoCentro("RUC: " + RUC);
            Ticket1.TextoCentro(DIRECCION);
            Ticket1.LineasGuion(); // imprime una linea de guiones
            Ticket1.TextoCentro(Properties.Settings.Default.nomsede);
            Ticket1.TextoCentro(Properties.Settings.Default.punto_venta +" "+ puntoventadesc);
            Ticket1.LineasGuion(); // imprime una linea de guiones

            Ticket1.TextoCentro(DateTime.Now.ToString());
            Ticket1.TextoCentro("RECIBO: " + dgvMOV_CAJAKARDEX.CurrentRow.Cells[4].Value.ToString());
            Ticket1.TextoCentro("#MOV: " + dgvMOV_CAJAKARDEX.CurrentRow.Cells[0].Value.ToString());
            Ticket1.TextoCentro("IMPORTE: " + dgvMOV_CAJAKARDEX.CurrentRow.Cells[5].Value.ToString());
            Ticket1.LineasGuion(); // imprime una linea de guiones
            Ticket1.TextoCentro(txtDESCRIPCION.Text);
            Ticket1.LineasGuion(); // imprime una linea de guiones

            //P_SERIE_Y_NUMERO_CORRELATIVO_POR_PTOVENTA(TIP_DOC, CBOPTOVENTA.Text);
            Ticket1.TextoCentro("USUARIO: "+Properties.Settings.Default.nomempleado);
            Ticket1.LineasGuion();
            Ticket1.TextoCentro("RECEPTOR:");
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("NOMBRE: ________________________________");
            Ticket1.TextoCentro("DNI: ____________________________");
            Ticket1.CortaTicket();
           
        }
        

        void P_IMPRIMIR_MOV_CAJA()
        {
            string DIRECCION = "";
            string RUC = "";
            string WEB = "";
            con.Open();
            SqlCommand cmv = new SqlCommand("SELECT DIRECCION,RUC,WEB_SITE FROM EMPRESA WHERE DESCRIPCION='" + Properties.Settings.Default.nomempresa + "'", con);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmv);
            da.Fill(dt);
            DIRECCION = dt.Rows[0][0].ToString();
            RUC = dt.Rows[0][1].ToString();
            WEB = dt.Rows[0][2].ToString();
            con.Close();
      

            string MAQREG = "";
            string puntoventadesc = "";
            con.Open();
            SqlCommand cmd2 = new SqlCommand("SELECT SERIE_MAQREG,DESCRIPCION FROM PUNTO_VENTA WHERE ID_PUNTOVENTA='" + Properties.Settings.Default.punto_venta + "'", con);
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            da2.Fill(dt2);
            MAQREG = dt2.Rows[0][0].ToString();
            puntoventadesc = dt2.Rows[0][1].ToString();
            con.Close();

            /*----------------------------------------QUERY DETALLE CAJA-------------------------------------------------*/
            string idcaja = Properties.Settings.Default.id_caja;
            string total = "";
            string totalmenosanul = "";
            string totalanul = "";
            string doctotal = "";
            string totDoc = "";
            string docanul = "";
            string tvotros = "";
            string tvefectivo = "";
            string total_iva = "";
            string total_ege = "";
            string total_eva = "";
            double saldoefectivo = 0;


            con.Open();
            SqlCommand com = new SqlCommand("SELECT	SUM(AMORTIZADO) TOTAL_VENTAS,SUM(IMPORTE_CAJA)," +
                                 "(SELECT SUM(IMPORTE_CAJA) FROM V_CAJA_KADEX WHERE FECHA_ANULADO != '' AND ID_CAJA = '" + idcaja + "')," +
                                 "(SELECT COUNT(FECHA_INICIAL) FROM V_CAJA_KADEX WHERE ID_CAJA = '" + idcaja + "')," +
                                 "(SELECT COUNT(FECHA_ANULADO) FROM V_CAJA_KADEX WHERE ID_CAJA = '" + idcaja + "' AND FECHA_ANULADO != '')," +
                                 "(SELECT SUM(IMPORTE_CAJA) FROM V_CAJA_KADEX WHERE ID_CAJA = '" + idcaja + "' AND ID_TIPOMOV = 'IVA')," +
                                 "(SELECT SUM(IMPORTE_CAJA) FROM V_CAJA_KADEX WHERE ID_CAJA = '" + idcaja + "' AND ID_TIPOMOV = 'EVA')," +
                                 "(SELECT SUM(IMPORTE_CAJA) FROM V_CAJA_KADEX WHERE ID_CAJA = '" + idcaja + "' AND ID_TIPOMOV = 'EGE')," +
                                 "(SELECT isnull(SUM(IMPORTE_CAJA), 0) FROM V_CAJA_KADEX WHERE ID_CAJA = '" + idcaja + "' AND TP_DESCRIPCION != 'EFECTIVO')" +
                                 " FROM V_CAJA_KADEX WHERE ID_CAJA = '" + idcaja + "'", con);

            DataTable dtable = new DataTable();
            SqlDataAdapter dadap = new SqlDataAdapter(com);
            dadap.Fill(dtable);
            total = dtable.Rows[0][0].ToString();
            totalmenosanul = dtable.Rows[0][1].ToString();
            totalanul = dtable.Rows[0][2].ToString();
            doctotal = dtable.Rows[0][3].ToString();
            docanul = dtable.Rows[0][4].ToString();
            tvotros = dtable.Rows[0][8].ToString();
            tvefectivo = dtable.Rows[0][0].ToString();
            total_iva = dtable.Rows[0][5].ToString();
            total_ege = dtable.Rows[0][7].ToString();
            total_eva = dtable.Rows[0][6].ToString();
            totDoc = (Convert.ToInt32(doctotal) - Convert.ToInt32(docanul)).ToString();
            saldoefectivo = (Double.Parse(tvefectivo) + Double.Parse(total_iva) + Double.Parse(total_ege) + Double.Parse(total_eva));
            con.Close();
            /*-------------------------------------------------------------------------------------------------------------*/

            int v_numero = 0;
            int id_compvent = 0;
            int totaltickets = 0;
            string tofor = "";
            con.Open();
            SqlCommand COMND = new SqlCommand("SELECT TOP 1 VT.V_NUMERO, ID_COMPVENT,(SELECT COUNT(ID_COMPVENT) FROM V_CAJA_KADEX WHERE ID_CAJA = '" + idcaja + "' GROUP BY ID_CAJA)" +
                                              "FROM V_CAJA_KADEX AS VC INNER JOIN V_TABLA_VENTAS AS VT ON  VC.ID_COMPVENT = VT.V_ID_VENTA WHERE ID_CAJA = '" + idcaja + "'" +
                                              "ORDER BY V_NUMERO",con);
            DataTable DT = new DataTable();
            SqlDataAdapter DAT = new SqlDataAdapter(COMND);
            DAT.Fill(DT);
            v_numero = Convert.ToInt32(DT.Rows[0][0].ToString());
            id_compvent = Convert.ToInt32(DT.Rows[0][1].ToString());
            totaltickets = Convert.ToInt32(DT.Rows[0][2].ToString());
            tofor = (v_numero + totaltickets).ToString("D7");
            con.Close();
            /*-------------------------------------------------------------------------------------------------------------*/
            /*-------------------------------------------------------------------------------------------------------------*/

            v_numero = Convert.ToInt32(DT.Rows[0][0].ToString());
            id_compvent = Convert.ToInt32(DT.Rows[0][1].ToString());
            totaltickets = Convert.ToInt32(DT.Rows[0][2].ToString());
            tofor = (v_numero + totaltickets).ToString("D7");

            double cantTB = 0;
            double cantBV = 0;
            double cantFV = 0;
            double initb = 0;
            double fintb = 0;
            double inibv = 0;
            double finbv = 0;
            double inifv = 0;
            double finfv = 0;
            con.Open();
            SqlCommand comand = new SqlCommand("SELECT  VT.V_TIPO_DOC, VT.V_NUMERO, ID_COMPVENT FROM V_CAJA_KADEX AS VC"+
                                               " INNER JOIN V_TABLA_VENTAS AS VT ON  VC.ID_COMPVENT = VT.V_ID_VENTA"+
                                               " WHERE ID_CAJA = '" + idcaja + "' ORDER BY V_TIPO_DOC", con);
            DataTable datatable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(comand);
            adapter.Fill(datatable);
            for (int i = 0;i<datatable.Rows.Count;i++)
            {
                if (datatable.Rows[i][0].ToString() == "TB")
                {
                    cantTB = cantTB + 1;
                    if (initb == 0)
                    {
                        initb = Convert.ToDouble(datatable.Rows[i][1].ToString());
                    }
                }
                if (datatable.Rows[i][0].ToString() == "BV")
                {
                    cantBV = cantBV + 1;
                    if (inibv == 0)
                    {
                        inibv = Convert.ToDouble(datatable.Rows[i][1].ToString());
                    }
                }
                if (datatable.Rows[i][0].ToString() == "FV")
                {
                    cantFV = cantFV + 1;
                    if (inifv == 0)
                    {
                        inifv = Convert.ToDouble(datatable.Rows[i][1].ToString());
                    }
                }
            }
            fintb = initb + cantTB;
            finbv = inibv + cantBV;
            finfv = inifv + cantFV;
            con.Close();
           
            CreaTicket Ticket1 = new CreaTicket();
            Ticket1.impresora = "BIXOLON SRP-270";

            Ticket1.TextoCentro("---- REPORTE DE CAJA ----");
            Ticket1.TextoCentro(Properties.Settings.Default.nomempresa);
            Ticket1.TextoCentro("RUC: " + RUC);
            Ticket1.TextoCentro(DIRECCION);
            Ticket1.LineasGuion(); // imprime una linea de guiones
            Ticket1.TextoCentro(Properties.Settings.Default.nomsede);
            Ticket1.TextoCentro("PV: " + Properties.Settings.Default.punto_venta + " " + puntoventadesc);
            Ticket1.TextoCentro("FECHA APERTURA: " + Properties.Settings.Default.fecha_apertura_caja);
            Ticket1.LineasGuion(); // imprime una linea de guiones

            Ticket1.TextoCentro("---- DETALLE INGRESOS POR VENTA ----");
            Ticket1.LineasGuion(); // imprime una linea de guiones

            /*--------------PRUEBA DETALLE CAJA-------------*/
            if (fintb != 0)///CORREGIR
            {
                Ticket1.TextoCentro("TB DESDE: " + Properties.Settings.Default.serie + "-" + initb.ToString("0000000"));
                Ticket1.TextoCentro("TB HASTA: " + Properties.Settings.Default.serie + "-" + fintb.ToString("0000000"));
                Ticket1.TextoCentro("");
            }
            if (finbv != 0)///CORREGIR
            {
                Ticket1.TextoCentro("BV DESDE: " + Properties.Settings.Default.serie + "-" + inibv.ToString("0000000"));
                Ticket1.TextoCentro("BV HASTA: " + Properties.Settings.Default.serie + "-" + finbv.ToString("0000000"));
                Ticket1.TextoCentro("");
            }
            if (finfv != 0)///CORREGIR
            {
                Ticket1.TextoCentro("FV DESDE: " + Properties.Settings.Default.serie + "-" + inifv.ToString("0000000"));
                Ticket1.TextoCentro("FV HASTA: " + Properties.Settings.Default.serie + "-" + finfv.ToString("0000000"));
                Ticket1.TextoCentro("");
            }
            
            Ticket1.TextoCentro("TOTAL ANULADOS: " + docanul + " DOC  S/. " + totalanul);
            Ticket1.TextoCentro("TOTAL VENTAS: " + totDoc + " DOC  S/. " + total);
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("T.V. EFECTIVO: " + tvefectivo);
            Ticket1.TextoCentro("T.V. OTROS: " + tvotros);
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("---- DETALLE INGRESOS OTROS ----");
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("TOTAL IVA: " + total_iva);
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("---- DETALLE DE EGRESOS ----");
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("TOTAL EGE: " + total_ege);
            Ticket1.TextoCentro("TOTAL EVA: " + total_eva);
            Ticket1.TextoCentro("");
            Ticket1.LineasGuion(); // imprime una linea de guiones
            Ticket1.TextoCentro("SALDO EFECTIVO: " + saldoefectivo);
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("");
            Ticket1.LineasGuion(); // imprime una linea de guiones
            Ticket1.TextoCentro("V.B:" + Properties.Settings.Default.nomempleado);
            Ticket1.TextoCentro(Properties.Settings.Default.id_empleado);
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("");
            Ticket1.LineasGuion(); // imprime una linea de guiones
            Ticket1.TextoCentro("V.B: ADMINISTRACION");
            Ticket1.TextoCentro("FECHA IMPRESION : " + DateTime.Now.ToString());

            Ticket1.CortaTicket();
          
        }
        
        void IMPRIMIR_SPOOL_TODOS_MOVCAJA()
        {
            DataTable DATOS_EMPRESA = new DataTable();
            DATOS_EMPRESA =  N_OBJEMPRESA.CONSULTAR_VISTA_EMPRESA(Properties.Settings.Default.id_empresa); //AQUI CARGO LOS DATOS DE MI VISTA V_EMPRESA

            DataTable DATOS_SEDE = new DataTable();
            DATOS_SEDE = N_OBJEMPRESA.CONSULTAR_VISTA_SEDE(Program.id_sede); //AQUI CARGO LOS DATOS DE MI VISTA V_SEDE 

            DataTable DATOS_CAJA_KARDEX = new DataTable();                         //ESTO ME PERMITE CREAR EL DATATABLE PARA LLAMAR A LOS DATOS DE MI CAJA KARDEX
            string ID_CAJA = Properties.Settings.Default.id_caja;                          //ESTO PERMITE GENERAR LA VARIABLE DEL ID_MOVIMIENTO

            DATOS_CAJA_KARDEX = N_OBJVENTAS.CONSULTA_IMPRESION_CAJA_KARDEX(ID_CAJA);        //ESTO ME PERMITE ALMACENAR TODOS LOS DATOS EN UN DATATABLE DE MI 
                                                                                            //CAJA_KARDEX PARA PODER ACCEDER A ELLO EN TODO MOMENTO

            //LIMPIANDO MI SPOOL SI ESQUE UBIERA IMPRESIONES PENDIENTES
            N_OBJVENTAS.SPOOL_ETIQUETERA(Properties.Settings.Default.punto_venta, string.Empty, "2");
            // ========================================================================================


            CreaTicket Ticket1 = new CreaTicket();
            Ticket1.impresora = "BIXOLON SRP-270";

            Ticket1.TextoCentro("---- REPORTE DE CAJA ----");
            Ticket1.TextoCentro(Properties.Settings.Default.nomempresa);
            Ticket1.TextoCentro("RUC: " + DATOS_EMPRESA.Rows[0]["RUC"].ToString());
            Ticket1.TextoCentro(DATOS_EMPRESA.Rows[0]["DIRECCION"].ToString());
            Ticket1.LineasGuion(); // imprime una linea de guiones
            Ticket1.TextoCentro(Properties.Settings.Default.nomsede);
            Ticket1.TextoCentro("PV: " + Properties.Settings.Default.punto_venta + " " + DATOS_CAJA_KARDEX.Rows[0]["PV_DESCRIPCION"].ToString());
            Ticket1.TextoCentro("FECHA APERTURA: " + Properties.Settings.Default.fecha_apertura_caja);
            Ticket1.LineasGuion(); // imprime una linea de guiones

            Ticket1.TextoCentro("---- DETALLE INGRESOS POR VENTA ----");
            Ticket1.LineasGuion(); // imprime una linea de guiones

            //AQUI ESTOY OBTENIENDO TODOS LOS DATOS DE LA EMPRESA
   

            string ANULADO = string.Empty;
            double TOTALANU = 0.00;
            int CONTANU = 0, CONTTOTAL = 0, IPV_CANT = 0, EVA_CANT = 0;
            int IVA_CANT = 0, EGE_CANT = 0;
            double TOTALMOV = 0.00;
            double IPV_EFECTIVO = 0.00, EVA_EFECTIVO = 0.00, IPV_EFECTIVO_OTROS = 0.00;
            double IVA_EFECTIVO = 0.00, EGE_EFECTIVO = 0.00;

            //GENERAR LOS REGISTROS DE INGRESOS POR VENTA

            Ticket1.TextoCentro("");

            Ticket1.TextoCentro("FECHA  TPAGO TDOC # DOC        M  IMPORTE A");
            for (int i = 0; i < DATOS_CAJA_KARDEX.Rows.Count; i++)
            {
                ANULADO = " ";
                string varMOVIMIENTO = DATOS_CAJA_KARDEX.Rows[i]["ID_TIPOMOV"].ToString();
                if (DATOS_CAJA_KARDEX.Rows[i]["FECHA_ANULADO"] != DBNull.Value && varMOVIMIENTO.ToString() == "IPV")
                {
                    ANULADO = "*";
                    CONTANU = CONTANU + 1;
                    TOTALANU = TOTALANU + Convert.ToDouble(DATOS_CAJA_KARDEX.Rows[i]["IMPORTE"]); //TOTALIZANDO LOS ANULADOS
                }

                //==============================================================================================================
                //OBTENGO EL VALOR DE MI CAMPO ID_TIPOMOV PARA VERIFICAR SI TIENE DATO O NO , PARA REALIZAR LA COMPARACIONES
                string varID_TIPOPAGO = DATOS_CAJA_KARDEX.Rows[i]["ID_TIPOPAGO"].ToString();
                string tipo_pago = DATOS_CAJA_KARDEX.Rows[i]["ID_TIPOPAGO"].ToString();
                string Corte = "";
                if (varMOVIMIENTO == "IPV")
                {
                    if (tipo_pago == "0001")
                    {
                        Corte = "E";
                    }
                    else if (tipo_pago == "0002")
                    {
                        Corte = "T";
                    }
                    else if (tipo_pago == "0003")
                    {
                        Corte = "T";
                    }
                    else if (tipo_pago == "0004")
                    {
                        Corte = "D";
                    }
                    else if (tipo_pago == "0005")
                    {
                        Corte = "T";
                    }
                    else if (tipo_pago == "0006")
                    {
                        Corte = "C";
                    }
                    Ticket1.TextoIzquierda(Convert.ToDateTime(DATOS_CAJA_KARDEX.Rows[i]["FECHA"]).ToString("dd/MM/yy") + "  " + Corte + " " + DATOS_CAJA_KARDEX.Rows[i]["TIPO_DOCU"].ToString() + "  " +
                    DATOS_CAJA_KARDEX.Rows[i]["NUMERO"].ToString() + " " + DATOS_CAJA_KARDEX.Rows[i]["MONEDA"].ToString() +"    " + DATOS_CAJA_KARDEX.Rows[i]["IMPORTE"].ToString() + " " + ANULADO);
                }

                //AQUI PROCESO LA INFORMACION PARA OBTENER LAS SUMAS DE LOS INGRESOS POR VENTA EN EFECTIVO Y QUE NO ESTEN ANULADOS
                if (varMOVIMIENTO == "IPV" && varID_TIPOPAGO == "0001" && DATOS_CAJA_KARDEX.Rows[i]["FECHA_ANULADO"] == DBNull.Value)
                {
                    IPV_EFECTIVO += Convert.ToDouble(DATOS_CAJA_KARDEX.Rows[i]["IMPORTE"]);
                    IPV_CANT += 1;
                }

                //AQUI PROCESO LA INFORMACION PARA OBTENER LAS SUMAS DE LOS INGRESOS POR VENTA CON TARJETA CREDITO O DEBITO Y QUE NO ESTEN ANULADOS
                if (varMOVIMIENTO == "IPV" && (varID_TIPOPAGO == "0002" || varID_TIPOPAGO == "0003") && DATOS_CAJA_KARDEX.Rows[i]["FECHA_ANULADO"] == DBNull.Value)
                {
                    IPV_EFECTIVO_OTROS += Convert.ToDouble(DATOS_CAJA_KARDEX.Rows[i]["IMPORTE"]);
                }

                //AQUI OBTENGO EL TOTAL DE ACTIVOS
                if (DATOS_CAJA_KARDEX.Rows[i]["FECHA_ANULADO"] == DBNull.Value && varMOVIMIENTO == "IPV")
                {
                    CONTTOTAL += 1;
                }

            }
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("TOTAL ANULADOS: " + CONTANU + " DOC  S/. " + TOTALANU.ToString("N2"));
            Ticket1.TextoCentro("TOTAL VENTAS: " + CONTTOTAL + " DOC  S/. " + (IPV_EFECTIVO + IPV_EFECTIVO_OTROS).ToString("N2"));
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("T.V. EFECTIVO: S/. " + IPV_EFECTIVO.ToString("N2"));
            Ticket1.TextoCentro("T.V. OTROS: S/. " + IPV_EFECTIVO_OTROS.ToString("N2"));
            
            Ticket1.TextoCentro("");                        // imprime una espacio
            Ticket1.TextoCentro("-----DETALLE INGRESOS OTROS-----");
            Ticket1.TextoCentro("");
            for (int i = 0; i < DATOS_CAJA_KARDEX.Rows.Count; i++)
            {
                ANULADO = " ";
                if (DATOS_CAJA_KARDEX.Rows[i]["FECHA_ANULADO"] != DBNull.Value)
                {
                    ANULADO = "*";
                }

                string varID_TIPOMOV = DATOS_CAJA_KARDEX.Rows[i]["ID_TIPOMOV"].ToString(); //OBTENGO EL VALOR DE MI CAMPO ID_COMVENTA PARA VERIFICAR SI TIENE DATO O NO , PARA REALIZAR LA COMPARACIONES
                if (varID_TIPOMOV == "IVA")
                {
                    Ticket1.TextoCentro(Convert.ToDateTime(DATOS_CAJA_KARDEX.Rows[i]["FECHA"]).ToString("dd/MM/yy") + " " + DATOS_CAJA_KARDEX.Rows[i]["ID_TIPOMOV"].ToString() + " " +
                    DATOS_CAJA_KARDEX.Rows[i]["ID_MOVIMIENTO"].ToString() + "  " + DATOS_CAJA_KARDEX.Rows[i]["MONEDA"].ToString() + "  " + DATOS_CAJA_KARDEX.Rows[i]["IMPORTE"].ToString() + "      " + ANULADO);
                }

                //AQUI CALCULO EL TOTAL DE LOS EGRESOS GERENCIA
                if (varID_TIPOMOV == "IVA" && DATOS_CAJA_KARDEX.Rows[i]["FECHA_ANULADO"] == DBNull.Value)
                {
                    IVA_EFECTIVO += Convert.ToDouble(DATOS_CAJA_KARDEX.Rows[i]["IMPORTE"]);
                    IVA_CANT += 1;
                }

            }
            Ticket1.TextoCentro("");                         // imprime una espacio
            Ticket1.TextoCentro("TOTAL IVA : S/. " + IVA_EFECTIVO.ToString("N2"));//IMPRIMIENDO TOTAL DE EFECTIVO

            //GENERAR LOS REGISTROS DE EGRESOS
            Ticket1.TextoCentro("");                        // imprime una espacio
            Ticket1.TextoCentro("-------DETALLE DE EGRESOS-------");
            Ticket1.TextoCentro("");
            for (int i = 0; i < DATOS_CAJA_KARDEX.Rows.Count; i++)
            {
                ANULADO = " ";
                if (DATOS_CAJA_KARDEX.Rows[i]["FECHA_ANULADO"] != DBNull.Value)
                {
                    ANULADO = "*";
                }

                string varID_TIPOMOV = DATOS_CAJA_KARDEX.Rows[i]["ID_TIPOMOV"].ToString(); //OBTENGO EL VALOR DE MI CAMPO ID_COMVENTA PARA VERIFICAR SI TIENE DATO O NO , PARA REALIZAR LA COMPARACIONES

                if (varID_TIPOMOV == "EGE" || varID_TIPOMOV == "EVA")
                {
                    Ticket1.TextoCentro(Convert.ToDateTime(DATOS_CAJA_KARDEX.Rows[i]["FECHA"]).ToString("dd/MM/yy") + " " + DATOS_CAJA_KARDEX.Rows[i]["ID_TIPOMOV"].ToString() + " " +
                    DATOS_CAJA_KARDEX.Rows[i]["ID_MOVIMIENTO"].ToString() + "  " + DATOS_CAJA_KARDEX.Rows[i]["MONEDA"].ToString() + "  " + DATOS_CAJA_KARDEX.Rows[i]["IMPORTE"].ToString() + " " + ANULADO);
                }

                //AQUI CALCULO EL TOTAL DE LOS EGRESOS GERENCIA
                if (varID_TIPOMOV == "EGE" && DATOS_CAJA_KARDEX.Rows[i]["FECHA_ANULADO"] == DBNull.Value)
                {
                    EGE_EFECTIVO += Convert.ToDouble(DATOS_CAJA_KARDEX.Rows[i]["IMPORTE"]);
                    EGE_CANT += 1;
                }

                //AQUI CALCULO EL TOTAL DE LOS EGRESOS VARIOS
                if (varID_TIPOMOV == "EVA" && DATOS_CAJA_KARDEX.Rows[i]["FECHA_ANULADO"] == DBNull.Value)
                {
                    EVA_EFECTIVO += Convert.ToDouble(DATOS_CAJA_KARDEX.Rows[i]["IMPORTE"]);
                    EVA_CANT += 1;
                }

            }
            Ticket1.TextoCentro("");                          // imprime una espacio
            Ticket1.TextoCentro("TOTAL EGE: S/. " + EGE_EFECTIVO.ToString("N2"));//IMPRIMIENDO TOTAL DE EFECTIVO
            Ticket1.TextoCentro("TOTAL EVA: S/. " + EVA_EFECTIVO.ToString("N2"));//IMPRIMIENDO TOTAL DE EFECTIVO
            Ticket1.TextoCentro("");
            Ticket1.LineasGuion();                         // imprime una linea de guiones
            Ticket1.TextoCentro("SALDO EFECTIVO CAJA: S/. " + ((IPV_EFECTIVO + IVA_EFECTIVO) - (EGE_EFECTIVO + EVA_EFECTIVO)).ToString("N2"));//IMPRIMIENDO TOTAL DE EFECTIVO
            Ticket1.LineasGuion();                                                                                                                 //=======================================================================================================================================================
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("");
            Ticket1.LineasGuion();                           // imprime una linea de guiones
            Ticket1.TextoCentro("V.B: " + Properties.Settings.Default.nomempleado); // obtenemos el NOMBRE DEL EMPLEADO
            Ticket1.TextoCentro("  " + Properties.Settings.Default.id_empleado); // obtenemos el USUARIO/DNI DEL EMPLEADO
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("");
            Ticket1.TextoCentro("");
            Ticket1.LineasGuion();                            // imprime una linea de guiones
            Ticket1.TextoCentro("V.B: ADMINISTRACION");
            Ticket1.TextoCentro("FECHA IMPRESION : " + DateTime.Now.ToShortDateString()); //formato de fecha g = 6/15/2008 9:15 PM
            Ticket1.CortaTicket();

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CONSULTAR_VENTAS(txtID_DOC.Text);
        }

        private void cboTIPO_MOV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTIPO_MOV.SelectedValue.ToString() == "EPC")
            {
                ESTADO_TEXBOX_VENTA(1);
            }
            if (cboTIPO_MOV.SelectedValue.ToString() == "EVA")
            {
                ESTADO_TEXBOX_VENTA(2);
            }
            if (cboTIPO_MOV.SelectedValue.ToString() == "IPV")
            {
                ESTADO_TEXBOX_VENTA(1);
            }
            if (cboTIPO_MOV.SelectedValue.ToString() == "IVA")
            {
                ESTADO_TEXBOX_VENTA(2);
            }
            if (cboTIPO_MOV.SelectedValue.ToString() == "EGE")
            {
                ESTADO_TEXBOX_VENTA(2);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {

            /*--------------------------VARIABLES DE RETORNO A CAJA--------------------------*/
            this.Hide();
            CAJA OBJCAJA = new CAJA();
            OBJCAJA.txtIDcaja.Text = Properties.Settings.Default.id_caja;
            OBJCAJA.id_empleado = m_id_empleado;
            OBJCAJA.id_puntoventa = m_id_puntoventa;
            OBJCAJA.sede = m_sede;
            OBJCAJA.tipo_cambio = m_tipo_cambio;
            OBJCAJA.nombre_empleado = m_nombre_empleado;
            OBJCAJA.id_empresa = m_id_empresa;
            OBJCAJA.Show();
            /*---------------------------------------------------------------------------------*/
        }

        private void dgvMOV_CAJAKARDEX_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SELECCIONAR_REGISTRO_CARGADATA();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            
        }

        private void cboTIPO_BUSQUEDA_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtMONTO_KeyPress(object sender, KeyPressEventArgs e)
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
            if (e.KeyChar == (char)13) { txtMONTO.Text = string.Format("{0:n0}", decimal.Parse(txtMONTO.Text)); }

        }
        /* ================================================ METODO PARA GENERAR TICKET 1 PARTE ===============================================================*/
        /* ================================================ METODO PARA GENERAR TICKET 1 PARTE ===============================================================*/
        /* ================================================ METODO PARA GENERAR TICKET 1 PARTE ===============================================================*/
        /* ================================================ METODO PARA GENERAR TICKET 1 PARTE ===============================================================*/

        public class CreaTicket
        {
            public string impresora;
            //{

            string ticket = "";
            string parte1, parte2;
            //string impresora = "\\\\FARMACIA-PVENTA\\Generic / Text Only"; // nombre exacto de la impresora como esta en el panel de control
            //string impresora = "Generic / Text Only"; // nombre exacto de la impresora como esta en el panel de control
            // string impresora = NombreImpresora; // nombre exacto de la impresora como esta en el panel de control
            int max, cort;
            public void LineasGuion()
            {
                ticket = "----------------------------------------\n";   // agrega lineas separadoras -
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime linea
            }
            public void LineasAsterisco()
            {
                ticket = "****************************************\n";   // agrega lineas separadoras *
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime linea
            }
            public void LineasIgual()
            {
                ticket = "========================================\n";   // agrega lineas separadoras =
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime linea
            }
            public void LineasTotales()
            {
                ticket = "                             -----------\n"; ;   // agrega lineas de total
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime linea
            }
            public void EncabezadoVenta()
            {
                //ticket = "Articulo        Can    P.Unit    Importe\n";   // agrega lineas de  encabezados
                ticket = "Cant       Articulo              Importe\n";   // agrega lineas de  encabezados
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime texto
            }
            public void TextoIzquierda(string par1)                          // agrega texto a la izquierda
            {
                max = par1.Length;
                if (max > 40)                                 // **********
                {
                    cort = max - 40;
                    parte1 = par1.Remove(40, cort);        // si es mayor que 40 caracteres, lo corta
                }
                else { parte1 = par1; }                      // **********
                ticket = parte1 + "\n";
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime texto
            }
            public void TextoDerecha(string par1)
            {
                ticket = "";
                max = par1.Length;
                if (max > 40)                                 // **********
                {
                    cort = max - 40;
                    parte1 = par1.Remove(40, cort);           // si es mayor que 40 caracteres, lo corta
                }
                else { parte1 = par1; }                      // **********
                max = 40 - par1.Length;                     // obtiene la cantidad de espacios para llegar a 40
                for (int i = 0; i < max; i++)
                {
                    ticket += " ";                          // agrega espacios para alinear a la derecha
                }
                ticket += parte1 + "\n";                    //Agrega el texto
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime texto
            }
            public void TextoCentro(string par1)
            {
                ticket = "";
                max = par1.Length;
                if (max > 40)                                 // **********
                {
                    cort = max - 40;
                    parte1 = par1.Remove(40, cort);          // si es mayor que 40 caracteres, lo corta
                }
                else { parte1 = par1; }                      // **********
                max = (int)(40 - parte1.Length) / 2;         // saca la cantidad de espacios libres y divide entre dos
                for (int i = 0; i < max; i++)                // **********
                {
                    ticket += " ";                           // Agrega espacios antes del texto a centrar
                }                                            // **********
                ticket += parte1 + "\n";
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime texto
            }
            public void TextoExtremos(string par1, string par2)
            {
                max = par1.Length;
                if (max > 25)                                 // **********
                {
                    cort = max - 25;
                    parte1 = par1.Remove(25, cort);          // si par1 es mayor que 18 lo corta
                }
                else { parte1 = par1; }                      // **********
                ticket = parte1;                             // agrega el primer parametro
                max = par2.Length;
                if (max > 25)                                 // **********
                {
                    cort = max - 25;
                    parte2 = par2.Remove(25, cort);          // si par2 es mayor que 18 lo corta
                }
                else { parte2 = par2; }
                max = 40 - (parte1.Length + parte2.Length);
                for (int i = 0; i < max; i++)                 // **********
                {
                    ticket += " ";                            // Agrega espacios para poner par2 al final
                }                                             // **********
                ticket += parte2 + "\n";                     // agrega el segundo parametro al final
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime texto
            }
            public void AgregaTotales(string par1, double total)
            {
                max = par1.Length;
                if (max > 25)                                 // **********
                {
                    cort = max - 25;
                    parte1 = par1.Remove(25, cort);          // si es mayor que 25 lo corta
                }
                else { parte1 = par1; }                      // **********
                ticket = parte1;
                parte2 = total.ToString("");
                max = 40 - (parte1.Length + parte2.Length);
                for (int i = 0; i < max; i++)                // **********
                {
                    ticket += " ";                           // Agrega espacios para poner el valor de moneda al final
                }                                            // **********
                ticket += parte2 + "\n";
                RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime texto
            }
            //public void AgregaArticulo(string par1, int cant, double precio, double total)
            //{
            //    if (cant.ToString().Length <= 3 && precio.ToString("c").Length <= 10 && total.ToString("c").Length <= 11) // valida que cant precio y total esten dentro de rango
            //    {
            //        max = par1.Length;
            //        if (max > 16)                                 // **********
            //        {
            //            cort = max - 16;
            //            parte1 = par1.Remove(16, cort);          // corta a 16 la descripcion del articulo
            //        }
            //        else { parte1 = par1; }                      // **********
            //        ticket = parte1;                             // agrega articulo
            //        max = (3 - cant.ToString().Length) + (16 - parte1.Length);
            //        for (int i = 0; i < max; i++)                // **********
            //        {
            //            ticket += " ";                           // Agrega espacios para poner el valor de cantidad
            //        }
            //        ticket += cant.ToString();                   // agrega cantidad
            //        max = 10 - (precio.ToString("").Length);
            //        for (int i = 0; i < max; i++)                // **********
            //        {
            //            ticket += " ";                           // Agrega espacios
            //        }                                            // **********
            //        ticket += precio.ToString(""); // agrega precio
            //        max = 11 - (total.ToString().Length);
            //        for (int i = 0; i < max; i++)                // **********
            //        {
            //            ticket += " ";                           // Agrega espacios
            //        }                                            // **********
            //        ticket += total.ToString("") + "\n"; // agrega precio
            //        RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime texto
            //    }
            //    else
            //    {
            //        MessageBox.Show("Valores fuera de rango");
            //        RawPrinterHelper.SendStringToPrinter(impresora, "Error, valor fuera de rango\n"); // imprime texto
            //    }
            //}
            //*****************************+

            //public void AgregaArticulo(string cant, string par1, double precio, double total)
            public void AgregaArticulo(string cant, string par1, string total)
            {
                //if (cant.ToString().Length <= 7 && precio.ToString("c").Length <= 10 && total.ToString("c").Length <= 18) // valida que cant precio y total esten dentro de rango
                if (cant.ToString().Length <= 7 && total.ToString().Length <= 15) // valida que cant precio y total esten dentro de rango
                {

                    ticket = "";
                    max = (7 - cant.ToString().Length);

                    for (int i = 0; i < max; i++)                // **********
                    {
                        ticket += " ";                           // Agrega espacios para poner el valor de cantidad
                    }
                    ticket += cant.ToString();                   // agrega cantidad
                                                                 //**************************************************************+
                    max = par1.Length;
                    if (max > 18)                                 // **********
                    {
                        cort = max - 18;
                        parte1 = par1.Remove(18, cort);          // corta a 16 la descripcion del articulo
                    }
                    else { parte1 = par1; }                      // **********
                    ticket += " " + parte1.ToString(); // agrega articulo

                    max = 15 - (total.ToString().Length);
                    for (int i = 0; i < max; i++)                // **********
                    {
                        ticket += " ";                           // Agrega espacios
                    }                                            // **********
                    ticket += total.ToString() + "\n"; // agrega total linea
                    RawPrinterHelper.SendStringToPrinter(impresora, ticket); // imprime texto
                }
                else
                {
                    String formato = String.Format("<script>javascript:mensaje('VALORES FUERA DE RANGO');</script>");

                    // MessageBox.Show("Valores fuera de rango");
                    RawPrinterHelper.SendStringToPrinter(impresora, "Error, valor fuera de rango\n"); // imprime texto
                }
            }
            //***************************************+
            public void CortaTicket()
            {
                string corte = "\x1B" + "m";                  // caracteres de corte
                string avance = "\x1B" + "d" + "\x09";        // avanza 9 renglones
                RawPrinterHelper.SendStringToPrinter(impresora, avance); // avanza
                RawPrinterHelper.SendStringToPrinter(impresora, corte); // corta
            }
            public void AbreCajon()
            {
                string cajon0 = "\x1B" + "p" + "\x00" + "\x0F" + "\x96";                  // caracteres de apertura cajon 0
                string cajon1 = "\x1B" + "p" + "\x01" + "\x0F" + "\x96";                 // caracteres de apertura cajon 1
                RawPrinterHelper.SendStringToPrinter(impresora, cajon0); // abre cajon0
                                                                         //RawPrinterHelper.SendStringToPrinter(impresora, cajon1); // abre cajon1
            }
        }




        /*===============================================================================================================================================*/
        /* ================================================ METODOS TICKET 2 PARTE ===============================================================*/

        public class RawPrinterHelper
        {
            // Structure and API declarions:
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
            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

            [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndDocPrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

            // SendBytesToPrinter()
            // When the function is given a printer name and an unmanaged array
            // of bytes, the function sends those bytes to the print queue.
            // Returns true on success, false on failure.
            public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
            {
                Int32 dwError = 0, dwWritten = 0;
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();
                bool bSuccess = false; // Assume failure unless you specifically succeed.

                di.pDocName = "My C#.NET RAW Document";
                di.pDataType = "RAW";

                // Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    // Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        // Start a page.
                        if (StartPagePrinter(hPrinter))
                        {
                            // Write your bytes.
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
                // If you did not succeed, GetLastError may give more information
                // about why not.
                if (bSuccess == false)
                {
                    dwError = Marshal.GetLastWin32Error();
                }
                return bSuccess;
            }

            public static bool SendStringToPrinter(string szPrinterName, string szString)
            {
                IntPtr pBytes;
                Int32 dwCount;
                // How many characters are in the string?
                dwCount = szString.Length;
                // Assume that the printer is expecting ANSI text, and then convert
                // the string to ANSI text.
                pBytes = Marshal.StringToCoTaskMemAnsi(szString);
                // Send the converted ANSI string to the printer.
                SendBytesToPrinter(szPrinterName, pBytes, dwCount);
                Marshal.FreeCoTaskMem(pBytes);
                return true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
