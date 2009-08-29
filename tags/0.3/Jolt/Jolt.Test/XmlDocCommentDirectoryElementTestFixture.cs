// ----------------------------------------------------------------------------
// XmlDocCommentDirectoryElementTestFixture.cs
//
// Contains the definition of the XmlDocCommentDirectoryElementTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 2/2/2009 6:30:45 PM
// ----------------------------------------------------------------------------

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Test
{
    // TODO: Static property tests; interrogate ElementInformation.
    [TestFixture]
    public sealed class XmlDocCommentDirectoryElementTestFixture
    {
        /// <summary>
        /// Verifies the default construction of the class.
        /// </summary>
        [Test]
        public void DefaultConstruction()
        {
            XmlDocCommentDirectoryElement element = new XmlDocCommentDirectoryElement();
            Assert.That(element.Name, Is.Empty);
        }

        /// <summary>
        /// Verifies the explicit construction of the class.
        /// </summary>
        [Test]
        public void ExplicitConstruction()
        {
            string expectedName = @"C:\test-directory";
            XmlDocCommentDirectoryElement element = new XmlDocCommentDirectoryElement(expectedName);

            Assert.That(element.Name, Is.SameAs(expectedName));
        }
    }
}