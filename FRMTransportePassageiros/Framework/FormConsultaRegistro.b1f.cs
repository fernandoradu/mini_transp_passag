using FRMTransportePassageiros.Framework.Classes;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRMTransportePassageiros.Framework
{
    [FormAttribute("FRMTransportePassageiros.Framework.FormConsultaRegistro", "Framework/FormConsultaRegistro.b1f")]
    public class FormConsultaRegistro : UserFormBase
    {
        public FormConsultaRegistro()
        {
            
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            //this.UIAPIRawForm.DataSources.DataTables.Add("DTConsul");
            this.lblProcurar = ((SAPbouiCOM.StaticText)(this.GetItem("lblPesq").Specific));
            this.txtProcurar = ((SAPbouiCOM.EditText)(this.GetItem("Item_1").Specific));
            this.mtxConsulta = ((SAPbouiCOM.Matrix)(this.GetItem("mtxCons1").Specific));
            this.btnSelecionar = ((SAPbouiCOM.Button)(this.GetItem("btnSelec").Specific));
            this.btnCancelar = ((SAPbouiCOM.Button)(this.GetItem("btnCanc1").Specific));
            //this.dtConsulta = this.UIAPIRawForm.DataSources.DataTables.Item("DTConsul"); 
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
           //this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);
        }
        //Este método é essencial para inicialização dos componentes (ou itens de form)
        //que 
        private void OnCustomInitialize()
        {
            this.Form_LoadAfter();
        }

        public void SetFilter(string[,] filtros)
        {
            this.filter = filtros;
        }
        public void SetQuery(bool reset = false, string[,] filtros = null)
        {

            if (query == "" || reset)
            {
                StringBuilder buildQuery = new StringBuilder();
                
                if (filtros != null)
                    this.SetFilter(filtros);

                buildQuery.Append("SELECT");
                buildQuery.Append(" T0.\"Code\",");
                buildQuery.Append(" T0.\"Name\" ");
                buildQuery.Append("FROM");
                buildQuery.AppendFormat(" \"{0}\" T0",_userTable);
                
                if ( filter != null )
                {
                    buildQuery.Append("WHERE ");

                    for (int ind = 0; ind <= filter.Length -1; ind++)
                    {
                        buildQuery.AppendFormat(" T0.\"{0}\" = '{1}' ", filter[ind,1], filter[ind,2]);

                        if (ind < filter.Length - 1)
                            buildQuery.Append(" AND ");

                    }                    
                }

                query = buildQuery.ToString();                
            }
        }

        public void MontaMatrizConsulta()
        {
            try
            {
                this.UIAPIRawForm.DataSources.DataTables.Item("DTCon1").ExecuteQuery(this.query);
                //this.dtConsulta.ExecuteQuery(this.query);
                mtxConsulta.Columns.Item("cCodCon").DataBind.Bind("DTCon1", "Code");
                mtxConsulta.Columns.Item("cNameCon").DataBind.Bind("DTCon1", "Name");

                mtxConsulta.AutoResizeColumns();

                this.UIAPIRawForm.Freeze(true);

                mtxConsulta.LoadFromDataSource();

                this.UIAPIRawForm.Freeze(false);

            }
            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, false);                
            }
        }

        private SAPbouiCOM.StaticText lblProcurar;
        private SAPbouiCOM.EditText txtProcurar;
        private SAPbouiCOM.Matrix mtxConsulta;
        private SAPbouiCOM.Button btnSelecionar;
        private SAPbouiCOM.Button btnCancelar;
        private SAPbouiCOM.DataTable dtConsulta = null;
        private string _userTable;
        private string query;
        private string[,] filter = null;

        private void Form_LoadAfter()//(SAPbouiCOM.SBOItemEventArg pVal)
        {
            this._userTable = Tools.TableLookUp;
            this.SetQuery(true);
            this.MontaMatrizConsulta();
        }
    }
}
