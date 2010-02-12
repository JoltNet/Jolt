using System.Collections.Generic;
using System.Linq;

using Jolt.Functional;

namespace Jolt.Linq
{
    public static class Enumerable
    {
        /// <summary>
        /// Creates an <see cref="IEnumerable&lt;T&gt;"/> that is not down-castable to its
        /// concrete collection type.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of element contained in the enumerator and source collection.
        /// </typeparam>
        /// 
        /// <param name="source">
        /// The collection to adapt to the non-castable reference.
        /// </param>
        /// 
        /// <returns>
        /// A new <see cref="IEnumerable&lt;T&gt;"/> instance that provides a new enumerator
        /// to the given collection.
        /// </returns>
        public static IEnumerable<T> AsNonCastableEnumerable<T>(this IEnumerable<T> source)
        {
            return source.Select(Functor.Identity<T>());
        }
    }
}