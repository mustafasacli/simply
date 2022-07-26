using Coddie.Common;
using System.Data.Entity;

namespace Coddie.Ef
{
    /// <summary>
    ///
    /// </summary>
    public static class DbModelBuilderExtensions
    {
        /// <summary>
        /// Equivalent is modelBuilder.Entity{TEntity}().ToTable(typeof(TEntity).GetTableName());
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="tableName">Mapping Table name. it can be empty.</param>
        /// <returns></returns>
        public static DbModelBuilder MapEntityToTable<TEntity>
            (this DbModelBuilder modelBuilder, string tableName = null)
            where TEntity : class
        {
            var tblName = tableName;
            if (string.IsNullOrWhiteSpace(tblName))
            {
                tblName =
                 typeof(TEntity).GetTableNameOfType();
            }

            modelBuilder.Entity<TEntity>().ToTable(tblName);
            return modelBuilder;
        }

        /// <summary>
        /// Equivalent is modelBuilder.Entity{TEntity}().ToTable(typeof(TEntity).GetTableName(),typeof(TEntity).GetSchemaName());
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="tableName">Mapping Table name. it can be empty.</param>
        /// <param name="schemaName">Mapping Schema name. it can be empty.</param>
        /// <returns></returns>
        public static DbModelBuilder MapEntityToTable<TEntity>
            (this DbModelBuilder modelBuilder, string tableName, string schemaName)
            where TEntity : class
        {
            var type = typeof(TEntity);

            var table = tableName;
            if (string.IsNullOrWhiteSpace(table))
            {
                table = type.GetTableNameOfType();
            }

            var schema = schemaName;
            if (string.IsNullOrWhiteSpace(schema))
            {
                schema = type.GetSchemaNameOfType();
            }

            modelBuilder.Entity<TEntity>().ToTable(table, schema);
            return modelBuilder;
        }
    }
}