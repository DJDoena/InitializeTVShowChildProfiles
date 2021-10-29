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

        private readonly IDVDProfilerAPI _api;

        #endregion

        #region Constructor

        public MainForm(IDVDProfilerAPI api)
        {
            _api = api;

            this.InitializeComponent();
        }

        #endregion

        #region Form Events

        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var aboutBox = new AboutBox(this.GetType().Assembly))
            {
                aboutBox.ShowDialog();
            }
        }

        private void OnCheckForUpdatesToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.CheckForNewVersion();
        }

        private void OnMainFormLoad(object sender, EventArgs e)
        {
            this.SuspendLayout();
            this.LayoutForm();
            this.ResumeLayout();

            if (Plugin.Settings.CurrentVersion != this.GetType().Assembly.GetName().Version.ToString())
            {
                Plugin.Settings.CurrentVersion = this.GetType().Assembly.GetName().Version.ToString();
            }
        }

        private void OnMainFormClosing(object sender, FormClosingEventArgs e)
        {
            this.WriteDefaultValues();
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            var parent = _api.GetDisplayedDVD();

            if (parent.GetProfileID() != null)
            {
                _api.DVDByProfileID(out parent, parent.GetProfileID(), -1, -1);

                for (int i = 0; i < parent.GetBoxSetContentCount(); i++)
                {
                    _api.DVDByProfileID(out var child, parent.GetBoxSetContentByIndex(i), -1, -1);

                    this.CopyFromParent(parent, child);

                    _api.SaveDVDToCollection(child);

                    _api.UpdateProfileInListDisplay(child.GetProfileID());
                }

                _api.RequeryDatabase();
            }

            this.Close();
        }

        private void OnAbortButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnEntireDvdLockCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            SetLockOnGroupBox.Enabled = (EntireDvdLockCheckBox.Checked == false);
        }

        #endregion

        #region Helper Functions
        private void CopyFromParent(IDVDInfo parent, IDVDInfo child)
        {
            #region Media Types

            if (MediaTypesCheckBox.Checked)
            {
                parent.GetMediaTypes(out var dvd, out var hddvd, out var bluRay, out var ultraHD);
                parent.GetCustomMediaType(out var custom);

                child.SetMediaTypes(dvd, hddvd, bluRay, ultraHD);

                if (string.IsNullOrEmpty(custom) == false)
                {
                    child.SetCustomMediaType(custom);
                }
                else
                {
                    child.SetCustomMediaType(null);
                }
            }

            if (MediaTypesLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_MediaTypes, true);
            }

            #endregion

            #region Edition

            if (EditionCheckBox.Checked)
            {
                child.SetEdition(parent.GetEdition());
            }

            #endregion

            #region Title

            if (OriginalTitleCheckBox.Checked)
            {
                child.SetOriginalTitle(parent.GetOriginalTitle());
            }
            if (TitleLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Title, true);
            }

            #endregion

            #region Production Year

            if (ProductionYearCheckBox.Checked)
            {
                child.SetProductionYear(parent.GetProductionYear());
            }

            if (ProductionYearLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_ProductionYear, true);
            }

            #endregion

            #region Country of Origin

            if (CountryOfOriginCheckBox.Checked)
            {
                for (var c = 1; c < 4; c++)
                {
                    child.SetCountryOfOrigin(c, parent.GetCountryOfOrigin(c));
                }
            }

            if (CountryOfOriginLockCheckBox.Checked)
            {
                //
            }

            #endregion

            #region Rating

            if (RatingCheckBox.Checked)
            {
                parent.GetRating(out var ratingSystem, out var ratingAge, out var ratingVariant);

                child.SetRating(ratingSystem, ratingAge, ratingVariant);
                child.SetRatingDescription(parent.GetRatingDescription());
            }

            if (RatingLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Rating, true);
            }

            #endregion

            #region Regions

            if (RegionsCheckBox.Checked)
            {
                for (var r = 0; r < 7; r++)
                {
                    child.SetRegionByID(r, parent.GetRegionByID(r));
                }
            }

            if (RegionsLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Regions, true);
            }

            #endregion

            #region Release Date

            if (ReleaseDateCheckBox.Checked)
            {
                child.SetDVDReleaseDate(parent.GetDVDReleaseDate());
            }

            if (ReleaseDateLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_ReleaseDate, true);
            }

            #endregion

            #region Case Type

            if (CaseTypeCheckBox.Checked)
            {
                child.SetCaseID(parent.GetCaseID());
                child.SetCaseSlipCover(parent.GetCaseSlipCover());
            }

            if (CaseTypeLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_CaseType, true);
            }

            #endregion

            #region SRP

            if (SrpCheckBox.Checked)
            {
                child.SetSRPValue(0);

                child.SetSRPCurrency(parent.GetSRPCurrency());
            }

            if (SrpLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_PurchasePrice, true);
            }

            #endregion

            #region Genres

            if (GenresCheckBox.Checked)
            {
                for (var g = 1; g < 6; g++)
                {
                    child.SetGenre(g, parent.GetGenre(g));
                }
            }

            if (GenresLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Genres, true);
            }

            #endregion

            #region Video Formats

            if (VideoFormatsCheckBox.Checked)
            {
                parent.GetDimensions(out var twoDimensional, out var anaglyph, out var threeDimensional);

                child.SetAspectRatio(parent.GetAspectRatio());
                child.SetFormatAnamorphic(parent.GetFormatAnamorphic());
                child.SetFormatFullFrame(parent.GetFormatFullFrame());
                child.SetFormatPanScan(parent.GetFormatPanScan());
                child.SetFormatWidescreen(parent.GetFormatWidescreen());
                child.SetVideoStandard(parent.GetVideoStandard());
                child.SetColorType(parent.GetColorType());
                child.SetDimensions(twoDimensional, anaglyph, threeDimensional);
            }

            if (VideoFormatsLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_VideoFormats, true);
            }

            #endregion

            #region Studios

            if (StudiosCheckBox.Checked)
            {
                for (var s = 1; s < 4; s++)
                {
                    child.SetStudio(s, parent.GetStudio(s));
                }
            }

            if (StudiosLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Studios, true);
            }

            #endregion

            #region Media Companies

            if (MediaCompaniesCheckBox.Checked)
            {
                for (var mc = 1; mc < 4; mc++)
                {
                    child.SetMediaCompany(mc, parent.GetMediaCompany(mc));
                }
            }

            if (MediaCompaniesLockCheckBox.Checked)
            {
                //
            }

            #endregion

            #region Features

            if (FeaturesCheckBox.Checked)
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

                SetFeature(parent, child, PluginConstants.FEATURE_CineChat);
                SetFeature(parent, child, PluginConstants.FEATURE_DBOX);
                SetFeature(parent, child, PluginConstants.FEATURE_MovieIQ);
                SetFeature(parent, child, PluginConstants.FEATURE_PlayAll);

                child.SetOtherFeatures(parent.GetOtherFeatures());
            }

            if (FeaturesLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Studios, true);
            }

            #endregion

            #region Audio Tracks

            if (AudioTracksCheckBox.Checked)
            {
                for (var a = 1; a < 17; a++)
                {
                    parent.GetAudioTrack(a, out var contentId, out var formatId, out var channelId);

                    child.SetAudioTrack(a, contentId, formatId, channelId);
                }
            }

            if (AudioTracksLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_AudioTracks, true);
            }

            #endregion

            #region Subtitles

            if (SubtitlesCheckBox.Checked)
            {
                child.ClearSubtitles();

                for (int s = 0; s < parent.GetSubtitleCount(); s++)
                {
                    child.AddSubtitle(parent.GetSubtitleByIndex(s));
                }
            }

            if (SubtitlesLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Subtitles, true);
            }

            #endregion

            #region Cast

            if (CastCheckBox.Checked)
            {
                child.ClearCast();

                for (var c = 0; c < parent.GetCastCount(); c++)
                {
                    parent.GetCastByIndex(c, out var firstName, out var middleName, out var lastName, out var birthYear, out var part, out var creditedAs
                        , out var voice, out var uncredited, out var puppeteer);

                    if (firstName == null && lastName == null)
                    {
                        parent.GetCastDividerByIndex(c, out var caption, out var dividerType);

                        child.AddCastDivider(caption, dividerType);
                    }
                    else
                    {
                        child.AddCast(firstName, middleName, lastName, birthYear, part, creditedAs, voice, uncredited, puppeteer);
                    }
                }
            }

            if (CastLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Cast, true);
            }

            #endregion

            #region Crew

            if (CrewCheckBox.Checked)
            {
                child.ClearCrew();

                for (var c = 0; c < parent.GetCrewCount(); c++)
                {
                    parent.GetCrewByIndex(c, out var firstName, out var middleName, out var lastName, out var birthYear, out var creditType, out var creditSubtype, out var creditedAs);

                    if (firstName == null && lastName == null)
                    {
                        parent.GetCrewDividerByIndex(c, out var caption, out var dividerType, out creditType);

                        child.AddCrewDivider(caption, dividerType, creditType);
                    }
                    else
                    {
                        child.AddCrew(firstName, middleName, lastName, birthYear, creditType, creditSubtype, creditedAs);
                    }
                }
            }

            if (CrewLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Crew, true);
            }

            #endregion

            #region Overview

            if (OverviewCheckBox.Checked)
            {
                child.SetOverview(parent.GetOverview());
            }
            if (OverviewLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Overview, true);
            }

            #endregion

            #region Easter Eggs

            if (EasterEggsCheckBox.Checked)
            {
                child.SetEasterEggs(parent.GetEasterEggs());
            }

            if (EasterEggsLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_EasterEggs, true);
            }

            #endregion

            #region Discs

            if (DiscsCheckBox.Checked)
            {
                child.ClearDiscs();

                for (var d = 0; d < parent.GetDiscCount(); d++)
                {
                    parent.GetDiscByIndex(d, out var descriptionSideA, out var descriptionSideB, out var labelSideA, out var labelSideB
                        , out var discIdSideA, out var discIdSideB, out var dualLayeredSideA, out var dualLayeredSideB, out var location, out var slot);

                    child.AddDisc(descriptionSideA, descriptionSideB, labelSideA, labelSideB
                        , discIdSideA, discIdSideB, dualLayeredSideA, dualLayeredSideB, location, slot);
                }
            }

            if (DiscsLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_DiscInformation, true);
            }

            #endregion

            #region Cover Images

            if (FrontCoverImagesCheckBox.Checked)
            {
                var source = parent.GetCoverImageFilename(true, false);

                CopyCover(parent, child, source);

                source = parent.GetCoverImageFilename(true, true);

                CopyCover(parent, child, source);
            }

            if (BackCoverImagesCheckBox.Checked)
            {
                var source = parent.GetCoverImageFilename(false, false);

                CopyCover(parent, child, source);

                source = parent.GetCoverImageFilename(false, true);

                CopyCover(parent, child, source);
            }

            if (CoverImagesLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Scans, true);
            }

            #endregion

            #region Entire DVD Lock

            if (EntireDvdLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_Entire, true);
            }

            #endregion

            #region Purchase Date

            if (PurchaseDateCheckBox.Checked)
            {
                child.SetPurchaseDate(parent.GetPurchaseDate());
            }

            #endregion

            #region Purchase Place

            if (PurchasePlaceCheckBox.Checked)
            {
                child.SetPurchasePlace(parent.GetPurchasePlace());
                child.SetPurchasePlaceIsOnline(parent.GetPurchasePlaceIsOnline());
                child.SetPurchasePlaceWebsite(parent.GetPurchasePlaceWebsite());
            }

            #endregion

            #region Purchase Price

            if (PurchasePriceCheckBox.Checked)
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

            if (CurrencyCheckBox.Checked)
            {
                child.SetPurchasePriceCurrency(parent.GetPurchasePriceCurrency());
            }

            #endregion

            #region Collection Number

            if (CollectionNumberCheckBox.Checked)
            {
                child.SetCollectionNumber(parent.GetCollectionNumber());
            }

            #endregion

            #region Count As

            if (CountAsCheckBox.Checked)
            {
                child.SetCountAs(parent.GetCountAs());
            }

            #endregion

            #region Tags

            if (TagsCheckBox.Checked)
            {
                child.ClearTags();

                for (var t = 0; t < parent.GetTagCount(); t++)
                {
                    child.AddTag(parent.GetTagByIndex(t));
                }
            }

            #endregion

            #region BoxSet Contents

            if (BoxSetContentLockCheckBox.Checked)
            {
                child.SetLockByID(PluginConstants.LOCK_BoxSetContents, true);
            }

            #endregion
        }

        private static void SetFeature(IDVDInfo parent, IDVDInfo child, int feature)
        {
            child.SetFeatureByID(feature, parent.GetFeatureByID(feature));
        }

        private static void CopyCover(IDVDInfo parent, IDVDInfo child, string source)
        {
            if (string.IsNullOrEmpty(source) == false)
            {
                var fi = new FileInfo(source);

                if (fi.Exists)
                {
                    var target = fi.FullName.Replace(parent.GetProfileID(), child.GetProfileID());

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
            var defaultValuesType = typeof(DefaultValues);
            var mainFormType = this.GetType();
            var defaultFields = defaultValuesType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var defaultFieldInfo in defaultFields)
            {
                var fieldInfo = mainFormType.GetField(defaultFieldInfo.Name + "CheckBox", BindingFlags.NonPublic | BindingFlags.Instance);

                var checkBox = (CheckBox)fieldInfo.GetValue(this);

                defaultFieldInfo.SetValue(Plugin.Settings.DefaultValues, checkBox.Checked);
            }
        }

        private void ReadDefaultValues()
        {
            var defaultValuesType = typeof(DefaultValues);

            var mainFormType = this.GetType();

            var defaultFields = defaultValuesType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var defaultFieldInfo in defaultFields)
            {
                var fieldInfo = mainFormType.GetField(defaultFieldInfo.Name + "CheckBox", BindingFlags.NonPublic | BindingFlags.Instance);

                var checkBox = (CheckBox)fieldInfo.GetValue(this);

                checkBox.Checked = (bool)defaultFieldInfo.GetValue(Plugin.Settings.DefaultValues);
            }
        }
        #endregion
    }
}