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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
            using (TextReader reader = new StreamReader(GetGraphMLResource(LengthMod3MachineResourceName)))
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
                    case "mod3(len) = 0":

                        Assert.That(fsm.StartState, Is.EqualTo(states[i]));
                        Assert.That(fsm.IsFinalState(states[i]));
                        break;

                    case "mod3(len) = 1":
                    case "mod3(len) = 2":

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
            using (TextReader reader = new StreamReader(GetGraphMLResource(LengthMod3MachineResourceName)))
            {
                fsm = FsmConverter.FromGraphML<char>(reader);
            }

            Assert.That(fsm.AsGraph.Edges.ToArray(), Is.EqualTo(FsmFactory.CreateLengthMod3Machine().AsGraph.Edges.ToArray()));
        }

        /// <summary>
        /// Verifies the behavior of the ToBinary() method, for the
        /// vertices of an FSM.
        /// </summary>
        [Test]
        public void ToBinary_Vertices()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            FiniteStateMachine<char> serializedFsm;

            using (Stream stream = new MemoryStream())
            {
                FsmConverter.ToBinary(fsm, stream);
                
                stream.Position = 0;
                serializedFsm = new BinaryFormatter().Deserialize(stream) as FiniteStateMachine<char>;
            }

            Assert.That(serializedFsm, Is.Not.Null);
            Assert.That(fsm.AsGraph.Vertices.ToArray(), Is.EqualTo(serializedFsm.AsGraph.Vertices.ToArray()));
            Assert.That(fsm.StartState, Is.EqualTo(serializedFsm.StartState));
            Assert.That(fsm.FinalStates.ToArray(), Is.EqualTo(fsm.FinalStates.ToArray()));
        }

        /// <summary>
        /// Verifies the behavior of the ToBinary() method, for the
        /// edges of an FSM.
        /// </summary>
        [Test]
        public void ToBinary_Edges()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            fsm.AsGraph.Edges.First().OnTransition += delegate { };

            FiniteStateMachine<char> serializedFsm;
            using (Stream stream = new MemoryStream())
            {
                FsmConverter.ToBinary(fsm, stream);

                stream.Position = 0;
                serializedFsm = new BinaryFormatter().Deserialize(stream) as FiniteStateMachine<char>;
            }

            Assert.That(serializedFsm, Is.Not.Null);
            Assert.That(fsm.AsGraph.Edges.ToArray(), Is.EqualTo(serializedFsm.AsGraph.Edges.ToArray()));
        }

        /// <summary>
        /// Verifies the behavior of the ToBinary() method, when
        /// an object in the FSM's object graph is not serializable.
        /// </summary>
        [Test, ExpectedException(typeof(SerializationException))]
        public void ToBinary_NonSerializable()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            fsm.AsGraph.Edges.First().OnTransition += __EventHandler;

            using (Stream stream = new MemoryStream())
            {
                FsmConverter.ToBinary(fsm, stream);
            }
        }

        /// <summary>
        /// Verifies the behavior of the FromBinary() method, for the
        /// vertices of an FSM.
        /// </summary>
        [Test]
        public void FromBinary_Vertices()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            FiniteStateMachine<char> deserializedFsm;

            using (Stream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, fsm);

                stream.Position = 0;
                deserializedFsm = FsmConverter.FromBinary<char>(stream);
            }

            Assert.That(fsm.AsGraph.Vertices.ToArray(), Is.EqualTo(deserializedFsm.AsGraph.Vertices.ToArray()));
            Assert.That(fsm.StartState, Is.EqualTo(deserializedFsm.StartState));
            Assert.That(fsm.FinalStates.ToArray(), Is.EqualTo(fsm.FinalStates.ToArray()));
        }

        /// <summary>
        /// Verifies the behavior of the FromBinary() method, for the
        /// edges of an FSM.
        /// </summary>
        [Test]
        public void FromBinary_Edges()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            fsm.AsGraph.Edges.First().OnTransition += delegate { };

            FiniteStateMachine<char> deserializedFsm;
            using (Stream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, fsm);

                stream.Position = 0;
                deserializedFsm = FsmConverter.FromBinary<char>(stream);
            }

            Assert.That(fsm.AsGraph.Edges.ToArray(), Is.EqualTo(deserializedFsm.AsGraph.Edges.ToArray()));
        }

        /// <summary>
        /// Verifies the behavior of the FromBinary() method when
        /// the binary stream can not be deserialized to an FSM.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidCastException))]
        public void FromBinary_InvalidStreamContents()
        {
            using (Stream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, "invalid");

                stream.Position = 0;
                FsmConverter.FromBinary<int>(stream);
            }
        }

        #region private methods -------------------------------------------------------------------

        private void __EventHandler(object sender, StateTransitionEventArgs<char> args) { }


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
        private static readonly string LengthMod3MachineResourceName = "Xml.LengthMod3Machine.xml";

        #endregion
    }
}