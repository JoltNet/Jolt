// ----------------------------------------------------------------------------
// XmlDocCommentBuilderTestFixture.cs
//
// Contains the definition of the XmlDocCommentBuilderTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/1/2009 14:14:30
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Jolt.Testing.CodeGeneration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Testing.Test.CodeGeneration
{
    [TestFixture]
    public sealed class XmlDocCommentBuilderTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
        {
            With.Mocks(delegate
            {
                IXmlDocCommentReader reader = Mocker.Current.CreateMock<IXmlDocCommentReader>();

                // Expectations
                // The reader successfully returns an element for a given type.
                Type realSubjectType = typeof(string);
                Expect.Call(reader.GetComments(realSubjectType))
                    .Return(CreateExistingMember(Jolt.Convert.ToXmlDocCommentMember(realSubjectType)));

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentBuilder builder = new XmlDocCommentBuilder(reader, realSubjectType, typeof(List<>), typeof(IList<>));
                XElement docComments = XElement.Load(builder.CreateReader());

                Assert.That(builder.XmlDocCommentReader, Is.SameAs(reader));
                Assert.That(docComments.Name.LocalName, Is.EqualTo(XmlDocCommentNames.MembersElement));
                AssertMembersExist(
                    docComments,
                    String.Concat(XDCTypePrefix, GenericIListXDCTypeName),
                    String.Concat(XDCTypePrefix, GenericListXDFTypeName));
            });
        }

        /// <summary>
        /// Verifies the construction of the class when given a real
        /// subject type that does not exist in the comments referenced
        /// by the given reader.
        /// </summary>
        [Test]
        public void Construction_InvalidRealSubjectType()
        {
            With.Mocks(delegate
            {
                IXmlDocCommentReader reader = Mocker.Current.CreateMock<IXmlDocCommentReader>();

                // Expectations
                // The reader did not locate a member that corresponds to the given type.
                Type realSubjectType = typeof(string);
                Expect.Call(reader.GetComments(realSubjectType)).Return(null);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentBuilder builder = new XmlDocCommentBuilder(reader, realSubjectType, typeof(List<>), typeof(IList<>));
                XElement docComments = XElement.Load(builder.CreateReader());

                Assert.That(builder.XmlDocCommentReader, Is.SameAs(reader));
                Assert.That(docComments.Name.LocalName, Is.EqualTo(XmlDocCommentNames.MembersElement));
                Assert.That(docComments.IsEmpty);
            });
        }

        /// <summary>
        /// Verifies the behavior of the AddConstructor() method.
        /// </summary>
        [Test]
        public void AddConstructor()
        {
            VerifyBehavior_AddMember(
                typeof(string).GetConstructors().Single(HasFourParameters),
                "#ctor",
                Jolt.Convert.ToXmlDocCommentMember);
        }

        /// <summary>
        /// Verifies the behavior of the AddConstructor() method when given
        /// a constructor that does not exist in the comments referenced
        /// by the given reader.
        /// </summary>
        [Test]
        public void AddConstructor_InvalidDeclaringType()
        {
            VerifyBehavior_AddMember_InvalidDeclaringType(typeof(string).GetConstructors().Single(HasFourParameters));
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method.
        /// </summary>
        [Test]
        public void AddEvent()
        {
            VerifyBehavior_AddMember(typeof(Console).GetEvent(CancelKeyPress), Jolt.Convert.ToXmlDocCommentMember);
        }

        /// <summary>
        /// Verifies the behavior of the AddEvent() method when given
        /// an event that does not exist in the comments referenced
        /// by the given reader.
        /// </summary>
        [Test]
        public void AddEvent_InvalidDeclaringType()
        {
            VerifyBehavior_AddMember_InvalidDeclaringType(typeof(Console).GetEvent(CancelKeyPress));
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method.
        /// </summary>
        [Test]
        public void AddMethod()
        {
            VerifyBehavior_AddMember(typeof(Uri).GetMethod(GetComponents), Jolt.Convert.ToXmlDocCommentMember);
        }

        /// <summary>
        /// Verifies the behavior of the AddMethod() method when given
        /// a method that does not exist in the comments referenced
        /// by the given reader.
        /// </summary>
        [Test]
        public void AddMethod_InvalidDeclaringType()
        {
            VerifyBehavior_AddMember_InvalidDeclaringType(typeof(Uri).GetMethod(GetComponents));
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method.
        /// </summary>
        [Test]
        public void AddProperty()
        {
            VerifyBehavior_AddMember(typeof(Array).GetProperty(Length), Jolt.Convert.ToXmlDocCommentMember);
        }

        /// <summary>
        /// Verifies the behavior of the AddProperty() method when given
        /// a property that does not exist in the XML doc comments.
        /// </summary>
        [Test]
        public void AddProperty_InvalidDeclaringType()
        {
            VerifyBehavior_AddMember_InvalidDeclaringType(typeof(Array).GetProperty(Length));
        }

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Verifies the behavior of the Add*() methods
        /// </summary>
        /// 
        /// <typeparam name="TMember">
        /// The type of the member that determines which method is tested.
        /// </typeparam>
        /// 
        /// <param name="member">
        /// The member passed to the AddMember method.
        /// </param>
        /// 
        /// <param name="convertToXmlDocCommentMember">
        /// A delegate that refers to the Jolt.Convert.ToXmlDocCommentMember()
        /// method overlad that accepts a type of TMember.
        /// </param>
        private static void VerifyBehavior_AddMember<TMember>(TMember member, Func<TMember, string> convertToXmlDocCommentMember)
            where TMember : MemberInfo
        {
            VerifyBehavior_AddMember(member, member.Name, convertToXmlDocCommentMember);
        }

        /// <summary>
        /// Verifies the behavior of the Add*() methods
        /// </summary>
        /// 
        /// <typeparam name="TMember">
        /// The type of the member that determines which method is tested.
        /// </typeparam>
        /// 
        /// <param name="member">
        /// The member passed to the AddMember method.
        /// </param>
        /// 
        /// <param name="expectedXDCMemberName">
        /// The expected local name of the member, once converted to XML doc
        /// comment member name format.
        /// </param>
        /// 
        /// <param name="convertToXmlDocCommentMember">
        /// A delegate that refers to the Jolt.Convert.ToXmlDocCommentMember()
        /// method overlad that accepts a type of TMember.
        /// </param>
        private static void VerifyBehavior_AddMember<TMember>(
            TMember member,
            string expectedXDCMemberName,
            Func<TMember, string> convertToXmlDocCommentMember)
            where TMember : MemberInfo
        {
            With.Mocks(delegate
            {
                IXmlDocCommentReader reader = Mocker.Current.CreateMock<IXmlDocCommentReader>();
                Func<TMember, XElement> getComments = CreateGetCommentsDelegateFor<TMember>(reader);

                // Expectations
                // The reader successfully returns an element for the given member.
                Expect.Call(reader.GetComments(member.DeclaringType)).Return(null);

                string xdcMemberName = convertToXmlDocCommentMember(member);
                Expect.Call(getComments(member)).Return(CreateExistingMember(xdcMemberName));

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentBuilder builder = new XmlDocCommentBuilder(reader, member.DeclaringType, typeof(List<>), typeof(IList<>));
                Action<TMember> addMember = CreateAddMemberDelegateFor<TMember>(builder);

                addMember(member);

                XElement docComments = XElement.Load(builder.CreateReader());

                string memberNameWithParameters = xdcMemberName.Substring(xdcMemberName.IndexOf('.' + expectedXDCMemberName));
                string memberTypePrefix = xdcMemberName.Substring(0, 2);

                Assert.That(docComments.Name.LocalName, Is.EqualTo(XmlDocCommentNames.MembersElement));
                AssertMembersExist(
                    docComments,
                    String.Concat(memberTypePrefix, GenericIListXDCTypeName, memberNameWithParameters),
                    String.Concat(memberTypePrefix, GenericListXDFTypeName, memberNameWithParameters));
            });
        }

        /// <summary>
        /// Verifies the behavior of the Add*() methods, when given a member
        /// that does not exist in the XML doc comments.
        /// </summary>
        /// 
        /// <typeparam name="TMember">
        /// The type of the member that determines which method is tested.
        /// </typeparam>
        /// 
        /// <param name="member">
        /// The member passed to the AddMember method.
        /// </param>
        private static void VerifyBehavior_AddMember_InvalidDeclaringType<TMember>(TMember member)
            where TMember : MemberInfo
        {
            With.Mocks(delegate
            {
                IXmlDocCommentReader reader = Mocker.Current.CreateMock<IXmlDocCommentReader>();
                Func<TMember, XElement> getComments = CreateGetCommentsDelegateFor<TMember>(reader);

                // Expectations
                // The reader did not locate a member that corresponds to the given member.
                Expect.Call(reader.GetComments(member.DeclaringType)).Return(null);
                Expect.Call(getComments(member)).Return(null);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                XmlDocCommentBuilder builder = new XmlDocCommentBuilder(reader, member.DeclaringType, typeof(List<>), typeof(IList<>));
                Action<TMember> addMember = CreateAddMemberDelegateFor<TMember>(builder);

                addMember(member);

                XElement docComments = XElement.Load(builder.CreateReader());

                Assert.That(docComments.Name.LocalName, Is.EqualTo(XmlDocCommentNames.MembersElement));
                Assert.That(docComments.IsEmpty);
            });
        }

        /// <summary>
        /// Creates an element with the given name and adds an empty child element
        /// named "content".
        /// </summary>
        /// 
        /// <param name="memberName">
        /// The name of the element to create.
        /// </param>
        private static XElement CreateExistingMember(string memberName)
        {
            return new XElement(XmlDocCommentNames.MemberElement, new XAttribute(XmlDocCommentNames.NameAttribute, memberName), new XElement(ContentElementName));
        }

        /// <summary>
        /// Verifies that the given element contains exactly two child elements
        /// with the given names, and each of the child elements contain
        /// an empty child element named "content".
        /// </summary>
        /// 
        /// <param name="members">
        /// The element to verify.
        /// </param>
        /// 
        /// <param name="expectedInterfaceMemberName">
        /// The interface name that is expected to be the name of a child
        /// element of <see cref="members"/>.
        /// </param>
        /// 
        /// <param name="expectedProxyMemberName">
        /// The proxy name that is expected to be the name of a child
        /// element of <see cref="members"/>.
        /// </param>
        private static void AssertMembersExist(XElement members, string expectedInterfaceMemberName, string expectedProxyMemberName)
        {
            Assert.That(members.Elements().Count(), Is.EqualTo(2));
            AssertMemberState(members.Elements().First(), expectedInterfaceMemberName);
            AssertMemberState(members.Elements().Last(), expectedProxyMemberName);
        }

        /// <summary>
        /// Verifies that the given element is named with the given name,
        /// and contains one empty child element named "content".
        /// </summary>
        /// 
        /// <param name="member">
        /// The element to verify.
        /// </param>
        /// 
        /// <param name="expectedName">
        /// The expected name of the element.
        /// </param>
        private static void AssertMemberState(XElement member, string expectedName)
        {
            Assert.That(member.Attribute(XmlDocCommentNames.NameAttribute).Value, Is.EqualTo(expectedName));
            Assert.That(member.Elements().Count(), Is.EqualTo(1));
            Assert.That(member.Elements().First().Name.LocalName, Is.EqualTo(ContentElementName));
            Assert.That(member.Elements().First().IsEmpty);
        }

        /// <summary>
        /// Creates a delegate bound to the given reader, for the GetComments()
        /// method overload accepting a parameter of type TMember.
        /// </summary>
        /// 
        /// <typeparam name="TMember">
        /// The type of the single delegate argument.
        /// </typeparam>
        /// 
        /// <param name="reader">
        /// The instance the delegate is bound to.
        /// </param>
        private static Func<TMember, XElement> CreateGetCommentsDelegateFor<TMember>(IXmlDocCommentReader reader)
        {
            return Delegate.CreateDelegate(
                typeof(Func<TMember, XElement>),
                reader,
                reader.GetType().GetMethod("GetComments", new Type[] { typeof(TMember) })) as Func<TMember, XElement>;
        }

        /// <summary>
        /// Creates a delegate bound to the given builder, for the Add*()
        /// method accepting a parameter of type TMember.
        /// </summary>
        /// 
        /// <typeparam name="TMember">
        /// The type of the single delegate argument.
        /// </typeparam>
        /// 
        /// <param name="builder">
        /// The instance the delegate is bound to.
        /// </param>
        private static Action<TMember> CreateAddMemberDelegateFor<TMember>(XmlDocCommentBuilder builder)
        {
            return Delegate.CreateDelegate(
                typeof(Action<TMember>),
                builder,
                builder.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(
                    method => method.Name.StartsWith("Add") &&
                              method.GetParameters().Single().ParameterType == typeof(TMember))) as Action<TMember>;
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private static readonly Func<ConstructorInfo, bool> HasFourParameters = constructor => constructor.GetParameters().Length == 4;
        
        private static readonly string CancelKeyPress = "CancelKeyPress";
        private static readonly string GetComponents = "GetComponents";
        private static readonly string Length = "Length";

        private static readonly string ContentElementName = "content";

        private static readonly string GenericIListXDCTypeName = "System.Collections.Generic.IList`1";
        private static readonly string GenericListXDFTypeName = "System.Collections.Generic.List`1";
        private static readonly string XDCTypePrefix = "T:";

        #endregion
    }
}