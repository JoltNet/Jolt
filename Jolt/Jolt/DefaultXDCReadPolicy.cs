// ----------------------------------------------------------------------------
// DefaultXDCReadPolicy.cs
//
// Contains the definition of the DefaultXDCReadPolicy class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/17/2009 8:33:33 AM
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using Jolt.GeneratedTypes.System.IO;

namespace Jolt
{
    /// <summary>
    /// Implements the <see cref="IXmlDocCommentReadPolicy"/> interface
    /// providing a policy that retrieves the requested elements from
    /// a memory-stored doc comment file.
    /// </summary>
    internal sealed class DefaultXDCReadPolicy : IXmlDocCommentReadPolicy
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the new instance, loading the contents of
        /// the given doc comments file using the given file system
        /// proxy.
        /// </summary>
        /// 
        /// <param name="docCommentsFullPath">
        /// The full path of the XML doc comments to be read by the class.
        /// </param>
        /// 
        /// <param name="fileProxy">
        /// The proxy to the file system.
        /// </param>
        internal DefaultXDCReadPolicy(string docCommentsFullPath, IFile fileProxy)
        {
            using (XmlReader reader = XmlReader.Create(fileProxy.OpenText(docCommentsFullPath), ReaderSettings))
            {
                m_docComments = XDocument.Load(reader);
            }
        }

        /// <summary>
        /// Initializes the static state of the class.
        /// </summary>
        static DefaultXDCReadPolicy()
        {
            ReaderSettings = new XmlReaderSettings();
            ReaderSettings.ValidationType = ValidationType.Schema;

            Type thisType = typeof(DefaultXDCReadPolicy);
            using (Stream schema = thisType.Assembly.GetManifestResourceStream(thisType, "Xml.DocComments.xsd"))
            {
                ReaderSettings.Schemas.Add(XmlSchema.Read(schema, null));
            }
        }

        #endregion

        // TODO: string constants.
        #region IXmlDocCommentReadPolicy implementation -------------------------------------------

        XElement IXmlDocCommentReadPolicy.ReadAssembly()
        {
            // Copy the <assembly> element from the DOM.
            return XElement.Load(m_docComments
                .Element("doc")
                .Element("assembly")
                .CreateReader());
        }

        XElement IXmlDocCommentReadPolicy.ReadMember(string memberName)
        {
            XElement member = m_docComments
                .Element("doc")
                .Element("members")
                .Elements("member")
                .SingleOrDefault(e => e.Attribute("name").Value == memberName);

            // Copy the <member> element from the DOM.
            return member == null ? null : XElement.Load(member.CreateReader());
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly XDocument m_docComments;
        private static readonly XmlReaderSettings ReaderSettings;

        #endregion
    }
}