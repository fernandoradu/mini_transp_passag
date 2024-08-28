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
            //this.UIAPIRawForm.SupportedModes = (int)SAPbouiCOM.BoAutoFormMode.afm_All;
            //FormLocalidade.SetOperation();
        }
        private void btnConfirm_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            string errorMessage = "";
            if (string.IsNullOrEmpty(txtLocali.String))
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
            Localidade localidade = new Localidade();
            SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;
            
            if (form.UniqueID == "FRMLocal" && pVal.ItemUID == "1")
            {
                //Recupera as informações do formulário para o objeto de repositório da classe Localidade
                localidade.FormToRepository(form);
                //Manipula os dados recuperados, persistindo na base de dados, via DI-Api
                localidade.SetFormMode(form);

                if (localidade.HasError)
                    Application.SBO_Application.SetStatusBarMessage(localidade.ErrorMessage, SAPbouiCOM.BoMessageTime.bmt_Short);
                else   //Se for inclusão de registro, roda novamente a query do objeto RecordSet{
                {
                    Tools.UserTabNavigator.Setup();

                    if (Tools.UserTabNavigator.RunningOperation == 1)       //Inclusão
                        Tools.UserTabNavigator.LastRecord();
                    else if (Tools.UserTabNavigator.RunningOperation == 2)  //Update
                        Tools.UserTabNavigator.SeekRecord(new object[] { "Code", localidade.Code});

                    Application.SBO_Application.StatusBar.SetText(localidade.OKMessage, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);                
                }

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