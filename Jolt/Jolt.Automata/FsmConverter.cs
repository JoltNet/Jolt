// ----------------------------------------------------------------------------
// FsmConverter.cs
//
// Contains the definition of the FsmConverter class.
// Copyright 2009 Steve Guidi.
//
// File created: 1/18/2009 12:38:46 PM
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using Jolt.Automata.QuickGraph;
using Jolt.Functional;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using QuickGraph.Serialization;

namespace Jolt.Automata
{
    /// <summary>
    /// Converts a <see cref="FiniteStateMachine"/> to various external representations.
    /// </summary>
    public static class FsmConverter
    {
        /// <summary>
        /// Serializes the given <see cref="FiniteStateMachine"/> to GraphML.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="fsm">
        /// The <see cref="FiniteStateMachine"/> to convert.
        /// </param>
        /// 
        /// <param name="graphMLWriter">
        /// The <see cref="System.Xml.XmlWriter"/> that receives the serialized data.
        /// </param>
        /// 
        /// <remarks>
        /// <paramref name="graphMLWriter"/> is not closed by this method.
        /// </remarks>
        public static void ToGraphML<TAlphabet>(FiniteStateMachine<TAlphabet> fsm, XmlWriter graphMLWriter)
        {
            // The GraphML serializer requires that all serializable properties
            // exist on explictly on the vertex, edge, and graph objects.  Since the FSM object
            // model contains non-XML-serializable objects (delegates), and that
            // some attributes of verties are owned by the FSM object (start/final
            // states), an intermim representation is required.
            BidirectionalGraph<GraphMLState, GraphMLTransition<TAlphabet>> graph = new BidirectionalGraph<GraphMLState, GraphMLTransition<TAlphabet>>();
            
            // Hash each newly created vertex so that referential integrity
            // is maintained when converting transitions to edges.
            IDictionary<string, GraphMLState> stateToVertexMap = fsm.AsGraph.Vertices.ToDictionary(
                Functor.Identity<string>(),
                state => new GraphMLState(state, fsm.StartState == state, fsm.IsFinalState(state)));

            graph.AddVertexRange(stateToVertexMap.Values);
            graph.AddEdgeRange(fsm.AsGraph.Edges.Select(
                e => new GraphMLTransition<TAlphabet>(
                    stateToVertexMap[e.Source],
                    stateToVertexMap[e.Target],
                    e.Description,
                    e.TransitionPredicate)));

            // Serialize to GraphML.
            int edgeId = 0;
            graph.SerializeToGraphML(
                graphMLWriter,
                delegate(GraphMLState v) { return v.Name.GetHashCode().ToString(); },
                delegate(GraphMLTransition<TAlphabet> e) { return edgeId++.ToString(); });
        }

        /// <summary>
        /// Deserializes the given GraphML data stream to a new <see cref="FiniteStateMachine"/>.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        /// 
        /// <param name="graphMLReader">
        /// The GraphML data stream to convert.
        /// </param>
        ///
        /// <returns>
        /// A new <see cref="FiniteStateMachine"/> equivalent to the given GraphML.
        /// </returns>
        /// 
        /// <remarks>
        /// <paramref name="graphMLReader"/> is not closed by this method.
        /// </remarks>
        public static FiniteStateMachine<TAlphabet> FromGraphML<TAlphabet>(TextReader graphMLReader)
        {
            // For the same reasons given in the ToGraphML() function, this method
            // will deserialize the GraphML data stream to a temporary graph,
            // then respectively convert each vertex and edge to an FSM state and edge.
            FiniteStateMachine<TAlphabet> fsm = new FiniteStateMachine<TAlphabet>();
            BidirectionalGraph<GraphMLState, GraphMLTransition<TAlphabet>> graph = new BidirectionalGraph<GraphMLState, GraphMLTransition<TAlphabet>>();

            // Convert vertices to states as they become available.
            graph.VertexAdded += vertex =>
            {
                fsm.AddState(vertex.Name);
                if (vertex.IsFinalState) { fsm.SetFinalState(vertex.Name); }
                if (vertex.IsStartState) { fsm.StartState = vertex.Name; }
            };

            // Convert edges to transitions as they become available.
            graph.EdgeAdded += edge => fsm.AddTransition(edge.ToTransition());

            // Deserialize from GraphML.
            graph.DeserializeAndValidateFromGraphML(
                graphMLReader,
                id => new GraphMLState(),
                (source, target, id) => new GraphMLTransition<TAlphabet>(source, target));

            return fsm;
        }

        /// <summary>
        /// Serializes the given <see cref="FiniteStateMachine"/> to a binary stream.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="fsm">
        /// The <see cref="FiniteStateMachine"/> to convert.
        /// </param>
        ///
        /// <param name="targetStream">
        /// The <see cref="System.IO.Stream"/> that receives the serailized data.
        /// </param>
        /// 
        /// <remarks>
        /// <paramref name="targetStream"/> is not closed by this method.
        /// </remarks>
        public static void ToBinary<TAlphabet>(FiniteStateMachine<TAlphabet> fsm, Stream targetStream)
        {
            new BinaryFormatter().Serialize(targetStream, fsm);
        }

        /// <summary>
        /// Deserializes the given data stream to a <see cref="FiniteStateMachine"/> object.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="binaryStream">
        /// The binary stream to convert.
        /// </param>
        /// 
        /// <returns>
        /// A new <see cref="FiniteStateMachine"/> equivalent to the given binary data.
        /// </returns>
        /// 
        /// <remarks>
        /// <paramref name="binaryStream"/> is not closed by this method.
        /// </remarks>
        public static FiniteStateMachine<TAlphabet> FromBinary<TAlphabet>(Stream binaryStream)
        {
            return (FiniteStateMachine<TAlphabet>)(new BinaryFormatter().Deserialize(binaryStream));
        }

        /// <summary>
        /// Serializes the given <see cref="FiniteStateMachine"/> to a GraphViz representation.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="fsm">
        /// The <see cref="FiniteStateMachine"/> to convert.
        /// </param>
        ///
        /// <param name="writer">
        /// The <see cref="System.IO.TextWriter"/> that receives the serialized data.
        /// </param>
        /// 
        /// <remarks>
        /// <paramref name="writer"/> is not closed by this method.
        /// </remarks>
        public static void ToGraphViz<TAlphabet>(FiniteStateMachine<TAlphabet> fsm, TextWriter writer)
        {
            GraphvizAlgorithm<string, Transition<TAlphabet>> algorithm = new GraphvizAlgorithm<string, Transition<TAlphabet>>(fsm.AsGraph);

            algorithm.FormatEdge += (s, args) => args.EdgeFormatter.Label.Value = args.Edge.Description;
            algorithm.FormatVertex += (s, args) =>
            {
                if (fsm.IsFinalState(args.Vertex))
                {
                    args.VertexFormatter.Shape = GraphvizVertexShape.DoubleCircle;
                }
                else
                {
                    args.VertexFormatter.Shape = GraphvizVertexShape.Circle;
                }

                if (fsm.StartState == args.Vertex)
                {
                    args.VertexFormatter.Style = GraphvizVertexStyle.Bold;
                }
            };

            algorithm.Generate(new TextWriterDotEngine(writer), String.Empty);
        }
    }
}