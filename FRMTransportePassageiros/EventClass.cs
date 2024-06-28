using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM.Framework;

namespace FRMTransportePassageiros
{
    class EventClass
    {
        public static void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            //if (pVal.BeforeAction)
            //{
                
            //    //if (pVal.FormTypeEx == "139" && pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED && pVal.ItemUID == "1")
            //    //{
            //    //    SAPbouiCOM.Form oForm = Application.SBO_Application.Forms.GetFormByTypeAndCount(139, pVal.FormTypeCount);

            //    //    var comments = ((SAPbouiCOM.EditText)oForm.Items.Item("16").Specific).String;

            //    //    if (comments.Length < 10)
            //    //    {
            //    //        Application.SBO_Application.SetStatusBarMessage("Campo Comentario com menos de 10 caracteres", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            //    //        BubbleEvent = false;
            //    //    }
            //    //}
            //    //else if (pVal.FormTypeEx == "139" && pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD)
            //    //{
            //    //    SAPbouiCOM.Form oForm = Application.SBO_Application.Forms.GetFormByTypeAndCount(139, pVal.FormTypeCount);

            //    //    SAPbouiCOM.Item RefItem = oForm.Items.Item("2");
            //    //    SAPbouiCOM.Item oItem = oForm.Items.Add("bt1", SAPbouiCOM.BoFormItemTypes.it_BUTTON);

            //    //    oItem.Top = RefItem.Top;
            //    //    oItem.Left = RefItem.Left + RefItem.Width + 5;

            //    //    SAPbouiCOM.Button oButton = ((SAPbouiCOM.Button)oItem.Specific);
            //    //    oButton.Caption = "Meu Botao";

            //    //    oButton.PressedAfter += OButton_PressedAfter;

            //    //}
            //    if ( pVal.FormTypeEx == "1290" )
            //    {
            //        Application.SBO_Application.SetStatusBarMessage("Teste 1290 - before", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            //    }
            //}
            //else if ( pVal.ActionSuccess )
            //{
            //    if ( pVal.FormTypeEx == "1290" )
            //    {
            //        Application.SBO_Application.SetStatusBarMessage("Teste 1290 - success", SAPbouiCOM.BoMessageTime.bmt_Short, false);                    
            //    }
            //}
        }
    }
}
