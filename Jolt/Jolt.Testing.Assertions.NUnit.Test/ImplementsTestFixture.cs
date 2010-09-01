// ----------------------------------------------------------------------------
// ImplementsTestFixture.cs
//
// Contains the definition of the ImplementsTestFixture class.
// Copyright 2010 Steve Guidi.
//
// File created: 8/31/2010 22:40:27
// ----------------------------------------------------------------------------

using Jolt.Testing.Assertions.NUnit.SyntaxHelpers;
using NUnit.Framework;

namespace Jolt.Testing.Assertions.NUnit.Test
{
    [TestFixture]
    public sealed class ImplementsTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the EqualityAxiom() method, returning
        /// an EqualityAxiomConstraint.
        /// </summary>
        [Test]
        public void EqualityAxiomConstraint()
        {
            ConstraintConstructionTests.EqualityAxiomConstraint<int>(Implements.EqualityAxiom);
        }

        /// <summary>
        /// Verifies the behavior of the EqualityAxiom() method, returning
        /// an EquatableAxiomConstraint.
        /// </summary> 
        [Test]
        public void EquatableAxiomConstraint()
        {
            ConstraintConstructionTests.EquatableAxiomConstraint<int>(Implements.EqualityAxiom);
        }

        /// <summary>
        /// Verifies the behavior of the EqualityAxiom() method, returning
        /// a ComparableAxiomConstraint.
        /// </summary> 
        [Test]
        public void ComparableAxiomConstraint()
        {
            ConstraintConstructionTests.ComparableAxiomConstraint<int>(Implements.EqualityAxiom);
        }

        /// <summary>
        /// Verifies the behavior of the EqualityAxiom() method, returning
        /// a EqualityCompararerAxiomConstraint.
        /// </summary> 
        [Test]
        public void EqualityComparerAxiomConstraint()
        {
            ConstraintConstructionTests.EqualityComparerAxiomConstraint<int>(Implements.EqualityAxiom);
        }
    }
}