using FRMTransPassag.Forms;
using FRMTransPassag.Framework;
using FRMTransPassag.Framework.Classes;
using FRMTransPassag.Repositories;
using SAPbobsCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FRMTransPassag
{
    class Menu
    {
        public Menu()
        {            
        }
        public void AddMenuItems()
        {
            String xmlMenu;
            try
            {
                RemoveMenu();

                xmlMenu = Resources.Menu.menuadd.ToString().Replace("%path%", Environment.CurrentDirectory);
                Application.SBO_Application.LoadBatchActions(ref xmlMenu);
            }
            catch
            {
                throw;
            }
        }
        public void RemoveMenu()
        {
            String xmlMenu;

            try
            {
                xmlMenu = Resources.Menu.menuremove.ToString().Replace("%path%", Environment.CurrentDirectory);
                Application.SBO_Application.LoadBatchActions(ref xmlMenu);
            }
            catch
            {
                throw;
            }
        }
        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            Tools.SetUICompany();

            if ( pVal.MenuUID.Contains("mnu") && pVal.BeforeAction)
            {
                ChooseForm(pVal.MenuUID);
            }
            else
            {
                //avalia se o formulário ativo pertence a lista de formulários do menu do Addon de Transporte de Passageiros
                if (pVal.BeforeAction &&
                    Array.Exists(Tools.ListFormsFRM, element => element == Application.SBO_Application.Forms.ActiveForm.UniqueID))
                {
                    HandleNavBar(pVal.MenuUID);
                }
            }
        }

        private void ChooseForm(string menuId)
        {
            string[] fields =
            {
                "Code",
                "Name"
            };

            switch (menuId)
            {
                case "mnu_Locali":
            
                    Tools.SetUserTableNavigator("@TB_LOCALIDADE");
                    Tools.UserTabNavigator.QueryToRecord(ref fields,fields);

                    _formLocalidade = new FormLocalidade();
                    //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                    _formLocalidade.Show();

                    break;

                case "mnu_Linha":

                    Tools.SetUserTableNavigator("@TB_LINHA");
                    Tools.UserTabNavigator.QueryToRecord(ref fields, fields);

                    _formLinha = new FormLinha();
                    //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                    _formLinha.Show();

                    break;

                case "mnu_Horario":
                    break;
                case "mnu_Viagem":
                    break;
                default:
                    break;
            }
        }
        
        private void HandleNavBar(string menuUID)
        {
            Tools.UserTabNavigator.RunningOperation = (int)Tools.OptionsHandler.Atualizacao;    //2-Operação de Atualização
            switch (menuUID)
            {
                case "1290":    //Evento sobre a Seta para ir para o primeiro registro
                    Tools.UserTabNavigator.FirstRecord();
                    break;
                case "1288":    //Evento sobre a Seta para ir para o próximo registro

                    if (this._formInitialized)
                        Tools.UserTabNavigator.NextRecord();
                    else
                        Tools.UserTabNavigator.FirstRecord();
                    
                    break;

                case "1289":    //Evento sobre a Seta para ir para o registro anterior
                    
                    if (this._formInitialized)
                        Tools.UserTabNavigator.PreviousRecord();
                    else
                        Tools.UserTabNavigator.FirstRecord();

                    break;

                case "1291":    //Evento sobre a Seta para ir para o registro final
                    Tools.UserTabNavigator.LastRecord();
                    break;
                case "1282":    //Evento sobre o menu (barra de ferramentas) Adicionar
                    Tools.UserTabNavigator.RunningOperation = (int)Tools.OptionsHandler.Inclusao;    //1-Operação de Inclusão de registro
                    break;
                case "1283":    //Evento sobre a opção (botão direito do mouse) Remover
                    Tools.UserTabNavigator.RunningOperation = (int)Tools.OptionsHandler.Exclusao;    //3-Operação de Exclusão                    
                    break;
                default:
                    break;
            }

            InitFormData(Tools.UserTabNavigator.RunningOperation);
        }
        private void InitFormData(int operation = -1)
        {
            SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;
            bool exclui = false;
            int howOperate = 0;

            this._formInitialized = true;

            //Definindo o modo em que o formulário deve trabalhar conforme a chamada do método
            //InitFormData(). A variável howOperate pode ser:
            //0 - Somente visualiza / pesquisa [pensar melhor nisto aqui]
            //1 - Adiciona novo registro
            //2 - Atualiza registro posicionado
            //3 - Remove (exclui) registro posicionado
            if (operation < 0 && Tools.UserTabNavigator.RunningOperation >= 0)
                howOperate = Tools.UserTabNavigator.RunningOperation;
            else if (operation >= 0)
                howOperate = operation;

            switch (howOperate)  
            {
                case 0:
                    form.Mode = (SAPbouiCOM.BoFormMode)SAPbouiCOM.BoFormMode.fm_OK_MODE; 
                    break;
                case 1:
                    form.Mode = (SAPbouiCOM.BoFormMode)SAPbouiCOM.BoFormMode.fm_ADD_MODE;
                    break;
                case 2:
                    form.Mode = (SAPbouiCOM.BoFormMode)SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                    break;
                case 3:
                    exclui = true;
                    break;
            }
            
            if (Tools.UserTabNavigator.UserTableName == "@TB_LOCALIDADE" && form.UniqueID == "FRMLocal")
            {
                this.HandleLocalidade(form, exclui);
            }
            else if (Tools.UserTabNavigator.UserTableName == "@TB_LINHA" && form.UniqueID == "FRMLinha")
            {
                this.HandleLinha(form, exclui);
            }
            else if (Tools.UserTabNavigator.UserTableName == "@TB_HORARIO" && form.UniqueID == "FRMHora")
            {

            }            
            
        }
        private void HandleLocalidade(SAPbouiCOM.Form form, bool exclui = false)
        {
            Localidade localidade = new Localidade(Tools.UserTabNavigator.RecordGetValue("Code").ToString(),
                                                    Tools.UserTabNavigator.RecordGetValue("Name").ToString());
            
            localidade.RepositoryToForm(form, Tools.UserTabNavigator.RunningOperation == (int)Tools.OptionsHandler.Inclusao);                
            
            if ( exclui && 
                Application.SBO_Application.MessageBox("Deseja excluir o registro?", 1, "Confirmar", "Cancelar") == 1 )
            {
                localidade.ManipulateData((int)Tools.OptionsHandler.Exclusao);
                
                if (localidade.HasError)
                    Application.SBO_Application.SetStatusBarMessage(localidade.ErrorMessage, SAPbouiCOM.BoMessageTime.bmt_Short);
                else   //Se for inclusão de registro, roda novamente a query do objeto RecordSet
                {
                    form.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                    Tools.UserTabNavigator.Setup();
                    Tools.UserTabNavigator.LastRecord();
                    
                    Application.SBO_Application.StatusBar.SetText(localidade.OKMessage, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                    this._formInitialized = false;
                }
            }            
        }
        private void HandleLinha(SAPbouiCOM.Form form, bool exclui = false)
        {
            //TODO: Alterar aqui a forma de manipular o formulário de cadastro de linha
            Linha linha= new Linha(Tools.UserTabNavigator.RecordGetValue("Code").ToString());

            linha.RepositoryToForm(form, Tools.UserTabNavigator.RunningOperation == (int)Tools.OptionsHandler.Inclusao);

            if (exclui &&
                Application.SBO_Application.MessageBox("Deseja excluir o registro?", 1, "Confirmar", "Cancelar") == 1)
            {
                linha.ManipulateData((int)Tools.OptionsHandler.Exclusao);

                if (linha.HasError)
                    Application.SBO_Application.SetStatusBarMessage(linha.ErrorMessage, SAPbouiCOM.BoMessageTime.bmt_Short);
                else   //Se for inclusão de registro, roda novamente a query do objeto RecordSet
                {
                    form.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                    Tools.UserTabNavigator.Setup();
                    Tools.UserTabNavigator.LastRecord();

                    Application.SBO_Application.StatusBar.SetText(linha.OKMessage, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                    this._formInitialized = false;
                }
            }
            //if ( !exclui )
            //{
            //    if (Tools.UserTabNavigator.RunningOperation == 1)   //Inclusão de dados, inicializa com os campos em branco
            //    {
            //        form.DataSources.DBDataSources.Item("@TB_LINHA").SetValue("Code", 0, "");
            //        form.DataSources.DBDataSources.Item("@TB_LINHA").SetValue("Name", 0, "");
            //    }
            //    else
            //    {
            //        form.DataSources.DBDataSources.Item("@TB_LINHA").SetValue("Code", 0, Tools.UserTabNavigator.RecordGetValue("Code").ToString());
            //        form.DataSources.DBDataSources.Item("@TB_LINHA").SetValue("Name", 0, Tools.UserTabNavigator.RecordGetValue("Name").ToString());

            //        //todo: Carregar a Matrix
            //    }
            //}
            //else
            //{
            //    if (Application.SBO_Application.MessageBox("Deseja excluir o registro?", 1, "Confirmar", "Cancelar") == 1) //1-Confirma, 2-Cancelar
            //    {
            //        if (_formLocalidade != null)
            //        {
            //            string Code = form.DataSources.DBDataSources.Item("@TB_LINHA").GetValue("Code", 0);
            //            string Name = form.DataSources.DBDataSources.Item("@TB_LINHA").GetValue("Name", 0);
            //            //Todo: Carregar a Matrix

            //            Linha linha = new Linha();
            //            linha.ManipulateData(Tools.UserTabNavigator.RunningOperation);
            //        }

            //    }
            //}
        }
        private FormLocalidade _formLocalidade;
        private FormLinha _formLinha;
        private bool _formInitialized = false;
        
    }
}
