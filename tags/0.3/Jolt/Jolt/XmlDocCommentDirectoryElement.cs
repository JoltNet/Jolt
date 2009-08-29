// ----------------------------------------------------------------------------
// XmlDocCommentDirectoryElement.cs
//
// Contains the definition of the XmlDocCommentDirectoryElement class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/1/2009 09:30:27
// ----------------------------------------------------------------------------

using System.Configuration;

namespace Jolt
{
    /// <summary>
    /// Provides configuration settings to control the search paths
    /// for locating XML doc comments.  Represents an element
    /// containing one path.
    /// </summary>
    public sealed class XmlDocCommentDirectoryElement : ConfigurationElement
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the object with the default configuration
        /// settings.  Intended to be called from by the XML serializer.
        /// </summary>
        internal XmlDocCommentDirectoryElement() { }

        /// <summary>
        /// Initializes the directory name for the configuration element.
        /// </summary>
        /// 
        /// <param name="directoryName">
        /// The directory name containing a user desired search path.
        /// </param>
        public XmlDocCommentDirectoryElement(string directoryName)
        {
            this["name"] = directoryName;
        }

        #endregion

        #region public properties -----------------------------------------------------------------

        /// <summary>
        /// Gets the search directory name stored by the configuration element.
        /// </summary>
        [ConfigurationProperty("name", IsRequired=true)]
        public string Name
        {
            get { return this["name"] as string; }
        }

        #endregion
    }
}