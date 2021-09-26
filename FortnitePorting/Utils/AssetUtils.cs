using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Animation;
using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.SkeletalMesh;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.Utils;
using CUE4Parse_Conversion.Animations;
using CUE4Parse_Conversion.Meshes;
using CUE4Parse_Conversion.Textures;
using static FortnitePorting.Utils.Structures;
using static FortnitePorting.Program;

namespace FortnitePorting.Utils
{
    public static class AssetUtils
    { 
        private static readonly DirectoryInfo SaveDirectory = new(AppDomain.CurrentDomain.BaseDirectory);
        public static void FillCharacterParts(UObject[] CharacterParts, List<CharacterPart> ProcessedCharacterParts) 
        {
            foreach (var CharacterPart in CharacterParts)
            {
                USkeletalMesh SkeletalMesh = CharacterPart.Get<USkeletalMesh>("SkeletalMesh");
                SkeletalMesh.ExportObject();
 
                CharacterPart.TryGetValue(out EFortCustomPartType CustomPartType, "CharacterPartType");
 
                FName AttachSocketName = default;
                if (CharacterPart.TryGetValue(out UObject AdditionalData, "AdditionalData"))
                {
                    AttachSocketName = AdditionalData.GetOrDefault("AttachSocketName", new FName(), StringComparison.OrdinalIgnoreCase);
                }

                var Materials = new List<Material>();
                if (SkeletalMesh.TryConvert(out var ConvertedMesh))
                {
                    foreach (var (Section, Idx) in ConvertedMesh.LODs[0].Sections.Value.Enumerate())
                    {
                        if (Section.Material == null) continue;
                        if (Section.Material.TryLoad(out UMaterialInstanceConstant MaterialInstance))
                        {
                            var Material = new Material
                            {
                                MaterialPath = MaterialInstance.GetPathName(),
                                MaterialOverrideIndex = Idx,
                                MaterialParameters = MaterialInstance.FillMaterialParams()
                            };
                            Materials.Add(Material);
                        }
                    }
                }

                if (CharacterPart.TryGetValue(out FStructFallback[] MaterialOverrides, "MaterialOverrides"))
                {
                    foreach (var MaterialOverride in MaterialOverrides)
                    {
                        var MaterialPath = MaterialOverride.Get<FSoftObjectPath>("OverrideMaterial");
                        var Idx = MaterialOverride.Get<int>("MaterialOverrideIndex");
                        if (MaterialPath.TryLoad(out UMaterialInstanceConstant MaterialInstance))
                        {
                            var Material = new Material
                            {
                                MaterialPath = MaterialInstance.GetPathName(),
                                MaterialOverrideIndex = Idx,
                                MaterialParameters = MaterialInstance.FillMaterialParams()
                            };
                            Materials.Add(Material);
                        }
                    }
                }
                    
                var ProcessedCharacterPart = new CharacterPart
                {
                    SkeletalMesh = SkeletalMesh.GetPathName(),
                    AttachSocketName = AttachSocketName.Text,
                    CharacterPartType = CustomPartType.ToString(),
                    Materials = Materials
                };
                ProcessedCharacterParts.Add(ProcessedCharacterPart);
            }
        }

