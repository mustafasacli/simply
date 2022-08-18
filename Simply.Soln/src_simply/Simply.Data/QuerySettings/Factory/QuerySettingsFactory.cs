using Simply.Data.Constants;
using Simply.Data.Enums;
using Simply.Data.Interfaces;
using System;
using System.Collections.Concurrent;

namespace Simply.Data.QuerySettings
{
    /// <summary>
    /// Defines the <see cref="QuerySettingsFactory" />.
    /// </summary>
    public static class QuerySettingsFactory
    {
        private static readonly ConcurrentDictionary<DbConnectionTypes, IQuerySetting> connectionTypeQuerySettingMappings =
            new ConcurrentDictionary<DbConnectionTypes, IQuerySetting>();

        /// <summary>
        /// Gets Query Option of DbConnectionType.
        /// </summary>
        /// <param name="connectionType">The connType<see cref="DbConnectionTypes"/>.</param>
        /// <returns>The <see cref="IQuerySetting"/>.</returns>
        public static IQuerySetting GetQuerySetting(DbConnectionTypes connectionType)
        {
            IQuerySetting queryOption =
                connectionTypeQuerySettingMappings.GetOrAdd(connectionType, (p) =>
                {
                    return FindQuerySetting(p);
                });

            return queryOption;
        }

        /// <summary>
        /// Finds the query querySetting.
        /// </summary>
        /// <param name="connectionType">The connection type.</param>
        /// <returns>A IQuerySetting.</returns>
        internal static IQuerySetting FindQuerySetting(DbConnectionTypes connectionType)
        {
            IQuerySetting querySetting = null;

            switch (connectionType)
            {
                case DbConnectionTypes.None:
                    break;

                case DbConnectionTypes.MsSql:
                    querySetting = new MsSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.Oracle:
                    querySetting = new OracleQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.PostgreSql:
                    querySetting = new PgSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.DB2:
                    querySetting = new DB2QuerySettings(connectionType);
                    break;

                case DbConnectionTypes.Odbc:
                    querySetting = new OdbcQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.Oledb:
                    querySetting = new OledbQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.MySql:
                    querySetting = new MySqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlCE:
                    querySetting = new SqlCEQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.Firebird:
                    querySetting = new FirebirdQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SQLite:
                    querySetting = new SQLiteQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.VistaDB:
                    querySetting = new VistaDbQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlBase:
                    querySetting = new SqlBaseQuerySettings(connectionType);
                    break;

                case DbConnectionTypes.Synergy:
                    querySetting = new SynergyQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlOdbc:
                    querySetting = new MsSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlOledb:
                    querySetting = new MsSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.OracleOdbc:
                    querySetting = new OracleQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.OracleOledb:
                    querySetting = new OracleQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.PostgreSqlOdbc:
                    querySetting = new PgSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.PostgreSqlOledb:
                    querySetting = new PgSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.DB2Odbc:
                    querySetting = new DB2QuerySettings(connectionType);
                    break;

                case DbConnectionTypes.DB2Oledb:
                    querySetting = new DB2QuerySettings(connectionType);
                    break;

                case DbConnectionTypes.MySqlOdbc:
                    querySetting = new MySqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.MySqlOledb:
                    querySetting = new MySqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.FirebirdOdbc:
                    querySetting = new FirebirdQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.FirebirdOledb:
                    querySetting = new FirebirdQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlBaseOdbc:
                    querySetting = new SqlBaseQuerySettings(connectionType);
                    break;

                case DbConnectionTypes.SqlBaseOledb:
                    querySetting = new SqlBaseQuerySettings(connectionType);
                    break;

                case DbConnectionTypes.SynergyOdbc:
                    querySetting = new SynergyQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SynergyOledb:
                    querySetting = new SynergyQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.NuoDb:
                    querySetting = new NuodbQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlDatabase:
                    querySetting = new SqlDatabaseQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SybaseASA:
                    querySetting = new SybaseQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SybaseASAOdbc:
                    querySetting = new SybaseQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SybaseASAOledb:
                    querySetting = new SybaseQuerySetting(connectionType);
                    break;

                default:
                    break;
            }

            if (querySetting == null)
                throw new Exception(DbAppMessages.InvalidConnectionType + connectionType.ToString());

            return querySetting;
        }
    }
}