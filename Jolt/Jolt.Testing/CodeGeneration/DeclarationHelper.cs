// ----------------------------------------------------------------------------
// DeclarationHelper.cs
//
// Contains the definition of the DeclarationHelper class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/26/2008 09:03:59
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    // Defines a delegate that matches the method signatures of
    // MethodBuilder.DefineParameter and ConstructorBuilder.DefineParameter.
    using DefineParameterDelegate = Func<int, ParameterAttributes, string, ParameterBuilder>;


    /// <summary>
    /// Contains helper methods for declaring types and methods.
    /// </summary>
    internal static class DeclarationHelper
    {
        /// <summary>
        /// Defines a copy of the given parameters in the order supplied,
        /// using the given delegate.
        /// </summary>
        /// 
        /// <param name="methodBuilder">
        /// The delegate that defines a parameter on an implicit object.
        /// </param>
        /// 
        /// <param name="parameters">
        /// The parameters that model the paramters to define.
        /// </param>
        internal static void DefineParametersWith(DefineParameterDelegate defineParameter, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; ++i)
            {
                defineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
            }
        }

        /// <summary>
        /// Determines if the given parameter array contains at least one
        /// generic parameter.
        /// </summary>
        /// 
        /// <param name="parameters">
        /// The parameter array to search.
        /// </param>
        /// 
        /// <remarks>
        /// Used in-place of MethodBase.ContainsGenericParameters as the property
        /// is not supported for all derivations of the MethodBase.
        /// </remarks>
        internal static bool ContainsGenericParameters(ParameterInfo[] parameters)
        {
            return Array.Exists(parameters, parameter => parameter.ParameterType.IsGenericParameter);
        }

        /// <summary>
        /// Copies the generic type constraints from a given array of generic types
        /// to another given array of generic type builders.
        /// </summary>
        /// 
        /// <param name="sourceTypes">
        /// The generic types containing the type constraints to copy.
        /// </param>
        /// 
        /// <param name="targetTypes">
        /// The types to which the type constraints are copied to.
        /// </param>
        internal static void CopyTypeConstraints(Type[] sourceTypes, GenericTypeParameterBuilder[] targetTypes)
        {
            if (sourceTypes.Length != targetTypes.Length) { throw new RankException(); }

            for (int i = 0; i < sourceTypes.Length; ++i)
            {
                targetTypes[i].SetGenericParameterAttributes(sourceTypes[i].GenericParameterAttributes);

                Type[] parameterConstraints = sourceTypes[i].GetGenericParameterConstraints();
                targetTypes[i].SetInterfaceConstraints(Array.FindAll(parameterConstraints, constraint => constraint.IsInterface));

                Type baseType = Array.Find(parameterConstraints, constraint => constraint.IsClass);
                if (baseType != null)
                {
                    targetTypes[i].SetBaseTypeConstraint(baseType);
                }
            }
        }
    }
}
