// ----------------------------------------------------------------------------
// EqualityAxiomAssertion.cs
//
// Contains the definition of the EqualityAxiomAssertion class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/8/2010 12:20:13
// ----------------------------------------------------------------------------

using System;

using Jolt.Testing.Properties;


namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Provides assertion methods for verifiying if the given type <typeparamref name="T"/>
    /// correctly implements equality semantics.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type whose equality semantics are validated.
    /// </typeparam>
    public class EqualityAxiomAssertion<T>
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="EqualityAxiomAssertion&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="factory">
        /// A factory that creates and modifies instances of <typeparamref name="T"/>.
        /// </param>
        public EqualityAxiomAssertion(IArgumentFactory<T> factory)
        {
            m_factory = factory;
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Vailidates equality semantics for <typeparamref name="T"/> by ensuring that all required
        /// equality axioms are supported.
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of <see cref="AssertionResult"/> containing the result of the assertion.
        /// </returns>
        public virtual AssertionResult Validate()
        {
            T x = m_factory.Create();
            T y = m_factory.Create();
            T z = m_factory.Create();

            if (!IsReflexive(x)) { return CreateReflexivityAxiomFailureResult(); }
            if (!IsSymmetric(x, y)) { return CreateSymmetryAxiomFailureResult(); }
            if (!IsTransitive(x, y, z)) { return CreateTransitivityAxiomFailureResult(); }
            if (!IsStateless(x, y)) { return CreateStatelessnessAxiomFailureResult(); }
            if (!IsNotEqualToNull(x)) { return CreateNullInequalityAxiomFailureResult(); }
            if (!IsHashCodeConsistent(x, y)) { return CreateHashCodeInconsistencyFailureResult(); }
            if (!IsHashCodeDistinct(x)) { return CreateHashCodeNotModifiedFailureResult(); }

            return new AssertionResult();
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the associated argument factory.
        /// </summary>
        internal IArgumentFactory<T> ArgumentFactory
        {
            get { return m_factory; }
        }

        #endregion

        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Determines if an instance of <typeparamref name="T"/> implements the
        /// reflexivity axiom.
        /// </summary>
        /// 
        /// <param name="x">
        /// The instance used to verify the axiom.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <seealso cref="AreEqual"/>(x, x) is true.
        /// Returns false otherwise.
        /// </returns>
        internal virtual bool IsReflexive(T x)
        {
            return AreEqual(x, x);
        }

        /// <summary>
        /// Determines if an instance of <typeparamref name="T"/> implements the
        /// symmetry axiom.
        /// </summary>
        /// 
        /// <param name="x">
        /// The first instance used to verify the axiom.
        /// </param>
        /// 
        /// <param name="y">
        /// The second instance used to verify the axiom.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <see cref="AreEqual"/>(x, y) equals <see cref="AreEqual"/>(y, x).
        /// Returns false otherwise.
        /// </returns>
        internal virtual bool IsSymmetric(T x, T y)
        {
            return AreEqual(x, y) == AreEqual(y, x);
        }

        /// <summary>
        /// Determines if an instance of <typeparamref name="T"/> implements the
        /// transitivity axiom.
        /// </summary>
        /// 
        /// <param name="x">
        /// The first instance used to verify the axiom.
        /// </param>
        /// 
        /// <param name="y">
        /// The first instance used to verify the axiom.
        /// </param>
        /// 
        /// <param name="y">
        /// The third instance used to verify the axiom.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <see cref="AreEqual"/>(x, y), <see cref="AreEqual"/>(y, z), and
        /// <see cref="AreEqual"/>(x, z) all return true.
        /// Returns false otherwise.
        /// </returns>
        internal virtual bool IsTransitive(T x, T y, T z)
        {
            return AreEqual(x, y) && AreEqual(y, z) && AreEqual(x, z);
        }

        /// <summary>
        /// Determines if an instance of <typeparamref name="T"/> implements the
        /// statelessness axiom.
        /// </summary>
        /// 
        /// <param name="x">
        /// The first instance used to verify the axiom.
        /// </param>
        /// 
        /// <param name="y">
        /// The second instance used to verify the axiom.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <see cref="AreEqual"/>(x, y) returns the same result after repeated calls.
        /// Returns false otherwise.
        /// </returns>
        internal virtual bool IsStateless(T x, T y)
        {
            bool initialValue = AreEqual(x, y);
            for (sbyte i = 0; i < SByte.MaxValue; ++i)
            {
                if (AreEqual(x, y) != initialValue) { return false; }
            }

            return true;
        }

        /// <summary>
        /// Determines if an instance of <typeparamref name="T"/> implements the
        /// null-inequality axiom.
        /// </summary>
        /// 
        /// <param name="x">
        /// The instance used to verify the axiom.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <typeparamref name="T"/> is a value type, or if <seealso cref="AreEqual"/>(x, null) is false.
        /// Returns false otherwise.
        /// </returns>
        internal virtual bool IsNotEqualToNull(T x)
        {
            // Only applicable for reference types; returns true for all value types.
            return default(T) != null || !AreEqual(x, default(T));
        }

        /// <summary>
        /// Determines if an instance of <typeparamref name="T"/> implements the
        /// hash-code consistency axiom.
        /// </summary>
        /// 
        /// <param name="x">
        /// The first instance used to verify the axiom.
        /// </param>
        /// 
        /// <param name="y">
        /// The second instance used to verify the axiom.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <see cref="AreEqual"/>(x, y) returns true and <see cref="GetHashCode"/>(x) equals <see cref="GetHashCode(y)"/>.
        /// Returns false otherwise.
        /// </returns>
        internal virtual bool IsHashCodeConsistent(T x, T y)
        {
            return AreEqual(x, y) && GetHashCode(x) == GetHashCode(y);
        }

        /// <summary>
        /// Determines if an instance of <typeparamref name="T"/> implements the
        /// distinct hash-code axiom.
        /// </summary>
        /// 
        /// <param name="x">
        /// The instance used to verify the axiom.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if <see cref="GetHashCode"/>(x) changes, after the equality state of <paramref name="x"/> is modified.
        /// Returns false otherwise.
        /// </returns>
        internal virtual bool IsHashCodeDistinct(T x)
        {
            int initialHashCode = GetHashCode(x);
            m_factory.Modify(ref x);

            return GetHashCode(x) != initialHashCode;
        }

        #endregion

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// Determines if two instances of <typeparamref name="T"/> are equal.
        /// </summary>
        /// 
        /// <param name="x">
        /// The first instance to validate for equality.
        /// </param>
        /// 
        /// <param name="y">
        /// The second instance to validate for equality.
        /// </param>
        /// 
        /// <returns>
        /// True if <paramref name="x"/> equals <paramref name="y"/>.
        /// False otherwise.
        /// </returns>
        protected virtual bool AreEqual(T x, T y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Retrieves the hash code for a given instance of <typeparamref name="T"/>.
        /// </summary>
        /// 
        /// <param name="x">
        /// The instance whose hash code is retrieved.
        /// </param>
        protected virtual int GetHashCode(T x)
        {
            return x.GetHashCode();
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of <see cref="AssertionResult"/> denoting an assertion failure
        /// due to an incorrectly implemented symmetry axiom.
        /// </summary>
        /// 
        /// <returns>
        /// The created instance of <see cref="AssertionResult."/>
        /// </returns>
        private static AssertionResult CreateSymmetryAxiomFailureResult()
        {
            return new AssertionResult(
                false,
                String.Format(Resources.AssertionFailure_Equality_SymmetryAxiom, typeof(T).ToString()));
        }

        /// <summary>
        /// Creates a new instance of <see cref="AssertionResult"/> denoting an assertion failure
        /// due to an incorrectly implemented reflexivity axiom.
        /// </summary>
        /// 
        /// <returns>
        /// The created instance of <see cref="AssertionResult."/>
        /// </returns>
        private static AssertionResult CreateReflexivityAxiomFailureResult()
        {
            return new AssertionResult(
                false,
                String.Format(Resources.AssertionFailure_Equality_ReflexivityAxiom, typeof(T).ToString()));
        }

        /// <summary>
        /// Creates a new instance of <see cref="AssertionResult"/> denoting an assertion failure
        /// due to an incorrectly implemented transitivity axiom.
        /// </summary>
        /// 
        /// <returns>
        /// The created instance of <see cref="AssertionResult."/>
        /// </returns>
        private static AssertionResult CreateTransitivityAxiomFailureResult()
        {
            return new AssertionResult(
                false,
                String.Format(Resources.AssertionFailure_Equality_TransitivityAxiom, typeof(T).ToString()));
        }

        /// <summary>
        /// Creates a new instance of <see cref="AssertionResult"/> denoting an assertion failure
        /// due to an incorrectly implemented statelessness axiom.
        /// </summary>
        /// 
        /// <returns>
        /// The created instance of <see cref="AssertionResult."/>
        /// </returns>
        private static AssertionResult CreateStatelessnessAxiomFailureResult()
        {
            return new AssertionResult(
                false,
                String.Format(Resources.AssertionFailure_Equality_StatelessnessAxiom, typeof(T).ToString()));
        }

        /// <summary>
        /// Creates a new instance of <see cref="AssertionResult"/> denoting an assertion failure
        /// due to an incorrectly implemented null-inequality axiom.
        /// </summary>
        /// 
        /// <returns>
        /// The created instance of <see cref="AssertionResult."/>
        /// </returns>
        private static AssertionResult CreateNullInequalityAxiomFailureResult()
        {
            return new AssertionResult(
                false,
                String.Format(Resources.AssertionFailure_Equality_NullInequality, typeof(T).ToString()));
        }

        /// <summary>
        /// Creates a new instance of <see cref="AssertionResult"/> denoting an assertion failure
        /// due to an incorrectly implemented hashcode consistency axiom.
        /// </summary>
        /// 
        /// <returns>
        /// The created instance of <see cref="AssertionResult."/>
        /// </returns>
        private static AssertionResult CreateHashCodeInconsistencyFailureResult()
        {
            return new AssertionResult(
                false,
                String.Format(Resources.AssertionFailure_Equality_HashCodeInconsistent, typeof(T).ToString()));
        }

        /// <summary>
        /// Creates a new instance of <see cref="AssertionResult"/> denoting an assertion failure
        /// due to an incorrectly implemented hash code modification axiom.
        /// </summary>
        /// 
        /// <returns>
        /// The created instance of <see cref="AssertionResult."/>
        /// </returns>
        private static AssertionResult CreateHashCodeNotModifiedFailureResult()
        {
            return new AssertionResult(
                false,
                String.Format(Resources.AssertionFailure_Equality_HashCodeNotModified, typeof(T).ToString()));
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IArgumentFactory<T> m_factory;

        #endregion
    }
}