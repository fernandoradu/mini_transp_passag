using FRMTransPassag.Forms;
using SAPbobsCOM;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FRMTransPassag.Framework.Classes
{
	/// <summary>
	/// Classe estática contendo métodos úteis para o framework.
	/// </summary>
	public static class Tools
	{
		/// <summary>
		/// Dicionário para armazenar informações de conexão.
		/// </summary>
		public static Dictionary<string, string> InfoConexao;

		/// <summary>
		/// Método para gerar a próxima string alfanumérica.
		/// </summary>
		/// <param name="valueInicial">Valor inicial para a geração da próxima string.</param>
		/// <returns>A próxima string alfanumérica.</returns>
		public static string StringNext(string valueInicial = "0")
		{
			// Expressão regular para verificar se a entrada é alfanumérica
			Regex alphanumericRegex = new Regex("^[A-Za-z0-9]+$");

			if (!alphanumericRegex.IsMatch(valueInicial))
			{
				throw new ArgumentException("A entrada deve ser alfanumérica.");
			}

			// Incrementa a string alfanumérica
			char[] chars = valueInicial.ToCharArray();
			for (int i = chars.Length - 1; i >= 0; i--)
			{
				if (char.IsDigit(chars[i]))
				{
					if (chars[i] == '9')
					{
						chars[i] = 'A';
						return new string(chars);
					}
					else
					{
						chars[i] = (char)(chars[i] + 1);
						return new string(chars);
					}
				}
				else if (char.IsLetter(chars[i]))
				{
					char upper = char.ToUpper(chars[i]);
					if (upper == 'Z')
					{
						chars[i] = '0';
						return new string(chars);
					}
					else
					{
						chars[i] = (char)(upper + 1);
						return new string(chars);
					}
				}
			}
			// Caso a string seja vazia, retorna '1'
			return "0";
		}
		/// <summary>
		/// Verifica se um determinado método está presente na pilha de chamadas.
		/// </summary>
		/// <param name="methodName">Nome do método a ser verificado.</param>
		/// <returns>True se o método estiver na pilha de chamadas, caso contrário, False.</returns>
		public static bool IsMethodInStack(string methodName)
		{
			bool ret = false;
			StackTrace stackTrace = new StackTrace();
			StackFrame frame = null;
			MethodBase baseMethod;

			for (int i = 0; i < stackTrace.FrameCount; i++)
			{
				frame = stackTrace.GetFrame(i);
				baseMethod = frame.GetMethod();

				if (baseMethod != null && baseMethod.Name == methodName)
				{
					ret = true;
					break;
				}

			}
			return ret;
		}
		/// <summary>
		/// Armazena informações de conexão.
		/// </summary>
		/// <param name="serverSLD">Servidor SLD.</param>
		/// <param name="portSLD">Porta SLD.</param>
		/// <param name="companyDB">Banco de dados da empresa.</param>
		/// <param name="portDB">Porta do banco de dados.</param>
		/// <param name="usuario">Nome de usuário.</param>
		/// <param name="senha">Senha do usuário.</param>
		public static void StoreInfoConexao(string serverSLD = null, string portSLD = "", string companyDB = "", string portDB = "", string usuario = "", string senha = "")
		{
			InfoConexao = new Dictionary<string, string>();
			InfoConexao.Add("ServerSLD", serverSLD);
			InfoConexao.Add("PortSLD", portSLD);
			InfoConexao.Add("CompanyDB", companyDB);
			InfoConexao.Add("PortDB", portDB);
			InfoConexao.Add("User", usuario);
			InfoConexao.Add("Password", senha);
		}
		/// <summary>
		/// Armazena a URL da Service Layer.
		/// </summary>
		/// <param name="urlServiceLayer">URL da Service Layer.</param>
		public static void StoreUrlSL(string urlServiceLayer)
		{
			if (!InfoConexao.ContainsKey("Url"))
				InfoConexao.Add("Url", urlServiceLayer);
			else if (string.IsNullOrEmpty(InfoConexao["Url"]))
			{
				InfoConexao["Url"] = urlServiceLayer;
			}
		}
		/// <summary>
		/// Obtém a URL da Service Layer.
		/// </summary>
		/// <returns>URL da Service Layer.</returns>
		public static string GetUrlSL()
		{
			string ret = "";

			if (InfoConexao.ContainsKey("Url"))
				ret = InfoConexao["Url"];

			return ret;
		}

		public static bool ExistReg(Recordset oRecSet, string table, string alias, 
			string[,] seek, string fieldIdReg, out string idReg, out string errorMessage)
        {
			bool existe = false;

			StringBuilder query = new StringBuilder();
			errorMessage = "";
			idReg = "";

			try
			{
				query.Append("SELECT ");
				query.AppendFormat("{0}.\"{1}\", ", alias, seek[0,0]);
				query.AppendFormat("{0}.\"{1}\" ", alias, fieldIdReg);
				query.AppendFormat(" FROM \"{0}\" ", table);
				query.AppendFormat(" {0} ", alias);
				
				for (int ind = 0; ind < seek.Length-1; ind++)
				{
					if (ind == 0)
						query.Append("WHERE ");
					
					query.AppendFormat("{0}.\"{1}\" = {2}", alias,seek[ind,0],seek[ind,1]);
                }

				oRecSet.DoQuery(query.ToString());

				oRecSet.MoveFirst();

				if ( oRecSet.Fields.Item(0).Value.ToString() != "" )
				{
					existe = !string.IsNullOrWhiteSpace(oRecSet.Fields.Item(0).Value.ToString());
					idReg = oRecSet.Fields.Item(fieldIdReg).Value.ToString();
				}
			}
			catch (Exception ex)
			{
				existe = false;
				errorMessage = ex.Message;
			}           

			return existe;
        }
		public static string ComposeSimpleQuery(string table, object fields = null, object[,] filter = null)
        {
			string query = "";
			StringBuilder buildQuery = new StringBuilder();

			buildQuery.Append("SELECT ");
			
			if (fields == null || fields.GetType().Name == "string")
				buildQuery.Append("* ");
			else if (fields.GetType().Name == "string[]")
            {
				string[] internalFields = (string[])fields;

                for (int i = 0; i < internalFields.Length - 1; i++)
                {
					buildQuery.AppendFormat("{0} ", internalFields[i]);

					if (i < internalFields.Length - 1)
						buildQuery.Append("AND ");
                }
            }

			buildQuery.AppendFormat("FROM {0} ", table);

			if (filter != null)
            {
				buildQuery.Append("WHERE ");

				for (int i = 0; i < filter.Length - 1; i++)
				{
					buildQuery.AppendFormat("{0} {1} {2} ",
						filter[i, 0].ToString(),
						filter.GetLength(1) > 2 && filter[i, 2] != null ? filter[i, 2].ToString() : "=",
						filter[i, 1].ToString());

					if (i < filter.Length - 1)
						if (filter.GetLength(1) > 3)
							buildQuery.AppendFormat("{0} ", filter[i, 3] != null ? filter[i, 3].ToString() : "AND");
                }

            }
			
			query = buildQuery.ToString();

			return query;
        }
		public static void SetUICompany()
        {
			if (Tools.Company == null)
				Tools.Company = (Company)Application.SBO_Application.Company.GetDICompany();
		}
		public static void SetUserTableNavigator(string table)
        {
			if (Tools.Company == null)
				Tools.SetUICompany();

			if (UserTabNavigator == null)
				UserTabNavigator = new UserDataTableNav(table, Tools.Company);

        }
		public static void ConsultaRegistro(object formCaller, string columnCode, string columnDescription)
        {
			Tools.FormCaller = formCaller;

            switch (formCaller.GetType().Name)
            {
				case "FormLinha":	// formulário chamador da consulta é de cadastro de Linhas
					Tools.TableLookUp = "@TB_LOCALIDADE";
					Tools.FormConsulta = new FormConsultaRegistro((FormLinha)formCaller, columnCode, columnDescription);
					break;
				default:
                    break;
            }
			
			Tools.FormConsulta.Show();
        }

		public static bool CreateTables(out string message)
        {
			bool hasCreated = true;
			message = "";

			try
            {
				if (Tools.Company == null)
				{
					Tools.SetUICompany();
                }
				
				//throw new Exception("Não foi possível carregar informaçõoes de Company da DI-API");
            }
            catch (Exception ex)
            {
				hasCreated = false;
				message = "Não foi possível carregar informaçõoes de Company.";
                //throw new ;
            }

			return hasCreated;
        }
		public static Company Company = null;
		public static UserDataTableNav UserTabNavigator = null;
		public static FormConsultaRegistro FormConsulta;
		public static string TableLookUp = "";
		public static object FormCaller = null;
		//public static object retConsulta = "";
		//seta para o primeiro registro;
		//seta para o registro anterior;
		//seta para o próximo registro;
		//seta para o registro final
		//Propiedade estática com os formulários que poderam ser manipulados pelos eventos de menu da menu bar
		public static string[] ListFormsFRM =
		{
			"FRMLocal",
			"FRMLinha",
			"FRMHorar",
			"FRMViagem"
		};
		//public static SAPbouiCOM.Form ActiveForm = null;		

	}
}