using System;
using System.Runtime.InteropServices;

namespace DoenaSoft.DVDProfiler.InitializeTVShowChildProfiles
{
    [ComVisible(false)]
    [Serializable()]
    public class DefaultValues
    {
        public bool MediaTypes = true;
        public bool MediaTypesLock = true;
        public bool Edition = false;
        public bool OriginalTitle = false;
        public bool TitleLock = false;
        public bool ProductionYear = true;
        public bool ProductionYearLock = false;
        public bool CountryOfOrigin = true;
        public bool CountryOfOriginLock = false;
        public bool Rating = true;
        public bool RatingLock = false;
        public bool Regions = true;
        public bool RegionsLock = false;
        public bool ReleaseDate = true;
        public bool ReleaseDateLock = false;
        public bool CaseType = true;
        public bool CaseTypeLock = false;
        public bool Srp = true;
        public bool SrpLock = true;
        public bool Genres = true;
        public bool GenresLock = true;
        public bool VideoFormats = true;
        public bool VideoFormatsLock = false;
        public bool Studios = true;
        public bool StudiosLock = false;
        public bool MediaCompanies = true;
        public bool MediaCompaniesLock = false;
        public bool Features = false;
        public bool FeaturesLock = false;
        public bool AudioTracks = true;
        public bool AudioTracksLock = false;
        public bool Subtitles = true;
        public bool SubtitlesLock = false;
        public bool Cast = false;
        public bool CastLock = false;
        public bool Crew = false;
        public bool CrewLock = false;
        public bool Overview = false;
        public bool OverviewLock = false;
        public bool EasterEggs = false;
        public bool EasterEggsLock = false;
        public bool Discs = false;
        public bool DiscsLock = false;
        public bool FrontCoverImages = false;
        public bool BackCoverImages = false;
        public bool CoverImagesLock = false;
        public bool EntireDvdLock = false;
        public bool PurchaseDate = true;
        public bool PurchasePlace = true;
        public bool PurchasePrice = false;
        public bool Currency = true;
        public bool CollectionNumber = false;
        public bool CountAs = false;
        public bool Tags = false;
        public bool BoxSetContentLock = false;
    }
}