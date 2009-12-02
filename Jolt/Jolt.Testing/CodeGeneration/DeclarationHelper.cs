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
    /// Contains helper methods facilitating in the declaration of types and methods.
    /// </summary>
    internal static class DeclarationHelper
    {
        /// <summary>
        /// Defines a method's parameters by invoking a custom parameter definition
        /// delegate.
        /// </summary>
        /// 
        /// <param name="defineParameter">
        /// The delegate that defines a parameter on an implicit method.
        /// </param>
        /// 
        /// <param name="parameters">
        /// The <see cref="System.Reflection.ParameterInfo"/> objects that model the
        /// paramters to define.
        /// </param>
        /// 
        /// <remarks>
        /// The parameters are defined in the order given be <paramref name="parameters"/>,
        /// and contain the same name and attributes as those in <paramref name="parameters"/>.
        /// </remarks>
        internal static void DefineParametersWith(DefineParameterDelegate defineParameter, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; ++i)
            {
                defineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);
            }
        }

        /// <summary>
        /// Determines if a given <see cref="System.Reflection.ParameterInfo"/> collection
        /// contains at least one generic parameter.
        /// </summary>
        /// 
        /// <param name="parameters">
        /// The <see cref="System.Reflection.ParameterInfo"/> collection to search.
        /// </param>
        /// 
        /// <returns>
        /// Returns true if at least one element of <see cref="System.Reflection.ParameterInfo"/>
        /// has a generic parameter type.
        /// </returns>
        /// 
        /// <remarks>
        /// Used in-place of <see cref="System.Reflection.MethodBase.ContainsGenericParameters"/>
        /// as this property is not supported in all derivations <see cref="System.Reflection.MethodBase"/>.
        /// </remarks>
        internal static bool ContainsGenericParameters(ParameterInfo[] parameters)
        {
            return Array.Exists(parameters, parameter => parameter.ParameterType.IsGenericParameter);
        }

        /// <summary>
        /// Applies generic type constraints to a given collection of
        /// <see cref="System.Reflection.Emit.GenericTypeParameterBuilder"/> objects.
        /// </summary>
        /// 
        /// <param name="sourceTypes">
        /// The generic <see cref="System.Type"/> objects containing the type
        /// constraints to copy.
        /// </param>
        /// 
        /// <param name="targetTypes">
        /// The <see cref="System.Reflection.Emit.GenericTypeParameterBuilder"/> objects
        /// to which the type constraints are copied to.
        /// </param>
        /// 
        /// <remarks>
        /// Requires that the length of <paramref name="targetTypes"/> is at least the length
        /// of <paramref name="sourceTypes"/>.
        /// </remarks>
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
