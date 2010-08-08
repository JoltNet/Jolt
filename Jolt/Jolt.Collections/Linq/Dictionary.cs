// ----------------------------------------------------------------------------
// Dictionary.cs
//
// Contains the definition of the Dictionary class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/7/2010 20:00:02
// ----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Jolt.Collections.Linq
{
    /// <summary>
    /// Defines extension methods for <see cref="System.Collections.Generic.IDictionary&lt;TKey, TValue&gt;"/>
    /// </summary>
    public static class Dictionary
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ReadOnlyDictionary&lt;TKey, TValue&gt;"/> class
        /// and adapts a given dictionary.
        /// </summary>
        /// 
        /// <typeparam name="TKey">
        /// The type of keys in the dictionary.
        /// </typeparam>
        /// 
        /// <typeparam name="TValue">
        /// The type of values in the dictionary.
        /// </typeparam>
        /// 
        /// <param name="source">
        /// The <see cref="System.Collections.Generic.IDictionary&lt;TKey, TValue&gt;"/> to adapt.
        /// </param>
        /// 
        /// <returns>
        /// A new <see cref="ReadOnlyDictionary&lt;TKey, TValue&gt;"/> instance that adapts <paramref name="source"/>.
        /// </returns>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            return new ReadOnlyDictionary<TKey, TValue>(source);
        }
    }
}