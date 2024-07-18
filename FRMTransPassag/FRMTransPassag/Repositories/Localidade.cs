using FRMTransPassag.Framework.Classes;
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRMTransPassag.Repositories
{
    public class Localidade : IRepository
    {
        public Localidade(string codigo = "", string nome = "")
        {
            if (Tools.Company == null)
            {
                Tools.SetUICompany();
            }
            
            this.TabLocalidade = Tools.Company.UserTables.Item("TB_LOCALIDADE");

            if (codigo != "")
                this.Code = codigo;

            if (nome != "")
                this.Name = nome;
        }

        public bool ManipulateData(int operation)
        {
            bool manipulation = false;

            if (operation == 1) //1- Inserção de dados
            {
                this.TabLocalidade.Code = this.Code;
                this.TabLocalidade.Name = this.Name;
            }
            else if (operation == 2 || operation == 3)  //2- Atualização de dados, 3- Exclusão de dados
            {
                this.TabLocalidade.GetByKey(this.Code);
                this.TabLocalidade.Name = this.Name;
            }

            switch (operation)
            {
                case 1: //Inserção de registro de nova localidade

                    manipulation = this.TabLocalidade.Add() == 0;

                    if (manipulation)
                    {
                        this.ResetError();

                        this.OKMessage = "Inclusão do registro de localidade realizado com sucesso! ";
                        this.OKMessage += "[Code: " + this.TabLocalidade.Code + "]";
                    }
                    else
                    {
                        this._error = true;
                        this.ErrorMessage = "Não foi possível efetuar a inclusão da localidade! ";
                        this.ErrorMessage += "[Code: " + this.TabLocalidade.Code + "]";
                    }
                    
                    break;

                case 2: //Atualização de localidade exsitente
                    
                    manipulation = this.TabLocalidade.Update() == 0;
                    
                    if (manipulation)
                    {
                        this.ResetError();
                        this.OKMessage = "Atualização do registro de localidade efetuado com sucesso! ";
                        this.OKMessage += "[Code: " + this.TabLocalidade.Code + "]";
                    }
                    else
                    {
                        this._error = true;
                        this.ErrorMessage = "Não foi possível atualizar o cadastro da localidade! ";
                        this.ErrorMessage += "[Code: " + this.TabLocalidade.Code + "]";
                    }
                   
                    break;

                case 3: //Remoção (exclusão) de uma localidade
                    
                    manipulation = this.TabLocalidade.Remove() == 0;
                    
                    if (manipulation)
                    {
                        this.ResetError();
                        this.OKMessage = "Registro de localidade excluído com sucesso! ";
                        this.OKMessage += "[Code: " + this.TabLocalidade.Code + "]";
                    }
                    else
                    {
                        this._error = true;
                        this.ErrorMessage = "Não foi possível excluir o cadastro da localidade! ";
                        this.ErrorMessage += "[Code: " + this.TabLocalidade.Code + "]";
                    }
                    
                    break;

                default:
                    break;
            }            

            return this._error;
        }
        public void FormToRepository(Form form)
        {
            DBDataSource dsLocalidade = form.DataSources.DBDataSources.Item("@TB_LOCALIDADE");
            
            this.Code = dsLocalidade.GetValue("Code", 0);   //((EditText)form.Items.Item("txtLocali").Specific).String;
            this.Name = dsLocalidade.GetValue("Name", 0);   //((EditText)form.Items.Item("txtNome").Specific).String;
        }
        public void RepositoryToForm(Form form, bool newReg = false)
        {            
            DBDataSource dsLocalidade = form.DataSources.DBDataSources.Item("@TB_LOCALIDADE");
            
            try
            {
                if (!newReg)
                {
                    dsLocalidade.SetValue("Code", 0, this.Code);    //((EditText)form.Items.Item("txtLocali").Specific).Value = this.Code;
                    dsLocalidade.SetValue("Name", 0, this.Name);    //((EditText)form.Items.Item("txtNome").Specific).Value = this.Name;
                }
                else
                {
                    dsLocalidade.SetValue("Code", 0, "");       //((EditText)form.Items.Item("txtLocali").Specific).Value = "";
                    dsLocalidade.SetValue("Name", 0, "");       //((EditText)form.Items.Item("txtNome").Specific).Value = "";
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void ResetError()
        {
            this._error = false;
            this.ErrorMessage = "";
        }
        #region Propriedades da Classe
        private bool _error = false;
        public string Code { set; get; }
        public string Name { set; get; }
        public UserTable TabLocalidade = null;
        public bool HasError { get { return this._error; } }
        public string ErrorMessage { set; get; }
        public string OKMessage { set; get; }
        #endregion
    }
}
