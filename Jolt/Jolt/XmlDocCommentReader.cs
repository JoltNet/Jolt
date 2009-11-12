// ----------------------------------------------------------------------------
// XmlDocCommentReader.cs
//
// Contains the definition of the XmlDocCommentReader class.
// Copyright 2008 Steve Guidi.
//
// File created: 12/28/2008 22:49:42
// ----------------------------------------------------------------------------

using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

using Jolt.GeneratedTypes.System.IO;
using Jolt.Properties;

namespace Jolt
{
    // Represents a factory method for creating types that implement
    // the IXmlDocCommentReadPolicy interface.  The string parameter
    // contains the full path of the XML doc comment file that is to
    // be read by the policy.
    using CreateReadPolicyDelegate=Func<string, IXmlDocCommentReadPolicy>;


    /// <summary>
    /// Provides methods to retrieve the XML Documentation Comments for an
    /// object having a metadata type from the System.Reflection namespace.
    /// </summary>
    public sealed class XmlDocCommentReader : IXmlDocCommentReader
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the reader by searching for a doc comments file
        /// that corresponds to the given assembly, from the application's
        /// configured search paths.
        /// </summary>
        /// 
        /// <param name="assembly">
        /// The assembly whose doc comments are retrieved.
        /// </param>
        public XmlDocCommentReader(Assembly assembly)
            : this(assembly, CreateDefaultReadPolicy) { }

        /// <summary>
        /// Initializes the reader by searching for a doc comments file
        /// that corresponds to the given assembly, from the application's
        /// configured search paths.  Configures the reader to use a user-defined read policy.
        /// </summary>
        /// 
        /// <param name="assembly">
        /// The assembly whose doc comments are retrieved.
        /// </param>
        /// 
        /// <param name="createReadPolicy">
        /// A factory method that accepts the full path to an XML doc comments file,
        /// returning a user-defined read policy.
        /// </param>
        public XmlDocCommentReader(Assembly assembly, CreateReadPolicyDelegate createReadPolicy)
            : this(assembly, ConfigurationManager.GetSection("XmlDocCommentsReader") as XmlDocCommentReaderSettings, createReadPolicy) { }

        /// <summary>
        /// Initializes the reader by searching for a doc comments file
        /// that corresponds to the given assembly, from the given search paths.
        /// Configures the reader to use a user-defined read policy.
        /// </summary>
        /// 
        /// <param name="assembly">
        /// The assembly whose doc comments are retrieved.
        /// </param>
        /// 
        /// <param name="settings">
        /// The configuration object containing the doc comment search paths.
        /// </param>
        /// 
        /// <param name="createReadPolicy">
        /// A factory method that accepts the full path to an XML doc comments file,
        /// returning a user-defined read policy.
        /// </param>
        public XmlDocCommentReader(Assembly assembly, XmlDocCommentReaderSettings settings, CreateReadPolicyDelegate createReadPolicy)
            : this(assembly, settings, DefaultFileProxy, createReadPolicy) { }

        /// <summary>
        /// Initializes the reader with the given path of the doc comments.
        /// </summary>
        /// 
        /// <param name="docCommentsFullPath">
        /// The full path of the XML doc comments.
        /// </param>
        public XmlDocCommentReader(string docCommentsFullPath)
            : this(docCommentsFullPath, CreateDefaultReadPolicy) { }

        /// <summary>
        /// Initializes the reader with the given path of the doc comments
        /// and a user defined read-policy.
        /// </summary>
        /// 
        /// <param name="docCommentsFullPath">
        /// The full path of the XML doc comments.
        /// </param>
        /// 
        /// <param name="createReadPolicy">
        /// A factory method that accepts the full path to an XML doc comments file,
        /// returning a user-defined read policy.
        /// </param>
        public XmlDocCommentReader(string docCommentsFullPath, CreateReadPolicyDelegate createReadPolicy)
            : this(docCommentsFullPath, DefaultFileProxy, createReadPolicy(docCommentsFullPath)) { }


        /// <summary>
        /// Initializes the reader by searching for a doc comments file
        /// that corresponds to the given assembly, from the given search paths.
        /// Configures the reader to use a user-defined read policy.
        /// </summary>
        /// 
        /// <param name="assembly">
        /// The assembly whose doc comments are retrieved.
        /// </param>
        /// 
        /// <param name="settings">
        /// The configuration object containing the doc comment search paths.
        /// </param>
        /// 
        /// <param name="fileProxy">
        /// The proxy to the file system.
        /// </param>
        /// 
        /// <param name="createReadPolicy">
        /// A factory method that accepts the full path to an XML doc comments file,
        /// returning a user-defined read policy.
        /// </param>
        internal XmlDocCommentReader(Assembly assembly, XmlDocCommentReaderSettings settings, IFile fileProxy, CreateReadPolicyDelegate createReadPolicy)
        {
            m_settings = settings ?? XmlDocCommentReaderSettings.Default;
            m_docCommentsFullPath = ResolveDocCommentsLocation(assembly, m_settings.DirectoryNames, fileProxy);
            m_fileProxy = fileProxy;
            m_docCommentsReadPolicy = createReadPolicy(m_docCommentsFullPath);
        }

