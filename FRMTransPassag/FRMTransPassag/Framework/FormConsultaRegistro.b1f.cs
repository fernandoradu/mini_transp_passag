﻿using FRMTransPassag.Forms;
using FRMTransPassag.Framework.Classes;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRMTransPassag.Framework
{
    [FormAttribute("FRMTransPassag.Framework.FormConsultaRegistro", "Framework/FormConsultaRegistro.b1f")]
    public class FormConsultaRegistro : UserFormBase
    {
        public FormConsultaRegistro(FormLinha form, string colCode, string colName)
        {
            this.formCaller = form;
            this.columnCode = colCode;
            this.columnName = colName;
        }
        //ToDo: Implementar o construtor para cada Formulário que efetua a chamada de consulta
        //---------------- FIM ----------------
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            
            this.lblProcurar = ((SAPbouiCOM.StaticText)(this.GetItem("lblPesq").Specific));
            this.txtProcurar = ((SAPbouiCOM.EditText)(this.GetItem("Item_1").Specific));
            this.mtxConsulta = ((SAPbouiCOM.Matrix)(this.GetItem("mtxCons1").Specific));
            this.btnSelecionar = ((SAPbouiCOM.Button)(this.GetItem("btnSelec").Specific));
            this.btnCancelar = ((SAPbouiCOM.Button)(this.GetItem("btnCanc1").Specific));
            
            this.OnCustomInitialize();
            this.SetItemsEvents();

        }
        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        
        }
        //Este método é essencial para inicialização dos componentes (ou itens de form)
        //que não foram, em especial, criados pela interface de design de tela.
        private void OnCustomInitialize()
        {
            this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE;
            this.mtxConsulta.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single;
            
            this.mtxConsulta.Columns.Item(1).Editable = true;
            this.mtxConsulta.Columns.Item(2).Editable = false;
            this.Form_LoadAfter();
        }
        private void SetItemsEvents()
        {
            this.btnSelecionar.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btnConfirmar_PressedBefore);
            this.txtProcurar.LostFocusAfter += new SAPbouiCOM._IEditTextEvents_LostFocusAfterEventHandler(this.txtProcurar_SeekReg);
            this.mtxConsulta.GotFocusAfter += new SAPbouiCOM._IMatrixEvents_GotFocusAfterEventHandler(this.mtxConsulta_GotFocusAfter);
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
                //Execução da query de consulta
                this.UIAPIRawForm.DataSources.DataTables.Item("DTCon1").ExecuteQuery(this.query);
                //Faz o vínculo do datatable com a matriz
                mtxConsulta.Columns.Item("cCodCon").DataBind.Bind("DTCon1", "Code");
                mtxConsulta.Columns.Item("cNameCon").DataBind.Bind("DTCon1", "Name");
                //Tamanho das colunas criadas de forma automática
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
        private void Form_LoadAfter()//(SAPbouiCOM.SBOItemEventArg pVal)
        {
            this._userTable = Tools.TableLookUp;
            this.SetQuery(true);
            this.MontaMatrizConsulta();
        }

        private void btnConfirmar_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            
            if (this.rowCons > 0)
            {
                SAPbouiCOM.Matrix mtxSecoes = this.formCaller.MatrizSecoes;
                this.retConsReg = ((SAPbouiCOM.EditText)mtxConsulta.Columns.Item("cCodCon").Cells.Item(this.rowCons).Specific).String;
                this.retDescription = ((SAPbouiCOM.EditText)mtxConsulta.Columns.Item("cNameCon").Cells.Item(this.rowCons).Specific).String;

                ((SAPbouiCOM.EditText)mtxSecoes.Columns.Item(this.columnCode).Cells.Item(mtxSecoes.GetCellFocus().rowIndex).Specific).Value = this.retConsReg.ToString();
                ((SAPbouiCOM.EditText)mtxSecoes.Columns.Item(this.columnName).Cells.Item(mtxSecoes.GetCellFocus().rowIndex).Specific).Value = this.retDescription.ToString();
            }
            
            Application.SBO_Application.Forms.ActiveForm.Close();
        }     
        private void txtProcurar_SeekReg(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            int row = 0;

            SAPbouiCOM.EditText cellCode = null;

            this.UIAPIRawForm.Freeze(true);
            
            for (int i = 1; i <= mtxConsulta.VisualRowCount; i++)
            {
                cellCode = (SAPbouiCOM.EditText)mtxConsulta.Columns.Item("cCodCon").Cells.Item(i).Specific;

                if ( cellCode.String.Contains(txtProcurar.String))
                {
                    row = i;
                    break;
                }
            }

            this.rowCons = row;
            this.UIAPIRawForm.Freeze(false);
            //this.mtxConsulta.SelectRow(row, true, false);            
        }
        public void mtxConsulta_GotFocusAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.ItemUID == "mtxCons1" && pVal.Row > 0)
            {
                if ( !string.IsNullOrEmpty(this.txtProcurar.String) && this.rowCons > 0 && this.rowCons != pVal.Row)
                {
                    mtxConsulta.SetCellFocus(this.rowCons, 1);
                }
                else if(string.IsNullOrEmpty(this.txtProcurar.String) && this.rowCons != pVal.Row)
                {
                    this.rowCons = pVal.Row;
                }
                
                int realCurrentRow = mtxConsulta.GetCellFocus().rowIndex;
                mtxConsulta.SelectRow(realCurrentRow, true, false);
            }
        }
        private int rowCons = 0;
        private string _userTable;
        private string query;
        private string[,] filter = null;
        private string columnCode = "";
        private string columnName = "";
        private SAPbouiCOM.StaticText lblProcurar;
        private SAPbouiCOM.EditText txtProcurar;
        private SAPbouiCOM.Matrix mtxConsulta;
        private SAPbouiCOM.Button btnSelecionar;
        private SAPbouiCOM.Button btnCancelar;
        private SAPbouiCOM.DataTable dtConsulta = null;
        private dynamic formCaller = null;
        private object retConsReg = null;
        private object retDescription = null;
    }
}
