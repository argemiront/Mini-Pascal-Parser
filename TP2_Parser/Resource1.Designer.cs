﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TP2_Parser {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource1 {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource1() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TP2_Parser.Resource1", typeof(Resource1).Assembly);
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
        ///   Looks up a localized string similar to (\s+|:|;|\[|\]|\(|\)|\+|-|\*|\/|&lt;|&gt;|\.|,|=)$.
        /// </summary>
        internal static string STR_DELIMITADORES {
            get {
                return ResourceManager.GetString("STR_DELIMITADORES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (([a-z]|[A-Z]|_)([a-z]|[A-Z]|_|[0-9])*)$.
        /// </summary>
        internal static string STR_ID {
            get {
                return ResourceManager.GetString("STR_ID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (PROGRAM|OF|BEGIN|END|VAR|ARRAY|CHAR|BOOLEAN|INTEGER|FUNCTION|PROCEDURE|IF|READ|WRITE|ELSE|WHILE|DO|THEN|NOT|AND|MOD|DIV|OR|CONST|TYPE|RECORD)$.
        /// </summary>
        internal static string STR_KEY {
            get {
                return ResourceManager.GetString("STR_KEY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (&apos;([^&apos;])+&apos;)$.
        /// </summary>
        internal static string STR_LIT {
            get {
                return ResourceManager.GetString("STR_LIT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (\d+)$.
        /// </summary>
        internal static string STR_NUM {
            get {
                return ResourceManager.GetString("STR_NUM", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (\+|-|\*|\/)$.
        /// </summary>
        internal static string STR_OP {
            get {
                return ResourceManager.GetString("STR_OP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (&gt;|&gt;=|&lt;|&lt;=|&lt;&gt;|=)$.
        /// </summary>
        internal static string STR_RELOP {
            get {
                return ResourceManager.GetString("STR_RELOP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (\.|,|\[|\]|\(|\)|:=|;|\.\.|:)$.
        /// </summary>
        internal static string STR_TERM {
            get {
                return ResourceManager.GetString("STR_TERM", resourceCulture);
            }
        }
    }
}
