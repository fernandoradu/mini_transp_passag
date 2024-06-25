using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRMTransportePassageiros.Repositories
{
    public class Linha
    {
        public void SetDataToForm(Form form)
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
        public string Code { get; set; }
        public string Name { get; set; }
        public List<SecaoLinha> SecoesLinha { get; set; }
    }
}
