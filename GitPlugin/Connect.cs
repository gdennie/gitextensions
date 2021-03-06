using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Win32;
using GitPlugin.Commands;

namespace GitPlugin
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
        private Plugin GitPlugin = null;

		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

        private string GetToolsMenuName()
        {
            string toolsMenuName;

            try
            {
                //If you would like to move the command to a different menu, change the word "Tools" to the 
                //  English version of the menu. This code will take the culture, append on the name of the menu
                //  then add the command to that menu. You can find a list of all the top-level menus in the file
                //  CommandBar.resx.
                string resourceName;
                ResourceManager resourceManager = new ResourceManager("GitPlugin.CommandBar", Assembly.GetExecutingAssembly());
                CultureInfo cultureInfo = new CultureInfo(_applicationObject.LocaleID);

                if (cultureInfo.TwoLetterISOLanguageName == "zh")
                {
                    System.Globalization.CultureInfo parentCultureInfo = cultureInfo.Parent;
                    resourceName = String.Concat(parentCultureInfo.Name, "Tools");
                }
                else
                {
                    resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");
                }
                toolsMenuName = resourceManager.GetString(resourceName);
            }
            catch
            {
                //We tried to find a localized version of the word Tools, but one was not found.
                //  Default to the en-US word, which may work for the current culture.
                toolsMenuName = "Tools";
            }

            return toolsMenuName;
        }

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
            if (GitPlugin != null)
                return;

            try
            {
                CultureInfo cultureInfo = new CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

                _applicationObject = (DTE2)application;

                //Place the command on the tools menu.
                //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
                Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

                CommandBarControl toolsControl = null;
                CommandBarPopup toolsPopup = null;
                try
                {
                    toolsControl = menuBarCommandBar.Controls["Git"];
                }
                catch
                {
                    toolsControl = null;
                }

                try
                {
                    if (toolsControl == null)
                    {
                        toolsControl = menuBarCommandBar.Controls.Add(MsoControlType.msoControlPopup, System.Type.Missing, System.Type.Missing, 4, true);
                        toolsControl.Caption = "Git";
                    }

                    toolsPopup = (CommandBarPopup)toolsControl;
                    toolsPopup.Caption = "Git";
                }
                catch
                {
                    if (toolsControl == null)
                    {
                        toolsControl = menuBarCommandBar.Controls[GetToolsMenuName()];
                        toolsPopup = (CommandBarPopup)toolsControl;
                    }
                }


                GitPlugin = new Plugin((DTE2)application, (AddIn)addInInst, "GitExtensions", "GitPlugin.Connect");
                try
                {
                    GitPlugin.RegisterCommand("GitExtensionsFileHistory", new ToolbarCommand<FileHistory>());
                    GitPlugin.RegisterCommand("GitExtensionsCommit", new ToolbarCommand<Commit>());
                    GitPlugin.RegisterCommand("GitExtensionsBrowse", new ToolbarCommand<Browse>());
                    GitPlugin.RegisterCommand("GitExtensionsClone", new ToolbarCommand<Clone>());
                    GitPlugin.RegisterCommand("GitExtensionsCreateBranch", new ToolbarCommand<CreateBranch>());
                    GitPlugin.RegisterCommand("GitExtensionsSwitchBranch", new ToolbarCommand<SwitchBranch>());
                    GitPlugin.RegisterCommand("GitExtensionsDiff", new ToolbarCommand<ViewDiff>());
                    GitPlugin.RegisterCommand("GitExtensionsInitRepository", new ToolbarCommand<Init>());
                    GitPlugin.RegisterCommand("GitExtensionsFormatPatch", new ToolbarCommand<FormatPatch>());
                    GitPlugin.RegisterCommand("GitExtensionsPull", new ToolbarCommand<Pull>());
                    GitPlugin.RegisterCommand("GitExtensionsPush", new ToolbarCommand<Push>());
                    GitPlugin.RegisterCommand("GitExtensionsRebase", new ToolbarCommand<Rebase>());
                    GitPlugin.RegisterCommand("GitExtensionsRevert", new ToolbarCommand<Revert>());
                    GitPlugin.RegisterCommand("GitExtensionsMerge", new ToolbarCommand<Merge>());
                    GitPlugin.RegisterCommand("GitExtensionsCherryPick", new ToolbarCommand<Cherry>());
                    GitPlugin.RegisterCommand("GitExtensionsStash", new ToolbarCommand<Stash>());
                    GitPlugin.RegisterCommand("GitExtensionsSettings", new ToolbarCommand<Settings>());
                    GitPlugin.RegisterCommand("GitExtensionsSolveMergeConflicts", new ToolbarCommand<SolveMergeConflicts>());
                    GitPlugin.RegisterCommand("GitExtensionsApplyPatch", new ToolbarCommand<ApplyPatch>());
                    GitPlugin.RegisterCommand("GitExtensionsAbout", new ToolbarCommand<About>());
                    GitPlugin.RegisterCommand("GitExtensionsBash", new ToolbarCommand<Bash>());
                    GitPlugin.RegisterCommand("GitExtensionsGitIgnore", new ToolbarCommand<GitIgnore>());
                    GitPlugin.RegisterCommand("GitExtensionsRemotes", new ToolbarCommand<Remotes>());
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }

                try
                {
                    // add the toolbar and menu commands
                    CommandBar commandBar = GitPlugin.AddCommandBar("GitExtensions", MsoBarPosition.msoBarTop);
                    GitPlugin.AddToolbarCommandWithText(commandBar, "GitExtensionsCommit", "Commit", "Commit changes", 7, 1);
                    GitPlugin.AddToolbarCommand(commandBar, "GitExtensionsBrowse", "Browse", "Browse repository", 12, 2);
                    GitPlugin.AddToolbarCommand(commandBar, "GitExtensionsPull", "Pull", "Pull changes from remote repository", 9, 3);
                    GitPlugin.AddToolbarCommand(commandBar, "GitExtensionsPush", "Push", "Push changes to remote repository", 8, 4);
                    GitPlugin.AddToolbarCommand(commandBar, "GitExtensionsStash", "Stash", "Stash changes", 3, 5);
                    GitPlugin.AddToolbarCommand(commandBar, "GitExtensionsSettings", "Settings", "Settings", 2, 6);
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }

                try
                {
                    int n = 1;
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsApplyPatch", "Apply patch", "Apply patch", 0, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsBrowse", "Browse", "Browse repository", 12, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsSwitchBranch", "Checkout branch", "Switch to branch", 10, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsCherryPick", "Cherry pick", "Cherry pick commit", 11, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsCommit", "Commit", "Commit changes", 7, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsCreateBranch", "Create branch", "Create new branch", 10, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsClone", "Clone repository", "Clone existing Git", 14, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsGitIgnore", "Edit .gitignore", "Edit .gitignore file", 0, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsFormatPatch", "Format patch", "Format patch", 0, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsBash", "Git bash", "Start git bash", 0, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsInitRepository", "Initialize new repository", "Initialize new Git repository", 13, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsRemotes", "Manage remotes", "Manage remote repositories", 0, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsMerge", "Merge", "merge", 0, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsPull", "Pull changes", "Pull changes from remote repository", 9, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsPush", "Push changes", "Push changes to remote repository", 8, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsRebase", "Rebase", "Rebase", 0, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsStash", "Stash changes", "Stash changes", 3, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsSettings", "Settings", "Settings", 2, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsSolveMergeConflicts", "Solve mergeconflicts", "Solve mergeconflicts", 0, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsDiff", "View changes", "View commit change history", 0, n++);
                    GitPlugin.AddPopupCommand(toolsPopup, "GitExtensionsAbout", "About Git Extensions", "About Git Extensions", 0, n++);
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }

                try
                {
                    GitPlugin.AddMenuCommand("Item", "GitExtensionsFileHistory", "File history", "Show file history", 6, 4);
                    GitPlugin.AddMenuCommand("Item", "GitExtensionsRevert", "Undo file changes", "Undo changes made to this file", 4, 5);
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
                try
                {
                    GitPlugin.AddMenuCommand("Easy MDI Document Window", "GitExtensionsFileHistory", "File history", "Show file history", 6, 4);
                    GitPlugin.AddMenuCommand("Easy MDI Document Window", "GitExtensionsRevert", "Undo file changes", "Undo changes made to this file", 4, 5);
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }

                try
                {
                    GitPlugin.AddMenuCommand("Code Window", "GitExtensionsFileHistory", "File history", "Show file history", 6, 10);
                    GitPlugin.AddMenuCommand("Code Window", "GitExtensionsRevert", "Undo file changes", "Undo changes made to this file", 4, 11);
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            
		}

        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone &&
                GitPlugin.CanHandleCommand(commandName))
            {
                if (GitPlugin.IsCommandEnabled(commandName))
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                else
                    status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported;
            }
        }

        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;
            if (executeOption != vsCommandExecOption.vsCommandExecOptionDoDefault)
                return;

            handled = GitPlugin.OnCommand(commandName);
        }

		private DTE2 _applicationObject = null;
		private AddIn _addInInstance = null;

        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom) { }
        public void OnAddInsUpdate(ref Array custom) { }
        public void OnStartupComplete(ref Array custom) { }

        public void OnBeginShutdown(ref Array custom)
        {
        }

    }
}