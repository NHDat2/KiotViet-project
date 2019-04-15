using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Management;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Diagnostics;


namespace OSM_Backend
{
    public class Connection
    {

        public static string ConnectionString = "";
        public static string ConnectionString1 = "";

        public static object GetValue(string SQL, object DefaultValue, int KetNoi = 0)
        {

            SqlCommand cmd = new SqlCommand(SQL);
            object vR = GetValue(cmd, DefaultValue, KetNoi);
            cmd.Dispose();
            return vR;
        }

        public static object GetValueFromStore(string store, object DefaultValue, params object[] param)
        {
            object iResult = null;
            using (SqlCommand cmd = new SqlCommand(store))
            {
                SqlConnection conn = GetConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                Open(conn);
                cmd.Connection = conn;
                SqlCommandBuilder.DeriveParameters(cmd);
                cmd.CommandType = CommandType.StoredProcedure;
                if (cmd.Parameters.Count > 0)
                {
                    cmd.Parameters.RemoveAt(0);
                    for (int i = 0; i <= cmd.Parameters.Count - 1; i++)
                    {
                        if (param[i] != null)
                        {
                            cmd.Parameters[i].Value = param[i];
                        }
                        else
                        {
                            cmd.Parameters[i].Value = DBNull.Value;
                        }
                    }
                }
                try
                {
                    iResult = cmd.ExecuteScalar();
                    if (iResult == null)
                    {
                        iResult = DefaultValue;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    CloseConectTion(conn);
                }
            }
            return iResult;
        }

        public static String GetValueString(string SQL, object DefaultValue, int KetNoi = 0)
        {
            Object vR = GetValue(SQL, DefaultValue, KetNoi);
            return String.Format("{0}", vR);
        }

