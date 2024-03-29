﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Simply.Common.Objects
{
    /// <summary>
    /// The simple db row.
    /// </summary>
    public class SimpleDbRow : DynamicObject
    {
        private readonly List<SimpleDbCell> cells;

        /// <summary>
        /// Prevents a default instance of the <see cref="SimpleDbRow"/> class from being created.
        /// </summary>
        private SimpleDbRow()
        {
            cells = new List<SimpleDbCell>();
        }

        /// <summary>
        /// News the row.
        /// </summary>
        /// <param name="throwOnColumnDuplicate">If true, throw on column duplicate.</param>
        /// <returns>A ISimpleDbRow.</returns>
        public static SimpleDbRow NewRow(bool throwOnColumnDuplicate = true)
        {
            return new SimpleDbRow() { ThrowOnColumnDuplicate = throwOnColumnDuplicate };
        }

        /// <summary>
        /// Gets the cell count.
        /// </summary>
        public int CellCount
        { get { return cells?.Count ?? 0; } }

        /// <summary>
        /// Gets a value indicating whether throw on column duplicate.
        /// </summary>
        public bool ThrowOnColumnDuplicate
        { get; private set; }

        /// <summary>
        /// Gets the cells.
        /// </summary>
        public List<SimpleDbCell> Cells
        { get { return cells; } }

        /// <summary>
        /// Gets value of element at given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get { return GetValue(index); }
        }

        /// <summary>
        /// Gets value of element at given cell name
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        public object this[string cellName]
        {
            get { return GetValue(cellName); }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="indx">The indx.</param>
        /// <returns>A Type.</returns>
        public Type GetType(int indx)
        {
            CheckIndexIsValid(indx);
            return Cells[indx].CellType;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="cellName">The column name.</param>
        /// <returns>A Type.</returns>
        public Type GetType(string cellName)
        {
            CheckColumnNameIsNull(cellName);
            SimpleDbCell cell = Cells.FirstOrDefault(q => q.CellName == cellName);
            if (cell == null)
                throw new ArgumentException($"Sequence not contains {cellName} given name.");

            return cell.CellType;
        }

        /// <summary>
        /// Removes the cell with given name.
        /// </summary>
        /// <param name="cellName">The column name.</param>
        /// <returns>An int.</returns>
        public int RemoveCell(string cellName)
        {
            CheckColumnNameIsNull(cellName);
            SimpleDbCell cell = Cells.FirstOrDefault(q => q.CellName == cellName);
            int result = 0;

            if (cell == null)
                return result;

            int indx = Cells.IndexOf(cell);
            if (indx == -1)
                return indx;

            Cells.RemoveAt(indx);
            result = 1;
            return result;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="index">The indx.</param>
        /// <returns>An object.</returns>
        public object GetValue(int index)
        {
            CheckIndexIsValid(index);
            return Cells[index].Value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="cellName">The cell name.</param>
        /// <returns>An object.</returns>
        public object GetValue(string cellName)
        {
            CheckColumnNameIsNull(cellName);
            SimpleDbCell cell = Cells.FirstOrDefault(q => q.CellName == cellName);
            if (cell == null)
                throw new ArgumentException($"Sequence not contains cell with \"{cellName}\" given name.");

            return cell.Value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="indx">The indx.</param>
        /// <returns>A T.</returns>
        public T GetValue<T>(int indx) where T : struct
        {
            object value = GetValue(indx);
            T instance = (T)value;
            return instance;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="indx">The indx.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A T.</returns>
        public T GetValue<T>(int indx, T defaultValue) where T : struct
        {
            object value = GetValue(indx);
            T instance = value.IsNullOrDbNull() ? defaultValue : (T)value;
            return instance;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>A T.</returns>
        public T GetValue<T>(string columnName) where T : struct
        {
            object value = GetValue(columnName);
            T instance = (T)value;
            return instance;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="cellName">The column name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A T.</returns>
        public T GetValue<T>(string cellName, T defaultValue) where T : struct
        {
            object value = GetValue(cellName);
            T instance = value.IsNullOrDbNull() ? defaultValue : (T)value;
            return instance;
        }

        /// <summary>
        /// Adds the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public void AddCell(SimpleDbCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));

            if (cell.CellName.IsNullOrSpace())
                throw new Exception("Columns name can not be empty.");

            int index = Cells.IndexOf(cell);
            if (index != -1 && ThrowOnColumnDuplicate)
                throw new Exception($"Columns duplicated with name({cell.CellName}).");

            if (index == -1)
                Cells.Add(new SimpleDbCell
                {
                    CellName = cell.CellName,
                    CellType = cell.CellType,
                    Value = cell.Value
                });
        }

        /// <summary>
        /// Adds the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns>Returns current instance.</returns>
        public SimpleDbRow AddCellAndReturn(SimpleDbCell cell)
        {
            AddCell(cell);
            return this;
        }

        /// <summary>
        /// Adds the cell.
        /// </summary>
        /// <param name="cellName">The column name.</param>
        /// <param name="columnType">The column type.</param>
        /// <param name="cellValue">The cell value.</param>
        public void AddCell(string cellName, Type columnType, object cellValue)
        {
            AddCell(new SimpleDbCell { CellName = cellName, CellType = columnType, Value = cellValue });
        }

        /// <summary>
        /// Adds the cell.
        /// </summary>
        /// <param name="cellName">The column name.</param>
        /// <param name="columnType">The column type.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <returns>Returns current instance.</returns>
        public SimpleDbRow AddCellAndReturn(string cellName, Type columnType, object cellValue)
        {
            AddCell(new SimpleDbCell { CellName = cellName, CellType = columnType, Value = cellValue });
            return this;
        }

        /// <summary>
        /// Contains the cell name.
        /// </summary>
        /// <param name="cellName">The cell name.</param>
        /// <returns>A bool.</returns>
        public bool ContainsCellName(string cellName)
        {
            bool result = cells.Any(q => q.CellName == cellName);
            return result;
        }

        /// <summary>
        /// Gets the dynamic member names.
        /// </summary>
        /// <returns>A list of string.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return cells.Select(q => q.CellName);
        }

        /// <summary>
        /// Tries the get member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="result">The result.</param>
        /// <returns>A bool.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetValue(binder.Name);
            return true;
        }

        /// <summary>
        /// Tries the set member.
        /// </summary>
        /// <param name="binder">The binder.</param>
        /// <param name="value">The value.</param>
        /// <returns>A bool.</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
            // return base.TrySetMember(binder, value);
        }

        /// <summary>
        /// Gets the cell values.
        /// </summary>
        /// <param name="cellNames">The cell names.</param>
        /// <returns>An array of object.</returns>
        public object[] GetCellValues(IEnumerable<string> cellNames)
        {
            object[] values = null;
            List<string> cellNameList = cellNames?.Where(q => q.IsValid())?.ToList() ?? ArrayHelper.EmptyList<string>();
            if (cellNameList.Any())
            {
                List<string> notContainedCells = cells.Select(q => q.CellName).Where(q => !cellNameList.Contains(q)).ToList() ?? ArrayHelper.EmptyList<string>();
                if (notContainedCells.Any())
                    throw new Exception($"{string.Join(",", notContainedCells)} cell names not contained to cell.");

                values = cells.Where(q => cellNameList.Contains(q.CellName)).Select(q => q.Value).ToArray();
            }

            return values ?? ArrayHelper.Empty<object>();
        }

        #region [ protected methods ]

        /// <summary>
        /// Checks the column name ıs null.
        /// </summary>
        /// <param name="cellName">The column name.</param>
        protected void CheckColumnNameIsNull(string cellName)
        {
            if (cellName.IsNullOrSpace())
                throw new ArgumentNullException(nameof(cellName));
        }

        /// <summary>
        /// Checks the index is valid.
        /// </summary>
        /// <param name="indx">The indx.</param>
        protected void CheckIndexIsValid(int indx)
        {
            if (indx < 0 || indx >= CellCount)
                throw new IndexOutOfRangeException($"Invalid index: {indx}");
        }

        #endregion [ protected methods ]
    }
}