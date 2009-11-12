// ----------------------------------------------------------------------------
// XmlDocCommentBuilderBase.cs
//
// Contains the definition of the XmlDocCommentBuilderBase class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/1/2009 10:50:02
// ----------------------------------------------------------------------------

using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Provides a base class for incrementally constructing an XML doc
    /// comments file pertaining to the members of a proxy and interface type.
    /// </summary>
    /// 
    /// <remarks>
    /// The methods of the base class perform no operations and thus instances
    /// are suitable for a default/no-op configuration.
    /// </remarks>
    internal class XmlDocCommentBuilderBase
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the builder with an empty XML doc comment store.
        /// </summary>
        internal XmlDocCommentBuilderBase()
        {
            m_docComments = new XDocument();
        }

        #endregion

        #region internal methods ------------------------------------------------------------------

        internal virtual void AddConstuctor(ConstructorInfo constructor) { }
        internal virtual void AddEvent(EventInfo eventInfo) { }
        internal virtual void AddMethod(MethodInfo method) { }
        internal virtual void AddProperty(PropertyInfo property) { }

        /// <summary>
        /// Creates a reader for accessing the current XML doc comment state.
        /// </summary>
        internal virtual XmlReader CreateReader() { return m_docComments.CreateReader(); }

        #endregion

        #region protected properties --------------------------------------------------------------

        /// <summary>
        /// Gets the document to which the buider appends XML doc comments.
        /// </summary>
        protected XDocument XmlDocComments
        {
            get { return m_docComments; }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly XDocument m_docComments;

        #endregion
    }
}