// ----------------------------------------------------------------------------
// XmlDocCommentBuilder.cs
//
// Contains the definition of the XmlDocCommentBuilder class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/1/2009 12:32:45
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Provides a class for incrementally constructing an XML doc
    /// comments file pertaining to the members of a proxy and interface type.
    /// </summary>
    internal sealed class XmlDocCommentBuilder : XmlDocCommentBuilderBase
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the builder with a XML doc comment file that refers to
        /// the assembly of the given proxy types, and contains documentation for
        /// the given proxy types.
        /// </summary>
        /// 
        /// <param name="reader">
        /// The reader that retrieves XML doc comments for transformation.
        /// </param>
        /// 
        /// <param name="realSubectType">
        /// The type whose assembly is referenced by the reader, and from which
        /// the proxy types are derived from.
        /// </param>
        /// 
        /// <param name="proxyType">
        /// The proxy type to document.
        /// </param>
        /// 
        /// <param name="proxyInterfaceType">
        /// The proxy interface type to document.
        /// </param>
        internal XmlDocCommentBuilder(IXmlDocCommentReader reader, Type realSubectType, Type proxyType, Type proxyInterfaceType)
            : base()
        {
            m_reader = reader;
            m_proxyType = proxyType;
            m_proxyInterfaceType = proxyInterfaceType;
            m_members = new XElement(XmlDocCommentNames.MembersElement);

            base.XmlDocComments.Add(m_members);
            TransformAndAddComments(m_reader.GetComments(realSubectType), realSubectType);
        }

        #endregion

        #region XmlDocCommentBuilderBase members --------------------------------------------------

        /// <summary>
        /// Adds the proxy and interface documentation for the given constructor to the builder.
        /// </summary>
        /// 
        /// <param name="constructor">
        /// The constructor whose documentation is transformed.
        /// </param>
        internal override void AddConstuctor(ConstructorInfo constructor)
        {
            TransformAndAddComments(m_reader.GetComments(constructor), constructor.DeclaringType);
        }

        /// <summary>
        /// Adds the proxy and interface documentation for the given event to the builder.
        /// </summary>
        /// 
        /// <param name="eventInfo">
        /// The event whose documentation is transformed.
        /// </param>
        internal override void AddEvent(EventInfo eventInfo)
        {
            TransformAndAddComments(m_reader.GetComments(eventInfo), eventInfo.DeclaringType);
        }

        /// <summary>
        /// Adds the proxy and interface documentation for the given method to the builder.
        /// </summary>
        /// 
        /// <param name="method">
        /// The method whose documentation is transformed.
        /// </param>
        internal override void AddMethod(MethodInfo method)
        {
            TransformAndAddComments(m_reader.GetComments(method), method.DeclaringType);
        }

        /// <summary>
        /// Adds the proxy and interface documentation for the given property to the builder.
        /// </summary>
        /// 
        /// <param name="property">
        /// The property whose documentation is transformed.
        /// </param>
        internal override void AddProperty(PropertyInfo property)
        {
            TransformAndAddComments(m_reader.GetComments(property), property.DeclaringType);
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the XML doc comment reader associated with the class.
        /// </summary>
        internal IXmlDocCommentReader XmlDocCommentReader
        {
            get { return m_reader; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Tranforms the given member element into an two elements that respectively
        /// represent the given member documentation for the proxy and proxy interface types.
        /// </summary>
        /// 
        /// <param name="memberElement">
        /// The member element (and documentation) to transform.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The type whose member is referenced in the member element.
        /// </param>
        private void TransformAndAddComments(XElement memberElement, Type realSubjectType)
        {
            if (memberElement != null)
            {
                m_members.Add(ReplaceTypeName(new XElement(memberElement), realSubjectType, m_proxyInterfaceType));
                m_members.Add(ReplaceTypeName(memberElement, realSubjectType, m_proxyType));
            }
        }


        /// <summary>
        /// Replaces the XML doc comment encoded type name of a given type with the
        /// encoded type name of another given type within the given element.
        /// </summary>
        /// 
        /// <param name="memberElement">
        /// The element whose name attribute is modified.
        /// </param>
        /// 
        /// <param name="typeToReplace">
        /// The type whose encoded name is replaced.
        /// </param>
        /// 
        /// <param name="newType">
        /// The new type whose name is encoded and introduce into the name attribute.
        /// </param>
        private static XElement ReplaceTypeName(XElement memberElement, Type typeToReplace, Type newType)
        {
            XAttribute memberName = memberElement.Attribute(XmlDocCommentNames.NameAttribute);
            StringBuilder memberNameBuilder = new StringBuilder(memberName.Value);

            // 1. Remove the substring containing the name of the real subect type
            //    from the name builder, excluding the two-character doc comment member
            //    type prefix (e.g. "T:", "M:", etc...).
            // 2. Insert the XML doc comment representation of the new type name into
            //    the name builder, after the existing two-character doc comment member
            //    type prefix.
            // 3. Remove the duplicate type prefix, added by step #2.
            int lengthOfNameToReplace = Jolt.Convert.ToXmlDocCommentMember(typeToReplace).Length - MemberPrefixLength;
            memberNameBuilder.Remove(TypeNameStartPos, lengthOfNameToReplace);
            memberNameBuilder.Insert(TypeNameStartPos, Jolt.Convert.ToXmlDocCommentMember(newType));
            memberNameBuilder.Remove(TypeNameStartPos, MemberPrefixLength);

            memberName.Value = memberNameBuilder.ToString();
            return memberElement;
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private readonly IXmlDocCommentReader m_reader;
        private readonly Type m_proxyType;
        private readonly Type m_proxyInterfaceType;
        private readonly XElement m_members;

        private static readonly int MemberPrefixLength = 2;
        private static readonly int TypeNameStartPos = 2;

        #endregion
    }
}