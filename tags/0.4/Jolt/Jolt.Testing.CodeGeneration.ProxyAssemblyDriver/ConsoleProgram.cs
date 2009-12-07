// ----------------------------------------------------------------------------
// ConsoleProgram.cs
//
// Contains the definition of the ConsoleProgram class.
// Copyright 2007 Steve Guidi.
//
// File created: 6/29/2007 15:05:36
// ----------------------------------------------------------------------------

using System;
using System.IO;

using Jolt.Testing.CodeGeneration.Xml;
using log4net.Config;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Provides a command line interface to the Jolt Proxy Assembly Code Generator.
    /// </summary>
    static class ConsoleProgram
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        /// 
        /// <param name="args">
        /// Command line arguments.
        /// </param>
        static void Main(string[] args)
        {
            BasicConfigurator.Configure();
            ProxyAssemblyBuilder builder;

            try
            {
                // Construct the assembly builder using the correct
                // overload; inspect command line arguments.
                switch (args.Length)
                {
                    // One parameter: the full path of the type import file.
                    case 1:
                        builder = new ProxyAssemblyBuilder();
                        break;

                    // Two parameters: the full path of the type import file; the proxy assembly namepsace.
                    case 2:
                        builder = new ProxyAssemblyBuilder(args[1]);
                        break;

                    // Three parameters: the full path of the type import file; the proxy assembly namepsace; the proxy assembly full path.
                    case 3:
                        builder = new ProxyAssemblyBuilder(args[1], args[2]);
                        break;

                    default:
                        Usage();
                        return;
                }

                // Load the subject types from the configuration file.
                using (Stream stream = File.Open(args[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    foreach (TypeDescriptor descriptor in Xml.XmlConfigurator.LoadRealSubjectTypes(stream))
                    {
                        Console.WriteLine(descriptor.RealSubjectType.FullName);
                        builder.AddType(descriptor.RealSubjectType, descriptor.ReturnTypeOverrides);
                    }
                }

                builder.CreateAssembly();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Prints the program usage to the console output.
        /// </summary>
        private static void Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("ProxyAssemblyGen.exe subjectTypesFullPath [proxyAssemblyNamespace [proxyAssemblyFullPath] ]");
            Console.WriteLine();
            Console.WriteLine("'subjectTypesFullPath' is the full path to an XML configuration file containing the real subject types.");
            Console.WriteLine("'proxyAssemblyNamespace' is the optional namespace of the generated proxy assembly.");
            Console.WriteLine("'proxyAssemblyFullPath' is the optional target path of the generated proxy assembly.");
            Console.WriteLine();
        }
    }
}
