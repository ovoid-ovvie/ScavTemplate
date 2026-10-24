using AsmResolver.DotNet;
using AsmResolver.DotNet.Builder;
using BepInEx;
using CUCoreLib.Data;
using CUCoreLib.Helpers;
using CUCoreLib.Registries;
using CUCoreLib.Saving;
using CUCoreLib.Networking;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Drawing.Text;
using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using UnityEngine.XR;
using System.Linq;
using static UnityEngine.ParticleSystem.PlaybackState;

internal class Patches
{
    [HarmonyPatch(typeof(ConsoleScript))]
    internal static class ConsolePatch
    {
        [HarmonyPatch(nameof(ConsoleScript.Start))]
        [HarmonyPostfix]
        private static void StartPatch()
        {
            ConsoleScript.instance.LogToConsole("Hello World!");
        }
    }
}

//[StatusOptions(Key = "com.subex.flag", ClientAuthoritative = true)]
public sealed class SubExFlag : BodyStatus
{
    public bool Sedated;
    public float SedativeReception;
    public bool SedativeConsume;
    public bool IsAmnesiac;
    public bool LethalSedativeMix;
    public float TripChance;
    public bool OverdosingOnSedatives;
}

//[StatusOptions(Key = "com.subex.prev", ClientAuthoritative = true)]
public sealed class PrevStats : BodyStatus
{
    public float Thirst;
    public float Hunger;
    public float HeartRate;
    public float Mood;
}

//[StatusOptions(Key = "com.subex.thirstblock", ClientAuthoritative = true)]
public sealed class ThirstBlock : BodyStatus
{
    public float Reception;
    public bool Reverse;
}

//[StatusOptions(Key = "com.subex.hungerblock", ClientAuthoritative = true)]
public sealed class HungerBlock : BodyStatus
{
    public float Reception;
}

//[StatusOptions(Key = "com.subex.diuretic", ClientAuthoritative = true)]
public sealed class Diuretic : BodyStatus
{
    public float Reception;
}

//[StatusOptions(Key = "com.subex.antihistamine", ClientAuthoritative = true)]
public sealed class Antihistamine : BodyStatus
{
    public float Reception;
    public int Toxicity;
}

//[StatusOptions(Key = "com.subex.alpha2", ClientAuthoritative = true)]
public sealed class Alpha2 : BodyStatus
{
    public float Reception;
    public int Toxicity;
}

//[StatusOptions(Key = "com.subex.benzo", ClientAuthoritative = true)]
public sealed class Benzo : BodyStatus
{
    public float Reception;
    public float ActualReception;
    public float Tolerance;
    public float Antagonist;
    public int Toxicity;
}

//[StatusOptions(Key = "com.subex.barbiturate", ClientAuthoritative = true)]
public sealed class Barbiturate : BodyStatus
{
    public float Reception;
    public int Toxicity;
}

//[StatusOptions(Key = "com.subex.zdrug", ClientAuthoritative = true)]
public sealed class ZDrug : BodyStatus
{
    public float Reception;
    public int Toxicity;
}

//[StatusOptions(Key = "com.subex.cbd", ClientAuthoritative = true)]
public sealed class CBD : BodyStatus
{
    public float Reception;
}

//[StatusOptions(Key = "com.subex.thc", ClientAuthoritative = true)]
public sealed class THC : BodyStatus
{
    public float Reception;
}

//[StatusOptions(Key = "com.subex.nicotine", ClientAuthoritative = true)]
public sealed class Nicotine : BodyStatus
{
    public float Reception;
    public float ActualReception;
    public float Tolerance;
    public float StaminaReduction;
}

//[StatusOptions(Key = "com.subex.steroid", ClientAuthoritative = true)]
public sealed class Steroid : BodyStatus
{
    public float Reception;
    public float Abuse;
}

//[StatusOptions(Key = "com.subex.ketamine", ClientAuthoritative = true)]
public sealed class Ketamine : BodyStatus
{
    public float Reception;
    public float Toxicity;
}

//[StatusOptions(Key = "com.subex.psychedelic", ClientAuthoritative = true)]
public sealed class Psychedelic : BodyStatus
{
    public float Reception;
    public float DMT;
}

//[StatusOptions(Key = "com.subex.mdma", ClientAuthoritative = true)]
public sealed class MDMA : BodyStatus
{
    public float Reception;
    public int Toxicity;
}

//public sealed class Corticosteroid : BodyStatus
//{
//    public float Reception;
//}

