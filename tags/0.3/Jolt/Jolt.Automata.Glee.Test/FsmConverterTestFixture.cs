// ----------------------------------------------------------------------------
// FsmConverterTestFixture.cs
//
// Contains the definition of the FsmConverterTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/27/2009 08:02:04
// ----------------------------------------------------------------------------

using Microsoft.Glee.Drawing;
using System.Collections.Generic;
using System.Linq;

using Jolt.Automata.Test;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Jolt.Automata.Glee.Test
{
    [TestFixture]
    public sealed class FsmConverterTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the ToGleeGraph() method,
        /// for the vertices of an FSM.
        /// </summary>
        [Test]
        public void ToGleeGraph_Vertices()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine(); 
            Graph graph = FsmConverter.ToGleeGraph(fsm);

            Assert.That(fsm.AsGraph.VertexCount, Is.EqualTo(graph.NodeCount));
            
            int nodeId = -1;
            IDictionary<string, string> nodeIdLabelMap = fsm.AsGraph.Vertices.ToDictionary(vertex => (++nodeId).ToString());

            foreach (Node vertex in graph.NodeMap.Values)
            {
                switch (vertex.Id)
                {
                    case "0":

                        Assert.That(fsm.StartState, Is.Not.EqualTo(vertex.Attr.Label));
                        Assert.That(!fsm.IsFinalState(vertex.Attr.Label));
                        Assert.That(!vertex.Attr.Styles.Contains(Style.Bold));
                        Assert.That(vertex.Attr.Shape, Is.EqualTo(Shape.Circle));
                        break;

                    case "1":

                        Assert.That(fsm.StartState, Is.Not.EqualTo(vertex.Attr.Label));
                        Assert.That(!fsm.IsFinalState(vertex.Attr.Label));
                        Assert.That(!vertex.Attr.Styles.Contains(Style.Bold));
                        Assert.That(vertex.Attr.Shape, Is.EqualTo(Shape.Circle));
                        break;

                    case "2":

                        Assert.That(fsm.StartState, Is.EqualTo(vertex.Attr.Label));
                        Assert.That(fsm.IsFinalState(vertex.Attr.Label));
                        Assert.That(vertex.Attr.Styles.Contains(Style.Bold));
                        Assert.That(vertex.Attr.Shape, Is.EqualTo(Shape.DoubleCircle));
                        break;

                    default:

                        Assert.Fail("Unexpect node.");
                        break;
                }

                Assert.That(vertex.Attr.Label, Is.EqualTo(nodeIdLabelMap[vertex.Id]));
            }
        }

        /// <summary>
        /// Verifies the behavior of the ToGleeGraph() method,
        /// for the edges of an FSM.
        /// </summary>
        [Test]
        public void ToGleeGraph_Edges()
        {
            FiniteStateMachine<char> fsm = FsmFactory.CreateLengthMod3Machine();
            Graph graph = FsmConverter.ToGleeGraph(fsm);
            Edge[] graphEdges = graph.Edges.ToArray();

            Assert.That(fsm.AsGraph.EdgeCount, Is.EqualTo(graph.EdgeCount));
            Assert.That(fsm.AsGraph.Edges.Select(edge => edge.Description).ToArray(),
                Is.EquivalentTo(graphEdges.Select(edge => edge.Attr.Label).ToArray()));

            // Expected transition order:
            // mod2 -> mod1
            // mod1 -> mod0
            // mod0 -> mod2
            for (int vertexId = 0; vertexId < graphEdges.Length; ++vertexId)
            {
                Assert.That(graphEdges[vertexId].Source, Is.EqualTo(vertexId.ToString()));
                Assert.That(graphEdges[vertexId].Target, Is.EqualTo(((vertexId + 1) % 3).ToString()));
            }
        }
    }
}