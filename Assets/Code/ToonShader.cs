using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ToonShader : MonoBehaviour
{
    public static string ShaderName = "Hidden/ToonShader";
    Shader Shader;
    Material Material;
    [System.Serializable]
    public struct Settings
    {
        public float LineThickness;
        public float DepthThreshold;
        public int PaletteQuantization;
    }
    public Settings settings;
    void OnEnable()
    {
        Shader = Shader.Find(ShaderName);
        Material = new Material(Shader);
        // set depthtexture mode
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
        Camera.main.allowMSAA = true;

        // Force 8x multi-sampling
        QualitySettings.antiAliasing = 8;
        // This will give us a lot more interesting depth buffer for nicer looking lines.
    }

    // After frame is rendered, we can apply the toon shader
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Using reflection, apply all settings to material, prefixing a '_' to the name
        foreach (var field in settings.GetType().GetFields())
        {
            if (field.FieldType == typeof(int))
                Material.SetInt("_" + field.Name, (int)field.GetValue(settings));
            else if (field.FieldType == typeof(float))
                Material.SetFloat("_" + field.Name, (float)field.GetValue(settings));
        }
        Graphics.Blit(src, dest, Material);
    }
}
