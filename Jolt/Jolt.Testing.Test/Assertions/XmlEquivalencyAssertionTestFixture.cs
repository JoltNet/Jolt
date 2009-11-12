// ----------------------------------------------------------------------------
// XmlEquivalencyAssertionTestFixture.cs
//
// Contains the definition of the XmlEquivalencyAssertionTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 6/19/2009 18:50:22
// ----------------------------------------------------------------------------

using System;
using System.Xml;
using System.Xml.Linq;

using Jolt.Testing.Assertions;
using Jolt.Testing.Properties;
using NUnit.Framework;

namespace Jolt.Testing.Test.Assertions
{
    [TestFixture]
    public sealed class XmlEquivalencyAssertionTestFixture
    {
        #region constructors ----------------------------------------------------------------------
        
        /// <summary>
        /// Initializes the static fields of the test fixture.
        /// </summary>
        static XmlEquivalencyAssertionTestFixture()
        {
            DefaultElementName = XName.Get("ElementName", "urn:document-namespace");
            DefaultAttributes = new XAttribute[]
            {
                new XAttribute("attribute_3", 300),
                new XAttribute("attribute_1", 100),
                new XAttribute("attribute_2", 200)
            };
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Verifies the construction of the class; only the storage of a
        /// given XML comparison flags enumeration.
        /// </summary>
        [Test]
        public void Construction()
        {
            AssertionConstructionTests.XmlEquivalencyAssertion(flags => new XmlEquivalencyAssertion(flags));
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain a different number of child elements,
        /// and the element order of a sequence is ignored.
        /// </summary>
        [Test]
        public void AreEquivalent_IgnoreSequenceOrder_ExcessElement()
        {
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreSequenceOrder)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-AdditionalElement.xml"));

            Assert.That(!comparisonResult.Result);
            
            ValidateComparisonResultElement(
                comparisonResult.ExpectedElement,
                XName.Get("anotherElement", DefaultElementName.NamespaceName),
                XName.Get("root", DefaultElementName.NamespaceName));
            
            ValidateComparisonResultElement(
                comparisonResult.ActualElement,
                XName.Get("element", DefaultElementName.NamespaceName),
                XName.Get("root", DefaultElementName.NamespaceName));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_UnexpectedElement, "anotherElement", "element")));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}root/{0}element", "urn:document-namespace:")));
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain a different number of descendant elements,
        /// and the element order of a sequence is ignored.
        /// </summary>
        [Test]
        public void AreEquivalent_IgnoreSequenceOrder_ExcessElementInDescendant()
        {
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreSequenceOrder)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-AdditionalElementInDescendant.xml"));

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(
                comparisonResult.ExpectedElement,
                XName.Get("yetAnotherElement", DefaultElementName.NamespaceName),
                XName.Get("grandChildElement", DefaultElementName.NamespaceName));

            ValidateComparisonResultElement(
                comparisonResult.ActualElement,
                XName.Get("anotherElementAgain", DefaultElementName.NamespaceName),
                XName.Get("grandChildElement", DefaultElementName.NamespaceName));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_UnexpectedElement, "yetAnotherElement", "anotherElementAgain")));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}root/{0}anotherElement/{0}childElement/{0}grandChildElement/{0}anotherElementAgain", "urn:document-namespace:")));
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain a different number of child elements,
        /// and the element order of a sequence is ignored.
        /// </summary>
        [Test]
        public void AreEquivalent_IgnoreSequenceOrder_MismatchingNumberOfChildren()
        {
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreSequenceOrder)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-MismatchingNumberOfChildren.xml"));

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, XName.Get("root", DefaultElementName.NamespaceName), null);
            ValidateComparisonResultElement(comparisonResult.ActualElement, XName.Get("root", DefaultElementName.NamespaceName), null);

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_ChildElementQuantityMismatch, "root")));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}root", "urn:document-namespace:")));
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain a different number of descendant elements,
        /// and the element order of a sequence is ignored.
        /// </summary>
        [Test]
        public void AreEquivalent_IgnoreSequenceOrder_MismatchingNumberOfChildrenInDescendant()
        {
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreSequenceOrder)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-MismatchingNumberOfChildrenInDescendant.xml"));

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(
                comparisonResult.ExpectedElement,
                XName.Get("anotherGrandChildElement", DefaultElementName.NamespaceName),
                XName.Get("childElement", DefaultElementName.NamespaceName));
            ValidateComparisonResultElement(
                comparisonResult.ActualElement,
                XName.Get("anotherGrandChildElement", DefaultElementName.NamespaceName),
                XName.Get("childElement", DefaultElementName.NamespaceName));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_ChildElementQuantityMismatch, "anotherGrandChildElement")));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}root/{0}anotherElement/{0}childElement/{0}anotherGrandChildElement", "urn:document-namespace:")));
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain a different number of descendant elements,
        /// and the element order of a sequence is ignored.
        /// </summary>
        [Test]
        public void AreEquivalent_IgnoreSequenceOrder()
        {
            Assert.That(new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreSequenceOrder)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-ElementSequenceTransformed.xml"))
                .Result);
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain similar elements in different namespaces,
        /// and the element namespaces are ignored.
        /// </summary>
        [Test]
        public void AreEquivalent_IgnoreElementNamespace()
        {
            Assert.That(new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreElementNamespaces)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-ElementNamespaceTransformed.xml"))
                .Result);
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain similar attributes in different namespaces,
        /// and the attribute namespaces are ignored.
        /// </summary>
        [Test]
        public void AreEquivalent_IgnoreAttributeNamespace()
        {
            Assert.That(new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreAttributeNamespaces)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-AttributeNamespaceTransformed.xml"))
                .Result);
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain the same set of attributes, but
        /// in different order.
        /// </summary>
        [Test]
        public void AreEquivalent_AttributeOrdering()
        {
            Assert.That(new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-AttributeOrderTransformed.xml"))
                .Result);
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain different attributes, and the
        /// attributes are ignored.
        /// </summary>
        [Test]
        public void AreEquivalent_IgnoreAttributes()
        {
            Assert.That(new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreAttributes)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-AttributeTransformed.xml"))
                .Result);
        }

        /// <summary>
        /// Verifies the behavior of the AreEquivalent() method, when given
        /// two XML elements that contain element differing in values,
        /// and element values are ingored.
        /// </summary>
        [Test]
        public void AreEquivalent_IgnoreElementValues()
        {
            Assert.That(new XmlEquivalencyAssertion(XmlComparisonFlags.IgnoreElementValues)
                .AreEquivalent(GetEmbeddedXml("AssertionInput.xml"), GetEmbeddedXml("AssertionInput-ElementValueTransformed.xml"))
                .Result);
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that are equal to one another.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict()
        {
            XElement expectedElement = new XElement(DefaultElementName,
                DefaultAttributes,
                new XElement(DefaultElementName),
                new XElement(DefaultElementName,
                    100, 200,
                    DefaultAttributes,
                    new XElement(DefaultElementName),
                    300));

            Assert.That(new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), expectedElement.CreateReader())
                .Result);
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in local name.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_LocalNameMismatch()
        {
            XElement expectedElement = new XElement(DefaultElementName);
            XElement actualElement = new XElement(XName.Get("DifferentElementName", DefaultElementName.NamespaceName));
            
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict).AreEquivalent(
                    expectedElement.CreateReader(),
                    actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, null);
            ValidateComparisonResultElement(
                comparisonResult.ActualElement,
                XName.Get("DifferentElementName", DefaultElementName.NamespaceName),
                null);

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_UnexpectedElement, DefaultElementName.LocalName, "DifferentElementName")));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}", DefaultElementName.NamespaceName, "DifferentElementName")));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in namespace.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_NamespaceMismatch()
        {
            XName unexpectedElement = XName.Get(DefaultElementName.LocalName, "urn:different-namespace");
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict).AreEquivalent(
                    new XElement(DefaultElementName).CreateReader(),
                    new XElement(unexpectedElement).CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, null);
            ValidateComparisonResultElement(comparisonResult.ActualElement, unexpectedElement, null);

            Assert.That(comparisonResult.Message, Is.EqualTo(String.Format(
                Resources.AssertionFailure_ElementNamespaceMismatch,
                DefaultElementName.LocalName,
                DefaultElementName.NamespaceName, 
                unexpectedElement.NamespaceName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}", unexpectedElement.NamespaceName, unexpectedElement.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in value.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ValueMismatch()
        {
            XElement expectedElement = new XElement(DefaultElementName, 100, 200, new XElement(DefaultElementName), 300);
            XElement actualElement = new XElement(DefaultElementName, 100, new XElement(DefaultElementName), 200, 400);

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, null);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, null);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_ElementValueMismatch, DefaultElementName.LocalName, "100200300", "100200400")));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in number of attributes.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_FewerAttributes()
        {
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict).AreEquivalent(
                    new XElement(DefaultElementName, new XAttribute("attribute_1", 100)).CreateReader(),
                    new XElement(DefaultElementName).CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, null);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, null);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_AttributeQuantityMismatch, DefaultElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in number of attributes.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_FewerAttributesInDescendant()
        {
            XElement expectedElement = new XElement(DefaultElementName, new XElement(DefaultElementName, new XAttribute("attribute_1", 100)));
            XElement actualElement = new XElement(DefaultElementName, new XElement(DefaultElementName));

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, DefaultElementName);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_AttributeQuantityMismatch, DefaultElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in number of attributes.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ExcessAttributes()
        {
            XElement expectedElement = new XElement(DefaultElementName, new XAttribute("attribute_1", 100));
            XElement actualElement = new XElement(DefaultElementName, new XAttribute("attribute_1", 100), new XAttribute("attribute_2", 200));

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, null);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, null);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_AttributeQuantityMismatch, DefaultElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in number of attributes.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ExcessAttributesInDescendant()
        {
            XElement expectedElement = new XElement(DefaultElementName, new XElement(DefaultElementName, new XAttribute("attribute_1", 100)));
            XElement actualElement = new XElement(DefaultElementName, new XElement(DefaultElementName, new XAttribute("attribute_1", 100), new XAttribute("attribute_2", 200)));

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, DefaultElementName);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_AttributeQuantityMismatch, DefaultElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in their attributes.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_AttributeNameMismatch()
        {
            XAttribute[] actualAttributes =
            {
                new XAttribute("attribute_6", 300),
                DefaultAttributes[1],
                DefaultAttributes[2]
            };

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict).AreEquivalent(
                    new XElement(DefaultElementName, DefaultAttributes).CreateReader(),
                    new XElement(DefaultElementName, actualAttributes).CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, null);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, null);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_UnexpectedAttribute, actualAttributes[0].Name.LocalName, DefaultElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in their attribute values.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_AttributeValueMismatch()
        {
            XAttribute[] actualAttributes =
            {
                new XAttribute(DefaultAttributes[0].Name, 600),
                DefaultAttributes[1],
                DefaultAttributes[2]
            };

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict).AreEquivalent(
                    new XElement(DefaultElementName, DefaultAttributes).CreateReader(),
                    new XElement(DefaultElementName, actualAttributes).CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, null);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, null);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(String.Format(
                Resources.AssertionFailure_AttributeValueMismatch,
                DefaultAttributes[0].Name,
                DefaultElementName.LocalName,
                DefaultAttributes[0].Value,
                actualAttributes[0].Value)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in number of a child elements.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ExcessElement()
        {
            XName excessElementName = XName.Get("excessElement", DefaultElementName.NamespaceName);
            XmlComparisonResult comparisonResult = 
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict).AreEquivalent(
                    new XElement(DefaultElementName, new XElement(DefaultElementName)).CreateReader(),
                    new XElement(DefaultElementName, new XElement(excessElementName), new XElement(DefaultElementName)).CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, excessElementName, DefaultElementName);

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_UnexpectedElement, DefaultElementName.LocalName, excessElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{0}:{2}", DefaultElementName.NamespaceName, DefaultElementName.LocalName, excessElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in number of a decendant elements.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ExcessElementInDescendant()
        {
            XName excessElementName = XName.Get("excessElement", DefaultElementName.NamespaceName);
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict).AreEquivalent(
                    new XElement(DefaultElementName,
                        new XElement(DefaultElementName,
                            new XElement(DefaultElementName),
                            new XElement(DefaultElementName))).CreateReader(),
                    new XElement(DefaultElementName,
                        new XElement(DefaultElementName,
                            new XElement(DefaultElementName),
                            new XElement(excessElementName),
                            new XElement(DefaultElementName))).CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, excessElementName, DefaultElementName);

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_UnexpectedElement, DefaultElementName.LocalName, excessElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{0}:{1}/{0}:{2}", DefaultElementName.NamespaceName, DefaultElementName.LocalName, excessElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in number of a child elements.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_MismatchingNumberOfChildren()
        {
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict).AreEquivalent(
                    new XElement(DefaultElementName).CreateReader(),
                    new XElement(DefaultElementName, new XElement(DefaultElementName)).CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, null);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, null);

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_ChildElementQuantityMismatch, DefaultElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in number of a descendant elements.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_MismatchingNumberOfChildrenInDescendant()
        {
            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict).AreEquivalent(
                    new XElement(DefaultElementName, new XElement(DefaultElementName)).CreateReader(),
                    new XElement(DefaultElementName, new XElement(DefaultElementName, new XElement(DefaultElementName))).CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, DefaultElementName);

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_ChildElementQuantityMismatch, DefaultElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }
        
        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in a child element local name.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ChildLocalNameMismatch()
        {
            XName differentElementName = XName.Get("DifferentElementName", DefaultElementName.NamespaceName);

            XElement expectedElement = new XElement(DefaultElementName, new XElement(DefaultElementName), new XElement(DefaultElementName));
            XElement actualElement = new XElement(DefaultElementName, new XElement(DefaultElementName), new XElement(differentElementName));

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, differentElementName, DefaultElementName);

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_UnexpectedElement, DefaultElementName.LocalName, differentElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{0}:{2}", DefaultElementName.NamespaceName, DefaultElementName.LocalName, differentElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in child element namespace.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ChildNamespaceMismatch()
        {
            XName differentElementName = XName.Get(DefaultElementName.LocalName, "urn:different-namespace");

            XElement expectedElement = new XElement(DefaultElementName, new XElement(DefaultElementName), new XElement(DefaultElementName));
            XElement actualElement = new XElement(DefaultElementName, new XElement(DefaultElementName), new XElement(differentElementName));

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, differentElementName, DefaultElementName);

            Assert.That(comparisonResult.Message, Is.EqualTo(String.Format(
                Resources.AssertionFailure_ElementNamespaceMismatch,
                DefaultElementName.LocalName,
                DefaultElementName.NamespaceName,
                differentElementName.NamespaceName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{2}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName, differentElementName.NamespaceName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in child element value.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ChildValueMismatch()
        {
            XElement expectedElement = new XElement(DefaultElementName,
                new XElement(DefaultElementName),
                new XElement(DefaultElementName,
                    100, 200,
                    new XElement(DefaultElementName),
                    300));
            XElement actualElement = new XElement(DefaultElementName,
                new XElement(DefaultElementName),
                new XElement(DefaultElementName,
                    100,
                    new XElement(DefaultElementName),
                    200, 400));

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, DefaultElementName);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_ElementValueMismatch, DefaultElementName.LocalName, "100200300", "100200400")));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in a child element's attributes.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ChildAttributeNameMismatch()
        {
            XAttribute[] actualAttributes =
            {
                new XAttribute("attribute_6", 300),
                DefaultAttributes[1],
                DefaultAttributes[2]
            };

            XElement expectedElement = new XElement(DefaultElementName, new XElement(DefaultElementName), new XElement(DefaultElementName, DefaultAttributes));
            XElement actualElement = new XElement(DefaultElementName, new XElement(DefaultElementName), new XElement(DefaultElementName, actualAttributes));

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, DefaultElementName);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(
                String.Format(Resources.AssertionFailure_UnexpectedAttribute, actualAttributes[0].Name.LocalName, DefaultElementName.LocalName)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        /// <summary>
        /// Verifies the strict behavior mode of the AreEquivalent() method, when given
        /// two XML elements that differ in a child element's attribute values.
        /// </summary>
        [Test]
        public void AreEquivalent_Strict_ChildAttributeValueMismatch()
        {
            XAttribute[] actualAttributes =
            {
                new XAttribute(DefaultAttributes[0].Name, 600),
                DefaultAttributes[1],
                DefaultAttributes[2]
            };

            XElement expectedElement = new XElement(DefaultElementName, new XElement(DefaultElementName), new XElement(DefaultElementName, DefaultAttributes));
            XElement actualElement = new XElement(DefaultElementName, new XElement(DefaultElementName), new XElement(DefaultElementName, actualAttributes));

            XmlComparisonResult comparisonResult =
                new XmlEquivalencyAssertion(XmlComparisonFlags.Strict)
                .AreEquivalent(expectedElement.CreateReader(), actualElement.CreateReader());

            Assert.That(!comparisonResult.Result);

            ValidateComparisonResultElement(comparisonResult.ExpectedElement, DefaultElementName, DefaultElementName);
            ValidateComparisonResultElement(comparisonResult.ActualElement, DefaultElementName, DefaultElementName);
            Assert.That(comparisonResult.ExpectedElement, Is.Not.SameAs(comparisonResult.ActualElement));

            Assert.That(comparisonResult.Message, Is.EqualTo(String.Format(
                    Resources.AssertionFailure_AttributeValueMismatch,
                    actualAttributes[0].Name.LocalName,
                    DefaultElementName.LocalName,
                    DefaultAttributes[0].Value,
                    actualAttributes[0].Value)));
            Assert.That(comparisonResult.XPathHint, Is.EqualTo(
                String.Format("/{0}:{1}/{0}:{1}", DefaultElementName.NamespaceName, DefaultElementName.LocalName)));
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Loads an embedded resource from the XML folder, with the given name.
        /// </summary>
        /// 
        /// <param name="resourceName">
        /// The name of the resource to load.
        /// </param>
        private static XmlReader GetEmbeddedXml(string resourceName)
        {
            Type thisType = typeof(XmlEquivalencyAssertionTestFixture);
            return XmlReader.Create(thisType.Assembly.GetManifestResourceStream(thisType, "Xml." + resourceName));
        }

        /// <summary>
        /// Validates the contents of a given XElement, stored in an
        /// <seealso cref="XmlComparisonResult"/>.
        /// </summary>
        /// 
        /// <param name="element">
        /// The element to validate.
        /// </param>
        /// 
        /// <param name="expectedElementName">
        /// The expected element name.
        /// </param>
        /// 
        /// <param name="expectedParentElementName">
        /// The name of the expected element's parent.
        /// </param>
        private static void ValidateComparisonResultElement(XElement element, XName expectedElementName, XName expectedParentElementName)
        {
            Assert.That(element.Name, Is.EqualTo(expectedElementName));
            if (expectedParentElementName == null)
            {
                Assert.That(element.Parent, Is.Null);
            }
            else
            {
                Assert.That(element.Parent.Name, Is.EqualTo(expectedParentElementName));
            }
        }

        #endregion

        #region private fields --------------------------------------------------------------------

        private static readonly XName DefaultElementName;
        private static readonly XAttribute[] DefaultAttributes;

        #endregion
    }
}