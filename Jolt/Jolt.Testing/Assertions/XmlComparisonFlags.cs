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
        Strict = 0x00,
        IgnoreElementNamespaces = 0x01,
        IgnoreAttributeNamespaces = 0x02,
        IgnoreAttributes = 0x04,
        IgnoreElementValues = 0x08,
        IgnoreSequenceOrder = 0x10
    }
}