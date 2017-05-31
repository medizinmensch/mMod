using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using InvAddIn.Properties;
using Inventor;
using Application = Inventor.Application;
using IPictureDisp = stdole.IPictureDisp;

namespace InvAddIn
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [Guid(ClientId)]
    public class StandardAddInServer : ApplicationAddInServer
    {
        internal static StandardAddInServer Instance;

        // Inventor application object.
        private Application _inventorApplication;

        //button of adding dockable window 
        ButtonDefinition _dockableWindow;

        private const string ClientId = "69fb36d0-465c-46ce-8869-35d5e1af37ca";


        public StandardAddInServer()
        {
            Instance = this;
        }

        internal Application InventorApplication
        {
            get { return _inventorApplication; }
        }

        internal static string ClientIdString
        {
            get { return ClientId; }
        }

        #region ApplicationAddInServer Members

        public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            _inventorApplication = addInSiteObject.Application;

            ControlDefinitions controlDefs = _inventorApplication.CommandManager.ControlDefinitions;
            IPictureDisp smallPicture2 = AxHostConverter.ImageToPictureDisp(Resources.CreateMasterM.ToBitmap());
            IPictureDisp largePicture2 = AxHostConverter.ImageToPictureDisp(Resources.CreateMasterM.ToBitmap());
            _dockableWindow = controlDefs.AddButtonDefinition("Dockable Window", "DockableWindow:Show", CommandTypesEnum.kNonShapeEditCmdType, "{" + ClientId + "}", null, null, smallPicture2, largePicture2);
            _dockableWindow.OnExecute += m_dockableWindow_OnExecute;

            // Get the initial ribbon.
            Ribbon ribbon = _inventorApplication.UserInterfaceManager.Ribbons["ZeroDoc"];
            // Get "Extras" tab.
            RibbonTab extrasTab = ribbon.RibbonTabs["id_TabTools"];

            const string chatPanelInternalName = "DockableWindow:ChatPanel";
            RibbonPanel panel = extrasTab.RibbonPanels.OfType<RibbonPanel>().SingleOrDefault(rp => rp.InternalName == chatPanelInternalName);
            if (panel == null)
                panel = extrasTab.RibbonPanels.Add("Chat", chatPanelInternalName, "{" + ClientId + "}");

            panel.CommandControls.AddButton(_dockableWindow, true);
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.
            Marshal.ReleaseComObject(_inventorApplication);
            _inventorApplication = null;

            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

        /// <summary>
        /// when [ActiveXBrowser] button is clicked
        /// </summary>
        /// <param name="Context"></param>
        /// <remarks></remarks>

        private void m_dockableWindow_OnExecute(NameValueMap Context)
        {
            //TODO: ruf sherlock
            /*var uiMgr = Instance.InventorApplication.UserInterfaceManager;

            var dockableWindows = TestForm.FindDockableWindows(uiMgr);

            if (dockableWindows.Any())
            {
                TestForm.HideDockableWindows(uiMgr, dockableWindows);
            }
            else
            {
                var form = new TestForm();
                form.ShowDockableWindow();
            }*/
        }
    }

    //from http://blogs.msdn.com/b/andreww/archive/2007/07/30/converting-between-ipicturedisp-and-system-drawing-image.aspx

    internal class AxHostConverter : AxHost
    {
        private AxHostConverter()
            : base("")
        {
        }


        public static IPictureDisp ImageToPictureDisp(Image image)
        {
            return (IPictureDisp)GetIPictureDispFromPicture(image);
        }


        public static Image PictureDispToImage(IPictureDisp pictureDisp)
        {
            return GetPictureFromIPicture(pictureDisp);
        }
    }
}
