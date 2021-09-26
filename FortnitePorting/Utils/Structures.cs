using System.Collections.Generic;

namespace FortnitePorting.Utils
{
    public class Structures
    {
        public class CharacterPart
        {
            public string SkeletalMesh;
            public string CharacterPartType;
            public string AttachSocketName;
            public List<Material> Materials;
        }
        public class Material
        {
            public string MaterialPath;
            public int MaterialOverrideIndex;
            public MaterialParameters MaterialParameters;
        }
        
        public class VariantMaterial 
        {
            public string OverrideMaterial;
            public string MaterialToSwap;
            public int MaterialOverrideIndex;

            public MaterialParameters MaterialParameters;
        }
        
        public class MaterialParameters
        {
            public List<TextureParameter> TextureParameters = new();
            public List<ScalarParameter> ScalarParameters = new();
            public List<VectorParameter> VectorParameters = new();
            public List<VectorParameter> ComponentMaskParameters = new();
            public List<SwitchParameter> SwitchParameters = new();
            public VectorParameter.Color SubsurfaceColor;
        }
        public class VariantMaterialParameters
        {
            public string MaterialToAlter;
            public List<TextureParameter> TextureParameters = new();
            public List<ScalarParameter> FloatParameters = new();
            public List<VectorParameter> ColorParameters = new();
        }
        public class TextureParameter
        {
            public string Info;
            public string Value;
        }
        public class ScalarParameter
        {
            public string Info;
            public float Value;
        }
        public class SwitchParameter
        {
            public string Info;
            public bool Value;
        }
        public class VectorParameter
        {
            public string Info;
            public Color Value;

            public class Color
            {
                private float r;
                private float g;
                private float b;
                private float a;

                public Color(float r, float g, float b, float a)
                {
                    this.r = r;
                    this.g = g;
                    this.b = b;
                    this.a = a;
                }
            }
        }

        public class Processed
        {
            public List<CharacterPart> DefaultVariant { get; set; } = new();
            public List<CharacterPart> VariantParts { get; set; } = new();
            public List<VariantMaterial> VariantMaterials { get; set; } = new();
            public List<VariantMaterialParameters> VariantMaterialParameters { get; set; } = new();
        }
        
        public enum EFortCustomPartType
        {
            Head,
            Body,
            Hat,
            Backpack,
            MiscOrTail,
            Face,
            Gameplay,
            NumTypes
        }
    }
}