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
        public void SetFormMode(Form form)
        {
            switch (form.Mode)
            {
                case SAPbouiCOM.BoFormMode.fm_FIND_MODE:
                    break;
                case SAPbouiCOM.BoFormMode.fm_OK_MODE:
                    goto case SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                case SAPbouiCOM.BoFormMode.fm_UPDATE_MODE:
                    this.ManipulateData(2);
                    break;
                case SAPbouiCOM.BoFormMode.fm_ADD_MODE:
                    this.ManipulateData(1);
                    form.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                    break;
                case SAPbouiCOM.BoFormMode.fm_VIEW_MODE:
                    break;
                case SAPbouiCOM.BoFormMode.fm_PRINT_MODE:
                    break;
                case SAPbouiCOM.BoFormMode.fm_EDIT_MODE:
                    break;
                case SAPbouiCOM.BoFormMode.fm_ARCHIVE_MODE:
                    break;
                default:
                    break;
            }
        }
        public void SetupRecordset(bool init = false)
        {
            this._recordset = (Recordset)Tools.Company.GetBusinessObject(BoObjectTypes.BoRecordset);            
            
            if (init)
            {
                this.ExecuteQuery();
            }
        }
        public void ExecuteQuery()
        { 
            try
            {
                if (this._recordset == null)
                    this.SetupRecordset();

                if (this._query != "")
                    this._recordset.DoQuery(this._query);
                else
                    throw new Exception("Erro na execução da consulta (query).");
            }
            catch (Exception ex)
            {
                if (this._recordset == null)
                    this.SetupRecordset();
                
                //Tenta executar uma query simples: Select * From @TB_LOCALIDADE
                this.SetFilterToQuery();
                _recordset.DoQuery(this._query);                
            }            
        }
        public void SetFilterToQuery(string[] fields = null, object[,] filter = null)
        {
            if (string.IsNullOrEmpty(this._table))
                this._table = "\"@TB_LOCALIDADE\"";

            this._query = Tools.ComposeSimpleQuery(this._table, fields, filter);//this.ComposeQuery(fields, filter);
            this.SetSelectionFields(fields);
        }
        public void SetSelectionFields(object fields = null)
        {
            StringBuilder selection = new StringBuilder();
            if (fields != null && fields.GetType().Name != "string")
            {
                this._recordFields = (string[])fields;
            
                for (int i = 0; i < this._recordFields.Length - 1; i++)
                {
                    selection.AppendFormat("{0} ", this._recordFields[i]);

                    if (i < this._recordFields.Length - 1)
                        selection.Append(", ");
                }

            }
            else
            {
                this._recordFields = null;
                selection.Append("SELECT * ");
            }

            this._selectionFields = selection.ToString();
        }
        public bool SeekLocalidade(string[,] searchReg, bool selfUpdate = true)
        {
            bool found = false;
            
            this.SetFilterToQuery(null, searchReg);
            this.ExecuteQuery();
            this.Recordset.MoveFirst();

            if (!this.Recordset.EoF)
            {
                found = true;
                
                if (selfUpdate)
                {
                    this.Code = this.Recordset.Fields.Item("Code").Value.ToString();
                    this.Name = this.Recordset.Fields.Item("Name").Value.ToString();
                }
            }
            
            return found;
        }
        #region Propriedades da Classe
        private string _table = "\"@TB_LOCALIDADE\"";
        private string _query = "";        
        private string _selectionFields = "";
        private string[] _recordFields = null;
        private bool _error = false;
        private Recordset _recordset = null;
        public string Code { set; get; }
        public string Name { set; get; }
        public UserTable TabLocalidade = null;
        public Recordset Recordset { get { return _recordset; } }
        public bool HasError { get { return this._error; } }
        public string ErrorMessage { set; get; }
        public string OKMessage { set; get; }
        public object TableName { get { return _table; } }
        #endregion
    }
}
