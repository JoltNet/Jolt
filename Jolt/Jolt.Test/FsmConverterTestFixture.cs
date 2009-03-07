// ----------------------------------------------------------------------------
// FsmConverterTestFixture.cs
//
// Contains the definition of the FsmConverterTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 1/18/2009 1:33:24 PM
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Test
{
    [TestFixture]
    public sealed class FsmConverterTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the ToGraphML() method, for
        /// the vertices of an FSM.
        /// </summary>
        [Test]
        public void ToGraphML_Vertices()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            IDictionary<int, GraphMLState> expectedVertices = fsm.AsGraph.Vertices.ToDictionary(
                state => state.GetHashCode(),
                state => new GraphMLState(state, fsm.StartState == state, fsm.IsFinalState(state)));

            XElement[] nodes = CreateGraphMLFor(fsm).Root.Descendants(GraphMLNamespace + "node").ToArray();
            Assert.That(nodes, Has.Length(expectedVertices.Count));

            for (int i = 0; i < nodes.Length; ++i)
            {
                // id
                int id = Int32.Parse(nodes[i].Attribute("id").Value);
                Assert.That(expectedVertices.ContainsKey(id));
                Assert.That(id, Is.EqualTo(expectedVertices[id].Name.GetHashCode()));

                // stateName
                Assert.That(GetDataElement(nodes[i], "stateName").Value, Is.EqualTo(expectedVertices[id].Name));

                // isStartState
                XElement data = GetDataElement(nodes[i], "isStartState");
                if (data != null)
                {
                    Assert.That(Boolean.Parse(data.Value));
                }

                // isFinalState
                data = GetDataElement(nodes[i], "isFinalState");
                if (data != null)
                {
                    Assert.That(Boolean.Parse(data.Value));
                }
            }
        }

        /// <summary>
        /// Verifies the behavior of the ToGraphML() method, for
        /// the edges of an FSM.
        /// </summary>
        [Test]
        public void ToGraphML_Edges()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            Transition<char>[] expectedEdges = fsm.AsGraph.Edges.ToArray();

            XElement[] edges = CreateGraphMLFor(fsm).Root.Descendants(GraphMLNamespace + "edge").ToArray();
            Assert.That(edges, Has.Length(expectedEdges.Length));

            for (int i = 0; i < edges.Length; ++i)
            {
                // id
                Assert.That(Int32.Parse(edges[i].Attribute("id").Value), Is.EqualTo(i));

                // source/target
                Assert.That(Int32.Parse(edges[i].Attribute("source").Value), Is.EqualTo(expectedEdges[i].Source.GetHashCode()));
                Assert.That(Int32.Parse(edges[i].Attribute("target").Value), Is.EqualTo(expectedEdges[i].Target.GetHashCode()));

                // description
                XElement data = GetDataElement(edges[i], "description");
                Assert.That(data, Is.Not.Null);
                Assert.That(data.Value, Is.EqualTo(expectedEdges[i].Description));

                // transitionPredicate
                data = GetDataElement(edges[i], "transitionPredicate");
                Assert.That(data, Is.Not.Null);
                Assert.That(data.Value, Is.EqualTo(String.Concat(
                    expectedEdges[i].TransitionPredicate.Method.Name, ';',
                    expectedEdges[i].TransitionPredicate.Method.DeclaringType.AssemblyQualifiedName)));
            }
        }

        /// <summary>
        /// Verifies the behavior of the FromGraphML() method when
        /// the given GraphML data is invalid.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void FromGraphML_InvalidXml()
        {
            XDocument invalidGraphML = new XDocument(new XElement(GraphMLNamespace + "invalid"));
            using (TextReader reader = new StringReader(invalidGraphML.ToString()))
            {
                FsmConverter.FromGraphML<char>(reader);
            }
        }

        /// <summary>
        /// Verifies the behavior of the FromGraphML() method, for
        /// the vertices of an FSM.
        /// </summary>
        [Test]
        public void FromGraphML_Vertices()
        {
            FiniteStateMachine<char> fsm;
            using (TextReader reader = new StreamReader(GetGraphMLResource("LengthMod3Machine.xml")))
            {
                fsm = FsmConverter.FromGraphML<char>(reader);
            }

            string[] states = fsm.AsGraph.Vertices.ToArray();
            Assert.That(states, Is.Unique);
            Assert.That(states, Has.Length(3));

            for (int i = 0; i < states.Length; ++i)
            {
                switch (states[i])
                {
                    case Mod0State:

                        Assert.That(fsm.StartState, Is.EqualTo(states[i]));
                        Assert.That(fsm.IsFinalState(states[i]));
                        break;

                    case Mod1State:
                    case Mod2State:

                        Assert.That(fsm.StartState, Is.Not.EqualTo(states[i]));
                        Assert.That(!fsm.IsFinalState(states[i]));
                        break;

                    default:

                        Assert.Fail("Unexpected state name.");
                        break;
                }
            }
        }

        /// <summary>
        /// Verifies the behavior of the FromGraphML() method, for
        /// the edges of an FSM.
        /// </summary>
        [Test]
        public void FromGraphML_Edges()
        {
            FiniteStateMachine<char> fsm;
            using (TextReader reader = new StreamReader(GetGraphMLResource("LengthMod3Machine.xml")))
            {
                fsm = FsmConverter.FromGraphML<char>(reader);
            }

            Transition<char>[] transitions = fsm.AsGraph.Edges.ToArray();
            Assert.That(transitions, Has.Length(3));
            Assert.That(transitions.Select(t => t.Source).ToArray(), Is.Unique);

            for (int i = 0; i < transitions.Length; ++i)
            {
                switch (transitions[i].Source)
                {
                    case Mod0State:

                        Assert.That(transitions[i].Target, Is.EqualTo(Mod2State));
                        break;

                    case Mod1State:

                        Assert.That(transitions[i].Target, Is.EqualTo(Mod0State));
                        break;

                    case Mod2State:

                        Assert.That(transitions[i].Target, Is.EqualTo(Mod1State));
                        break;

                    default:

                        Assert.Fail("Unexpected transition.");
                        break;
                }

                Assert.That(transitions[i].Description, Is.EqualTo("<.cctor>b__0"));
                Assert.That(transitions[i].TransitionPredicate, Is.EqualTo(Delegate.CreateDelegate(
                    typeof(Predicate<char>),
                    typeof(FsmFactory).GetMethod(transitions[i].Description, BindingFlags.NonPublic | BindingFlags.Static))));
            }
        }

        #region private class methods -------------------------------------------------------------

        /// <summary>
        /// Creates the GraphML for the given FSM.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        /// 
        /// <param name="fsm">
        /// The finite state machine to serialize to GraphML.
        /// </param>
        private static XDocument CreateGraphMLFor<TAlphabet>(FiniteStateMachine<TAlphabet> fsm)
        {
            XDocument graph = new XDocument();
            using (XmlWriter graphWriter = graph.CreateWriter())
            {
                FsmConverter.ToGraphML(fsm, graphWriter);
            }

            return graph;
        }

        /// <summary>
        /// Retrieves a GraphML data element with the given key, from
        /// the child elements of the given node.
        /// </summary>
        /// 
        /// <param name="node">
        /// The node from which the data element is retrieved.
        /// </param>
        /// 
        /// <param name="key">
        /// The key of the data element.
        /// </param>
        private static XElement GetDataElement(XElement node, string key)
        {
            return node.Elements(GraphMLNamespace + "data").SingleOrDefault(d => d.Attribute("key").Value == key);
        }

        /// <summary>
        /// Retrieves the given embedded resource in a stream.
        /// </summary>
        /// 
        /// <param name="resourceName">
        /// The name of the resource to retrieve.
        /// </param>
        private static Stream GetGraphMLResource(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(FsmConverterTestFixture), resourceName);
        }

        #endregion

        #region private data ----------------------------------------------------------------------

        private static readonly XNamespace GraphMLNamespace = "http://graphml.graphdrawing.org/xmlns";

        private const string Mod0State = "mod3(len) = 0";
        private const string Mod1State = "mod3(len) = 1";
        private const string Mod2State = "mod3(len) = 2";

        #endregion
    }
}