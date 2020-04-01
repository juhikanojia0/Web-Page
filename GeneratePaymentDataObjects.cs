/*************************************************************
* Project   : CIFMIS_GeneratePayment Web Portal for E10
* Author    : Mahesh D. Deore
* Date      : 10 Dec 2015
* Purpose   : to get database connection
* Version   : 1.0.0.0
* 
* Version   Modified By        Modified Date   Purpose
* --------  ------------------ --------------- --------------------------------------------------- 
* 1.0.0.1   Mahesh Deore       16-Jun-2016     1) Added company code in dropdown.
* 1.0.0.2   Shekhar Chaudhari  19-Mar-2018     Incorported 10.1.600.18 changes in 10.2.100.9 ver.
*                                              1) Resolved VSO Bug No :-13046  Generate Payment 
                                                  Web portal users should not be mandatory 
											      to have Ministries access rights in Epicor.  
* 1.0.0.3   Shekhar Chaudhari  14-Jan-2019     Task ID 21080:- Incorporate SiteID Chages 
*                                              related to Cost Center Segregation in GPW .
**************************************************************************************************/
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Stcl.Global.GlobalMethods;

namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.DataAccessLayer
{
    public class GeneratePaymentDataObjects
    {
        BusinessAccessLayer.GeneratePaymentBusinessObjects objBo = new BusinessAccessLayer.GeneratePaymentBusinessObjects();
        EventLog ObjEvent = new EventLog();

        public DataSet CheckLogin(string userId, string password)
        {
            string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection conn = new SqlConnection(connetionstring);
            SqlCommand cmddata = new SqlCommand();
            cmddata.CommandTimeout = 0;
            try
            {
                conn.Open();
                cmddata = new SqlCommand("CsApprovalProcess", conn);
                cmddata.CommandType = CommandType.StoredProcedure;
                cmddata.Parameters.AddWithValue("@UserId", userId);
                cmddata.Parameters.AddWithValue("@Password", password);
                using (SqlDataAdapter da = new SqlDataAdapter(cmddata))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects =>CheckLogin ", ex.Message.ToString(), ex);
                DataSet ds = new DataSet();
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataSet GetPayGroupID(string company, string sourceCompany, Boolean isSalaryPay)
        {
            string ConnetionString = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection Conn = new SqlConnection(ConnetionString);
            SqlCommand CmdData = new SqlCommand();
            CmdData.CommandTimeout = 0;
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                CmdData = new SqlCommand("Stcl_CIFMIS_Global_GetPayGroupId", Conn);
                CmdData.CommandType = CommandType.StoredProcedure;
                CmdData.Parameters.AddWithValue("@company", company);
                CmdData.Parameters.AddWithValue("@glbCompany", sourceCompany);
                CmdData.Parameters.AddWithValue("@isSalaryPay", isSalaryPay);
                using (SqlDataAdapter Da = new SqlDataAdapter(CmdData))
                {
                    DataSet Ds = new DataSet();
                    Da.Fill(Ds);
                    return Ds;
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects => GetPayGroupID ", ex.Message.ToString(), ex);
                DataSet Ds = new DataSet();
                return Ds;
            }
            finally
            {
                Conn.Close();
            }
        }

        public DataSet GetGeneratePaymentData(string sourceCompany, string generator, string bankAcctID, string srcGroupId, string vendorID, string legalNumber, Boolean isSalaryPay, string payRefNo)
        {
            string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection conn = new SqlConnection(connetionstring);
            SqlCommand cmddata = new SqlCommand();
            cmddata.CommandTimeout = 0;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmddata = new SqlCommand("Stcl_CIFMIS_Global_GetDataForGeneratePayment", conn);
                cmddata.CommandType = CommandType.StoredProcedure;
                cmddata.Parameters.AddWithValue("@sourceCompany", sourceCompany);
                cmddata.Parameters.AddWithValue("@generator", generator);
                cmddata.Parameters.AddWithValue("@bankAcctID", bankAcctID);
                cmddata.Parameters.AddWithValue("@srcGroupId", srcGroupId);
                cmddata.Parameters.AddWithValue("@vendorID", vendorID);
                cmddata.Parameters.AddWithValue("@legalNumber", legalNumber);
                cmddata.Parameters.AddWithValue("@isSalaryPay", isSalaryPay);
                cmddata.Parameters.AddWithValue("@payRefNo", payRefNo);
                using (SqlDataAdapter da = new SqlDataAdapter(cmddata))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects =>GetGeneratePaymentData ", ex.Message.ToString(), ex);
                DataSet ds = new DataSet();
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataSet GetConsolidatedGeneratePaymentData(string PaymentGeneratorUserId, string PaymentCompanyId, string BankAccId, Boolean isSalaryPay)
        {
            string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection conn = new SqlConnection(connetionstring);
            SqlCommand cmddata = new SqlCommand();
            cmddata.CommandTimeout = 0;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmddata = new SqlCommand("Stcl_CIFMIS_Global_ConsolidatedGeneratePaymentData", conn);
                cmddata.CommandType = CommandType.StoredProcedure;
                cmddata.Parameters.AddWithValue("@User", PaymentGeneratorUserId);
                cmddata.Parameters.AddWithValue("@CompanyId", PaymentCompanyId);
                cmddata.Parameters.AddWithValue("@BankAccId", BankAccId);
                cmddata.Parameters.AddWithValue("@isSalaryPay", isSalaryPay);

                using (SqlDataAdapter da = new SqlDataAdapter(cmddata))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects =>GetConsolidatedGeneratePaymentData ", ex.Message.ToString(), ex);
                DataSet Ds = new DataSet();
                return Ds;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataSet GetGlobalData(string code)
        {
            string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection conn = new SqlConnection(connetionstring);
            SqlCommand cmddata = new SqlCommand();
            cmddata.CommandTimeout = 0;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmddata = new SqlCommand("Stcl_CIFMIS_Global_GetGlobalData", conn);
                cmddata.CommandType = CommandType.StoredProcedure;
                cmddata.Parameters.AddWithValue("@Code", code);
                using (SqlDataAdapter da = new SqlDataAdapter(cmddata))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects =>GetGlobalData ", ex.Message.ToString(), ex);
                DataSet ds = new DataSet();
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataSet ValidateAndGetData(Int16 option, string treasuryCompany, Int32 vendorNum, string bankAcctID, string sourceCompany, string generator, string srcGroupId, string invoiceNum, decimal invoiceAmt, Boolean isSalaryPay, string payRefNo)
        {
            string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection conn = new SqlConnection(connetionstring);
            SqlCommand cmddata = new SqlCommand();
            cmddata.CommandTimeout = 0;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmddata = new SqlCommand("Stcl_CIFMIS_Global_ValidateGeneratePaymentData", conn);
                cmddata.CommandType = CommandType.StoredProcedure;
                cmddata.Parameters.AddWithValue("@option", option);
                cmddata.Parameters.AddWithValue("@company", treasuryCompany);
                cmddata.Parameters.AddWithValue("@bankAcctID", bankAcctID);
                cmddata.Parameters.AddWithValue("@srcCompany", sourceCompany);
                cmddata.Parameters.AddWithValue("@generator", generator);
                cmddata.Parameters.AddWithValue("@srcGroupId", srcGroupId);
                cmddata.Parameters.AddWithValue("@invoiceNum", invoiceNum);
                cmddata.Parameters.AddWithValue("@invoiceAmt", invoiceAmt);
                cmddata.Parameters.AddWithValue("@vendorNum", vendorNum);
                cmddata.Parameters.AddWithValue("@isSalaryPay", isSalaryPay);
                using (SqlDataAdapter da = new SqlDataAdapter(cmddata))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects=>ValidateAndGetData ", ex.Message.ToString(), ex);
                DataSet ds = new DataSet();
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataSet GetCashInfo(string company, string bankAcctID, string glAccount, decimal invoiceAmt)
        {
            string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection conn = new SqlConnection(connetionstring);
            SqlCommand cmddata = new SqlCommand();
            cmddata.CommandTimeout = 0;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmddata = new SqlCommand("Stcl_CIFMIS_Global_GetCashInfoForPayment", conn);
                cmddata.CommandType = CommandType.StoredProcedure;
                cmddata.Parameters.AddWithValue("@company", company);
                cmddata.Parameters.AddWithValue("@bankAcctID", bankAcctID);
                cmddata.Parameters.AddWithValue("@glAccount", glAccount);
                cmddata.Parameters.AddWithValue("@invoiceAmt", invoiceAmt);
                using (SqlDataAdapter da = new SqlDataAdapter(cmddata))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects=>GetCashInfo ", ex.Message.ToString(), ex);
                DataSet ds = new DataSet();
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataSet ValidateSingleSignOn(string userName, string domainName)
        {
            string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection conn = new SqlConnection(connetionstring);
            SqlCommand cmddata = new SqlCommand();
            cmddata.CommandTimeout = 0;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmddata = new SqlCommand(string.Format("SELECT DcdUserID,Password,RequireSso FROM ERP.UserFile WITH(NOLOCK) WHERE DcdUserID ='{0}' AND RequireSso = 1 AND UserDisabled = 0 AND DomainName ='{1}'", userName, domainName), conn);

                cmddata.CommandType = CommandType.Text;

                using (SqlDataAdapter da = new SqlDataAdapter(cmddata))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects => ValidateSingleSignOn ", ex.Message.ToString(), ex);
                DataSet ds = new DataSet();
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataSet GetUserCompany(string UsrId)
        {
            string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection conn = new SqlConnection(connetionstring);
            SqlCommand cmddata = new SqlCommand();
            cmddata.CommandTimeout = 0;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmddata = new SqlCommand("Stcl_CIFMIS_Global_GetCPOUserCompanyList", conn);
                cmddata.CommandType = CommandType.StoredProcedure;
                cmddata.Parameters.AddWithValue("@userId", UsrId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmddata))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects => GetUserCompany ", ex.Message.ToString(), ex);
                DataSet ds = new DataSet();
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }

        public string LoginDomain(string loginID)
        {
            string s = loginID;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(0, stop) : string.Empty;
        }

        public string LoginIDNoDomain(string loginID)
        {
            if (loginID.IndexOf("\\") >= 0)
            {
                loginID = loginID.Substring(loginID.IndexOf("\\") + 1);
            }

            return loginID;
        }

        public string GetSysParam(string code)
        {
            var ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cons"].ConnectionString;
            SqlConnection Con = new SqlConnection(ConnectionString.ToString());
            SqlCommand Cmd = new SqlCommand();

            try
            {
                Con.Open();
                Cmd.CommandText = "Stcl_CIFMIS_Global_GetGlobalData";
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Connection = Con;
                Cmd.Parameters.AddWithValue("@code", code);
                SqlDataAdapter Da = new SqlDataAdapter(Cmd);
                DataSet Ds = new DataSet();
                Da.Fill(Ds);
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }

                if (Ds != null)
                {
                    if (Ds.Tables.Count > 0)
                    {
                        if (Ds.Tables[0].Rows.Count > 0)
                        {
                            return Ds.Tables[0].Rows[0][1].ToString();
                        }
                        else
                        {
                            return "";
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects => GetSysParam ", ex.Message.ToString(), ex);
                DataSet ds = new DataSet();
                return "";
            }
            finally
            {
                Con.Close();
            }
        }

        public Boolean CheckCPOUserYN(string UserId)
        {
            DataAccessLayer.GeneratePaymentDataObjects ObjDo = new DataAccessLayer.GeneratePaymentDataObjects();

            DataSet DsRights = new DataSet();
            DsRights = GetUserCompany(UserId);
            if (DsRights.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHeadNum(string company, string groupId, int vendorNum, Boolean isSalaryPay)
        {
            string ConnetionString = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
            SqlConnection Conn = new SqlConnection(ConnetionString);
            SqlCommand CmdData = new SqlCommand();
            CmdData.CommandTimeout = 0;
            int headNum = 0;
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                CmdData = new SqlCommand("Stcl_CIFMIS_Global_GetHeadNum", Conn);
                CmdData.CommandType = CommandType.StoredProcedure;
                CmdData.Parameters.AddWithValue("@company", company);
                CmdData.Parameters.AddWithValue("@groupID", groupId);
                CmdData.Parameters.AddWithValue("@vendorNum", vendorNum);
                CmdData.Parameters.AddWithValue("@isSalaryPay", isSalaryPay);
                using (SqlDataAdapter Da = new SqlDataAdapter(CmdData))
                {
                    DataSet Ds = new DataSet();
                    Da.Fill(Ds);

                    headNum = Convert.ToInt32(Ds.Tables[0].Rows[0]["HeadNum"]);
                }
                return headNum;
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects => GetHeadNum ", ex.Message.ToString(), ex);
                DataSet Ds = new DataSet();
                return 0;
            }
            finally
            {
                Conn.Close();
            }
        }

        internal string GetPrimaryBank(string compId, int VendorNum, string InvoiceNum)
        {
            var ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cons"].ConnectionString;
            SqlConnection Con = new SqlConnection(ConnectionString.ToString());
            SqlCommand Cmd = new SqlCommand();
            try
            {
                Con.Open();
                string sql = @"SELECT CASE WHEN(API.PymtBank_c = '') THEN V.PrimaryBankID ELSE API.PymtBank_c END AS PrimaryBankId FROM APInvHed API WITH (NOLOCK) INNER JOIN Vendor V WITH (NOLOCK)  ON API.COMPANY = V.Company AND API.VendorNum = V.VendorNum where API.Company = '" + compId + "' AND API.VendorNum = " + VendorNum + "  AND API.InvoiceNum = '" + InvoiceNum + "'  AND API.OpenPayable = 1 AND API.DebitMemo = 0 AND API.Posted = 1";
                string PBankId = string.Empty;
                Cmd.CommandText = sql;
                Cmd.Connection = Con;
                SqlDataAdapter Da = new SqlDataAdapter(Cmd);
                DataSet Ds = new DataSet();
                Da.Fill(Ds);

                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                if (Ds != null)
                {
                    if (Ds.Tables.Count > 0)
                    {
                        if (Ds.Tables[0].Rows.Count > 0)
                        {
                            PBankId = Ds.Tables[0].Rows[0][0].ToString();
                        }
                        else
                        {
                            PBankId = "";
                        }
                    }
                    else
                    {
                        PBankId = "";
                    }
                }
                else
                {
                    PBankId = "";
                }

                return PBankId;
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects => GetPrimaryBank ", ex.Message.ToString(), ex);
                DataSet Ds = new DataSet();
                return "";
            }
            finally
            {
                Con.Close();
            }
        }

        internal string GetPrimaryBankOrig(string compId, int VendorNum)
        {
            var ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cons"].ConnectionString;
            SqlConnection Con = new SqlConnection(ConnectionString.ToString());
            SqlCommand Cmd = new SqlCommand();
            try
            {
                Con.Open();
                string sql = @"SELECT V.PrimaryBankID  AS PrimaryBankId FROM Vendor V WITH (NOLOCK) where V.Company = '" + compId + "' AND V.VendorNum = " + VendorNum + "";
                string PBankId = string.Empty;
                Cmd.CommandText = sql;
                Cmd.Connection = Con;
                SqlDataAdapter Da = new SqlDataAdapter(Cmd);
                DataSet Ds = new DataSet();

                Da.Fill(Ds);
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                if (Ds != null)
                {
                    if (Ds.Tables.Count > 0)
                    {
                        if (Ds.Tables[0].Rows.Count > 0)
                        {

                            PBankId = Ds.Tables[0].Rows[0][0].ToString();
                        }
                        else
                        {
                            PBankId = "";
                        }
                    }
                    else
                    {
                        PBankId = "";
                    }
                }
                else
                {
                    PBankId = "";
                }
                return PBankId;
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects => GetPrimaryBankOrig ", ex.Message.ToString(), ex);
                DataSet Ds = new DataSet();
                return "";
            }
            finally
            {
                Con.Close();
            }
        }

        #region GetSiteId
        internal string GetScalarValue(string sqlQry)
        {
            var ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cons"].ConnectionString;
            SqlConnection Con = new SqlConnection(ConnectionString.ToString());
            SqlCommand Cmd = new SqlCommand();

            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }              
                Cmd.CommandText = sqlQry;
                Cmd.Connection = Con;
                string value = Convert.ToString(Cmd.ExecuteScalar());               
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                return value;
            }
            catch (Exception ex)
            {
                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentDataObjects => GetScalarValue ", ex.Message.ToString(), ex);
                DataSet Ds = new DataSet();
                return "";
            }
            finally
            {
                Con.Close();
                Cmd.Dispose();
            }
        }
        #endregion
    }
}