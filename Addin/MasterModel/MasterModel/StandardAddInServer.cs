using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Inventor;
using Microsoft.Win32;
using System.Windows.Forms;
using InvAddIn;
using InvAddIn.Properties;
using Application = Inventor.Application;
using IPictureDisp = stdole.IPictureDisp;

namespace MasterModel
{
    /// <summary> .
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("b7604cb8-7da8-40b8-bed8-4671fdc9c758")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        #region Data Members

        // Inventor application object.
        private Inventor.Application m_inventorApplication;
        private Inventor.ApplicationEvents m_appEvents;

        //buttons
        private BenjaminButton ButtON;
        private BenjaminButton Help;

        //user interface event
        private UserInterfaceEvents m_userInterfaceEvents;

        // ribbon panel
        RibbonPanel partToolMasterRibbonPanel;

        //event handler delegates
        //to make changes in comandbar or enviroment ui
        private Inventor.UserInterfaceEventsSink_OnResetCommandBarsEventHandler UserInterfaceEventsSink_OnResetCommandBarsEventDelegate;
        private Inventor.UserInterfaceEventsSink_OnResetEnvironmentsEventHandler UserInterfaceEventsSink_OnResetEnvironmentsEventDelegate;
        private Inventor.UserInterfaceEventsSink_OnResetRibbonInterfaceEventHandler UserInterfaceEventsSink_OnResetRibbonInterfaceEventDelegate;

        #endregion

