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
    public class Linha : IRepository
    {
        public Linha(string codeLinha = "")
        {
            if ( Tools.Company == null )
            {
                Tools.SetUICompany();
            }            
            
            this.TabLinhas = Tools.Company.UserTables.Item("TB_LINHA");
            this.TabSecLinhas = Tools.Company.UserTables.Item("TB_SECLINHA");

            if (!string.IsNullOrEmpty(codeLinha))
            {
                this.SeekLinha(new string[,] { {"Code", codeLinha } });
            }
        }
        public void FormToRepository(Form form)
        {
            Matrix matrix = (Matrix)form.Items.Item("mtxSecoes").Specific;
            EditText cCode = null;
            EditText cLocOrig = null;
            EditText cNLocOri = null;
            EditText cLocDest = null;
            EditText cNLocDes = null;

            if (this.SecoesLinha == null)
                this.SecoesLinha = new List<SecaoLinha>();

            this.Code = ((EditText)form.Items.Item("txtLinha").Specific).String;
            this.Name = ((EditText)form.Items.Item("txtNLinha").Specific).String;

            this.SecoesLinha.Clear();

            for (int row = 1; row <= matrix.RowCount; row++)
            {
                cCode = (EditText)matrix.Columns.Item("cCode").Cells.Item(row).Specific;
                cLocOrig = (EditText)matrix.Columns.Item("cLocOrig").Cells.Item(row).Specific;
                cNLocOri = (EditText)matrix.Columns.Item("cNLocOri").Cells.Item(row).Specific;
                cLocDest = (EditText)matrix.Columns.Item("cLocDest").Cells.Item(row).Specific;
                cNLocDes = (EditText)matrix.Columns.Item("cNLocDes").Cells.Item(row).Specific;

                if (!string.IsNullOrEmpty(cCode.String) )
                {
                    SecoesLinha.Add(new SecaoLinha(cCode.String, this.Code, cLocOrig.String, cLocDest.String));
                }
            }
        }
        public bool ManipulateData(int operation)
        {
            bool ret = false;

            switch (operation)
            {
                case (int)Tools.OptionsHandler.Inclusao:

                    this.TabLinhas.Code = this.Code;
                    this.TabLinhas.Name = this.Name;

                    for (int row = 0; row <= this.SecoesLinha.Count - 1; row++)
                    {

                        ret = this.HandleSecoes(row);

                        if (!ret)
                            break;
                    }

                    if (ret)
                    {
                        ret = this.TabLinhas.Add() == 0;
                    }
                    
                    break;

                    //TODO: Testar comportamento da Matrix, durante exclusão de linha 
                case (int)Tools.OptionsHandler.Atualizacao:
                    
                    this.TabLinhas.GetByKey(this.Code);
                    this.TabLinhas.Name = this.Name;

                    for (int row = 0; row < this.SecoesLinha.Count; row++)
                    {
                        ret = this.HandleSecoes(row, !(this.TabSecLinhas.GetByKey(this.SecoesLinha[row].Code)));
                    }

                    if (ret)
                        ret = this.TabLinhas.Update() == 0;

                    break;
                case (int)Tools.OptionsHandler.Exclusao:

                    this.TabLinhas.GetByKey(this.Code);
                    
                    for (int row = 0; row < this.SecoesLinha.Count; row++)
                    {
                        ret = this.HandleSecoes(row, !(this.TabSecLinhas.GetByKey(this.SecoesLinha[row].Code)), true);
                    }
                    
                    break;

                default:
                    break;
            }

            return ret;
        }
        private bool HandleSecoes(int row, bool inclui = true, bool exclui = false)
        {
            bool ret = true;

            if (!exclui)
            {
                if (inclui)
                    this.TabSecLinhas.Code = this.SecoesLinha[row].Code;

                this.TabSecLinhas.UserFields.Fields.Item("U_CodeLinha").Value = this.SecoesLinha[row].CodeLinha;
                this.TabSecLinhas.UserFields.Fields.Item("U_LocalPartida").Value = this.SecoesLinha[row].LocalPartida;
                this.TabSecLinhas.UserFields.Fields.Item("U_LocalChegada").Value = this.SecoesLinha[row].LocalChegada;

                if (inclui)
                    ret = this.TabSecLinhas.Add() == 0;
                else
                    ret = this.TabSecLinhas.Update() == 0;
            }
            else
                ret = this.TabSecLinhas.Remove() == 0;

            return ret;
        }
        public void RepositoryToForm(Form form ,bool newReg = false)
        {
            EditText fieldLinha = (EditText)form.Items.Item("txtLinha").Specific;
            EditText fieldNLinha = (EditText)form.Items.Item("txtNLinha").Specific;
            Matrix mtxSecoes = (Matrix)form.Items.Item("mtxSecoes").Specific;
            Localidade localPartida = new Localidade();
            Localidade localChegada = new Localidade();            

            int currentRow = 0;
            if (newReg)
            {
                //TODO:  Fazer a inicialização e tratamento dos dados aqui.
            }
            else
            {
                fieldLinha.Value = this.Code;
                fieldNLinha.Value = this.Name;

                foreach (SecaoLinha secao in this.SecoesLinha)
                {
                    string[,] seekPartida = new string[,] { { "Code", "'" + secao.LocalPartida + "'"} };
                    string[,] seekChegada = new string[,] { { "Code", "'" + secao.LocalChegada + "'"} };

                    localPartida.SeekLocalidade(seekPartida);
                    localChegada.SeekLocalidade(seekChegada);

                    mtxSecoes.AddRow();
                    currentRow = mtxSecoes.RowCount;
                    ((EditText)mtxSecoes.Columns.Item("cCode").Cells.Item(currentRow).Specific).Value = secao.Code;
                    ((EditText)mtxSecoes.Columns.Item("cLocOrig").Cells.Item(currentRow).Specific).Value = secao.LocalPartida;
                    ((EditText)mtxSecoes.Columns.Item("cNLocOri").Cells.Item(currentRow).Specific).Value = localPartida.Name;
                    ((EditText)mtxSecoes.Columns.Item("cLocDest").Cells.Item(currentRow).Specific).Value = secao.LocalChegada;
                    ((EditText)mtxSecoes.Columns.Item("cNLocDes").Cells.Item(currentRow).Specific).Value = localChegada.Name;

            }
                
            }
        }
        public void ResetError()
        {
            throw new NotImplementedException();
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
            this.Recordset = (Recordset)Tools.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            if (init)
                this.ExecuteQuery();            
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
            this._query = Tools.ComposeSimpleQuery( new string[] 
                { this._tableFather + "T0", this._tableChild + "T1"}, 
               fields, filter, new string[,] { { "T0.Code","T1.U_CodeLinha" } });            
        }
        
        public bool SeekLinha(string[,] searchReg, bool selfUpdate = true)
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

        #region Propiedades da Classe
        private string _tableFather = "\"@TB_LINHA\"";
        private string _tableChild = "\"@TB_SECLINHA\"";
        private string _query = "";
        //private string _selectionFields = "";
        //private string[] _recordFields = null;        
        private Recordset _recordset = null;
        private bool _error = false;
        public string Code { get; set; }
        public string Name { get; set; }
        public string ErrorMessage { get; set; }
        public string OKMessage { get; set; }
        public object TableName { get; set; }
        public bool HasError { get { return this._error; } }
        public UserTable TabLinhas = null;
        public UserTable TabSecLinhas = null;
        public Recordset Recordset { get; set; }
        public List<SecaoLinha> SecoesLinha { get; set; }
        #endregion
    }
}
