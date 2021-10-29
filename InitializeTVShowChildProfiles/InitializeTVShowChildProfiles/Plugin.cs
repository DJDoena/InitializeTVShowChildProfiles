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

        private readonly string SettingsFile;

        private readonly string ErrorFile;

        private readonly string ApplicationPath;

        private IDVDProfilerAPI Api;

        private const int MenuId = 1;

        private string MenuTokenISCP = "";

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
                    MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeRead, this.SettingsFile, ex.Message)
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
                MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeWritten, this.SettingsFile, ex.Message)
                    , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Api = null;
        }

        public void HandleEvent(int EventType, object EventData)
        {
            if(EventType == PluginConstants.EVENTID_CustomMenuClick)
            {
                this.HandleMenuClick((int)EventData);
            }
        }

        #region IDVDProfilerPluginInfo Members
        public string GetName()
        {
            return (Texts.PluginName);
        }

        public string GetDescription()
        {
            return (Texts.PluginDescription);
        }

        public string GetAuthorName()
        {
            return ("Doena Soft.");
        }

        public string GetAuthorWebsite()
        {
            //if(Settings.DefaultValues.UiCulture != 0)
            //{
            //    return (Texts.ResourceManager.GetString("PluginUrl", CultureInfo.GetCultureInfo(Settings.DefaultValues.UiCulture)));
            //}
            return (Texts.PluginUrl);
        }

        public int GetPluginAPIVersion()
        {
            return (PluginConstants.API_VERSION);
        }

        public int GetVersionMajor()
        {
            Version version;

            version = System.Reflection.Assembly.GetAssembly(this.GetType()).GetName().Version;
            return (version.Major);
        }

        public int GetVersionMinor()
        {
            Version version;

            version = System.Reflection.Assembly.GetAssembly(this.GetType()).GetName().Version;
            return (version.Minor * 100 + version.Build * 10 + version.Revision);
        }
        #endregion

        private void HandleMenuClick(int MenuEventID)
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

                        MessageBox.Show(string.Format(MessageBoxTexts.CriticalError, ex.Message, this.ErrorFile)
                            , MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        if(File.Exists(this.ErrorFile))
                        {
                            File.Delete(ErrorFile);
                        }
                        exceptionXml = new ExceptionXml(ex);
                        DVDProfilerSerializer<ExceptionXml>.Serialize(ErrorFile, exceptionXml);
                    }
                    catch(Exception inEx)
                    {
                        MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeWritten, this.ErrorFile, inEx.Message)
                            , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

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