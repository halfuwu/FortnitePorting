using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using FortnitePorting.Types;
using FortnitePorting.Utils;
using Newtonsoft.Json;
using Serilog;
using static FortnitePorting.Utils.SimpleLogger;

namespace FortnitePorting
{
    static class Program
    {
        public static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public static readonly string MappingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mappings");
        public static readonly string ProcessedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "processed");
        public static readonly SimpleLogger Logger = new ("FortnitePorting");
        public static DefaultFileProvider Provider;
        
        private static Config _config;
        
        public static void Main(string[] args)
        {
            if (args.Length == 0) Logger.Log("No program arguments found!", ELogLevel.Critical);
            
            if (args[0] == "-fill")
            {
                BenbotApi.UpdateKeys();
                BenbotApi.UpdateMappings();
                PromptExit(0);
            }
            Log.Logger = Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            LoadConfig();
            LoadProvider();

            var input = args[1].Trim();
            switch (args[0].ToLower())
            {
                case "-c": 
                case "-character":
                    Character.ProcessCharacter(input);
                    break;
                case "-b": 
                case "-backpack":
                    Backpack.ProcessBackpack(input);
                    break;
            }
        }

        static void LoadConfig()
        {
            if (!File.Exists(ConfigPath)) 
            {
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(new Config()));
                Logger.Log($"Config file does not exist at {ConfigPath}", ELogLevel.Critical);
            }

            Logger.Log($"Reading Config File {ConfigPath}");
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));

            if (!Directory.Exists(_config.PaksDirectory)) Logger.Log($"Paks directory {_config.PaksDirectory} does not exist", ELogLevel.Critical);
        }
        static void LoadProvider()
        {
            Logger.Log($"Loading Game Files at {_config.PaksDirectory}");
            Provider = new DefaultFileProvider(_config.PaksDirectory, SearchOption.TopDirectoryOnly, true);
            Provider.Initialize();
            Provider.SubmitKey(new FGuid(), new FAesKey(_config.MainKey));

            var usmap = GetNewestUsmap("Mappings");
            if (usmap != String.Empty)
                Provider.MappingsContainer = new FileUsmapTypeMappingsProvider(usmap);
            else
                Provider.LoadMappings(); // BenbotMappingProvider

            foreach (var entry in _config.DynamicKeys)
            {
                var vfs = Provider.UnloadedVfs.FirstOrDefault(pak => pak.Name.Equals(entry.FileName));
                if (vfs != null) Provider.SubmitKey(vfs.EncryptionKeyGuid, new FAesKey(entry.Key));
                else Logger.Log($"Failed to Submit Key {entry.Key} for {entry.FileName}");
            }
        }

        public static void PromptExit(int code)
        {
            Console.WriteLine("Press any button to exit...");
            Console.ReadKey();
            Environment.Exit(code);
        }

        private static string GetNewestUsmap(string mappingsFolder)
        {
            if (!Directory.Exists(mappingsFolder))
                return String.Empty;

            var directory = new DirectoryInfo(mappingsFolder);
            var selectedFilePath = String.Empty;
            var modifiedTime = long.MinValue;
            foreach (var file in directory.GetFiles())
            {
                if (file.Name.EndsWith(".usmap") && file.LastWriteTime.ToFileTimeUtc() > modifiedTime)
                {
                    selectedFilePath = file.FullName;
                    modifiedTime = file.LastWriteTime.ToFileTimeUtc();
                }
            }

            return selectedFilePath;
        }
        public class Config
        {
            public string PaksDirectory { get; set; }
            public string MainKey { get; set; }
            public List<KeyEntry> DynamicKeys { get; set; }

            public class KeyEntry
            {
                public string FileName { get; set; }
                public string Key { get; set; }
            }
        }
    }
} 