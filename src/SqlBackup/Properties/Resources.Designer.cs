﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SqlBackup.Properties {
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
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SqlBackup.Properties.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The argument is null, empty or whitespaces..
        /// </summary>
        public static string ArgumentIsNullOrWhitespace {
            get {
                return ResourceManager.GetString("ArgumentIsNullOrWhitespace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The backup file type &apos;{0}&apos; not supported..
        /// </summary>
        public static string BackupFileTypeUnsupported {
            get {
                return ResourceManager.GetString("BackupFileTypeUnsupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to retrieve database files information..
        /// </summary>
        public static string CorruptDatabaseFileInfo {
            get {
                return ResourceManager.GetString("CorruptDatabaseFileInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No diff backup found from server &apos;{0}&apos;, for database &apos;{1}&apos;, before {2}..
        /// </summary>
        public static string DiffBackupNotFound {
            get {
                return ResourceManager.GetString("DiffBackupNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The database file type &apos;{0}&apos; is not supported..
        /// </summary>
        public static string FileTypeNotSupported {
            get {
                return ResourceManager.GetString("FileTypeNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No full backup found from server &apos;{0}&apos;, for database &apos;{1}&apos;, before {2}..
        /// </summary>
        public static string FullBackupNotFound {
            get {
                return ResourceManager.GetString("FullBackupNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The full backup was not found for differential : {0}..
        /// </summary>
        public static string FullNotFoundForDiffBackup {
            get {
                return ResourceManager.GetString("FullNotFoundForDiffBackup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The table contains an invalid column..
        /// </summary>
        public static string InvalidColumn {
            get {
                return ResourceManager.GetString("InvalidColumn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No backup files given for the restore operation..
        /// </summary>
        public static string NoBackupFileToRestore {
            get {
                return ResourceManager.GetString("NoBackupFileToRestore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Source server and source database options are required when lastest only option is true..
        /// </summary>
        public static string SourceRequiredWithLastestOption {
            get {
                return ResourceManager.GetString("SourceRequiredWithLastestOption", resourceCulture);
            }
        }
    }
}
