using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using System.Collections.Generic;
using System.Text;

namespace OneTimeDisablerExtension.Commands {
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ListExtensionsCommand {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("a84f652b-4f12-40bd-9963-34e553920b4b");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;
        private readonly IVsExtensionManager _extManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListExtensionsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ListExtensionsCommand(AsyncPackage package, OleMenuCommandService commandService, IVsExtensionManager extManager) {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this._extManager = extManager;
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ListExtensionsCommand Instance {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider {
            get {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package, IVsExtensionManager extenService) {
            // Switch to the main thread - the call to AddCommand in ListExtensionsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ListExtensionsCommand(package, commandService, extenService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();

            var exts = new List<string>();
            var message = new StringBuilder();
            foreach (var ext in _extManager.GetInstalledExtensions().Where(ex => !ex.Header.SystemComponent && !ex.IsPackComponent)) {
                message.Append($"{{Name = {ext.Header.Name} , Identifier = {ext.Header.Identifier}}}" + Environment.NewLine);
            }

            string title = "Extension Identifiers";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.package,
                message.ToString(),
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
