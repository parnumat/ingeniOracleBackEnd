using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Persistence.DataTracker;
using testAspOracle01.persistence.Helpers;
// using Persistence.Data;
// using Persistence.DataTracker;
// using Persistence.Helpers;
// using Persistence.Models;
// using Persistence.SignalR;

namespace testAspOracle01 {
    public class DataContext {
        private readonly IMapper _mapper;
        public DataContext (IMapper mapper) => _mapper = mapper;

        public DataContext () { }

        /// <summary>
        /// ดึงข้อมูลจาก Database ด้วย ADO แบบ Synchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="query"></param>
        /// <param name="tableName"></param>
        public Task<DataTable> GetResultADOAsync (User user, DataBaseHostEnum databasehost, string query, string tableName) {
            DateTime dtStart = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();

            DataSet ds = new DataSet ();
            return Task<DataTable>.Factory.StartNew (() => {
                OracleConnection instance = ConnectionFactory.GetDatabaseInstanceByHost (databasehost);
                OracleDataAdapter da = new OracleDataAdapter (query, instance);
                da.Fill (ds);
                ds.Tables[0].TableName = tableName;

                DateTime dtEnd = DateTime.Now;
                stopwatch.Stop ();
                string Header = dtStart.ToString () + " => GetResultDapperAsync" + " DataBase Host : " + databasehost.ToString ();
                WebApiTracking.SendToSignalRWithTimeUsed (user, Header, query, string.Empty, stopwatch, dtStart, dtEnd);
                return ds.Tables[0].Copy ();
            });

        }

