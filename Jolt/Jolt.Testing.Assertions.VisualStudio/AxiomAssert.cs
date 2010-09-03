// ----------------------------------------------------------------------------
// AxiomAssert.cs
//
// Contains the definition of the AxiomAssert class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/14/2010 08:40:44
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jolt.Testing.Assertions.VisualStudio
{
    /// <summary>
    /// Provides assertion methods for verifying the semantics
    /// of a type through well-defined axioms.
    /// </summary>
    public static class AxiomAssert
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Asserts that <typeparamref name="T"/> implements the required equality axioms.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are validated.
        /// </typeparam>
        /// 
        /// <param name="factory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public static void Equality<T>(IArgumentFactory<T> factory)
        {
            InvokeAssertion(Factory.CreateEqualityAxiomAssertion(factory));
        }

        /// <summary>
        /// Asserts that <typeparamref name="T"/> implements the required equality axioms,
        /// for implementations of <see cref="System.IEquatable&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are validated.
        /// </typeparam>
        /// 
        /// <param name="factory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public static void Equality<T>(IEquatableFactory<T> factory)
            where T : IEquatable<T>
        {
            InvokeAssertion(Factory.CreateEquatableAxiomAssertion(factory));
        }

        /// <summary>
        /// Asserts that <typeparamref name="T"/> implements the required equality axioms,
        /// for implementations of <see cref="System.IComparable&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are validated.
        /// </typeparam>
        /// 
        /// <param name="factory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public static void Equality<T>(IComparableFactory<T> factory)
            where T : IComparable<T>
        {
            InvokeAssertion(Factory.CreateComparableAxiomAssertion(factory));
        }

        /// <summary>
        /// Asserts that the given <see cref="System.Collections.Generic.IEqualityComparer&lt;T&gt;"/> implements
        /// the required equality axioms.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type specialization for the equality comparer to validate.
        /// </typeparam>
        /// 
        /// <param name="factory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        /// 
        /// <param name="comparer">
        /// The equality comparer instance whose equality semantics are verified.
        /// </param>
        public static void Equality<T>(IArgumentFactory<T> factory, IEqualityComparer<T> comparer)
        {
            InvokeAssertion(Factory.CreateEqualityComparerAxiomAssertion(factory, comparer));
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Invokes the given assertion, raising an exception on failure.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are validated.
        /// </typeparam>
        /// 
        /// <param name="assertion">
        /// The assertion to invoke.
        /// </param>
        /// 
        /// <exception cref="AssertFailedException">
        /// <paramref name="assertion"/> returned a failed result when invoked.
        /// </exception>
        private static void InvokeAssertion<T>(EqualityAxiomAssertion<T> assertion)
        {
            AssertionResult assertionResult = assertion.Validate();
            if (!assertionResult.Result)
            {
                throw new AssertFailedException(assertionResult.Message);
            }
        }

        #endregion

        #region internal fields -------------------------------------------------------------------

        internal static IAssertionFactory Factory = new AssertionFactory();

        #endregion
    }
}