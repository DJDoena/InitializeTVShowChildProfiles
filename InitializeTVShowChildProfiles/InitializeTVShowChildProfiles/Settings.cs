using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace DoenaSoft.DVDProfiler.InitializeTVShowChildProfiles
{
    [ComVisible(false)]
    [Serializable()]
    public class Settings
    {
        public SizableForm MainForm;

        public string CurrentVersion;

        public DefaultValues DefaultValues;

        private static XmlSerializer s_XmlSerializer;

        [XmlIgnore]
        public static XmlSerializer XmlSerializer
        {
            get
            {
                if (s_XmlSerializer == null)
                {
                    s_XmlSerializer = new XmlSerializer(typeof(Settings));
                }

                return s_XmlSerializer;
            }
        }

        public static void Serialize(string fileName, Settings instance)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var xtw = new XmlTextWriter(fs, Encoding.UTF8))
                {
                    xtw.Formatting = Formatting.Indented;

                    XmlSerializer.Serialize(xtw, instance);
                }
            }
        }

        public void Serialize(string fileName)
        {
            Serialize(fileName, this);
        }

        public static Settings Deserialize(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var xtr = new XmlTextReader(fs))
                {
                    var instance = (Settings)XmlSerializer.Deserialize(xtr);

                    return instance;
                }
            }
        }
    }

    [ComVisible(false)]
    [Serializable()]
    public class BaseForm
    {
        public int Top = 50;

        public int Left = 50;
    }

    [ComVisible(false)]
    [Serializable()]
    public class SizableForm : BaseForm
    {
        public int Height = 500;

        public int Width = 900;

        public FormWindowState WindowState = FormWindowState.Normal;

        public Rectangle RestoreBounds;
    }
}