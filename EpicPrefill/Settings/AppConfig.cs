namespace EpicPrefill.Settings
{
    public static class AppConfig
    {
        static AppConfig()
        {
            // Create required folders
            Directory.CreateDirectory(ConfigDir);
            Directory.CreateDirectory(CacheDir);
        }
        

        /// <summary>
        /// Downloaded manifests, as well as other metadata, are saved into this directory to speedup future prefill runs.
        /// All data in here should be able to be deleted safely.
        /// </summary>
        public static readonly string CacheDir = GetCacheDirBaseDirectories();

        /// <summary>
        /// Increment when there is a breaking change made to the files in the cache directory
        /// </summary>
        private const string CacheDirVersion = "v1";

        /// <summary>
        /// Contains user configuration.  Should not be deleted, doing so will reset the app back to defaults.
        /// </summary>
        public static readonly string ConfigDir = Path.Combine(AppContext.BaseDirectory, "Config");

        //TODO comment
        public static int MaxConcurrentRequests => 30;

        //TODO comment
        private static bool _verboseLogs;
        public static bool VerboseLogs
        {
            get => _verboseLogs;
            set
            {
                _verboseLogs = value;
                AnsiConsoleExtensions.WriteVerboseLogs = value;
            }
        }

        #region Timeouts

        //TODO comment
        public static TimeSpan DefaultRequestTimeout => TimeSpan.FromSeconds(60);

        #endregion

        #region Serialization file paths

        public static readonly string AccountSettingsStorePath = Path.Combine(ConfigDir, "userAccount.json");

        public static readonly string UserSelectedAppsPath = Path.Combine(ConfigDir, "selectedAppsToPrefill.json");

        /// <summary>
        /// Keeps track of which apps + versions have been previously downloaded.  Is used to determine whether or not a game is up to date.
        /// </summary>
        public static readonly string SuccessfullyDownloadedAppsPath = Path.Combine(ConfigDir, "successfullyDownloadedApps.json");

        #endregion

#if DEBUG

        public static bool SkipDownloads { get; set; }

#endif

        //TODO move to lancacheprefill.common
        /// <summary>
        /// Gets the base directories for the cache folder, determined by which Operating System the app is currently running on.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static string GetCacheDirBaseDirectories()
        {
            if (System.OperatingSystem.IsWindows())
            {
                string pathAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(pathAppData, "EpicPrefill", "Cache", CacheDirVersion);
            }
            if (System.OperatingSystem.IsLinux())
            {
                // Gets base directories for the XDG Base Directory specification (Linux)
                string pathHome = Environment.GetEnvironmentVariable("HOME")
                                  ?? throw new ArgumentNullException("HOME", "Could not determine HOME directory");

                string pathXdgCacheHome = Environment.GetEnvironmentVariable("XDG_CACHE_HOME")
                                          ?? Path.Combine(pathHome, ".cache");

                return Path.Combine(pathXdgCacheHome, "EpicPrefill", CacheDirVersion);
            }
            if (System.OperatingSystem.IsMacOS())
            {
                string pathLibraryCaches = Path.GetFullPath("~/Library/Caches");
                return Path.Combine(pathLibraryCaches, "EpicPrefill", CacheDirVersion);
            }

            throw new NotSupportedException($"Unknown platform {RuntimeInformation.OSDescription}");
        }
    }
}