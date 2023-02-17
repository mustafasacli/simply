using Simply.Common;
using Simply.Common.Interfaces;
using Simply.Data.Constants;
using Simply.Data.Enums;
using Simply.Data.Interfaces;
using Simply.Data.QuerySettings;
using Simply.Definitor.Attribute;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DbConnectionTypeBuilder"/>.
    /// </summary>
    public static class DbConnectionTypeBuilder
    {
        /// <summary>
        /// Defines the connectionTypePairs.
        /// </summary>
        private static readonly ConcurrentDictionary<string, DbConnectionTypes>
            connectionTypePairs = new ConcurrentDictionary<string, DbConnectionTypes>();

        /// <summary>
        /// Gets Odbc Connection Types.
        /// </summary>
        public static readonly DbConnectionTypes[] OdbcConnectionTypes = new DbConnectionTypes[]
            {
                DbConnectionTypes.Odbc,
                DbConnectionTypes.DB2Odbc,
                DbConnectionTypes.FirebirdOdbc,
                DbConnectionTypes.MySqlOdbc,
                DbConnectionTypes.OracleOdbc,
                DbConnectionTypes.PostgreSqlOdbc,
                DbConnectionTypes.SqlBaseOdbc,
                DbConnectionTypes.SqlOdbc,
                DbConnectionTypes.SynergyOdbc,
                DbConnectionTypes.NuoDb,
                DbConnectionTypes.SybaseASAOdbc
            };

        /// <summary>
        /// Gets Oledb Connection Types.
        /// </summary>
        public static readonly DbConnectionTypes[] OledbConnectionTypes = new DbConnectionTypes[]
            {
                DbConnectionTypes.Oledb,
                DbConnectionTypes.DB2Oledb,
                DbConnectionTypes.FirebirdOledb,
                DbConnectionTypes.MySqlOledb,
                DbConnectionTypes.OracleOledb,
                DbConnectionTypes.PostgreSqlOledb,
                DbConnectionTypes.SqlBaseOledb,
                DbConnectionTypes.SqlOledb,
                DbConnectionTypes.SynergyOledb,
                DbConnectionTypes.SybaseASAOledb
            };

        /// <summary>
        /// Gets Oracle Connection Types.
        /// </summary>
        public static readonly DbConnectionTypes[] OracleConnectionTypes = new DbConnectionTypes[]
            {
                DbConnectionTypes.Oracle,
                DbConnectionTypes.OracleOdbc,
                DbConnectionTypes.OracleOledb
            };

        /// <summary>
        /// Gets ConnectionTypes for Scalar Insert op.
        /// </summary>
        public static readonly DbConnectionTypes[] ScalarInsertConnectionTypes = new DbConnectionTypes[]
            {
                DbConnectionTypes.MsSql,
                DbConnectionTypes.VistaDB,
                DbConnectionTypes.MySql,
                DbConnectionTypes.Oledb,
                DbConnectionTypes.PostgreSql,
                DbConnectionTypes.SQLite,
                DbConnectionTypes.SqlCE,
                DbConnectionTypes.DB2,
                DbConnectionTypes.SybaseASA
            };

        /// <summary>
        /// Gets Connection Type.
        /// </summary>
        /// <param name="connection">Connection instance.</param>
        /// <returns>returns DbConnectionTypes enum.</returns>
        public static DbConnectionTypes GetDbConnectionType(this IDbConnection connection)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            DbConnectionTypes connectionType = GetConnectionType(connection);
            return connectionType;
        }

        /// <summary>
        /// The GetConnectionType.
        /// </summary>
        /// <param name="connection">Database connection <see cref="IDbConnection"/>.</param>
        /// <returns>The <see cref="DbConnectionTypes"/>.</returns>
        private static DbConnectionTypes GetConnectionType(IDbConnection connection)
        {
            string connClassFullName = connection.GetType().FullName;
            DbConnectionTypes connectionType;

            if (!connClassFullName.IsMember(new string[] { DbConnectionNames.OleDb, DbConnectionNames.Odbc }))
            {
                if (!connectionTypePairs.TryGetValue(connClassFullName, out connectionType))
                {
                    //MySql
                    if (connClassFullName == DbConnectionNames.MySql
                        || connClassFullName == DbConnectionNames.MySqlDevart)
                    {
                        connectionType = DbConnectionTypes.MySql;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //SQLite
                    if (connClassFullName == DbConnectionNames.SQLite
                        || connClassFullName == DbConnectionNames.Devart_SQLite)
                    {
                        connectionType = DbConnectionTypes.SQLite;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //Firebird
                    if (connClassFullName == DbConnectionNames.Firebird)
                    {
                        connectionType = DbConnectionTypes.Firebird;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //Oracle
                    if (connClassFullName == DbConnectionNames.Oracle
                        || connClassFullName == DbConnectionNames.OracleManaged
                        || connClassFullName == DbConnectionNames.Devart_Oracle
                        || connClassFullName == DbConnectionNames.Oracle_Win)
                    {
                        connectionType = DbConnectionTypes.Oracle;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //PostgreSql
                    if (connClassFullName == DbConnectionNames.PostgreSql
                        || connClassFullName == DbConnectionNames.PostgreSqlDevart)
                    {
                        connectionType = DbConnectionTypes.PostgreSql;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //VistaDB
                    if (connClassFullName == DbConnectionNames.VistaDB)
                    {
                        connectionType = DbConnectionTypes.VistaDB;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //MsSql
                    if (connClassFullName == DbConnectionNames.Sql)
                    {
                        connectionType = DbConnectionTypes.MsSql;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //SqlBase

                    if (connClassFullName == DbConnectionNames.SqlBaseGupta
                    || connClassFullName == DbConnectionNames.SqlBaseUnify)
                    {
                        connectionType = DbConnectionTypes.SqlBase;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //SqlCE
                    if (connClassFullName == DbConnectionNames.SqlCe)
                    {
                        connectionType = DbConnectionTypes.SqlCE;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //Synergy
                    if (connClassFullName == DbConnectionNames.Sde)
                    {
                        connectionType = DbConnectionTypes.Synergy;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //DB2
                    if (connClassFullName == DbConnectionNames.DB2)
                    {
                        connectionType = DbConnectionTypes.DB2;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //NuoDb
                    if (connClassFullName == DbConnectionNames.NuoDb)
                    {
                        connectionType = DbConnectionTypes.NuoDb;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //SQLDatabaseNet
                    if (connClassFullName == DbConnectionNames.SQLDatabaseNet)
                    {
                        connectionType = DbConnectionTypes.SqlDatabase;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    //Sybase
                    if (connClassFullName == DbConnectionNames.Sybase)
                    {
                        connectionType = DbConnectionTypes.SybaseASA;
                        connectionTypePairs.GetOrAdd(connClassFullName, connectionType);
                        return connectionType;
                    }

                    connectionType = DbConnectionTypes.None;
                }
            }
            else
            {
                bool isOdbc = connClassFullName == DbConnectionNames.Odbc;

                connectionType =
                    isOdbc ? DbConnectionTypes.Odbc : DbConnectionTypes.Oledb;

                string sConnStr = connection?.ConnectionString ?? string.Empty;
                string driver = DbConnectionNames.Ole_Driver;
                string key;

                if (sConnStr.Contains(driver))
                {
                    key = DbConnectionNames.Sql;
                    if (sConnStr.IndexOf(driver) < sConnStr.IndexOf(key))
                    {
                        connectionType =
                            isOdbc ? DbConnectionTypes.SqlOdbc : DbConnectionTypes.SqlOledb;
                        return connectionType;
                    }

                    key = DbConnectionNames.Ole_Oracle;
                    if (sConnStr.IndexOf(driver) < sConnStr.IndexOf(key))
                    {
                        connectionType =
                            isOdbc ? DbConnectionTypes.OracleOdbc : DbConnectionTypes.OracleOledb;
                        return connectionType;
                    }

                    key = DbConnectionNames.Ole_Ora;
                    if (sConnStr.IndexOf(driver) < sConnStr.IndexOf(key))
                    {
                        connectionType =
                            isOdbc ? DbConnectionTypes.OracleOdbc : DbConnectionTypes.OracleOledb;
                        return connectionType;
                    }

                    key = DbConnectionNames.Ole_DB2;
                    if (sConnStr.IndexOf(driver) < sConnStr.IndexOf(key))
                    {
                        connectionType =
                            isOdbc ? DbConnectionTypes.OracleOdbc : DbConnectionTypes.OracleOledb;
                        return connectionType;
                    }

                    key = DbConnectionNames.Ole_MySql;
                    if (sConnStr.IndexOf(driver) < sConnStr.IndexOf(key))
                    {
                        connectionType =
                            isOdbc ? DbConnectionTypes.MySqlOdbc : DbConnectionTypes.MySqlOledb;
                        return connectionType;
                    }
                    // TODO : Sybase veritabanı için; Oledb ve Odbc Connection Type Oluşturma yapılacak.
                    /*
                    key = "sybase";
                    if (sConnStr.IndexOf(driver) < sConnStr.IndexOf(key, StringComparison.OrdinalIgnoreCase))
                    {
                        connectionType =
                            isOdbc ? DbConnectionTypes.sybaseOdbc : DbConnectionTypes.sybaseOledb;
                        return connectionType;
                    }

                    key = "INTERSOLV";
                    if (sConnStr.IndexOf(driver) < sConnStr.IndexOf(key))
                    {
                        connectionType =
                            isOdbc ? DbConnectionTypes.OracleOdbc : DbConnectionTypes.;
                        return connectionType;
                    }
                    //SYBASE
                    //INTERSOLV
                    //Sybase
                    */
                }
            }

            return connectionType;
        }

        /// <summary>
        /// Gets Connection Type.
        /// </summary>
        /// <typeparam name="T">DbConnection class.</typeparam>
        /// <param name="connection">Connection instance.</param>
        /// <returns>returns DbConnectionTypes enum.</returns>
        public static DbConnectionTypes GetDbConnectionType<T>(this T connection) where T : class, IDbConnection
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            DbConnectionTypes connectionType = GetConnectionType(connection);
            return connectionType;
        }

        /// <summary>
        /// Checks Connection Type is in Insert Scalar Mode.
        /// </summary>
        /// <param name="connectionType">Connection tType <see cref="DbConnectionTypes"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsInsertScalarMode(this DbConnectionTypes connectionType)
        {
            bool isMember = connectionType.IsMember(ScalarInsertConnectionTypes);
            return isMember;
        }

        /// <summary>
        /// Checks DbConnection is Odbc connection type.
        /// </summary>
        /// <param name="connectionType">
        /// The connectionType <see cref="DbConnectionTypes"/> Db Connectype enum instance.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> if connnection type is one of these (Odbc, DB2Odbc, FirebirdOdbc,
        /// MySqlOdbc, OracleOdbc, PostgreSqlOdbc, SqlBaseOdbc, SqlOdbc, SynergyOdbc, NuoDb) returns
        /// true else returns false.
        /// </returns>
        public static bool IsOdbcConn(this DbConnectionTypes connectionType)
        {
            bool isOdbc = connectionType.IsMember(OdbcConnectionTypes);
            return isOdbc;
        }

        /// <summary>
        /// Checks DbConnection is Oledb connection type.
        /// </summary>
        /// <param name="connectionType">
        /// The connectionType <see cref="DbConnectionTypes"/> Db Connectype enum instance.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> if connnection type is one of these (Oledb, DB2Oledb,
        /// FirebirdOledb, MySqlOledb, OracleOledb, PostgreSqlOledb, SqlBaseOledb, SqlOledb,
        /// SynergyOledb) returns true else returns false.
        /// </returns>
        public static bool IsOledbConn(this DbConnectionTypes connectionType)
        {
            bool isOledb = connectionType.IsMember(OledbConnectionTypes);
            return isOledb;
        }

        /// <summary>
        /// Checks Connection type is Oracle.
        /// </summary>
        /// <param name="connectionType">
        /// The connectionType <see cref="DbConnectionTypes"/> Db Connectype enum instance.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> if connnection type is one of these (Oracle, OracleOdbc,
        /// OracleOledb) returns true else returns false.
        /// </returns>
        public static bool IsOracleConnection(this DbConnectionTypes connectionType)
        {
            bool isOracle = connectionType.IsMember(OracleConnectionTypes);
            return isOracle;
        }

        /// <summary>
        /// Gets Table Name with Schema includes connection type prefix-suffix.
        /// </summary>
        /// <typeparam name="T">T class.</typeparam>
        /// <param name="connectionType">
        /// The connectionType <see cref="DbConnectionTypes"/> Db Connectype enum instance.
        /// </param>
        /// <param name="includeSchemaName">if true reuslt includes schema info else does not include.</param>
        /// <param name="simpleDefinitor">Simmple Definitor for table-class, column-property mapping.</param>
        /// <returns>Returns Table Name with Schema includes connection type prefix-suffix.</returns>
        public static string GetFullTableName<T>(this DbConnectionTypes connectionType, ISimpleDefinitor<T> simpleDefinitor, bool includeSchemaName = true) where T : class
        {
            string tableName = (simpleDefinitor ?? AttributeDefinitor<T>.New()).GetTableName();
            IQuerySetting querySetting = QuerySettingsFactory.GetQuerySetting(connectionType);
            string fullTableName = $"{querySetting.Prefix}{tableName}{querySetting.Suffix}";

            if (!includeSchemaName) return fullTableName;

            string schema = simpleDefinitor.GetSchemaName();
            if (!string.IsNullOrWhiteSpace(schema))
            {
                fullTableName = $"{querySetting.Prefix}{schema}{querySetting.Suffix}.{fullTableName}";
            }

            return fullTableName;
        }
    }
}