        public static object GetValue(SqlCommand cmd, object DefaultValue, int KetNoi = 0)
        {
            object vR = DefaultValue;
            DataTable dt = null;
            DataSet ds = GetDataSet(cmd, KetNoi);
            if (ds != null)
            {
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0] != DBNull.Value)
                    {
                        vR = dt.Rows[0][0];
                    }
                }
                ds.Dispose();
            }
            return vR;
        }

        public static string GetValueString(SqlCommand cmd, string DefaultValue, int KetNoi = 0)
        {
            Object vR = GetValue(cmd, DefaultValue, KetNoi);
            return String.Format("{0}", vR);
        }


        public static DataTable GetDataTable(string SQL, int KetNoi = 0)
        {
            SqlCommand cmd = new SqlCommand(SQL);
            DataTable dtR = GetDataTable(cmd, KetNoi);
            cmd.Dispose();
            return dtR;
        }

        public static SqlConnection GetConnection()
        {
            SqlConnection conn = null;

            String sConnString = ConnectionString;
            conn = new SqlConnection(sConnString);
            return conn;
        }

        public static void CloseConectTion(SqlConnection con)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        public static void Open(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        public static DataSet GetDataTableFromStore(string store, params object[] param)
        {
            SqlDataAdapter adapter = null;
            DataSet dsR = null;
            using (SqlCommand cmd = new SqlCommand(store))
            {
                SqlConnection conn = GetConnection();
                cmd.CommandType = CommandType.StoredProcedure;
                Open(conn);
                cmd.Connection = conn;
                SqlCommandBuilder.DeriveParameters(cmd);
                cmd.CommandType = CommandType.StoredProcedure;
                if (cmd.Parameters.Count > 0)
                {
                    cmd.Parameters.RemoveAt(0);
                    for (int i = 0; i <= cmd.Parameters.Count - 1; i++)
                    {
                        if (param[i] != null)
                        {
                            cmd.Parameters[i].Value = param[i];
                        }
                        else
                        {
                            cmd.Parameters[i].Value = DBNull.Value;
                        }
                    }
                }
                try
                {
                    adapter = new SqlDataAdapter(cmd);
                    dsR = new DataSet();
                    adapter.Fill(dsR);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    CloseConectTion(conn);
                    if (adapter != null) adapter.Dispose();
                }
            }
            return dsR;
        }

        public static DataTable GetDataTable(SqlCommand cmd, int KetNoi = 0)
        {
            DataTable dtR = null;
            DataSet ds = GetDataSet(cmd, KetNoi);
            if (ds != null)
            {
                dtR = ds.Tables[0];
            }
            return dtR;
        }

        public static int GetParameters(ref SqlCommand cmd, int KetNoi = 0)
        {
            int vR = 0;
            String sConnString = (KetNoi == 1) ? ConnectionString1 : ConnectionString;
            SqlConnection conn = null;
            //try
            //{
            conn = new SqlConnection(sConnString);
            conn.Open();
            cmd.Connection = conn;
            vR = cmd.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{
            //    vR = 0;
            //}
            if (conn != null) conn.Dispose();
            return vR;
        }

        public static DataSet GetDataSet(string SQL, int KetNoi = 0)
        {
            SqlCommand cmd = new SqlCommand(SQL);
            DataSet dsR = GetDataSet(cmd, KetNoi);
            cmd.Dispose();
            return dsR;
        }

        public static DataSet GetDataSet(SqlCommand cmd, int KetNoi = 0)
        {
            SqlConnection conn = null;
            SqlDataAdapter adapter = null;
            DataSet dsR = null;

            String sConnString = (KetNoi == 1) ? ConnectionString1 : ConnectionString;
            conn = new SqlConnection(sConnString);
            conn.Open();
            cmd.Connection = conn;
            adapter = new SqlDataAdapter(cmd);
            dsR = new DataSet();

            adapter.Fill(dsR);
            if (conn != null) conn.Dispose();
            if (adapter != null) adapter.Dispose();
            return dsR;
        }

        public static int UpdateDatabase(SqlCommand cmd, int KetNoi = 0)
        {
            int vR = 0;
            SqlConnection conn = null;
            try
            {
                String sConnString = (KetNoi == 1) ? ConnectionString1 : ConnectionString;
                conn = new SqlConnection(sConnString);
                conn.Open();
                cmd.Connection = conn;
                vR = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                vR = 0;
                throw ex;
            }
            if (conn != null) conn.Dispose();
            return vR;
        }

        public static int DeleteRecord(string TableName, string FieldKeyName, object FieldKeyValue, int KetNoi = 0)
        {
            string SQL = "";
            int vR = 0;
            //Start-20140219 bang co nhieu khoa
            String[] strA = FieldKeyName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            String[] strValue = FieldKeyValue.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int count = strA.Length;
            string condition = "";
            if (count >= 1)
            {
                for (int i = 0; i < count; i++)
                {
                    condition += strA[i] + "=@" + strA[i] + " AND ";
                }
                condition = condition.Substring(0, condition.LastIndexOf("AND"));
                FieldKeyName = condition;
            }
            //End-20140219 bang co nhieu khoa


            SQL = String.Format("DELETE FROM {0} WHERE {1};", TableName, FieldKeyName);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SQL;
            if (strValue.Length == count)
            {
                for (int i = 0; i < count; i++)
                {
                    cmd.Parameters.AddWithValue("@" + strA[i].Trim(), strValue[i].Trim());
                }
            }

            vR = UpdateDatabase(cmd, KetNoi);
            return vR;
        }

        public static int InsertRecord(string TableName, SqlCommand cmd, int KetNoi = 0)
        {
            return InsertRecord(TableName, "", cmd, KetNoi);
        }

        public static int InsertRecord(string TableName, string FieldKeyName, SqlCommand cmd, int KetNoi = 0)
        {
            SqlParameterCollection prms = cmd.Parameters;
            string SQL = "", DSTruong = "", DSGiaTri = "";
            int i, vR = 0;

            for (i = 0; i < prms.Count; i++)
            {
                if (prms[i].ParameterName != "@" + FieldKeyName)
                {
                    string ParamName = prms[i].ParameterName;
                    string FieldName = ParamName.Substring(1);
                    DSGiaTri += ((DSGiaTri == "") ? "" : ",") + ParamName;
                    DSTruong += ((DSTruong == "") ? "" : ",") + FieldName;
                }
            }
            if (DSTruong != "")
            {
                SQL = String.Format("INSERT INTO {0}({1}) VALUES({2});", TableName, DSTruong, DSGiaTri);
                cmd.CommandText = SQL;
                try
                {
                    vR = UpdateDatabase(cmd, KetNoi);
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return vR;
        }

        public static int UpdateRecord(string TableName, string FieldKeyName, SqlCommand cmd, int KetNoi = 0)
        {
            //start-xu ly trong truong hop co nhieu khoa chinh
            SqlParameterCollection prms = cmd.Parameters;
            String[] strA = FieldKeyName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int count = strA.Length;
            string SQL = "", str = "", KeyParameterName = "";
            string Condition = "";
            if (count > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    KeyParameterName += "@" + strA[i].ToUpper();
                    Condition += strA[i] + "=@" + strA[i] + " AND ";
                }
                Condition = Condition.Substring(0, Condition.LastIndexOf("AND"));
            }
            //end-xu ly trong truong hop co nhieu khoa chinh

            //start-co 1 khoa chinh
            if (count == 1)
            {
                KeyParameterName = "@" + FieldKeyName.ToUpper();
                Condition = FieldKeyName + "=@" + FieldKeyName;
            }
            //end- co 1 khoa chinh

            int vR = 0;
            //object FieldKeyValue = prms["@" + FieldKeyName].Value;

            for (int i = 0; i < prms.Count; i++)
            {
                if (KeyParameterName.IndexOf(prms[i].ParameterName.ToUpper()) == -1)
                {
                    string ParamName = prms[i].ParameterName;
                    string FieldName = ParamName.Substring(1);
                    str += ((str == "") ? "" : ",") + FieldName + "=" + ParamName;
                }
            }
            if (str != "")
            {
                SQL = String.Format("UPDATE {0} SET {1} WHERE {2};", TableName, str, Condition);
                cmd.CommandText = SQL;
                vR = UpdateDatabase(cmd, KetNoi);
            }
            return vR;
        }

        public static int InsertOrUpdateRecord(string TableName, string FieldKeyName, SqlCommand cmd, int KetNoi = 0)
        {
            int vR = 0;
            Boolean ok = true;
            if (cmd.Parameters.IndexOf("@" + FieldKeyName) >= 0)
            {
                object FieldKeyValue = cmd.Parameters["@" + FieldKeyName].Value;
                if (String.IsNullOrEmpty((string)(FieldKeyValue)) == false)
                {
                    ok = false;
                }
            }
            if (ok)
            {
                vR = InsertRecord(TableName, FieldKeyName, cmd, KetNoi);
            }
            else
            {
                vR = UpdateRecord(TableName, FieldKeyName, cmd, KetNoi);
            }
            return vR;
        }
    }
}
