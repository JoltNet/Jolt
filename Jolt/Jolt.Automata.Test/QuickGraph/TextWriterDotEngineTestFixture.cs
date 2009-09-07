// ----------------------------------------------------------------------------
// TextWriterDotEngineTestFixture.cs
//
// Contains the definition of the TextWriterDotEngineTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/16/2009 19:35:13
// ----------------------------------------------------------------------------

using System.IO;

using Jolt.Automata.QuickGraph;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using Rhino.Mocks;

namespace Jolt.Automata.Test.QuickGraph
{
    [TestFixture]
    public sealed class TextWriterDotEngineTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the Run() method.
        /// </summary>
        [Test]
        public void Run()
        {
            TextWriter writer = MockRepository.GenerateMock<TextWriter>();

            IDotEngine engine = new TextWriterDotEngine(writer);
            string expectedResult = Path.GetRandomFileName();
            string expectedGraphViz = "graph-viz-data";
            string result = engine.Run(GraphvizImageType.Png, expectedGraphViz, expectedResult);

            Assert.That(result, Is.SameAs(expectedResult));

            writer.AssertWasCalled(w => w.Write(expectedGraphViz));
        }
    }
}