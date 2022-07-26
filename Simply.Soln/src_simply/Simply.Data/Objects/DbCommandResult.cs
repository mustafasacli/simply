using Simply.Common;
using Simply.Data.Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Simply.Data.Objects
{
    /// <summary>
    /// Defines the <see cref="DbCommandResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">.</typeparam>
    [DataContract]
    public class DbCommandResult<T> : IDbCommandResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandResult{T}"/> class.
        /// </summary>
        public DbCommandResult()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandResult{T}"/> class.
        /// </summary>
        /// <param name="t">The t <see cref="T"/>.</param>
        public DbCommandResult(T t)
        { Result = t; }

        /// <summary>
        /// Gets or sets the OutputValues Parameters for Output values.
        /// </summary>
        [DataMember]
        public IDictionary<string, object> AdditionalValues
        { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the OutputParameters.
        /// </summary>
        [DataMember]
        public DbCommandParameter[] OutputParameters
        { get; set; } = ArrayHelper.Empty<DbCommandParameter>();

        /// <summary>
        /// Gets or sets the Result value.
        /// </summary>
        [DataMember]
        public T Result
        { get; set; }

        /// <summary>
        /// Execution Result.
        /// </summary>
        public int ExecutionResult
        { get; set; }
    }
}