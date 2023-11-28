using Simply.Common.Objects;
using Simply.Data.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        /// Gets simple db row List with skip and take options.
        /// </summary>
        /// <param name="reader">Data reader object instance.</param>
        /// <param name="skip">Count for Skip.</param>
        /// <param name="take">Count for Take.</param>
        /// <param name="closeAtFinal">if true datareader will be closed at final else not.</param>
        //// <returns>Simple db row list.</returns>
        public static IEnumerable<SimpleDbRow> GetSimpleRowListSkipAndTake(this IDataReader reader,
           uint skip = 0, uint take = 0, bool closeAtFinal = false)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            try
            {
                uint counter = 0;
                if (!reader.IsClosed && take != 0)
                {
                    while (reader.Read())
                    {
                        if (counter < skip)
                            continue;

                        if (counter > (skip + take))
                            break;

                        counter++;
                        yield return reader.GetSimpleDbRow();
                    }
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }
        }

        /// <summary>
        /// Gets Result Set as simple row list.
        /// </summary>
        /// <param name="reader">.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        //// <returns>Simple db row list.</returns>
        public static IEnumerable<SimpleDbRow> GetResultSetAsDbRow(
            this IDataReader reader, bool closeAtFinal = false)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            try
            {
                if (!reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        yield return reader.GetSimpleDbRow();
                    }
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }
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
            List<List<SimpleDbRow>> multiSimpleDbRowList = new List<List<SimpleDbRow>>();

            try
            {
                do
                {
                    List<SimpleDbRow> simpleDbRowListlist = reader.GetResultSetAsDbRow()?.ToList() ?? new List<SimpleDbRow>();
                    multiSimpleDbRowList.Add(simpleDbRowListlist);
                } while (reader.NextResult());
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return multiSimpleDbRowList;
        }

        /// <summary>
        /// Returns First Row as dynamic object.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>An simple db row instance.</returns>
        public static SimpleDbRow FirstDbRow(this IDataReader reader, bool closeAtFinal = false)
        {
            if (reader is null)
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
        /// <returns>An simple db row instance.</returns>
        public static SimpleDbRow LastDbRow(this IDataReader reader, bool closeAtFinal = false)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            SimpleDbRow simpleDbRow = SimpleDbRow.NewRow();

            if (reader.IsClosed)
                return simpleDbRow;

            try
            {
                while (reader.Read())
                {
                    simpleDbRow = reader.GetSimpleDbRow();
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return simpleDbRow;
        }

        /// <summary>
        /// Returns Single Row as dynamic object.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <returns>An simple db row instance.</returns>
        public static SimpleDbRow SingleDbRow(this IDataReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            SimpleDbRow simpleDbRow = SimpleDbRow.NewRow();

            if (reader.IsClosed)
                return simpleDbRow;

            try
            {
                if (reader.Read())
                {
                    simpleDbRow = reader.GetSimpleDbRow();

                    if (reader.Read())
                        throw new Exception(DbAppMessages.SingleRowError);
                }
            }
            finally
            { reader.CloseIfNot(); }

            return simpleDbRow;
        }

        /// <summary>
        /// Returns DataReader metadata as SimpleDbRow.
        /// </summary>
        /// <param name="dataReader">Datareader instance.</param>
        /// <returns>Returns SimpleDbRow object instance. </returns>
        public static SimpleDbRow GetDataReaderMetadataRow(this IDataReader dataReader)
        {
            SimpleDbRow metaDataRow = SimpleDbRow.NewRow();

            int fieldCount = dataReader.FieldCount;
            for (int counter = 0; counter < fieldCount; counter++)
            {
                string columnName = dataReader.GetName(counter);
                Type columnDataType = dataReader.GetFieldType(counter);

                if (metaDataRow.ContainsCellName(columnName))
                {
                    int columnCounter = 1;
                    while (metaDataRow.ContainsCellName(columnName))
                    {
                        columnName = $"{columnName}_{columnCounter}";
                        columnCounter++;
                    }
                }

                metaDataRow.AddCell(columnName, columnDataType, null);
            }

            return metaDataRow;
        }

        /// <summary>
        /// Gets ordinal value.
        /// </summary>
        /// <typeparam name="T">Type for conversion.</typeparam>
        /// <param name="dr">data row object instance.</param>
        /// <param name="ordinal">data row column ordinal</param>
        /// <returns>Returns coverted value.</returns>
        public static T Get<T>(this IDataRecord dr, int ordinal) where T :
            struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            T nullValue = default(T);
            return dr.IsDBNull(ordinal) ? nullValue : (T)dr.GetValue(ordinal);
        }
    }
}