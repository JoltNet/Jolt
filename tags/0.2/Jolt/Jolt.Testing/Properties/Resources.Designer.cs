﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3082
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jolt.Testing.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Jolt.Testing.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The constructor &apos;{0}&apos; must be declared at the same time its corresponding ConstructorBuilder is created..
        /// </summary>
        internal static string Error_DelayedConstructorDeclaration {
            get {
                return ResourceManager.GetString("Error_DelayedConstructorDeclaration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicate method, property, or event detected within a type..
        /// </summary>
        internal static string Error_DuplicateMember {
            get {
                return ResourceManager.GetString("Error_DuplicateMember", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can not add non-static method &apos;{0}&apos; to the proxy builder as the real subject type is abstract..
        /// </summary>
        internal static string Error_InstanceMethodAddedFromAbstractType {
            get {
                return ResourceManager.GetString("Error_InstanceMethodAddedFromAbstractType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The property &apos;{0}&apos; has neither a public getter or a public setter..
        /// </summary>
        internal static string Error_InvalidProperty {
            get {
                return ResourceManager.GetString("Error_InvalidProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The real subject type &apos;{0}&apos; is invalid.  It can not be a delegate or interface type..
        /// </summary>
        internal static string Error_InvalidRealSubjectType {
            get {
                return ResourceManager.GetString("Error_InvalidRealSubjectType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The proxy builder can only accept public methods, properties, and events..
        /// </summary>
        internal static string Error_MemberNotPublic {
            get {
                return ResourceManager.GetString("Error_MemberNotPublic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The method &apos;{0}&apos; does not belong to the proxy builder&apos;s real subject type..
        /// </summary>
        internal static string Error_MethodNotMemberOfRealSubject {
            get {
                return ResourceManager.GetString("Error_MethodNotMemberOfRealSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The non-abstract real subject type &apos;{0}&apos; requires at least one public constructor..
        /// </summary>
        internal static string Error_RealSubjectType_LackingConstructor {
            get {
                return ResourceManager.GetString("Error_RealSubjectType_LackingConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The method &apos;{0}&apos; was not included as part of the proxy type as it is already defined as an event..
        /// </summary>
        internal static string Info_SkipDefinedEventMethod {
            get {
                return ResourceManager.GetString("Info_SkipDefinedEventMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The method &apos;{0}&apos; was not included as part of the proxy type as it is already defined as a property..
        /// </summary>
        internal static string Info_SkipDefinedPropertyMethod {
            get {
                return ResourceManager.GetString("Info_SkipDefinedPropertyMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not locate XML doc comments file for assembly &apos;{0}&apos;.  Procuction of XML doc comments is disabled..
        /// </summary>
        internal static string Info_XmlDocCommentsDisabled {
            get {
                return ResourceManager.GetString("Info_XmlDocCommentsDisabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not load the type &apos;{0}&apos;..
        /// </summary>
        internal static string Warn_TypeNotLoaded {
            get {
                return ResourceManager.GetString("Warn_TypeNotLoaded", resourceCulture);
            }
        }
    }
}
