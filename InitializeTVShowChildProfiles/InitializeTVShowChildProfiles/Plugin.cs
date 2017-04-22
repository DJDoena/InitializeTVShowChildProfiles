using System;
using System.Runtime.InteropServices;
using System.Text;
using Invelos.DVDProfilerPlugin;
using System.Windows.Forms;
using System.IO;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;

namespace DoenaSoft.DVDProfiler.InitializeTVShowChildProfiles
{
    /// <summary>
    /// Summary description for HelloWorld.
    /// </summary>
    /// 
    [Guid("B57886BD-AEA0-4a23-A691-2A36175989B5"), ComVisible(true)]
    public class Plugin : IDVDProfilerPlugin, IDVDProfilerPluginInfo
    {
        internal static Settings Settings;

        private readonly String SettingsFile;

        private readonly String ErrorFile;

        private readonly String ApplicationPath;

        private IDVDProfilerAPI Api;

        private const Int32 MenuId = 1;

        private String MenuTokenISCP = "";

        public Plugin()
        {
            this.ApplicationPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Doena Soft\InitTVShowChildren\";
            this.SettingsFile = ApplicationPath + "InitTVShowChildrenSettings.xml";
            this.ErrorFile = Environment.GetEnvironmentVariable("TEMP") + @"\InitTVShowChildrenCrash.xml";
        }

        public void Load(IDVDProfilerAPI api)
        {
            this.Api = api;
            if(Directory.Exists(this.ApplicationPath) == false)
            {
                Directory.CreateDirectory(this.ApplicationPath);
            }
            if(File.Exists(this.SettingsFile))
            {
                try
                {
                    Settings = Settings.Deserialize(this.SettingsFile);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeRead, this.SettingsFile, ex.Message)
                        , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            CreateSettings();
            this.Api.RegisterForEvent(PluginConstants.EVENTID_FormCreated);
            MenuTokenISCP = this.Api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"DVD", "Initialize TV Show Child Profiles", MenuId);
        }

        public void Unload()
        {
            this.Api.UnregisterMenuItem(MenuTokenISCP);
            try
            {
                Settings.Serialize(this.SettingsFile);
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, this.SettingsFile, ex.Message)
                    , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Api = null;
        }

        public void HandleEvent(Int32 EventType, Object EventData)
        {
            if(EventType == PluginConstants.EVENTID_CustomMenuClick)
            {
                this.HandleMenuClick((Int32)EventData);
            }
        }

        #region IDVDProfilerPluginInfo Members
        public String GetName()
        {
            //if(Settings.DefaultValues.UiCulture!=0)
            //{
            //    return (Texts.ResourceManager.GetString("PluginName", CultureInfo.GetCultureInfo(Settings.DefaultValues.UiCulture)));
            //}
            return (Texts.PluginName);
        }

        public String GetDescription()
        {
            //if(Settings.DefaultValues.UiCulture != 0)
            //{
            //    return (Texts.ResourceManager.GetString("PluginDescription", CultureInfo.GetCultureInfo(Settings.DefaultValues.UiCulture)));
            //}
            return (Texts.PluginDescription);
        }

        public String GetAuthorName()
        {
            return ("Doena Soft.");
        }

        public String GetAuthorWebsite()
        {
            //if(Settings.DefaultValues.UiCulture != 0)
            //{
            //    return (Texts.ResourceManager.GetString("PluginUrl", CultureInfo.GetCultureInfo(Settings.DefaultValues.UiCulture)));
            //}
            return (Texts.PluginUrl);
        }

        public Int32 GetPluginAPIVersion()
        {
            return (PluginConstants.API_VERSION);
        }

        public Int32 GetVersionMajor()
        {
            Version version;

            version = System.Reflection.Assembly.GetAssembly(this.GetType()).GetName().Version;
            return (version.Major);
        }

        public Int32 GetVersionMinor()
        {
            Version version;

            version = System.Reflection.Assembly.GetAssembly(this.GetType()).GetName().Version;
            return (version.Minor * 100 + version.Build * 10 + version.Revision);
        }
        #endregion

