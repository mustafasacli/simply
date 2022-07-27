using Simply.Common;
using Simply.Data;
using Simply.Data.Objects;
using KisayolYoneticisi.Source.BO;
using KisayolYoneticisi.Source.QO;
using KisayolYoneticisi.Source.Util;
using KisayolYoneticisi.Source.Variables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace KisayolYoneticisi.Source.OpManager
{
    internal class SQLiteManager
    {
        #region [ Execute method ]

        internal static int Execute(string query, params DbParam[] parameters)
        {
            int retInt = -1;

            using (SQLiteConnection conn = new SQLiteConnection(AppVariables.ConnectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = query;
                            cmd.Transaction = trans;
                            if (parameters != null)
                            {
                                foreach (var prm in parameters)
                                {
                                    cmd.Parameters.AddWithValue(prm.Name, prm.Value);
                                }
                            }
                            retInt = cmd.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                        finally
                        {
                            cmd.Parameters.Clear();
                            conn.Close();
                        }
                    }// end command
                }// end transaction
            }// end connection

            return retInt;
        }

        #endregion [ Execute method ]


        #region [ Add method]

        public static int Add(Kisayol kisayol)
        {
            int retInt = -1;

            using (SQLiteConnection conn = new SQLiteConnection(AppVariables.ConnectionString))
            {
                retInt = conn.Execute(Crud.InsertQuery(), new { kisayol.KisayolAdi, kisayol.Yol, kisayol.Tarih });
            }

            return retInt;
        }

        #endregion [ Add method]

        #region [ Delete method ]

        public static int Delete(Kisayol kisayol)
        {
            int retInt = -1;

            using (SQLiteConnection conn = new SQLiteConnection(AppVariables.ConnectionString))
            {
                retInt = conn.Execute(Crud.DeleteQuery(), new { kisayol.Id });
            }

            return retInt;
        }

        #endregion [ Delete method ]

        #region [ Update method ]

        public static int Update(Kisayol kisayol)
        {
            int retInt = -1;

            using (SQLiteConnection conn = new SQLiteConnection(AppVariables.ConnectionString))
            {
                retInt = conn.Execute(Crud.UpdateQuery(), new { kisayol.KisayolAdi, kisayol.Yol, kisayol.Tarih, kisayol.Id });
            }

            return retInt;
        }

        #endregion [ Update method ]

        #region [ GetTable method ]

        public static DataTable GetTable()
        {
            DataTable table = null;

            using (SQLiteConnection conn = new SQLiteConnection(AppVariables.ConnectionString))
            {
                DataSet set =
                    conn.GetResultSetQuery(commandDefinition: new DbCommandDefinition
                    {
                        CommandText = Crud.GetTable()
                    }).Result;
                table = set.Tables[0];
            }

            return table;
        }

        #endregion [ GetTable method ]

        #region [ AllList method ]

        public static List<Kisayol> AllList()
        {
            return DataUtility.ToList(GetTable());
        }

        #endregion [ AllList method ]

        public static int GetIdentity()
        {
            int retInt = -1;

            DataTable table = null;

            using (SQLiteConnection conn = new SQLiteConnection(AppVariables.ConnectionString))
            {
                DataSet set =
                    conn.GetResultSetQuery(commandDefinition: new DbCommandDefinition { CommandText = Crud.GetIdentity() }).Result;
                table = set.Tables[0];
            }

            foreach (DataRow row in table.Rows)
            {
                retInt = row[0].ToInt();
            }

            return retInt;
        }
    }
}