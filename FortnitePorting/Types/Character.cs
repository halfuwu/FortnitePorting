using System;
using System.IO;
using CUE4Parse.UE4.Assets.Exports;
using FortnitePorting.Utils;
using Newtonsoft.Json;
using static FortnitePorting.Program;
using static FortnitePorting.Utils.Structures;

namespace FortnitePorting.Types
{
    public static class Character
    {
        public static void ProcessCharacter(string input)
        {
            var ProcessedFile = new Processed();
            var path = $"FortniteGame/Content/Athena/Items/Cosmetics/Characters/{input}.{input}";
            if (!input.StartsWith("CID_"))
            {
                path = BenbotApi.GetCosmeticPath(BenbotApi.EBackendType.AthenaCharacter, input);
            }

            if (Provider.TryLoadObject(path, out var CharacterItemDefinition))
            {
                var CharacterParts = CharacterItemDefinition.Get<UObject>("HeroDefinition")
                    .Get<UObject[]>("Specializations")[0]
                    .Get<UObject[]>("CharacterParts");

                if (CharacterParts.Length > 0)
                {
                    AssetUtils.FillCharacterParts(CharacterParts, ProcessedFile.DefaultVariant);
                }

                if (CharacterItemDefinition.TryGetValue(out UObject[] ItemVariants, "ItemVariants"))
                {
                    AssetUtils.FillVariants(ItemVariants, ProcessedFile);
                }

                var json = JsonConvert.SerializeObject(ProcessedFile, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });
                Directory.CreateDirectory(ProcessedPath);
                File.WriteAllText(Path.Combine(ProcessedPath, CharacterItemDefinition.Name + ".json"), json);
            }
            else
            {
                Logger.Log("Failed to Load Character...", SimpleLogger.ELogLevel.Critical);
            }
        }   
    }
}