using Simply.Common;
using Simply.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Simply.Data
{
    /// <summary>
    /// Defines the <see cref="DataExtensions" />.
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// convert datatable To List.
        /// </summary>
        /// <typeparam name="T">T object type.</typeparam>
        /// <param name="datatable">Datatable object.</param>
        /// <param name="accordingToColumn">The accordingToColumn<see cref="bool"/>.</param>
        /// <returns>Returns A List of T object.</returns>
        public static List<T> ToList<T>(this DataTable datatable, bool accordingToColumn) where T : new()
        {
            List<T> liste = new List<T>();

            object rowCellValue;
            T item;

            PropertyInfo[] properties = typeof(T).GetProperties();
            if (accordingToColumn)
            {
                PropertyInfo propertyInfo;
                foreach (DataRow row in datatable.Rows)
                {
                    item = new T();
                    foreach (DataColumn col in datatable.Columns)
                    {
                        rowCellValue = row[col.ColumnName];
                        if (rowCellValue != null && rowCellValue != DBNull.Value)
                        {
                            propertyInfo = properties.FirstOrDefault(p => p.Name == col.ColumnName);
                            propertyInfo?.SetValue(item, rowCellValue);
                        }
                    }
                    liste.Add(item);
                }
            }
            else
            {
                foreach (DataRow row in datatable.Rows)
                {
                    item = new T();
                    for (int proCounter = 0; proCounter < properties.Length; proCounter++)
                    {
                        rowCellValue = row[properties[proCounter].Name];
                        if (!rowCellValue.IsNullOrDbNull())
                            properties[proCounter].SetValue(item, rowCellValue.GetValueWithCheckDbNull());
                    }
                    liste.Add(item);
                }
            }

            return liste;
        }

        /// <summary>
        /// convert datatable To List.
        /// </summary>
        /// <typeparam name="T">T object type.</typeparam>
        /// <param name="datatable">Datatable object.</param>
        /// <param name="unForceNullValueBind"></param>
        /// <returns>   Returns A List of T object.</returns>
        public static List<T> ToList_V2<T>(this DataTable datatable, bool unForceNullValueBind = true) where T : new()
        {
            PropertyInfo[] properties = typeof(T).GetValidPropertiesOfTypeV2();
            IDictionary<string, string> column2PropertyMap = properties.GetColumnsReverse();
            DataColumnCollection columnColection = datatable.Columns;
            List<string> columnNames = column2PropertyMap.Select(q => q.Key).Where(q => columnColection.Contains(q)).ToList() ?? new List<string>();
            List<string> propertyNames = column2PropertyMap.Where(q => columnNames.Contains(q.Key)).Select(q => q.Value).ToList() ?? new List<string>();
            PropertyInfo[] rowProperties = properties.Where(q => propertyNames.Contains(q.Name)).ToArray() ?? new PropertyInfo[0];
            column2PropertyMap = rowProperties.GetColumns();

            List<T> liste = datatable.AsEnumerable().Select(row => Row2Instance<T>(row, rowProperties, column2PropertyMap, unForceNullValueBind: unForceNullValueBind)).ToList();

            return liste;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="rowProperties"></param>
        /// <param name="column2PropertyMap"></param>
        /// <param name="unForceNullValueBind"></param>
        /// <returns></returns>
        private static T Row2Instance<T>(DataRow row, PropertyInfo[] rowProperties,
            IDictionary<string, string> column2PropertyMap, bool unForceNullValueBind = true) where T : new()
        {
            T instance = new T();

            foreach (PropertyInfo propertyInfo in rowProperties)
            {
                propertyInfo.SetValue(instance, row[column2PropertyMap[propertyInfo.Name]].GetValueWithCheckDbNull(), null);
            }

            return instance;
        }

        /// <summary>
        /// Copies datatable to a new datatble.
        /// </summary>
        /// <param name="datatable">DataTable object.</param>
        /// <returns>A DataTable.</returns>
        public static DataTable CopyColumnsAsDatatable(this DataTable datatable)
        {
            DataTable newDatatable = new DataTable();

            foreach (DataColumn col in datatable.Columns)
            {
                newDatatable.Columns.Add(col.ColumnName, col.DataType);
            }

            return newDatatable;
        }

        /// <summary>
        /// Gets Page Of DataTable.
        /// </summary>
        /// <param name="datatable">DataTable object.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="rowCount">Number of rows.</param>
        /// <returns>The page of data table.</returns>
        public static DataTable GetPageOfDataTable(this DataTable datatable, int pageNumber, int rowCount)
        {
            if (datatable == null)
                throw new System.NullReferenceException("DataTable object can not be null.");

            if (pageNumber < 0)
                throw new Exception("Page Number cannot be less than 0.");

            if (rowCount < 1)
                throw new Exception("Row Count of Page cannot be less than 1.");

            DataTable data = datatable.CopyColumnsAsDatatable();

            int totalRow = datatable.Rows.Count;
            int rowNo = pageNumber * rowCount;
            if (totalRow > rowNo)
            {
                int rowForCount = totalRow > (rowNo + rowCount) ? rowCount : (totalRow - rowNo);

                for (int rowForCounter = 0; rowForCounter < rowForCount; rowForCounter++)
                {
                    DataRow row = datatable.Rows[rowNo + rowForCounter];
                    data.Rows.Add(row.ItemArray);
                }
            }

            return data;
        }

        /// <summary>
        /// Gets Columns Of DataTable.
        /// </summary>
        /// <param name="datatable">DataTable object.</param>
        /// <param name="columnList">column names array.</param>
        /// <returns>Returns a DataTable with Selected column names.</returns>
        public static DataTable GetColumnsOfDataTable(this DataTable datatable, string[] columnList)
        {
            DataTable newDatatable = new DataTable();
            DataColumn dtColumn;

            foreach (string columnName in columnList)
            {
                dtColumn = new DataColumn(datatable.Columns[columnName].ColumnName, datatable.Columns[columnName].DataType);
                newDatatable.Columns.Add(dtColumn);
            }
            List<object> rowItems;
            foreach (DataRow row in datatable.Rows)
            {
                rowItems = new List<object>();
                foreach (string colName in columnList)
                {
                    rowItems.Add(row[colName]);
                }
                newDatatable.Rows.Add(rowItems.ToArray());
            }

            return newDatatable;
        }

        /// <summary>
        /// Gets Columns Of DataTable.
        /// </summary>
        /// <param name="datatable">DataTable object.</param>
        /// <param name="columnList">column numbers array.</param>
        /// <returns>Returns a DataTable with Selected column numbers.</returns>
        public static DataTable GetColumnsOfDataTable(this DataTable datatable, int[] columnList)
        {
            DataTable newDatatable = new DataTable();
            DataColumn dtColumn;
            foreach (int colNo in columnList)
            {
                dtColumn = new DataColumn(datatable.Columns[colNo].ColumnName, datatable.Columns[colNo].DataType);
                newDatatable.Columns.Add(dtColumn);
            }

            List<object> rowItems;
            foreach (DataRow row in datatable.Rows)
            {
                rowItems = new List<object>();
                foreach (int colNo in columnList)
                {
                    rowItems.Add(row[colNo]);
                }
                newDatatable.Rows.Add(rowItems.ToArray());
            }

            return newDatatable;
        }

        /// <summary>
        /// Gets Object With Selected Column.
        /// </summary>
        /// <param name="datatable">DataTable object.</param>
        /// <param name="refColumn">Name of Reference Column.</param>
        /// <param name="refValue">Value of Reference Column.</param>
        /// <param name="destinationColumn">Name of Destination Column.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public static object GetObjectWithSelectedColumn(this DataTable datatable, string refColumn, object refValue, string destinationColumn)
        {
            object retObj = null;

            foreach (DataRow row in datatable.Rows)
            {
                if (row[refColumn] == refValue)
                {
                    retObj = row[destinationColumn];
                    break;
                }
            }

            return retObj;
        }

        /// <summary>
        /// A DataTable extension method that export as excel with ınclude columns.
        /// </summary>
        /// <param name="datatable">DataTable object.</param>
        /// <param name="fileName">Filename of the file.</param>
        /// <param name="includeColumns">A variable-length parameters list containing include columns.</param>
        public static void ExportAsExcelWithIncludeColumns(this DataTable datatable, string fileName, object[] includeColumns)
        {
            if (includeColumns.IsNull())
                return;

            using (StreamWriter sWriter = new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate)))
            {
                sWriter.AutoFlush = true;

                foreach (string col in includeColumns)
                {
                    sWriter.Write("{0}\t", col);
                }
                sWriter.Write("\n");

                foreach (DataRow rw in datatable.Rows)
                {
                    foreach (string col in includeColumns)
                    {
                        sWriter.Write("{0}\t", rw[col].ToStr().Replace("\n", " "));
                    }
                    sWriter.Write("\n");
                }
            }
        }

        /// <summary>
        /// A DataTable extension method that export as excel with exclude columns.
        /// </summary>
        /// <param name="datatable">               DataTable object.</param>
        /// <param name="fileName">         Filename of the file.</param>
        /// <param name="excludeColumns">   A variable-length parameters list containing exclude columns.</param>
        public static void ExportAsExcelWithExcludeColumns(this DataTable datatable, string fileName, object[] excludeColumns)
        {
            if (excludeColumns.IsNull())
            {
                using (StreamWriter sWriter = new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate)))
                {
                    sWriter.AutoFlush = true;

                    foreach (DataColumn col in datatable.Columns)
                    {
                        sWriter.Write("{0}\t", col.ColumnName);
                    }
                    sWriter.Write("\n");

                    foreach (DataRow rw in datatable.Rows)
                    {
                        foreach (DataColumn col in datatable.Columns)
                        {
                            sWriter.Write("{0}\t", rw[col].ToStr().Replace("\n", " "));
                        }
                        sWriter.Write("\n");
                    }
                }
            }
            else
            {
                List<string> colList = new List<string>();

                foreach (DataColumn col in datatable.Columns)
                {
                    colList.Add(col.ColumnName);
                }

                foreach (object obj in excludeColumns)
                {
                    if (colList.Contains(obj.ToStr()))
                    {
                        colList.Remove(obj.ToStr());
                    }
                }

                using (StreamWriter streamWriter = new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate)))
                {
                    streamWriter.AutoFlush = true;

                    foreach (string col in colList)
                    {
                        streamWriter.Write(string.Format("{0}\t", col));
                    }
                    streamWriter.Write("\n");

                    foreach (DataRow rw in datatable.Rows)
                    {
                        foreach (string col in colList)
                        {
                            streamWriter.Write("{0}\t", rw[col].ToStr().Replace("\n", " "));
                        }
                        streamWriter.Write("\n");
                    }
                }
            }
        }

        /// <summary>
        /// Copies datatable to a new datatable.
        /// </summary>
        /// <param name="datatable">   DataTable object.</param>
        /// <param name="pageInfo"></param>
        /// <returns>   A DataTable.</returns>
        public static DataTable CopyDatatable(this DataTable datatable, IPageInfo pageInfo = null)
        {
            if (pageInfo == null)
            {
                return datatable.Copy();
            }
            else
            {
                DataTable data = datatable.CopyColumnsAsDatatable();

                if (!pageInfo.IsPageable)
                {
                    return data;
                }

                int totalRowCount = datatable.Rows.Count;
                int skip = (int)pageInfo.Skip;
                int take = (int)pageInfo.Take;
                int rowCount = totalRowCount >= (skip + take) ? take : skip + take - totalRowCount;

                for (int counter = 0; counter < rowCount; counter++)
                {
                    int index = skip + 1 + counter;
                    data.Rows.Add(datatable.Rows[index].ItemArray);
                }

                return data;
            }
        }

        /// <summary>
        /// Get Some Columns As DataTable.
        /// </summary>
        /// <param name="datatable">DataTable object.</param>
        /// <param name="columnList">column names array.</param>
        /// <returns>some columns as table.</returns>
        public static DataTable GetSomeColumnsAsTable(this DataTable datatable, string[] columnList)
        {
            DataTable dtNew = new DataTable();
            if (datatable == null)
                return dtNew;

            if (columnList == null)
                return dtNew;

            if (columnList.Length == 0)
                return dtNew;

            DataColumn _col;
            foreach (string col in columnList)
            {
                _col = datatable.Columns[col];
                dtNew.Columns.Add(_col.ColumnName, _col.DataType);
            }

            DataRow dr;
            foreach (DataRow row in datatable.Rows)
            {
                dr = dtNew.NewRow();
                foreach (string col in columnList)
                {
                    dr[col] = row[col];
                }
                dtNew.Rows.Add(dr);
            }
            return dtNew;
        }

        /// <summary>
        /// convert datarow to T object instance.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="row">The row to act on.</param>
        /// <param name="columns">(Optional) The columns.</param>
        /// <returns>A T instance.</returns>
        public static T RowToObject<T>(this DataRow row, DataColumnCollection columns = null)
            where T : new()
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            T instance = Activator.CreateInstance<T>();

            if (columns == null)
                columns = row.Table.Columns;

            PropertyInfo[] props = typeof(T).GetProperties();

            props = props
                .AsQueryable()
                .Where(p => p.CanWrite && columns.Contains(p.Name))
                .ToArray();

            foreach (PropertyInfo p in props)
            {
                p.SetValue(instance, row[p.Name] == DBNull.Value ? null : row[p.Name]);
            }

            return instance;
        }

        /// <summary>
        /// Gets Column As Unique List.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dataTable">The dataTable to act on.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The column as unique list.</returns>
        public static List<T> GetColumnAsUniqueList<T>(this DataTable dataTable, string columnName)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentNullException(nameof(columnName));

            if (!dataTable.Columns.Contains(columnName))
                throw new ArgumentException("There is no column with given name in datatable.");

            List<T> result = (from row in dataTable.AsEnumerable()
                              select row.Field<T>(columnName))
                      .Distinct()
                      .ToList() ?? new List<T>();

            return result;
        }

        /// <summary>
        /// convert datatable to expandoobject list.
        /// </summary>
        /// <param name="table">The table to act on.</param>
        /// <returns>Table as a List{dynamic}.</returns>
        public static List<dynamic> ToDynamicList(this DataTable table)
        {
            List<dynamic> list = new List<dynamic>();

            if (table == null)
                return list;

            if (table.Rows.Count < 1)
                return list;

            List<string> columns = new List<string>();

            foreach (DataColumn col in table.Columns)
            {
                columns.Add(col.ColumnName);
            }

            foreach (DataRow row in table.Rows)
            {
                IDictionary<string, object> dict = new ExpandoObject();
                columns.ForEach(s => dict[s] = row[s] == DBNull.Value ? null : row[s]);
                ExpandoObject expando = dict as ExpandoObject;
                list.Add(expando as dynamic);
            }

            return list;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="table">datatable convert to dynamic object.</param>
        /// <returns>returns IEnumerable{dynamic} objects instances.</returns>
        public static IEnumerable<dynamic> AsDynamicEnumerable(this DataTable table)
        {
            // https://stackoverflow.com/questions/7794818/how-can-i-convert-a-datatable-into-a-dynamic-object
            return table.AsEnumerable().Select(row => new DynamicRow(row));
        }
    }

    internal sealed class DynamicRow : DynamicObject
    {
        // https://stackoverflow.com/questions/7794818/how-can-i-convert-a-datatable-into-a-dynamic-object
        private readonly DataRow _row;

        internal DynamicRow(DataRow row)
        { _row = row; }

        // Interprets a member-access as an indexer-access on the
        // contained DataRow.
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var returnValue = _row.Table.Columns.Contains(binder.Name);
            result = returnValue ? _row[binder.Name] : null;
            return returnValue;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var returnValue = _row.Table.Columns.Contains(binder.Name);
            if (returnValue)
            {
                _row[binder.Name] = value;
            }

            return returnValue;
            //return base.TrySetMember(binder, value);
        }
    }
}