using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Mongo2Go.Helper
{

    public class MongoBinaryLocator : IMongoBinaryLocator
    {
        public const string DefaultWindowsSearchPattern = @"**\mongodb-win32*\bin";
        public const string DefaultLinuxSearchPattern = "*/tools/mongodb-linux*/bin";
        public const string DefaultOsxSearchPattern = "*/tools/mongodb-osx*/bin";
        public static readonly string OsxAndLinuxNugetCacheLocation = Environment.GetEnvironmentVariable("HOME") + "/.nuget/packages/apiiro.mongo2go/";
        private string _binFolder = string.Empty;
        private readonly string _searchPattern;
        private readonly string _additionalSearchDirectory;

        public MongoBinaryLocator(string searchPatternOverride, string additionalSearchDirectory)
        {
            _additionalSearchDirectory = additionalSearchDirectory;
            if (string.IsNullOrEmpty(searchPatternOverride))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    _searchPattern = DefaultOsxSearchPattern;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    _searchPattern = DefaultLinuxSearchPattern;
                }
                else
                {
                    throw new MonogDbBinariesNotFoundException($"Unknown OS: {RuntimeInformation.OSDescription}");
                }
            }
            else
            {
                _searchPattern = searchPatternOverride;
            }
        }

        public string Directory {
            get {
                if (string.IsNullOrEmpty(_binFolder)){
                    return _binFolder = ResolveBinariesDirectory ();
                } else {
                    return _binFolder;
                }
            }
        }

        private string ResolveBinariesDirectory()
        {
            return FindBinariesDirectory();
        }

        private string FindBinariesDirectory()
        {
            var binaryFolder = OsxAndLinuxNugetCacheLocation.FindFolderUpwards(_searchPattern);
            if (binaryFolder != null) return binaryFolder;
            throw new MonogDbBinariesNotFoundException(
                $"Could not find Mongo binaries using the search patterns \"{Path.Combine(OsxAndLinuxNugetCacheLocation, _searchPattern)}\".  " +
                $"You can override the search pattern and directory when calling MongoDbRunner.Start.  We have detected the OS as {RuntimeInformation.OSDescription}.\n"
            );
        }
    }
}