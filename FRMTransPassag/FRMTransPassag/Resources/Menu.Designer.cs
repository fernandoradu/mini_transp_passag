﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FRMTransPassag.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Menu {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Menu() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FRMTransPassag.Resources.Menu", typeof(Menu).Assembly);
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
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;Application&gt;
        ///	&lt;Menus&gt;
        ///		&lt;action type=&quot;add&quot;&gt;
        ///			&lt;Menu Checked=&quot;0&quot; Enabled=&quot;1&quot; FatherUID=&quot;43520&quot; Position=&quot;-1&quot; String=&quot;Transporte de Passageiros&quot; Type=&quot;2&quot; UniqueID=&quot;mnu_separa&quot; Image=&quot;%path%\Imagens\delivery01.bmp&quot;&gt;
        ///				&lt;Menus&gt;
        ///					&lt;action type=&quot;add&quot;&gt;						
        ///						&lt;Menu Checked=&quot;0&quot; Enabled=&quot;1&quot; FatherUID=&quot;mnu_separa&quot; Position=&quot;-1&quot; String=&quot;Conferência de Entrega&quot; Type=&quot;1&quot; UniqueID=&quot;mnu_Entrega&quot;&gt;							
        ///						&lt;/Menu&gt;
        ///					&lt;/action&gt;
        ///				&lt;/Menus&gt;
        ///			&lt;/Menu&gt;
        ///		&lt;/ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string menuadd {
            get {
                return ResourceManager.GetString("menuadd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;Application&gt;
        ///	&lt;Menus&gt;
        ///		&lt;action type=&quot;remove&quot;&gt;
        ///			&lt;Menu UniqueID=&quot;mnu_mymenu&quot;&gt;&lt;/Menu&gt;
        ///		&lt;/action&gt;
        ///	&lt;/Menus&gt;
        ///&lt;/Application&gt;.
        /// </summary>
        internal static string menuremove {
            get {
                return ResourceManager.GetString("menuremove", resourceCulture);
            }
        }
    }
}
