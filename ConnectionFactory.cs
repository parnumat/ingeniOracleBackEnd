using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace testAspOracle01 {
    public class ConnectionFactory {
        public static string ToppConnectionString = string.Empty;
        public static string LapConnectionString = string.Empty;
        public static string KppConnectionString = string.Empty;
        public static string KprConnectionString = string.Empty;
        private static OracleConnection instanceTopp;
        public static OracleConnection InstanceTopp {
            get {
                if (instanceTopp == null) {
                    string sqlConnection = GetConnectionStringTopp ();
                    instanceTopp = new OracleConnection (sqlConnection);
                }

                if (instanceTopp.State == ConnectionState.Closed) {
                    instanceTopp.Open ();
                }
                return instanceTopp;
            }
        }
        public static string GetConnectionStringTopp () {
            return ConnectionFactory.ToppConnectionString;
        }
        private static OracleConnection instanceLap;
        public static OracleConnection InstanceLap {
            get {
                if (instanceLap == null) {
                    string sqlConnection = GetConnectionStringLap ();
                    instanceLap = new OracleConnection (sqlConnection);
                }

                if (instanceLap.State == ConnectionState.Closed) {
                    instanceLap.Open ();
                }
                return instanceLap;
            }
        }
        public static string GetConnectionStringLap () {
            return ConnectionFactory.LapConnectionString;
        }
        private static OracleConnection instanceKpp;
        public static OracleConnection InstanceKpp {
            get {
                if (instanceKpp == null) {
                    string sqlConnection = GetConnectionStringKpp ();
                    instanceKpp = new OracleConnection (sqlConnection);
                }
                if (instanceKpp.State == ConnectionState.Closed) {
                    instanceKpp.Open ();
                }
                return instanceKpp;
            }
        }
        public static string GetConnectionStringKpp () {
            return ConnectionFactory.KppConnectionString;
        }
        private static OracleConnection instanceKpr;
        public static OracleConnection InstanceKpr {
            get {
                if (instanceKpr == null) {
                    string sqlConnection = GetConnectionStringKpr ();
                    instanceKpr = new OracleConnection (sqlConnection);
                }
                if (instanceKpr.State == ConnectionState.Closed) {
                    instanceKpr.Open ();
                }
                return instanceKpr;
            }
        }
        public static string GetConnectionStringKpr () {
            return ConnectionFactory.KprConnectionString;
        }
        public static OracleConnection GetNewInstance (DataBaseHostEnum databasehost) {
            OracleConnection instance = null;
            switch (databasehost) {
                case DataBaseHostEnum.OPPN:
                    instance = new OracleConnection (GetConnectionStringTopp ());
                    break;
                case DataBaseHostEnum.LAP:
                    instance = new OracleConnection (GetConnectionStringLap ());
                    break;
                case DataBaseHostEnum.KPP:
                    instance = new OracleConnection (GetConnectionStringKpp ());
                    break;
                case DataBaseHostEnum.KPR:
                    instance = new OracleConnection (GetConnectionStringKpr ());
                    break;
            }
            if (instance.State == ConnectionState.Closed) {
                instance.Open ();
            }

            return instance;
        }
        public OracleConnection GetNewInstanceOpp () {
            string sqlConnection = GetConnectionStringTopp ();
            var instance = new OracleConnection (sqlConnection);

            if (instance.State == ConnectionState.Closed) {
                instance.Open ();
            }

            return instance;
        }

        public OracleConnection GetNewInstanceLap () {
            string sqlConnection = GetConnectionStringLap ();
            var instance = new OracleConnection (sqlConnection);

            if (instance.State == ConnectionState.Closed) {
                instance.Open ();
            }

            return instance;
        }

        public OracleConnection GetNewInstanceKpp () {
            string sqlConnection = GetConnectionStringKpp ();
            var instance = new OracleConnection (sqlConnection);

            if (instance.State == ConnectionState.Closed) {
                instance.Open ();
            }

            return instance;
        }

        public OracleConnection GetNewInstanceKpr () {
            string sqlConnection = GetConnectionStringKpp ();
            var instance = new OracleConnection (sqlConnection);

            if (instance.State == ConnectionState.Closed) {
                instance.Open ();
            }

            return instance;
        }
        public void CloseConnection (OracleConnection conn) {
            conn.Close ();
        }

        /// <summary>
        /// ดึง Connection String ตาม Database Host
        /// </summary>
        /// <param name="databasehost"></param>
        public static string GetConnectionStringByHost (DataBaseHostEnum databasehost) {
            string connectionString = string.Empty;
            switch (databasehost) {
                case DataBaseHostEnum.OPPN:
                    connectionString = ConnectionFactory.ToppConnectionString;
                    break;
                case DataBaseHostEnum.LAP:
                    connectionString = ConnectionFactory.LapConnectionString;
                    break;
                case DataBaseHostEnum.KPP:
                    connectionString = ConnectionFactory.KppConnectionString;
                    break;
            }

            return connectionString;
        }

        /// <summary>
        /// ดึง Oracle Connection Instant ตาม Database Host
        /// </summary>
        /// <param name="databasehost"></param>
        public static OracleConnection GetDatabaseInstanceByHost (DataBaseHostEnum databasehost) {
            // ConnectionFactory cf = new ConnectionFactory();
            // return cf.GetNewInstance(databasehost);
            OracleConnection instance = null;
            switch (databasehost) {
                case DataBaseHostEnum.OPPN:
                    instance = ConnectionFactory.InstanceTopp;
                    break;
                case DataBaseHostEnum.LAP:
                    instance = ConnectionFactory.InstanceLap;
                    break;
                case DataBaseHostEnum.KPP:
                    instance = ConnectionFactory.InstanceKpp;
                    break;
                case DataBaseHostEnum.KPR:
                    instance = ConnectionFactory.InstanceKpr;
                    break;
            }

            return instance;
        }

        /// <summary>
        /// ดึง Oracle Connection Instant ตาม Database Host
        /// </summary>
        /// <param name="org"></param>
        public static DataBaseHostEnum GetDatabaseHostByName (string org) {
            // ConnectionFactory cf = new ConnectionFactory();
            // return cf.GetNewInstance(databasehost);
            DataBaseHostEnum host = DataContextConfiguration.DEFAULT_DATABASE;
            switch (org) {
                case "OPP":
                case "TOPP":
                    host = DataBaseHostEnum.OPPN;
                    break;
                case "LAP":
                    host = DataBaseHostEnum.LAP;
                    break;
                case "KPP":
                    host = DataBaseHostEnum.KPP;
                    break;

            }
            return host;
        }

        /// <summary>
        /// ดึง Oracle Connection Instant ตาม Database Host
        /// </summary>
        /// <param name="org"></param>
        public static OracleConnection GetDatabaseInstanceByHost (string org) {
            // ConnectionFactory cf = new ConnectionFactory();
            // return cf.GetNewInstance(databasehost);
            OracleConnection instance = null;
            switch (org) {
                case "OPP":
                case "TOPP":
                    instance = ConnectionFactory.InstanceTopp;
                    break;
                case "LAP":
                    instance = ConnectionFactory.InstanceLap;
                    break;
                case "KPP":
                    instance = ConnectionFactory.InstanceKpp;
                    break;
            }

            return instance;
        }

        /// <summary>
        /// ดึง Oracle Connection Instant ตาม Database Host โดยเป็นการประกาศใหม่
        /// </summary>
        /// <param name="databasehost"></param>
        public static OracleConnection GetNewDatabaseInstanceByHost (DataBaseHostEnum databasehost) {
            ConnectionFactory conFac = new ConnectionFactory ();
            OracleConnection instance = null;
            switch (databasehost) {
                case DataBaseHostEnum.OPPN:
                    instance = conFac.GetNewInstanceOpp ();
                    break;
                case DataBaseHostEnum.LAP:
                    instance = conFac.GetNewInstanceLap ();
                    break;
                case DataBaseHostEnum.KPP:
                    instance = conFac.GetNewInstanceKpp ();
                    break;
            }

            return instance;
        }

    }
}