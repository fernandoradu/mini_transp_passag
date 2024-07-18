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
        void FormToRepository(Form form);
        void RepositoryToForm(Form form, bool newReg);
        void ResetError();
        bool ManipulateData(int operation);
        #endregion
        //---------------------------------------
        #region Propriedades obrigatórias
        string ErrorMessage { set; get; }
        bool HasError { get; }
        #endregion
    }
}
