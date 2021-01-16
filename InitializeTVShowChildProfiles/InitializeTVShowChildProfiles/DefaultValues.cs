using System ;
using System.Runtime.InteropServices ;
using System.Globalization;

namespace DoenaSoft.DVDProfiler.InitializeTVShowChildProfiles
{
    [ComVisible(false)]
    [Serializable()]
    public class DefaultValues
    {
        public Boolean MediaTypes = true;
        public Boolean MediaTypesLock = true;
        public Boolean Edition = false;
        public Boolean OriginalTitle = false;
        public Boolean ProductionYear = true;
        public Boolean ProductionYearLock = false;
        public Boolean CountryOfOrigin = true;
        public Boolean CountryOfOriginLock = false;
        public Boolean Rating = true;
        public Boolean RatingLock = false;
        public Boolean Regions = true;
        public Boolean RegionsLock = false;
        public Boolean ReleaseDate = true;
        public Boolean ReleaseDateLock = false;
        public Boolean CaseType = true;
        public Boolean CaseTypeLock = false;
        public Boolean Srp = true;
        public Boolean SrpLock = true;
        public Boolean Genres = true;
        public Boolean GenresLock = true;
        public Boolean VideoFormats = true;
        public Boolean VideoFormatsLock = false;
        public Boolean Studios = true;
        public Boolean StudiosLock = false;
        public Boolean MediaCompanies = true;
        public Boolean MediaCompaniesLock = false;
        public Boolean Features = false;
        public Boolean FeaturesLock = false;
        public Boolean AudioTracks = true;
        public Boolean AudioTracksLock = false;
        public Boolean Subtitles = true;
        public Boolean SubtitlesLock = false;
        public Boolean Cast = false;
        public Boolean CastLock = false;
        public Boolean Crew = false;
        public Boolean CrewLock = false;
        public Boolean Overview = false;
        public Boolean OverviewLock = false;
        public Boolean EasterEggs = false;
        public Boolean EasterEggsLock = false;
        public Boolean Discs = false;
        public Boolean DiscsLock = false;
        public Boolean FrontCoverImages = false;
        public Boolean BackCoverImages = false;
        public Boolean CoverImagesLock = false;
        public Boolean EntireDvdLock = false;
        public Boolean PurchaseDate = true;
        public Boolean PurchasePlace = true;
        public Boolean PurchasePrice = false;
        public Boolean Currency = true;
        public Boolean CollectionNumber = false;
        public Boolean CountAs = false;
        public Boolean Tags = false;
    }
}