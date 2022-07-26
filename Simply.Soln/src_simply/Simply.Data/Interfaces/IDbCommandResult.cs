using Simply.Data.Objects;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Simply.Data.Interfaces
{
    /// <summary>
    /// Object contains Result and Output parameter values.
    /// </summary>
    /// <typeparam name="T">T class.</typeparam>
    public interface IDbCommandResult<T>
    {
        /// <summary>
        /// Gets or sets the AdditionalValues
        /// Parameters for Additional values.
        /// </summary>
        [DataMember]
        IDictionary<string, object> AdditionalValues
        { get; set; }

        /// <summary>
        /// Gets or sets the OutputParameters.
        /// </summary>
        DbCommandParameter[] OutputParameters
        { get; set; }

        /// <summary>
        /// Gets or sets the Result
        /// Result value.
        /// </summary>
        [DataMember]
        T Result
        { get; set; }

        /// <summary>
        /// Execution Result.
        /// </summary>
        int ExecutionResult
        { get; set; }
    }
}