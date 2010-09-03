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
    /// Provides assertion methods for performing user-defined equivalency
    /// assertions against two XML structures.
    /// </summary>
    public class XmlEquivalencyAssertion
    {
        #region constructors ----------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance of the <see cref="XmlEquivalencyAssertion"/> class,
        /// accepting the equivalency strictness.
        /// </summary>
        /// 
        /// <param name="strictness">
        /// An <see cref="XmlComparisonFlags"/> enumeration denoting the definition of
        /// equivalency for this instance.
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

        /// <summary>
        /// Initializes the static state of the <see cref="XmlEquivalencyAssertion"/> class.
        /// </summary>
        static XmlEquivalencyAssertion()
        {
            ReaderSettings = new XmlReaderSettings();
            ReaderSettings.IgnoreComments = true;
            ReaderSettings.IgnoreProcessingInstructions = true;
            ReaderSettings.IgnoreWhitespace = true;

            SuccessfulComparisonResult = new XmlComparisonResult();
        }

        #endregion

        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Compares two <see cref="System.Xml.XmlReader"/> objects for equivalency.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected XML, referenced by an <see cref="System.Xml.XmlReader"/>.
        /// </param>
        /// 
        /// <param name="actual">
        /// The XML to validate, referenced by an <see cref="System.Xml.XmlReader"/>.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlComparisonResult"/> containing the result
        /// of the assertion.
        /// </returns>
        ///
        /// <remarks>
        /// Equivalency is defined as per the configuration of the <see cref="XmlEquivalencyAssertion"/>
        /// instance.
        /// </remarks>
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
        /// Gets the <see cref="XmlComparisonFlags"/> associated with <see cref="XmlEquivalencyAssertion"/>
        /// instance.
        /// </summary>
        internal XmlComparisonFlags ComparisonFlags
        {
            get { return m_comparisonFlags; }
        }

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Compares two <see cref="System.Xml.XLinq.XElement"/> objects for equivalency,
        /// and verifying that the order of child elements are the same in both XML
        /// trees.
        /// </summary>
        /// 
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
        /// Compares two <see cref="System.Xml.XLinq.XElement"/> objects for equivalency,
        /// disregarding the order of child elements both XML trees.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected XML, referenced by an <see cref="System.Xml.XLinq.XElement"/>.
        /// </param>
        /// 
        /// <param name="actual">
        /// The XML to validate, referenced by an <see cref="System.Xml.XLinq.XElement"/>.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlComparisonResult"/> containing the result
        /// of the comparison.
        /// </returns>
        /// 
        /// <remarks>
        /// Orders the elements in the <paramref name="actual"/> XML tree so that the "ordered"
        /// version of this algorithm may detect missing elements.
        /// </remarks>
        private XmlComparisonResult AreEquivalentAndUnordered(XElement expected, XElement actual)
        {
            NormalizeElementOrder_FirstPass(expected, actual);
            NormalizeElementOrder_SecondPass(expected, actual);
            return AreEquivalentAndOrdered(expected, actual);  // TODO: consider comparison message upon detection of error.
        }

        /// <summary>
        /// Orders the child elements of the XML referenced by <paramref name="actual"/>
        /// such that their order matches that of the XML referenced by <paramref name="expected"/>.
        /// Does not consider a child element's children when searching for a matching reference
        /// element.
        /// </summary>
        /// 
        /// <param name="expected">
        /// An <see cref="System.Xml.XLinq.XElement"/> whose child elements define the desired element order.
        /// </param>
        /// 
        /// <param name="actual">
        /// An <see cref="System.Xml.XLinq.XElement"/> whose child elements are to be reordered.
        /// </param>
        /// 
        /// <remarks>
        /// An element is moved if it is considered equivalent to the reference element as per
        /// the configuration of the <see cref="XmlEquivalencyAssertion"/> instance.  Furthermore,
        /// the function returns immediately when an element can not be paired against an existing
        /// reference element.
        /// </remarks>
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
        /// Orders the child elements of the XML referenced by <paramref name="actual"/>
        /// such that their order matches that of the XML referenced by <paramref name="expected"/>.
        /// Consider a child element's children when searching for a matching reference element.
        /// </summary>
        /// 
        /// <param name="expected">
        /// An <see cref="System.Xml.XLinq.XElement"/> whose child elements define the desired element order.
        /// </param>
        /// 
        /// <param name="actual">
        /// An <see cref="System.Xml.XLinq.XElement"/> whose child elements are to be reordered.
        /// </param>
        /// 
        /// <remarks>
        /// An element is moved if it is considered equivalent to the reference element as per
        /// the configuration of the <see cref="XmlEquivalencyAssertion"/> instance.  Furthermore,
        /// the function returns immediately when an element can not be paired against an existing
        /// reference element.
        /// </remarks>
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

        /// <summary>
        /// Exchanges the position of two elemens in a given collection.
        /// </summary>
        /// 
        /// <typeparam name="TElement">
        /// The type of element contained in the given collection.
        /// </typeparam>
        /// 
        /// <param name="collection">
        /// An <see cref="System.Collections.Generic.IList"/> containing the elements to exchange.
        /// </param>
        /// 
        /// <param name="firstIndex">
        /// The index of the first element to exchange.
        /// </param>
        /// 
        /// <param name="secondIndex">
        /// The index of the second element to exchange.
        /// </param>
        private void SwapElement<TElement>(IList<TElement> collection, int firstIndex, int secondIndex)
        {
            // TODO: Share this code.
            TElement element = collection[firstIndex];
            collection[firstIndex] = collection[secondIndex];
            collection[secondIndex] = element;
        }

        /// <summary>
        /// Compares two <see cref="System.Xml.XLinq.XElement"/> objects for equivalency,
        /// disregarding the analysis child elements either XML tree.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected XML, referenced by an <see cref="System.Xml.XLinq.XElement"/>.
        /// </param>
        /// 
        /// <param name="actual">
        /// The XML to validate, referenced by an <see cref="System.Xml.XLinq.XElement"/>.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlComparisonResult"/> containing the result
        /// of the comparison.
        /// </returns>
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
        /// Adapts <see cref="AreElementsEquivalent"/> method, returning
        /// a Boolean instead of an <see cref="XmlComparisonResult"/>.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The expected XML, referenced by an <see cref="System.Xml.XLinq.XElement"/>.
        /// </param>
        /// 
        /// <param name="actual">
        /// The XML to validate, referenced by an <see cref="System.Xml.XLinq.XElement"/>.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if the given elements are equivalent, false otherwise.
        /// </returns>
        private bool AreEquivalent(XElement expected, XElement actual)
        {
            return AreElementsEquivalent(expected, actual).Result;
        }

        /// <summary>
        /// Compares the attributes two <see cref="System.Xml.XLinq.XElement"/> objects for equivalency.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The <see cref="System.Xml.XLinq.XElement"/> containing the expected set of attributes.
        /// </param>
        /// 
        /// <param name="actual">
        /// The <see cref="System.Xml.XLinq.XElement"/> containing the attributes to validate.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlComparisonResult"/> containing the result
        /// of the comparison.
        /// </returns>
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
        /// Creates a new instance of an <see cref="System.Collections.Generic.IEnumerator"/> for
        /// enumerating the attributes of a given <see cref="System.Xml.XLinq.XElement"/>.
        /// </summary>
        /// 
        /// <param name="element">
        /// The <see cref="System.Xml.XLinq.XElement"/> whose attributes are enumerated.
        /// </param>
        /// 
        /// <returns>
        /// The requested enumerator, which enumerates the attributes of <paramref name="element"/>.
        /// </returns>
        /// 
        /// <remarks>
        /// The returned enumerator ignores namespace declarations and enumerates attributes
        /// by name-sorted order.
        /// </remarks>
        private IEnumerator<XAttribute> CreateAttributeEnumerator(XElement element)
        {
            return element.Attributes()
                          .Where(a => !a.IsNamespaceDeclaration)    // TODO: implement generic ! functor.
                          .OrderBy(m_attributeKeySelector)
                          .GetEnumerator();
        }


        /// <summary>
        /// Determines if an enumeration value is contained in an
        /// <see cref="XmlComparisonFlags"/> enumeration.
        /// </summary>
        /// 
        /// <param name="flag">
        /// The <see cref="XmlComparisonFlags"/> value to search for.
        /// </param>
        /// 
        /// <param name="value">
        /// The <see cref="XmlComparisonFlags"/> that is queried.
        /// </param>
        private static bool ContainsFlag(XmlComparisonFlags flag, XmlComparisonFlags value)
        {
            // TODO: Generalize this method.
            return (value & flag) == flag;
        }

        /// <summary>
        /// Compares the namespaces of two <see cref="System.Xml.XLinq.XAttribute"/> objects for equality.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The <see cref="System.Xml.XLinq.XAttribute"/>  containing the expected namespace value.
        /// </param>
        /// 
        /// <param name="actual">
        /// The <see cref="System.Xml.XLinq.XAttribute"/> containing the namespace value to validate.
        /// </param>
        /// 
        /// <param name="expectedElement">
        /// The <see cref="System.Xml.XLinq.XElement"/> containing <paramref name="expected"/>.
        /// </param>
        /// 
        /// <param name="actualElement">
        /// The <see cref="System.Xml.XLinq.XElement"/> containing <paramref name="actual"/>.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlComparisonResult"/> containing the result
        /// of the comparison.
        /// </returns>
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
        /// Compares the namespaces of two <see cref="System.Xml.XLinq.XElement"/> objects for equality.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The <see cref="System.Xml.XLinq.XElement"/>  containing the expected namespace value.
        /// </param>
        /// 
        /// <param name="actual">
        /// The <see cref="System.Xml.XLinq.XElement"/> containing the namespace value to validate.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlComparisonResult"/> containing the result
        /// of the comparison.
        /// </returns>
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
        /// Compares the element-values of two <see cref="System.Xml.XLinq.XElement"/> objects for equality.
        /// </summary>
        /// 
        /// <param name="expected">
        /// The <see cref="System.Xml.XLinq.XElement"/>  containing the expected value.
        /// </param>
        /// 
        /// <param name="actual">
        /// The <see cref="System.Xml.XLinq.XElement"/> containing the value to validate.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of the <see cref="XmlComparisonResult"/> containing the result
        /// of the comparison.
        /// </returns>
        /// 
        /// <remarks>
        /// The value of an element is defined as the concatenation of an element's child text nodes.
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
        /// Gets the element-value of an <see cref="System.Xml.XLinq.XElement"/>.
        /// </summary>
        /// 
        /// <param name="element">
        /// The <see cref="System.Xml.XLinq.XElement"/> whose value is retrieved.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the element value - a concatenation of the the child text nodes
        /// of <paramref name="element"/>.
        /// </returns>
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

        #region private fields --------------------------------------------------------------------

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