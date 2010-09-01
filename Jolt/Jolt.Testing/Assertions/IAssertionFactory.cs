// ----------------------------------------------------------------------------
// IAssertionFactory.cs
//
// Contains the definition of the IAssertionFactory interface.
// Copyright 2009 Steve Guidi.
//
// File created: 8/31/2009 11:14:13
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Defines a contract for a factory class that creates core assertions.
    /// </summary>
    internal interface IAssertionFactory
    {
        /// <summary>
        /// <see cref="XmlValidityAssertion.ctor(XmlSchemaSet)"/>
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlValidityAssertion"/> class, initialized
        /// with <paramref name="schemas"/>.
        /// </returns>
        XmlValidityAssertion CreateXmlValidityAssertion(XmlSchemaSet schemas);

        /// <summary>
        /// <see cref="XmlValidityAssertion.ctor(XmlSchemaSet, XmlSchemaValidationFlags)"/>
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlValidityAssertion"/> class, initialized
        /// with <paramref name="schemas"/> and <paramref name="flags"/>.
        /// </returns>
        XmlValidityAssertion CreateXmlValidityAssertion(XmlSchemaSet schemas, XmlSchemaValidationFlags flags);

        /// <summary>
        /// <see cref="XmlEqualityAssertion.ctor()"/>
        /// </summary>
        /// 
        /// <returns>
        /// The newly created instance of the <see cref="XmlEqualityAssertion"/> class.
        /// </returns>
        XmlEqualityAssertion CreateXmlEqualityAssertion();

        /// <summary>
        /// <see cref="XmlEquivalencyAssertion.ctor(XmlComparisonFlags)"/>
        /// </summary>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlEquivalencyAssertion"/>, initialized with
        /// <paramref name="strictness"/>.
        /// </returns>
        XmlEquivalencyAssertion CreateXmlEquivalencyAssertion(XmlComparisonFlags strictness);

        /// <summary>
        /// <see cref="EqualityAxiomAssertion&lt;T&gt;.ctor(IArgumentFactory&lt;T&gt;)"/>
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are validated.
        /// </typeparam>
        ///
        /// <returns>
        /// A new instance of the <see cref="EqualityAxiomAssertion&lt;T&gt;"/> class,
        /// initialized with <param name="argumentFactory"/>.
        /// </returns>
        EqualityAxiomAssertion<T> CreateEqualityAxiomAssertion<T>(IArgumentFactory<T> argumentFactory);

        /// <summary>
        /// <see cref="EquatableAxiomAssertion&lt;T&gt;.ctor(IEquatableFactory&lt;T&gt;)"/>
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are validated.
        /// </typeparam>
        ///
        /// <returns>
        /// A new instance of the <see cref="EquatableAxiomAssertion&lt;T&gt;"/> class,
        /// initialized with <param name="argumentFactory"/>.
        /// </returns>
        EquatableAxiomAssertion<T> CreateEquatableAxiomAssertion<T>(IEquatableFactory<T> argumentFactory)
            where T : IEquatable<T>;

        /// <summary>
        /// <see cref="ComparableAxiomAssertion&lt;T&gt;.ctor(IComparableFactory&lt;T&gt;)"/>
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type whose equality semantics are validated.
        /// </typeparam>
        ///
        /// <returns>
        /// A new instance of the <see cref="ComparableAxiomAssertion&lt;T&gt;"/> class,
        /// initialized with <param name="argumentFactory"/>.
        /// </returns>
        ComparableAxiomAssertion<T> CreateComparableAxiomAssertion<T>(IComparableFactory<T> argumentFactory)
            where T : IComparable<T>;

        /// <summary>
        /// <see cref="EqualityComparerAxiomAssertion&lt;T&gt;.ctor(IArgumentFactory&lt;T&gt;, IEqualityComparer&lt;T&gt;)"/>
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type specialization for the equality comparer to validate.
        /// </typeparam>
        ///
        /// <returns>
        /// A new instance of the <see cref="EqualityComparableAxiomAssertion&lt;T&gt;"/> class,
        /// initialized with <paramref name="argumentFactory"/> and <paramref name="comparer"/>.
        /// </returns>
        EqualityComparerAxiomAssertion<T> CreateEqualityComparerAxiomAssertion<T>(IArgumentFactory<T> argumentFactory, IEqualityComparer<T> comparer);
    }
}