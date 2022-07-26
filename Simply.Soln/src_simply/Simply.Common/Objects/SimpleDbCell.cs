using System;

namespace Simply.Common.Objects
{
    /// <summary>
    /// The simple db cell.
    /// </summary>
    public class SimpleDbCell
    {
        /// <summary>
        /// Gets or sets the cell name.
        /// </summary>
        public string CellName
        { get; set; }

        /// <summary>
        /// Gets or sets the cell type.
        /// </summary>
        public Type CellType
        { get; set; }

        private object _value = null;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value.GetValueWithCheckDbNull(); }
        }

        /// <summary>
        /// Equals the.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>A bool.</returns>
        public override bool Equals(object obj)
        {
            bool result = false;

            if (obj is SimpleDbCell)
            {
                SimpleDbCell cell = obj as SimpleDbCell;
                result = cell.CellName == this.CellName && cell.CellType == this.CellType;
            }

            return result;
        }
    }
}