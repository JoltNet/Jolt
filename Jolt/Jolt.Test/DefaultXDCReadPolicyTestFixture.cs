// ----------------------------------------------------------------------------
// DefaultXDCReadPolicyTestFixture.cs
//
// Contains the definition of the DefaultXDCReadPolicyTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/17/2009 9:02:14 PM
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

using Jolt.GeneratedTypes.System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Test
{
    [TestFixture]
    public sealed class DefaultXDCReadPolicyTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void Construction()
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
                
                DefaultXDCReadPolicy policy = new DefaultXDCReadPolicy(expectedFileName, fileProxy);
                Assert.That(expectedReader.BaseStream.Position, Is.EqualTo(expectedReader.BaseStream.Length));
            });
        }

        /// <summary>
        /// Verifies the construction of the class when the given
        /// XML doc comment file is invalid.
        /// </summary>
        [Test, ExpectedException(typeof(XmlSchemaValidationException))]
        public void Construction_InvalidXml()
        {
            With.Mocks(delegate
            {
                IFile fileProxy = Mocker.Current.CreateMock<IFile>();

                // Expectations.
                // The doc comments file is accessed via a stream reader.
                string expectedFileName = Path.GetRandomFileName();
                StreamReader expectedReader = new StreamReader(new MemoryStream(Encoding.Default.GetBytes("<invalidXml/>")));

                Expect.Call(fileProxy.OpenText(expectedFileName)).Return(expectedReader);

                // Verification and assertions.
                Mocker.Current.ReplayAll();
                IXmlDocCommentReadPolicy policy = new DefaultXDCReadPolicy(expectedFileName, fileProxy);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ReadAssembly() method.
        /// </summary>
        [Test]
        public void ReadAssembly()
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

                IXmlDocCommentReadPolicy policy = new DefaultXDCReadPolicy(expectedFileName, fileProxy);
                XElement element = policy.ReadAssembly();

                Assert.That(element.Document, Is.Null); 
                Assert.That(element.Name.LocalName, Is.EqualTo("assembly"));
                Assert.That(element.Elements().Count(), Is.EqualTo(1));
                Assert.That(element.Element("name"), Is.Not.Null);
                Assert.That(element.Element("name").Value, Is.EqualTo("assembly-name"));
                Assert.That(element.Element("name").Elements().Count(), Is.EqualTo(0));
            });
        }

        /// <summary>
        /// Verifies the behavior of the ReadMember() method.
        /// </summary>
        [Test]
        public void ReadMember()
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

                IXmlDocCommentReadPolicy policy = new DefaultXDCReadPolicy(expectedFileName, fileProxy);
                string memberName = "another-member-name";
                XElement element = policy.ReadMember(memberName);

                Assert.That(element.Document, Is.Null);
                Assert.That(element.Name.LocalName, Is.EqualTo("member"));
                Assert.That(element.Attribute("name").Value, Is.EqualTo(memberName));
                Assert.That(element.Elements().Count(), Is.EqualTo(1));
                Assert.That(element.Element("otherContent"), Is.Not.Null);
                Assert.That(element.Element("otherContent").IsEmpty);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ReadMember() method when the
        /// requested member does not exist in the XML doc comments file.
        /// </summary>
        [Test]
        public void ReadMember_MemberDoesNotExist()
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

                IXmlDocCommentReadPolicy policy = new DefaultXDCReadPolicy(expectedFileName, fileProxy);

                Assert.That(policy.ReadMember("invalidMemberName"), Is.Null);
            });
        }

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Retrieves a stream reader that reads the test fixutres sample
        /// doc comments XML.
        /// </summary>
        private static StreamReader OpenDocCommentsXml()
        {
            Type thisType = typeof(DefaultXDCReadPolicyTestFixture);
            return new StreamReader(thisType.Assembly.GetManifestResourceStream(thisType, "Xml.DocComments.xml"));
        }

        #endregion
    }
}