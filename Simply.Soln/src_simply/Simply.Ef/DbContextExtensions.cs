using Simply.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace Simply.Ef
{
    /// <summary>
    ///
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Gets Type List of Entity classes which has no key property.
        /// </summary>
        /// <typeparam name="TContext">DbContext class</typeparam>
        /// <param name="context">DbContext class instance.</param>
        /// <returns>Returns Keyless Entity Type list.</returns>
        public static List<Type> GetKeylessEntityTypes<TContext>(this TContext context)
            where TContext : DbContext
        {
            var types = new List<Type>();

            (context?.GetType() ?? typeof(TContext))
            .GetRuntimeProperties()
            .Where(o =>
                o.PropertyType.IsGenericType &&
                o.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                o.PropertyType.GenericTypeArguments.Any())
                .Select(q => q.PropertyType.GenericTypeArguments)
                .ToList()
                .ForEach(q =>
                {
                    types.AddRange(q);
                });
            types = types.Distinct().ToList();
            types =
            types.Where(q => q.GetKeysOfType().Length < 1)
            .ToList() ?? new List<Type>();

            return types;
        }
    }
}