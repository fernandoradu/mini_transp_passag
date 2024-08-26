using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRMTransPassag.Repositories
{
    public interface IRepository
    {
        #region Métodos obrigatórios
        bool ManipulateData(int operation);
        void FormToRepository(Form form);
        void RepositoryToForm(Form form, bool newReg);
        void ResetError();
        void SetFormMode(Form form);
        void SetupRecordset(bool init);
        void ExecuteQuery();
        #endregion
        //---------------------------------------
        #region Propriedades obrigatórias
        string ErrorMessage { set; get; }
        string OKMessage { set; get; }
        bool HasError { get; }
        object TableName { get; }
        Recordset Recordset { get; }
        
        #endregion
    }
}
