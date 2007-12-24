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

namespace Jolt.Testing.CodeGeneration
{
    internal interface IProxyTypeBuilder
    {
        /// <summary>
        /// Adds a method to the proxy builder.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method to add to the builder.
        /// </param>
        void AddMethod(MethodInfo method);

        /// <summary>
        /// Adds a property to the proxy builder.
        /// </summary>
        /// 
        /// <param name="property">
        /// The property to add to the builder.
        /// </param>
        void AddProperty(PropertyInfo property);

        /// <summary>
        /// Adds an event to the proxy builder.
        /// </summary>
        /// 
        /// <param name="eventInfo">
        /// The event to add to the builder.
        /// </param>
        void AddEvent(EventInfo eventInfo);

        /// <summary>
        /// Creates the proxy interface type for the current state of the builder.
        /// </summary>
        Type CreateInterface();
        
        /// <summary>
        /// Creates the proxy interface type.
        /// </summary>
        Type CreateProxy();
    }
}