        public StandardAddInServer()
        {

        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            try
            {
                //the Activate method is called by Inventor when it loads the addin
                //the AddInSiteObject provides access to the Inventor Application object
                //the FirstTime flag indicates if the addin is loaded for the first time

                //initialize AddIn members
                m_inventorApplication = addInSiteObject.Application;
                InvAddIn.Button.InventorApplication = m_inventorApplication;

                //initialize event delegates
                m_userInterfaceEvents = m_inventorApplication.UserInterfaceManager.UserInterfaceEvents;

                UserInterfaceEventsSink_OnResetCommandBarsEventDelegate = new UserInterfaceEventsSink_OnResetCommandBarsEventHandler(UserInterfaceEvents_OnResetCommandBars);
                m_userInterfaceEvents.OnResetCommandBars += UserInterfaceEventsSink_OnResetCommandBarsEventDelegate;

                UserInterfaceEventsSink_OnResetEnvironmentsEventDelegate = new UserInterfaceEventsSink_OnResetEnvironmentsEventHandler(UserInterfaceEvents_OnResetEnvironments);
                m_userInterfaceEvents.OnResetEnvironments += UserInterfaceEventsSink_OnResetEnvironmentsEventDelegate;

                UserInterfaceEventsSink_OnResetRibbonInterfaceEventDelegate = new UserInterfaceEventsSink_OnResetRibbonInterfaceEventHandler(UserInterfaceEvents_OnResetRibbonInterface);
                m_userInterfaceEvents.OnResetRibbonInterface += UserInterfaceEventsSink_OnResetRibbonInterfaceEventDelegate;

                //load image icons for UI items
                //Icon createMasterMIcon = new Icon(this.GetType(), "CreateMasterM.ico");
                IPictureDisp createMasterMIcon = AxHostConverter.ImageToPictureDisp(Resources.CreateMasterM.ToBitmap());
                IPictureDisp createMasterMICON = AxHostConverter.ImageToPictureDisp(Resources.CreateMasterM.ToBitmap());

                //retrieve the GUID for this class
                GuidAttribute addInCLSID;
                addInCLSID = (GuidAttribute)GuidAttribute.GetCustomAttribute(typeof(StandardAddInServer), typeof(GuidAttribute));
                string addInCLSIDString;
                addInCLSIDString = "{" + addInCLSID.Value + "}";

                //create buttons
                ButtON = new BenjaminButton(
                    "MasterModel", "MasterModel:StandardAddInServer:BenjaminBUTTON", CommandTypesEnum.kShapeEditCmdType,
                    addInCLSIDString, "Create a Master Model File",
                    "keep the model simple", createMasterMIcon, createMasterMICON, ButtonDisplayEnum.kDisplayTextInLearningMode);
                //ButtON.HeySherlock = (PartDocument) m_inventorApplication.ActiveDocument;
                Help = new BenjaminButton(
                    "Help", "Help:StandardAddInServer:BenjaminBUTTON", CommandTypesEnum.kShapeEditCmdType,
                    addInCLSIDString, "Help Create a Master Model File",
                    "keep the model simple", createMasterMIcon, createMasterMICON, ButtonDisplayEnum.kDisplayTextInLearningMode);
                //create the command category
                CommandCategory MasterMCmdCategory = m_inventorApplication.CommandManager.CommandCategories.Add("Master Model", "MasterModel:StandardAddInServer:BenjaminBUTTON", addInCLSIDString);

                MasterMCmdCategory.Add(ButtON.ButtonDefinition);
                MasterMCmdCategory.Add(Help.ButtonDefinition);

                if (firstTime == true)
                {
                    //access user interface manager
                    UserInterfaceManager userInterfaceManager;
                    userInterfaceManager = m_inventorApplication.UserInterfaceManager;

                    InterfaceStyleEnum interfaceStyle;
                    interfaceStyle = userInterfaceManager.InterfaceStyle;

                    //create the UI for classic interface
                    if (interfaceStyle == InterfaceStyleEnum.kClassicInterface)
                    {
                        //create toolbar
                        CommandBar MasterCommander;
                        MasterCommander = userInterfaceManager.CommandBars.Add("Master Model", "MasterModel:StandardAddInServer:BenjaminBUTTONToolbar", CommandBarTypeEnum.kRegularCommandBar, addInCLSIDString);

                        //add buttons to toolbar
                        MasterCommander.Controls.AddButton(ButtON.ButtonDefinition, 0);
                        MasterCommander.Controls.AddButton(Help.ButtonDefinition, 0);

                        //Get the 2d sketch environment base object
                        Inventor.Environment partToolEnvironment;
                        partToolEnvironment = userInterfaceManager.Environments["PMxPartSkEnvironment"];

                        //make this command bar accessible in the panel menu for the Tool environment.
                        partToolEnvironment.PanelBar.CommandBarList.Add(MasterCommander);
                    }
                    //create the UI for ribbon interface
                    else
                    {
                        //get the ribbon associated with part document
                        Inventor.Ribbons ribbons;
                        ribbons = userInterfaceManager.Ribbons;

                        Inventor.Ribbon partRibbon;
                        partRibbon = ribbons["Part"];

                        //get the tabs associated with part ribbon
                        RibbonTabs ribbonTabs;
                        ribbonTabs = partRibbon.RibbonTabs;

                        //get the Tool tab
                        RibbonTab partToolRibbonTab;
                        partToolRibbonTab = ribbonTabs["id_TabTools"];

                        //create a new panel with the tab
                        RibbonPanels ribbonPanels;
                        ribbonPanels = partToolRibbonTab.RibbonPanels;

                        partToolMasterRibbonPanel = ribbonPanels.Add("Master Model", "MasterModel:StandardAddInServer:BenjaminBUTTONRibbonPanel", "{DB59D9A7-EE4C-434A-BB5A-F93E8866E872}", "", false);

                        //add controls to the MasterModel panel
                        CommandControls partToolMasterRibbonPanelCtrls;
                        partToolMasterRibbonPanelCtrls = partToolMasterRibbonPanel.CommandControls;

                        //add the buttons to the ribbon panel
                        CommandControl MasterMoRiPaCtrl;
                        MasterMoRiPaCtrl = partToolMasterRibbonPanelCtrls.AddButton(ButtON.ButtonDefinition, false, true, "", false);
                        MasterMoRiPaCtrl = partToolMasterRibbonPanelCtrls.AddButton(Help.ButtonDefinition, false, true, "", false);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.
            m_inventorApplication = null;
            m_appEvents.OnActivateDocument -= new ApplicationEventsSink_OnActivateDocumentEventHandler(ApplicationEvents_OnActivateDocument);

            m_appEvents = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }



        private void ApplicationEvents_OnActivateDocument(_Document DocumentObject,EventTimingEnum BeforeOrAfter,NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            HandlingCode = Inventor.HandlingCodeEnum.kEventNotHandled;

            if (BeforeOrAfter != Inventor.EventTimingEnum.kAfter)
            {
                return;
            }

            HandlingCode =Inventor.HandlingCodeEnum.kEventHandled;
            MessageBox.Show(DocumentObject.DisplayName,"C# - OnActivateDocument",MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void UserInterfaceEvents_OnResetCommandBars(ObjectsEnumerator commandBars, NameValueMap context)
        {
            try
            {
                CommandBar commandBar;
                for (int commandBarCt = 1; commandBarCt <= commandBars.Count; commandBarCt++)
                {
                    commandBar = (Inventor.CommandBar)commandBars[commandBarCt];
                    if (commandBar.InternalName == "MasterModel:StandardAddInServer:BenjaminBUTTONToolbar")
                    {

                        //add buttons to toolbar
                        commandBar.Controls.AddButton(ButtON.ButtonDefinition, 0);
                        commandBar.Controls.AddButton(Help.ButtonDefinition, 0);

                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void UserInterfaceEvents_OnResetEnvironments(ObjectsEnumerator environments, NameValueMap context)
        {
            try
            {
                Inventor.Environment environment;
                for (int environmentCt = 1; environmentCt <= environments.Count; environmentCt++)
                {
                    environment = (Inventor.Environment)environments[environmentCt];
                    if (environment.InternalName == "PMxPartEnvironment")
                    {
                        //make this command bar accessible in the panel menu for the 2d sketch environment.
                        environment.PanelBar.CommandBarList.Add(m_inventorApplication.UserInterfaceManager.CommandBars["MasterModel:StandardAddInServer:BenjaminBUTTONToolbar"]);

                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void UserInterfaceEvents_OnResetRibbonInterface(NameValueMap context)
        {
            try
            {

                UserInterfaceManager userInterfaceManager;
                userInterfaceManager = m_inventorApplication.UserInterfaceManager;

                //get the ribbon associated with part document
                Inventor.Ribbons ribbons;
                ribbons = userInterfaceManager.Ribbons;

                Inventor.Ribbon partRibbon;
                partRibbon = ribbons["Part"];

                //get the tabls associated with part ribbon
                RibbonTabs ribbonTabs;
                ribbonTabs = partRibbon.RibbonTabs;

                RibbonTab partSketchRibbonTab;
                partSketchRibbonTab = ribbonTabs["id_TabSketch"];

                //create a new panel with the tab
                RibbonPanels ribbonPanels;
                ribbonPanels = partSketchRibbonTab.RibbonPanels;

                partToolMasterRibbonPanel = ribbonPanels.Add("MasterModel", "MasterModel:StandardAddInServer:BenjaminBUTTONRibbonPanel",
                                                             "{DB59D9A7-EE4C-434A-BB5A-F93E8866E872}", "", false);

                //add controls to the MasterModel panel
                CommandControls partSketchMasterMoRiPaCtrl;
                partSketchMasterMoRiPaCtrl = partToolMasterRibbonPanel.CommandControls;

                //add the buttons to the ribbon panel
                CommandControl MasterCommander;
                MasterCommander = partSketchMasterMoRiPaCtrl.AddButton(ButtON.ButtonDefinition, false, true, "", false);
                MasterCommander = partSketchMasterMoRiPaCtrl.AddButton(Help.ButtonDefinition, false, true, "", false);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        #endregion
    }
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
