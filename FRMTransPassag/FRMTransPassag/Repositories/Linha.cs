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
        public Linha()
        {
            if ( Tools.Company == null )
            {
                Tools.SetUICompany();
                this.TabLinhas = Tools.Company.UserTables.Item("TB_LINHA");
                this.TabSecLinhas = Tools.Company.UserTables.Item("TB_SECLINHA");
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

            this.Code = ((EditText)form.Items.Item("txtLinha").Specific).String;
            this.Name = ((EditText)form.Items.Item("txtNLinha").Specific).String;

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

            this.TabLinhas.Code = this.Code;
            this.TabLinhas.Name = this.Name;            

            if ( operation == 1 ) //1- Inserção de dados
            {
                for (int row = 0; row <= this.SecoesLinha.Count - 1; row++)
                {
                    this.TabSecLinhas.Code = this.SecoesLinha[row].Code;
                    this.TabSecLinhas.UserFields.Fields.Item("U_CodeLinha").Value = this.SecoesLinha[row].CodeLinha;
                    this.TabSecLinhas.UserFields.Fields.Item("U_LocalPartida").Value = this.SecoesLinha[row].LocalPartida;
                    this.TabSecLinhas.UserFields.Fields.Item("U_LocalChegada").Value = this.SecoesLinha[row].LocalChegada;

                    ret = this.TabSecLinhas.Add() == 0;

                    if (!ret)
                        break;
                }

                if (ret)
                {
                    ret = this.TabLinhas.Add() == 0;
                }
            }
            else if ( operation == 2)   //2 - update
            {
            }
            else if ( operation == 3)   //3 - delete
            { 
            }
            return ret;
        }
        public void RepositoryToForm(Form form ,bool newReg = false)
        {

        }
        public void ResetError()
        {

        }
        #region Propiedades da Classe
        private bool _error = false;
        public string Code { get; set; }
        public string Name { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get { return this._error; } }
        public UserTable TabLinhas = null;
        public UserTable TabSecLinhas = null;
        public List<SecaoLinha> SecoesLinha { get; set; }
        #endregion
    }
}
