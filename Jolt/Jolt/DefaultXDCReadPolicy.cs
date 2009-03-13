// ----------------------------------------------------------------------------
// DefaultXDCReadPolicy.cs
//
// Contains the definition of the DefaultXDCReadPolicy class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/17/2009 8:33:33 AM
// ----------------------------------------------------------------------------

using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Jolt.GeneratedTypes.System.IO;

namespace Jolt
{
    /// <summary>
    /// Implements the <see cref="IXmlDocCommentReadPolicy"/> interface
    /// providing a policy that retrieves the requested elements from
    /// a memory-stored doc comment file.
    /// </summary>
    internal sealed class DefaultXDCReadPolicy : AbstractXDCReadPolicy, IXmlDocCommentReadPolicy
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the new instance, loading the contents of
        /// the given doc comments file using the given file system
        /// proxy.
        /// </summary>
        /// 
        /// <param name="xmlDocCommentsFullPath">
        /// The full path of the XML doc comments to be read by the class.
        /// </param>
        /// 
        /// <param name="fileProxy">
        /// The proxy to the file system.
        /// </param>
        internal DefaultXDCReadPolicy(string xmlDocCommentsFullPath, IFile fileProxy)
            : base(xmlDocCommentsFullPath, fileProxy)
        {
            using (XmlReader reader = base.CreateReader())
            {
                m_docComments = XDocument.Load(reader);
            }
        }

        #endregion

        #region IXmlDocCommentReadPolicy implementation -------------------------------------------

        XElement IXmlDocCommentReadPolicy.ReadMember(string memberName)
        {
            XElement member = m_docComments
                .Element(XmlDocCommentNames.DocElement)
                .Element(XmlDocCommentNames.MembersElement)
                .Elements(XmlDocCommentNames.MemberElement)
                .SingleOrDefault(e => e.Attribute(XmlDocCommentNames.NameAttribute).Value == memberName);

            // Copy the <member> element from the DOM.
            return member == null ? null : XElement.Load(member.CreateReader());
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly XDocument m_docComments;

        #endregion
    }
}