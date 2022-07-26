using Simply.Common.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DataReaderOperator"/>.
    /// </summary>
    public static class DataReaderOperator
    {
        /// <summary>
        /// Returns First Row as dynamic object.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>An dynamic object.</returns>
        public static dynamic FirstDynamicRow(this IDataReader reader, bool closeAtFinal = false)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            dynamic expando = new ExpandoObject();

            if (reader.IsClosed)
                return expando;

            try
            {
                if (reader.Read())
                {
                    expando = reader.GetDynamicRow();
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return expando;
        }

        /// <summary>
        /// Returns Last Row as dynamic object.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>An dynamic object.</returns>
        public static dynamic LastDynamicRow(this IDataReader reader, bool closeAtFinal = false)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            dynamic expando = new ExpandoObject();

            if (reader.IsClosed)
                return expando;

            try
            {
                while (reader.Read())
                {
                    expando = reader.GetDynamicRow();
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return expando;
        }

        /// <summary>
        /// GetDynamicResultSet Gets ResultSet as dynamic object list.
        /// </summary>
        /// <param name="reader">.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>Returns dynamic object list.</returns>
        public static List<dynamic> GetResultSetAsDynamic(
            this IDataReader reader, bool closeAtFinal = false)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            List<dynamic> list = new List<dynamic>();

            if (reader.IsClosed)
                return list;

            try
            {
                while (reader.Read())
                {
                    dynamic expando = reader.GetDynamicRow();
                    list.Add(expando);
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return list;
        }

        /// <summary>
        /// GetDynamicResultSetWithPaging Gets IDataReader ResultSet as dynamic Object list with paging.
        /// </summary>
        /// <param name="reader">.</param>
        /// <param name="pageNumber">(Optional) The page number.</param>
        /// <param name="pageItemCount">(Optional) Number of page ıtems.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>The dynamic result set with paging.</returns>
        public static List<dynamic> GetResultSetWithPagingAsDynamic(this IDataReader reader,
            uint pageNumber = 1, uint pageItemCount = 10, bool closeAtFinal = false)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            List<dynamic> list = new List<dynamic>();

            if (reader.IsClosed)
                return list;

            try
            {
                uint cntr = 1;
                uint max = pageNumber * pageItemCount;
                uint min = (pageNumber - 1) * pageItemCount;

                while (reader.Read())
                {
                    if (cntr <= min)
                        continue;

                    if (cntr > max)
                        break;

                    cntr++;

                    dynamic expando = reader.GetDynamicRow();
                    list.Add(expando);
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return list;
        }

        /// <summary>
        /// GetMultiDynamicResultSet The GetMultiDynamicResultSet.
        /// </summary>
        /// <param name="reader">.</param>
        /// <param name="closeAtFinal">(Optional) .</param>
        /// <returns>The multi dynamic result set.</returns>
        public static List<List<dynamic>> GetMultiDynamicList(
            this IDataReader reader, bool closeAtFinal = false)
        {
            List<List<dynamic>> objDynList = new List<List<dynamic>>();

            try
            {
                do
                {
                    List<dynamic> resultSet = reader.GetResultSetAsDynamic();
                    objDynList.Add(resultSet);
                } while (reader.NextResult());
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return objDynList;
        }

        /// <summary>
        /// Gets IDataReader record as dynamic.
        /// </summary>
        /// <param name="dataReader">The dataReader to act on.</param>
        /// <returns>The dynamic object from data reader.</returns>
        internal static dynamic GetDynamicRow(this IDataReader dataReader)
        {
            IDictionary<string, object> dictionary = new ExpandoObject();

            int fieldCount = dataReader.FieldCount;
            string columnName;
            object value;
            int columnCounter;

            for (int counter = 0; counter < fieldCount; counter++)
            {
                columnName = dataReader.GetName(counter);
                value = dataReader.GetValue(counter);
                value = value == DBNull.Value ? null : value;

                if (dictionary.ContainsKey(columnName))
                {
                    columnCounter = 1;
                    while (dictionary.ContainsKey(columnName))
                    {
                        columnName = $"{columnName}_{columnCounter}";
                        columnCounter++;
                    }
                }

                dictionary[columnName] = value;
            }

            dynamic expando = dictionary as ExpandoObject;
            return expando;
        }

        /// <summary>
        /// GetDynamicResultSetSkipAndTake Get dynamic object List with skip and take options.
        /// </summary>
        /// <param name="reader">Data reader object instance.</param>
        /// <param name="skip">Count for Skip.</param>
        /// <param name="take">Count for Take.</param>
        /// <param name="closeAtFinal">if true datareader will be closed at final else not.</param>
        /// <returns>Returns dynamic object list.</returns>
        public static List<dynamic> GetDynamicReaderListSkipAndTake(this IDataReader reader,
           uint skip = 0, uint take = 0, bool closeAtFinal = false)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            List<dynamic> list = new List<dynamic>();

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
                    dynamic row = reader.GetDynamicRow();
                    list.Add(row);
                }
            }
            finally
            { if (closeAtFinal) reader.CloseIfNot(); }

            return list;
        }

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
        /// Returns Single Row as dynamic object.
        /// </summary>
        /// <param name="reader">The dataReader to act on.</param>
        /// <returns>An dynamic object.</returns>
        public static dynamic SingleDynamicRow(this IDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            dynamic expando = new ExpandoObject();

            if (reader.IsClosed)
                return expando;

            try
            {
                if (reader.Read())
                {
                    expando = reader.GetDynamicRow();

                    if (reader.Read())
                        throw new Exception("Sequence should not contains more than one element.");
                }
            }
            finally
            { reader.CloseIfNot(); }

            return expando;
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
                        throw new Exception("Sequence should not contains more than one element.");
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