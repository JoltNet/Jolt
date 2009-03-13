// ----------------------------------------------------------------------------
// AbstractXDCReadPolicyTestFixture.cs
//
// Contains the definition of the AbstractXDCReadPolicyTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/25/2009 22:52:16
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Jolt.GeneratedTypes.System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Test
{
    public abstract class AbstractXDCReadPolicyTestFixture
    {
        /// <summary>
        /// Verifies the construction of the policy class when given
        /// only a path.
        /// </summary>
        /// 
        /// <param name="createPolicy">
        /// A factory method that creates the policy object to test.
        /// </param>
        /// 
        /// <param name="assert">
        /// A delegate that executes an additional set of assertions.
        /// </param>
        /// 
        /// <typeparam name="TPolicy">
        /// The type of policy being tested.
        /// </typeparam>
        protected void Construction<TPolicy>(Func<string, TPolicy> createPolicy, Action<TPolicy> assert)
            where TPolicy : AbstractXDCReadPolicy
        {
            string expectedFullPath = Path.GetRandomFileName();
            TPolicy policy = createPolicy(expectedFullPath);

            Assert.That(policy.XmlDocCommentsFullPath, Is.SameAs(expectedFullPath));
            Assert.That(policy.FileProxy, Is.InstanceOfType(typeof(FileProxy)));
            assert(policy);
        }

        /// <summary>
        /// Verifies the internal construction of the policy class.
        /// </summary>
        /// 
        /// <param name="createPolicy">
        /// A factory method that creates the policy object to test.
        /// </param>
        /// 
        /// <param name="assert">
        /// A delegate that executes an additional set of assertions.
        /// </param>
        ///
        /// <param name="expect">
        /// A delegate that executes an additional set of expectations.
        /// </param>
        /// 
        /// <typeparam name="TPolicy">
        /// The type of policy being tested.
        /// </typeparam>
        protected void Constructrion_Internal<TPolicy>(Func<string, IFile, TPolicy> createPolicy, Action<string, IFile> expect, Action<TPolicy> assert)
            where TPolicy : AbstractXDCReadPolicy
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();
                string expectedFullPath = Path.GetRandomFileName();
                expect(expectedFullPath, fileProxy);

                Mocker.Current.ReplayAll();
                TPolicy policy = createPolicy(expectedFullPath, fileProxy);

                Assert.That(policy.XmlDocCommentsFullPath, Is.SameAs(expectedFullPath));
                Assert.That(policy.FileProxy, Is.SameAs(fileProxy));
                assert(policy);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ReadMember() method.
        /// </summary>
        /// 
        /// <param name="createPolicy">
        /// A factory method that creates the policy object to test.
        /// </param>
        /// 
        /// <param name="assert">
        /// A delegate that executes an additional set of assertions.
        /// </param>
        /// 
        /// <typeparam name="TPolicy">
        /// The type of policy being tested.
        /// </typeparam>
        protected void ReadMember<TPolicy>(Func<string, IFile, TPolicy> createPolicy, Action<TPolicy> assert)
            where TPolicy : AbstractXDCReadPolicy, IXmlDocCommentReadPolicy
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();

                // Expectations.
                // The doc comments file is accessed via a stream reader.
                string expectedFileName = Path.GetRandomFileName();
                StreamReader expectedReader = OpenDocCommentsXml();

                Expect.Call(fileProxy.OpenText(expectedFileName)).Return(expectedReader);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                TPolicy policy = createPolicy(expectedFileName, fileProxy);
                string memberName = "another-member-name";
                XElement element = policy.ReadMember(memberName);

                Assert.That(element.Document, Is.Null);
                Assert.That(element.Name.LocalName, Is.EqualTo(XmlDocCommentNames.MemberElement));
                Assert.That(element.Attribute(XmlDocCommentNames.NameAttribute).Value, Is.EqualTo(memberName));
                Assert.That(element.Elements().Count(), Is.EqualTo(1));
                Assert.That(element.Element("otherContent"), Is.Not.Null);
                Assert.That(element.Element("otherContent").IsEmpty);
                assert(policy);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ReadMember() method when the
        /// requested member does not exist in the XML doc comments file.
        /// </summary>
        /// 
        /// <param name="createPolicy">
        /// A factory method that creates the policy object to test.
        /// </param>
        /// 
        /// <param name="assert">
        /// A delegate that executes an additional set of assertions.
        /// </param>
        /// 
        /// <typeparam name="TPolicy">
        /// The type of policy being tested.
        /// </typeparam>
        protected void ReadMember_DoesNotExist<TPolicy>(Func<string, IFile, TPolicy> createPolicy, Action<TPolicy> assert)
            where TPolicy : AbstractXDCReadPolicy, IXmlDocCommentReadPolicy
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();

                // Expectations.
                // The doc comments file is accessed via a stream reader.
                string expectedFileName = Path.GetRandomFileName();
                StreamReader expectedReader = OpenDocCommentsXml();

                Expect.Call(fileProxy.OpenText(expectedFileName)).Return(expectedReader);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                TPolicy policy = createPolicy(expectedFileName, fileProxy);

                Assert.That(policy.ReadMember("invalidMemberName"), Is.Null);
                assert(policy);
            });
        }

        #region protected class methods -----------------------------------------------------------

        /// <summary>
        /// Retrieves a stream reader that reads the test fixutres sample
        /// doc comments XML.
        /// </summary>
        protected static StreamReader OpenDocCommentsXml()
        {
            Type thisType = typeof(DefaultXDCReadPolicyTestFixture);
            return new StreamReader(thisType.Assembly.GetManifestResourceStream(thisType, "Xml.DocComments.xml"));
        }

        #endregion
    }
}