        private void HandleMenuClick(Int32 MenuEventID)
        {
            if(MenuEventID == MenuId)
            {
                try
                {
                    using(MainForm mainForm = new MainForm(this.Api))
                    {
                        mainForm.ShowDialog();
                    }
                }
                catch(Exception ex)
                {
                    try
                    {
                        ExceptionXml exceptionXml;

                        MessageBox.Show(String.Format(MessageBoxTexts.CriticalError, ex.Message, this.ErrorFile)
                            , MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        if(File.Exists(this.ErrorFile))
                        {
                            File.Delete(ErrorFile);
                        }
                        exceptionXml = new ExceptionXml(ex);
                        Serializer<ExceptionXml>.Serialize(ErrorFile, exceptionXml);
                    }
                    catch(Exception inEx)
                    {
                        MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, this.ErrorFile, inEx.Message)
                            , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        //private void HandleMenuClick(Int32 MenuEventID)
        //{
        //    if(MenuEventID == MenuId)
        //    {
        //        IDVDInfo parent;
        //        String messageBoxText;

        //        parent = DVDProfilerAPI.GetDisplayedDVD();
        //        messageBoxText = "Initialize child profiles?" + Environment.NewLine
        //            + "Warning: The following data will be overwritten with data of the parent profile:"
        //            + Environment.NewLine + "Region"
        //            + Environment.NewLine + "Video"
        //            + Environment.NewLine + "Studios"
        //            + Environment.NewLine + "Media Companies"
        //            + Environment.NewLine + "SRP (will be set to 0.00)"
        //            + Environment.NewLine + "Release Date"
        //            + Environment.NewLine + "Subtitles"
        //            + Environment.NewLine + "Genres"
        //            + Environment.NewLine + "Country of Origin"
        //            + Environment.NewLine + "Production Year"
        //            + Environment.NewLine + "Cover Images (optional)"
        //            + Environment.NewLine + "Cast (optional)"
        //            + Environment.NewLine + "Crew (optional)";
        //        if((parent.GetProfileID() != null) && (MessageBox.Show(messageBoxText, "Initialize Child Profiles?"
        //            , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
        //        {
        //            DialogResult copyImages;
        //            DialogResult copyCastCrew;

        //            DVDProfilerAPI.DVDByProfileID(out parent, parent.GetProfileID(), -1, -1);
        //            copyImages = MessageBox.Show("Copy Images?", "Copy Images?", MessageBoxButtons.YesNo
        //                , MessageBoxIcon.Question);
        //            copyCastCrew = MessageBox.Show("Copy Cast & Crew?", "Copy Cast & Crew?", MessageBoxButtons.YesNo
        //                , MessageBoxIcon.Question);
        //            for(Int32 i = 0; i < parent.GetBoxSetContentCount(); i++)
        //            {
        //                IDVDInfo child;

        //                DVDProfilerAPI.DVDByProfileID(out child, parent.GetBoxSetContentByIndex(i), -1, -1);
        //                this.CopyFromParent(parent, child, copyImages, copyCastCrew);
        //                DVDProfilerAPI.SaveDVDToCollection(child);
        //                DVDProfilerAPI.UpdateProfileInListDisplay(child.GetProfileID());
        //            }
        //            DVDProfilerAPI.RequeryDatabase();
        //        }
        //    }
        //}

        private static void CreateSettings()
        {
            if(Settings == null)
            {
                Settings = new Settings();
            }
            if(Settings.MainForm == null)
            {
                Settings.MainForm = new SizableForm();
            }
            if(Settings.DefaultValues == null)
            {
                Settings.DefaultValues = new DefaultValues();
            }
        }

        [DllImport("user32.dll")]
        public extern static int SetParent(int child, int parent);

        [ComImport(), Guid("0002E005-0000-0000-C000-000000000046")]
        internal class StdComponentCategoriesMgr { }

        [ComRegisterFunction()]
        public static void RegisterServer(Type t)
        {
            CategoryRegistrar.ICatRegister cr = (CategoryRegistrar.ICatRegister)new StdComponentCategoriesMgr();
            Guid clsidThis = new Guid("B57886BD-AEA0-4a23-A691-2A36175989B5");
            Guid catid = new Guid("833F4274-5632-41DB-8FC5-BF3041CEA3F1");

            cr.RegisterClassImplCategories(ref clsidThis, 1,
                new Guid[] { catid });
        }

        [ComUnregisterFunction()]
        public static void UnregisterServer(Type t)
        {
            CategoryRegistrar.ICatRegister cr = (CategoryRegistrar.ICatRegister)new StdComponentCategoriesMgr();
            Guid clsidThis = new Guid("B57886BD-AEA0-4a23-A691-2A36175989B5");
            Guid catid = new Guid("833F4274-5632-41DB-8FC5-BF3041CEA3F1");

            cr.UnRegisterClassImplCategories(ref clsidThis, 1,
                new Guid[] { catid });
        }
    }
}