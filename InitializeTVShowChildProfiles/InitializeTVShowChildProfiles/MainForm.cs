using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.InitializeTVShowChildProfiles
{
    internal partial class MainForm : Form
    {
        #region Fields
        private IDVDProfilerAPI Api;
        #endregion

        #region Constructor
        public MainForm(IDVDProfilerAPI api)
        {
            this.Api = api;
            InitializeComponent();
        }
        #endregion

        #region Form Events
        private void OnAboutToolStripMenuItemClick(Object sender, EventArgs e)
        {
            using (AboutBox aboutBox = new AboutBox(this.GetType().Assembly))
            {
                aboutBox.ShowDialog();
            }
        }

        private void OnCheckForUpdatesToolStripMenuItemClick(Object sender, EventArgs e)
        {
            this.CheckForNewVersion();
        }

        private void OnMainFormLoad(Object sender, EventArgs e)
        {
            this.SuspendLayout();
            this.LayoutForm();
            this.ResumeLayout();
            if (Plugin.Settings.CurrentVersion != this.GetType().Assembly.GetName().Version.ToString())
            {
                Plugin.Settings.CurrentVersion = this.GetType().Assembly.GetName().Version.ToString();
            }
        }

        private void OnMainFormClosing(Object sender, FormClosingEventArgs e)
        {
            this.WriteDefaultValues();
        }

        private void OnOkButtonClick(Object sender, EventArgs e)
        {
            IDVDInfo parent;

            parent = this.Api.GetDisplayedDVD();
            if (parent.GetProfileID() != null)
            {
                this.Api.DVDByProfileID(out parent, parent.GetProfileID(), -1, -1);
                for (Int32 i = 0; i < parent.GetBoxSetContentCount(); i++)
                {
                    IDVDInfo child;

                    this.Api.DVDByProfileID(out child, parent.GetBoxSetContentByIndex(i), -1, -1);
                    this.CopyFromParent(parent, child);
                    this.Api.SaveDVDToCollection(child);
                    this.Api.UpdateProfileInListDisplay(child.GetProfileID());
                }
                this.Api.RequeryDatabase();
            }
            this.Close();
        }

        private void OnAbortButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnEntireDvdLockCheckBoxCheckedChanged(Object sender, EventArgs e)
        {
            this.SetLockOnGroupBox.Enabled = (this.EntireDvdLockCheckBox.Checked == false);
        }
        #endregion

        #region Helper Functions
        private void CopyFromParent(IDVDInfo parent, IDVDInfo child)
        {
            #region Media Types
            if (this.MediaTypesCheckBox.Checked)
            {
                Boolean dvd;
                Boolean hddvd;
                Boolean bluRay;
                String custom;

                parent.GetMediaTypes(out dvd, out hddvd, out bluRay);
                child.SetMediaTypes(dvd, hddvd, bluRay);
                parent.GetCustomMediaType(out custom);
                if (String.IsNullOrEmpty(custom) == false)
                {
                    child.SetCustomMediaType(custom);
                }
                else
                {
                    child.SetCustomMediaType(null);
                }
            }
            if (this.MediaTypesLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_MediaTypes, true);
            }
            #endregion
            #region Original Title
            if (this.OriginalTitleCheckBox.Checked)
            {
                child.SetOriginalTitle(parent.GetOriginalTitle());
            }
            #endregion
            #region Production Year
            if (this.ProductionYearCheckBox.Checked)
            {
                child.SetProductionYear(parent.GetProductionYear());
            }
            if (this.ProductionYearLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_ProductionYear, true);
            }
            #endregion
            #region Country of Origin
            if (this.CountryOfOriginCheckBox.Checked)
            {
                for (Int32 c = 1; c < 4; c++)
                {
                    child.SetCountryOfOrigin(c, parent.GetCountryOfOrigin(c));
                }
            }
            if (this.CountryOfOriginLockCheckBox.Checked)
            {
                //
            }
            #endregion
            #region Rating
            if (this.RatingCheckBox.Checked)
            {
                Int32 ratingSystem;
                Int32 ratingAge;
                Int32 ratingVariant;

                parent.GetRating(out ratingSystem, out ratingAge, out ratingVariant);
                child.SetRating(ratingSystem, ratingAge, ratingVariant);
                child.SetRatingDescription(parent.GetRatingDescription());
            }
            if (this.RatingLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Rating, true);
            }
            #endregion
            #region Regions
            if (this.RegionsCheckBox.Checked)
            {
                for (Int32 r = 0; r < 7; r++)
                {
                    child.SetRegionByID(r, parent.GetRegionByID(r));
                }
            }
            if (this.RegionsLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Regions, true);
            }
            #endregion
            #region Release Date
            if (this.ReleaseDateCheckBox.Checked)
            {
                child.SetDVDReleaseDate(parent.GetDVDReleaseDate());
            }
            if (this.ReleaseDateLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_ReleaseDate, true);
            }
            #endregion
            #region Case Type
            if (this.CaseTypeCheckBox.Checked)
            {
                child.SetCaseID(parent.GetCaseID());
                child.SetCaseSlipCover(parent.GetCaseSlipCover());
            }
            if (this.CaseTypeLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_CaseType, true);
            }
            #endregion
            #region SRP
            if (this.SrpCheckBox.Checked)
            {
                child.SetSRPValue(0);
                child.SetSRPCurrency(parent.GetSRPCurrency());
            }
            if (this.SrpLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_PurchasePrice, true);
            }
            #endregion
            #region Genres
            if (this.GenresCheckBox.Checked)
            {
                for (Int32 g = 1; g < 6; g++)
                {
                    child.SetGenre(g, parent.GetGenre(g));
                }
            }
            if (this.GenresLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Genres, true);
            }
            #endregion
            #region Video Formats
            if (this.VideoFormatsCheckBox.Checked)
            {
                Boolean twoDimensional;
                Boolean threeDimensional;
                Boolean anaglyph;

                child.SetAspectRatio(parent.GetAspectRatio());
                child.SetFormatAnamorphic(parent.GetFormatAnamorphic());
                child.SetFormatFullFrame(parent.GetFormatFullFrame());
                child.SetFormatPanScan(parent.GetFormatPanScan());
                child.SetFormatWidescreen(parent.GetFormatWidescreen());
                child.SetVideoStandard(parent.GetVideoStandard());
                child.SetColorType(parent.GetColorType());
                parent.GetDimensions(out twoDimensional, out anaglyph, out threeDimensional);
                child.SetDimensions(twoDimensional, anaglyph, threeDimensional);
            }
            if (this.VideoFormatsLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_VideoFormats, true);
            }
            #endregion
            #region Studios
            if (this.StudiosCheckBox.Checked)
            {
                for (Int32 s = 1; s < 4; s++)
                {
                    child.SetStudio(s, parent.GetStudio(s));
                }
            }
            if (this.StudiosLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Studios, true);
            }
            #endregion
            #region Media Companies
            if (this.MediaCompaniesCheckBox.Checked)
            {
                for (Int32 mc = 1; mc < 4; mc++)
                {
                    child.SetMediaCompany(mc, parent.GetMediaCompany(mc));
                }
            }
            if (this.MediaCompaniesLockCheckBox.Checked)
            {
                //
            }
            #endregion
            #region Features
            if (this.FeaturesCheckBox.Checked)
            {
                SetFeature(parent, child, PluginConstants.FEATURE_SceneAccess);
                SetFeature(parent, child, PluginConstants.FEATURE_Trailer);
                SetFeature(parent, child, PluginConstants.FEATURE_BonusTrailers);

                SetFeature(parent, child, PluginConstants.FEATURE_Documentary); // Featurettes
                SetFeature(parent, child, PluginConstants.FEATURE_Commentary);
                SetFeature(parent, child, PluginConstants.FEATURE_DeletedScenes);
                SetFeature(parent, child, PluginConstants.FEATURE_Interviews);
                SetFeature(parent, child, PluginConstants.FEATURE_Bloopers);

                SetFeature(parent, child, PluginConstants.FEATURE_StoryboardComps);
                SetFeature(parent, child, PluginConstants.FEATURE_Gallery);
                SetFeature(parent, child, PluginConstants.FEATURE_ProductionNotes);

                SetFeature(parent, child, PluginConstants.FEATURE_DVDROMContent);
                SetFeature(parent, child, PluginConstants.FEATURE_InteractiveGame);
                SetFeature(parent, child, PluginConstants.FEATURE_MultiAngle);
                SetFeature(parent, child, PluginConstants.FEATURE_MusicVideos);

                SetFeature(parent, child, PluginConstants.FEATURE_THX);
                SetFeature(parent, child, PluginConstants.FEATURE_ClosedCaptioned);

                SetFeature(parent, child, PluginConstants.FEATURE_DigitalCopy);
                SetFeature(parent, child, PluginConstants.FEATURE_PIP);
                SetFeature(parent, child, PluginConstants.FEATURE_BDLive);

                child.SetOtherFeatures(parent.GetOtherFeatures());
            }
            if (this.FeaturesLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Studios, true);
            }
            #endregion
            #region Audio Tracks
            if (this.AudioTracksCheckBox.Checked)
            {
                for (Int32 a = 1; a < 17; a++)
                {
                    Int32 contentId;
                    Int32 formatId;
                    Int32 channelId;

                    parent.GetAudioTrack(a, out contentId, out formatId, out channelId);
                    child.SetAudioTrack(a, contentId, formatId, channelId);
                }
            }
            if (this.AudioTracksLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_AudioTracks, true);
            }
            #endregion
            #region Subtitles
            if (this.SubtitlesCheckBox.Checked)
            {
                child.ClearSubtitles();
                for (Int32 s = 0; s < parent.GetSubtitleCount(); s++)
                {
                    child.AddSubtitle(parent.GetSubtitleByIndex(s));
                }
            }
            if (this.SubtitlesLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Subtitles, true);
            }
            #endregion
            #region Cast
            if (this.CastCheckBox.Checked)
            {
                child.ClearCast();
                for (Int32 c = 0; c < parent.GetCastCount(); c++)
                {
                    String firstName;
                    String middleName;
                    String lastName;
                    Int32 birthYear;
                    String part;
                    String creditedAs;
                    Boolean voice;
                    Boolean uncredited;

                    parent.GetCastByIndex(c, out firstName, out middleName, out lastName, out birthYear, out part
                        , out creditedAs, out voice, out uncredited);
                    if (firstName == null && lastName == null)
                    {
                        String caption;
                        Int32 dividerType;

                        parent.GetCastDividerByIndex(c, out caption, out dividerType);
                        child.AddCastDivider(caption, dividerType);
                    }
                    else
                    {
                        child.AddCast(firstName, middleName, lastName, birthYear, part, creditedAs, voice, uncredited);
                    }
                }
            }
            if (this.CastLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Cast, true);
            }
            #endregion
            #region Crew
            if (this.CrewCheckBox.Checked)
            {
                child.ClearCrew();
                for (Int32 c = 0; c < parent.GetCrewCount(); c++)
                {
                    String firstName;
                    String middleName;
                    String lastName;
                    Int32 birthYear;
                    Int32 creditType;
                    Int32 creditSubtype;
                    String creditedAs;

                    parent.GetCrewByIndex(c, out firstName, out middleName, out lastName, out birthYear, out creditType
                        , out creditSubtype, out creditedAs);
                    if (firstName == null && lastName == null)
                    {
                        String caption;
                        Int32 dividerType;

                        parent.GetCrewDividerByIndex(c, out caption, out dividerType, out creditType);
                        child.AddCrewDivider(caption, dividerType, creditType);
                    }
                    else
                    {
                        child.AddCrew(firstName, middleName, lastName, birthYear, creditType, creditSubtype, creditedAs);
                    }
                }
            }
            if (this.CrewLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Crew, true);
            }
            #endregion
            #region Overview
            if (this.OverviewCheckBox.Checked)
            {
                child.SetOverview(parent.GetOverview());
            }
            if (this.OverviewLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Overview, true);
            }
            #endregion
            #region Easter Eggs
            if (this.EasterEggsCheckBox.Checked)
            {
                child.SetEasterEggs(parent.GetEasterEggs());
            }
            if (this.EasterEggsLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_EasterEggs, true);
            }
            #endregion
            #region Discs
            if (this.DiscsCheckBox.Checked)
            {
                child.ClearDiscs();
                for (Int32 d = 0; d < parent.GetDiscCount(); d++)
                {
                    String descriptionSideA;
                    String descriptionSideB;
                    String labelSideA;
                    String labelSideB;
                    String discIdSideA;
                    String discIdSideB;
                    Boolean dualLayeredSideA;
                    Boolean dualLayeredSideB;
                    String location;
                    String slot;

                    parent.GetDiscByIndex(d, out descriptionSideA, out descriptionSideB, out labelSideA, out labelSideB
                        , out discIdSideA, out discIdSideB, out dualLayeredSideA, out dualLayeredSideB, out location, out slot);
                    child.AddDisc(descriptionSideA, descriptionSideB, labelSideA, labelSideB
                        , discIdSideA, discIdSideB, dualLayeredSideA, dualLayeredSideB, location, slot);
                }
            }
            if (this.DiscsLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_DiscInformation, true);
            }
            #endregion
            #region Cover Images
            if (this.FrontCoverImagesCheckBox.Checked)
            {
                String source;

                source = parent.GetCoverImageFilename(true, false);
                CopyCover(parent, child, source);
                source = parent.GetCoverImageFilename(true, true);
                CopyCover(parent, child, source);
            }
            if (this.BackCoverImagesCheckBox.Checked)
            {
                String source;

                source = parent.GetCoverImageFilename(false, false);
                CopyCover(parent, child, source);
                source = parent.GetCoverImageFilename(false, true);
                CopyCover(parent, child, source);
            }
            if (this.CoverImagesLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Scans, true);
            }
            #endregion
            #region Entire DVD Lock
            if (this.EntireDvdLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Entire, true);
            }
            #endregion
            #region Purchase Date
            if (this.PurchaseDateCheckBox.Checked)
            {
                child.SetPurchaseDate(parent.GetPurchaseDate());
            }
            #endregion
            #region Purchase Place
            if (this.PurchasePlaceCheckBox.Checked)
            {
                child.SetPurchasePlace(parent.GetPurchasePlace());
                child.SetPurchasePlaceIsOnline(parent.GetPurchasePlaceIsOnline());
                child.SetPurchasePlaceWebsite(parent.GetPurchasePlaceWebsite());
            }
            #endregion
            #region Purchase Price
            if (this.PurchasePriceCheckBox.Checked)
            {
                if (parent.GetPurchasePriceIsEmpty())
                {
                    child.ClearPurchasePrice();
                }
                else
                {
                    child.SetPurchasePriceValue(parent.GetPurchasePriceValue());
                }
            }
            #endregion
            #region Currency
            if (this.CurrencyCheckBox.Checked)
            {
                child.SetPurchasePriceCurrency(parent.GetPurchasePriceCurrency());
            }
            #endregion
            #region Collection Number
            if (this.CollectionNumberCheckBox.Checked)
            {
                child.SetCollectionNumber(parent.GetCollectionNumber());
            }
            #endregion
            #region Count As
            if (this.CountAsCheckBox.Checked)
            {
                child.SetCountAs(parent.GetCountAs());
            }
            #endregion
            #region Tags
            if (this.TagsCheckBox.Checked)
            {
                child.ClearTags();
                for (Int32 t = 0; t < parent.GetTagCount(); t++)
                {
                    child.AddTag(parent.GetTagByIndex(t));
                }
            }
            #endregion
        }

        private static void SetFeature(IDVDInfo parent, IDVDInfo child, Int32 feature)
        {
            child.SetFeatureByID(feature, parent.GetFeatureByID(feature));
        }

        private static void CopyCover(IDVDInfo parent, IDVDInfo child, String source)
        {
            FileInfo fi;

            if (String.IsNullOrEmpty(source) == false)
            {
                fi = new FileInfo(source);
                if (fi.Exists)
                {
                    String target;

                    target = fi.FullName.Replace(parent.GetProfileID(), child.GetProfileID());
                    target = target.Replace(".JPG", ".jpg");
                    target = target.Replace("F.jpg", "f.jpg");
                    target = target.Replace("B.jpg", "b.jpg");
                    File.Copy(fi.FullName, target, true);
                }
            }
        }

        private void CheckForNewVersion()
        {
            OnlineAccess.Init("Doena Soft.", "InitializeTVShowChildProfiles");
            OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.0/versions.xml", this, "InitializeTVShowChildProfiles", this.GetType().Assembly);
        }

        private void LayoutForm()
        {
            if (Plugin.Settings.MainForm.WindowState == FormWindowState.Normal)
            {
                this.Left = Plugin.Settings.MainForm.Left;
                this.Top = Plugin.Settings.MainForm.Top;
                if (Plugin.Settings.MainForm.Width > this.MinimumSize.Width)
                {
                    this.Width = Plugin.Settings.MainForm.Width;
                }
                else
                {
                    this.Width = this.MinimumSize.Width;
                }
                if (Plugin.Settings.MainForm.Height > this.MinimumSize.Height)
                {
                    this.Height = Plugin.Settings.MainForm.Height;
                }
                else
                {
                    this.Height = this.MinimumSize.Height;
                }
            }
            else
            {
                this.Left = Plugin.Settings.MainForm.RestoreBounds.X;
                this.Top = Plugin.Settings.MainForm.RestoreBounds.Y;
                if (Plugin.Settings.MainForm.RestoreBounds.Width > this.MinimumSize.Width)
                {
                    this.Width = Plugin.Settings.MainForm.RestoreBounds.Width;
                }
                else
                {
                    this.Width = this.MinimumSize.Width;
                }
                if (Plugin.Settings.MainForm.RestoreBounds.Height > this.MinimumSize.Height)
                {
                    this.Height = Plugin.Settings.MainForm.RestoreBounds.Height;
                }
                else
                {
                    this.Height = this.MinimumSize.Height;
                }
            }
            if (Plugin.Settings.MainForm.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = Plugin.Settings.MainForm.WindowState;
            }
            this.ReadDefaultValues();
        }

        private void WriteDefaultValues()
        {
            Type defaultValuesType;
            FieldInfo[] defaultFields;
            Type mainFormType;

            defaultValuesType = typeof(DefaultValues);
            mainFormType = this.GetType();
            defaultFields = defaultValuesType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo defaultFieldInfo in defaultFields)
            {
                FieldInfo fieldInfo;
                CheckBox checkBox;

                fieldInfo = mainFormType.GetField(defaultFieldInfo.Name + "CheckBox"
                    , BindingFlags.NonPublic | BindingFlags.Instance);
                checkBox = (CheckBox)(fieldInfo.GetValue(this));
                defaultFieldInfo.SetValue(Plugin.Settings.DefaultValues, checkBox.Checked);
            }
        }

        private void ReadDefaultValues()
        {
            Type defaultValuesType;
            FieldInfo[] defaultFields;
            Type mainFormType;

            defaultValuesType = typeof(DefaultValues);
            mainFormType = this.GetType();
            defaultFields = defaultValuesType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo defaultFieldInfo in defaultFields)
            {
                FieldInfo fieldInfo;
                CheckBox checkBox;

                fieldInfo = mainFormType.GetField(defaultFieldInfo.Name + "CheckBox"
                    , BindingFlags.NonPublic | BindingFlags.Instance);
                checkBox = (CheckBox)(fieldInfo.GetValue(this));
                checkBox.Checked = (Boolean)(defaultFieldInfo.GetValue(Plugin.Settings.DefaultValues));
            }
        }
        #endregion
    }
}