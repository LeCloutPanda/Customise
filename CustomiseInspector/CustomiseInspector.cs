﻿using BaseX;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using NeosModLoader;
using System;
using System.Collections.Generic;

public class CustomiseInspector : NeosMod
{
    public override string Name => "Customise-Inspector";
    public override string Author => "LeCloutPanda";
    public override string Version => "1.1.2";

    public static ModConfiguration config;

    [AutoRegisterConfigKey]
    private static ModConfigurationKey<bool> ENABLED = new ModConfigurationKey<bool>("Enabled", "", () => true);
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<string> TITLE_TEXT = new ModConfigurationKey<string>("Title Text", "", () => "Scene Inspector");
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<color> TITLE_COLOR = new ModConfigurationKey<color>("Title Color", "", () => new color(0f));

    //Main Panel
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<color> MAIN_ALBEDO = new ModConfigurationKey<color>("Main Albedo Color", "", () => new color(1f, 0.87f, 0.55f, 0.2f));
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<float> MAIN_METALIC = new ModConfigurationKey<float>("Main Metalic Amount", "", () => 0.5f);
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<bool> MAIN_BLUR_ENABLED = new ModConfigurationKey<bool>("Main Blur Enabled", "", () => true);
    //Handle and Header Panel
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<color> SECONDARY_ALBEDO = new ModConfigurationKey<color>("Secondary Albedo Color", "", () => new color(1f, 0.87f, 0.55f, 0.2f));
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<float> SECONDARY_METALIC = new ModConfigurationKey<float>("Secondary Metalic Amount", "", () => 0.5f);
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<bool> SECONDARY_BLUR_ENABLED = new ModConfigurationKey<bool>("Secondary Blur Enabled", "", () => true);
    
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<color> LEFT_SPLIT_COLOR = new ModConfigurationKey<color>("Left Split Color", "", () => new color(1f, 1f, 0f, 0.2f));
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<color> RIGHT_SPLIT_COLOR = new ModConfigurationKey<color>("Right Split Color", "", () => new color(0f, 1f, 1f, 0.2f));
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<float> SPLIT_RATIO = new ModConfigurationKey<float>("Split Ratio", "", () => 0.4f);

    // Foreground image
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<bool> FOREGROUND_IMAGE_ENABLED = new ModConfigurationKey<bool>("Enable Foreground Image", "", () => true);
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<string> FOREGROUND_IMAGE = new ModConfigurationKey<string>("Foreground Image", "", () => "neosdb:///63ef318d96b5d0d0ceba6e04a4e622b1158335cdc67c49e27839132c6f655058.png");
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<color> FOREGROUND_IMAGE_COLOR = new ModConfigurationKey<color>("Foreground Image Color", "", () => new color(0f, 0f));

    // Background image
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<bool> BACKGROUND_IMAGE_ENABLED = new ModConfigurationKey<bool>("Enable Background Image", "", () => true);
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<string> BACKGROUND_IMAGE = new ModConfigurationKey<string>("Background Image", "", () => "neosdb:///63ef318d96b5d0d0ceba6e04a4e622b1158335cdc67c49e27839132c6f655058.png");
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<color> BACKGROUND_IMAGE_COLOR = new ModConfigurationKey<color>("Background Image Color", "", () => new color(0f, 0f));
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<float3> BACKGROUND_IMAGE_ROTATION = new ModConfigurationKey<float3>("Background Image Rotation", "", () => new float3(0f, 0f, 0f));

    [AutoRegisterConfigKey]
    private static ModConfigurationKey<float2> CANVAS_SIZE = new ModConfigurationKey<float2>("Canvas Size", "", () => new float2(1000, 2000));
    [AutoRegisterConfigKey]
    private static ModConfigurationKey<float> SPAWN_SCALE = new ModConfigurationKey<float>("Spawn Scale Multiplier", "", () => 1);

