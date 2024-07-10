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
        void FormToRepository(Form form);
        void RepositoryToForm(Form form);
        bool ManipulateData(int operation);
    }
}