        /// <summary>
        /// ดึงข้อมูลจาก Database ด้วย Dapper ORM แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="query"></param>
        /// <param name="dynamic parameter"></param>
        public async Task<Object> GetResultDapperAsync (User user, DataBaseHostEnum databasehost, string query, dynamic param) {
            DateTime dtStart = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();

            Type type = param.GetType ();
            PropertyInfo[] propertiesInfo = type.GetProperties ();
            string parameterText = string.Empty;
            foreach (PropertyInfo p in propertiesInfo) {
                string propName = p.Name;
                object value = p.GetValue (param, null);
                string propValue = (value == null) ? string.Empty : value.ToString ();
                parameterText = parameterText + System.Environment.NewLine + "</br>" + propName + " : " + propValue;
            }

            OracleConnection instance = ConnectionFactory.GetDatabaseInstanceByHost (databasehost);
            var parameter = new DynamicParameters (param);
            var result = await SqlMapper.QueryAsync (instance, query, parameter);
            // var result = await SqlMapper.QueryAsync(instance, query, commandType: CommandType.Text);

            DateTime dtEnd = DateTime.Now;
            stopwatch.Stop ();

            if (!string.IsNullOrEmpty (parameterText)) {
                parameterText = System.Environment.NewLine + "</br></br>" + "Record Count : " + result.Count () +
                    System.Environment.NewLine + "</br></br>" + "Parameter List " + parameterText;
            }

            string Header = dtStart.ToString () + " => GetResultDapperAsync" + " DataBase Host : " + databasehost.ToString ();
            await WebApiTracking.SendToSignalRWithTimeUsedAsync (user, Header, query, parameterText, stopwatch, dtStart, dtEnd);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database ด้วย Dapper ORM แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="query"></param>
        /// <param name="Dapper dynamic parameter"></param>
        public async Task<Object> GetResultDapperWithDapperParmAsync (User user, DataBaseHostEnum databasehost, string query, DynamicParameters dapperParam) {
            DateTime dtStart = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();

            OracleConnection instance = ConnectionFactory.GetDatabaseInstanceByHost (databasehost);
            var result = await SqlMapper.QueryAsync (instance, query, dapperParam);
            // var result = await SqlMapper.QueryAsync(instance, query, commandType: CommandType.Text);

            DateTime dtEnd = DateTime.Now;
            stopwatch.Stop ();
            var parameterText = string.Empty;

            foreach (var paramName in dapperParam.ParameterNames) {
                parameterText = parameterText + System.Environment.NewLine + "</br>" + paramName + " : " + dapperParam.Get<dynamic> (paramName);
            }

            if (!string.IsNullOrEmpty (parameterText)) {
                parameterText = System.Environment.NewLine + "</br></br>" + "Record Count : " + result.Count () +
                    System.Environment.NewLine + "</br></br>" + "Parameter List " + parameterText;
            }

            string Header = dtStart.ToString () + " => GetResultDapperAsync" + " DataBase Host : " + databasehost.ToString ();
            await WebApiTracking.SendToSignalRWithTimeUsedAsync (user, Header, query, parameterText, stopwatch, dtStart, dtEnd);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database ด้วย Dapper ORM แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="query"></param>
        public async Task<Object> GetResultDapperAsync (User user, DataBaseHostEnum databasehost, string query) {
            DateTime dtStart = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();

            OracleConnection instance = ConnectionFactory.GetDatabaseInstanceByHost (databasehost);
            var result = await SqlMapper.QueryAsync (instance, query, commandType : CommandType.Text);

            DateTime dtEnd = DateTime.Now;
            stopwatch.Stop ();

            string Header = dtStart.ToString () + " => GetResultDapperAsync" + " DataBase Host : " + databasehost.ToString ();
            string footer = System.Environment.NewLine + "</br></br>" + "Record Count : " + result.Count ();
            await WebApiTracking.SendToSignalRWithTimeUsedAsync (user, Header, query, footer, stopwatch, dtStart, dtEnd);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database OPPN ด้วย Dapper ORM แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="query"></param>
        public async Task<Object> GetResultDapperAsync (User user, string query) {
            var result = await GetResultDapperAsync (user, DataContextConfiguration.DEFAULT_DATABASE, query);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="query"></param>
        public async Task<Object> GetResultAsync (User user, DataBaseHostEnum databasehost, string query) {
            var result = await GetResultDapperAsync (user, databasehost, query);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database OPPN แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="query"></param>
        public async Task<Object> GetResultAsync (User user, string query) {
            var result = await GetResultDapperAsync (user, query);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="query"></param>
        /// <param name="dynamic parameter"></param>
        public async Task<Object> GetResultAsync (User user, DataBaseHostEnum databasehost, string query, dynamic param) {
            var result = await GetResultDapperAsync (user, databasehost, query, param);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="query"></param>
        /// <param name="dynamic parameter"></param>
        public async Task<Object> GetResultAsync (User user, string query, dynamic param) {
            object result = null;
            if (param == null)
                result = await GetResultDapperAsync (user, DataContextConfiguration.DEFAULT_DATABASE, query);
            else
                result = await GetResultDapperAsync (user, DataContextConfiguration.DEFAULT_DATABASE, query, param);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database ด้วย Dapper ORM แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="query"></param>
        /// <param name="dynamic parameter"></param>
        public async Task<int> ExecuteDapperAsync (User user, DataBaseHostEnum databasehost, string query, dynamic param) {
            DateTime dtStart = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();

            Type type = param.GetType ();
            PropertyInfo[] propertiesInfo = type.GetProperties ();
            string parameterText = string.Empty;
            foreach (PropertyInfo p in propertiesInfo) {
                string propName = p.Name;
                object value = p.GetValue (param, null);
                string propValue = (value == null) ? string.Empty : value.ToString ();
                parameterText = parameterText + System.Environment.NewLine + "</br>" + propName + " : " + propValue;
            }

            OracleConnection instance = ConnectionFactory.GetDatabaseInstanceByHost (databasehost);
            var parameter = new DynamicParameters (param);
            var result = SqlMapper.ExecuteAsync (instance, query, parameter).Result;

            DateTime dtEnd = DateTime.Now;
            stopwatch.Stop ();

            if (!string.IsNullOrEmpty (parameterText)) {
                parameterText = System.Environment.NewLine + "</br></br>" + "Result : " + (result == 0) +
                    System.Environment.NewLine + "</br></br>" + "Parameter List " + parameterText;
            }

            string Header = dtStart.ToString () + " => ExecuteDapperAsync" + " DataBase Host : " + databasehost.ToString ();
            await WebApiTracking.SendToSignalRWithTimeUsedAsync (user, Header, query, parameterText, stopwatch, dtStart, dtEnd);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database ด้วย Dapper ORM แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="query"></param>
        public async Task<int> ExecuteDapperAsync (User user, DataBaseHostEnum databasehost, string query) {
            DateTime dtStart = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();

            OracleConnection instance = ConnectionFactory.GetDatabaseInstanceByHost (databasehost);
            var result = SqlMapper.ExecuteAsync (instance, query).Result;

            DateTime dtEnd = DateTime.Now;
            stopwatch.Stop ();

            var parameterText = System.Environment.NewLine + "</br></br>" + "Result : " + (result == 0);

            string Header = dtStart.ToString () + " => ExecuteDapperAsync" + " DataBase Host : " + databasehost.ToString ();
            await WebApiTracking.SendToSignalRWithTimeUsedAsync (user, Header, query, parameterText, stopwatch, dtStart, dtEnd);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database ด้วย Dapper ORM แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="queries"></param>
        public async Task<int> ExecuteDapperMultiAsync (User user, DataBaseHostEnum databasehost, List<string> queries) {
            DateTime dtStart = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();

            int rowEffect = 0;

            using (var tran = new TransactionScope (TransactionScopeAsyncFlowOption.Enabled))
            using (var instance = ConnectionFactory.GetNewInstance (DataContextConfiguration.DEFAULT_DATABASE)) {
                foreach (var query in queries) {
                    var result = await SqlMapper.ExecuteAsync (instance, query);
                    if (result < 0) {
                        tran.Complete ();
                        return result;
                    }

                    rowEffect = rowEffect + result;
                }
                tran.Complete ();
            }

            DateTime dtEnd = DateTime.Now;
            stopwatch.Stop ();
            var parameterText = System.Environment.NewLine + "</br></br>" + "Result : " + rowEffect + " rows updated";
            string Header = dtStart.ToString () + " => ExecuteDapperAsync" + " DataBase Host : " + databasehost.ToString ();
            await WebApiTracking.SendToSignalRWithTimeUsedAsync (user, Header, string.Join ("</br>", queries), parameterText, stopwatch, dtStart, dtEnd);
            return rowEffect;
        }

        /// <summary>
        /// ดึงข้อมูลจาก Database ด้วย ADO แบบ Synchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="selectStrings"></param>
        /// <param name="err">out</param>
        public DataSet ExecuteReader (User user, DataBaseHostEnum databasehost, string[] selectStrings, ref string err) {
            try {
                // WebApiTracking.SendToSignalR(DateTime.Now.ToString() + " => ExecuteReader" + " DataBase Host : " + databasehost.ToString(), TrackingTypeEnum.HEADER);

                var databaseInstance = ConnectionFactory.GetDatabaseInstanceByHost (databasehost);
                DataSet dsOutput = new DataSet ();
                OracleDataAdapter[] da = new OracleDataAdapter[selectStrings.Length];

                for (int i = 0; i < selectStrings.Length; i++) {
                    // WebApiTracking.SendToSignalR(selectStrings[i]);
                    da[i] = new OracleDataAdapter (selectStrings[i], databaseInstance);
                    OracleCommandBuilder cb = new OracleCommandBuilder (da[i]);
                    da[i].InsertCommand = cb.GetInsertCommand ();
                    da[i].UpdateCommand = cb.GetUpdateCommand ();
                    da[i].DeleteCommand = cb.GetDeleteCommand ();
                    da[i].Fill (dsOutput, "Table " + i.ToString ());
                }
                return dsOutput;
            } catch (Exception ex) {
                err = ex.Message;
                // WebApiTracking.SendToSignalR(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// บันทึกข้อมูลลง Database ด้วย ADO แบบ Synchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="dsInput"></param>
        /// <param name="selectStrings"></param>
        /// <param name="err">out</param>
        public bool ExecuteUpdate (User user, DataBaseHostEnum databasehost, string[] selectStrings, DataSet dsInput, ref string err) {
            DateTime dtStart = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();

            DataSet dsOutput = new DataSet ();
            var databaseInstance = ConnectionFactory.GetNewDatabaseInstanceByHost (databasehost);
            OracleDataAdapter[] da = new OracleDataAdapter[dsInput.Tables.Count];
            for (int i = 0; i < selectStrings.Length; i++) {
                da[i] = new OracleDataAdapter (selectStrings[i], databaseInstance);
                OracleCommandBuilder cb = new OracleCommandBuilder (da[i]);
                da[i].InsertCommand = cb.GetInsertCommand ();
                da[i].UpdateCommand = cb.GetUpdateCommand ();
                da[i].DeleteCommand = cb.GetDeleteCommand ();
                da[i].Fill (dsOutput, "Table " + i.ToString ());
            }

            OracleTransaction trans = databaseInstance.BeginTransaction ();
            try {
                for (int i = 0; i < dsInput.Tables.Count; i++) {
                    da[i].InsertCommand.Transaction = trans;
                    da[i].UpdateCommand.Transaction = trans;
                    da[i].DeleteCommand.Transaction = trans;
                    da[i].Update (dsInput.Tables[i]);
                }
                trans.Commit ();
                databaseInstance.Close ();
                DateTime dtEnd = DateTime.Now;
                stopwatch.Stop ();
                return true;
            } catch (Exception ex) {
                trans.Rollback ();
                databaseInstance.Close ();
                err = ex.Message;
                DateTime dtEnd = DateTime.Now;
                stopwatch.Stop ();
                // WebApiTracking.SendToSignalR(dtStart.ToString() + " => ExecuteUpdate" + " DataBase Host : " + databasehost.ToString(), TrackingTypeEnum.HEADER);
                // WebApiTracking.SendToSignalRWithTimeUsed("result: False => " + err, stopwatch, dtStart, dtEnd, TrackingTypeEnum.ERROR);
                return false;
            }

        }

        /// <summary>
        /// บันทึกข้อมูลลง Database ด้วย ADO แบบ Synchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="databasehost"></param>
        /// <param name="dsInput"></param>
        /// <param name="selectStrings"></param>
        /// <param name="err">out</param>
        public Task<string> ExecuteUpdateAsync (User user, DataBaseHostEnum databasehost, string[] selectStrings, DataSet dsInput) {
            DateTime dtStart = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch ();
            stopwatch.Start ();
            return Task<string>.Factory.StartNew (() => {
                string err = string.Empty;
                DataSet dsOutput = new DataSet ();
                var databaseInstance = ConnectionFactory.GetNewDatabaseInstanceByHost (databasehost);
                OracleDataAdapter[] da = new OracleDataAdapter[dsInput.Tables.Count];
                for (int i = 0; i < selectStrings.Length; i++) {
                    da[i] = new OracleDataAdapter (selectStrings[i], databaseInstance);
                    OracleCommandBuilder cb = new OracleCommandBuilder (da[i]);
                    da[i].InsertCommand = cb.GetInsertCommand ();
                    da[i].UpdateCommand = cb.GetUpdateCommand ();
                    da[i].DeleteCommand = cb.GetDeleteCommand ();
                    da[i].Fill (dsOutput, "Table " + i.ToString ());
                }

                OracleTransaction trans = databaseInstance.BeginTransaction ();
                try {
                    for (int i = 0; i < dsInput.Tables.Count; i++) {
                        da[i].InsertCommand.Transaction = trans;
                        da[i].UpdateCommand.Transaction = trans;
                        da[i].DeleteCommand.Transaction = trans;
                        da[i].Update (dsInput.Tables[i]);
                    }
                    trans.Commit ();
                    databaseInstance.Close ();
                    DateTime dtEnd = DateTime.Now;
                    stopwatch.Stop ();

                    string Header = dtStart.ToString () + " => ExecuteUpdateAsync" + " DataBase Host : " + databasehost.ToString ();
                    string query = string.Join ("</br></br>", selectStrings);
                    string footer = System.Environment.NewLine + "</br></br>" + "Result: Success";
                    WebApiTracking.SendToSignalRWithTimeUsed (user, Header, query, footer, stopwatch, dtStart, dtEnd);
                    return "OK";
                } catch (Exception ex) {
                    trans.Rollback ();
                    databaseInstance.Close ();
                    err = ex.Message;
                    DateTime dtEnd = DateTime.Now;
                    stopwatch.Stop ();
                    string Header = dtStart.ToString () + " => ExecuteUpdateAsync" + " DataBase Host : " + databasehost.ToString ();
                    string query = string.Join ("</br></br>", selectStrings);
                    string footer = System.Environment.NewLine + "</br></br>" + "Result: Fail";
                    WebApiTracking.SendToSignalRWithTimeUsed (user, Header, query, footer, stopwatch, dtStart, dtEnd);
                    return err;
                }
            });
        }

        /// <summary>
        /// ดึง SQL Statment จาก SQL TAB แบบ Synchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="appId"></param>
        /// <param name="sqlNo"></param>
        public async Task<string> GetSqlStmtFromSqlTab (User user, int appId, int sqlNo) {
            var callSqlTab = "SELECT sql_stmt FROM KPDBA.SQL_TAB_OPPN WHERE APP_ID = " + appId + " and SQL_NO = " + sqlNo;
            var rawData = await GetResultAsync (user, callSqlTab);
            var results =
                _mapper.Map<IEnumerable<SqlTab>>
                (rawData).ToList ();

            if (results.Count > 0) {
                return results[0].SQL_STMT as string;
            }
            return string.Empty;
        }

        /// <summary>
        /// ดึง SQL Statment จาก SQL TAB แบบ Synchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="appId"></param>
        /// <param name="sqlNo"></param>
        public async Task<string> GetSqlStmtFromSqlTab (User user, DataBaseHostEnum dbHost, int appId, int sqlNo) {
            var callSqlTab = "SELECT sql_stmt FROM KPDBA.SQL_TAB_OPPN WHERE APP_ID = " + appId + " and SQL_NO = " + sqlNo;
            var rawData = await GetResultAsync (user, dbHost, callSqlTab);
            var results =
                _mapper.Map<IEnumerable<SqlTab>>
                (rawData).ToList ();

            if (results.Count > 0) {
                return results[0].SQL_STMT as string;
            }
            return string.Empty;
        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="host"></param>
        /// <param name="appId"></param>
        /// <param name="sqlNo"></param>
        /// <param name="parm"></param>
        public async Task<object> GetResultFromSQLTab (User user, DataBaseHostEnum host, int appId, int sqlNo, List<Param> parm, string queryExtend = "") {
            // WebApiTracking.SendToSignalR(DateTime.Now.ToString() + " => GetResultFromSQLTab", TrackingTypeEnum.HEADER);

            object result = null;
            string sql = await GetSqlStmtFromSqlTab (user, appId, sqlNo);

            if (string.IsNullOrEmpty (sql))
                return result;

            if (parm.Count > 0) {
                foreach (Param p in parm) {
                    string paramValueText = string.Empty;

                    if (string.IsNullOrEmpty (p.ParamValue)) {
                        paramValueText = " null ";
                    } else {
                        p.ParamValue.Replace ("--", ""); //ป้องกัน SQL Injection
                        switch (p.ParamType) {
                            case ParamMeterTypeEnum.STRING:
                                paramValueText = "'" + p.ParamValue.Replace ("'", "''") + "'";
                                break;
                            case ParamMeterTypeEnum.INTEGER:
                            case ParamMeterTypeEnum.DECIMAL:
                                paramValueText = p.ParamValue;
                                break;
                            case ParamMeterTypeEnum.DATETIME:
                                paramValueText = " to_date('" + p.ParamValue + "', 'dd/mm/yyyy hh24:mi:ss')";
                                break;
                            case ParamMeterTypeEnum.DATE:
                                paramValueText = " to_date('" + p.ParamValue + "', 'dd/mm/yyyy')";
                                break;
                        }
                    }

                    sql = sql.Replace (":" + p.ParamName, paramValueText);
                }
            }

            string strReplace = ":I_";
            while (sql.IndexOf (strReplace) > 0) {
                int index = sql.IndexOf (strReplace);
                int indexWord = sql.IndexOf (" ", index);
                int indexWord2 = sql.IndexOf (",", index);
                if (indexWord > indexWord2)
                    indexWord = indexWord2;
                string strSub = sql.Substring (index, (indexWord - index));
                sql = sql.Replace (strSub, " NULL ");
            }

            if (sql.IndexOf (strReplace) < 0) {
                try {
                    sql = sql + queryExtend;
                    // WebApiTracking.SendToSignalR(sql);

                    result = await GetResultAsync (user, host, sql);
                } catch (Exception ex) {
                    // WebApiTracking.SendToSignalR(ex.ToString());
                    throw ex;
                }
            }

            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="host"></param>
        /// <param name="appId"></param>
        /// <param name="sqlNo"></param>
        /// <param name="parm"></param>
        /// <param name="replaceTitle"></param>
        /// <param name="replaceText"></param>
        public async Task<object> GetResultFromSQLTabWithReplace (User user, DataBaseHostEnum host, int appId, int sqlNo, dynamic parm, string replaceTitle, string replaceText) {
            // WebApiTracking.SendToSignalR(DateTime.Now.ToString() + " => GetResultFromSQLTab", TrackingTypeEnum.HEADER);

            object result = null;
            string sql = await GetSqlStmtFromSqlTab (user, appId, sqlNo);

            if (string.IsNullOrEmpty (sql))
                return result;

            sql = sql.Replace (replaceTitle, replaceText);

            if (parm == null)
                result = await GetResultDapperAsync (user, host, sql);
            else
                result = await GetResultDapperAsync (user, host, sql, parm);
            return result;

        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="host"></param>
        /// <param name="appId"></param>
        /// <param name="sqlNo"></param>
        /// <param name="parm"></param>
        /// <param name="replaceTitle"></param>
        /// <param name="replaceText"></param>
        public async Task<object> GetResultFromSQLTabWithReplace (User user, DataBaseHostEnum hostStmt, DataBaseHostEnum hostRetrieve, int appId, int sqlNo, dynamic parm, string replaceTitle, string replaceText) {
            // WebApiTracking.SendToSignalR(DateTime.Now.ToString() + " => GetResultFromSQLTab", TrackingTypeEnum.HEADER);

            object result = null;
            string sql = await GetSqlStmtFromSqlTab (user, hostStmt, appId, sqlNo);

            if (string.IsNullOrEmpty (sql))
                return result;

            sql = sql.Replace (replaceTitle, replaceText);

            if (parm == null)
                result = await GetResultDapperAsync (user, hostRetrieve, sql);
            else
                result = await GetResultDapperAsync (user, hostRetrieve, sql, parm);
            return result;

        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="host"></param>
        /// <param name="appId"></param>
        /// <param name="sqlNo"></param>
        /// <param name="parm"></param>
        /// <param name="replaceTitle"></param>
        /// <param name="replaceText"></param>
        public async Task<object> GetResultFromSQLTabWithReplace (User user, DataBaseHostEnum host, int appId, int sqlNo, List<Param> parm, string replaceTitle, string replaceText) {
            // WebApiTracking.SendToSignalR(DateTime.Now.ToString() + " => GetResultFromSQLTab", TrackingTypeEnum.HEADER);

            object result = null;
            string sql = await GetSqlStmtFromSqlTab (user, appId, sqlNo);

            if (string.IsNullOrEmpty (sql))
                return result;

            sql = sql.Replace (replaceTitle, replaceText);
            if (parm.Count > 0) {
                foreach (Param p in parm) {
                    string paramValueText = string.Empty;

                    if (string.IsNullOrEmpty (p.ParamValue)) {
                        paramValueText = " null ";
                    } else {
                        p.ParamValue.Replace ("--", ""); //ป้องกัน SQL Injection
                        switch (p.ParamType) {
                            case ParamMeterTypeEnum.STRING:
                                paramValueText = "'" + p.ParamValue.Replace ("'", "''") + "'";
                                break;
                            case ParamMeterTypeEnum.INTEGER:
                            case ParamMeterTypeEnum.DECIMAL:
                                paramValueText = p.ParamValue;
                                break;
                            case ParamMeterTypeEnum.DATETIME:
                                paramValueText = " to_date('" + p.ParamValue + "', 'dd/mm/yyyy hh24:mi:ss')";
                                break;
                            case ParamMeterTypeEnum.DATE:
                                paramValueText = " to_date('" + p.ParamValue + "', 'dd/mm/yyyy')";
                                break;
                        }
                    }

                    sql = sql.Replace (":" + p.ParamName, paramValueText);
                }
            }

            string strReplace = ":I_";
            while (sql.IndexOf (strReplace) > 0) {
                int index = sql.IndexOf (strReplace);
                int indexWord = sql.IndexOf (" ", index);
                int indexWord2 = sql.IndexOf (",", index);
                if (indexWord > indexWord2)
                    indexWord = indexWord2;
                string strSub = sql.Substring (index, (indexWord - index));
                sql = sql.Replace (strSub, " NULL ");
            }

            if (sql.IndexOf (strReplace) < 0) {
                try {
                    result = await GetResultAsync (user, host, sql);
                } catch (Exception ex) {
                    throw ex;
                }
            }

            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="user">user ที่ดึงข้อมูล</param>
        /// <param name="host">Database ที่ต้องการดึงข้อมูล</param>
        /// <param name="appId">รหัส Application</param>
        /// <param name="sqlNo">รหัส SqlTab</param>
        /// <param name="parm">parameter</param>
        public async Task<object> GetResultFromSQLTab (User user, DataBaseHostEnum host, int appId, int sqlNo, dynamic param) {
            object result = null;
            string sql = await GetSqlStmtFromSqlTab (user, appId, sqlNo);

            if (param == null)
                result = await GetResultDapperAsync (user, host, sql);
            else
                result = await GetResultDapperAsync (user, host, sql, param);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="user">user ที่ดึงข้อมูล</param>
        /// <param name="appId">รหัส Application</param>
        /// <param name="sqlNo">รหัส SqlTab</param>
        /// <param name="parm">parameter</param>
        public async Task<object> GetResultFromSQLTab (User user, int appId, int sqlNo, dynamic param) {
            object result = null;
            result = await GetResultFromSQLTab (user, DataContextConfiguration.DEFAULT_DATABASE, appId, sqlNo, param);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="user">user ที่ดึงข้อมูล</param>
        /// <param name="appId">รหัส Application</param>
        /// <param name="sqlNo">รหัส SqlTab</param>
        /// <param name="parm">parameter</param>
        public async Task<object> GetResultFromSQLTabWithDapperParm (User user, int appId, int sqlNo, DynamicParameters dapperParam) {
            object result = null;
            string sql = await GetSqlStmtFromSqlTab (user, appId, sqlNo);
            result = await GetResultDapperWithDapperParmAsync (user, DataContextConfiguration.DEFAULT_DATABASE, sql, dapperParam);
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="user">user ที่ดึงข้อมูล</param>
        /// <param name="appId">รหัส Application</param>
        /// <param name="sqlNo">รหัส SqlTab</param>
        public async Task<object> GetResultFromSQLTabWithOutParm (User user, int appId, int sqlNo) {
            object result = null;
            result = await GetResultFromSQLTab (user, appId, sqlNo, new List<Param> ());
            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="user">user ที่ดึงข้อมูล</param>
        ///<param name="host">base ที่ต้องการดึงข้อมูล</param>
        /// <param name="appId">รหัส Application</param>
        /// <param name="sqlNo">รหัส SqlTab</param>
        public async Task<object> GetResultFromSQLTabWithOutParm (User user, DataBaseHostEnum host, int appId, int sqlNo) {
            object result = null;
            result = await GetResultFromSQLTab (user, host, appId, sqlNo, new List<Param> ());
            return result;
        }

        /// <summary>
        /// ดึง SQL Statment จาก SQL TAB PORTAL แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="sqlNo"></param>
        /// <param name="parm"></param>
        public async Task<object> GetResultFromSQLTab (User user, int appId, int sqlNo, List<Param> parm, string queryExtend = "") {
            return await GetResultFromSQLTab (user, DataContextConfiguration.DEFAULT_DATABASE, appId, sqlNo, parm, queryExtend);
        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="host"></param>
        /// <param name="sqlNo"></param>
        /// <param name="parm"></param>
        public async Task<object> GetResultFromSQLTabPortal (User user, int sqlNo, dynamic param) {
            // WebApiTracking.SendToSignalR(DateTime.Now.ToString() + " => GetResultFromSQLTab", TrackingTypeEnum.HEADER);

            object result = null;
            string sql = await GetSqlStmtFromSqlTabPortal (user, sqlNo);

            if (string.IsNullOrEmpty (sql))
                return result;

            result = await GetResultDapperAsync (user, DataContextConfiguration.DEFAULT_DATABASE, sql, param);

            return result;
        }

        /// <summary>
        /// ดึงข้อมูลจาก SQL Statment จาก SQL TAB แบบ Asynchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="host"></param>
        /// <param name="sqlNo"></param>
        /// <param name="parm"></param>
        public async Task<object> GetResultFromSQLTabPortal (User user, int sqlNo, List<Param> parm, string queryExtend = "") {
            // WebApiTracking.SendToSignalR(DateTime.Now.ToString() + " => GetResultFromSQLTab", TrackingTypeEnum.HEADER);

            object result = null;
            string sql = await GetSqlStmtFromSqlTabPortal (user, sqlNo);

            if (string.IsNullOrEmpty (sql))
                return result;

            if (parm.Count > 0) {
                foreach (Param p in parm) {
                    string paramValueText = string.Empty;

                    if (string.IsNullOrEmpty (p.ParamValue)) {
                        paramValueText = " null ";
                    } else {
                        switch (p.ParamType) {
                            case ParamMeterTypeEnum.STRING:
                                paramValueText = "'" + p.ParamValue.Replace ("'", "''") + "'";
                                break;
                            case ParamMeterTypeEnum.INTEGER:
                            case ParamMeterTypeEnum.DECIMAL:
                                paramValueText = p.ParamValue;
                                break;
                            case ParamMeterTypeEnum.DATETIME:
                                paramValueText = " to_date('" + p.ParamValue + "', 'dd/mm/yyyy hh24:mi:ss')";
                                break;
                            case ParamMeterTypeEnum.DATE:
                                paramValueText = " to_date('" + p.ParamValue + "', 'dd/mm/yyyy')";
                                break;
                        }
                    }

                    sql = sql.Replace (":" + p.ParamName, paramValueText);
                }
            }

            string strReplace = ":I_";
            while (sql.IndexOf (strReplace) > 0) {
                int index = sql.IndexOf (strReplace);
                int indexWord = sql.IndexOf (" ", index);
                int indexWord2 = sql.IndexOf (",", index);
                if (indexWord > indexWord2)
                    indexWord = indexWord2;
                string strSub = sql.Substring (index, (indexWord - index));
                sql = sql.Replace (strSub, " NULL ");
            }

            if (sql.IndexOf (strReplace) < 0) {
                try {
                    sql = sql + queryExtend;
                    // WebApiTracking.SendToSignalR(sql);

                    result = await GetResultAsync (user, DataContextConfiguration.DEFAULT_DATABASE, sql);
                } catch (Exception ex) {
                    // WebApiTracking.SendToSignalR(ex.ToString());
                    throw ex;
                }
            }

            return result;
        }

        /// <summary>
        /// ดึง SQL Statment จาก SQL TAB แบบ Synchronize
        /// </summary>
        /// <param name="User"></param>
        /// <param name="appId"></param>
        /// <param name="sqlNo"></param>
        public async Task<string> GetSqlStmtFromSqlTabPortal (User user, int sqlNo) {
            var callSqlTab = "SELECT sql_stmt FROM KPDBA.SQL_TAB_PORTAL WHERE SQL_NO = " + sqlNo;
            var rawData = await GetResultAsync (user, callSqlTab);
            var results =
                _mapper.Map<IEnumerable<SqlTab>>
                (rawData).ToList ();

            if (results.Count > 0) {
                return results[0].SQL_STMT as string;
            }
            return string.Empty;
        }
        /// <summary>
        /// เรียกใช้งาน StoreProcedure
        /// </summary>
        /// <param name="databasehost"></param>
        /// <param name="storedName"></param>
        /// <param name="param"></param>
        /// //User user, 
        public async Task<IEnumerable<object>> CallStoredProcudure (DataBaseHostEnum databasehost, string storedName, List<Param> param) {
            try {
                var dyParam = new OracleDynamicParameters ();
                foreach (Param p in param) {
                    dyParam.Add (p.ParamName, GetOracleDbType (p.ParamType), ParameterDirection.Input, p.ParamValue);
                }
                //cursor name
                dyParam.Add ("CUR_RET", OracleDbType.RefCursor, ParameterDirection.Output);

                var conn = ConnectionFactory.GetDatabaseInstanceByHost (databasehost);
                if (conn.State == ConnectionState.Closed) {
                    conn.Open ();
                }
                Console.WriteLine ("Hello");
                if (conn.State == ConnectionState.Open) {
                    var rawResult = await SqlMapper.QueryAsync (conn, storedName, param : dyParam, commandType : CommandType.StoredProcedure);
                    return rawResult;
                }
            } catch (Exception ex) {
                throw ex;
            }

            return null;
        }

        public static OracleDbType GetOracleDbType (ParamMeterTypeEnum type) {
            OracleDbType oracleType = OracleDbType.Varchar2;
            switch (type) {
                case ParamMeterTypeEnum.STRING:
                    oracleType = OracleDbType.Varchar2;
                    break;
                case ParamMeterTypeEnum.DECIMAL:
                    oracleType = OracleDbType.Decimal;
                    break;
                case ParamMeterTypeEnum.INTEGER:
                    oracleType = OracleDbType.Int32;
                    break;
                case ParamMeterTypeEnum.DATE:
                case ParamMeterTypeEnum.DATETIME:
                    oracleType = OracleDbType.Date;
                    break;
                default:
                    oracleType = OracleDbType.Varchar2;
                    break;
            }

            return oracleType;
        }
    }
}