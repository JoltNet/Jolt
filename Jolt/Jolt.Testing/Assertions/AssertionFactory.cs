// ----------------------------------------------------------------------------
// AssertionFactory.cs
//
// Contains the definition of the AssertionFactory class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/31/2009 11:41:17
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Schema;

namespace Jolt.Testing.Assertions
{
    internal sealed class AssertionFactory : IAssertionFactory
    {
        #region IAssertionFactory members ---------------------------------------------------------

        /// <summary>
        /// <see cref="IAssertionFactory.CreateXmlValidityAssertion(XmlSchemaSet)"/>
        /// </summary>
        XmlValidityAssertion IAssertionFactory.CreateXmlValidityAssertion(XmlSchemaSet schemas)
        {
            return new XmlValidityAssertion(schemas);
        }

        /// <summary>
        /// <see cref="IAssertionFactory.CreateXmlValidityAssertion(XmlSchemaSet, XmlSchemaValidationFlags)"/>
        /// </summary>
        XmlValidityAssertion IAssertionFactory.CreateXmlValidityAssertion(XmlSchemaSet schemas, XmlSchemaValidationFlags flags)
        {
            return new XmlValidityAssertion(schemas, flags);
        }

        /// <summary>
        /// <see cref="IAssertionFactory.CreateXmlEqualityAssertion"/>
        /// </summary>
        XmlEqualityAssertion IAssertionFactory.CreateXmlEqualityAssertion()
        {
            return new XmlEqualityAssertion();
        }

        /// <summary>
        /// <see cref="IAssertionFactory.CreateXmlEquivalencyAssertion"/>
        /// </summary>
        XmlEquivalencyAssertion IAssertionFactory.CreateXmlEquivalencyAssertion(XmlComparisonFlags strictness)
        {
            return new XmlEquivalencyAssertion(strictness);
        }

        /// <summary>
        /// <see cref="IAssertionFactory.CreateEqualityAxiomAssertion&lt;T&gt;>"/>
        /// </summary>
        EqualityAxiomAssertion<T> IAssertionFactory.CreateEqualityAxiomAssertion<T>(IArgumentFactory<T> argumentFactory)
        {
            return new EqualityAxiomAssertion<T>(argumentFactory);
        }

        /// <summary>
        /// <see cref="IAssertionFactory.CreateEquatableAxiomAssertion&lt;T&gt;>"/>
        /// </summary>
        EquatableAxiomAssertion<T> IAssertionFactory.CreateEquatableAxiomAssertion<T>(IEquatableFactory<T> argumentFactory)
        {
            return new EquatableAxiomAssertion<T>(argumentFactory);
        }

        /// <summary>
        /// <see cref="IAssertionFactory.CreateComparerAxiomAssertion&lt;T&gt;>"/>
        /// </summary>
        ComparableAxiomAssertion<T> IAssertionFactory.CreateComparableAxiomAssertion<T>(IComparableFactory<T> argumentFactory)
        {
            return new ComparableAxiomAssertion<T>(argumentFactory);
        }

        /// <summary>
        /// <see cref="IAssertionFactory.CreateEqualityComparerAxiomAssertion&lt;T&gt;>"/>
        /// </summary>
        EqualityComparerAxiomAssertion<T> IAssertionFactory.CreateEqualityComparerAxiomAssertion<T>(IArgumentFactory<T> argumentFactory, IEqualityComparer<T> comparer)
        {
            return new EqualityComparerAxiomAssertion<T>(argumentFactory, comparer);
        }

        #endregion
    }
}