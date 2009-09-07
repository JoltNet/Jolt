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
    /// Converts a FiniteStateMachine class to various external representations.
    /// </summary>
    public static class FsmConverter
    {
        #region public methods --------------------------------------------------------------------

        /// <summary>
        /// Serializes the given finite state machine to GraphML.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="fsm">
        /// The finite state machine to convert.
        /// </param>
        /// 
        /// <param name="graphMLWriter">
        /// The target of the serialization operation.
        /// </param>
        /// 
        /// <remarks>
        /// The given XmlWriter is not closed by this method.
        /// </remarks>
        public static void ToGraphML<TAlphabet>(FiniteStateMachine<TAlphabet> fsm, XmlWriter graphMLWriter)
        {
            // The GraphML serializer requires that all serializable properties
            // exist on the vertex, edge, and graph objects.  Since the FSM object
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
        /// Deserializes the given GraphML data stream to a FiniteStateMachine
        /// object.
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
        /// <remarks>
        /// The given TextReader is not closed by this method.
        /// </remarks>
        public static FiniteStateMachine<TAlphabet> FromGraphML<TAlphabet>(TextReader graphMLReader)
        {
            // For the same reasons given in the ToGraphML() function, this method
            // will deserialize the GraphML data stream to a temporary graph,
            // then respectively convert each vertex and edge to an FSM state and edge.
            FiniteStateMachine<TAlphabet> fsm = new FiniteStateMachine<TAlphabet>();
            BidirectionalGraph<GraphMLState, GraphMLTransition<TAlphabet>> graph = new BidirectionalGraph<GraphMLState, GraphMLTransition<TAlphabet>>();

            // Convert vertices to states as they become available.
            graph.VertexAdded += delegate(GraphMLState vertex)
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
        /// Serializes the given finite state machine to binary.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="fsm">
        /// The finite state machine to convert.
        /// </param>
        ///
        /// <param name="stream">
        /// The target of the serialization operation.
        /// </param>
        /// 
        /// <remarks>
        /// The given Stream is not closed by this method.
        /// </remarks>
        public static void ToBinary<TAlphabet>(FiniteStateMachine<TAlphabet> fsm, Stream targetStream)
        {
            new BinaryFormatter().Serialize(targetStream, fsm);
        }

        /// <summary>
        /// Deserializes the given data stream to a FiniteStateMachine object.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="stream">
        /// The binary stream to convert.
        /// </param>
        /// 
        /// <remarks>
        /// The given Stream is not closed by this method.
        /// </remarks>
        public static FiniteStateMachine<TAlphabet> FromBinary<TAlphabet>(Stream binaryStream)
        {
            return (FiniteStateMachine<TAlphabet>)(new BinaryFormatter().Deserialize(binaryStream));
        }

        /// <summary>
        /// Serializes the given finite state machine to a GraphViz representation.
        /// </summary>
        /// 
        /// <typeparam name="TAlphabet">
        /// The type that represents the alphabet operated upon by the
        /// finite state machine.
        /// </typeparam>
        ///
        /// <param name="fsm">
        /// The finite state machine to convert.
        /// </param>
        ///
        /// <param name="writer">
        /// The target of the serialization operation.
        /// </param>
        /// 
        /// <remarks>
        /// The given TextWriter is not closed by this method.
        /// </remarks>
        public static void ToGraphViz<TAlphabet>(FiniteStateMachine<TAlphabet> fsm, TextWriter writer)
        {
            GraphvizAlgorithm<string, Transition<TAlphabet>> algorithm = new GraphvizAlgorithm<string, Transition<TAlphabet>>(fsm.AsGraph);

            algorithm.FormatEdge += (s, args) => args.EdgeFormatter.Label.Value = args.Edge.Description;
            algorithm.FormatVertex += delegate(object sender, FormatVertexEventArgs<string> args)
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

        #endregion
    }
}