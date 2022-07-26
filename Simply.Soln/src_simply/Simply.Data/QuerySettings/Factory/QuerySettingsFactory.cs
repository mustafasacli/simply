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
        /// Finds the query setting.
        /// </summary>
        /// <param name="connectionType">The connection type.</param>
        /// <returns>A IQuerySetting.</returns>
        internal static IQuerySetting FindQuerySetting(DbConnectionTypes connectionType)
        {
            IQuerySetting setting = null;

            switch (connectionType)
            {
                case DbConnectionTypes.None:
                    break;

                case DbConnectionTypes.MsSql:
                    setting = new MsSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.Oracle:
                    setting = new OracleQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.PostgreSql:
                    setting = new PgSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.DB2:
                    setting = new DB2QuerySettings(connectionType);
                    break;

                case DbConnectionTypes.Odbc:
                    setting = new OdbcQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.Oledb:
                    setting = new OledbQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.MySql:
                    setting = new MySqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlCE:
                    setting = new SqlCEQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.Firebird:
                    setting = new FirebirdQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SQLite:
                    setting = new SQLiteQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.VistaDB:
                    setting = new VistaDbQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlBase:
                    setting = new SqlBaseQuerySettings(connectionType);
                    break;

                case DbConnectionTypes.Synergy:
                    setting = new SynergyQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlOdbc:
                    setting = new MsSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlOledb:
                    setting = new MsSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.OracleOdbc:
                    setting = new OracleQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.OracleOledb:
                    setting = new OracleQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.PostgreSqlOdbc:
                    setting = new PgSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.PostgreSqlOledb:
                    setting = new PgSqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.DB2Odbc:
                    setting = new DB2QuerySettings(connectionType);
                    break;

                case DbConnectionTypes.DB2Oledb:
                    setting = new DB2QuerySettings(connectionType);
                    break;

                case DbConnectionTypes.MySqlOdbc:
                    setting = new MySqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.MySqlOledb:
                    setting = new MySqlQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.FirebirdOdbc:
                    setting = new FirebirdQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.FirebirdOledb:
                    setting = new FirebirdQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlBaseOdbc:
                    setting = new SqlBaseQuerySettings(connectionType);
                    break;

                case DbConnectionTypes.SqlBaseOledb:
                    setting = new SqlBaseQuerySettings(connectionType);
                    break;

                case DbConnectionTypes.SynergyOdbc:
                    setting = new SynergyQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SynergyOledb:
                    setting = new SynergyQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.NuoDb:
                    setting = new NuodbQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SqlDatabase:
                    setting = new SqlDatabaseQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SybaseASA:
                    setting = new SybaseQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SybaseASAOdbc:
                    setting = new SybaseQuerySetting(connectionType);
                    break;

                case DbConnectionTypes.SybaseASAOledb:
                    setting = new SybaseQuerySetting(connectionType);
                    break;

                default:
                    break;
            }

            if (setting == null)
                throw new Exception("No Query Setting found for with connection type: " + connectionType.ToString());

            return setting;
        }
    }
}