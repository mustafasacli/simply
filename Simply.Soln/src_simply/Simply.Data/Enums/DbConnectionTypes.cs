////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Enums\DbConnectionTypes.cs
//
// summary:	Implements the database connection types class
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Simply.Data.Enums
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Values that represent Database connection types. </summary>
    ///
    /// <remarks>   Mustafa SAÇLI, 4.05.2019. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum DbConnectionTypes : byte
    {
        /// <summary>
        ///
        /// </summary>
        None = 0,

        /// <summary>
        ///
        /// </summary>
        MsSql = 101,

        /// <summary>
        ///
        /// </summary>
        Oracle = 102,

        /// <summary>
        ///
        /// </summary>
        PostgreSql = 103,

        /// <summary>
        ///
        /// </summary>
        DB2 = 104,

        /// <summary>
        ///
        /// </summary>
        Odbc = 105,

        /// <summary>
        ///
        /// </summary>
        Oledb = 106,

        /// <summary>
        ///
        /// </summary>
        MySql = 107,

        /// <summary>
        ///
        /// </summary>
        SqlCE = 108,

        /// <summary>
        ///
        /// </summary>
        Firebird = 109,

        /// <summary>
        ///
        /// </summary>
        SQLite = 110,

        /// <summary>
        ///
        /// </summary>
        VistaDB = 111,

        /// <summary>
        ///
        /// </summary>
        SqlBase = 112,

        /// <summary>
        ///
        /// </summary>
        Synergy = 113,

        /// <summary>   An enum constant representing the MsSql odbc option. </summary>
        SqlOdbc = 114,

        /// <summary>   An enum constant representing the MsSql oledb option. </summary>
        SqlOledb = 115,

        /// <summary>   An enum constant representing the Oracle odbc option. </summary>
        OracleOdbc = 116,

        /// <summary>   An enum constant representing the Oracle oledb option. </summary>
        OracleOledb = 117,

        /// <summary>   An enum constant representing the PostgreSql odbc option. </summary>
        PostgreSqlOdbc = 118,

        /// <summary>   An enum constant representing the PostgreSql oledb option. </summary>
        PostgreSqlOledb = 119,

        /// <summary>   An enum constant representing the DB2 odbc option. </summary>
        DB2Odbc = 120,

        /// <summary>   An enum constant representing the DB2 oledb option. </summary>
        DB2Oledb = 121,

        /// <summary>   An enum constant representing the MySql odbc option. </summary>
        MySqlOdbc = 122,

        /// <summary>   An enum constant representing the MySql oledb option. </summary>
        MySqlOledb = 123,

        /// <summary>   An enum constant representing the Firebird odbc option. </summary>
        FirebirdOdbc = 124,

        /// <summary>   An enum constant representing the Firebird oledb option. </summary>
        FirebirdOledb = 125,

        /// <summary>   An enum constant representing the SQL base odbc option. </summary>
        SqlBaseOdbc = 126,

        /// <summary>   An enum constant representing the SQL base oledb option. </summary>
        SqlBaseOledb = 127,

        /// <summary>   An enum constant representing the synergy ODBC option. </summary>
        SynergyOdbc = 128,

        /// <summary>   An enum constant representing the synergy oledb option. </summary>
        SynergyOledb = 129,

        /// <summary>
        ///  An enum constant representing the nuo db= 130 option. http://www.nuodb.com/
        /// </summary>
        NuoDb = 130,

        /// <summary>
        ///  An enum constant representing the SQL database= 131 option. SQLDATABASE.NET,
        ///  http://sqldatabase.net/
        /// </summary>
        SqlDatabase = 131,

        /// <summary>
        /// Connection Type enum for SybaseASA database.
        /// </summary>
        SybaseASA = 132,

        /// <summary>   An enum constant representing the Sybase ASA ODBC option. </summary>
        SybaseASAOdbc = 133,

        /// <summary>   An enum constant representing the Sybase ASA oledb option. </summary>
        SybaseASAOledb = 134,
    };
}