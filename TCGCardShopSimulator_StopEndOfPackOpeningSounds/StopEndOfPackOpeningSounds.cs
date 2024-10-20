using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

[BepInPlugin("com.TNoStone.StopEndOfPackOpeningSounds", "Stop End Of Pack Opening Sounds", "1.0.0")]
public class StopEndOfPackOpeningSounds : BaseUnityPlugin
{
    public static ConfigEntry<bool> DisableExpIncrease;
    public static ConfigEntry<bool> DisableStarJingle3;
    public static ConfigEntry<bool> DisableGift;

    public static bool isCardOpeningSequenceActive = false;

    private void Awake()
    {
        DisableExpIncrease = Config.Bind("Settings", "DisableExpIncrease", true, "Disable 'ExpIncrease' sound during card opening sequence");
        DisableStarJingle3 = Config.Bind("Settings", "DisableStarJingle3", true, "Disable 'SFX_PercStarJingle3' sound during card opening sequence");
        DisableGift = Config.Bind("Settings", "DisableGift", true, "Disable 'SFX_Gift' sound during card opening sequence");

        Harmony harmony = new Harmony("com.TNoStone.StopEndOfPackOpeningSounds");
        harmony.PatchAll();
    }

    [HarmonyPatch]
    public static class CardOpeningSequence_InitOpenSequence_Patch
    {
        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("CardOpeningSequence");
            return AccessTools.Method(type, "InitOpenSequence");
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            isCardOpeningSequenceActive = true;
        }
    }

    [HarmonyPatch]
    public static class CardOpeningSequence_OnPressFinishGetCard_Patch
    {
        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("CardOpeningSequence");
            return AccessTools.Method(type, "OnPressFinishGetCard");
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            isCardOpeningSequenceActive = false;
        }
    }

    [HarmonyPatch]
    public static class CardOpeningSequenceUI_HideTotalValue_Patch
    {
        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("CardOpeningSequenceUI");
            return AccessTools.Method(type, "HideTotalValue");
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            isCardOpeningSequenceActive = false;
        }
    }

    [HarmonyPatch(typeof(SoundManager), "PlayAudio", new[] { typeof(string), typeof(float), typeof(float) })]
    public static class SoundManager_PlayAudio_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(string audioName)
        {
            if (isCardOpeningSequenceActive)
            {
                if (DisableStarJingle3.Value && audioName == "SFX_PercStarJingle3")
                {
                    return false;
                }
                if (DisableGift.Value && audioName == "SFX_Gift")
                {
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch]
    public static class CardOpeningSequenceUI_StartShowTotalValue_Patch
    {
        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("CardOpeningSequenceUI");
            return AccessTools.Method(type, "StartShowTotalValue");
        }

        [HarmonyPrefix]
        public static void Prefix(ref float totalValue, ref bool hasFoilCard)
        {
            if (isCardOpeningSequenceActive && DisableExpIncrease.Value)
            {
            }
        }
    }

    [HarmonyPatch]
    public static class SoundManager_SetEnableSound_ExpIncrease_Patch
    {
        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("SoundManager");
            return AccessTools.Method(type, "SetEnableSound_ExpIncrease");
        }

        [HarmonyPrefix]
        public static bool Prefix(bool isEnable)
        {
            if (isEnable && isCardOpeningSequenceActive && DisableExpIncrease.Value)
            {
                return false;
            }
            return true;
        }
    }
}