        /// <summary>
        /// Initializes the reader with the given path of the doc comments
        /// and a user defined read-policy.
        /// </summary>
        /// 
        /// <param name="docCommentsFullPath">
        /// The full path of the XML doc comments.
        /// </param>
        /// 
        /// <param name="fileProxy">
        /// The proxy to the file system.
        /// </param>
        /// 
        /// <param name="createPolicy">
        /// The doc comment read policy.
        /// </param>
        internal XmlDocCommentReader(string docCommentsFullPath, IFile fileProxy, IXmlDocCommentReadPolicy readPolicy)
        {
            if (!fileProxy.Exists(docCommentsFullPath))
            {
                throw new FileNotFoundException(
                    String.Format(Resources.Error_XmlDocComments_FileNotFound, docCommentsFullPath),
                    docCommentsFullPath);
            }

            m_fileProxy = fileProxy;
            m_docCommentsFullPath = docCommentsFullPath;
            m_docCommentsReadPolicy = readPolicy;
            m_settings = XmlDocCommentReaderSettings.Default;
        }

        #endregion


        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Retrieves the xml doc comments for a given type.
        /// </summary>
        /// 
        /// <param name="type">
        /// The type for which the doc comments are retrieved.
        /// </param>
        public XElement GetComments(Type type)
        {
            return m_docCommentsReadPolicy.ReadMember(Convert.ToXmlDocCommentMember(type));
        }

        /// <summary>
        /// Retrieves the xml doc comments for a given event.
        /// </summary>
        /// 
        /// <param name="eventInfo">
        /// The event for which the doc comments are retrieved.
        /// </param>
        public XElement GetComments(EventInfo eventInfo)
        {
            return m_docCommentsReadPolicy.ReadMember(Convert.ToXmlDocCommentMember(eventInfo));
        }

        /// <summary>
        /// Retrieves the xml doc comments for a given field.
        /// </summary>
        /// 
        /// <param name="field">
        /// The field for which the doc comments are retrieved.
        /// </param>
        public XElement GetComments(FieldInfo field)
        {
            return m_docCommentsReadPolicy.ReadMember(Convert.ToXmlDocCommentMember(field));
        }

        /// <summary>
        /// Retrieves the xml doc comments for a given property.
        /// </summary>
        /// 
        /// <param name="property">
        /// The property for which the doc comments are retrieved.
        /// </param>
        public XElement GetComments(PropertyInfo property)
        {
            return m_docCommentsReadPolicy.ReadMember(Convert.ToXmlDocCommentMember(property));
        }

        /// <summary>
        /// Retrieves the xml doc comments for a given constructor.
        /// </summary>
        /// 
        /// <param name="constructor">
        /// The constructor for which the doc comments are retrieved.
        /// </param>
        public XElement GetComments(ConstructorInfo constructor)
        {
            return m_docCommentsReadPolicy.ReadMember(Convert.ToXmlDocCommentMember(constructor));
        }

        /// <summary>
        /// Retrieves the xml doc comments for a given method.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method for which the doc comments are retrieved.
        /// </param>
        public XElement GetComments(MethodInfo method)
        {
            return m_docCommentsReadPolicy.ReadMember(Convert.ToXmlDocCommentMember(method));
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the full path to the XML doc comments file that is
        /// read by the reader.
        /// </summary>
        public string FullPath
        {
            get { return m_docCommentsFullPath; }
        }

        /// <summary>
        /// Gets the configuration settings associated with this object.
        /// </summary>
        public XmlDocCommentReaderSettings Settings
        {
            get { return m_settings; }
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the file proxy used by the class.
        /// </summary>
        internal IFile FileProxy
        {
            get { return m_fileProxy; }
        }

        /// <summary>
        /// Gets the read policy used by the class.
        /// </summary>
        internal IXmlDocCommentReadPolicy ReadPolicy
        {
            get { return m_docCommentsReadPolicy; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Returns the full path to the XML doc comment file that corresponds
        /// to the given assembly by searching in the given directories.
        /// </summary>
        /// 
        /// <param name="assembly">
        /// The assembly whose doc comments are retrieved.
        /// </param>
        /// 
        /// <param name="directories">
        /// The directory names to search.
        /// </param>
        /// 
        /// <param name="fileProxy">
        /// The proxy to the file system.
        /// </param>
        private static string ResolveDocCommentsLocation(Assembly assembly, XmlDocCommentDirectoryElementCollection directories, IFile fileProxy)
        {
            string assemblyFileName = assembly.GetName().Name;
            string xmlDocCommentsFilename = String.Concat(assemblyFileName, XmlFileExtension);

            foreach (XmlDocCommentDirectoryElement directory in directories)
            {
                string xmlDocCommentsFullPath = Path.GetFullPath(Path.Combine(directory.Name, xmlDocCommentsFilename));
                if (fileProxy.Exists(xmlDocCommentsFullPath))
                {
                    return xmlDocCommentsFullPath;
                }
            }

            throw new FileNotFoundException(
                String.Format(Resources.Error_XmlDocComments_AssemblyNotResolved, assemblyFileName),
                assemblyFileName);
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IFile m_fileProxy;
        private readonly string m_docCommentsFullPath;
        private readonly IXmlDocCommentReadPolicy m_docCommentsReadPolicy;
        private readonly XmlDocCommentReaderSettings m_settings;

        private static readonly IFile DefaultFileProxy = new FileProxy();
        private static readonly CreateReadPolicyDelegate CreateDefaultReadPolicy = path => new DefaultXDCReadPolicy(path, DefaultFileProxy);
        private static readonly string XmlFileExtension = ".xml";

        #endregion
    }
}