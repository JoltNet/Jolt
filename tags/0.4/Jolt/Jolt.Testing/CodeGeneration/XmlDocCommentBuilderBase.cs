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
    /// Defines a base class for incrementally constructing an XML doc
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
        /// Creates a new instance of the <see cref="XmlDocCommentBuilderBase"/> class, with an 
        /// empty XML doc comments document.
        /// </summary>
        internal XmlDocCommentBuilderBase()
        {
            m_docComments = new XDocument();
        }

        #endregion

        #region internal methods ------------------------------------------------------------------

        /// <summary>
        /// Default implementation -- no operation.
        /// </summary>
        internal virtual void AddConstuctor(ConstructorInfo constructor) { }

        /// <summary>
        /// Default implementation -- no operation.
        /// </summary>
        internal virtual void AddEvent(EventInfo eventInfo) { }

        /// <summary>
        /// Default implementation -- no operation.
        /// </summary>
        internal virtual void AddMethod(MethodInfo method) { }

        /// <summary>
        /// Default implementation -- no operation.
        /// </summary>
        internal virtual void AddProperty(PropertyInfo property) { }

        /// <summary>
        /// Creates a new instance of an <see cref="System.Xml.XmlReader"/> for accessing the current
        /// state of the XML doc comment document.
        /// </summary>
        internal virtual XmlReader CreateReader() { return m_docComments.CreateReader(); }

        #endregion

        #region protected properties --------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="System.Xml.XLinq.XDocument"/> document in which the
        /// buider is incrementally populating.
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