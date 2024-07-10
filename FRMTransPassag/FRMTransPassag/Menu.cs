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
                    Tools.UserTabNavigator.QueryToRecord(fields);

                    _formLocalidade = new FormLocalidade();
                    //System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                    _formLocalidade.Show();

                    break;

                case "mnu_Linha":

                    Tools.SetUserTableNavigator("@TB_LINHA");
                    Tools.UserTabNavigator.QueryToRecord(fields);

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
            Tools.UserTabNavigator.RunningOperation = 2;    //2-Operação de Atualização
            switch (menuUID)
            {
                case "1290":    //Evento sobre a Seta para ir para o primeiro registro
                    Tools.UserTabNavigator.FirstRecord();
                    break;
                case "1288":    //Evento sobre a Seta para ir para o próximo registro
                    Tools.UserTabNavigator.NextRecord();
                    break;
                case "1289":    //Evento sobre a Seta para ir para o registro anterior
                    Tools.UserTabNavigator.PreviousRecord();
                    break;
                case "1291":    //Evento sobre a Seta para ir para o registro final
                    Tools.UserTabNavigator.LastRecord();
                    break;
                case "1282":
                    Tools.UserTabNavigator.RunningOperation = 1;    //1-Operação de Inclusão de registro
                    break;
                case "1283":
                    Tools.UserTabNavigator.RunningOperation = 3;    //3-Operação de Exclusão                    
                    break;
                default:
                    break;
            }

            InitFormData(Tools.UserTabNavigator.RunningOperation == 3);
        }
        private void InitFormData(bool exclui = false)
        {
            SAPbouiCOM.Form form = Application.SBO_Application.Forms.ActiveForm;

            if (!exclui)
            {
                if (Tools.UserTabNavigator.UserTableName == "@TB_LOCALIDADE" && form.UniqueID == "FRMLocal")
                {
                    if (Tools.UserTabNavigator.RunningOperation == 1)   //Inclusão de dados, inicializa com os campos em branco
                    {
                        form.DataSources.DBDataSources.Item("@TB_LOCALIDADE").SetValue("Code", 0, "");
                        form.DataSources.DBDataSources.Item("@TB_LOCALIDADE").SetValue("Name", 0, "");
                    }
                    else
                    {
                        form.DataSources.DBDataSources.Item("@TB_LOCALIDADE").SetValue("Code", 0, Tools.UserTabNavigator.RecordGetValue("Code").ToString());
                        form.DataSources.DBDataSources.Item("@TB_LOCALIDADE").SetValue("Name", 0, Tools.UserTabNavigator.RecordGetValue("Name").ToString());
                    }
                }
                else if (Tools.UserTabNavigator.UserTableName == "@TB_LINHA" && form.UniqueID == "FRMLinha")
                {

                }
                //TODO: Preparar para os demais formulários
            }
            else
            {
                //Exclusão de registro
                if (form.UniqueID == "FRMLocal")
                {
                    if (Application.SBO_Application.MessageBox("Deseja excluir o registro?", 1, "Confirmar", "Cancelar") == 1) //1-Confirma, 2-Cancelar
                    {
                        if ( _formLocalidade != null)
                        {
                            string Code = form.DataSources.DBDataSources.Item("@TB_LOCALIDADE").GetValue("Code", 0);
                            string Name = form.DataSources.DBDataSources.Item("@TB_LOCALIDADE").GetValue("Name", 0);
                            
                            Localidade localidade = new Localidade(Code,Name);
                            localidade.ManipulateData(Tools.UserTabNavigator.RunningOperation);
                        }
                            //FormLocalidade.HandlingRegister(Tools.Company, (UserTable)Tools.Company.UserTables.Item("TB_LOCALIDADE"),
                            //    (Recordset)Tools.Company.GetBusinessObject(BoObjectTypes.BoRecordset), 
                            //    FormLocalidade.SetOperation(),
                            //    form.DataSources.DBDataSources.Item("@TB_LOCALIDADE").GetValue("Code",0), 
                            //    form.DataSources.DBDataSources.Item("@TB_LOCALIDADE").GetValue("Name", 0));
                    }

                }
                else if (form.UniqueID == "FRMLinha")
                {
                    //TODO: Tratamento para exclusão do cadastro de Linha
                }
                else if (form.UniqueID == "FRMHorar")
                {
                    //TODO: Tratamento para exclusão do cadastro de Linha
                }
                else if (form.UniqueID == "FRMViagem")
                {
                    //TODO: Tratamento para exclusão do cadastro de Linha
                }
            }

        }
        private FormLocalidade _formLocalidade;
        private FormLinha _formLinha;
    }
}
