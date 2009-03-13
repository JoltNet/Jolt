// ----------------------------------------------------------------------------
// IXmlDocCommentReadPolicy.cs
//
// Contains the definition of the IXmlDocCommentReadPolicy interface.
// Copyright 2009 Steve Guidi.
//
// File created: 2/17/2009 8:27:14 AM
// ----------------------------------------------------------------------------

using System.Xml.Linq;

namespace Jolt
{
    /// <summary>
    /// Defines an interface for retrieving XML data from any
    /// XML doc comment data store.
    /// </summary>
    public interface IXmlDocCommentReadPolicy
    {
        /// <summary>
        /// Creates an XML element containing the member element
        /// with the given name from an XML doc comment data store.
        /// </summary>
        /// 
        /// <param name="memberName">
        /// The name of the member element to retrieve.
        /// </param>
        XElement ReadMember(string memberName);
    }
}