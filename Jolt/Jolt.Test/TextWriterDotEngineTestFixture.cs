// ----------------------------------------------------------------------------
// TextWriterDotEngineTestFixture.cs
//
// Contains the definition of the TextWriterDotEngineTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/16/2009 19:35:13
// ----------------------------------------------------------------------------

using System.IO;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using Rhino.Mocks;

namespace Jolt.Test
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
            With.Mocks(delegate
            {
                TextWriter writer = Mocker.Current.CreateMock<TextWriter>();

                // Expectations.
                // GraphViz data is written to the writer.
                string expectedGraphViz = "graph-viz-data";
                writer.Write(expectedGraphViz);

                // Verification and assertions.
                Mocker.Current.ReplayAll();

                IDotEngine engine = new TextWriterDotEngine(writer);
                string expectedResult = Path.GetRandomFileName();
                string result = engine.Run(GraphvizImageType.Png, expectedGraphViz, expectedResult);

                Assert.That(result, Is.SameAs(expectedResult));
            });
        }
    }
}