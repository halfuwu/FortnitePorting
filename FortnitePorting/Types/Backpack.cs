using System;
using System.IO;
using CUE4Parse.UE4.Assets.Exports;
using FortnitePorting.Utils;
using Newtonsoft.Json;
using static FortnitePorting.Program;
using static FortnitePorting.Utils.Structures;

namespace FortnitePorting.Types
{
    public static class Backpack
    {
        public static void ProcessBackpack(string input)
        {
            var ProcessedFile = new Processed();
            var path = $"FortniteGame/Content/Athena/Items/Cosmetics/Backpacks/{input}.{input}";
            if (!input.StartsWith("BID_"))
            {
                path = BenbotApi.GetCosmeticPath(BenbotApi.EBackendType.AthenaBackpack, input);
            }
            if (Provider.TryLoadObject(path, out var ItemDefinition))
            {
                AssetUtils.FillCharacterParts(ItemDefinition.GetOrDefault<UObject[]>("CharacterParts", Array.Empty<UObject>()), ProcessedFile.DefaultVariant);
                AssetUtils.FillVariants(ItemDefinition.GetOrDefault<UObject[]>("ItemVariants", Array.Empty<UObject>()), ProcessedFile);

                var json = JsonConvert.SerializeObject(ProcessedFile, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });
                Directory.CreateDirectory(ProcessedPath);
                File.WriteAllText(Path.Combine(ProcessedPath, ItemDefinition.Name + ".json"), json);
            }
            
        }
    }
}