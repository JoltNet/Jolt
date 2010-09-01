// ----------------------------------------------------------------------------
// Implements.cs
//
// Contains the definition of the Implements class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/27/2010 13:46:12
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Jolt.Testing.Assertions.NUnit.SyntaxHelpers
{
    public static class Implements
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EqualityAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are verified.
        /// </typeparam>
        ///
        /// <param name="argumentFactory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public static EqualityAxiomConstraint<T> EqualityAxiom<T>(IArgumentFactory<T> argumentFactory)
        {
            return new EqualityAxiomConstraint<T>(argumentFactory);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EquatableAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are verified.
        /// </typeparam>
        /// 
        /// <param name="argumentFactory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public static EquatableAxiomConstraint<T> EqualityAxiom<T>(IEquatableFactory<T> argumentFactory)
            where T: IEquatable<T>
        {
            return new EquatableAxiomConstraint<T>(argumentFactory);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ComparableAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are verified.
        /// </typeparam>
        ///
        /// <param name="argumentFactory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public static ComparableAxiomConstraint<T> EqualityAxiom<T>(IComparableFactory<T> argumentFactory)
            where T : IComparable<T>
        {
            return new ComparableAxiomConstraint<T>(argumentFactory);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EqualityCompararerAxiomConstraint&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type that the equality comparer validates.
        /// </typeparam>
        ///
        /// <param name="argumentFactory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        /// 
        /// <param name="comparer">
        /// The equality comparer instance whose equality semantics are verified.
        /// </param>
        public static EqualityComparerAxiomConstraint<T> EqualityAxiom<T>(IArgumentFactory<T> argumentFactory, IEqualityComparer<T> comparer)
        {
            return new EqualityComparerAxiomConstraint<T>(argumentFactory, comparer);
        }
    }
}