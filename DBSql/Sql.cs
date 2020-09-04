using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace ingeniOracleBackEnd.DBSql {

    /// <summary>
    /// Summary description for Sql
    /// </summary>
    public class Sql {
        public Sql () {
            //
            // TODO: Add constructor logic here
            //
        }
        public static SqlConnection Con () {
            SqlConnection cn = new SqlConnection ("Data Source=192.168.1.7;Initial Catalog=PORTAL_PROD;User Id=sa;Password=Cognos3004252303;Connection Timeout=999999990");
            //SqlConnection cn = new SqlConnection(@"Data Source=192.168.0.14\SQLEXPRESS;Initial Catalog=KPI_KPR;User Id=portal;Password=1q2w3e4r;Connection Timeout=0");
            //SqlConnection cn = new SqlConnection(@"Data Source=192.168.55.6;Initial Catalog=PORTAL_PROD;User Id=sa;Password=P@ssw0rd;Connection Timeout=999999990");
            return cn;
        }
        public static string GetString (DataTable dtInput, int index) {
            try {
                if (dtInput != null && dtInput.Rows.Count > 0) {
                    if (dtInput.Columns.Count > index) {
                        return dtInput.Rows[0][index].ToString ();
                    }
                }
                return null;
            } catch (Exception ex) {
                return null;
            }
        }
        public static string GetString (DataTable dtInput, string name) {
            try {
                if (dtInput != null && dtInput.Rows.Count > 0) {
                    return dtInput.Rows[0][name].ToString ();
                }
                return null;
            } catch (Exception ex) {
                return null;
            }
        }

        // public static SqlDataAdapter GatDataAdapter (string sql) {
        //     DataTable dt = new DataTable ();
        //     SqlDataAdapter da = new SqlDataAdapter (sql, Con ());
        //     SqlCommandBuilder cb = new SqlCommandBuilder (da);
        //     return da;
        // }
        public static DataTable Execute (string sql) {
            DataTable dt = new DataTable ();

            SqlDataAdapter da = new SqlDataAdapter (sql, Con ());
            da.SelectCommand.CommandTimeout = 99999;
            da.Fill (dt);

            return dt;
        }
        public static string ExecuteTran (List<string> lsInput) {
            string[] _str = new string[lsInput.Count];
            for (int i = 0; i < lsInput.Count; i++) {
                _str[i] = lsInput[i];
            }
            return ExecuteTran (_str);
        }
        public static string ExecuteTran (params string[] _sql) {
            SqlTransaction tr;
            SqlConnection cn = Con ();
            cn.Open ();
            tr = cn.BeginTransaction ();
            try {
                int i = 0, intRow = 0;
                SqlCommand cm;
                intRow = _sql.Length;

                for (i = 0; i < intRow; i++) {
                    cm = new SqlCommand ();
                    cm.Connection = cn;
                    cm.CommandTimeout = 900000;
                    cm.Transaction = tr;
                    cm.CommandText = _sql[i];
                    cm.ExecuteNonQuery ();
                }

                tr.Commit ();
                cn.Close ();

                return "T";
            } catch (Exception e) {
                tr.Rollback ();
                cn.Close ();
                return e.Message;
            }
        }
        public static DataTable ExecuteProc (string spName, string[] _paramName = null, object[] _paramValue = null) {
            try {
                DataTable dt = new DataTable ();

                SqlConnection cn = Con ();

                SqlCommand cmd = cn.CreateCommand ();
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (_paramName != null) {
                    for (int i = 0; i < _paramName.Length; i++) {
                        cmd.Parameters.AddWithValue (_paramName[i], _paramValue[i]);
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter (cmd);
                da.Fill (dt);

                return dt;
            } catch (Exception ex) {
                return null;
            }
        }
        public static DataTable ExecuteProc (string spName, string paramName, object paramValue) {
            string[] _str = { paramName };
            object[] _obj = { paramValue };
            return ExecuteProc (spName, _str, _obj);
        }
        public static DataSet ExecuteProcSet (string spName, string[] _paramName = null, object[] _paramValue = null) {
            try {
                DataSet ds = new DataSet ();

                SqlConnection cn = Con ();

                SqlCommand cmd = cn.CreateCommand ();
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (_paramName != null) {
                    for (int i = 0; i < _paramName.Length; i++) {
                        cmd.Parameters.AddWithValue (_paramName[i], _paramValue[i]);
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter (cmd);
                da.Fill (ds);

                return ds;
            } catch (Exception ex) {
                return null;
            }
        }
        public static DataSet ExecuteProcSet (string spName, string paramName, object paramValue) {
            string[] _str = { paramName };
            object[] _obj = { paramValue };
            return ExecuteProcSet (spName, _str, _obj);
        }
        public static bool NotifInsert (string desc, string from, string[] _to, string link, string toolID) {
            try {
                DataTable dt = new DataTable ();
                dt = Sql.Execute ("SELECT IsNull(MAX(NOTIF_ID),0) FROM NOTIF");
                int newID = Convert.ToInt32 (Sql.GetString (dt, 0));

                // SqlDataAdapter da = Sql.GatDataAdapter ("SELECT TOP 1 * FROM NOTIF");
                // da.Fill (dt);

                for (int i = 0; i < _to.Length; i++) {
                    newID++;
                    DataRow dr = dt.NewRow ();
                    dr["NOTIF_ID"] = newID;
                    dr["NOTIF_DESC"] = desc;
                    dr["NOTIF_DATE"] = DateTime.Now;
                    dr["NOTIF_FORM"] = from;
                    dr["NOTIF_TO"] = _to[i];
                    dr["NOTIF_FLAG"] = "F";
                    dr["NOTIF_LINK"] = link;
                    dr["TOOL_ID"] = toolID;
                    dt.Rows.Add (dr);
                }
                // da.Update (dt);
                return true;
            } catch (Exception ex) {
                return false; // ex.Message;
            }
        }
    }
}