        public static MaterialParameters FillMaterialParams(this UMaterialInstanceConstant MaterialInstance)
        {
            var Parameters = new MaterialParameters();

            foreach (var Param in MaterialInstance.TextureParameterValues)
            {
                if (Param.ParameterValue.TryLoad(out UTexture2D Texture))
                {
                    var Parameter = new TextureParameter
                    {
                        Info = Param.ParameterInfo.Name.Text,
                        Value = Texture.GetPathName()
                    };
                    Texture.ExportObject();
                    Parameters.TextureParameters.Add(Parameter);
                }
            }
            foreach (var Param in MaterialInstance.ScalarParameterValues)
            {
                var Parameter = new ScalarParameter
                {
                    Info = Param.ParameterInfo.Name.Text,
                    Value = Param.ParameterValue
                };
                Parameters.ScalarParameters.Add(Parameter);
            }
            foreach (var Param in MaterialInstance.VectorParameterValues)
            {
                if (Param.ParameterValue == null) continue;
                var Parameter = new VectorParameter
                {
                    Info = Param.ParameterInfo.Name.Text,
                    Value = Param.ParameterValue.Value.ToParamColor()
                };
                Parameters.VectorParameters.Add(Parameter);
            }
            if (MaterialInstance.TryGetValue(out FStructFallback StaticParameters, "StaticParameters"))
            {
                if (StaticParameters.TryGetValue(out FStructFallback[] StaticComponentMaskParameters, "StaticComponentMaskParameters"))
                {
                    foreach (var Param in StaticComponentMaskParameters)
                    {
                        var Parameter = new VectorParameter
                        {
                            Info =  Param.Get<FStructFallback>("ParameterInfo").Get<FName>("Name").Text,
                            Value = new VectorParameter.Color(
                                Param.Get<bool>("R") ? 1 : 0,
                                Param.Get<bool>("G") ? 1 : 0,
                                Param.Get<bool>("B") ? 1 : 0,
                                Param.Get<bool>("A") ? 1 : 0)
                        };
                        Parameters.ComponentMaskParameters.Add(Parameter);
                    }
                }
                if (StaticParameters.TryGetValue(out FStructFallback[] StaticSwitchParameters, "StaticSwitchParameters"))
                {
                    foreach (var Param in StaticSwitchParameters)
                    {
                        var StaticSwitch = new SwitchParameter
                        {
                            Info = Param.Get<FStructFallback>("ParameterInfo").Get<FName>("Name").Text,
                            Value = Param.Get<bool>("Value")
                        };
                        Parameters.SwitchParameters.Add(StaticSwitch);
                    }
                }
            }
            if (MaterialInstance.TryGetValue(out UObject SubsurfaceProfile, "SubsurfaceProfile"))
            {
                if (SubsurfaceProfile.TryGetValue(out FStructFallback Settings, "Settings"))
                {
                    Parameters.SubsurfaceColor = Settings.Get<FLinearColor>("SubsurfaceColor").ToParamColor();
                }
            }

            return Parameters;
        }
        private static string FixPath(string fullPath, string ext = null)
        {
            if (fullPath.StartsWith("/")) fullPath = fullPath[1..];
            var finalPath = Path.Combine(SaveDirectory.FullName, fullPath) + (ext != null ? $".{ext.ToLower()}" : "");
            return finalPath;
        }
        public static void ExportObject(this UObject self)
        {
            switch(self)
            {
                case USkeletalMesh SkelMesh:
                {
                    if (!File.Exists(FixPath(SkelMesh.Outer?.Name ?? SkelMesh.Name, "psk")))
                    {
                        var Save = new MeshExporter(SkelMesh, ELodFormat.FirstLod, false);
                        Logger.Log(Save.TryWriteToDir(SaveDirectory, out string SaveFileName)
                            ? $"Successfully saved {SaveFileName}"
                            : $"Failed to save {SaveFileName}");
                    }
                    break;
                }
                case UStaticMesh StaticMesh:
                {
                    if (!File.Exists(FixPath(StaticMesh.Outer?.Name ?? StaticMesh.Name, "pskx")))
                    {
                        var Save = new MeshExporter(StaticMesh, ELodFormat.FirstLod, false);
                        Logger.Log(Save.TryWriteToDir(SaveDirectory, out string SaveFileName)
                            ? $"Successfully saved {SaveFileName}"
                            : $"Failed to save {SaveFileName}");
                    }
                    break;
                }
                case UTexture2D Texture:
                {
                    var TexturePath = FixPath(Texture.Outer?.Name ?? Texture.Name, "tga");
                    if (!File.Exists(TexturePath))
                    {
                        Directory.CreateDirectory(TexturePath.Replace('\\', '/').SubstringBeforeLast('/'));
                        using var stream = new FileStream(TexturePath, FileMode.Create, FileAccess.Write);
                        Texture.Decode()?.Encode().AsStream().CopyTo(stream);
                        Logger.Log($"Successfully saved LOD 0 as '.tga' for '{Texture.Name}'");
                    }
                    break;
                }
                case UAnimSequence Anim:
                    if (!File.Exists(FixPath(Anim.Outer?.Name ?? Anim.Name, "psa")))
                    {
                        var Save = new AnimExporter(Anim);
                        Logger.Log(Save.TryWriteToDir(SaveDirectory, out string SaveFileName)
                            ? $"Successfully saved {SaveFileName}"
                            : $"Failed to save {SaveFileName}");
                    }
                    break;
            }
        }
        
        public static IEnumerable<(T item, int index)> Enumerate<T>(this IEnumerable<T> self)
            => self.Select((item, index) => (item, index));

        public static VectorParameter.Color ToParamColor(this FLinearColor self)
            => new (self.R, self.G, self.B, self.A);
    }
}