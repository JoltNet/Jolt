// ----------------------------------------------------------------------------
// AbstractXDCReadPolicy.cs
//
// Contains the definition of the AbstractXDCReadPolicy class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/25/2009 22:16:52
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using Jolt.GeneratedTypes.System.IO;

namespace Jolt
{
    /// <summary>
    /// Defines the common functionality for all implementations of the
    /// <see cref="IXmlDocCommentReadPolicy"/> interface.
    /// </summary>
    public abstract class AbstractXDCReadPolicy
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initialize the instance with the given file path.
        /// </summary>
        /// 
        /// <param name="xmlDocCommentsFullPath">
        /// The full path to the XML doc comments that will be managed
        /// by this instance.
        /// </param>
        protected AbstractXDCReadPolicy(string xmlDocCommentsFullPath)
            : this(xmlDocCommentsFullPath, new FileProxy()) { }

        /// <summary>
        /// Initialize the instance with the given file path and file proxy.
        /// </summary>
        /// 
        /// <param name="xmlDocCommentsFullPath">
        /// The full path to the XML doc comments that will be managed
        /// by this instance.
        /// </param>
        /// 
        /// <param name="fileProxy">
        /// The proxy to the file system.
        /// </param>
        internal AbstractXDCReadPolicy(string xmlDocCommentsFullPath, IFile fileProxy)
        {
            m_xmlDocCommentsFullPath = xmlDocCommentsFullPath;
            m_fileProxy = fileProxy;
        }

        /// <summary>
        /// Initializes the static state of the class.
        /// </summary>
        static AbstractXDCReadPolicy()
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

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the full path of the XML doc comments.
        /// </summary>
        internal string XmlDocCommentsFullPath
        {
            get { return m_xmlDocCommentsFullPath; }
        }

        /// <param name="fileProxy">
        /// The proxy to the file system.
        /// </param>
        internal IFile FileProxy
        {
            get { return m_fileProxy; }
        }

        #endregion

        #region protected methods -----------------------------------------------------------------

        /// <summary>
        /// Creates a validating XML reader that can read the XML doc
        /// comments associated with the instance.
        /// </summary>
        protected XmlReader CreateReader()
        {
            return XmlReader.Create(m_fileProxy.OpenText(m_xmlDocCommentsFullPath), ReaderSettings);
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly string m_xmlDocCommentsFullPath;
        private readonly IFile m_fileProxy;

        private static readonly XmlReaderSettings ReaderSettings;
        
        #endregion
    }
}