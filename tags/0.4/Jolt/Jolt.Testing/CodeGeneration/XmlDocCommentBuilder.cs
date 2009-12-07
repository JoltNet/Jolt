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
    /// Defines a concrete implementation of the <see cref="XmlDocCommentBuilderBase"/> class,
    /// incrementally constructing an XML doc comments file pertaining to the members of a proxy
    /// and interface type.  Transforms a given set of XML doc comments for a real subject type,
    /// to XML doc comments suitable for a corresponding proxy and interface type.
    /// </summary>
    internal sealed class XmlDocCommentBuilder : XmlDocCommentBuilderBase
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="XmlDocCommentBuilder"/> class
        /// </summary>
        /// 
        /// <param name="reader">
        /// A <see cref="IXmlDocCommentReader"/> that retrieves the reference set of XML doc
        /// comments, for a real subject type.
        /// </param>
        /// 
        /// <param name="realSubectType">
        /// The <see cref="System.Type"/> whose assembly is referenced by <paramref name="reader"/>, and from which
        /// the proxy and interface types are modelled after.
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
        /// Adds the proxy and interface documentation for a given
        /// <see cref="System.Reflection.ConstructorInfo"/> to the builder.
        /// </summary>
        /// 
        /// <param name="constructor">
        /// The <see cref="System.Reflection.ConstructorInfo"/> whose documentation is transformed.
        /// </param>
        internal override void AddConstuctor(ConstructorInfo constructor)
        {
            TransformAndAddComments(m_reader.GetComments(constructor), constructor.DeclaringType);
        }

        /// <summary>
        /// Adds the proxy and interface documentation for a given
        /// <see cref="System.Reflection.EventInfo"/> to the builder.
        /// </summary>
        /// 
        /// <param name="eventInfo">
        /// The <see cref="System.Reflection.EventInfo"/> whose documentation is transformed.
        /// </param>
        internal override void AddEvent(EventInfo eventInfo)
        {
            TransformAndAddComments(m_reader.GetComments(eventInfo), eventInfo.DeclaringType);
        }

        /// <summary>
        /// Adds the proxy and interface documentation for a given
        /// <see cref="System.Reflection.MethodInfo"/> to the builder.
        /// </summary>
        /// 
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> whose documentation is transformed.
        /// </param>
        internal override void AddMethod(MethodInfo method)
        {
            TransformAndAddComments(m_reader.GetComments(method), method.DeclaringType);
        }

        /// <summary>
        /// Adds the proxy and interface documentation for a given
        /// <see cref="System.Reflection.PropertyInfo"/> to the builder.
        /// </summary>
        /// 
        /// <param name="property">
        /// The <see cref="System.Reflection.PropertyInfo"/> whose documentation is transformed.
        /// </param>
        internal override void AddProperty(PropertyInfo property)
        {
            TransformAndAddComments(m_reader.GetComments(property), property.DeclaringType);
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="IXmlDocCommentReader"/> associated with the instance.
        /// </summary>
        internal IXmlDocCommentReader XmlDocCommentReader
        {
            get { return m_reader; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Tranforms the given member <see cref="System.Xml.XLinq.XElement"/> documentation node
        /// into two <see cref="System.Xml.XLinq.XElement"/> nodes that respectively represent the
        /// member documentation nodes for the proxy and proxy interface types.
        /// </summary>
        /// 
        /// <param name="memberElement">
        /// The member <see cref="System.Xml.XLinq.XElement"/> documentation to transform.
        /// </param>
        /// 
        /// <param name="realSubjectType">
        /// The <see cref="System.Type"/> whose member is referenced by <paramref name="memberElement"/>.
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
        /// Replaces the XML doc comment encoded type name of a given documentation node
        /// with the encoded type name of another given <see cref="System.Type"/>.
        /// </summary>
        /// 
        /// <param name="memberElement">
        /// The <see cref="System.Xml.XLinq.XElement"/> whose name attribute is modified.
        /// </param>
        /// 
        /// <param name="typeToReplace">
        /// The <see cref="System.Type"/> whose name is referenced by <paramref name="memberElement"/>.
        /// </param>
        /// 
        /// <param name="newType">
        /// The new <see cref="System.Type"/> whose name is encoded and replaces the name attribute
        /// of <paramref name="memberElement"/>
        /// </param>
        /// 
        /// <returns>
        /// A reference to <paramref name="memberElement"/>, after it has been modified.
        /// </returns>
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