[HarmonyPatch(typeof(BuildingEntity), "Update")]
public static class GrabberPlantDropPatch
{
    public static void Postfix(BuildingEntity __instance)
    {
        if (__instance.health >= 0.5f)
        {
            return;
        }
        if (__instance.id != "grabberplant")
        {
            return;
        }
        GameObject obj = CustomInstantiate.InstantiateReturn("grabbergland", __instance.transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
    }
}

//[HarmonyPatch(typeof(ConsoleScript), "TryExecuteCommand")]
//public static class HealThing
//{
//    [HarmonyPostfix]
//    static void TryExecuteCommand_Postfix(string[] args)
//    {
//        if (args == null || args.Length == 0 || !args[0].Equals("heal", StringComparison.OrdinalIgnoreCase))
//        {
//            return;
//        }

//        CUCoreUtils.TryGetBody(out Body body);
//        if (body == null)
//        {
//            return;
//        }

//        SubExFlag flag = body.GetStatus<SubExFlag>();
//        PrevStats prev = body.GetStatus<PrevStats>();
//        ThirstBlock statusThirstBlock = body.GetStatus<ThirstBlock>();
//        HungerBlock statusHungerBlock = body.GetStatus<HungerBlock>();
//        Diuretic statusDiuretic = body.GetStatus<Diuretic>();
//        Antihistamine statusAntihistamine = body.GetStatus<Antihistamine>();
//        Alpha2 statusAlpha2 = body.GetStatus<Alpha2>();
//        Benzo statusBenzo = body.GetStatus<Benzo>();
//        Barbiturate statusBarbiturate = body.GetStatus<Barbiturate>();
//        ZDrug statusZDrug = body.GetStatus<ZDrug>();
//        CBD statusCBD = body.GetStatus<CBD>();
//        THC statusTHC = body.GetStatus<THC>();
//        Nicotine statusNicotine = body.GetStatus<Nicotine>();
//        Steroid statusSteroid = body.GetStatus<Steroid>();
//        Ketamine statusKetamine = body.GetStatus<Ketamine>();
//        Psychedelic statusPsychedelic = body.GetStatus<Psychedelic>();
//        MDMA statusMDMA = body.GetStatus<MDMA>();

//        flag.Sedated = false;
//        flag.SedativeReception = 0f;
//        flag.SedativeConsume = false;
//        flag.IsAmnesiac = false;
//        flag.LethalSedativeMix = false;
//        flag.TripChance = 0f;
//        flag.OverdosingOnSedatives = false;

//        statusThirstBlock.Reception = 0f;
//        statusHungerBlock.Reception = 0f;
//        statusDiuretic.Reception = 0f;
//        statusAntihistamine.Reception = 0f;
//        statusAntihistamine.Toxicity = 0;
//        statusAlpha2.Reception = 0f;
//        statusAlpha2.Toxicity = 0;
//        statusBenzo.Reception = 0f;
//        statusBenzo.ActualReception = 0f;
//        statusBenzo.Toxicity = 0;
//        statusBenzo.Tolerance = 0f;
//        statusBenzo.Antagonist = 0f;
//        statusBarbiturate.Reception = 0f;
//        statusBarbiturate.Toxicity = 0;
//        statusZDrug.Reception = 0f;
//        statusZDrug.Toxicity = 0;
//        statusCBD.Reception = 0f;
//        statusTHC.Reception = 0f;
//        statusNicotine.Reception = 0f;
//        statusNicotine.ActualReception = 0f;
//        statusNicotine.Tolerance = 0f;
//        statusNicotine.StaminaReduction = 0f;
//        statusSteroid.Reception = 0f;
//        statusSteroid.Abuse = 0f;
//        statusKetamine.Reception = 0f;
//        statusKetamine.Toxicity = 0;
//        statusPsychedelic.Reception = 0f;
//        statusPsychedelic.DMT = 0f;
//        statusMDMA.Reception = 0f;
//        statusMDMA.Toxicity = 0;
//    }
//}

//[HarmonyPatch(typeof(TraderScript), "actualReputation", MethodType.Getter)]
//public static class MDMAThing
//{
//    [HarmonyPrefix]
//    private static bool Prefix(TraderScript __instance, ref float __result)
//    {
//        CUCoreUtils.TryGetBody(out Body body);
//        MDMA status = body.GetStatus<MDMA>();
//        __result = __instance.reputation - (100f - body.consciousness) * 0.5f;
//        if (status.Reception > 0f)
//        {
//            __result += status.Reception / 3;
//        }
//        return false;
//    }
//}

//[HarmonyPatch(typeof(TraderScript), "ValueMultiplier")]
//public static class MDMADiscount
//{
//    [HarmonyPrefix]
//    private static bool MDMADiscount_Prefix(TraderScript __instance, ref float __result)
//    {
//        CUCoreUtils.TryGetBody(out Body body);
//        MDMA status = body.GetStatus<MDMA>();
//        float num = __instance.reputation;
//        if (status.Reception > 0f)
//        {
//            float factor = Mathf.Ceil(status.Reception / 5f) * 5f;
//            num *= 1f + (factor * 0.01f);
//        }
//        __result = __instance.valueMultiplierCurve.Evaluate(num * 0.01f);
//        return false;
//    }
//}

[HarmonyPatch(typeof(PlayerCamera), "HandleScreenShaders")]
public static class MPCompatibleShaders
{
    [HarmonyPostfix]
    private static void MPCompatibleShaders_Postfix(PlayerCamera __instance)
    {
        CUCoreUtils.TryGetBody(out Body body);
        Ketamine statusKetamine = body.GetStatus<Ketamine>();
        Psychedelic statusPsychedelic = body.GetStatus<Psychedelic>();
        THC statusTHC = body.GetStatus<THC>();
        Steroid statusSteroid = body.GetStatus<Steroid>();

        if (statusKetamine.Reception > 0f)
        {
            float ketamineIntensity = Mathf.Clamp((80f - statusKetamine.Reception) / 100f, 0f, 1f);
            PlayerCamera.main.mainScreenPass.SetFloat("_SaturationMult", ketamineIntensity * PlayerCamera.main.satSmooth);
        }
        if (statusPsychedelic.Reception > 0f || statusPsychedelic.DMT > 0f)
        {
            if (statusPsychedelic.DMT > 0f)
            {
                float intensity = statusPsychedelic.DMT * 16f * 0.01f / 0.07f;
                PlayerCamera.main.damagePass.SetFloat("_BrainDamage", intensity * intensity);
            }
            else
            {
                float psychedelicIntensity = statusPsychedelic.Reception / 1.66f * 0.03f;
                if (psychedelicIntensity < ((100f - body.brainHealth) * 0.03))
                {
                    return;
                }
                PlayerCamera.main.bonusPass.SetFloat("_Intensity", psychedelicIntensity + __instance.bonusAbber);
            }
        }
        if (statusTHC.Reception > 0f)
        {
            float saturateIntensity = Mathf.Clamp((statusTHC.Reception) / 100f, 0f, 1f);
            PlayerCamera.main.mainScreenPass.SetFloat("_SaturationMult", (1f + saturateIntensity) * PlayerCamera.main.satSmooth);
        }
    }
}

[HarmonyPatch(typeof(TraderScript), "MeetPlayer")]
public static class MDMAReputationBoost
{
    [HarmonyPostfix]
    private static void MDMAReputationBoost_Postfix(TraderScript __instance)
    {
        CUCoreUtils.TryGetBody(out Body body);
        SubExFlag flag = body.GetStatus<SubExFlag>();
        Antihistamine statusAntihistamine = body.GetStatus<Antihistamine>();
        Benzo statusBenzo = body.GetStatus<Benzo>();
        Nicotine statusNicotine = body.GetStatus<Nicotine>();
        THC statusTHC = body.GetStatus<THC>();
        CBD statusCBD = body.GetStatus<CBD>();
        Steroid statusSteroid = body.GetStatus<Steroid>();
        Psychedelic statusPsychedelic = body.GetStatus<Psychedelic>();
        Ketamine statusKetamine = body.GetStatus<Ketamine>();
        MDMA statusMDMA = body.GetStatus<MDMA>();
        if (statusMDMA.Reception > 0f)
        {
            __instance.reputation += statusMDMA.Reception * 0.75f;
        }
        if ((statusAntihistamine.Toxicity > 0 || statusBenzo.Toxicity > 1 || statusTHC.Reception >= 55f) && !body.talker.impairedSpeech)
        {
            __instance.reputation -= 20f;
        }
        if (flag.Sedated)
        {
            __instance.hostility = Math.Max(__instance.hostility - 30f, 0);
        }
        if (statusTHC.Reception > 0f)
        {
            //__instance.reputation -= statusTHC.Reception >= 55f ? 15f : 7.5f;
            string[] lines = new string[0];
            if (__instance.character == 0)
            {
                lines = new string[]
                {
                    "Hi! Oh- your eyes, they're so yellow...",
                    "Why are you licking your lips so much?",
                    "Are you... high?"
                };
            }
            else if (__instance.character == 1)
            {
                lines = new string[]
                {
                    "I have never seen someone so stoned.",
                    "I see you've been indulging...",
                    "Hey, eyes on me. Don't get too distracted."
                };
            }
            else if (__instance.character == 2)
            {
                lines = new string[]
                {
                    "Fri-end eyes... y-e-low...",
                    "Fr-iend distracted...",
                    "Is fri-end o-kay?"
                };
            }
            __instance.talker.TalkDelayed(1.5f, lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
        }
        if (statusCBD.Reception > 0f)
        {
            __instance.reputation += 7.5f;
        }
        if (statusPsychedelic.Reception > 0f)
        {
            __instance.reputation -= 12.5f;
            string[] lines = new string[0];
            if (__instance.character == 0)
            {
                lines = new string[]
                {
                    "Why are you looking at me like that?",
                    "You seem... weird...",
                    "Are you... looking at the wall behind me?"
                };
            }
            else if (__instance.character == 1)
            {
                lines = new string[]
                {
                    "Well, you clearly aren't sober.",
                    "Please stop staring at the ceiling.",
                    "Is there something only you can see?"
                };
            }
            else if (__instance.character == 2)
            {
                lines = new string[]
                {
                    "Fri-end eyes wi-de...",
                    "Fr-iend see th-i-ngs?",
                    "Is fri-end o-kay?"
                };
            }
            __instance.talker.TalkDelayed(1.5f, lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
        }
        if (statusKetamine.Reception > 0f)
        {
            __instance.hostility += 20f;
            __instance.reputation -= 17.5f;
            string[] lines = new string[0];
            if (__instance.character == 0)
            {
                lines = new string[]
                {
                    "Why are you moving so weird?",
                    "You seem off...",
                    "I've got a bad feeling..."
                };
            }
            else if (__instance.character == 1)
            {
                lines = new string[]
                {
                    "What's wrong with you? I'm really asking.",
                    "Don't look at me like that.",
                    "Are you even sane?"
                };
            }
            else if (__instance.character == 2)
            {
                lines = new string[]
                {
                    "Fri-end look w-eird...",
                    "Fr-iend move w-eird...",
                    "Is fri-end o-kay?"
                };
            }
            __instance.talker.TalkDelayed(1.5f, lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
        }
        if (statusSteroid.Reception > 0f || statusSteroid.Abuse >= 45f)
        {
            __instance.hostility += 30f;
            __instance.reputation -= 20f;
            string[] lines = new string[0];
            if (__instance.character == 0)
            {
                lines = new string[]
                {
                    "Woah, calm down, calm down- why so twitchy?",
                    "I can see your veins pulsing, what the fuck?!",
                    "Chill... why so tense?"
                };
            }
            else if (__instance.character == 1)
            {
                lines = new string[]
                {
                    "Uh... hello. You're sweating a lot.",
                    "Why are you so angry?",
                    "Don't make me regret trusting you."
                };
            }
            else if (__instance.character == 2)
            {
                lines = new string[]
                {
                    "Fri-end mad...",
                    "Fr-iend ten-se...",
                    "Is fri-end o-kay?"
                };
            }
            __instance.talker.TalkDelayed(1.5f, lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
        }
        if (statusNicotine.ActualReception <= -20f)
        {
            if (UnityEngine.Random.value <= 0.4f)
            {
                __instance.items.Add(new TraderItem
                {
                    id = "cigarette",
                    preference = TraderScript.TraderItemPreference.Indifferent,
                    value = 0
                });
                __instance.items = (from x in __instance.items
                              orderby (int)x.preference
                              select x).ToList<TraderItem>();
                string[] lines = new string[0];
                if (__instance.character == 0)
                {
                    lines = new string[]
                    {
                        "Oh... you're shaking so much... here, I have a smoke to spare.",
                        "You poor thing... I've got something that might help."
                    };
                }
                else if (__instance.character == 1)
                {
                    lines = new string[]
                    {
                        "I can see you shaking. I might have a cigarette.",
                        "Ah, an addict. I have something that could help you."
                    };
                }
                else if (__instance.character == 2)
                {
                    lines = new string[]
                    {
                        "Fri-end s-ick... du-ne hel-p.",
                        "Fr-iend sh-ake... n-eed f-ire thing? Du-ne have."
                    };
                }
                __instance.talker.TalkDelayed(1.5f, lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
            }
            else
            {
                string[] lines = new string[0];
                if (__instance.character == 0)
                {
                    lines = new string[]
                    {
                        "Oh... you're shaking so much...",
                        "You poor thing..."
                    };
                }
                else if (__instance.character == 1)
                {
                    lines = new string[]
                    {
                        "I can see you shaking.",
                        "Ah, an addict."
                    };
                }
                else if (__instance.character == 2)
                {
                    lines = new string[]
                    {
                        "Fri-end s-ick...",
                        "Fr-iend sh-ake... n-eed f-ire thing?"
                    };
                }
                __instance.talker.TalkDelayed(1.5f, lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
            }
        }
        if (statusBenzo.ActualReception <= -12f)
        {
            if (UnityEngine.Random.value <= 0.4f)
            {
                __instance.items.Add(new TraderItem
                {
                    id = "diazepam",
                    preference = TraderScript.TraderItemPreference.Indifferent,
                    value = 0
                });
                __instance.items = (from x in __instance.items
                                    orderby (int)x.preference
                                    select x).ToList<TraderItem>();
                string[] lines = new string[0];
                if (__instance.character == 0)
                {
                    lines = new string[]
                    {
                        "Oh... you're shaking so much... here, I have a needle to spare.",
                        "You poor thing... I've got something that might help."
                    };
                }
                else if (__instance.character == 1)
                {
                    lines = new string[]
                    {
                        "I can see you shaking. I might have some medication.",
                        "Ah, an addict. I have something that could help you."
                    };
                }
                else if (__instance.character == 2)
                {
                    lines = new string[]
                    {
                        "Fri-end s-ick... du-ne hel-p.",
                        "Fr-iend sh-ake... n-eed po-key thing? Du-ne have."
                    };
                }
                __instance.talker.TalkDelayed(1.5f, lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
            }
            else
            {
                string[] lines = new string[0];
                if (__instance.character == 0)
                {
                    lines = new string[]
                    {
                        "Oh... you're shaking so much...",
                        "You poor thing..."
                    };
                }
                else if (__instance.character == 1)
                {
                    lines = new string[]
                    {
                        "I can see you shaking.",
                        "Ah, an addict."
                    };
                }
                else if (__instance.character == 2)
                {
                    lines = new string[]
                    {
                        "Fri-end s-ick...",
                        "Fr-iend sh-ake... n-eed po-key thing?"
                    };
                }
                __instance.talker.TalkDelayed(1.5f, lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
            }
        }
    }
}

[HarmonyPatch(typeof(Talker), "brainDamaged", MethodType.Getter)]
public static class BrainDamagedThing
{
    [HarmonyPrefix]
    private static bool Prefix(Talker __instance, ref bool __result)
    {
        CUCoreUtils.TryGetBody(out Body body);
        THC statusTHC = body.GetStatus<THC>();
        Antihistamine statusAntihistamine = body.GetStatus<Antihistamine>();
        Benzo statusBenzo = body.GetStatus<Benzo>();
        __result = body.brainHealth < 82.5f || body.strokeAmount > 20f || statusTHC.Reception >= 55f || statusAntihistamine.Toxicity > 1 || statusBenzo.Toxicity > 0;
        return false;
    }
}

//[HarmonyPatch(typeof(Body), "HandleBodyTemperature")]
//public static class BodyTemperature
//{
//    [HarmonyPrefix]
//    private static bool BodyTemperature_Prefix(Body __instance)
//    {
//        CUCoreUtils.TryGetBody(out Body body);
//        Painkillers pnk;
//        body.TryGetComponent<Painkillers>(out pnk);
//        Steroid status = body.GetStatus<Steroid>();
//        if (Time.time - __instance.tempCheckTime >= 1f)
//        {
//            __instance.tempCheckTime = Time.time;
//            float totalInsulation = __instance.GetTotalInsulation();
//            float addition = status.Abuse / 45f;
//            float thing = WorldGeneration.world.ambientTemperature + addition;
//            __instance.temperature = Mathf.Lerp(__instance.temperature, thing, __instance.baseTemperatureLerpRate / totalInsulation);
//            float num = 1f - Mathf.Clamp01(0.3f - __instance.energy * 0.01f);
//            if (pnk && pnk.actualOpiateReception > 0f)
//            {
//                num -= pnk.actualOpiateReception * 0.005f;
//            }
//            __instance.temperature += 0.04f * num;
//            if (__instance.temperature > 37.5f)
//            {
//                float num2 = (__instance.temperature - 37.5f) * 20f;
//                if (__instance.wetness < num2)
//                {
//                    __instance.wetness += 1f;
//                }
//            }
//            __instance.temperature -= __instance.wetness * 0.001f;
//            if (__instance.temperature < 36.5f)
//            {
//                __instance.temperature += Mathf.Max(__instance.hunger * 0.01f, 0.3f) * 0.03f * num;
//            }
//            if (__instance.temperature < 32f)
//            {
//                __instance.hunger -= 0.035f;
//                __instance.temperature += 0.01f;
//            }
//        }
//        if (__instance.temperature > 42f)
//        {
//            __instance.brainHealth -= Time.deltaTime * 0.5f;
//        }
//        if (__instance.temperature > 41f)
//        {
//            __instance.bloodVolume -= Time.deltaTime * 0.15f;
//        }
//        return false;
//    }
//}

[HarmonyPatch(typeof(Body), "allowUseItem", MethodType.Getter)]
public static class AllowUseItemUpdate
{
    [HarmonyPrefix]
    private static bool Prefix(Body __instance, ref bool __result)
    {
        Benzo statusBenzo = __instance.GetStatus<Benzo>();
        Ketamine statusKetamine = __instance.GetStatus<Ketamine>();
        THC statusTHC = __instance.GetStatus<THC>();
        __result = __instance.totalHappiness > -20f || UnityEngine.Random.value >= __instance.depressionChanceCurve.Evaluate(__instance.totalHappiness);
        if (statusBenzo.Toxicity > 1 && __result)
        {
            System.Random rand = new System.Random();
            int roll = rand.Next(1, 101);
            if (roll <= Math.Min(statusBenzo.ActualReception * 0.42f, 80f))
            {
                __result = false;
            }    
        }
        if (statusKetamine.Toxicity > 0 && __result)
        {
            System.Random rand = new System.Random();
            int roll = rand.Next(1, 101);
            if (roll <= Math.Min(statusKetamine.Reception * 0.2f, 80f))
            {
                __result = false;
            }
        }
        if (statusTHC.Reception >= 55f && __result)
        {
            System.Random rand = new System.Random();
            int roll = rand.Next(1, 101);
            if (roll <= 8)
            {
                __result = false;
            }
        }
        return false;
    }
}

[HarmonyPatch(typeof(Body), "canTakeNap", MethodType.Getter)]
public static class BodyCanHonkShoo
{
    [HarmonyPrefix]
    private static bool Prefix(Body __instance, ref bool __result)
    {
        SubExFlag flag = __instance.GetStatus<SubExFlag>();
        Antihistamine statusAntihistamine = __instance.GetStatus<Antihistamine>();
        Alpha2 statusAlpha2 = __instance.GetStatus<Alpha2>();
        Benzo statusBenzo = __instance.GetStatus<Benzo>();
        Barbiturate statusBarbiturate = __instance.GetStatus<Barbiturate>();
        Nicotine statusNicotine = __instance.GetStatus<Nicotine>();
        Psychedelic statusPsychedelic = __instance.GetStatus<Psychedelic>();
        bool sedativeFlag = (flag.SedativeReception > __instance.energy) || (statusAntihistamine.Reception > __instance.energy) || (statusAlpha2.Reception > __instance.energy * 0.65) || (statusBenzo.Reception > __instance.energy * 0.45) || (statusBarbiturate.Reception > __instance.energy * 0.25);
        __result = sedativeFlag || (__instance.energy < 35f && __instance.averagePain < 31f && __instance.sicknessAmount < 80f && statusNicotine.ActualReception >= -35f && statusPsychedelic.DMT <= 0f) || __instance.GetComponent<SleepingPills>() || WorldGeneration.GetRunSettingBool("nosleeprestrictions");
        return false;
    }
}

[HarmonyPatch(typeof(Body), "WakeUp")]
public static class BodyWakeUpUpdate
{
    [HarmonyPrefix]
    private static bool Prefix(Body __instance)
    {
        SubExFlag flag = __instance.GetStatus<SubExFlag>();
        if (flag.Sedated && !flag.OverdosingOnSedatives && (__instance.energy <= 99f || (__instance.curSleep == Body.SleepQuality.Mediocre && __instance.energy <= 85f) || (__instance.curSleep == Body.SleepQuality.Bad && __instance.energy <= 70f)))
        {
            return false;
        }
        return true;
    }
}

//[HarmonyPatch(typeof(Body), "TryLastStand")]
//public static class LastStandUpdate
//{
//    [HarmonyPostfix]
//    private static void Postfox(Body __instance)
//    {
//        CUCoreUtils.TryGetBody(out Body body);
//        SubExFlag flag = body.GetStatus<SubExFlag>();
//        PrevStats prev = body.GetStatus<PrevStats>();
//        ThirstBlock statusThirstBlock = body.GetStatus<ThirstBlock>();
//        HungerBlock statusHungerBlock = body.GetStatus<HungerBlock>();
//        Diuretic statusDiuretic = body.GetStatus<Diuretic>();
//        Antihistamine statusAntihistamine = body.GetStatus<Antihistamine>();
//        Alpha2 statusAlpha2 = body.GetStatus<Alpha2>();
//        Benzo statusBenzo = body.GetStatus<Benzo>();
//        Barbiturate statusBarbiturate = body.GetStatus<Barbiturate>();
//        ZDrug statusZDrug = body.GetStatus<ZDrug>();
//        CBD statusCBD = body.GetStatus<CBD>();
//        THC statusTHC = body.GetStatus<THC>();
//        Nicotine statusNicotine = body.GetStatus<Nicotine>();
//        Steroid statusSteroid = body.GetStatus<Steroid>();
//        Ketamine statusKetamine = body.GetStatus<Ketamine>();
//        Psychedelic statusPsychedelic = body.GetStatus<Psychedelic>();
//        MDMA statusMDMA = body.GetStatus<MDMA>();

//        flag.Sedated = false;
//        flag.SedativeReception = 0f;
//        flag.SedativeConsume = false;
//        flag.IsAmnesiac = false;
//        flag.LethalSedativeMix = false;
//        flag.TripChance = 0f;
//        flag.OverdosingOnSedatives = false;

//        statusThirstBlock.Reception = 0f;
//        statusHungerBlock.Reception = 0f;
//        statusDiuretic.Reception = 0f;
//        statusAntihistamine.Reception = 0f;
//        statusAntihistamine.Toxicity = 0;
//        statusAlpha2.Reception = 0f;
//        statusAlpha2.Toxicity = 0;
//        statusBenzo.Reception = 0f;
//        statusBenzo.ActualReception = 0f;
//        statusBenzo.Toxicity = 0;
//        statusBenzo.Antagonist = 0f;
//        statusBarbiturate.Reception = 0f;
//        statusBarbiturate.Toxicity = 0;
//        statusZDrug.Reception = 0f;
//        statusZDrug.Toxicity = 0;
//        statusCBD.Reception = 0f;
//        statusTHC.Reception = 0f;
//        statusNicotine.Reception = 0f;
//        statusNicotine.ActualReception = 0f;
//        statusSteroid.Reception = 0f;
//        statusKetamine.Reception = 0f;
//        statusKetamine.Toxicity = 0;
//        statusPsychedelic.Reception = 0f;
//        statusPsychedelic.DMT = 0f;
//        statusMDMA.Reception = 0f;
//        statusMDMA.Toxicity = 0;
//    }
//}

[HarmonyPatch(typeof(Body), "HandleBody")]
public static class HandleBodyStatusUpdate
{
    [HarmonyPrefix]
    private static void Prefix(Body __instance)
    {
        PrevStats prev = __instance.GetStatus<PrevStats>();
        ThirstBlock status = __instance.GetStatus<ThirstBlock>();
        prev.Thirst = __instance.thirst;
        prev.Hunger = __instance.hunger;
        prev.Mood = __instance.happiness;
        if (status.Reception > 0f)
        {
            status.Reverse = __instance.thirst > 60f ? true : false;
        }
    }

    [HarmonyPostfix]
    private static void Postfix(Body __instance)
    {
        if (__instance.brainHealth < 0.1f)
        {
            return;
        }
        SubExFlag flag = __instance.GetStatus<SubExFlag>();
        PrevStats prev = __instance.GetStatus<PrevStats>();
        ThirstBlock statusThirstBlock = __instance.GetStatus<ThirstBlock>();
        HungerBlock statusHungerBlock = __instance.GetStatus<HungerBlock>();
        Diuretic statusDiuretic = __instance.GetStatus<Diuretic>();
        Antihistamine statusAntihistamine = __instance.GetStatus<Antihistamine>();
        Alpha2 statusAlpha2 = __instance.GetStatus<Alpha2>();
        Benzo statusBenzo = __instance.GetStatus<Benzo>();
        Barbiturate statusBarbiturate = __instance.GetStatus<Barbiturate>();
        ZDrug statusZDrug = __instance.GetStatus<ZDrug>();
        CBD statusCBD = __instance.GetStatus<CBD>();
        THC statusTHC = __instance.GetStatus<THC>();
        Nicotine statusNicotine = __instance.GetStatus<Nicotine>();
        Steroid statusSteroid = __instance.GetStatus<Steroid>();
        Ketamine statusKetamine = __instance.GetStatus<Ketamine>();
        Psychedelic statusPsychedelic = __instance.GetStatus<Psychedelic>();
        MDMA statusMDMA = __instance.GetStatus<MDMA>();
        //Corticosteroid statusCorticosteroid = __instance.GetStatus<Corticosteroid>();
        if (statusTHC.Reception >= 55f)
        {
            if (__instance.thirst < prev.Thirst)
            {
                float difference = prev.Thirst - __instance.thirst;
                //MultiplayerApi.Apply(__instance, "thirst", -difference);
                __instance.thirst -= difference;
            }
        }
        if (statusThirstBlock.Reception > 0f && !statusThirstBlock.Reverse)
        {
            if (__instance.thirst < prev.Thirst)
            {
                //MultiplayerApi.ApplyAbsolute(__instance, "thirst", prev.Thirst);
                __instance.thirst = prev.Thirst;
            }
        }
        else if (statusThirstBlock.Reception > 0f && statusThirstBlock.Reverse)
        {
            //MultiplayerApi.Apply(__instance, "thirst", -(1f * Time.deltaTime));
            __instance.thirst -= 1f * Time.deltaTime;
        }
        if (statusDiuretic.Reception > 0)
        {
            //MultiplayerApi.Apply(__instance, "thirst", -(0.025f * statusDiuretic.Reception * Time.deltaTime));
            __instance.thirst -= 0.025f * statusDiuretic.Reception * Time.deltaTime;
        }
        if ((statusHungerBlock.Reception > 0f && __instance.hunger < 99.9f) || ((statusNicotine.ActualReception > 0f && statusNicotine.Tolerance <= 27.5f) && __instance.hunger < 99.9f))
        {
            if (__instance.hunger < prev.Hunger)
            {
                //MultiplayerApi.ApplyAbsolute(__instance, "hunger", prev.Hunger);
                __instance.hunger = prev.Hunger;
            }
        }
        //flag.SedativeReception = statusAntihistamine.Reception + statusAlpha2.Reception + statusBenzo.Reception + statusBarbiturate.Reception + statusZDrug.Reception;
        flag.Sedated = statusAntihistamine.Reception > 0 || statusAlpha2.Reception > 0 || statusBenzo.Reception > 0 || statusBarbiturate.Reception > 0 || statusZDrug.Reception > 0;
        flag.OverdosingOnSedatives = statusAntihistamine.Toxicity > 1 || statusAlpha2.Toxicity > 1 || statusBenzo.Toxicity > 1 || statusBarbiturate.Toxicity > 1 || statusZDrug.Toxicity > 1;
        if (statusAntihistamine.Reception > 0f)
        {
            if (statusAntihistamine.Reception <= 50)
            {
                statusAntihistamine.Toxicity = 0;
            }
            else if (statusAntihistamine.Reception <= 65)
            {
                statusAntihistamine.Toxicity = 1;
                //MultiplayerApi.Apply(__instance, "temperature", 2.75f / 60f * Time.deltaTime);
                __instance.temperature += 2.75f / 60f * Time.deltaTime;
                if (__instance.limbs[0].pain < 25)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.02f * statusAntihistamine.Reception * Time.deltaTime);
                    __instance.limbs[0].pain += 0.02f * statusAntihistamine.Reception * Time.deltaTime;
                }
            }
            else if (statusAntihistamine.Reception <= 90)
            {
                statusAntihistamine.Toxicity = 2;
                //MultiplayerApi.Apply(__instance, "temperature", 5f / 60f * Time.deltaTime);
                __instance.temperature += 5f / 60f * Time.deltaTime;
                if (__instance.limbs[0].pain < 35)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.02f * statusAntihistamine.Reception * Time.deltaTime);
                    __instance.limbs[0].pain += 0.02f * statusAntihistamine.Reception * Time.deltaTime;
                }
            }
            else
            {
                statusAntihistamine.Toxicity = 3;
                //MultiplayerApi.Apply(__instance, "temperature", 6.75f / 60f * Time.deltaTime);
                __instance.temperature += 6.75f / 60f * Time.deltaTime;
                if (__instance.limbs[0].pain < 50)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.025f * statusAntihistamine.Reception * Time.deltaTime);
                    __instance.limbs[0].pain += 0.025f * statusAntihistamine.Reception * Time.deltaTime;
                }
            }
        }
        if (statusAlpha2.Reception > 0f)
        {
            //MultiplayerApi.Apply(__instance, "averagePain", -(0.1f * statusAlpha2.Reception * Time.deltaTime));
            __instance.averagePain -= 0.1f * statusAlpha2.Reception * Time.deltaTime;
            if (statusAlpha2.Reception <= 55)
            {
                statusAlpha2.Toxicity = 0;
            }
            else if (statusAlpha2.Reception <= 65)
            {
                statusAlpha2.Toxicity = 1;
                //MultiplayerApi.Apply(__instance, "temperature", -(1.5f / 60f * Time.deltaTime));
                __instance.temperature -= 1.5f / 60f * Time.deltaTime;
            }
            else if (statusAlpha2.Reception <= 80)
            {
                statusAlpha2.Toxicity = 2;
                //MultiplayerApi.Apply(__instance, "temperature", -(2.75f / 60f * Time.deltaTime));
                //MultiplayerApi.Apply(__instance, "respiratoryRate", -(0.5f * 4f / 60f * Time.deltaTime));
                __instance.temperature -= 2.75f / 60f * Time.deltaTime;
                __instance.respiratoryRate -= 0.5f * 4f / 60f * Time.deltaTime;
            }
            else
            {
                statusAlpha2.Toxicity = 3;
                //MultiplayerApi.Apply(__instance, "temperature", -(4.5f / 60f * Time.deltaTime));
                //MultiplayerApi.Apply(__instance, "respiratoryRate", -(2f / 60f * Time.deltaTime));
                __instance.temperature -= 4.5f / 60f * Time.deltaTime;
                __instance.respiratoryRate -= 2f / 60f * Time.deltaTime;
            }
        }
        if (statusBenzo.ActualReception > 0f)
        {
            if (statusBenzo.ActualReception <= 65)
            {
                statusBenzo.Toxicity = 0;
            }
            else if (statusBenzo.ActualReception <= 80)
            {
                statusBenzo.Toxicity = 1;
            }
            else if (statusBenzo.ActualReception <= 95)
            {
                statusBenzo.Toxicity = 2;
                if (__instance.consciousness >= 60f)
                {
                    //MultiplayerApi.Apply(__instance, "consciousness", -(0.05f * statusBenzo.ActualReception * Time.deltaTime));
                    __instance.consciousness -= 0.05f * statusBenzo.ActualReception * Time.deltaTime;
                }
                //MultiplayerApi.Apply(__instance, "respiratoryRate", -(1f * 4f / 60f * Time.deltaTime));
                __instance.respiratoryRate -= 1f * 4f / 60f * Time.deltaTime;
            }
            else
            {
                statusBenzo.Toxicity = 3;
                if (__instance.consciousness >= 60f)
                {
                    //MultiplayerApi.Apply(__instance, "consciousness", -(0.115f * statusBenzo.ActualReception * Time.deltaTime));
                    __instance.consciousness -= 0.115f * statusBenzo.ActualReception * Time.deltaTime;
                }
                //MultiplayerApi.Apply(__instance, "respiratoryRate", -(5f * 4f / 60f * Time.deltaTime));
                __instance.respiratoryRate -= 5f * 4f / 60f * Time.deltaTime;
            }
            if (statusBenzo.Toxicity > 0)
            {
                //MultiplayerApi.Apply(__instance, "secondCheckTime", Time.deltaTime);
                __instance.secondCheckTime += Time.deltaTime;
                System.Random rand = new System.Random();
                float mult = statusBenzo.Toxicity switch
                {
                    1 => 0.25f,
                    2 => 0.6f,
                    3 => 1f,
                    _ => 0.6f
                };
                flag.TripChance += mult * Time.deltaTime;
                int roll = rand.Next(1, 101);
                if (roll <= flag.TripChance && __instance.secondCheckTime >= 1)
                {
                    __instance.Ragdoll();
                    flag.TripChance = 0f;
                }
                //MultiplayerApi.Apply(__instance, "secondCheckTime", -Time.deltaTime);
                __instance.secondCheckTime -= Time.deltaTime;
            }
        }
        else if (statusBenzo.ActualReception < 0f)
        {
            if (statusBenzo.ActualReception <= -28f)
            {
                //MultiplayerApi.Apply(__instance, "happiness", -(2f * Time.deltaTime));
                //MultiplayerApi.Apply(__instance, "brainHealth", -(0.25f * Time.deltaTime));
                __instance.happiness -= 2f * Time.deltaTime;
                __instance.brainHealth -= 0.25f * Time.deltaTime;
                if (__instance.consciousness >= 60f)
                {
                    //MultiplayerApi.Apply(__instance, "consciousness", -(7.5f / 60f * Time.deltaTime));
                    __instance.consciousness -= 7.5f / 60f * Time.deltaTime;
                }
                if (__instance.limbs[0].pain <= 45f)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.73f * Time.deltaTime);
                    __instance.limbs[0].pain += 0.73f * Time.deltaTime;
                }
                if (__instance.traumaAmount <= 65f)
                {
                    //MultiplayerApi.Apply(__instance, "traumaAmount", 0.3f * Time.deltaTime);
                    __instance.traumaAmount += 0.3f * Time.deltaTime;
                }
            }
            else if (statusBenzo.ActualReception <= -20f)
            {
                if (__instance.traumaAmount <= 65f)
                {
                    //MultiplayerApi.Apply(__instance, "traumaAmount", 0.3f * Time.deltaTime);
                    __instance.traumaAmount += 0.3f * Time.deltaTime;
                }
                if (__instance.consciousness >= 60f)
                {
                    //MultiplayerApi.Apply(__instance, "consciousness", -(0.2f * Time.deltaTime));
                    __instance.consciousness -= 0.2f * Time.deltaTime;
                }
                if (__instance.limbs[0].pain <= 45f)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.73f * Time.deltaTime);
                    __instance.limbs[0].pain += 0.73f * Time.deltaTime;
                }
                //MultiplayerApi.Apply(__instance, "happiness", -(0.175f * Time.deltaTime));
                __instance.happiness -= 0.175f * Time.deltaTime;
            }
            else if (statusBenzo.ActualReception <= -12f)
            {
                if (__instance.sicknessAmount <= 50f)
                {
                    //MultiplayerApi.Apply(__instance, "sicknessAmount", 1f * Time.deltaTime);
                    __instance.sicknessAmount += 1f * Time.deltaTime;
                }
                if (__instance.traumaAmount <= 35f)
                {
                    //MultiplayerApi.Apply(__instance, "traumaAmount", 0.15f * Time.deltaTime);
                    __instance.traumaAmount += 0.15f * Time.deltaTime;
                }
            }
        }
        if (statusBarbiturate.Reception > 0f)
        {
            if (statusBarbiturate.Reception <= 50f)
            {
                statusBarbiturate.Toxicity = 0;
            }
            else if (statusBarbiturate.Reception <= 60f)
            {
                statusBarbiturate.Toxicity = 1;
                if (__instance.limbs[0].pain <= 70f)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.06f * statusBarbiturate.Reception * Time.deltaTime);
                    __instance.limbs[0].pain += 0.06f * statusBarbiturate.Reception * Time.deltaTime;
                }
                if (__instance.consciousness >= 60f)
                {
                    //MultiplayerApi.Apply(__instance, "consciousness", 0.15f * Time.deltaTime);
                    __instance.consciousness -= 0.15f * Time.deltaTime;
                }
                //MultiplayerApi.Apply(__instance, "temperature", -(2f / 60f * Time.deltaTime));
                __instance.temperature -= 2f / 60f * Time.deltaTime;
            }
            else if (statusBarbiturate.Reception <= 70f)
            {
                statusBarbiturate.Toxicity = 2;
                if (__instance.limbs[0].pain <= 70f)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.06f * statusBarbiturate.Reception * Time.deltaTime);
                    __instance.limbs[0].pain += 0.06f * statusBarbiturate.Reception * Time.deltaTime;
                }
                if (__instance.consciousness >= 60f)
                {
                    //MultiplayerApi.Apply(__instance, "consciousness", 0.25f * Time.deltaTime);
                    __instance.consciousness -= 0.25f * Time.deltaTime;
                }
                //MultiplayerApi.Apply(__instance, "temperature", -(5f / 60f * Time.deltaTime));
                __instance.temperature -= 5f / 60f * Time.deltaTime;
            }
            else
            {
                statusBarbiturate.Toxicity = 3;
                //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.06f * statusBarbiturate.Reception * Time.deltaTime);
                //MultiplayerApi.Apply(__instance, "consciousness", -(0.25f * Time.deltaTime));
                //MultiplayerApi.Apply(__instance, "temperature", -(10f / 60f * Time.deltaTime));
                //MultiplayerApi.Apply(__instance, "respiratoryRate", -(0.5f * 4f * Time.deltaTime));
                __instance.limbs[0].pain += 0.06f * statusBarbiturate.Reception * Time.deltaTime;
                __instance.consciousness -= 0.25f * Time.deltaTime;
                __instance.temperature -= 10f / 60f * Time.deltaTime;
                __instance.respiratoryRate -= 0.5f * 4f * Time.deltaTime;
            }
        }
        if (statusKetamine.Reception > 0f)
        {
            //MultiplayerApi.ApplyAbsolute(__instance, "averagePain", Math.Max(0, __instance.averagePain - statusKetamine.Reception * 0.635f * Time.deltaTime));
            __instance.averagePain = Math.Max(0, __instance.averagePain - statusKetamine.Reception * 0.635f * Time.deltaTime);
            Limb[] limbs = __instance.limbs;
            for (int i = 0; i < limbs.Length; i++)
            {
                Limb limb = limbs[i];
                //MultiplayerApi.ApplyAbsolute(limb, "pain", Math.Max(0, limb.pain - statusKetamine.Reception * 0.635f * Time.deltaTime));
                limb.pain = Math.Max(0, limb.pain - statusKetamine.Reception * 0.635f * Time.deltaTime);
            }
            if (statusKetamine.Reception <= 60f)
            {
                statusKetamine.Toxicity = 0;
            }
            else if (statusKetamine.Reception <= 70f)
            {
                statusKetamine.Toxicity = 1;
                if (__instance.sicknessAmount <= 50f)
                {
                    //MultiplayerApi.Apply(__instance, "sicknessAmount", 0.025f * Time.deltaTime);
                    __instance.sicknessAmount += 0.025f * Time.deltaTime;
                }
            }
            else if (statusKetamine.Reception <= 85f)
            {
                statusKetamine.Toxicity = 2;
                if (__instance.sicknessAmount <= 70f)
                {
                    //MultiplayerApi.Apply(__instance, "sicknessAmount", 0.025f * Time.deltaTime);
                    __instance.sicknessAmount += 0.025f * Time.deltaTime;
                }
            }
            else
            {
                statusKetamine.Toxicity = 3;
                //MultiplayerApi.Apply(__instance, "sicknessAmount", 0.025f * Time.deltaTime);
                //MultiplayerApi.Apply(__instance, "respiratoryRate", -(15f * 4f / 60f * Time.deltaTime));
                __instance.sicknessAmount += 0.025f * Time.deltaTime;
                __instance.respiratoryRate -= 15f * 4f / 60f * Time.deltaTime;
            }
        }
        if (statusMDMA.Reception > 0f)
        {
            if (statusMDMA.Reception <= 55f)
            {
                statusMDMA.Toxicity = 0;
            }
            else if (statusMDMA.Reception <= 70f)
            {
                statusMDMA.Toxicity = 1;
                if (__instance.limbs[0].pain < 13.5f)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 1f * Time.deltaTime);
                    __instance.limbs[0].pain += 1f * Time.deltaTime;
                }
            }
            else if (statusMDMA.Reception <= 85f)
            {
                statusMDMA.Toxicity = 2;
                if (__instance.limbs[0].pain < 25f)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 1f * Time.deltaTime);
                    __instance.limbs[0].pain += 1f * Time.deltaTime;
                }
                System.Random rand = new System.Random();
                flag.TripChance += 1f * Time.deltaTime;
                int roll = rand.Next(1, 101);
                if (roll <= flag.TripChance)
                {
                    __instance.Ragdoll();
                    flag.TripChance = 0f;
                }
                //MultiplayerApi.Apply(__instance, "temperature", 3f / 60f * Time.deltaTime);
                __instance.temperature += 3f / 60f * Time.deltaTime;
            }
            else
            {
                statusMDMA.Toxicity = 3;
                if (__instance.limbs[0].pain < 32.5f)
                {
                    //MultiplayerApi.Apply(__instance.limbs[0], "pain", 1f * Time.deltaTime);
                    __instance.limbs[0].pain += 1f * Time.deltaTime;
                }
                System.Random rand = new System.Random();
                flag.TripChance += 1f * Time.deltaTime;
                int roll = rand.Next(1, 101);
                if (roll <= flag.TripChance)
                {
                    __instance.Ragdoll();
                    flag.TripChance = 0f;
                }
                //MultiplayerApi.Apply(__instance, "strokeAmount", 12.5f / 60f * Time.deltaTime);
                //MultiplayerApi.Apply(__instance, "temperature", 5f / 60f * Time.deltaTime);
                __instance.strokeAmount += 12.5f / 60f * Time.deltaTime;
                __instance.temperature += 5f / 60f * Time.deltaTime;
            }
        }
        //if (flag.Sedated && !__instance.sleeping && !flag.SedativeConsume)
        //{
        //    __instance.energy -= 0.06f * flag.SedativeReception * Time.deltaTime;
        //}
        //else if (flag.Sedated && __instance.sleeping && !flag.SedativeConsume)
        //{
        //    flag.SedativeConsume = true;
        //}
        //if (!flag.Sedated)
        //{
        //    flag.SedativeConsume = false;
        //}
        if (statusCBD.Reception > 0f)
        {
            //MultiplayerApi.Apply(__instance, "averagePain", -(0.145f * statusCBD.Reception * Time.deltaTime));
            __instance.averagePain -= 0.145f * statusCBD.Reception * Time.deltaTime;
        }
        if (statusNicotine.ActualReception >= -35f && statusNicotine.ActualReception < -20f)
        {
            if (__instance.limbs[0].pain < 25)
            {
                //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.15f * Time.deltaTime);
                __instance.limbs[0].pain += 0.15f * Time.deltaTime;
            }    
        }
        else if (statusNicotine.ActualReception >= -50f && statusNicotine.ActualReception < -35f)
        {
            //MultiplayerApi.Apply(__instance, "happiness", -(0.1f * Time.deltaTime));
            __instance.happiness -= 0.1f * Time.deltaTime;
            if (__instance.traumaAmount <= 35f)
            {
                //MultiplayerApi.Apply(__instance, "traumaAmount", 0.15f * Time.deltaTime);
                __instance.traumaAmount += 0.15f * Time.deltaTime;
            }
            if (__instance.limbs[0].pain < 37.5f)
            {
                //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.35f * Time.deltaTime);
                __instance.limbs[0].pain += 0.35f * Time.deltaTime;
            }
        }
        else if (statusNicotine.ActualReception <= -50f)
        {
            //MultiplayerApi.Apply(__instance, "happiness", -(0.225f * Time.deltaTime));
            __instance.happiness -= 0.225f * Time.deltaTime;
            if (__instance.traumaAmount <= 65f)
            {
                //MultiplayerApi.Apply(__instance, "traumaAmount", 0.25f * Time.deltaTime);
                __instance.traumaAmount += 0.25f * Time.deltaTime;
            }
            if (__instance.limbs[0].pain < 65f)
            {
                //MultiplayerApi.Apply(__instance.limbs[0], "pain", 0.6f * Time.deltaTime);
                __instance.limbs[0].pain += 0.6f * Time.deltaTime;
            }
        }
        if (statusSteroid.Reception > 0)
        {
            CUCoreUtils.TryGetBody(out Body body);
            body.skills.AddExp(0, 0.11f * statusSteroid.Reception  * Time.deltaTime);
        }
        if (statusSteroid.Abuse >= 70)
        {
            if (__instance.happiness < prev.Mood)
            {
                float difference = prev.Mood - __instance.happiness;
                //MultiplayerApi.ApplyAbsolute(__instance, "happiness", prev.Mood - difference * 1.5f);
                __instance.happiness = prev.Mood - difference * 1.5f;
            }
            //MultiplayerApi.Apply(__instance, "traumaAmount", 0.2f * Time.deltaTime);
            __instance.traumaAmount += 0.2f * Time.deltaTime;
        }
        else if (statusSteroid.Abuse >= 45)
        {
            if (__instance.happiness < prev.Mood)
            {
                float difference = prev.Mood - __instance.happiness;
                //MultiplayerApi.ApplyAbsolute(__instance, "happiness", prev.Mood - difference * 1.5f);
                __instance.happiness = prev.Mood - difference * 1.5f;
            }
            if (__instance.traumaAmount <= 65f)
            {
                //MultiplayerApi.Apply(__instance, "traumaAmount", 0.2f * Time.deltaTime);
                __instance.traumaAmount += 0.2f * Time.deltaTime;
            }
        }
        else if (statusSteroid.Abuse >= 25)
        {
            if (__instance.traumaAmount <= 35f)
            {
                //MultiplayerApi.Apply(__instance, "traumaAmount", 0.2f * Time.deltaTime);
                __instance.traumaAmount += 0.2f * Time.deltaTime;
            }
        }
        //if (statusCorticosteroid.Reception > 0)
        //{
        //    __instance.averagePain -= 0.225f * statusCorticosteroid.Reception * Time.deltaTime;
        //}
    }
}

