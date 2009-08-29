// ----------------------------------------------------------------------------
// FunctorTestFixture.cs
//
// Contains the definition of the FunctorTestFixture class.
// Copyright 2009 Steve Guidi.
//
// File created: 3/23/2009 08:44:38
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Jolt.Functional;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Jolt.Test.Functional
{
    [TestFixture]
    public sealed class FunctorTestFixture
    {
        /// <summary>
        /// Verifies the behavior of the ToAction() method, for functions
        /// that have zero arguments.
        /// </summary>
        [Test]
        public void ToAction_NoArgs()
        {
            With.Mocks(delegate
            {
                Func<int> function = Mocker.Current.CreateMock<Func<int>>();
                Expect.Call(function()).Return(0);
                Mocker.Current.ReplayAll();

                Action action = Functor.ToAction(function);
                action();
            });
        }

        /// <summary>
        /// Verifies the behavior of the ToAction() method, for functions
        /// that have one argument.
        /// </summary>
        [Test]
        public void ToAction_OneArg()
        {
            With.Mocks(delegate
            {
                Func<string, int> function = Mocker.Current.CreateMock<Func<string, int>>();

                string functionArg = "first-arg";
                Expect.Call(function(functionArg)).Return(0);
                Mocker.Current.ReplayAll();

                Action<string> action = Functor.ToAction(function);
                action(functionArg);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ToAction() method, for functions
        /// that have two arguments.
        /// </summary>
        [Test]
        public void ToAction_TwoArgs()
        {
            With.Mocks(delegate
            {
                Func<string, Stream, int> function = Mocker.Current.CreateMock<Func<string, Stream, int>>();

                string functionArg = "first-arg";
                Expect.Call(function(functionArg, Stream.Null)).Return(0);
                Mocker.Current.ReplayAll();

                Action<string, Stream> action = Functor.ToAction(function);
                action(functionArg, Stream.Null);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ToAction() method, for functions
        /// that have three arguments.
        /// </summary>
        [Test]
        public void ToAction_ThreeArgs()
        {
            With.Mocks(delegate
            {
                Func<string, Stream, TextReader, int> function = Mocker.Current.CreateMock<Func<string, Stream, TextReader, int>>();

                string functionArg_1 = "first-arg";
                StreamReader functionArg_3 = new StreamReader(Stream.Null);
                Expect.Call(function(functionArg_1, Stream.Null, functionArg_3)).Return(0);
                Mocker.Current.ReplayAll();

                Action<string, Stream, TextReader> action = Functor.ToAction(function);
                action(functionArg_1, Stream.Null, functionArg_3);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ToAction() method, for functions
        /// that have four arguments.
        /// </summary>
        [Test]
        public void ToAction_FourArgs()
        {
            With.Mocks(delegate
            {
                Func<string, Stream, TextReader, DayOfWeek, int> function = Mocker.Current.CreateMock<Func<string, Stream, TextReader, DayOfWeek, int>>();

                string functionArg_1 = "first-arg";
                StreamReader functionArg_3 = new StreamReader(Stream.Null);
                DayOfWeek functionArg_4 = DayOfWeek.Friday;
                Expect.Call(function(functionArg_1, Stream.Null, functionArg_3, functionArg_4)).Return(0);
                Mocker.Current.ReplayAll();

                Action<string, Stream, TextReader, DayOfWeek> action = Functor.ToAction(function);
                action(functionArg_1, Stream.Null, functionArg_3, functionArg_4);
            });
        }

        /// <summary>
        /// Verifies the behavior of the ToAction() method, accepting
        /// an EventHandler for adapting.
        /// </summary>
        [Test]
        public void ToAction_EventHandler()
        {
            With.Mocks(delegate
            {
                EventHandler<EventArgs> eventHandler = Mocker.Current.CreateMock<EventHandler<EventArgs>>();
                Mocker.Current.ReplayAll();

                Action<object, EventArgs> action = Functor.ToAction(eventHandler);
                Assert.That(action.Method, Is.SameAs(eventHandler.Method));
                Assert.That(action.Target, Is.SameAs(eventHandler.Target));
            });
        }

        /// <summary>
        /// Verifies the behavior of the ToEventHandler() method.
        /// </summary>
        [Test]
        public void ToEventHandler()
        {
            With.Mocks(delegate
            {
                Action<object, EventArgs> action = Mocker.Current.CreateMock<Action<object, EventArgs>>();
                Mocker.Current.ReplayAll();

                EventHandler<EventArgs> eventHandler = Functor.ToEventHandler(action);
                Assert.That(eventHandler.Method, Is.SameAs(action.Method));
                Assert.That(eventHandler.Target, Is.SameAs(action.Target));
            });
        }

        /// <summary>
        /// Verifies the behavior of the ToPredicate() method.
        /// </summary>
        [Test]
        public void ToPredicate()
        {
            With.Mocks(delegate
            {
                Func<int, bool> functionPredicate = Mocker.Current.CreateMock<Func<int, bool>>();
                Mocker.Current.ReplayAll();

                Predicate<int> predicate = Functor.ToPredicate(functionPredicate);
                Assert.That(predicate.Method, Is.SameAs(functionPredicate.Method));
                Assert.That(predicate.Target, Is.SameAs(functionPredicate.Target));
            });
        }

        /// <summary>
        /// Verifies the behavior of the ToPredicateFunc() method.
        /// </summary>
        [Test]
        public void ToPredicateFunc()
        {
            With.Mocks(delegate
            {
                Predicate<int> predicate = Mocker.Current.CreateMock<Predicate<int>>();
                Mocker.Current.ReplayAll();

                Func<int, bool> functionPredicate = Functor.ToPredicateFunc(predicate);
                Assert.That(functionPredicate.Method, Is.SameAs(predicate.Method));
                Assert.That(functionPredicate.Target, Is.SameAs(predicate.Target));
            });
        }

        /// <summary>
        /// Verifies the behavior of the Idempotency() method, for functions
        /// that have zero arguments.
        /// </summary>
        [Test]
        public void Idempotency_NoArgs()
        {
            string constant = "constant-value";
            Func<string> function = Functor.Idempotency(constant);
            Assert.That(function.Target, Is.Not.Null);

            for (int i = 0; i < 200; ++i)
            {
                Assert.That(function(), Is.SameAs(constant));
            }
        }

        /// <summary>
        /// Verifies the behavior of the Idempotency() method, for functions
        /// that have one argument.
        /// </summary>
        [Test]
        public void Idempotency_OneArg()
        {
            string constant = "constant-value";
            Func<int, string> function = Functor.Idempotency<int, string>(constant);
            Assert.That(function.Target, Is.Not.Null);

            for (int i = 0; i < 20; ++i)
            {
                Assert.That(function(i), Is.SameAs(constant));
            }
        }

        /// <summary>
        /// Verifies the behavior of the Idempotency() method, for functions
        /// that have two arguments.
        /// </summary>
        [Test]
        public void Idempotency_TwoArgs()
        {
            string constant = "constant-value";
            Func<int, double, string> function = Functor.Idempotency<int, double, string>(constant);
            Assert.That(function.Target, Is.Not.Null);

            for (int i = 0; i < 200; ++i)
            {
                Assert.That(function(i, 2.5 * i), Is.SameAs(constant));
            }
        }

        /// <summary>
        /// Verifies the behavior of the Idempotency() method, for functions
        /// that have three arguments.
        /// </summary>
        [Test]
        public void Idempotency_ThreeArgs()
        {
            string constant = "constant-value";
            Func<int, double, DateTime, string> function = Functor.Idempotency<int, double, DateTime, string>(constant);
            Assert.That(function.Target, Is.Not.Null);

            for (int i = 0; i < 200; ++i)
            {
                Assert.That(function(i, 2.5 * i, DateTime.Now), Is.SameAs(constant));
            }
        }

        /// <summary>
        /// Verifies the behavior of the Idempotency() method, for functions
        /// that have four arguments.
        /// </summary>
        [Test]
        public void Idempotency_FourArgs()
        {
            string constant = "constant-value";
            Func<int, double, DateTime, char, string> function = Functor.Idempotency<int, double, DateTime, char, string>(constant);
            Assert.That(function.Target, Is.Not.Null);

            for (int i = 0; i < 200; ++i)
            {
                Assert.That(function(i, 2.5 * i, DateTime.Now, System.Convert.ToChar(i)), Is.SameAs(constant));
            }
        }

        /// <summary>
        /// Verifies the behavior of the NoOperation() method, for functions
        /// that have zero arguments.
        /// </summary>
        [Test]
        public void NoOperation_NoArgs()
        {
            Action no_op = Functor.NoOperation();
            Assert.That(no_op.Target, Is.Null);
            Assert.That(no_op.Method, Is.SameAs(GetNoOpMethod()));
        }

        /// <summary>
        /// Verifies the behavior of the NoOperation() method, for functions
        /// that have one argument.
        /// </summary>
        [Test]
        public void NoOperation_OneArg()
        {
            Action<int> no_op = Functor.NoOperation<int>();
            Assert.That(no_op.Target, Is.Null);
            Assert.That(no_op.Method, Is.SameAs(GetNoOpMethod(typeof(int))));
        }

        /// <summary>
        /// Verifies the behavior of the NoOperation() method, for functions
        /// that have two arguments.
        /// </summary>
        [Test]
        public void NoOperation_TwoArgs()
        {
            Action<int, char> no_op = Functor.NoOperation<int, char>();
            Assert.That(no_op.Target, Is.Null);
            Assert.That(no_op.Method, Is.SameAs(GetNoOpMethod(typeof(int), typeof(char))));
        }

        /// <summary>
        /// Verifies the behavior of the NoOperation() method, for functions
        /// that have three arguments.
        /// </summary>
        [Test]
        public void NoOperation_ThreeArgs()
        {
            Action<int, char, string> no_op = Functor.NoOperation<int, char, string>();
            Assert.That(no_op.Target, Is.Null);
            Assert.That(no_op.Method, Is.SameAs(GetNoOpMethod(typeof(int), typeof(char), typeof(string))));
        }

        /// <summary>
        /// Verifies the behavior of the NoOperation() method, for functions
        /// that have four arguments.
        /// </summary>
        [Test]
        public void NoOperation_FourArgs()
        {
            Action<int, char, string, byte> no_op = Functor.NoOperation<int, char, string, byte>();
            Assert.That(no_op.Target, Is.Null);
            Assert.That(no_op.Method, Is.SameAs(GetNoOpMethod(typeof(int), typeof(char), typeof(string), typeof(byte))));
        }

        /// <summary>
        /// Verifies the behavior of the Identity() method.
        /// </summary>
        [Test]
        public void Identity()
        {
            Func<string, string> identity = Functor.Identity<string>();
            Assert.That(identity.Target, Is.Null);

            for (int i = 0; i < 200; ++i)
            {
                string functionArg = new String('z', i);
                Assert.That(identity(functionArg), Is.SameAs(functionArg));
            }
        }

        /// <summary>
        /// Verifies the behavior of the TrueForAll() method.
        /// </summary>
        [Test]
        public void TrueForAll()
        {
            Func<int, bool> predicate = Functor.TrueForAll<int>();
            Assert.That(predicate.Target, Is.Null);

            for (int i = 0; i < 200; ++i)
            {
                Assert.That(predicate(i));
            }
        }

        /// <summary>
        /// Verifies the behavior of the FalseForAll() method.
        /// </summary>
        [Test]
        public void FalseForAll()
        {
            Func<int, bool> predicate = Functor.FalseForAll<int>();
            Assert.That(predicate.Target, Is.Null);

            for (int i = 0; i < 200; ++i)
            {
                Assert.That(!predicate(i));
            }
        }

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Gets the compiler-generated no-op method, constructed with the given
        /// generic method parameters.
        /// </summary>
        /// 
        /// <param name="genericMethodArgs">
        /// The generic method parameters used to construct the resulting method.
        /// </param>
        private static MethodInfo GetNoOpMethod(params Type[] genericMethodArgs)
        {
            MethodInfo noOpMethod =  typeof(Functor).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(
                method => method.Name.StartsWith("<NoOperation>") &&
                          method.GetGenericArguments().Length == genericMethodArgs.Length);
            return !noOpMethod.IsGenericMethod ? noOpMethod : noOpMethod.MakeGenericMethod(genericMethodArgs);
        }

        #endregion
    }
}