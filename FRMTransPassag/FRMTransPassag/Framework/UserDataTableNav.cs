using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM.Framework;
using SAPbobsCOM;
using FRMTransPassag.Framework.Classes;

namespace FRMTransPassag.Framework
{
    public class UserDataTableNav
    {
        public UserDataTableNav(string userTable, Company company)
        {
            this._userTable = userTable;
            this._company = company;
            this.renew = true;

            _recUserTable = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
        }

        public void Setup(string query = null)
        {
            renew = query != null;
            
            if (renew)
                _storedQuery = query;

            try
            {
                _recUserTable.DoQuery(_storedQuery.ToString());
                _recUserTable.MoveFirst();
                renew = false;
            }
            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(ex.Message,SAPbouiCOM.BoMessageTime.bmt_Short);
            }
        }
        public void NextRecord()
        {
            try
            {
                if (UserTableRecords.EoF)
                    UserTableRecords.MoveLast();
                else
                    UserTableRecords.MoveNext();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void PreviousRecord()
        {
            try
            {
                if (UserTableRecords.BoF)
                    UserTableRecords.MoveLast();
                else
                    UserTableRecords.MovePrevious();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void LastRecord()
        {
            try
            {
                UserTableRecords.MoveLast();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void FirstRecord()
        {
            try
            {
                UserTableRecords.MoveFirst();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public dynamic RecordGetValue(object fieldSource)
        {
            dynamic anyValue = null;
            bool hasAquired = false;

            try
            {
                anyValue = this.UserTableRecords.Fields.Item(fieldSource).Value.ToString();
                hasAquired = true;
            }
            catch (Exception)
            {
                this.Setup();
            }
            finally
            {
                if (!hasAquired)
                {
                    anyValue = this.UserTableRecords.Fields.Item(fieldSource).Value.ToString();
                    hasAquired = true;
                }
            }
            
            return anyValue;
        }
        public void QueryToRecord(ref string[] indexFields, string[] fields = null, string table = null)
        {
            StringBuilder query = new StringBuilder();
            bool hasOrderOk = indexFields != null;

            if (string.IsNullOrEmpty(_storedQuery) || this.UserTableName != table)
            {
                if (table != null && this.UserTableName != table)
                    this._userTable = table;

                query.Append("SELECT ");

                if (fields != null)
                {
                    for (int ind = 0; ind <= fields.Length - 1; ind++)
                    {
                        query.AppendFormat("\"{0}\"" + (ind < fields.Length - 1 ? ", " : " "), fields[ind]);
                        
                        if ( fields[ind].Contains("Code") && !hasOrderOk )
                        {
                            indexFields = new string[] { fields[ind] };
                            hasOrderOk = true;
                        }
                    }
                }
                else
                    query.Append("* ");

                query.Append("FROM ");
                query.AppendFormat("\"{0}\" T0 ", UserTableName);

                if ( hasOrderOk )
                {
                    query.Append("ORDER BY ");

                    for (int i = 0; i <= indexFields.Length-1; i++)
                    {
                        query.AppendFormat("\"{0}\"" + (i < indexFields.Length - 1 ? ", " : " "), indexFields[i]);
                    }
                }
                _storedQuery = query.ToString();
            }

            this.Setup(this._storedQuery);
            
        }
        
        private string _userTable;
        private string _storedQuery;
        private Recordset _recUserTable;
        private Company _company;
        private bool renew;

        public enum OperationOnRegister
        {
            View,
            Insert,
            Update,
            Delete
        }
        public int RunningOperation;    //0-View, 1-Insert, 2-Update, 3-Delete
        public string UserTableName { get { return _userTable; } }
        public Recordset UserTableRecords { get { return _recUserTable; } }
    }
}
