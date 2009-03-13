// ----------------------------------------------------------------------------
// IProxyTypeBuilder.cs
//
// Contains the definition of the IProxyTypeBuilder interface.
// Copyright 2007 Steve Guidi.
//
// File created: 10/21/2007 14:05:39
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Xml;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Internal interface to support testing of objects that
    /// have/use a ProxyTypeBuilder.
    /// </summary>
    internal interface IProxyTypeBuilder
    {
        void AddMethod(MethodInfo method);
        void AddProperty(PropertyInfo property);
        void AddEvent(EventInfo eventInfo);
        Type CreateInterface();
        Type CreateProxy();
        XmlReader CreateXmlDocCommentReader();
    }
}