    public override void OnEngineInit()
    {
        config = GetConfiguration();
        config.Save(true);

        Harmony harmony = new Harmony($"dev.{Author}.{Name}");
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(InspectorPanel), "Setup")]
    class InspectorPanelPatch
    {
        [HarmonyPrefix]
        static void Prefix(InspectorPanel __instance)
        {
            if (!config.GetValue(ENABLED))
                return;

            __instance.Slot.Name = "Custom Inspector Panel";

            Slot Assets = __instance.Slot.AddSlot("Assets");
            Assets.Tag = "Customise.Assets";
        }

        [HarmonyPostfix]
        static void Postfix(InspectorPanel __instance)
        {
            if (!config.GetValue(ENABLED) && __instance.Slot.Name != "Custom Inspector Panel")
                return;

            __instance.Slot.GlobalScale = __instance.Slot.GlobalScale * config.GetValue(SPAWN_SCALE);

            Slot panelSlot = __instance.Slot.FindChild(ch => ch.Name.Equals("Panel"), 1);
            Slot handleSlot = __instance.Slot.FindChild(ch => ch.Name.Equals("Handle"), 1);
            Slot titleMeshSlot = __instance.Slot.FindChild(ch => ch.Name.Equals("Title Mesh"), 2);
            Slot titleTextSlot = __instance.Slot.FindChild(ch => ch.Name.Equals("Title"), 2);
            Slot contentSlot = __instance.Slot.FindChild(ch => ch.Name.Equals("Content"), 1);
            Slot assetsSlot = __instance.Slot.FindChild(ch => ch.Tag.Equals("Customise.Assets"));

            PBS_RimMetallic newMaterial1 = assetsSlot.AttachComponent<PBS_RimMetallic>(true, null);
            newMaterial1.OffsetFactor.Value = 1;
            newMaterial1.OffsetUnits.Value = 1;
            newMaterial1.ForceZWrite.Value = true;
            newMaterial1.Transparent.Value = true;
            newMaterial1.RenderQueue.Value = 2995;
            newMaterial1.AlbedoColor.Value = config.GetValue(MAIN_ALBEDO);
            newMaterial1.RimColor.Value = new color(0f, 0f);
            newMaterial1.Metallic.Value = config.GetValue(MAIN_METALIC);

            PBS_RimMetallic newMaterial2 = assetsSlot.AttachComponent<PBS_RimMetallic>(true, null);
            newMaterial2.OffsetFactor.Value = 1;
            newMaterial2.OffsetUnits.Value = 1;
            newMaterial2.ForceZWrite.Value = true;
            newMaterial2.Transparent.Value = true;
            newMaterial2.RenderQueue.Value = 2995;
            newMaterial2.AlbedoColor.Value = config.GetValue(SECONDARY_ALBEDO);
            newMaterial2.RimColor.Value = new color(0f, 0f);
            newMaterial2.Metallic.Value = config.GetValue(SECONDARY_METALIC);

            bool enableBlur1 = config.GetValue(MAIN_BLUR_ENABLED);
            bool enableBlur2 = config.GetValue(SECONDARY_BLUR_ENABLED);

            DoFunny(panelSlot, newMaterial1, enableBlur1);
            DoFunny(handleSlot, newMaterial2, enableBlur2);
            DoFunny(titleMeshSlot, newMaterial2, enableBlur2);

            if (config.GetValue(BACKGROUND_IMAGE_ENABLED))
            {
                float3 rotaton = config.GetValue(BACKGROUND_IMAGE_ROTATION);
                AddBackgroundImage(__instance.Slot, contentSlot.GetComponent<Canvas>(), rotaton);
            }

            TextRenderer textRenderer = titleTextSlot.GetComponents<TextRenderer>(null, false)[0];
            textRenderer.Color.Value = config.GetValue(TITLE_COLOR);
            
            Slot imageSlot = contentSlot[0];

            if (config.GetValue(FOREGROUND_IMAGE_ENABLED))
            {
                Image image = imageSlot.GetComponent<Image>();
                SpriteProvider spriteProvider = imageSlot.AttachSprite(new Uri(config.GetValue(FOREGROUND_IMAGE)), true, false, true, null);
                image.Tint.Value = config.GetValue(FOREGROUND_IMAGE_COLOR);
                image.Sprite.Target = spriteProvider;
            }

            Slot leftSplit = imageSlot.FindChild(ch => ch.Name.Equals("Split"), 1);
            leftSplit.Name = "Left Split";
            leftSplit = imageSlot.FindChild(ch => ch.Name.Equals("Left Split"), 1);
            leftSplit.GetAllChildren()[0].GetComponent<Image>().Tint.Value = config.GetValue(LEFT_SPLIT_COLOR);
            leftSplit.GetComponent<RectTransform>().AnchorMax.Value = new BaseX.float2(config.GetValue(SPLIT_RATIO), 1);

            Slot rightSplit = imageSlot.FindChild(ch => ch.Name.Equals("Split"), 1);
            rightSplit.Name = "Right Split";
            rightSplit = imageSlot.FindChild(ch => ch.Name.Equals("Right Split"), 1);
            rightSplit.GetAllChildren()[0].GetComponent<Image>().Tint.Value = config.GetValue(RIGHT_SPLIT_COLOR);
            rightSplit.GetComponent<RectTransform>().AnchorMin.Value = new float2(config.GetValue(SPLIT_RATIO), 0);


            // Adjust canvas which adjust everything else
            Canvas canvas = contentSlot.GetComponent<Canvas>();
            canvas.Size.Value = config.GetValue(CANVAS_SIZE);

            // Actually change the title text to what ever you want
            NeosPanel __result = __instance.Slot.GetComponent<NeosPanel>();

            __result.RunInUpdates(3, () => {
                __result.MarkChangeDirty();
                __result.Title = config.GetValue(TITLE_TEXT);
            });
        }
    }

    public static void DoFunny(Slot slot, MaterialProvider material, bool blur = false)
    {
        List<MeshRenderer> mesh = slot.GetComponents<MeshRenderer>();
        mesh[0].Material.Target = material;

        if (blur != true)
        {
            for (int i = 1; i < mesh.Count; i++)
            {
                mesh[i].Destroy();
            }
        }
    }

    public static void AddBackgroundImage(Slot parent, Canvas canvas, float3 newRotation)
    {
        Slot backImageSlot = parent.AddSlot("Background", true);
        Canvas backImageCanvas = backImageSlot.AttachComponent<Canvas>();
        Image backImageImage = backImageSlot.AttachComponent<Image>();
        ValueCopy<float2> backImageValueCopy = backImageSlot.AttachComponent<ValueCopy<float2>>();

        backImageSlot.LocalPosition = new float3(0, 0, 0.011f);
        floatQ rotation = floatQ.Euler(newRotation);
        backImageSlot.LocalRotation = rotation;
        backImageSlot.LocalScale = canvas.Slot.LocalScale;

        backImageValueCopy.Source.Value = canvas.Size.ReferenceID;
        backImageValueCopy.Target.Value = backImageCanvas.Size.ReferenceID;

        backImageImage.Sprite.Target = backImageImage.Slot.AttachSprite(new Uri(config.GetValue(BACKGROUND_IMAGE)), true, false, true, null);
        backImageImage.Tint.Value = config.GetValue(BACKGROUND_IMAGE_COLOR);
    }
}