[HarmonyPatch(typeof(Body), "HandlePeriodicChecks")]
public static class BodyPeriodicChecksUpdate
{
    [HarmonyPrefix]
    private static void Prefix(Body __instance)
    {
        CUCoreUtils.TryGetBody(out Body body);
        if (__instance.brainHealth < 0.1f)
        {
            return;
        }
        //MultiplayerApi.Apply(__instance, "halfMinuteCheckTime", Time.deltaTime);
        //MultiplayerApi.Apply(__instance, "minuteCheckTime", Time.deltaTime);
        __instance.halfMinuteCheckTime += Time.deltaTime;
        __instance.minuteCheckTime += Time.deltaTime;
        SubExFlag flag = __instance.GetStatus<SubExFlag>();
        ThirstBlock statusThirstBlock = __instance.GetStatus<ThirstBlock>();
        HungerBlock statusHungerBlock = __instance.GetStatus<HungerBlock>();
        Diuretic statusDiuretic = __instance.GetStatus<Diuretic>();
        Antihistamine statusAntihistamine = __instance.GetStatus<Antihistamine>();
        Alpha2 statusAlpha2 = __instance.GetStatus<Alpha2>();
        Benzo statusBenzo = __instance.GetStatus<Benzo>();
        Barbiturate statusBarbiturate = __instance.GetStatus<Barbiturate>();
        ZDrug statusZDrug = __instance.GetStatus<ZDrug>();
        CBD statusCBD = __instance.GetStatus<CBD>();
        THC statusTHC = __instance.GetStatus<THC>();
        Nicotine statusNicotine = __instance.GetStatus<Nicotine>();
        Steroid statusSteroid = __instance.GetStatus<Steroid>();
        Ketamine statusKetamine = __instance.GetStatus<Ketamine>();
        Psychedelic statusPsychedelic = __instance.GetStatus<Psychedelic>();
        MDMA statusMDMA = __instance.GetStatus<MDMA>();
        if (statusKetamine.Reception > 0f)
        {
            float ketamineIntensity = Mathf.Clamp(statusKetamine.Reception / 100f, 0f, 1f);
            //PlayerCamera.main.damagePass.SetFloat("_BrainDamage", Mathf.Pow((1f - (1 - ketamineIntensity) * 0.01f / 0.7f), 2));
            if (__instance.halfMinuteCheckTime > 30f)
            {
                if (UnityEngine.Random.value > 1 - ketamineIntensity && UnityEngine.Random.value < 0.75f)
                {
                    Talker talker = body.talker;
                    talker.Talk(Locale.GetCharacter("hitbycreature"));
                    PlayerCamera.main.threatMusicTime = 8f;
                }
                if (UnityEngine.Random.value > 1 - ketamineIntensity && UnityEngine.Random.value < 0.75f)
                {
                    __instance.StartCoroutine("BrainControlReverse");
                }
                if (UnityEngine.Random.value > 1 - ketamineIntensity && UnityEngine.Random.value < 0.75f)
                {
                    Vomiter vomiter = body.vomiter;
                    vomiter.Vomit();
                }
                if (UnityEngine.Random.value > 1 - ketamineIntensity)
                {
                    Talker talker = body.talker;
                    if (UnityEngine.Random.value < 0.75f)
                    {
                        __instance.DropItem(0);
                    }
                    if (UnityEngine.Random.value < 0.75f)
                    {
                        __instance.DropItem(1);
                    }
                    if (UnityEngine.Random.value < 0.75f)
                    {
                        __instance.DropItem(2);
                    }
                    talker.Talk("..?");
                }
                if (UnityEngine.Random.value > (1 - ketamineIntensity) * 100f * 0.006f)
                {
                    __instance.StartCoroutine(__instance.BrainDamageRagdoll());
                }
                if (__instance.consciousness > 20f && (double)UnityEngine.Random.value > (double)1 - ketamineIntensity && !PlayerCamera.main.lastStandPanel.activeSelf)
                {
                    PlayerCamera.main.StartCoroutine("FlashBrain");
                }
            }
        }
        if (statusPsychedelic.Reception > 0f || statusPsychedelic.DMT > 0f)
        {
            if (__instance.halfMinuteCheckTime > 30f)
            {
                float psychedelicIntensity = Mathf.Clamp(statusPsychedelic.Reception / 100f, 0f, 1f);
                //PlayerCamera.main.damagePass.SetFloat("_BrainDamage", Mathf.Pow((1f - (1 - psychedelicIntensity) * 0.01f / 0.07f), 2));
                if (UnityEngine.Random.value > 1 - psychedelicIntensity && UnityEngine.Random.value < 0.75f)
                {
                    Talker talker = body.talker;
                    talker.Talk(Locale.GetCharacter("hitbycreature"));
                    PlayerCamera.main.threatMusicTime = 8f;
                }
                if (UnityEngine.Random.value > 1 - psychedelicIntensity && UnityEngine.Random.value < 0.75f)
                {
                    __instance.StartCoroutine("BrainControlReverse");
                }
                if ((__instance.consciousness > 20f && (double)UnityEngine.Random.value > (double)1 - psychedelicIntensity && !PlayerCamera.main.lastStandPanel.activeSelf) && statusPsychedelic.DMT <= 0f)
                {
                    PlayerCamera.main.StartCoroutine("FlashBrain");
                }
            }
            if (statusPsychedelic.DMT > 0f)
            {
                float psychedelicIntensity = Mathf.Clamp(statusPsychedelic.DMT * 4f / 100f, 0f, 1f);
                //PlayerCamera.main.damagePass.SetFloat("_BrainDamage", Mathf.Pow((1f - (1 - psychedelicIntensity) * 0.01f / 0.07f), 2));
                if (UnityEngine.Random.value > 1 - psychedelicIntensity && UnityEngine.Random.value < 0.75f)
                {
                    Talker talker = body.talker;
                    talker.Talk(Locale.GetCharacter("hitbycreature"));
                    PlayerCamera.main.threatMusicTime = 8f;
                }
                if (UnityEngine.Random.value > 1 - psychedelicIntensity && UnityEngine.Random.value < 0.75f)
                {
                    __instance.StartCoroutine("BrainControlReverse");
                }
                //if (__instance.consciousness > 20f && (double)UnityEngine.Random.value > (double)1 - psychedelicIntensity && !PlayerCamera.main.lastStandPanel.activeSelf)
                //{
                //    PlayerCamera.main.StartCoroutine("FlashBrain");
                //}
            }
        }
        if (__instance.minuteCheckTime > 60f)
        {
            if (UnityEngine.Random.value < 0.5f)
            {
                if (statusNicotine.ActualReception <= -20f)
                {
                    string[] lines = new string[]
                    {
                        "I could really use a smoke...",
                        "My head hurts...",
                        "Please... just one puff...",
                        "Fuck, I need a cigarette.",
                        "I'd kill for a smoke right now.",
                        "Won't stop twitching... need a drag.",
                        "So restless..."
                    };
                    __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                }
                else if (statusNicotine.ActualReception > 0f)
                {
                    if (statusNicotine.Tolerance < 15f)
                    {
                        string[] lines = new string[]
                        {
                            "Maybe this isn't so bad...",
                            "What a rush!",
                            "I'm not getting hungry..?",
                            "Feels better than expected.",
                            "I feel so alert!"
                        };
                        __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else if (statusNicotine.Tolerance < 35f)
                    {
                        string[] lines = new string[]
                        {
                            "Oh... it's not as good as before...",
                            "I feel tingly.",
                            "Am I getting used to this?",
                            "Maybe I just need more...",
                            "Not what I expected, but I guess it works."
                        };
                        __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else
                    {
                        string[] lines = new string[]
                        {
                            "This used to feel like something.",
                            "Quiet for a minute... finally.",
                            "Hands are steady again.",
                            "Finally settled down.",
                            "Never should have started using that crap...",
                            "All that, just to feel normal again?"
                        };
                        __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                }
            }
            if (UnityEngine.Random.value < 0.5f)
            {
                if (statusCBD.Reception > 0f || statusTHC.Reception > 0f)
                {
                    string[] lines = new string[0];
                    if (statusTHC.Reception >= 55f)
                    {
                        lines = new string[]
                        {
                            "Eheh... hehe... hahhh...",
                            "Ouhhhh... that's the stuff...",
                            "Mmmh... *giggle*",
                            "Woahhhh...",
                            "So... hhheeavy....",
                            "Thisss feelinnngg...",
                            "Sssooo nnice..!",
                            "Ooghh...",
                            "Everythhhings so fffunnny now..!"
                        };
                    }
                    else
                    {
                        lines = new string[]
                        {
                            "Pain's dulling down... nice.",
                            "Head feels like a balloon.",
                            "Moving so slow... don't care.",
                            "Breathing nice and easy for once.",
                            "I am STARVING..."
                        };
                    }
                    __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                }
            }
            if (UnityEngine.Random.value < 0.5f)
            {
                if (statusPsychedelic.Reception > 0f)
                {
                    string[] lines = new string[]
                    {
                        "Woah... look at the rust breathing...",
                        "Colours... everywhere in the dark.",
                        "Everything sounds so distant...",
                        "Everything is glowing so bright...",
                        "The floor looks like water ripples."
                    };
                    __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                }
            }
            if (UnityEngine.Random.value < 0.5f)
            {
                if (statusKetamine.Reception > 0f)
                {
                    string[] lines = new string[]
                    {
                        "Can't feel my hands... are they mine?",
                        "Watching myself walk from the ceiling...",
                        "Floating... numb all over...",
                        "Where... where did my body go..?",
                        "None of this is real. At least, I think..."
                    };
                    __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                }
            }
            if (UnityEngine.Random.value < 0.5f)
            {
                if (statusMDMA.Reception > 0f)
                {
                    string[] lines = new string[]
                    {
                        "My chest is so warm... tingling all over...",
                        "Oh, I'm clenching my jaw.",
                        "Everything is so SOFT...",
                        "Mmm... whole body's buzzing.",
                        "I need someone to hug."
                    };
                    __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                }
            }
            if (UnityEngine.Random.value < 0.5f)
            {
                if (flag.SedativeReception >= 35f)
                {
                    string[] lines = new string[]
                    {
                        "Eyelids... so heavy...",
                        "Head's full of cotton...",
                        "Drifting off... so tired...",
                        "Just... need to lay down..."
                    };
                    __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                }
            }
            if (UnityEngine.Random.value < 0.5f)
            {
                if (statusBenzo.ActualReception <= -16f)
                {
                    string[] lines = new string[]
                    {
                        "Sh-shaking... all over... can't stop!",
                        "My- my heart- i-it's pounding out of my CHEST!",
                        "A-a cold sweat... please, where are the pills?!",
                        "Losing my mind... need the meds!"
                    };
                    __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                }
            }
            if (UnityEngine.Random.value < 0.5f)
            {
                if (statusSteroid.Reception > 0f)
                {
                    string[] lines = new string[]
                    {
                        "Get the FUCK out of my way!",
                        "I can feel the blood pounding in my ears!",
                        "I'll rip right through this damn rock!",
                        "Ready to break someone's neck!",
                        "I feel like I could tear iron in half!"
                    };
                    __instance.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                }
            }
        }
        //MultiplayerApi.Apply(__instance, "halfMinuteCheckTime", -Time.deltaTime);
        //MultiplayerApi.Apply(__instance, "minuteCheckTime", -Time.deltaTime);
        __instance.halfMinuteCheckTime -= Time.deltaTime;
        __instance.minuteCheckTime -= Time.deltaTime;
    }
}

[HarmonyPatch(typeof(Body), "HandleCirculation")]
public static class BodyCirculationUpdate
{
    [HarmonyPrefix]
    private static void Prefix(Body __instance)
    {
        PrevStats prev = __instance.GetStatus<PrevStats>();
        prev.HeartRate = __instance.heartRate;
    }

    [HarmonyPostfix]
    private static void Postfix(Body __instance)
    {
        if (__instance.brainHealth < 0.1f)
        {
            return;
        }
        PrevStats prev = __instance.GetStatus<PrevStats>();
        Antihistamine statusAntihistamine = __instance.GetStatus<Antihistamine>();
        Steroid statusSteroid = __instance.GetStatus<Steroid>();
        Alpha2 statusAlpha2 = __instance.GetStatus<Alpha2>();
        Benzo statusBenzo = __instance.GetStatus<Benzo>();
        Barbiturate statusBarbiturate = __instance.GetStatus<Barbiturate>();
        Ketamine statusKetamine = __instance.GetStatus<Ketamine>();
        MDMA statusMDMA = __instance.GetStatus<MDMA>();

        float multiplier = Time.deltaTime * 0.15f;
        if (multiplier < 0.0001f)
        {
            return;
        }

        float num5 = ((__instance.heartRate - prev.HeartRate) / multiplier) + prev.HeartRate;

        if (statusAntihistamine.Toxicity == 1)
        {
            num5 += 65f;
        }
        else if (statusAntihistamine.Toxicity == 2)
        {
            num5 += 90f;
        }
        else if (statusAntihistamine.Toxicity == 3)
        {
            num5 += 120f;
        }

        if (statusAlpha2.Toxicity == 0)
        {
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", 0.001f * statusAlpha2.Reception * Time.deltaTime);
            __instance.bloodVesselSize += 0.001f * statusAlpha2.Reception * Time.deltaTime;
        }
        else if (statusAlpha2.Toxicity == 1)
        {
            num5 -= 30f;
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", 0.0015f * statusAlpha2.Reception * Time.deltaTime);
            __instance.bloodVesselSize += 0.0015f * statusAlpha2.Reception * Time.deltaTime;
        }
        else if (statusAlpha2.Toxicity == 2)
        {
            num5 -= 40f;
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", 0.0015f * statusAlpha2.Reception * Time.deltaTime);
            __instance.bloodVesselSize += 0.0015f * statusAlpha2.Reception * Time.deltaTime;
        }
        else if (statusAlpha2.Toxicity == 3)
        {
            num5 -= 62.5f;
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", 0.0015f * statusAlpha2.Reception * Time.deltaTime);
            __instance.bloodVesselSize += 0.0015f * statusAlpha2.Reception * Time.deltaTime;
        }

        if (statusBenzo.Toxicity == 2)
        {
            num5 -= 30f;
        }
        else if (statusBenzo.Toxicity == 3)
        {
            num5 -= 67.5f;
        }

        if (statusBarbiturate.Toxicity == 1)
        {
            num5 -= 35f;
        }
        else if (statusBarbiturate.Toxicity == 2)
        {
            num5 -= 47.5f;
        }
        else if (statusBarbiturate.Toxicity == 3)
        {
            num5 -= 85f;
        }

        if (statusSteroid.Abuse >= 70)
        {
            num5 += 132.5f;
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", -(0.004f * Time.deltaTime));
            __instance.bloodVesselSize -= 0.004f * Time.deltaTime;
        }
        else if (statusSteroid.Abuse >= 45)
        {
            num5 += 95f;
            //__instance.bloodVesselSize -= 0.00175f * Time.deltaTime;
        }
        else if (statusSteroid.Abuse >= 25)
        {
            num5 += 67.5f;
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", -(0.0003f * Time.deltaTime));
            __instance.bloodVesselSize -= 0.0003f * Time.deltaTime;
        }

        if (statusKetamine.Toxicity == 2)
        {
            num5 += 95f;
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", -(0.00175f * Time.deltaTime));
            __instance.bloodVesselSize -= 0.00175f * Time.deltaTime;
        }
        else if (statusKetamine.Toxicity == 3)
        {
            num5 += 145f;
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", -(0.004f * Time.deltaTime));
            __instance.bloodVesselSize -= 0.004f * Time.deltaTime;
        }

        if (statusMDMA.Toxicity == 1)
        {
            num5 += 67.5f;
        }
        else if (statusMDMA.Toxicity == 2)
        {
            num5 += 90f;
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", -(0.00175f * Time.deltaTime));
            __instance.bloodVesselSize -= 0.00175f * Time.deltaTime;
        }
        else if (statusMDMA.Toxicity == 3)
        {
            num5 += 120f;
            //MultiplayerApi.Apply(__instance, "bloodVesselSize", -(0.004f * Time.deltaTime));
            __instance.bloodVesselSize -= 0.004f * Time.deltaTime;
        }
        //MultiplayerApi.ApplyAbsolute(__instance, "heartRate", Mathf.Clamp(Mathf.Lerp(prev.HeartRate, num5, multiplier), 0f, 3000f));
        __instance.heartRate = Mathf.Clamp(Mathf.Lerp(prev.HeartRate, num5, multiplier), 0f, 3000f);
    }
}

[HarmonyPatch(typeof(Body), "Update")]
public static class BodyStatusUpdate
{
    [HarmonyPostfix]
    private static void Postfix(Body __instance)
    {
        if (__instance.brainHealth < 0.1f)
        {
            return;
        }
        SubExFlag flag = __instance.GetStatus<SubExFlag>();
        ThirstBlock statusThirstBlock = __instance.GetStatus<ThirstBlock>();
        HungerBlock statusHungerBlock = __instance.GetStatus<HungerBlock>();
        Diuretic statusDiuretic = __instance.GetStatus<Diuretic>();
        Antihistamine statusAntihistamine = __instance.GetStatus<Antihistamine>();
        Alpha2 statusAlpha2 = __instance.GetStatus<Alpha2>();
        Benzo statusBenzo = __instance.GetStatus<Benzo>();
        Barbiturate statusBarbiturate = __instance.GetStatus<Barbiturate>();
        ZDrug statusZDrug = __instance.GetStatus<ZDrug>();
        CBD statusCBD = __instance.GetStatus<CBD>();
        THC statusTHC = __instance.GetStatus<THC>();
        Nicotine statusNicotine = __instance.GetStatus<Nicotine>();
        Steroid statusSteroid = __instance.GetStatus<Steroid>();
        Ketamine statusKetamine = __instance.GetStatus<Ketamine>();
        Psychedelic statusPsychedelic = __instance.GetStatus<Psychedelic>();
        MDMA statusMDMA = __instance.GetStatus<MDMA>();
        //Corticosteroid statusCorticosteroid = __instance.GetStatus<Corticosteroid>();

        if (flag.LethalSedativeMix)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.drugoverdose.png", 120f);
            MoodleRegistry.AddMoodle(
                3,
                icon,
                LocaleRegistry.Get("moodle", "adverseInteraction", "Adverse Interaction"),
                LocaleRegistry.Get("moodle", "adverseInteractionDesc", "You've taken drugs that shouldn't be mixed. Health complications likely."),
                critical: true,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if (statusThirstBlock.Reception > 0f)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.thirstblocked.png");
            MoodleRegistry.AddMoodle(
                4,
                icon,
                LocaleRegistry.Get("moodle", "thirstSuppressed", "Thirst Suppressed"),
                LocaleRegistry.Get("moodle", "thirstSuppressedDesc", "You don't feel any thirst."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if ((statusHungerBlock.Reception > 0f) || (statusNicotine.ActualReception > 0f && statusNicotine.Tolerance <= 27.5f))
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.hungerblocked.png");
            MoodleRegistry.AddMoodle(
                4,
                icon,
                LocaleRegistry.Get("moodle", "appetiteSuppressed", "Appetite Suppressed"),
                LocaleRegistry.Get("moodle", "appetiteSuppressedDesc", "You don't feel any hunger."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if (statusDiuretic.Reception > 0f)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.diuresis.png");
            MoodleRegistry.AddMoodle(
                0,
                icon,
                LocaleRegistry.Get("moodle", "diuresis", "Diuresis"),
                LocaleRegistry.Get("moodle", "diuresisDesc", "Fluid is rapidly draining from your body."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if (flag.Sedated)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.sedated.png", 120f);
            MoodleRegistry.AddMoodle(
                0,
                icon,
                LocaleRegistry.Get("moodle", "sedated", "Sedated"),
                LocaleRegistry.Get("moodle", "sedatedDesc", "Under the influence of sedatives. You feel your energy shrinking by the minute. Any circumstance seems comfy when you're this tired."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if (statusAntihistamine.Toxicity > 0)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.drugoverdose.png", 120f);
            if (statusAntihistamine.Toxicity == 1)
            {
                MoodleRegistry.AddMoodle(
                    1,
                    icon,
                    LocaleRegistry.Get("moodle", "antichollinergicActivity", "Antichollinergic Activity"),
                    LocaleRegistry.Get("moodle", "antichollinergicActivityDesc", "You've had a lot of antihistamines and are starting to feel the effects. Thirsty, with a racing heart and a warm feeling in your core."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusAntihistamine.Toxicity == 2)
            {
                MoodleRegistry.AddMoodle(
                    2,
                    icon,
                    LocaleRegistry.Get("moodle", "antichollinergicToxicity", "Antichollinergic Toxicity"),
                    LocaleRegistry.Get("moodle", "antichollinergicToxicityDesc", "You've overdosed on antihistamines. Burning up, with slurred speech. Your heart can't take much more of this."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusAntihistamine.Toxicity == 3)
            {
                MoodleRegistry.AddMoodle(
                    3,
                    icon,
                    LocaleRegistry.Get("moodle", "antichollinergicCrisis", "Antichollinergic Crisis"),
                    LocaleRegistry.Get("moodle", "antichollinergicCrisisDesc", "You've severely overdosed on antihistamines. With severe hyperthermia and a heart on the brink of fibrillation, death is a real possibility."),
                    critical: true,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
        }
        if (statusAlpha2.Toxicity > 0)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.drugoverdose.png", 120f);
            if (statusAlpha2.Toxicity == 1)
            {
                MoodleRegistry.AddMoodle(
                    1,
                    icon,
                    LocaleRegistry.Get("moodle", "mildSympatholyticToxicity", "Mild Sympatholytic Toxicity"),
                    LocaleRegistry.Get("moodle", "mildSympatholyticToxicityDesc", "You've had a large dose of an alpha-2 agonist and are starting to feel the effects. Lowered blood pressure and heart rate. You feel surprisingly calm."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusAlpha2.Toxicity == 2)
            {
                MoodleRegistry.AddMoodle(
                    2,
                    icon,
                    LocaleRegistry.Get("moodle", "sympatholyticToxicity", "Sympatholytic Toxicity"),
                    LocaleRegistry.Get("moodle", "sympatholyticToxicityDesc", "You’ve overdosed on an alpha-2 agonist. Your lungs feel heavy, your body feels cold, and your heart has slowed dramatically. Everything feels less urgent now."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusAlpha2.Toxicity == 3)
            {
                MoodleRegistry.AddMoodle(
                    3,
                    icon,
                    LocaleRegistry.Get("moodle", "sympatholyticCrisis", "Sympatholytic Crisis"),
                    LocaleRegistry.Get("moodle", "sympatholyticCrisisDesc", "You've severely overdosed on an alpha-2 agonist. Lethal hypotension and lowered heart rate combined with hypothermia. Your lungs feel like dead weight, but for some reason, you aren't worried. Death imminent."),
                    critical: true,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
        }
        if (statusBenzo.Toxicity > 0)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.drugoverdose.png", 120f);
            if (statusBenzo.Toxicity == 1)
            {
                MoodleRegistry.AddMoodle(
                    1,
                    icon,
                    LocaleRegistry.Get("moodle", "mildHypnoticToxicity", "Mild Hypnotic Toxicity"),
                    LocaleRegistry.Get("moodle", "mildHypnoticToxicityDesc", "You've had a large dose of a sedative-hypnotic drug and are starting to feel the effects. Your speech is slurred and your coordination is getting worse. Still alert."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusBenzo.Toxicity == 2)
            {
                MoodleRegistry.AddMoodle(
                    2,
                    icon,
                    LocaleRegistry.Get("moodle", "hypnoticToxicity", "Hypnotic Toxicity"),
                    LocaleRegistry.Get("moodle", "hypnoticToxicityDesc", "You've overdosed on a sedative-hypnotic drug. It’s getting harder to breathe, and your coordination has further degraded. Slightly confused, with a slower heart rate."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusBenzo.Toxicity == 3)
            {
                MoodleRegistry.AddMoodle(
                    3,
                    icon,
                    LocaleRegistry.Get("moodle", "severeHypnoticToxicity", "Severe Hypnotic Toxicity"),
                    LocaleRegistry.Get("moodle", "severeHypnoticToxicityDesc", "You've severely overdosed on a sedative-hypnotic drug. Your lungs are failing, and so is your heart. Death imminent."),
                    critical: true,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
        }
        if (statusBenzo.ActualReception < 0f)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.corticosteroids.png", 120f);
            if (statusBenzo.ActualReception <= -28f)
            {
                MoodleRegistry.AddMoodle(
                    3,
                    icon,
                    LocaleRegistry.Get("moodle", "lethalBenzodiazepineWithdrawal", "Lethal Benzodiazepine Withdrawal"),
                    LocaleRegistry.Get("moodle", "lethalBenzodiazepineWithdrawalDesc", "Your body is failing to sustain itself without the benzodiazepines it became so reliant on. Your nervous system is in chaos and you’re severely suicidal. Brain damage imminent."),
                    critical: true,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusBenzo.ActualReception <= -20f)
            {
                MoodleRegistry.AddMoodle(
                    2,
                    icon,
                    LocaleRegistry.Get("moodle", "severeBenzodiazepineWithdrawal", "Severe Benzodiazepine Withdrawal"),
                    LocaleRegistry.Get("moodle", "severeBenzodiazepineWithdrawalDesc", "Your mind and body are struggling to regulate themselves without benzodiazepines. Experiencing panic attacks, confusion, muscle pain, and depression."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusBenzo.ActualReception <= -12f)
            {
                MoodleRegistry.AddMoodle(
                    1,
                    icon,
                    LocaleRegistry.Get("moodle", "benzodiazepineWithdrawal", "Benzodiazepine Withdrawal"),
                    LocaleRegistry.Get("moodle", "benzodiazepineWithdrawalDesc", "Your body is desperate for another hit. Anxious and nauseous. Your mouth feels dry."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusBenzo.ActualReception <= -5f)
            {
                MoodleRegistry.AddMoodle(
                    0,
                    icon,
                    LocaleRegistry.Get("moodle", "benzodiazepineCraving", "Benzodiazepine Craving"),
                    LocaleRegistry.Get("moodle", "benzodiazepineCravingDesc", "You really want another hit."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
        }
        if (statusBarbiturate.Toxicity == 1)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.drugoverdose.png", 120f);
            MoodleRegistry.AddMoodle(
                1,
                icon,
                LocaleRegistry.Get("moodle", "barbiturateToxicity", "Barbiturate Toxicity"),
                LocaleRegistry.Get("moodle", "barbiturateToxicityDesc", "You’ve minorly overdosed on barbiturates. Thirsty, with a lowered heart rate and moderate confusion. Your body feels heavy and cold."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        else if (statusBarbiturate.Toxicity == 2)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.drugoverdose.png", 120f);
            MoodleRegistry.AddMoodle(
                2,
                icon,
                LocaleRegistry.Get("moodle", "severeBarbiturateToxicity", "Severe Barbiturate Toxicity"),
                LocaleRegistry.Get("moodle", "severeBarbiturateToxicityDesc", "You’ve overdosed on barbiturates. Lowered blood pressure, heart rate, respiration rate, and body temperature. Severe confusion. Death likely without rapid intervention."),
                critical: true,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        else if (statusBarbiturate.Toxicity == 3)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.drugoverdose.png", 120f);
            MoodleRegistry.AddMoodle(
                3,
                icon,
                LocaleRegistry.Get("moodle", "horrificBarbiturateToxicity", "Horrific Barbiturate Toxicity"),
                LocaleRegistry.Get("moodle", "horrificBarbiturateToxicityDesc", "You’ve horrifically overdosed on barbiturates. Fatally low blood pressure, heart rate, and body temperature. Extreme thirst causes rapid dehydration. Your lungs are failing and you can barely move. Extremely confused. Death all but guaranteed."),
                critical: true,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if (statusCBD.Reception > 0 || statusTHC.Reception > 0)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.cannabinoidintoxication.png");
            if (statusTHC.Reception < 55f)
            {
                MoodleRegistry.AddMoodle(
                    0,
                    icon,
                    LocaleRegistry.Get("moodle", "cannabinoidIntoxication", "Cannabinoid Intoxication"),
                    LocaleRegistry.Get("moodle", "cannabinoidIntoxicationDesc", "Euphoric, calm, and feeling less pain. Your body tingles and feels slightly heavier than usual. You could really do for a bite."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else
            {
                MoodleRegistry.AddMoodle(
                    1,
                    icon,
                    LocaleRegistry.Get("moodle", "intenseCannabinoidIntoxication", "Intense Cannabinoid Intoxication"),
                    LocaleRegistry.Get("moodle", "intenseCannabinoidIntoxicationDesc", "Intensely euphoric and feeling less pain. Your body tingles all over and feels much heavier than usual, combined with reduced coordination, thirst, and impaired speech. You could really do for a bite."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
        }
        if (statusNicotine.ActualReception <= -50f)
        {
            if (!flag.Sedated)
            {
                __instance.sleeping = false;
            }
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.nicotinewithdrawal.png");
            MoodleRegistry.AddMoodle(
                3,
                icon,
                LocaleRegistry.Get("moodle", "horrificNicotineWithdrawal", "Horrific Nicotine Withdrawal"),
                LocaleRegistry.Get("moodle", "horrificNicotineWithdrawalDesc", "Your mind is crying out at the lack of nicotine, and your body follows. Experiencing tremors and a severe headache. You don't want any drug that isn't nicotine, and the longer you go without it, the more miserable you'll get. Can't sleep."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        else if (statusNicotine.ActualReception <= -35f)
        {
            if (!flag.Sedated)
            {
                __instance.sleeping = false;
            }
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.nicotinewithdrawal.png");
            MoodleRegistry.AddMoodle(
                2,
                icon,
                LocaleRegistry.Get("moodle", "severeNicotineWithdrawal", "Severe Nicotine Withdrawal"),
                LocaleRegistry.Get("moodle", "severeNicotineWithdrawalDesc", "You're struggling to hold your mind together. Irritable, paranoid, and depressed. Can't sleep. You need another hit, right now."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        else if (statusNicotine.ActualReception <= -20f)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.nicotinewithdrawal.png");
            MoodleRegistry.AddMoodle(
                1,
                icon,
                LocaleRegistry.Get("moodle", "nicotineWithdrawal", "Nicotine Withdrawal"),
                LocaleRegistry.Get("moodle", "nicotineWithdrawalDesc", "Your body is desperate for another hit. Restless and tingly with a nasty headache."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        else if (statusNicotine.ActualReception <= -8f)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.nicotinewithdrawal.png");
            MoodleRegistry.AddMoodle(
                0,
                icon,
                LocaleRegistry.Get("moodle", "nicotineCraving", "Nicotine Craving"),
                LocaleRegistry.Get("moodle", "nicotineCravingDesc", "You really want another hit."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        else if (statusNicotine.ActualReception > 0f)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.nicotinated.png");
            if (statusNicotine.Tolerance <= 15f)
            {
                MoodleRegistry.AddMoodle(
                    0,
                    icon,
                    LocaleRegistry.Get("moodle", "nicotinated", "Nicotinated"),
                    LocaleRegistry.Get("moodle", "nicotinatedDescNoTolerance", "Energised and a little happier. Less hungry. It feels nice."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusNicotine.Tolerance >= 15f && statusNicotine.Tolerance <= 27.5f)
            {
                MoodleRegistry.AddMoodle(
                    0,
                    icon,
                    LocaleRegistry.Get("moodle", "nicotinated", "Nicotinated"),
                    LocaleRegistry.Get("moodle", "nicotinatedDescLowTolerance", "Slightly less hungry with minor energy gain. Doesn't feel as good as it used to."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusNicotine.Tolerance >= 27.5f)
            {
                MoodleRegistry.AddMoodle(
                    0,
                    icon,
                    LocaleRegistry.Get("moodle", "nicotinated", "Nicotinated"),
                    LocaleRegistry.Get("moodle", "nicotinatedDescHighTolerance", "No real effects, just tides over the withdrawals."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
        }
        if (statusSteroid.Reception > 0)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.anabolicsteroids.png", 120f);
            MoodleRegistry.AddMoodle(
                0,
                icon,
                LocaleRegistry.Get("moodle", "anabolicSteroids", "Anabolic Steroids"),
                LocaleRegistry.Get("moodle", "anabolicSteroidsDesc", "Increases strength over time. You feel angry."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if (statusSteroid.Abuse >= 70)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.steroidabuse.png");
            MoodleRegistry.AddMoodle(
                3,
                icon,
                LocaleRegistry.Get("moodle", "lethalSteroidAbuse", "Lethal Steroid Abuse"),
                LocaleRegistry.Get("moodle", "lethalSteroidAbuseDesc", "You’ve been abusing steroids to the extreme. The sweat won't stop, and your heart is ready to burst under its own pressure."),
                critical: true,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        else if (statusSteroid.Abuse >= 45)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.steroidabuse.png");
            MoodleRegistry.AddMoodle(
                2,
                icon,
                LocaleRegistry.Get("moodle", "steroidAbuse", "Steroid Abuse"),
                LocaleRegistry.Get("moodle", "steroidAbuseDesc", "You’ve been using steroids way too much. Greatly elevated heart rate and blood pressure. Your mood is highly volatile and you feel hot."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        else if (statusSteroid.Abuse >= 25)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.steroidabuse.png");
            MoodleRegistry.AddMoodle(
                1,
                icon,
                LocaleRegistry.Get("moodle", "steroidOveruse", "Steroid Overuse"),
                LocaleRegistry.Get("moodle", "steroidOveruseDesc", "You’ve been using steroids too much. Elevated heart rate and blood pressure. You feel anxious and slightly warm."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if (statusKetamine.Reception > 0f)
        {
            Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.drugoverdose.png", 120f);
            if (statusKetamine.Toxicity == 0)
            {
                MoodleRegistry.AddMoodle(
                    0,
                    AssetLoader.LoadEmbeddedSprite("Images.dissociating.png"),
                    LocaleRegistry.Get("moodle", "dissociating", "Dissociating"),
                    LocaleRegistry.Get("moodle", "dissociatingDesc", "Your brain is in a completely different place. Pain isn’t something you can understand anymore."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusKetamine.Toxicity == 1)
            {
                MoodleRegistry.AddMoodle(
                    1,
                    icon,
                    LocaleRegistry.Get("moodle", "mildDissociativeToxicity", "Mild Dissociative Toxicity"),
                    LocaleRegistry.Get("moodle", "mildDissociativeToxicityDesc", "You've had a large amount of dissociatives and are starting to feel the effects. Nauseous and experiencing involuntary movements. You feel like a passenger in your own body."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                 );
            }
            else if (statusKetamine.Toxicity == 2)
            {
                MoodleRegistry.AddMoodle(
                    2,
                    icon,
                    LocaleRegistry.Get("moodle", "dissociativeToxicity", "Dissociative Toxicity"),
                    LocaleRegistry.Get("moodle", "dissociativeToxicityDesc", "You've overdosed on dissociatives. Your consciousness is fading while your blood pressure and heart rate rise. Your lungs feel slightly heavy."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusKetamine.Toxicity == 3)
            {
                MoodleRegistry.AddMoodle(
                    3,
                    icon,
                    LocaleRegistry.Get("moodle", "severeDissociativeToxicity", "Severe Dissociative Toxicity"),
                    LocaleRegistry.Get("moodle", "severeDissociativeToxicityDesc", "You've severely overdosed on dissociatives. Dangerously high heart rate and blood pressure along with respiratory depression. Coma and death possible."),
                    critical: true,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
        }
        if (statusPsychedelic.Reception > 0f)
        {
            MoodleRegistry.AddMoodle(
                0,
                AssetLoader.LoadEmbeddedSprite("breakthrough.png"),
                LocaleRegistry.Get("moodle", "tripping", "Tripping"),
                LocaleRegistry.Get("moodle", "trippingDesc", "Reality distorted. You can’t interact with the world properly, but you feel so happy…"),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if (statusPsychedelic.DMT > 0f)
        {
            MoodleRegistry.AddMoodle(
                0,
                AssetLoader.LoadEmbeddedSprite("elsewhere.png"),
                LocaleRegistry.Get("moodle", "elsewhere", "Elsewhere"),
                LocaleRegistry.Get("moodle", "elsewhereDesc", "You’re in an entirely different dimension."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
            MoodleRegistry.AddMoodle(
                0,
                AssetLoader.LoadEmbeddedSprite("breakthrough.png"),
                LocaleRegistry.Get("moodle", "breakthrough", "Breakthrough"),
                LocaleRegistry.Get("moodle", "breakthroughDesc", "Any trace of the real world is absent from your mind. You feel at peace, content with the world."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
        }
        if (statusMDMA.Reception > 0f)
        {
            MoodleRegistry.AddMoodle(
                0,
                AssetLoader.LoadEmbeddedSprite("rolling.png"),
                LocaleRegistry.Get("moodle", "rolling", "Rolling"),
                LocaleRegistry.Get("moodle", "rollingDesc", "Everything feels amazing. You feel warm and euphoric in waves that come and go, and socialising is so much easier than ever before."),
                critical: false,
                chippedOnly: false,
                important: true,
                key: null,
                holdSeconds: 0.05f
            );
            Sprite icon = AssetLoader.LoadEmbeddedSprite("drugoverdose.png", 120f);
            if (statusMDMA.Toxicity == 1)
            {
                MoodleRegistry.AddMoodle(
                    1,
                    icon,
                    LocaleRegistry.Get("moodle", "sympathomimeticActivity", "Sympathomimetic Activity"),
                    LocaleRegistry.Get("moodle", "sympathomimeticActivityDesc", "You've had too much entactogens and are starting to feel the effects. Your mouth is painfully dry, your heart is pounding, and you're sensitive to light."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusMDMA.Toxicity == 2)
            {
                MoodleRegistry.AddMoodle(
                    2,
                    icon,
                    LocaleRegistry.Get("moodle", "sympathomimeticToxicity", "Sympathomimetic Toxicity"),
                    LocaleRegistry.Get("moodle", "sympathomimeticToxicityDesc", "You've overdosed on entactogens. You're clumsy, and your heart rate and blood pressure are dangerously high."),
                    critical: false,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
            else if (statusMDMA.Toxicity == 3)
            {
                MoodleRegistry.AddMoodle(
                    3,
                    icon,
                    LocaleRegistry.Get("moodle", "sympathomimeticCrisis", "Sympathomimetic Crisis"),
                    LocaleRegistry.Get("moodle", "sympathomimeticCrisisDesc", "You've severely overdosed on entactogens. Your heart rate and blood pressure are through the roof. Stroke and death likely."),
                    critical: true,
                    chippedOnly: false,
                    important: true,
                    key: null,
                    holdSeconds: 0.05f
                );
            }
        }
        //if (statusCorticosteroid.Reception > 0)
        //{
        //    Sprite icon = AssetLoader.LoadEmbeddedSprite("Images.corticosteroids.png", 120f);
        //    MoodleRegistry.AddMoodle(
        //        0,
        //        icon,
        //        "Corticosteroids",
        //        "Under the influence of corticosteroids. Feeling less pain.",
        //        critical: false,
        //        chippedOnly: false
        //    );
        //}

        statusThirstBlock.Reception = Math.Max(0, statusThirstBlock.Reception - 0.063f * Time.deltaTime);
        statusHungerBlock.Reception = Math.Max(0, statusHungerBlock.Reception - 0.063f * Time.deltaTime);
        statusDiuretic.Reception = Math.Max(0, statusDiuretic.Reception - 0.5f * Time.deltaTime);
        statusAntihistamine.Reception = Math.Max(0, statusAntihistamine.Reception - 0.052f * Time.deltaTime);
        statusAlpha2.Reception = Math.Max(0, statusAlpha2.Reception - 0.06f * Time.deltaTime);
        statusBenzo.Reception = Math.Max(0, statusBenzo.Reception - 0.06f * Time.deltaTime);
        statusBenzo.Antagonist = Math.Max(0, statusBenzo.Antagonist - 0.12f * Time.deltaTime);
        statusBarbiturate.Reception = Math.Max(0, statusBarbiturate.Reception - 0.056f * Time.deltaTime);
        statusZDrug.Reception = Math.Max(0, statusZDrug.Reception - 0.06f * Time.deltaTime);
        statusCBD.Reception = Math.Max(0, statusCBD.Reception - 0.059f * Time.deltaTime);
        statusTHC.Reception = Math.Max(0, statusTHC.Reception - 0.059f * Time.deltaTime);
        statusSteroid.Reception = Math.Max(0, statusSteroid.Reception - 0.125f * Time.deltaTime);
        statusKetamine.Reception = Math.Max(0, statusKetamine.Reception - 0.09f * Time.deltaTime);
        statusPsychedelic.Reception = Math.Max(0, statusPsychedelic.Reception - 0.167f * Time.deltaTime);
        statusPsychedelic.DMT = Math.Max(0, statusPsychedelic.DMT - 1f * Time.deltaTime);
        statusMDMA.Reception = Math.Max(0, statusMDMA.Reception - 0.05f * Time.deltaTime);
        //statusCorticosteroid.Reception = Math.Max(0, statusCorticosteroid.Reception - 0.059f * Time.deltaTime);
        statusBenzo.ActualReception = statusBenzo.Reception - statusBenzo.Tolerance;
        statusNicotine.ActualReception = statusNicotine.Reception - statusNicotine.Tolerance;
        if (statusBenzo.Antagonist > 0f)
        {
            if (statusBenzo.Tolerance <= -20f)
            {
                statusBenzo.ActualReception = -30f;
            }
            else
            {
                statusBenzo.ActualReception = Math.Max(0, statusBenzo.ActualReception - 100f);
            }
        }
        if (statusMDMA.Reception > 0f)
        {
            //MultiplayerApi.Apply(__instance, "happiness", -(0.85f * Time.deltaTime * 0.05f));
            __instance.happiness -= 0.85f * Time.deltaTime * 0.05f;
        }    
        if (statusBenzo.Reception > 0)
        {
            statusBenzo.Tolerance += 0.028f * Time.deltaTime;
        }
        else
        {
            float penalty = statusBenzo.ActualReception < -15f ? 0.015f * Time.deltaTime : 0.04f * Time.deltaTime;
            statusBenzo.Tolerance = Math.Max(0, statusBenzo.Tolerance - penalty);
        }
        if (statusSteroid.Abuse > 0 && statusSteroid.Reception <= 0)
        {
            statusSteroid.Abuse = Math.Max(0, statusSteroid.Abuse - 0.025f * Time.deltaTime);
        }
        if (statusNicotine.Reception > 0)
        {
            statusNicotine.Tolerance += statusNicotine.Reception > 50 ? 0.015f * Time.deltaTime : 0.012f * Time.deltaTime;
            statusNicotine.Reception -= 0.071f * Time.deltaTime;
            float num = 0f;
            if (statusNicotine.Tolerance < 20f)
            {
                num = 0.25f;
            }
            else if (statusNicotine.Tolerance < 40f)
            {
                num = 0.15f;
            }
            //MultiplayerApi.Apply(__instance, "happiness", -(num * Time.deltaTime * 0.071f));
            __instance.happiness -= num * Time.deltaTime * 0.071f;
        }
        else if (statusNicotine.Tolerance > 0 && statusNicotine.Reception <= 0)
        {
            statusNicotine.Tolerance = Math.Max(0, statusNicotine.Tolerance - 0.015f * Time.deltaTime);
        }
        if (statusNicotine.Reception <= 0 && statusNicotine.StaminaReduction > 0)
        {
            statusNicotine.StaminaReduction = Math.Max(0, statusNicotine.StaminaReduction - 0.003f * Time.deltaTime);
        }
        if (statusCBD.Reception > 0)
        {
            //MultiplayerApi.Apply(__instance, "traumaAmount", 0.25f * Time.deltaTime * 0.059f);
            __instance.traumaAmount += 0.25f * Time.deltaTime * 0.059f;
        }
        if (statusTHC.Reception > 0)
        {
            //MultiplayerApi.Apply(__instance, "happiness", -(0.25f * Time.deltaTime * 0.059f));
            __instance.happiness -= 0.25f * Time.deltaTime * 0.059f;
        }
        if (statusCBD.Reception > 150f)
        {
            statusCBD.Reception = 150f;
        }
        if (statusTHC.Reception > 150f)
        {
            statusTHC.Reception = 150f;
        }
        if (statusNicotine.StaminaReduction > 0f)
        {
            if (__instance.stamina > 100f - statusNicotine.StaminaReduction)
            {
                //MultiplayerApi.ApplyAbsolute(__instance, "stamina", 100f - statusNicotine.StaminaReduction);
                __instance.stamina = 100f - statusNicotine.StaminaReduction;
            }
        }
        if (statusNicotine.Reception > 150f)
        {
            statusNicotine.Reception = 150f;
        }
        if (statusHungerBlock.Reception > 100f)
        {
            statusHungerBlock.Reception = 100f;
        }
        if (statusThirstBlock.Reception > 100f)
        {
            statusThirstBlock.Reception = 100f;
        }
        if (statusSteroid.Reception > 50f)
        {
            statusSteroid.Reception = 50f;
        }
        if (statusPsychedelic.Reception > 125f)
        {
            statusPsychedelic.Reception = 125f;
        }
        if (statusPsychedelic.DMT > 50f)
        {
            statusPsychedelic.DMT = 50f;
        }

        CUCoreUtils.TryGetBody(out Body body);
        Painkillers painkillers;
        body.TryGetComponent<Painkillers>(out painkillers);
        if (flag.SedativeReception >= 20f && painkillers.actualOpiateReception > 15f)
        {
            flag.LethalSedativeMix = true;
            //__instance.overdoseIndex = 2;
            //MultiplayerApi.Apply(__instance, "bloodPressure", -(Time.deltaTime * 2.5f));
            __instance.bloodPressure -= Time.deltaTime * 2.5f;
            if (__instance.respiratoryRate > 70f)
            {
                //MultiplayerApi.ApplyAbsolute(__instance, "respiratoryRate", Mathf.MoveTowards(__instance.respiratoryRate, 70f, Time.deltaTime * 15f));
                __instance.respiratoryRate = Mathf.MoveTowards(__instance.respiratoryRate, 70f, Time.deltaTime * 15f);
            }
            if (!__instance.conscious)
            {
                __instance.bloodPressure -= Time.deltaTime * 2.5f;
                __instance.heartRate -= Time.deltaTime;
                if (__instance.respiratoryRate > 45f)
                {
                    //MultiplayerApi.ApplyAbsolute(__instance, "respiratoryRate", Mathf.MoveTowards(__instance.respiratoryRate, 40f, Time.deltaTime * 15f));
                    __instance.respiratoryRate = Mathf.MoveTowards(__instance.respiratoryRate, 40f, Time.deltaTime * 15f);
                }
            }
        }
        else
        {
            flag.LethalSedativeMix = false;
        }
    }
}