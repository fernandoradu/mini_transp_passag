using FRMTransportePassageiros.Framework.Classes;
using FRMTransportePassageiros.Repositories;
using SAPbobsCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRMTransportePassageiros.Foms
{
    [FormAttribute("FRMTransportePassageiros.Foms.FormLinhas", "Foms/FormLinhas.b1f")]
    class FormLinhas : FRMForm
    {
        public FormLinhas()
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
            this.mtxSecoes = ((SAPbouiCOM.Matrix)(this.GetItem("mtxSecoes").Specific));
            this.btnConfirmar = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.btnConfirmar.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.btnConfirmar_PressedAfter);
            this.btnConfirmar.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btnConfirmar_PressedBefore);
            this.btnCancelar = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            //this.DataLoadAfter += new DataLoadAfterHandler(this.Form_DataLoadAfter);

        }

        private void CreateChooseFromList()
        {
            SAPbouiCOM.Form oForm = Application.SBO_Application.Forms.ActiveForm;

            // Criar a ChooseFromList
            SAPbouiCOM.ChooseFromListCollection oCFLs = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);
            oCFLCreationParams.MultiSelection = false;
            oCFLCreationParams.ObjectType = "TB_LOCALIDADES"; // Nome da tabela de usuário
            oCFLCreationParams.UniqueID = "cflLocal";

            SAPbouiCOM.ChooseFromList oCFL = oCFLs.Add(oCFLCreationParams);

            // Associar a ChooseFromList ao campo no formulário
            SAPbouiCOM.Matrix matrix = (SAPbouiCOM.Matrix)oForm.Items.Item("mtxSecoes").Specific;
            SAPbouiCOM.EditText oEditText = (SAPbouiCOM.EditText)oForm.Items.Item("cLocOrig").Specific;
            oEditText.ChooseFromListUID = "cflLocal";
            oEditText.ChooseFromListAlias = "Code"; // Nome da coluna na tabela de usuário

            // Configurar os eventos da ChooseFromList
            //oEditText.ChooseFromListAfter += new SAPbouiCOM._IEditTextEvents_ChooseFromListAfterEventHandler(this.event_ChooseFromListAfter);
        }

        private void event_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.ActionSuccess)
            {
                SAPbouiCOM.Form oForm = Application.SBO_Application.Forms.Item(pVal.FormUID);
                SAPbouiCOM.IChooseFromListEvent oCFL = (SAPbouiCOM.IChooseFromListEvent)pVal;
                SAPbouiCOM.ChooseFromList oCFLObj = oForm.ChooseFromLists.Item(oCFL.ChooseFromListUID);

                if (oCFL.SelectedObjects != null)
                {
                    SAPbouiCOM.DataTable oDataTable = oCFL.SelectedObjects;
                    string selectedValue = oDataTable.GetValue("Code", 0).ToString();
                    SAPbouiCOM.Matrix matrix = (SAPbouiCOM.Matrix)oForm.Items.Item("mtxSecoes").Specific;
                    SAPbouiCOM.EditText oEditText = (SAPbouiCOM.EditText)oForm.Items.Item("cLocOrig").Specific;
                    oEditText.Value = selectedValue;
                }
            }
        }
        private void OnCustomInitialize()
        {

            this.UIAPIRawForm.EnableMenu("1282", true); //Adicionar novo registro
            this.UIAPIRawForm.EnableMenu("1283", true); //Remover novo registro
            this.UIAPIRawForm.EnableMenu("1290", true); //Seta para ir ao primeiro registro
            this.UIAPIRawForm.EnableMenu("1288", true); //Seta para ir ao próximo registro
            this.UIAPIRawForm.EnableMenu("1289", true); //Seta para ir ao registro anterior
            this.UIAPIRawForm.EnableMenu("1291", true); //Seta para ir ao último registro
        }

        public void LoadMatrixSecao(string idLinha)
        {
            if (query == "")
            {
                StringBuilder buildQuery = new StringBuilder();

                buildQuery.Append("SELECT");
                buildQuery.Append("T0.\"Code\",");
                buildQuery.Append("\"U_CodeLinha\",");
                buildQuery.Append("\"U_LocalPartida\",");
                buildQuery.Append("\"U_LocalChegada\",");
                buildQuery.Append("T2.\"Name\" \"NomeLocalOrig\",");
                buildQuery.Append("T3.\"Name\" \"NomeLocalDest\"");
                buildQuery.Append("FROM");
                buildQuery.Append("\"@TB_SECLINHA\" T0");
                buildQuery.Append("INNER JOIN");
                buildQuery.Append("\"@TB_LINHAS\" T1");
                buildQuery.Append("ON");
                buildQuery.Append("T1.\"Code\" = T0.\"U_CodeLinha\"");
                buildQuery.Append("INNER JOIN");
                buildQuery.Append("\"@TB_LOCALIDADE\" T2");
                buildQuery.Append("ON");
                buildQuery.Append("T2.\"Code\" = T0.\"U_LocalPartida\"");
                buildQuery.Append("INNER JOIN");
                buildQuery.Append("\"@TB_LOCALIDADE\" T3");
                buildQuery.Append("ON");
                buildQuery.Append("T3.\"Code\" = T0.\"U_LocalChegada\"");
                buildQuery.Append("WHERE");
                buildQuery.AppendFormat("T0.\"U_CodeLinha\" = '{0}'", idLinha);

                query = buildQuery.ToString();

                this.UIAPIRawForm.DataSources.DataTables.Item("dtSecoes").ExecuteQuery(this.query);

                mtxSecoes.Columns.Item("cCode").DataBind.Bind("dtSecoes", "Code");
                mtxSecoes.Columns.Item("cLocOrig").DataBind.Bind("dtSecoes", "IdLocalOrig");
                mtxSecoes.Columns.Item("cNLocOri").DataBind.Bind("dtSecoes", "NomeLocalOrig");
                mtxSecoes.Columns.Item("cLocDest").DataBind.Bind("dtSecoes", "IdLocalDest");
                mtxSecoes.Columns.Item("cNLocDes").DataBind.Bind("dtSecoes", "NomeLocalDest");
                
                this.UIAPIRawForm.Freeze(true);

                mtxSecoes.LoadFromDataSource();
                
                this.UIAPIRawForm.Freeze(false);
            }

        }
        private void Form_DataLoadAfter(ref SAPbouiCOM.BusinessObjectInfo pVal)
        {
            this.LoadMatrixSecao(txtLinha.String);
        }

        private void btnConfirmar_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            throw new System.NotImplementedException();
        }

        private void btnConfirmar_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            int operation = Tools.UserTabNavigator != null ? Tools.UserTabNavigator.RunningOperation : FormLinhas.SetOperation();

            Linha linha = new Linha();
            SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;

            if (form.UniqueID == "FRMLinha")
            {
                linha.SetDataToForm(form);
                linha.ManipulateData(operation);
            }

        }
        private SAPbouiCOM.EditText txtLinha;
        private SAPbouiCOM.StaticText lblNLinha;
        private SAPbouiCOM.EditText txtNLinha;
        private SAPbouiCOM.Matrix mtxSecoes;
        private SAPbouiCOM.Button btnConfirmar;
        private SAPbouiCOM.Button btnCancelar;
        private SAPbouiCOM.StaticText lblLinha;
        private string query = "";

        

        /*public static void HandlingRegister(Linha linha, int operation = 0)
        {
            string CardCode = "";
            string msgError = "";

            bool ret = true;

            string msgSuccess = "";

            string[,] seek = {
                { "Code","'" + linha.Code + "'" }//txtLocali.String
            };

            //tabLocalidade = (UserTable)oCompany.UserTables.Item("TB_LOCALIDADE");
            //oResultSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            if (operation == 0)
                operation = SetOperation();

            Tools.ExistReg((Recordset)Tools.Company.GetBusinessObject(BoObjectTypes.BoRecordset), "@TB_LINHAS", "Linhas", seek, "Code", out CardCode, out msgError);
            //operation 0-View, 1-Insert, 2-Update, 3-Delete
            if (operation == 2 || operation == 3 && !string.IsNullOrEmpty(CardCode))  //if (this.operation == 2 || this.operation == 3 && !string.IsNullOrEmpty(CardCode))
            {
                //TODO: Ajustar o fonte para utilizar tanto o objeto de "tabela de usuário" de @TB_LINHAS quanto @TB_SECLINHA
                //tabLocalidade.GetByKey(CardCode);
                msgSuccess = operation == 2 ? "Registro atualizado com sucesso!" : "Registro excluído com sucesso!";   //this.operation
                msgError = operation == 2 ? "Registro não foi atualizado!" : "Registro não foi excluído!"; //this.operation
            }
            else
            {
                //tabLocalidade.Code = codeLocalidade;    //txtLocali.String;
                msgSuccess = "Registro incluído com sucesso!";
                msgError = "Registro não foi incluído!";
            }

            //tabLocalidade.Name = nomeLocalidade;    //txtNome.String;

            if (operation > 0) //se não for visualização //this.operation
            {
                if (operation == 1)    //inclusão  //this.operation
                    ret = false;    //tabLocalidade.Add() == 0;
                else if (operation == 2)   //atualização   //this.operation
                    ret = false;    //tabLocalidade.Update() == 0;
                else if (operation == 3)   //exclusão  //this.operation
                    ret = false;    //tabLocalidade.Remove() == 0;

                if (ret)
                {
                    //Tools.SetSBOFormActive(Application.SBO_Application.Forms.ActiveForm);
                    if (operation == 1)    //this.operation
                    {
                        Tools.SetUserTableNavigator("@TB_LOCALIDADE");
                        Tools.UserTabNavigator.Setup();
                    }
                    else if (operation == 3)   //this.operation
                    {
                        SAPbouiCOM.DBDataSource dBDataSource = Application.SBO_Application.Forms.ActiveForm.DataSources.DBDataSources.Item("@TB_LOCALIDADE");
                        dBDataSource.SetValue("Code", 0, "");
                        dBDataSource.SetValue("Name", 0, "");
                    }

                    CardCode = Tools.Company.GetNewObjectKey(); //oCompany.GetNewObjectKey();
                    Application.SBO_Application.SetStatusBarMessage(msgSuccess, SAPbouiCOM.BoMessageTime.bmt_Short, false);
                }
                else
                    Application.SBO_Application.SetStatusBarMessage(msgError + " - " + Tools.Company.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short, true);   //oCompany.GetLastErrorDescription()
            }
        }*/
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

                Tools.SetUserTableNavigator("@TB_LINHAS");
                Tools.UserTabNavigator.QueryToRecord(fields);
            }
            operation = Tools.UserTabNavigator.RunningOperation == null ? 0 : Tools.UserTabNavigator.RunningOperation;

            return operation;
        }
    }
}
