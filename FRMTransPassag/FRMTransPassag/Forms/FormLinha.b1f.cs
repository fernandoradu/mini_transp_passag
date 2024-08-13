using FRMTransPassag.Framework.Classes;
using FRMTransPassag.Repositories;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRMTransPassag.Forms
{
    [FormAttribute("FRMTransPassag.Forms.FormLinha", "Forms/FormLinha.b1f")]
    public class FormLinha : UserFormBase
    {
        public FormLinha()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            SAPbouiCOM.EditText txtLocOrig = null;
            SAPbouiCOM.EditText txtLocDest = null;

            this.lblLinha = ((SAPbouiCOM.StaticText)(this.GetItem("lblLinha").Specific));
            this.txtLinha = ((SAPbouiCOM.EditText)(this.GetItem("txtLinha").Specific));
            this.lblNLinha = ((SAPbouiCOM.StaticText)(this.GetItem("lblNLinha").Specific));
            this.txtNLinha = ((SAPbouiCOM.EditText)(this.GetItem("txtNLinha").Specific));
            this.btnCancelar = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.btnConfirmar = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.btnConfirmar.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.btnConfirmar_PressedAfter);
            
            this.mtxSecoes = ((SAPbouiCOM.Matrix)(this.GetItem("mtxSecoes").Specific));
            this.mtxSecoes.KeyDownBefore += new SAPbouiCOM._IMatrixEvents_KeyDownBeforeEventHandler(this.mtxSecoes_KeyDownBefore);
            this.mtxSecoes.KeyDownAfter += new SAPbouiCOM._IMatrixEvents_KeyDownAfterEventHandler(this.mtxSecoes_KeyDownAfter);
            this.mtxSecoes.DoubleClickAfter += new SAPbouiCOM._IMatrixEvents_DoubleClickAfterEventHandler(this.Local_DoubleClickAfter);

            this.OnCustomInitialize();

        }

        private void MtxSecoes_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }
        private void OnCustomInitialize()
        {
            this.UIAPIRawForm.EnableMenu("1282", true); //Adicionar novo registro
            this.UIAPIRawForm.EnableMenu("1283", true); //Remover novo registro
            this.UIAPIRawForm.EnableMenu("1290", true); //Seta para ir ao primeiro registro
            this.UIAPIRawForm.EnableMenu("1288", true); //Seta para ir ao próximo registro
            this.UIAPIRawForm.EnableMenu("1289", true); //Seta para ir ao registro anterior
            this.UIAPIRawForm.EnableMenu("1291", true); //Seta para ir ao último registro
            this.UIAPIRawForm.EnableMenu("1292", true); //Inserir Linha
            this.LoadMatrixSecao();
            //this.UIAPIRawForm.EnableMenu("1292", true); //Adicionar linha (Matrix ou Grid)

        }
        public void LoadMatrixSecao(string idLinha = null)
        {
            if ( this.query == "" )
            {
                StringBuilder buildQuery = new StringBuilder();

                buildQuery.Append("SELECT");
                buildQuery.Append(" T0.\"Code\",");
                buildQuery.Append(" T0.\"U_CodeLinha\",");
                buildQuery.Append(" T0.\"U_LocalPartida\",");
                buildQuery.Append(" T0.\"U_LocalChegada\",");
                buildQuery.Append(" T2.\"Name\" \"NomeLocalOrig\",");
                buildQuery.Append(" T3.\"Name\" \"NomeLocalDest\" ");
                buildQuery.Append("FROM");
                buildQuery.Append(" \"@TB_SECLINHA\" T0 ");
                buildQuery.Append("INNER JOIN");
                buildQuery.Append(" \"@TB_LINHA\" T1 ");
                buildQuery.Append("ON");
                buildQuery.Append(" T1.\"Code\" = T0.\"U_CodeLinha\" ");
                buildQuery.Append("INNER JOIN");
                buildQuery.Append(" \"@TB_LOCALIDADE\" T2 ");
                buildQuery.Append("ON");
                buildQuery.Append(" T2.\"Code\" = T0.\"U_LocalPartida\" ");
                buildQuery.Append("INNER JOIN");
                buildQuery.Append(" \"@TB_LOCALIDADE\" T3 ");
                buildQuery.Append("ON");
                buildQuery.Append(" T3.\"Code\" = T0.\"U_LocalChegada\" ");
                buildQuery.Append("WHERE");
                
                if (idLinha != null)
                    buildQuery.AppendFormat("   T0.\"U_CodeLinha\" = '{0}'", idLinha);
                else  // query que não trará resultado
                    buildQuery.AppendFormat("   T0.\"U_CodeLinha\" = '{0}'", "(ZZZZ)" + idLinha + "(ZZZZ)");

                query = buildQuery.ToString();

                this.UIAPIRawForm.DataSources.DataTables.Item("dtSecoes").ExecuteQuery(this.query);

                mtxSecoes.Columns.Item("cCode").DataBind.Bind("dtSecoes", "Code");
                mtxSecoes.Columns.Item("cLocOrig").DataBind.Bind("dtSecoes", "U_LocalPartida");
                mtxSecoes.Columns.Item("cNLocOri").DataBind.Bind("dtSecoes", "U_LocalChegada");
                mtxSecoes.Columns.Item("cLocDest").DataBind.Bind("dtSecoes", "NomeLocalOrig");
                mtxSecoes.Columns.Item("cNLocDes").DataBind.Bind("dtSecoes", "NomeLocalDest");

                this.UIAPIRawForm.Freeze(true);

                mtxSecoes.LoadFromDataSource();

                this.UIAPIRawForm.Freeze(false);
            }
            //Necessário criar uma linha para poder editar os dados.
            this.AddSecao();
        }
        public static int SetOperation()
        {
            int operation = 0;
            if (Tools.UserTabNavigator == null)
            {
                string[] fields =
                {
                    "Code",
                    "Name"
                };

                Tools.SetUserTableNavigator("@TB_LINHA");
                Tools.UserTabNavigator.QueryToRecord(ref fields,fields);
            }
            operation = Tools.UserTabNavigator.RunningOperation == null ? 0 : Tools.UserTabNavigator.RunningOperation;

            return operation;
        }
        public void AddSecao()
        {
            SAPbouiCOM.EditText cellCode = null; 
            string code = "000001";
            int lastRow = 0;
            int currentRow = 0;
            string lastCode = "";

            lastRow = mtxSecoes.RowCount;
            
            if (lastRow > 0)
            {
                currentRow = mtxSecoes.GetCellFocus().rowIndex;
                cellCode = (SAPbouiCOM.EditText)mtxSecoes.Columns.Item("cCode").Cells.Item(lastRow).Specific;
                
                lastCode = cellCode.Value;            
            }

            if ( currentRow == lastRow)
            {
                if (lastCode != "")
                    code = Tools.StringNext(lastCode);
                
                this.mtxSecoes.AddRow();

                lastRow = mtxSecoes.RowCount;
                ((SAPbouiCOM.EditText)this.mtxSecoes.Columns.Item("cCode").Cells.Item(lastRow).Specific).Value = code;
            }            

            return;
        }
        private void btnConfirmar_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            int operation = Tools.UserTabNavigator != null ? Tools.UserTabNavigator.RunningOperation : FormLinha.SetOperation();

            Linha linha = new Linha();
            SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;

            if (form.UniqueID == "FRMLinha")
            {
                linha.FormToRepository(form);
                linha.ManipulateData(operation);
            }
        }
               
        private void Local_DoubleClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.ColUID == "cLocOrig" || pVal.ColUID == "cLocDest")
            {
                SAPbouiCOM.CellPosition cell = mtxSecoes.GetCellFocus();
                string colCode = this.mtxSecoes.Columns.Item(cell.ColumnIndex).UniqueID;
                string colName = colCode == "cLocOrig"? "cNLocOri" : "cNLocDes";

                Tools.ConsultaRegistro(this,colCode,colName);
            }         

        }
        
        private void mtxSecoes_KeyDownBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.CharPressed == 40) //40 é igual a seta para baixo. 38 é seta para cima
                this.AddSecao();
        }
        private void mtxSecoes_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

        }
        private SAPbouiCOM.StaticText lblLinha;
        private SAPbouiCOM.EditText txtLinha;
        private SAPbouiCOM.StaticText lblNLinha;
        private SAPbouiCOM.EditText txtNLinha;
        private SAPbouiCOM.Matrix mtxSecoes;
        private SAPbouiCOM.Button btnCancelar;
        private SAPbouiCOM.Button btnConfirmar;        
        private string query = "";

        public SAPbouiCOM.Matrix MatrizSecoes { get { return mtxSecoes; } }
    }
}
