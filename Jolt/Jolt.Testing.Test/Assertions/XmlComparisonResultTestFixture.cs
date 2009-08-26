// ----------------------------------------------------------------------------
// XmlComparisonResultTestFixture.cs
//
// Contains the definition of the XmlComparisonResultTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 8/18/2009 18:08:57
// ----------------------------------------------------------------------------

using System.Linq;
using System.Xml.Linq;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Jolt.Testing.Assertions;
using System.Runtime.Serialization;
using System;
using System.Reflection;
using System.Globalization;

namespace Jolt.Testing.Test.Assertions
{
    [TestFixture]
    public sealed class XmlComparisonResultTestFixture
    {
        /// <summary>
        /// Verifies the construction of the class.
        /// </summary>
        [Test]
        public void DefaultConstruction()
        {
            XmlComparisonResult result = new XmlComparisonResult();

            Assert.That(result.Result);
            Assert.That(result.Message, Is.Empty);
            Assert.That(result.ExpectedElement, Is.Null);
            Assert.That(result.ActualElement, Is.Null);
            Assert.That(result.XPathHint, Is.Empty);
        }

        /// <summary>
        /// Verifies the explicit construction of the class.
        /// </summary>
        [Test]
        public void ExplicitConstruction()
        {
            string expectedMessage = "message";
            XElement expectedElement = new XElement("expected");
            XElement actualElement = new XElement("actual");
            XmlComparisonResult result = new XmlComparisonResult(true, expectedMessage, expectedElement, actualElement);

            Assert.That(result.Result);
            Assert.That(result.Message, Is.SameAs(expectedMessage));
            Assert.That(result.ExpectedElement, Is.SameAs(expectedElement));
            Assert.That(result.ActualElement, Is.SameAs(actualElement));
            Assert.That(result.XPathHint, Is.EqualTo("/actual"));
        }

        /// <summary>
        /// Verifies the creation of the XPathHint propery value.
        /// </summary>
        [Test]
        public void XPathHint()
        {
            XElement descendantElement = new XElement(XName.Get("descendant", "ns-2"));
            XDocument dom = new XDocument(
                new XElement(XName.Get("root", "ns-1"),
                    new XElement("child",
                        descendantElement)));

            XmlComparisonResult result = new XmlComparisonResult(false, String.Empty, null, descendantElement);

            Assert.That(result.XPathHint, Is.EqualTo("/ns-1:root/child/ns-2:descendant"));
        }

        //SerializationInfo info = new SerializationInfo(
        //typeof(XmlComparisonAssertionException),
        //new FormatterConverter());

        //Exception expectedInnerException = new Exception();
        //string expectedMessage = "exception-message";
        //info.AddValue("ClassName", String.Empty);
        //info.AddValue("Message", expectedMessage);
        //info.AddValue("InnerException", expectedInnerException);
        //info.AddValue("HelpURL", String.Empty);
        //info.AddValue("StackTraceString", String.Empty);
        //info.AddValue("RemoteStackTraceString", String.Empty);
        //info.AddValue("RemoteStackIndex", 0);
        //info.AddValue("ExceptionMethod", String.Empty);
        //info.AddValue("HResult", 1);
        //info.AddValue("Source", String.Empty);

        //XmlComparisonAssertionException exception = Activator.CreateInstance(
        //typeof(XmlComparisonAssertionException),
        //BindingFlags.NonPublic | BindingFlags.Instance,
        //null,
        //new object[] { info, new StreamingContext() },
        //null) as XmlComparisonAssertionException;

        //Assert.That(exception.Message, Is.SameAs(expectedMessage));
        //Assert.That(exception.InnerException, Is.SameAs(expectedInnerException));

    }
}