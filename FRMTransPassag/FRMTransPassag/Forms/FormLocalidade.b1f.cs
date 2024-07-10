using FRMTransPassag.Framework.Classes;
using FRMTransPassag.Repositories;
using SAPbobsCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace FRMTransPassag.Forms
{
    [FormAttribute("FRMTransPassag.Forms.FormLocalidade", "Forms/FormLocalidade.b1f")]
    class FormLocalidade : UserFormBase
    {
        public FormLocalidade()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblLocali = ((SAPbouiCOM.StaticText)(this.GetItem("lblLocali").Specific));
            this.lblNome = ((SAPbouiCOM.StaticText)(this.GetItem("lblNome").Specific));
            this.txtLocali = ((SAPbouiCOM.EditText)(this.GetItem("txtLocali").Specific));
            this.txtNome = ((SAPbouiCOM.EditText)(this.GetItem("txtNome").Specific));
            this.btnConfirm = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.btnConfirm.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.btnConfirm_PressedAfter);
            this.btnConfirm.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btnConfirm_PressedBefore);
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
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
            //FormLocalidade.SetOperation();
        }
        private void btnConfirm_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            
            string errorMessage = "";
            if ( string.IsNullOrEmpty(txtLocali.String))
            {
                BubbleEvent = false;
                errorMessage = "O campo Id. Localidade não foi preenchido";
            }

            if (!BubbleEvent)
            {
                Application.SBO_Application.SetStatusBarMessage(errorMessage, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }

        }
        private void btnConfirm_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            int operation = Tools.UserTabNavigator != null ? Tools.UserTabNavigator.RunningOperation : FormLocalidade.SetOperation();

            Localidade localidade = new Localidade();
            SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;

            if (Application.SBO_Application.Forms.ActiveForm.UniqueID == "FRMLocal")
            {
                localidade.FormToRepository(form);
                localidade.ManipulateData(operation);
            }
            //FormLocalidade.HandlingRegister(Tools.Company, (UserTable)Tools.Company.UserTables.Item("TB_LOCALIDADE"),
            //    (Recordset)Tools.Company.GetBusinessObject(BoObjectTypes.BoRecordset), FormLocalidade.SetOperation(),
            //    txtLocali.String, txtNome.String);

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

                Tools.SetUserTableNavigator("@TB_LOCALIDADE");
                Tools.UserTabNavigator.QueryToRecord(fields);
            }
            operation = Tools.UserTabNavigator.RunningOperation == null ? 0 : Tools.UserTabNavigator.RunningOperation;

            return operation;
        }
       
        public static void HandlingRegister(Company oCompany, UserTable tabLocalidade, Recordset oResultSet,
            int operation, string codeLocalidade, string nomeLocalidade)
        {
            string CardCode = "";
            string msgError = "";
            
            bool ret = true;
            
            string msgSuccess = "";
            
            string[,] seek = {
                { "Code","'" + codeLocalidade + "'" }//txtLocali.String
            };

            //tabLocalidade = (UserTable)oCompany.UserTables.Item("TB_LOCALIDADE");
            //oResultSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            SetOperation();

            Tools.ExistReg(oResultSet, "@TB_LOCALIDADE", "Local", seek, "Code", out CardCode, out msgError);
            //operation 0-View, 1-Insert, 2-Update, 3-Delete
            if (operation == 2 || operation == 3 && !string.IsNullOrEmpty(CardCode))  //if (this.operation == 2 || this.operation == 3 && !string.IsNullOrEmpty(CardCode))
            {
                tabLocalidade.GetByKey(CardCode);
                msgSuccess = operation == 2 ? "Registro atualizado com sucesso!" : "Registro excluído com sucesso!";   //this.operation
                msgError = operation == 2 ? "Registro não foi atualizado!" : "Registro não foi excluído!"; //this.operation
            }
            else
            {
                tabLocalidade.Code = codeLocalidade;    //txtLocali.String;
                msgSuccess = "Registro incluído com sucesso!";
                msgError = "Registro não foi incluído!";
            }

            tabLocalidade.Name = nomeLocalidade;    //txtNome.String;

            if (operation > 0) //se não for visualização //this.operation
            {
                if (operation == 1)    //inclusão  //this.operation
                    ret = tabLocalidade.Add() == 0;
                else if (operation == 2)   //atualização   //this.operation
                    ret = tabLocalidade.Update() == 0;
                else if (operation == 3)   //exclusão  //this.operation
                    ret = tabLocalidade.Remove() == 0;

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
        }
        private SAPbouiCOM.StaticText lblLocali;
        private SAPbouiCOM.StaticText lblNome;
        private SAPbouiCOM.EditText txtLocali;
        private SAPbouiCOM.EditText txtNome;
        private SAPbouiCOM.Button btnConfirm;  
        private SAPbouiCOM.Button btnCancel;
        private Recordset oResultSet;
        private SAPbouiCOM.DBDataSource dBDataSource;
        private UserTable tabLocalidade;
        private int operation;
        public string CardCode;

        //Company oCompany = (Company)Application.SBO_Application.Company.GetDICompany();

    }
}