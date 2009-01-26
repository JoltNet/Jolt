// ----------------------------------------------------------------------------
// FsmConverter.cs
//
// Contains the definition of the FsmConverter class.
// Copyright 2009 Steve Guidi.
//
// File created: 1/18/2009 12:38:46 PM
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

using QuickGraph;
using QuickGraph.Serialization;

namespace Jolt
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
        /// The destination of the serialization operation.
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
            // TODO: Make the fsm.FinalStates.Contains(s) operation more efficient.
            IDictionary<string, GraphMLState> stateToVertexMap = fsm.AsGraph.Vertices.ToDictionary(
                state => state,
                state => new GraphMLState(state, fsm.StartState == state, fsm.FinalStates.Contains(state)));

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

        #endregion
    }
}