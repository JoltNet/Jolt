// ----------------------------------------------------------------------------
// TypeDescriptor.cs
//
// Contains the definition of the TypeDescriptor class.
// Copyright 2009 Steve Guidi.
//
// File created: 4/13/2009 14:44:00
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Jolt.Testing.CodeGeneration.Xml
{
    /// <summary>
    /// Provides information about a type entry in the configuration file
    /// that is parsed by the <see cref="XmlConfigurator"/> class.
    /// </summary>
    public sealed class TypeDescriptor
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the the TypeDescriptor.
        /// </summary>
        /// 
        /// <param name="realSubjectType">
        /// The descriptor's real subject type.
        /// </param>
        /// 
        /// <param name="returnTypeOverrides">
        /// The return type overrides for the real subject type.
        /// </param>
        public TypeDescriptor(Type realSubjectType, IDictionary<Type, Type> returnTypeOverrides)
        {
            m_realSubjectType = realSubjectType;
            m_returnTypeOverrides = returnTypeOverrides;
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the descriptor's real subject type.
        /// </summary>
        public Type RealSubjectType
        {
            get { return m_realSubjectType; }
        }

        /// <summary>
        /// Gets the real subject type's return type overrides.
        /// </summary>
        public IDictionary<Type, Type> ReturnTypeOverrides
        {
            get { return m_returnTypeOverrides; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly Type m_realSubjectType;
        private readonly IDictionary<Type, Type> m_returnTypeOverrides;

        #endregion
    }
}