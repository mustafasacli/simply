using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace Simply.Data.Objects
{
    /// <summary>
    /// Defines the <see cref="DbCommandParameter"/>.
    /// </summary>
    [DataContract]
    public class DbCommandParameter : DbParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandParameter"/> class.
        /// </summary>
        public DbCommandParameter()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandParameter"/> class.
        /// </summary>
        /// <param name="value">parameter value <see cref="object"/>.</param>
        /// <param name="dbType">parameter db type.</param>
        /// <param name="direction">parameter direction.</param>
        /// <param name="isnullable">is parameter nullable</param>
        public DbCommandParameter(object value, DbType? dbType = null,
            ParameterDirection direction = ParameterDirection.Input, bool? isnullable = null)
        {
            this.Value = value;
            this.ParameterDbType = dbType;
            this.Direction = direction;
            if (isnullable != null)
                this.IsNullable = isnullable.Value;
        }

        /// <summary>
        /// Gets or sets the ParameterName Parameter Name.
        /// </summary>
        [DataMember]
        public override string ParameterName
        { get; set; }

        /// <summary>
        /// Gets or sets the Value Parameter value.
        /// </summary>
        [DataMember]
        public override object Value
        { get; set; }

        /// <summary>
        /// Gets or sets the Direction Parameter Direction.
        /// </summary>
        [DataMember]
        public override ParameterDirection Direction
        { get; set; } = ParameterDirection.Input;

        /// <summary>
        /// Gets or sets a value indicating whether IsNullable.
        /// </summary>
        [DataMember]
        public override bool IsNullable
        { get; set; }

        /// <summary>
        /// Gets or sets the DbType Db Type of Parameter.
        /// </summary>
        [DataMember]
        public DbType? ParameterDbType
        { get; set; }

        /// <summary>
        /// Gets or sets the ParameterColumnName Parameter Column Name.
        /// </summary>
        [DataMember]
        public string ParameterColumnName
        { get; set; }

        /// <summary>
        /// Gets or sets the Precision Precision of Parameter.
        /// </summary>
        [DataMember]
        public byte? ParameterPrecision
        { get; set; }

        /// <summary>
        /// Gets or sets the Scale Scale of Parameter.
        /// </summary>
        [DataMember]
        public byte? ParameterScale
        { get; set; }

        /// <summary>
        /// Gets or sets the Size of Parameter.
        /// </summary>
        [DataMember]
        public int? ParameterSize
        { get; set; }

        /// <summary>
        /// Gets, Sets Parameter precision.
        /// </summary>
        [DataMember]
        public override byte Precision
        { get; set; }

        /// <summary>
        /// Gets, Sets Parameter scale.
        /// </summary>
        [DataMember]
        public override byte Scale
        { get; set; }

        /// <summary>
        /// Gets, Sets Parameter size.
        /// </summary>
        [DataMember]
        public override int Size
        { get; set; }

        /// <summary>
        /// Gets, Sets Parameter Db Type.
        /// </summary>
        [DataMember]
        public override DbType DbType
        { get; set; } = DbType.Object;

        /// <summary>
        /// Gets, Sets Source Column.
        /// </summary>
        [DataMember]
        public override string SourceColumn
        { get; set; }

        /// <summary>
        /// Gets, Sets Source Version.
        /// </summary>
        [DataMember]
        public override DataRowVersion SourceVersion
        { get; set; }

        /// <summary>
        /// Gets, sets Source Column Null Mapping.
        /// </summary>
        public override bool SourceColumnNullMapping
        { get; set; }

        /// <summary>
        /// Gets DbCommand Parameter as IDbDataParameter.
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter GetDbDataParameter()
        {
            if (ParameterDbType != null)
                DbType = ParameterDbType.Value;

            if (ParameterPrecision != null)
                Precision = ParameterPrecision.Value;

            if (ParameterScale != null)
                Scale = ParameterScale.Value;

            if (ParameterSize != null)
                Size = ParameterSize.Value;

            return this;
        }

        /// <summary>
        /// Resets Database Type.
        /// </summary>
        public override void ResetDbType()
        {
            DbType = DbType.Object;
        }
    }
}