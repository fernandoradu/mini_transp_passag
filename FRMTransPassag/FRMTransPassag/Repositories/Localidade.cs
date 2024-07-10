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
                this.TabLocalidade = Tools.Company.UserTables.Item("@TB_LOCALIDADE");
            }

            if (codigo != "")
                this.Code = codigo;

            if (nome != "")
                this.Name = nome;
        }

        public bool ManipulateData(int operation)
        {
            bool ret = false;

            this.TabLocalidade.GetByKey(this.Code);

            this.TabLocalidade.Code = this.Code;
            this.TabLocalidade.Name = this.Name;

            if (operation == 1) //1- Inserção de dados
            {
                ret = this.TabLocalidade.Add() == 0;
            }
            else if (operation == 2)   //2 - update
            {
                ret = this.TabLocalidade.Update() == 0;
            }
            else if (operation == 3)   //3 - delete
            {
                ret = this.TabLocalidade.Remove() == 0;
            }
            return ret;
        }
        public void FormToRepository(Form form)
        {

            this.Code = ((EditText)form.Items.Item("txtLocali").Specific).String;
            this.Name = ((EditText)form.Items.Item("txtNome").Specific).String;
        }
        public void RepositoryToForm(Form form)
        {

        }
        #region Propriedades da Classe
        public string Code { set; get; }
        public string Name { set; get; }
        public UserTable TabLocalidade = null;
        #endregion
    }
}
