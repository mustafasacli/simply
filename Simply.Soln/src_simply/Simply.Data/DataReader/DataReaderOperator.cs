using Simply.Common.Objects;
using Simply.Data.Constants;
using System;
using System.Collections.Generic;
using System.Data;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DataReaderOperator"/>.
    /// </summary>
    public static class DataReaderOperator
    {
        /// <summary>
        /// Closes IDataReader object if it is not closed.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        public static void CloseIfNot(this IDataReader reader)
        {
            if (reader != null && !reader.IsClosed)
                reader.Close();
        }

        /// <summary>
        /// Checks Data Reader has any rows.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="closeAtFinal"></param>
        /// <returns>Returns true Datat reader has rows, else returns false.</returns>
        public static bool Any(this IDataReader reader, bool closeAtFinal = false)
        {
            bool any = false;

            try
            {
                if (reader.IsClosed)
                    return any;

                any = reader.Read();
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return any;
        }

        /// <summary>
        /// Returns DataReader metadata as dictionary.
        /// </summary>
        /// <param name="dataReader">Datareader instance.</param>
        /// <returns>Returns Dictionary{string, Type} object instance. </returns>
        public static IDictionary<string, Type> GetDataReaderMetadata(this IDataReader dataReader)
        {
            IDictionary<string, Type> readerMetadata = new Dictionary<string, Type>();

            int fieldCount = dataReader.FieldCount;
            for (int counter = 0; counter < fieldCount; counter++)
            {
                string columnName = dataReader.GetName(counter);
                Type columnDataType = dataReader.GetFieldType(counter);

                if (readerMetadata.ContainsKey(columnName))
                {
                    int columnCounter = 1;
                    while (readerMetadata.ContainsKey(columnName))
                    {
                        columnName = $"{columnName}_{columnCounter}";
                        columnCounter++;
                    }
                }

                readerMetadata[columnName] = columnDataType;
            }

            return readerMetadata;
        }

        /// <summary>
        /// GetDynamicResultSetSkipAndTake Get dynamic object List with skip and take options.
        /// </summary>
        /// <param name="reader">Data reader object instance.</param>
        /// <param name="skip">Count for Skip.</param>
        /// <param name="take">Count for Take.</param>
        /// <param name="closeAtFinal">if true datareader will be closed at final else not.</param>
        /// <returns>Returns dynamic object list.</returns>
        public static List<SimpleDbRow> GetSimleRowListSkipAndTake(this IDataReader reader,
           uint skip = 0, uint take = 0, bool closeAtFinal = false)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            List<SimpleDbRow> list = new List<SimpleDbRow>();

            try
            {
                if (reader.IsClosed || take == 0)
                    return list;

                uint cntr = 0;

                while (reader.Read())
                {
                    if (cntr <= skip)
                        continue;

                    if (cntr > (skip + take))
                        break;

                    cntr++;
                    SimpleDbRow row = reader.GetSimpleDbRow();
                    list.Add(row);
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return list;
        }

        /// <summary>
        /// GetDynamicResultSet Gets ResultSet as dynamic object list.
        /// </summary>
        /// <param name="reader">.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>Returns dynamic object list.</returns>
        public static List<SimpleDbRow> GetResultSetAsDbRow(
            this IDataReader reader, bool closeAtFinal = false)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            List<SimpleDbRow> list = new List<SimpleDbRow>();

            if (reader.IsClosed)
                return list;

            try
            {
                while (reader.Read())
                {
                    SimpleDbRow expando = reader.GetSimpleDbRow();
                    list.Add(expando);
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return list;
        }

        /// <summary>
        /// Gets the simple db row.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns>A SimpleDbRow.</returns>
        internal static SimpleDbRow GetSimpleDbRow(this IDataReader dataReader)
        {
            SimpleDbRow row = SimpleDbRow.NewRow();

            int fieldCount = dataReader.FieldCount;
            int columnCounter;

            for (int counter = 0; counter < fieldCount; counter++)
            {
                string columnName = dataReader.GetName(counter);
                Type type = dataReader.GetFieldType(counter);
                object value = dataReader.GetValue(counter);

                if (row.ContainsCellName(columnName))
                {
                    columnCounter = 1;
                    while (row.ContainsCellName(columnName))
                    {
                        columnName = $"{columnName}_{columnCounter}";
                        columnCounter++;
                    }
                }
                row.AddCell(columnName, type, value);
            }

            return row;
        }

        /// <summary>
        /// Gets the multi db row list.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="closeAtFinal">If true, close at final.</param>
        /// <returns>A list of List.</returns>
        public static List<List<SimpleDbRow>> GetMultiDbRowList(
            this IDataReader reader, bool closeAtFinal = false)
        {
            List<List<SimpleDbRow>> objDynList = new List<List<SimpleDbRow>>();

            try
            {
                do
                {
                    List<SimpleDbRow> resultSet = reader.GetResultSetAsDbRow();
                    objDynList.Add(resultSet);
                } while (reader.NextResult());
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return objDynList;
        }

        /// <summary>
        /// Returns First Row as dynamic object.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>An dynamic object.</returns>
        public static SimpleDbRow FirstDbRow(this IDataReader reader, bool closeAtFinal = false)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            SimpleDbRow row = SimpleDbRow.NewRow();

            if (reader.IsClosed)
                return row;

            try
            {
                if (reader.Read())
                {
                    row = reader.GetSimpleDbRow();
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return row;
        }

        /// <summary>
        /// Returns Last Row as dynamic object.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>An dynamic object.</returns>
        public static SimpleDbRow LastDbRow(this IDataReader reader, bool closeAtFinal = false)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            SimpleDbRow row = SimpleDbRow.NewRow();

            if (reader.IsClosed)
                return row;

            try
            {
                while (reader.Read())
                {
                    row = reader.GetSimpleDbRow();
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return row;
        }

        /// <summary>
        /// Returns Single Row as dynamic object.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <returns>An dynamic object.</returns>
        public static SimpleDbRow SingleDbRow(this IDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            SimpleDbRow expando = SimpleDbRow.NewRow();

            if (reader.IsClosed)
                return expando;

            try
            {
                if (reader.Read())
                {
                    expando = reader.GetSimpleDbRow();

                    if (reader.Read())
                        throw new Exception(DbAppMessages.SingleRowError);
                }
            }
            finally
            { reader.CloseIfNot(); }

            return expando;
        }

        /// <summary>
        /// Returns DataReader metadata as SimpleDbRow.
        /// </summary>
        /// <param name="dataReader">Datareader instance.</param>
        /// <returns>Returns SimpleDbRow object instance. </returns>
        public static SimpleDbRow GetDataReaderMetadataRow(this IDataReader dataReader)
        {
            SimpleDbRow readerMetadata = SimpleDbRow.NewRow();

            int fieldCount = dataReader.FieldCount;
            for (int counter = 0; counter < fieldCount; counter++)
            {
                string columnName = dataReader.GetName(counter);
                Type columnDataType = dataReader.GetFieldType(counter);

                if (readerMetadata.ContainsCellName(columnName))
                {
                    int columnCounter = 1;
                    while (readerMetadata.ContainsCellName(columnName))
                    {
                        columnName = $"{columnName}_{columnCounter}";
                        columnCounter++;
                    }
                }

                readerMetadata.AddCell(columnName, columnDataType, null);
            }

            return readerMetadata;
        }
    }
}