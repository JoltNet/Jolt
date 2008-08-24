// ----------------------------------------------------------------------------
// MethodDeclarerHelper.cs
//
// Contains the definition of the MethodDeclarerHelper class.
// Copyright 2008 Steve Guidi.
//
// File created: 7/26/2008 09:03:59
// ----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jolt.Testing.CodeGeneration
{
    /// <summary>
    /// Contains helper methods for the MethodDeclarer and
    /// MethodDeclarerImpl heirarchies.
    /// </summary>
    internal static class MethodDeclarerHelper
    {
        #region internal methods ------------------------------------------------------------------

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
        /// Throws an InvalidOperationException when the given predicate evaluates to false.
        /// </summary>
        /// 
        /// <param name="isMethodValid">
        /// The predicate to evaluate.
        /// </param>
        /// 
        /// <param name="method">
        /// The predicate parameter.
        /// </param>
        /// 
        /// <param name="exceptionMessageResourceString">
        /// The resource string containing the exception message.
        /// Must contain a substitution parameter at position 0 representing the method name.
        /// </param>
        internal static void ValidateMethod<TMethod>(TMethod method, Predicate<TMethod> isMethodValid, string exceptionMessageResourceString)
            where TMethod : MethodBase
        {
            if (!isMethodValid(method))
            {
                throw new InvalidOperationException(String.Format(exceptionMessageResourceString, method.Name));
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
            return Array.Exists(parameters, IsGeneric);
        }

        #endregion

        #region internal delegate types -----------------------------------------------------------

        /// <summary>
        /// Defines a delegate that matches the method signatures of
        /// <seealso cref="MethodBuilder.DefineParameter"/> and
        /// <seealso cref="ConstructorBuilder.DefineParameter"/>.
        /// </summary>
        /// 
        /// <param name="nParamPosition">
        /// The one-based index of the parameter in the method signature.
        /// </param>
        /// 
        /// <param name="paramAttributes">
        /// The attributes of the parameter.
        /// </param>
        /// 
        /// <param name="sParamName">
        /// The parameter name.
        /// </param>
        internal delegate ParameterBuilder DefineParameterDelegate(int nParamPosition, ParameterAttributes paramAttributes, string sParamName);

        #endregion

        #region private methods -------------------------------------------------------------------

        /// <summary>
        /// Returns TRUE when the given parameter is generic, FALSE otherwise.
        /// </summary>
        /// 
        /// <param name="parameter">
        /// The parameter to validate.
        /// </param>
        private static bool IsGeneric(ParameterInfo parameter)
        {
            return parameter.ParameterType.IsGenericParameter;
        }

        #endregion
    }
}
