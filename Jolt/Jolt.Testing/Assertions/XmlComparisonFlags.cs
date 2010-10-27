// ----------------------------------------------------------------------------
// XmlComparisonFlags.cs
//
// Contains the definition of the XmlComparisonFlags enumeration.
// Copyright 2009 Steve Guidi.
//
// File created: 6/1/2009 17:53:54
// ----------------------------------------------------------------------------

using System;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Controls the level of equivalency strictness when applied to the
    /// <see cref="XmlEquivalencyAssertion"/> class.
    /// </summary>
    [Flags]
    public enum XmlComparisonFlags
    {
        /// <summary>
        /// Performs the strictest-possible XML comparison during evaluation of the asserion.
        /// </summary>
        Strict = 0x00,

        /// <summary>
        /// Ignores the namespace of elements during evaluation of the assertion.
        /// </summary>
        IgnoreElementNamespaces = 0x01,

        /// <summary>
        /// Ignores the namespace of attributes during evaluation of the assertion.
        /// </summary>
        IgnoreAttributeNamespaces = 0x02,

        /// <summary>
        /// Ignores attributes and their values during evaluation of the assertion.
        /// </summary>
        IgnoreAttributes = 0x04,

        /// <summary>
        /// Ignores the values of elements during evaluation of the assertion.
        /// </summary>
        IgnoreElementValues = 0x08,

        /// <summary>
        /// Ignores the ordering of sibling elements during evaluation of the assertion.
        /// </summary>
        IgnoreSequenceOrder = 0x10
    }
}