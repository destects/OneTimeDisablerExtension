using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Task = System.Threading.Tasks.Task;
using System.ComponentModel;

namespace OneTimeDisablerExtension.Options {
    public class OptionsPageGrid : DialogPage {
        [Category("One Time Disabler")]
        [DisplayName("Target Extension Identifier")]
        [Description("Identifier of the target extension")]
        public string TargetExtensionIdentifier {
            get;
            set;
        } = string.Empty;
        [Category("One Time Disabler")]
        [DisplayName("Is In Disable Cycle")]
        [Description("Should the target Extension be scheduled for enabling on next restart")]
        public bool IsInDisableCycle {
            get; set;
        } = false;
    }
}
