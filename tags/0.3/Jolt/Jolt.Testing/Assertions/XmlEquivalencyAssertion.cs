// ----------------------------------------------------------------------------
// XmlEquivalencyAssertion.cs
//
// Contains the definition of the XmlEquivalencyAssertion class.
// Copyright 2009 Steve Guidi.
//
// File created: 6/1/2009 17:59:39
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Jolt.Functional;
using Jolt.Testing.Properties;

namespace Jolt.Testing.Assertions
{
    /// <summary>
    /// Compares two XML structures for user-defined equivalency, for
    /// both XML structure and element/attribute values.
    /// </summary>
    public class XmlEquivalencyAssertion
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Initializes the static state of the class.
        /// </summary>
        static XmlEquivalencyAssertion()
        {
            ReaderSettings = new XmlReaderSettings();
            ReaderSettings.IgnoreComments = true;
            ReaderSettings.IgnoreProcessingInstructions = true;
            ReaderSettings.IgnoreWhitespace = true;

            SuccessfulComparisonResult = new XmlComparisonResult();
        }

        /// <summary>
        /// Initializes a new instance of the XmlEquivalencyAssertion class.
        /// </summary>
        /// 
        /// <param name="strictness">
        /// A set of flags denoting the definition of equivalency,
        /// for this instance.
        /// </param>
        public XmlEquivalencyAssertion(XmlComparisonFlags strictness)
        {
            m_comparisonFlags = strictness;

            Func<XElement, XElement, XmlComparisonResult> comparisonAlwaysSuccessful =
                Functor.Idempotency<XElement, XElement, XmlComparisonResult>(SuccessfulComparisonResult);

            m_validateAttributeEquivalency = ContainsFlag(XmlComparisonFlags.IgnoreAttributes, strictness) ? comparisonAlwaysSuccessful : CompareAttributes;
            m_validateElementValueEquivalency = ContainsFlag(XmlComparisonFlags.IgnoreElementValues, strictness) ? comparisonAlwaysSuccessful : CompareElementValues;
            m_validateElementNamespaceEquivalency = ContainsFlag(XmlComparisonFlags.IgnoreElementNamespaces, strictness) ? comparisonAlwaysSuccessful : CompareElementNamespaces;
            m_areEquivalent = ContainsFlag(XmlComparisonFlags.IgnoreSequenceOrder, strictness) ?
                new Func<XElement, XElement, XmlComparisonResult>(AreEquivalentAndUnordered) :
                new Func<XElement, XElement, XmlComparisonResult>(AreEquivalentAndOrdered);

            if (ContainsFlag(XmlComparisonFlags.IgnoreAttributeNamespaces, strictness))
            {
                m_attributeKeySelector = a => a.Name.LocalName;
                m_validateAttributeNamespaceEquivalency =
                    Functor.Idempotency<XAttribute, XAttribute, XElement, XElement, XmlComparisonResult>(SuccessfulComparisonResult);
            }
            else
            {
                m_attributeKeySelector = a => a.Name.ToString();
                m_validateAttributeNamespaceEquivalency = CompareAttributeNamespaces;
            }
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Compares two given XmlReaders for element/document equivalency
        /// in terms of structure and contents, according to the configuration
        /// of the class.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected element/document.
        /// </param>
        /// 
        /// <param name="actual">
        /// The element/document being validated for equivalency.
        /// </param>
        public virtual XmlComparisonResult AreEquivalent(XmlReader expected, XmlReader actual)
        {
            using (XmlReader expectedXml = XmlReader.Create(expected, ReaderSettings),
                             actualXml = XmlReader.Create(actual, ReaderSettings))
            {
                return m_areEquivalent(XElement.Load(expectedXml), XElement.Load(actualXml));
            }
        }

        #endregion

        #region internal properties ---------------------------------------------------------------

        /// <summary>
        /// Gets the comparison flags used to initialize the instance.
        /// </summary>
        internal XmlComparisonFlags ComparisonFlags
        {
            get { return m_comparisonFlags; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Verifies the equivalency of the two given elements according to the 
        /// configuration of the class, assuming that the child elements of two given
        /// elements are ordered.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The element containing the expected set of child elements.
        /// </param>
        /// 
        /// <param name="actual">
        /// The element containing the child element set to validate.
        /// </param>
        private XmlComparisonResult AreEquivalentAndOrdered(XElement expected, XElement actual)
        {
            XmlComparisonResult areEquivalent = AreElementsEquivalent(expected, actual);
            if (!areEquivalent.Result) { return areEquivalent; }

            using (IEnumerator<XElement> expectedChildElements = expected.Elements().GetEnumerator(),
                                         actualChildElements = actual.Elements().GetEnumerator())
            {
                bool moreExpectedChildElements, moreActualChildElements;

                while (true)
                {
                    moreExpectedChildElements = expectedChildElements.MoveNext();
                    moreActualChildElements = actualChildElements.MoveNext();

                    if (!moreExpectedChildElements || !moreActualChildElements)
                    {
                        if (moreExpectedChildElements ^ moreActualChildElements)
                        {
                            return new XmlComparisonResult(
                                false,
                                String.Format(Resources.AssertionFailure_ChildElementQuantityMismatch, actual.Name.LocalName),
                                expected,
                                actual);
                        }

                        break;
                    }

                    // Ordering of child elements must match.
                    areEquivalent = AreEquivalentAndOrdered(expectedChildElements.Current, actualChildElements.Current);
                    if (!areEquivalent.Result) { return areEquivalent; }
                }
            }

            return SuccessfulComparisonResult;
        }

        /// <summary>
        /// Verifies the equivalency of the two given elements according to the 
        /// configuration of the class, assuming that the child elements of two given
        /// elements are unordered.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The element containing the expected set of child elements.
        /// </param>
        /// 
        /// <param name="actual">
        /// The element containing the child element set to validate.
        /// </param>
        private XmlComparisonResult AreEquivalentAndUnordered(XElement expected, XElement actual)
        {
            NormalizeElementOrder_FirstPass(expected, actual);
            NormalizeElementOrder_SecondPass(expected, actual);
            return AreEquivalentAndOrdered(expected, actual);  // TODO: consider comparison message upon detection of error.
        }

        /// <summary>
        /// Orders the child elementselements contained in <paramref name="actualChildren"/>
        /// such that they match the element order of those in <paramref name="expectedChildren"/>.
        /// An element is moved if it is considered equivalent to the reference element as per
        /// the configuration of the class (<see cref="AreElementsEquivalent"/>).
        /// </summary>
        /// 
        /// <param name="expectedChildren">
        /// The collection of reference elements, defining the desired element order.
        /// </param>
        /// 
        /// <param name="actualChildren">
        /// The collection containing the elements to reorder.
        /// </param>
        private void NormalizeElementOrder_FirstPass(XElement expected, XElement actual)
        {
            if (!AreEquivalent(expected, actual)) { return; }

            List<XElement> unorderedActualChildren = actual.Elements().ToList();
            if (expected.Elements().Count() != unorderedActualChildren.Count) { return; }

            int nextUnorderedChildIndex = 0;
            foreach (XElement expectedChild in expected.Elements())
            {
                int numChildElements = expectedChild.Elements().Count();

                Predicate<XElement> isEquivalentToExpectedChild =
                    Functor.ToPredicate(Bind.First<XElement, XElement, bool>(AreEquivalent, expectedChild));
                Predicate<XElement> isEquivalentToExpectedChild_Strict =
                    candidateElement => isEquivalentToExpectedChild(candidateElement) &&
                                        numChildElements == candidateElement.Elements().Count();

                // Look for the element that best resembles expectedChild considering the configuration
                // the assertion, along with the number of child elements for expectedChild.
                int equivalentChildIndex = unorderedActualChildren.FindIndex(nextUnorderedChildIndex, isEquivalentToExpectedChild_Strict);

                if (equivalentChildIndex < 0)
                {
                    // We couldn't find a matching element.  Relax the search criteria
                    // by removing the matching child element count constraint and try again.
                    equivalentChildIndex = unorderedActualChildren.FindIndex(nextUnorderedChildIndex, isEquivalentToExpectedChild);
                    if (equivalentChildIndex < 0) { return; }   // No match found, stop normalizing element order.
                }

                // Exchange the unordered elements and then apply the normalization to
                // the children of expected/actual.
                unorderedActualChildren[nextUnorderedChildIndex].ReplaceWith(unorderedActualChildren[equivalentChildIndex]);
                unorderedActualChildren[equivalentChildIndex].ReplaceWith(unorderedActualChildren[nextUnorderedChildIndex]);
                SwapElement(unorderedActualChildren, nextUnorderedChildIndex, equivalentChildIndex);
                ++nextUnorderedChildIndex;

                NormalizeElementOrder_FirstPass(expectedChild, unorderedActualChildren[equivalentChildIndex]);
            }
        }

        /// <summary>
        /// Orders the child elementselements contained in <paramref name="actualChildren"/>
        /// such that they match the element order of those in <paramref name="expectedChildren"/>.
        /// An element is moved if it is considered equivalent to the reference element as per
        /// the configuration of the class (<see cref="AreElementsEquivalent"/>).
        /// </summary>
        /// 
        /// <param name="expectedChildren">
        /// The collection of reference elements, defining the desired element order.
        /// </param>
        /// 
        /// <param name="actualChildren">
        /// The collection containing the elements to reorder.
        /// </param>
        private bool NormalizeElementOrder_SecondPass(XElement expected, XElement actual)
        {
            if (!AreEquivalent(expected, actual)) { return false; }

            List<XElement> unorderedActualChildren = actual.Elements().ToList();
            if (expected.Elements().Count() != unorderedActualChildren.Count) { return false; }

            // Reorder the actual child element, but considers
            // the structure of the entire sub tree.  Guarantees
            // the most acurate reordering.
            int nextUnorderedChildIndex = 0;
            foreach (XElement expectedChild in expected.Elements())
            {
                // Look for the element that best resembles expectedChild considering the configuration
                // the assertion, along with the number of child elements for expectedChild.
                int equivalentChildIndex = unorderedActualChildren.FindIndex(
                    nextUnorderedChildIndex,
                    Functor.ToPredicate(Bind.First<XElement, XElement, bool>(NormalizeElementOrder_SecondPass, expectedChild)));

                // If we can not find an equivalent child, the entire child list is inequivalent.
                if (equivalentChildIndex < 0) { return false; }

                // Exchange the unordered elements and then apply the normalization to
                // the children of expected/actual.
                unorderedActualChildren[nextUnorderedChildIndex].ReplaceWith(unorderedActualChildren[equivalentChildIndex]);
                unorderedActualChildren[equivalentChildIndex].ReplaceWith(unorderedActualChildren[nextUnorderedChildIndex]);
                SwapElement(unorderedActualChildren, nextUnorderedChildIndex, equivalentChildIndex);
                ++nextUnorderedChildIndex;
            }

            return true;
        }


        /// Exchanges the elements in the given collection, at the given indecies.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The the of element contained in the given collection.
        /// </typeparam>
        /// 
        /// <param name="collection">
        /// The collection containing the elements to exchange.
        /// </param>
        /// 
        /// <param name="firstIndex">
        /// The index of the first element to exchange.
        /// </param>
        /// 
        /// <param name="secondIndex">
        /// The index of the second element to exchange.
        /// </param>
        private void SwapElement<T>(IList<T> collection, int firstIndex, int secondIndex)
        {
            // TODO: Share this code.
            T element = collection[firstIndex];
            collection[firstIndex] = collection[secondIndex];
            collection[secondIndex] = element;
        }

        /// <summary>
        /// Compares two given elements for equivalency in terms of contents,
        /// excluding child element analysis, according to the configuration of the class.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected element.
        /// </param>
        /// 
        /// <param name="actual">
        /// The element being validated for equivalency.
        /// </param>
        private XmlComparisonResult AreElementsEquivalent(XElement expected, XElement actual)
        {
            XmlComparisonResult areEquivalent = m_validateElementNamespaceEquivalency(expected, actual);
            if (!areEquivalent.Result) { return areEquivalent; }

            if (expected.Name.LocalName != actual.Name.LocalName)
            {
                return new XmlComparisonResult(
                    false,
                    String.Format(Resources.AssertionFailure_UnexpectedElement, expected.Name.LocalName, actual.Name.LocalName),
                    expected,
                    actual);
            }

            areEquivalent = m_validateElementValueEquivalency(expected, actual);
            if (!areEquivalent.Result) { return areEquivalent; }

            return m_validateAttributeEquivalency(expected, actual);
        }

        /// <summary>
        /// Invokes the <see cref="AreElementsEquivalent"/> method, returning
        /// a Boolean value denoting the success of the method execution.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected element argument for the method.
        /// </param>
        /// 
        /// <param name="actual">
        /// The acutal element argument for the method.
        /// </param>
        private bool AreEquivalent(XElement expected, XElement actual)
        {
            return AreElementsEquivalent(expected, actual).Result;
        }

        /// <summary>
        /// Determines the equivalency of the attributes of two given elements,
        /// by comparing attribute names and their values.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The element containing the expected set of attributes.
        /// </param>
        /// 
        /// <param name="actual">
        /// The element containing the attribute set to validate.
        /// </param>
        private XmlComparisonResult CompareAttributes(XElement expected, XElement actual)
        {
            using (IEnumerator<XAttribute> expectedAttributes = CreateAttributeEnumerator(expected),
                                           actualAttributes = CreateAttributeEnumerator(actual))
            {
                bool moreExpectedAttributes, moreActualAttributes;

                while (true)
                {
                    moreExpectedAttributes = expectedAttributes.MoveNext();
                    moreActualAttributes = actualAttributes.MoveNext();

                    // Attribute quantity must match.
                    if (!moreExpectedAttributes || !moreActualAttributes)
                    {
                        if (moreExpectedAttributes ^ moreActualAttributes)
                        {
                            return new XmlComparisonResult(
                                false,
                                String.Format(Resources.AssertionFailure_AttributeQuantityMismatch, actual.Name.LocalName),
                                expected,
                                actual);
                        }

                        break;
                    }

                    // Attribute local name and value must match, and optionally attribute namespace.
                    XmlComparisonResult areEquivalent = m_validateAttributeNamespaceEquivalency(expectedAttributes.Current, actualAttributes.Current, expected, actual);
                    if (!areEquivalent.Result) { return areEquivalent; }

                    if (expectedAttributes.Current.Name.LocalName != actualAttributes.Current.Name.LocalName)
                    {
                        return new XmlComparisonResult(
                            false,
                            String.Format(
                                Resources.AssertionFailure_UnexpectedAttribute,
                                actualAttributes.Current.Name.LocalName,
                                actual.Name.LocalName),
                            expected,
                            actual);
                    }

                    if (expectedAttributes.Current.Value != actualAttributes.Current.Value)
                    {
                        return new XmlComparisonResult(
                            false,
                            String.Format(
                                Resources.AssertionFailure_AttributeValueMismatch,
                                actualAttributes.Current.Name.LocalName,
                                actual.Name.LocalName,
                                expectedAttributes.Current.Value,
                                actualAttributes.Current.Value),
                            expected,
                            actual);
                    }
                }
            }

            return SuccessfulComparisonResult;
        }

        /// <summary>
        /// Creates an enumerator to enumerate the given element's attributes,
        /// according to the configuration of the class.
        /// </summary>
        /// 
        /// <param name="element">
        /// The element whose attributes are enumerated.
        /// </param>
        private IEnumerator<XAttribute> CreateAttributeEnumerator(XElement element)
        {
            return element.Attributes()
                          .Where(a => !a.IsNamespaceDeclaration)    // TODO: implement generic ! functor.
                          .OrderBy(m_attributeKeySelector)
                          .GetEnumerator();
        }


        /// <summary>
        /// Determines if a value is contained in an XmlComparisonFlags enumeration.
        /// </summary>
        /// 
        /// <param name="flag">
        /// The flag to search for.
        /// </param>
        /// 
        /// <param name="value">
        /// The value that is queried.
        /// </param>
        private static bool ContainsFlag(XmlComparisonFlags flag, XmlComparisonFlags value)
        {
            // TODO: Generalize this method.
            return (value & flag) == flag;
        }

        /// <summary>
        /// Compares the namespaces of two given attributes for equality.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The attribute containing the expected namespace value.
        /// </param>
        /// 
        /// <param name="actual">
        /// The attribute containing the namespace to validate.
        /// </param>
        /// 
        /// <param name="expectedElement">
        /// The element containing <paramref name="expected"/>.
        /// </param>
        /// 
        /// <param name="actualElement">
        /// The element containing <paramref name="actual"/>.
        /// </param>
        private static XmlComparisonResult CompareAttributeNamespaces(XAttribute expected, XAttribute actual, XElement expectedElement, XElement actualElement)
        {
            if (expected.Name.Namespace == actual.Name.Namespace) { return SuccessfulComparisonResult; }

            return new XmlComparisonResult(
                false,
                String.Format(Resources.AssertionFailure_AttributeNamespaceMismatch,
                    actual.Name.LocalName,
                    actualElement.Name.LocalName,
                    expected.Name.Namespace,
                    actual.Name.Namespace),
                expectedElement,
                actualElement);
        }

        /// <summary>
        /// Compares the namespaces of two given elements for equality.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The element containing the expected namespace value.
        /// </param>
        /// 
        /// <param name="actual">
        /// The element containing the namespace to validate.
        /// </param>
        private static XmlComparisonResult CompareElementNamespaces(XElement expected, XElement actual)
        {
            if (expected.Name.Namespace == actual.Name.Namespace) { return SuccessfulComparisonResult; }

            return new XmlComparisonResult(
                false,
                String.Format(Resources.AssertionFailure_ElementNamespaceMismatch,
                        actual.Name.LocalName,
                        expected.Name.Namespace,
                        actual.Name.Namespace),
                expected,
                actual);
        }

        /// <summary>
        /// Compares the values of two given elements for equality.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The element containing the expected value.
        /// </param>
        /// 
        /// <param name="actual">
        /// The element containing the value to validate.
        /// </param>
        /// 
        /// <remarks>
        /// The value of an element is defined as the concatenation of an
        /// element's child text nodes.
        /// </remarks>
        private static XmlComparisonResult CompareElementValues(XElement expected, XElement actual)
        {
            string expectedValue = GetElementValue(expected);
            string actualValue = GetElementValue(actual);

            if (expectedValue == actualValue) { return SuccessfulComparisonResult; }

            return new XmlComparisonResult(
                false,
                String.Format(Resources.AssertionFailure_ElementValueMismatch,
                    actual.Name.LocalName,
                    expectedValue,
                    actualValue),
                expected,
                actual);
        }

        /// <summary>
        /// Gets the value of an element by concatenating only its
        /// child text nodes.
        /// </summary>
        /// 
        /// <param name="element">
        /// The element whose value is retrieved.
        /// </param>
        private static string GetElementValue(XElement element)
        {
            StringBuilder valueBuilder = new StringBuilder();
            foreach (XNode node in element.Nodes().OfType<XText>())
            {
                valueBuilder.Append(node.ToString());
            }

            return valueBuilder.ToString();
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private readonly XmlComparisonFlags m_comparisonFlags;
        private readonly Func<XElement, XElement, XmlComparisonResult> m_validateAttributeEquivalency;
        private readonly Func<XAttribute, XAttribute, XElement, XElement, XmlComparisonResult> m_validateAttributeNamespaceEquivalency;
        private readonly Func<XElement, XElement, XmlComparisonResult> m_validateElementValueEquivalency;
        private readonly Func<XElement, XElement, XmlComparisonResult> m_validateElementNamespaceEquivalency;
        private readonly Func<XElement, XElement, XmlComparisonResult> m_areEquivalent;
        private readonly Func<XAttribute, string> m_attributeKeySelector;

        private static readonly XmlReaderSettings ReaderSettings;
        private static readonly XmlComparisonResult SuccessfulComparisonResult;

        #endregion
    }
}