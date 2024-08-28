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
            this.lblLinha = ((SAPbouiCOM.StaticText)(this.GetItem("lblLinha").Specific));
            this.txtLinha = ((SAPbouiCOM.EditText)(this.GetItem("txtLinha").Specific));
            this.lblNLinha = ((SAPbouiCOM.StaticText)(this.GetItem("lblNLinha").Specific));
            this.txtNLinha = ((SAPbouiCOM.EditText)(this.GetItem("txtNLinha").Specific));
            this.btnCancelar = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.btnConfirmar = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.mtxSecoes = ((SAPbouiCOM.Matrix)(this.GetItem("mtxSecoes").Specific));
            this.SetItemsEvents();
            this.OnCustomInitialize();
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
        private void SetItemsEvents()
        {            
            this.btnConfirmar.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.btnConfirmar_PressedAfter);
            this.mtxSecoes.KeyDownBefore += new SAPbouiCOM._IMatrixEvents_KeyDownBeforeEventHandler(this.mtxSecoes_KeyDownBefore);
            this.mtxSecoes.DoubleClickAfter += new SAPbouiCOM._IMatrixEvents_DoubleClickAfterEventHandler(this.Local_DoubleClickAfter);
            this.mtxSecoes.ValidateBefore += new SAPbouiCOM._IMatrixEvents_ValidateBeforeEventHandler(this.mtxSecoes_ValidateBefore);
            this.mtxSecoes.LostFocusAfter += new SAPbouiCOM._IMatrixEvents_LostFocusAfterEventHandler(this.mtxSecoes_LostFocusAfter);
            this.btnConfirmar.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btnConfirmar_PressedBefore);
            this.btnConfirmar.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btnConfirmar_PressedBefore);
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
            this.mtxSecoes.Columns.Item("cNLocOri").Editable = false;
            this.mtxSecoes.Columns.Item("cNLocDes").Editable = false;
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

                if ( currentRow == lastRow)
                {
                    if (lastCode != "")
                        code = Tools.StringNext(lastCode);

                    if (currentRow > 0)
                    {
                        this.mtxSecoes.AddRow();
                        this.mtxSecoes.SetCellFocus(lastRow, 1);                    
                    }                

                    lastRow = mtxSecoes.RowCount;            
                    ((SAPbouiCOM.EditText)this.mtxSecoes.Columns.Item("cCode").Cells.Item(lastRow).Specific).Value = code;
                }            
            }
            else
            {
                mtxSecoes.AddRow();
                lastRow = 1;                
                ((SAPbouiCOM.EditText)this.mtxSecoes.Columns.Item("cCode").Cells.Item(lastRow).Specific).Value = code;
            }

            return;
        }
        private void btnConfirmar_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            string errorMessage = "";
            BubbleEvent = true;

            if (string.IsNullOrEmpty(this.txtLinha.String) || string.IsNullOrEmpty(this.txtNLinha.String))
            {
                BubbleEvent = false;
                errorMessage = "Identificador [Id. Linha] e/ou nome da linha não foi(ram) preenchido(s).";
            }
            else
            {
                if (this.mtxSecoes.RowCount == 0)
                {
                    BubbleEvent = false;
                    errorMessage = "As seções da linha não foram informadas.";
                }
                else
                {
                    SAPbouiCOM.EditText cellLocOrig = null;
                    SAPbouiCOM.EditText cellLocDest = null;
                    for (int ind = 1; ind <= this.mtxSecoes.RowCount; ind++)
                    {
                        cellLocOrig = (SAPbouiCOM.EditText)this.mtxSecoes.Columns.Item("cLocOrig").Cells.Item(ind).Specific;
                        cellLocDest = (SAPbouiCOM.EditText)this.mtxSecoes.Columns.Item("cLocDest").Cells.Item(ind).Specific;

                        if (string.IsNullOrEmpty(cellLocOrig.String) || string.IsNullOrEmpty(cellLocDest.String))
                        {
                            BubbleEvent = false;
                            errorMessage = "Localidade de origem ou destino não foi(ram) preenchida(s)";
                            break;
                        }
                    }

                }
            }
            if (!BubbleEvent)
            {
                Application.SBO_Application.SetStatusBarMessage(errorMessage, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }
        private void btnConfirmar_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            int operation = Tools.UserTabNavigator != null ? Tools.UserTabNavigator.RunningOperation : FormLinha.SetOperation();

            Linha linha = new Linha();
            SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;

            if (form.UniqueID == "FRMLinha" && pVal.ItemUID == "1")
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
        private void mtxSecoes_ValidateBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            string message = "";
            BubbleEvent = true;

            if (pVal.ColUID.Contains("cLoc") && pVal.ItemChanged)
            {
                //A partir da segunda linha:
                //Checar se a localidade inicial é a mesma que a final da linha anterior
                if (pVal.Row > 1)
                {
                    if (pVal.ColUID == "cLocOrig")                    {

                        SAPbouiCOM.EditText localOrigem = (SAPbouiCOM.EditText)mtxSecoes.Columns.Item(pVal.ColUID).Cells.Item(pVal.Row).Specific;
                        SAPbouiCOM.EditText localDestino = (SAPbouiCOM.EditText)mtxSecoes.Columns.Item("cLocDest").Cells.Item(pVal.Row - 1).Specific;

                        if (localOrigem.String != localDestino.String)
                        {
                            BubbleEvent = false;
                            message = "Sequência de localidades incorreta. ";
                            message += "Necessário que o Local de Origem seja o mesmo que o Local de Destino da linha anterior.";
                            message += " [Local Origem: " + localOrigem.String + " - Local Destino: " + localDestino.String + "] ";
                        }
                    }
                }
            }

            if (!BubbleEvent)
                Application.SBO_Application.SetStatusBarMessage(message, SAPbouiCOM.BoMessageTime.bmt_Short);
        }
        private void mtxSecoes_LostFocusAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            SAPbouiCOM.EditText cellLocalidade = (SAPbouiCOM.EditText)mtxSecoes.GetCellSpecific(pVal.ColUID, pVal.Row);
            
            if (pVal.ColUID.Contains("cLoc") &&  !string.IsNullOrEmpty(cellLocalidade.String))
            {
                Localidade local = new Localidade();
                string colName = pVal.ColUID == "cLocOrig" ? "cNLocOri" : "cNLocDes";
                string[,] seek = new string[1, 2];
                
                seek[0, 0] = "Code";
                seek[0, 1] = "'" + cellLocalidade.String + "'";
                
                if (local.SeekLocalidade(seek))
                    ((SAPbouiCOM.EditText)mtxSecoes.Columns.Item(colName).Cells.Item(pVal.Row).Specific).Value = local.Name;
            }
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
