using AsmResolver.DotNet;
using AsmResolver.DotNet.Builder;
using BepInEx;
using BepInEx.Logging;
using CUCoreLib.Data;
using CUCoreLib.Helpers;
using CUCoreLib.Registries;
using CUCoreLib.Saving;
using CUCoreLib.Networking;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Instrumentation;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UIElements;
using UnityEngine.XR;
using System.Linq;

namespace SubEx
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("net.cucorelib", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string ModGUID = "com.shiri.substancesExpanded";
        public const string ModName = "Shiri's Substances Expanded";
        public const string ModVersion = "1.2.0";

        internal static new ManualLogSource Logger;
        private readonly Harmony _harmony = new(ModGUID);
        public static Plugin Instance { get; private set; } = null!;

        internal static void OnLastStand()
        {
            CUCoreUtils.TryGetBody(out Body body);
            SubExFlag flag = body.GetStatus<SubExFlag>();
            PrevStats prev = body.GetStatus<PrevStats>();
            ThirstBlock statusThirstBlock = body.GetStatus<ThirstBlock>();
            HungerBlock statusHungerBlock = body.GetStatus<HungerBlock>();
            Diuretic statusDiuretic = body.GetStatus<Diuretic>();
            Antihistamine statusAntihistamine = body.GetStatus<Antihistamine>();
            Alpha2 statusAlpha2 = body.GetStatus<Alpha2>();
            Benzo statusBenzo = body.GetStatus<Benzo>();
            Barbiturate statusBarbiturate = body.GetStatus<Barbiturate>();
            ZDrug statusZDrug = body.GetStatus<ZDrug>();
            CBD statusCBD = body.GetStatus<CBD>();
            THC statusTHC = body.GetStatus<THC>();
            Nicotine statusNicotine = body.GetStatus<Nicotine>();
            Steroid statusSteroid = body.GetStatus<Steroid>();
            Ketamine statusKetamine = body.GetStatus<Ketamine>();
            Psychedelic statusPsychedelic = body.GetStatus<Psychedelic>();
            MDMA statusMDMA = body.GetStatus<MDMA>();

            flag.Sedated = false;
            flag.SedativeReception = 0f;
            flag.SedativeConsume = false;
            flag.IsAmnesiac = false;
            flag.LethalSedativeMix = false;
            flag.TripChance = 0f;
            flag.OverdosingOnSedatives = false;

            statusThirstBlock.Reception = 0f;
            statusHungerBlock.Reception = 0f;
            statusDiuretic.Reception = 0f;
            statusAntihistamine.Reception = 0f;
            statusAntihistamine.Toxicity = 0;
            statusAlpha2.Reception = 0f;
            statusAlpha2.Toxicity = 0;
            statusBenzo.Reception = 0f;
            statusBenzo.ActualReception = 0f;
            statusBenzo.Toxicity = 0;
            statusBenzo.Tolerance = 0f;
            statusBenzo.Antagonist = 0f;
            statusBarbiturate.Reception = 0f;
            statusBarbiturate.Toxicity = 0;
            statusZDrug.Reception = 0f;
            statusZDrug.Toxicity = 0;
            statusCBD.Reception = 0f;
            statusTHC.Reception = 0f;
            statusNicotine.Reception = 0f;
            statusNicotine.ActualReception = 0f;
            statusNicotine.Tolerance = 0f;
            statusNicotine.StaminaReduction = 0f;
            statusSteroid.Reception = 0f;
            statusSteroid.Abuse = 0f;
            statusKetamine.Reception = 0f;
            statusKetamine.Toxicity = 0;
            statusPsychedelic.Reception = 0f;
            statusPsychedelic.DMT = 0f;
            statusMDMA.Reception = 0f;
            statusMDMA.Toxicity = 0;
        }

        //public static void Register()
        //{
        //    CUCoreUtils.TryGetBody(out Body body);
        //    Steroid statusSteroid = body.GetStatus<Steroid>();
        //    MultiplayerApi.RegisterHandler("subex.addxp", payload => payload?.Value<float>(0.11f * statusSteroid.Reception * Time.deltaTime));
        //}

        void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            _harmony.PatchAll();
            Logger.LogInfo($"Plugin {ModName} is loaded!");

            //Register();

            CUCoreUtils.OnLastStand += OnLastStand;
            CUCoreUtils.OnHeal += OnLastStand;
            DoStuff();
        }

        private void DoStuff()
        {
            AudioClip smokeSound = AssetLoader.LoadEmbeddedAudio("Sound.smoking.wav");
            AssetLoader.CacheAudioClip("subex.smoke", smokeSound);

            string[] ids =
            {
                "clonidine",
                "diazepam",
                "phenobarbital",
                "ketamine",
                "hydrochloricacid",
                "acetone",
                "woodvinegar",
                "saltpeter",
                "nitricacid"
            };

            Liquids.DangerList.AddRange(ids.Except(Liquids.DangerList));

            Logger.LogInfo($"Added the following liquids to the danger list: {string.Join(", ", ids)}");
            Logger.LogInfo($"Full danger list is now: {string.Join(", ", Liquids.DangerList)}");

            ConsoleCommandRegistry.Register(
                "subexsetreception",
                "[Substances Expanded] Sets the reception of a specific drug to a chosen value.",
                args =>
                {
                    CUCoreUtils.TryGetBody(out Body body);
                    float value = float.Parse(args[2]);
                    switch (args[1])
                    {
                        case "diphenhydramine":
                            body.GetStatus<Antihistamine>().Reception = value;
                            break;
                        case "clonidine":
                            body.GetStatus<Alpha2>().Reception = value;
                            break;
                        case "diazepam":
                            body.GetStatus<Benzo>().Reception = value;
                            break;
                        case "phenobarbital":
                            body.GetStatus<Barbiturate>().Reception = value;
                            break;
                        case "ketamine":
                            body.GetStatus<Ketamine>().Reception = value;
                            break;
                        case "testosterone":
                            body.GetStatus<Steroid>().Reception = value;
                            break;
                        case "semaglutide":
                            body.GetStatus<HungerBlock>().Reception = value;
                            break;
                        case "vigatrodipsin":
                            body.GetStatus<ThirstBlock>().Reception = value;
                            break;
                        case "hydrochlorothiazide":
                            body.GetStatus<Diuretic>().Reception = value;
                            break;
                        case "psilocybin":
                            body.GetStatus<Psychedelic>().Reception = value;
                            break;
                        case "lsd":
                            body.GetStatus<Psychedelic>().Reception = value;
                            break;
                        case "dimethyltryptamine":
                            body.GetStatus<Psychedelic>().DMT = value;
                            break;
                        case "nicotine":
                            body.GetStatus<Nicotine>().Reception = value;
                            break;
                        case "thc":
                            body.GetStatus<THC>().Reception = value;
                            break;
                        case "cbd":
                            body.GetStatus<CBD>().Reception = value;
                            break;
                        case "mdma":
                            body.GetStatus<MDMA>().Reception = value;
                            break;
                                                                                                        }
                    CUCoreUtils.ConsoleLog(ConsoleScript.instance, $"[Substances Expanded] Set {args[1]} reception to {args[2]}.");
                },
                new Dictionary<int, List<string>>
                {
                    { 0, new List<string>
                    {
                        "diphenhydramine",
                        "clonidine",
                        "diazepam",
                        "phenobarbital",
                        "ketamine",
                        "testosterone",
                        "semaglutide",
                        "vigatrodipsin",
                        "hydrochlorothiazide",
                        "psilocybin",
                        "lsd",
                        "dimethyltryptamine",
                        "nicotine",
                        "thc",
                        "cbd",
                        "mdma"
                    } }
                },
                ("string drug", "The drug to set the reception of."),
                ("float reception", "The value to set the reception to.")
            );
            ConsoleCommandRegistry.Register(
                "subexsettolerance",
                "[Substances Expanded] Sets the tolerance of either nicotine or diazepam to a chosen value.",
                args =>
                {
                    CUCoreUtils.TryGetBody(out Body body);
                    float value = float.Parse(args[2]);
                    switch (args[1])
                    {
                        case "nicotine":
                            body.GetStatus<Nicotine>().Tolerance = value;
                            break;
                        case "diazepam":
                            body.GetStatus<Benzo>().Tolerance = value;
                            break;
                                                                                                        }
                    CUCoreUtils.ConsoleLog(ConsoleScript.instance, $"[Substances Expanded] Set {args[1]} tolerance to {args[2]}.");
                },
                new Dictionary<int, List<string>>
                {
                    { 0, new List<String> { "nicotine", "diazepam" } }
                },
                ("string drug", "The drug to set the tolerance of."),
                ("float tolerance", "The value to set the tolerance to.")
            );
            ConsoleCommandRegistry.Register(
                "subexsetreduction",
                "[Substances Expanded] Sets the stamina reduction for nicotine to a chosen value.",
                args =>
                {
                    CUCoreUtils.TryGetBody(out Body body);
                    Nicotine status = body.GetStatus<Nicotine>();
                    status.StaminaReduction = float.Parse(args[1]);
                    CUCoreUtils.ConsoleLog(ConsoleScript.instance, $"[Substances Expanded] Set nicotine stamina reduction to {args[1]}.");
                },
                null,
                ("float value", "The value to set the stamina reduction to.")
            );
            ConsoleCommandRegistry.Register(
                "subexsetabuse",
                "[Substances Expanded] Sets the abuse value for testosterone to a chosen value.",
                args =>
                {
                    CUCoreUtils.TryGetBody(out Body body);
                    Steroid status = body.GetStatus<Steroid>();
                    status.Abuse = float.Parse(args[1]);
                    CUCoreUtils.ConsoleLog(ConsoleScript.instance, $"[Substances Expanded] Set steroid abuse to {args[1]}.");
                },
                null,
                ("float value", "The value to set the abuse level to.")
            );
            ConsoleCommandRegistry.Register(
                "subexsober",
                "[Substances Expanded] Sets the reception level of all drugs to 0, optionally resetting tolerance, stamina reduction, and abuse too.",
                args =>
                {
                    CUCoreUtils.TryGetBody(out Body body);
                    SubExFlag flag = body.GetStatus<SubExFlag>();
                    PrevStats prev = body.GetStatus<PrevStats>();
                    ThirstBlock statusThirstBlock = body.GetStatus<ThirstBlock>();
                    HungerBlock statusHungerBlock = body.GetStatus<HungerBlock>();
                    Diuretic statusDiuretic = body.GetStatus<Diuretic>();
                    Antihistamine statusAntihistamine = body.GetStatus<Antihistamine>();
                    Alpha2 statusAlpha2 = body.GetStatus<Alpha2>();
                    Benzo statusBenzo = body.GetStatus<Benzo>();
                    Barbiturate statusBarbiturate = body.GetStatus<Barbiturate>();
                    ZDrug statusZDrug = body.GetStatus<ZDrug>();
                    CBD statusCBD = body.GetStatus<CBD>();
                    THC statusTHC = body.GetStatus<THC>();
                    Nicotine statusNicotine = body.GetStatus<Nicotine>();
                    Steroid statusSteroid = body.GetStatus<Steroid>();
                    Ketamine statusKetamine = body.GetStatus<Ketamine>();
                    Psychedelic statusPsychedelic = body.GetStatus<Psychedelic>();
                    MDMA statusMDMA = body.GetStatus<MDMA>();

                    flag.Sedated = false;
                    flag.SedativeReception = 0f;
                    flag.SedativeConsume = false;
                    flag.IsAmnesiac = false;
                    flag.LethalSedativeMix = false;
                    flag.TripChance = 0f;
                    flag.OverdosingOnSedatives = false;

                    statusThirstBlock.Reception = 0f;
                    statusHungerBlock.Reception = 0f;
                    statusDiuretic.Reception = 0f;
                    statusAntihistamine.Reception = 0f;
                    statusAntihistamine.Toxicity = 0;
                    statusAlpha2.Reception = 0f;
                    statusAlpha2.Toxicity = 0;
                    statusBenzo.Reception = 0f;
                    statusBenzo.ActualReception = 0f;
                    statusBenzo.Toxicity = 0;
                    statusBenzo.Antagonist = 0f;
                    statusBarbiturate.Reception = 0f;
                    statusBarbiturate.Toxicity = 0;
                    statusZDrug.Reception = 0f;
                    statusZDrug.Toxicity = 0;
                    statusCBD.Reception = 0f;
                    statusTHC.Reception = 0f;
                    statusNicotine.Reception = 0f;
                    statusNicotine.ActualReception = 0f;
                    statusSteroid.Reception = 0f;
                    statusKetamine.Reception = 0f;
                    statusKetamine.Toxicity = 0;
                    statusPsychedelic.Reception = 0f;
                    statusPsychedelic.DMT = 0f;
                    statusMDMA.Reception = 0f;
                    statusMDMA.Toxicity = 0;

                    if (bool.Parse(args[1]))
                    {
                        statusBenzo.Tolerance = 0f;
                        statusNicotine.Tolerance = 0f;
                        statusNicotine.StaminaReduction = 0f;
                        statusSteroid.Abuse = 0f;
                    }

                    CUCoreUtils.ConsoleLog(ConsoleScript.instance, $"[Substances Expanded] Sobered up. Reset={args[1]}");
                },
                null,
                ("bool reset", "Whether to reset tolerance, stamina reduction, and abuse (true) or not (false).")
            );

            LiquidRegistry.Register("semaglutide", new CustomLiquidInfo
            {
                name = "Semaglutide",
                description = "An appetite suppressant. Stops hunger from decaying. Drinking not recommended.",
                color = new Color(0.259f, 0.529f, 0.961f),
                valuePerLiter = 130f,
                onDrink = (ml, body) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    HungerBlock statusHungerBlock = body.GetStatus<HungerBlock>();
                    float temp = statusHungerBlock.Reception;
                    statusHungerBlock.Reception += ml * 4f * 15f * 0.1f;
                    //MultiplayerApi.Apply(body, "sicknessAmount", statusHungerBlock.Reception - temp);
                    body.sicknessAmount += statusHungerBlock.Reception - temp;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    HungerBlock statusHungerBlock = body.GetStatus<HungerBlock>();
                    float temp = statusHungerBlock.Reception;
                    statusHungerBlock.Reception += ml * 4f * 15f;
                    //MultiplayerApi.Apply(body, "sicknessAmount", statusHungerBlock.Reception - temp);
                    body.sicknessAmount += statusHungerBlock.Reception - temp;
                },
                healthUsable = false,
                injectable = true
            });
            LiquidRegistry.Register("vigatrodipsin", new CustomLiquidInfo
            {
                name = "Vigatrodipsin",
                description = "A thirst suppressant. Stops thirst from decaying. Drinking not recommended.",
                color = new Color(0.243f, 0.702f, 0.710f),
                valuePerLiter = 144f,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    ThirstBlock statusThirstBlock = body.GetStatus<ThirstBlock>();
                    statusThirstBlock.Reception += ml * 4f * 15f * 0.05f;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    ThirstBlock statusThirstBlock = body.GetStatus<ThirstBlock>();
                    statusThirstBlock.Reception += ml * 4f * 15f;
                },
                healthUsable = false,
                injectable = true
            });
            LiquidRegistry.Register("cbd", new CustomLiquidInfo
            {
                name = "CBD",
                description = "A cannabinoid in oil form. Reduces pain and anxiety.",
                color = new Color(1.0f, 0.749f, 0.0f),
                valuePerLiter = 100,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    CBD statusCBD = body.GetStatus<CBD>();
                    statusCBD.Reception += ml / (20f / 25f);
                    //MultiplayerApi.Apply(body, "traumaAmount", -(0.35f * (ml * (20f / 25f))));
                    body.traumaAmount -= 0.35f * (ml * (20f / 25f));
                },
                healthUsable = false,
                injectable = false
            });
            LiquidRegistry.Register("thc", new CustomLiquidInfo
            {
                name = "THC",
                description = "A cannabinoid in oil form. Causes euphoria and makes you hunger.",
                color = new Color(1.0f, 0.749f, 0.0f),
                valuePerLiter = 100,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    THC statusTHC = body.GetStatus<THC>();
                    statusTHC.Reception += ml * (20f / 25f);
                    //MultiplayerApi.Apply(body, "happiness", 0.33f * (ml * (20f / 25f)));
                    //MultiplayerApi.Apply(body, "hunger", -(0.25f * (ml * (20f / 25f))));
                    body.happiness += 0.33f * (ml * (20f / 25f));
                    body.hunger -= 0.25f * (ml * (20f / 25f));
                },
                healthUsable = false,
                injectable = false
            });
            LiquidRegistry.Register("testosterone", new CustomLiquidInfo
            {
                name = "Testosterone",
                description = "An anabolic steroid. Makes you stronger over time. Less effective if drunk.",
                color = new Color(0.659f, 0.196f, 0.306f),
                valuePerLiter = 100,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    Steroid status = body.GetStatus<Steroid>();
                    status.Reception += ml / 200 * 15;
                    status.Abuse += ml / 200 * 15 * 0.5f;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    Steroid status = body.GetStatus<Steroid>();
                    status.Reception += ml / 100 * 15;
                    status.Abuse += ml / 100 * 15 * 0.5f;
                },
                healthUsable = false,
                injectable = true
            });
                                                                                                                                                                                                                                                                                    LiquidRegistry.Register("clonidine", new CustomLiquidInfo
            {
                name = "Clonidine",
                description = "Potent sedative and alpha-2 agonist. Also lowers blood pressure and provides pain relief. The safe dose is extremely low. Consider diluting. If it doesn't work, up the dose.",
                color = new Color(0.969f, 0.506f, 0.855f),
                valuePerLiter = 110,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    Alpha2 status = body.GetStatus<Alpha2>();
                    status.Reception += ml * 10f * 19f;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    Alpha2 status = body.GetStatus<Alpha2>();
                    status.Reception += ml * 10f * 19f;
                },
                healthUsable = false,
                injectable = true
            });
            LiquidRegistry.Register("diazepam", new CustomLiquidInfo
            {
                name = "Diazepam",
                description = "Potent benzodiazepine sedative. Hard to overdose on, but highly addictive. If it doesn't work, up the dose.",
                color = new Color(0.227f, 0.741f, 0.435f),
                valuePerLiter = 100,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    Benzo status = body.GetStatus<Benzo>();
                    status.Reception += ml * (20f / 10f);
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    Benzo status = body.GetStatus<Benzo>();
                    status.Reception += ml * (20f / 10f);
                },
                healthUsable = false,
                injectable = true
            });
            LiquidRegistry.Register("phenobarbital", new CustomLiquidInfo
            {
                name = "Phenobarbital",
                description = "Highly potent barbiturate sedative. Much stronger than Diazepam. If it doesn't work, up to the dose.",
                color = new Color(0.522f, 0.659f, 0.196f),
                valuePerLiter = 120,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    Barbiturate status = body.GetStatus<Barbiturate>();
                    status.Reception += ml * (28f / 30f);
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    Barbiturate status = body.GetStatus<Barbiturate>();
                    status.Reception += ml * (28f / 30f);
                },
                healthUsable = false,
                injectable = true
            });
            LiquidRegistry.Register("ketamine", new CustomLiquidInfo
            {
                name = "Ketamine",
                description = "Dissociative anaesthetic. Extremely effective at relieving pain.",
                color = new Color(0.596f, 0.690f, 0.839f),
                valuePerLiter = 120,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    Ketamine status = body.GetStatus<Ketamine>();
                    status.Reception += ml * 0.15f;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    Ketamine status = body.GetStatus<Ketamine>();
                    status.Reception += ml;
                },
                healthUsable = false,
                injectable = true
            });
            LiquidRegistry.Register("hydrochloricacid", new CustomLiquidInfo
            {
                name = "Hydrochloric Acid",
                description = "A powerful acid. Useful in chemistry.",
                color = new Color(0.529f, 0.941f, 0.396f),
                valuePerLiter = 100,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    //MultiplayerApi.Apply(body.limbs[0], "skinHealth", -(ml * 2));
                    //MultiplayerApi.Apply(body.limbs[0], "muscleHealth", - (ml * 2));
                    //MultiplayerApi.Apply(body.limbs[0], "pain", ml * 2);
                    body.limbs[0].skinHealth -= ml * 2;
                    body.limbs[0].muscleHealth -= ml * 2;
                    body.limbs[0].pain += ml * 2f;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    Limb[] limbs = body.limbs;
                    for (int i = 0; i < limbs.Length; i++)
                    {
                        Limb _limb = limbs[i];
                        //MultiplayerApi.Apply(_limb, "skinHealth", -(ml * 3));
                        //MultiplayerApi.Apply(_limb, "muscleHealth", -(ml * 3));
                        //MultiplayerApi.Apply(_limb, "pain", ml * 10);
                        _limb.skinHealth -= ml * 3f;
                        _limb.muscleHealth -= ml * 3f;
                        _limb.pain += ml * 10f;
                    }
                },
                qualities =
                {
                    new CraftingQuality("acid", 1f)
                },
                healthUsable = false,
                injectable = true
            });
            LiquidRegistry.Register("rootbeer", new CustomLiquidInfo
            {
                name = "Root Beer",
                description = "A sweet carbonated drink. Flavoured with sassafras oil, a precursor to MDMA.",
                color = new Color(0.322f, 0.231f, 0.078f),
                valuePerLiter = 100,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    //MultiplayerApi.Apply(body, "thirst", ml / 100f * 10f);
                    body.thirst += ml / 100f * 10f;
                },
                healthUsable = false,
                injectable = false
            });
            LiquidRegistry.Register("acetone", new CustomLiquidInfo
            {
                name = "Acetone",
                description = "A sweet-smelling solvent.",
                color = new Color(0.878f, 0.667f, 0.298f),
                valuePerLiter = 100,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    //MultiplayerApi.Apply(body.limbs[0], "skinHealth", -(ml * 2f));
                    //MultiplayerApi.Apply(body.limbs[0], "muscleHealth", -(ml * 2f));
                    //MultiplayerApi.Apply(body, "immunity", -(ml * 3f));
                    //MultiplayerApi.Apply(body, "respiratoryRate", -6f);
                    body.limbs[0].skinHealth -= ml * 2;
                    body.limbs[0].muscleHealth -= ml * 2;
                    body.immunity -= ml * 3f;
                    body.respiratoryRate -= 5f;
                    Limb[] limbs = body.limbs;
                    for (int i = 0; i < limbs.Length; i++)
                    {
                        Limb limb = limbs[i];
                        //MultiplayerApi.Apply(limb, "pain", ml);
                        limb.pain += ml;
                    }
                    //MultiplayerApi.Apply(body, "sicknessAmount", ml * 4f);
                    body.sicknessAmount += ml * 4f;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    //MultiplayerApi.Apply(body, "immunity", -(ml * 5f));
                    //MultiplayerApi.Apply(body, "respiratoryRate", -(ml / 2f));
                    //MultiplayerApi.Apply(body, "sicknessAmount", ml * 10f);
                    body.immunity -= ml * 5f;
                    body.respiratoryRate -= ml / 2;
                    body.sicknessAmount += ml * 10;
                    Limb[] limbs = body.limbs;
                    for (int i = 0; i < limbs.Length; i++)
                    {
                        Limb _limb = limbs[i];
                        //MultiplayerApi.Apply(_limb, "pain", ml);
                        //MultiplayerApi.Apply(_limb, "skinHealth", -ml);
                        //MultiplayerApi.Apply(_limb, "muscleHealth", -ml);
                        _limb.pain += ml;
                        _limb.skinHealth -= ml;
                        _limb.muscleHealth -= ml;
                    }
                },
                healthUsable = false,
                injectable = true
            });
                                                                                                                                                                                                LiquidRegistry.Register("woodvinegar", new CustomLiquidInfo
            {
                name = "Wood Vinegar",
                description = "A dark, pungent, and highly acidic liquid collected from baking wood in an airtight container.",
                color = new Color(0.7216f, 0.451f, 0.2f),
                valuePerLiter = 40,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    //MultiplayerApi.Apply(body.limbs[0], "skinHealth", -(ml * 2f));
                    //MultiplayerApi.Apply(body.limbs[0], "muscleHealth", -(ml * 2f));
                    //MultiplayerApi.Apply(body.limbs[0], "pain", ml * 2f);
                    body.limbs[0].skinHealth -= ml * 2;
                    body.limbs[0].muscleHealth -= ml * 2;
                    body.limbs[0].pain += ml * 2f;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    Limb[] limbs = body.limbs;
                    for (int i = 0; i < limbs.Length; i++)
                    {
                        Limb _limb = limbs[i];
                        //MultiplayerApi.Apply(_limb, "skinHealth", -(ml * 3f));
                        //MultiplayerApi.Apply(_limb, "muscleHealth", -(ml * 3f));
                        //MultiplayerApi.Apply(_limb, "pain", ml * 10f);
                        _limb.skinHealth -= ml * 3f;
                        _limb.muscleHealth -= ml * 3f;
                        _limb.pain += ml * 10f;
                    }
                },
                healthUsable = false,
                injectable = true
            });
            
            LiquidRegistry.Register("saltpeter", new CustomLiquidInfo
            {
                name = "Saltpeter Solution",
                description = "Saltpeter crystals dissolved in water. Can act as an oxidiser during chemical synthesis.",
                color = new Color(0.7882f, 0.7294f, 0.6706f),
                valuePerLiter = 40,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    //MultiplayerApi.Apply(body, "sicknessAmount", ml / 3f);
                    body.sicknessAmount += ml / 3;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    //MultiplayerApi.Apply(body, "brainHealth", -(ml / 5f));
                    body.brainHealth -= ml / 5;
                },
                healthUsable = false,
                injectable = true
            });
            LiquidRegistry.Register("nitricacid", new CustomLiquidInfo
            {
                name = "Nitric Acid",
                description = "A crude acid produced with saltpeter and scrap metal. Useful in chemistry.",
                color = new Color(0.5686f, 0.1922f, 0.1647f),
                valuePerLiter = 100,
                onDrink = (ml, body) =>
                {
                    if (!CUCoreUtils.TryGetBody(out var localBody) || localBody != body)
                    {
                        return;
                    }
                    //MultiplayerApi.Apply(body.limbs[0], "skinHealth", -(ml * 2f));
                    //MultiplayerApi.Apply(body.limbs[0], "muscleHealth", -(ml * 2f));
                    //MultiplayerApi.Apply(body.limbs[0], "pain", ml * 2f);
                    body.limbs[0].skinHealth -= ml * 2;
                    body.limbs[0].muscleHealth -= ml * 2;
                    body.limbs[0].pain += ml * 2f;
                },
                onHealthUse = (ml, limb) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != limb.body)
                    {
                        return;
                    }
                    CUCoreUtils.TryGetBody(out Body body);
                    Limb[] limbs = body.limbs;
                    for (int i = 0; i < limbs.Length; i++)
                    {
                        Limb _limb = limbs[i];
                        //MultiplayerApi.Apply(_limb, "skinHealth", -(ml * 3f));
                        //MultiplayerApi.Apply(_limb, "muscleHealth", -(ml * 3f));
                        //MultiplayerApi.Apply(_limb, "pain", ml * 10f);
                        _limb.skinHealth -= ml * 3f;
                        _limb.muscleHealth -= ml * 3f;
                        _limb.pain += ml * 10f;
                    }
                },
                qualities =
                {
                    new CraftingQuality("acid", 1f)
                },
                healthUsable = false,
                injectable = true
            });

            ItemRegistry.Register("woodash", new ItemInfo
            {
                fullName = "Wood Ash",
                description = "Powdery, alkaline residue left behind from completely burning wood.",
                category = "custom",
                tags = "",
                weight = 0.02f,
                value = 2,
                usable = false,
                rec = new Recognition(3),
            }, AssetLoader.LoadEmbeddedSprite("Images.woodash.png"), 0);

                                                                                                                                                                                                                                                                                                
            ItemRegistry.Register("testosterone", new CustomItemInfo
            {
                fullName = "Testosterone Syringe",
                description = "A 100ml syringe containing an anabolic steroid in the form of testosterone. Makes you stronger over time but causes complications if overused.",
                category = "drug",
                tags = "medicine",
                destroyAtZeroCondition = false,
                scaleWeightWithCondition = true,
                weight = 0.25f,
                value = 22,
                rec = new Recognition(8),
                Syringe = new SyringeProperties
                {
                    Capacity = 100f,
                    AmountPerFullUse = 100f,
                    AutoFill = false,
                    DefaultContents =
                    {
                        new LiquidStack("testosterone", 100f)
                    }
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.testosteronesyringe.png"));

            ItemRegistry.Register("ketamine", new CustomItemInfo
            {
                fullName = "Ketamine Syringe",
                description = "A 100ml syringe containing ketamine, a dissociative. Provides highly effective pain relief but causes altered perception and a disconnect from reality. The overdose label reads 60ml.",
                category = "drug",
                tags = "medicine",
                destroyAtZeroCondition = false,
                scaleWeightWithCondition = true,
                weight = 0.22f,
                rec = new Recognition(7),
                Syringe = new SyringeProperties
                {
                    Capacity = 100f,
                    AmountPerFullUse = 100f,
                    AutoFill = false,
                    DefaultContents =
                    {
                        new LiquidStack("ketamine", 100f)
                    }
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.ketaminesyringe.png"));

            Sprite semaglutideSprite = AssetLoader.LoadEmbeddedSprite("Images.semaglutidesyringe.png");
            ItemRegistry.Register("semaglutide", new CustomItemInfo
            {
                fullName = "Semaglutide Syringe",
                description = "A 100ml syringe containing semaglutide, an appetite suppressant. Stops hunger from decaying but causes sickness. The overdose label reads 75ml.",
                category = "drug",
                tags = "medicine",
                slotRotation = -0f,
                destroyAtZeroCondition = false,
                scaleWeightWithCondition = true,
                weight = 0.25f,
                value = 15,
                rec = new Recognition(7),
                Syringe = new SyringeProperties
                {
                    Capacity = 100f,
                    AmountPerFullUse = 100f,
                    AutoFill = false,
                    DefaultContents =
                    {
                        new LiquidStack("semaglutide", 1f),
                        new LiquidStack("water", 99f)
                    }
                },
                SpawnFrequency = 1
            }, semaglutideSprite);

            Sprite vigatrodipsinSprite = AssetLoader.LoadEmbeddedSprite("Images.vigatrodipsinsyringe.png");
            ItemRegistry.Register("vigatrodipsin", new CustomItemInfo
            {
                fullName = "Vigatropdipsin Syringe",
                description = "A 100ml syringe containing vigatrodipsin, a thirst suppressant. Stops thirst from decaying but acts as a diuretic if you’re above 60 thirst, rapidly draining fluids.",
                category = "drug",
                tags = "medicine",
                slotRotation = -0f,
                destroyAtZeroCondition = false,
                scaleWeightWithCondition = true,
                weight = 0.25f,
                value = 15,
                rec = new Recognition(7),
                Syringe = new SyringeProperties
                {
                    Capacity = 100f,
                    AmountPerFullUse = 100f,
                    AutoFill = false,
                    DefaultContents =
                    {
                        new LiquidStack("vigatrodipsin", 1f),
                        new LiquidStack("water", 99f)
                    }
                }
            }, vigatrodipsinSprite);

            ItemRegistry.Register("clonidine", new CustomItemInfo
            {
                fullName = "Clonidine Syringe",
                description = "A 100ml syringe containing clonidine, an alpha-2 agonist. Lets you sleep regardless of your physical condition and provides minor pain relief. If it doesn't work, up the dose. Don’t combine with other drugs. Moderately effective but lowers blood pressure. The overdose label reads 30ml.",
                category = "drug",
                tags = "medicine",
                slotRotation = -0f,
                destroyAtZeroCondition = false,
                scaleWeightWithCondition = true,
                weight = 0.25f,
                value = 19,
                rec = new Recognition(9),
                Syringe = new SyringeProperties
                {
                    Capacity = 100f,
                    AmountPerFullUse = 100f,
                    AutoFill = false,
                    DefaultContents =
                    {
                        new LiquidStack("clonidine", 1f),
                        new LiquidStack("water", 99f)
                    }
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.clonidinesyringe.png"));
            
            ItemRegistry.Register("diazepam", new CustomItemInfo
            {
                fullName = "Diazepam Syringe",
                description = "A 100ml syringe containing diazepam, a benzodiazepine. Lets you sleep regardless of your physical condition. If it doesn't work, up the dose. Don't combine with other drugs. Moderately effective but highly addictive. The overdose label reads 67ml.",
                category = "drug",
                tags = "medicine",
                slotRotation = -0f,
                destroyAtZeroCondition = false,
                scaleWeightWithCondition = true,
                weight = 0.25f,
                value = 18,
                rec = new Recognition(8),
                Syringe = new SyringeProperties
                {
                    Capacity = 100f,
                    AmountPerFullUse = 100f,
                    AutoFill = false,
                    DefaultContents =
                    {
                        new LiquidStack("diazepam", 50f),
                        new LiquidStack("water", 50f)
                    }
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.diazepamsyringe.png"));

            ItemRegistry.Register("phenobarbital", new CustomItemInfo
            {
                fullName = "Phenobarbital Syringe",
                description = "A 100ml syringe containing phenobarbital, a barbiturate. Lets you sleep regardless of your physical condition. If it doesn't work, up the dose. Don't combine with other drugs. Highly effective, but causes extreme complications if you take too much. The overdose label reads 55ml.",
                category = "drug",
                tags = "medicine",
                slotRotation = -0f,
                destroyAtZeroCondition = false,
                scaleWeightWithCondition = true,
                weight = 0.25f,
                value = 20,
                rec = new Recognition(8),
                Syringe = new SyringeProperties
                {
                    Capacity = 100f,
                    AmountPerFullUse = 100f,
                    AutoFill = false,
                    DefaultContents =
                    {
                        new LiquidStack("phenobarbital", 100f)
                    }
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.phenobarbitalsyringe.png"));

            ItemRegistry.Register("mdma", new ItemInfo
            {
                fullName = "MDMA",
                description = "A small plastic packet containing three 100mg pills of MDMA, an entactogen. Boosts energy and mood, along with making you more sociable and charismatic. The overdose label reads 200mg.",
                category = "drug",
                tags = "medicine",
                weight = 0.2f,
                scaleWeightWithCondition = true,
                value = 20,
                usable = true,
                rec = new Recognition(7),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    MDMA status = body.GetStatus<MDMA>();
                    status.Reception += 30f;
                    //MultiplayerApi.ApplyAbsolute(body, "energy", 100f);
                    //MultiplayerApi.Apply(body, "happiness", 30f * 0.5f);
                    body.energy = 100f;
                    body.happiness += 30f * 0.5f;
                    item.condition -= 1f / 3f;
                }
            }, AssetLoader.LoadEmbeddedSprite("mdma.png"));

            Sprite hydrochlorothiazideSprite = AssetLoader.LoadEmbeddedSprite("Images.hydrochlorothiazidebottle.png");
            ItemRegistry.Register("hydrochlorothiazide", new ItemInfo
            {
                fullName = "Hydrochlorothiazide Bottle",
                description = "A small plastic bottle containing ten hydrochlorothiazide pills, a diuretic. Rapidly drains fluid from your body. Useful if you’re drowning in it, dangerous if you’re not.",
                category = "drug",
                combineable = true,
                weight = 0.5f,
                scaleWeightWithCondition = true,
                value = 12,
                usable = true,
                tags = "medicine",
                rec = new Recognition(2),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Diuretic status = body.GetStatus<Diuretic>();
                    status.Reception += 15f;
                    item.condition -= 1f / 10f;
                }
            }, hydrochlorothiazideSprite);

            ItemRegistry.Register("diphenhydramine", new ItemInfo
            {
                fullName = "Diphenhydramine Pack",
                description = "A blister pack of twelve 15mg diphenhydramine tablets, a sedative antihistamine. Lets you sleep regardless of your physical condition. If it doesn't work, up the dose. Don’t combine with other drugs. Mildly effective at standard doses. In very large doses, highly effective but toxic. The overdose label reads 150mg.",
                category = "drug",
                weight = 0.55f,
                value = 13,
                scaleWeightWithCondition = true,
                usable = true,
                tags = "medicine",
                rec = new Recognition(2),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Antihistamine status = body.GetStatus<Antihistamine>();
                    status.Reception += 10f;
                    item.condition -= 1f / 12f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.diphenhydramine.png"));

            ItemRegistry.Register("clonidinebottle", new ItemInfo
            {
                fullName = "Clonidine Bottle",
                description = "A small plastic bottle containing eight 0.1mg clonidine pills, an alpha-2 agonist. Lets you sleep regardless of your physical condition and provides minor pain relief. If it doesn't work, up the dose. Don’t combine with other drugs. Moderately effective but lowers blood pressure. The overdose label reads 0.3mg.",
                category = "drug",
                combineable = true,
                weight = 0.1f,
                scaleWeightWithCondition = true,
                value = 16,
                usable = true,
                tags = "medicine",
                rec = new Recognition(3),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Alpha2 status = body.GetStatus<Alpha2>();
                    status.Reception += 19f;
                    item.condition -= 1f / 8f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.clonidinebottle.png"));

            ItemRegistry.Register("diazepambottle", new ItemInfo
            {
                fullName = "Diazepam Bottle",
                description = "A small plastic bottle containing eight 10mg diazepam pills, a benzodiazepine. Lets you sleep regardless of your physical condition. If it doesn't work, up the dose. Don't combine with other drugs. Moderately effective but highly addictive. The overdose label reads 40mg.",
                category = "drug",
                combineable = true,
                weight = 0.25f,
                scaleWeightWithCondition = true,
                value = 15,
                usable = true,
                tags = "medicine",
                rec = new Recognition(2),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Benzo status = body.GetStatus<Benzo>();
                    status.Reception += 20f;
                    item.condition -= 1f / 8f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.diazepambottle.png"));

            ItemRegistry.Register("phenobarbitalbottle", new ItemInfo
            {
                fullName = "Phenobarbital Bottle",
                description = "A small plastic bottle containing five 30mg phenobarbital pills, a barbiturate. Lets you sleep regardless of your physical condition. If it doesn't work, up the dose. Don't combine with other drugs. Highly effective, but causes extreme complications if you take too much. The overdose label reads 55mg.",
                category = "drug",
                combineable = true,
                weight = 0.3f,
                scaleWeightWithCondition = true,
                value = 20,
                usable = true,
                tags = "medicine",
                rec = new Recognition(4),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Barbiturate status = body.GetStatus<Barbiturate>();
                    status.Reception += 28f;
                    item.condition -= 1f / 5f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.phenobarbitalbottle.png"));

                                                                                                                                                                                                                                    
            Sprite jointSprite = AssetLoader.LoadEmbeddedSprite("Images.joint.png", 10f);
            ItemRegistry.Register("joint", new ItemInfo
            {
                fullName = "Joint",
                description = "A small cardboard roach with rolling paper wrapped around it, full of cannabis. The tip features chemicals that remind you of matches. You could probably strike it with your claws.",
                category = "drug",
                weight = 0.05f,
                value = 17,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(5),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    CBD statusCBD = body.GetStatus<CBD>();
                    THC statusTHC = body.GetStatus<THC>();
                    string[] lines = new string[0];
                    if (item.isWet)
                    {
                        lines = new string[]
                        {
                            "Won't light...",
                            "Not dry enough.",
                            "Gotta wait for it to dry off...",
                            "It's completely soaked."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        return;
                    }
                    System.Random rand = new System.Random();
                    int strain = rand.Next(1, 5);
                    float thc = 0;
                    float cbd = 0;
                    if (strain == 1)
                    {
                        thc = 70;
                        cbd = 5;
                    }
                    else if (strain == 2)
                    {
                        thc = 40;
                        cbd = 40;
                    }
                    else if (strain == 3)
                    {
                        thc = 5;
                        cbd = 70;
                    }
                    else if (strain == 4)
                    {
                        thc = 25;
                        cbd = 15;
                    }
                    statusTHC.Reception += thc;
                    statusCBD.Reception += cbd;
                    //MultiplayerApi.Apply(body, "happiness", 0.33f * thc);
                    //MultiplayerApi.Apply(body, "hunger", -(0.25f * thc));
                    //MultiplayerApi.Apply(body, "traumaAmount", -(0.35f * cbd));
                    //MultiplayerApi.Apply(body, "stamina", -30f);
                    body.happiness += 0.33f * thc;
                    body.hunger -= 0.25f * thc;
                    body.traumaAmount -= 0.35f * cbd;
                    body.stamina -= 30;
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
                    body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    CUCoreUtils.PlaySoundAt(
                        AssetLoader.GetCachedAudioClip("subex.smoke"),
                        volume: 1f,
                        delay: 0.1f,
                        position: body.transform.position,
                        pitch: 1f
                    );
                    item.condition = 0f;
                }
            }, jointSprite);

            ItemRegistry.Register("cannabiscigarette", new ItemInfo
            {
                fullName = "Cannabis Cigarette",
                description = "A standard cigarette with a shrivelling filter, only filled with cannabis instead of tobacco. The filter blocks a portion of the cannabinoids, leading to a lower overall dose compared to joints. The tip features chemicals that remind you of matches. You could probably strike it with your claws.",
                category = "drug",
                weight = 0.05f,
                value = 18,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(6),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    CBD statusCBD = body.GetStatus<CBD>();
                    THC statusTHC = body.GetStatus<THC>();
                    string[] lines = new string[0];
                    if (item.isWet)
                    {
                        lines = new string[]
                        {
                            "Won't light...",
                            "Not dry enough.",
                            "Gotta wait for it to dry off...",
                            "It's completely soaked."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        return;
                    }
                    System.Random rand = new System.Random();
                    int strain = rand.Next(1, 5);
                    float thc = 0;
                    float cbd = 0;
                    if (strain == 1)
                    {
                        thc = 70;
                        cbd = 5;
                    }
                    else if (strain == 2)
                    {
                        thc = 40;
                        cbd = 40;
                    }
                    else if (strain == 3)
                    {
                        thc = 5;
                        cbd = 70;
                    }
                    else if (strain == 4)
                    {
                        thc = 25;
                        cbd = 15;
                    }
                    statusTHC.Reception += thc * 0.75f;
                    statusCBD.Reception += cbd * 0.75f;
                    //MultiplayerApi.Apply(body, "happiness", 0.33f * (thc * 0.75f));
                    //MultiplayerApi.Apply(body, "hunger", -(0.25f * (thc * 0.75f)));
                    //MultiplayerApi.Apply(body, "traumaAmount", -(0.35f * (cbd * 0.75f)));
                    //MultiplayerApi.Apply(body, "stamina", -30f);
                    body.happiness += 0.33f * (thc * 0.75f);
                    body.hunger -= 0.25f * (thc * 0.75f);
                    body.traumaAmount -= 0.35f * (cbd * 0.75f);
                    body.stamina -= 30;
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
                    body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    CUCoreUtils.PlaySoundAt(
                        AssetLoader.GetCachedAudioClip("subex.smoke"),
                        volume: 1f,
                        delay: 0.1f,
                        position: body.transform.position,
                        pitch: 1f
                    );
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.cigarette.png", 10f), 0);

            ItemRegistry.Register("tobaccojoint", new ItemInfo
            {
                fullName = "Tobacco Joint",
                description = "A small cardboard roach with rolling paper wrapped around it, filled with tobacco. Delivers more tar due to the lack of a filter. The tip features chemicals that remind you of matches. You could probably strike it with your claws.",
                category = "drug",
                weight = 0.05f,
                value = 3,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(4),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Nicotine status = body.GetStatus<Nicotine>();
                    if (item.isWet)
                    {
                        if (status.Tolerance < 8)
                        {
                            string[] lines = new string[]
                            {
                                "Won't light...",
                                "Not dry enough.",
                                "Gotta wait for it to dry off...",
                                "It's completely soaked."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (status.Tolerance < 20)
                        {
                            string[] lines = new string[]
                            {
                                "It's wet... what a shame.",
                                "Can't this thing dry any faster?",
                                "Shouldn't have got it wet..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (status.Tolerance < 35)
                        {
                            string[] lines = new string[]
                            {
                                "Fuck... it's wet.",
                                "Wet... damn it, I need this..!",
                                "I can't afford to wait for it to dry.",
                                "Shit, it's soaked..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else
                        {
                            string[] lines = new string[]
                            {
                                "no, no, dry off, dry off..!",
                                "Shit..! I need this, why is it soaked?!",
                                "DAMN IT! I NEED IT DRY!!",
                                "It's soaked? But I can't wait any longer..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        return;
                    }
                    status.Reception += 50f;
                    if (status.Tolerance >= 15f && status.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 50f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 50f);
                        body.happiness += 0.15f * 50f;
                        body.energy += 0.325f * 50f;
                    }
                    else if (status.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 50f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 50f);
                        body.happiness += 0.25f * 50f;
                        body.energy += 0.725f * 50f;
                    }
                    status.StaminaReduction += 0.02f * 100f;
                    //MultiplayerApi.Apply(body, "stamina", -30f);
                    body.stamina -= 30;
                    if (status.Tolerance < 8)
                    {
                        string[] lines = new string[]
                        {
                            "*cough* *cough*",
                            "This tastes weird.",
                            "Augh.. that's way too hot!",
                            "Isn't this stuff bad for you..?",
                            "I think some of it blew out of my nose.",
                            "How do people do this every day?"
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else if (status.Tolerance < 20)
                    {
                        string[] lines = new string[]
                        {
                            "Ohhh... I see why people like this!",
                            "I could get used to this.",
                            "So energising!",
                            "Whew... that's nice.",
                            "I'm staring to like this.",
                            "That's the good stuff..."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else
                    {
                        string[] lines = new string[]
                        {
                            "Ahhh... that's better.",
                            "Fuuuck, I really needed that.",
                            "Finally feeling normal again...",
                            "Don't know what I'd do without my fix.",
                            "Warm ember... much better.",
                            "Smoke in my lungs clears the head.",
                            "I'm so pathetic...",
                            "I need to stop..."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    CUCoreUtils.PlaySoundAt(
                        AssetLoader.GetCachedAudioClip("subex.smoke"),
                        volume: 1f,
                        delay: 0.1f,
                        position: body.transform.position,
                        pitch: 1f
                    );
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("tobaccojoint.png", 10f));
            
            ItemRegistry.Register("emptyjoint", new ItemInfo
            {
                fullName = "Empty Joint",
                description = "A small cardboard roach with rolling paper wrapped around it, only empty.",
                category = "medical",
                weight = 0.025f,
                value = 1,
                usable = false,
                tags = "medicine,cangetwet",
                rec = new Recognition(4)
            }, AssetLoader.LoadEmbeddedSprite("emptyjoint.png", 10f), 0);

            ItemRegistry.Register("spliff", new ItemInfo
            {
                fullName = "Spliff",
                description = "A small cardboard roach with rolling paper wrapped around it, full of a mix of cannabis and tobacco. The tip features chemicals that remind you of matches. You could probably strike it with your claws.",
                category = "drug",
                weight = 0.05f,
                value = 20,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(6),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    CBD statusCBD = body.GetStatus<CBD>();
                    THC statusTHC = body.GetStatus<THC>();
                    Nicotine statusNicotine = body.GetStatus<Nicotine>();
                    string[] lines = new string[0];
                    if (item.isWet)
                    {
                        if (statusNicotine.Tolerance < 8)
                        {
                            lines = new string[]
                            {
                                "Won't light...",
                                "Not dry enough.",
                                "Gotta wait for it to dry off...",
                                "It's completely soaked."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (statusNicotine.Tolerance < 20)
                        {
                            lines = new string[]
                            {
                                "It's wet... what a shame.",
                                "Can't this thing dry any faster?",
                                "Shouldn't have got it wet..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (statusNicotine.Tolerance < 35)
                        {
                            lines = new string[]
                            {
                                "Fuck... it's wet.",
                                "Wet... damn it, I need this..!",
                                "I can't afford to wait for it to dry.",
                                "Shit, it's soaked..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else
                        {
                            lines = new string[]
                            {
                                "no, no, dry off, dry off..!",
                                "Shit..! I need this, why is it soaked?!",
                                "DAMN IT! I NEED IT DRY!!",
                                "It's soaked? But I can't wait any longer..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        return;
                    }
                    System.Random rand = new System.Random();
                    int strain = rand.Next(1, 5);
                    float thc = 0;
                    float cbd = 0;
                    if (strain == 1)
                    {
                        thc = 70;
                        cbd = 5;
                    }
                    else if (strain == 2)
                    {
                        thc = 40;
                        cbd = 40;
                    }
                    else if (strain == 3)
                    {
                        thc = 5;
                        cbd = 70;
                    }
                    else if (strain == 4)
                    {
                        thc = 25;
                        cbd = 15;
                    }
                    statusTHC.Reception += thc;
                    statusCBD.Reception += cbd;
                    //MultiplayerApi.Apply(body, "happiness", 0.33f * thc);
                    //MultiplayerApi.Apply(body, "hunger", -(0.25f * thc));
                    //MultiplayerApi.Apply(body, "traumaAmount", -(0.35f * cbd));
                    //MultiplayerApi.Apply(body, "stamina", -30f);
                    body.happiness += 0.33f * thc;
                    body.hunger -= 0.25f * thc;
                    body.traumaAmount -= 0.35f * cbd;
                    body.stamina -= 30;
                    statusNicotine.Reception += 50f;
                    if (statusNicotine.Tolerance >= 15f && statusNicotine.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 50f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 50f);
                        body.happiness += 0.15f * 50f;
                        body.energy += 0.325f * 50f;
                    }
                    else if (statusNicotine.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 50f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 50f);
                        body.happiness += 0.25f * 50f;
                        body.energy += 0.725f * 50f;
                    }
                    statusNicotine.StaminaReduction += 0.02f * 100f;
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
                    body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    CUCoreUtils.PlaySoundAt(
                        AssetLoader.GetCachedAudioClip("subex.smoke"),
                        volume: 1f,
                        delay: 0.1f,
                        position: body.transform.position,
                        pitch: 1f
                    );
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("spinjoint.png", 10f));

            Sprite cigaretteSprite = AssetLoader.LoadEmbeddedSprite("Images.cigarette.png", 10f);
            ItemRegistry.Register("cigarette", new ItemInfo
            {
                fullName = "Cigarette",
                description = "A standard cigarette with a shrivelling filter. The chemicals on the tip remind you of matches. You could probably strike it with your claws.",
                category = "drug",
                weight = 0.05f,
                value = 3,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(3),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Nicotine status = body.GetStatus<Nicotine>();
                    string[] lines = new string[0];
                    if (item.isWet)
                    {
                        if (status.Tolerance < 8)
                        {
                            lines = new string[]
                            {
                                "Won't light...",
                                "Not dry enough.",
                                "Gotta wait for it to dry off...",
                                "It's completely soaked."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (status.Tolerance < 20)
                        {
                            lines = new string[]
                            {
                                "It's wet... what a shame.",
                                "Can't this thing dry any faster?",
                                "Shouldn't have got it wet..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (status.Tolerance < 35)
                        {
                            lines = new string[]
                            {
                                "Fuck... it's wet.",
                                "Wet... damn it, I need this..!",
                                "I can't afford to wait for it to dry.",
                                "Shit, it's soaked..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else
                        {
                            lines = new string[]
                            {
                                "no, no, dry off, dry off..!",
                                "Shit..! I need this, why is it soaked?!",
                                "DAMN IT! I NEED IT DRY!!",
                                "It's soaked? But I can't wait any longer..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        return;
                    }
                    status.Reception += 50f;
                    if (status.Tolerance >= 15f && status.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 50f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 50f);
                        body.happiness += 0.15f * 50f;
                        body.energy += 0.325f * 50f;
                    }
                    else if (status.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 50f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 50f);
                        body.happiness += 0.25f * 50f;
                        body.energy += 0.725f * 50f;
                    }
                    status.StaminaReduction += 0.02f * 50f;
                    //MultiplayerApi.Apply(body, "stamina", -30f);
                    body.stamina -= 30;
                    if (status.Tolerance < 8)
                    {
                        lines = new string[]
                        {
                            "*cough* *cough*",
                            "This tastes weird.",
                            "Augh.. that's way too hot!",
                            "Isn't this stuff bad for you..?",
                            "I think some of it blew out of my nose.",
                            "How do people do this every day?"
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else if (status.Tolerance < 20)
                    {
                        lines = new string[]
                        {
                            "Ohhh... I see why people like this!",
                            "I could get used to this.",
                            "So energising!",
                            "Whew... that's nice.",
                            "I'm staring to like this.",
                            "That's the good stuff..."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else
                    {
                        lines = new string[]
                        {
                            "Ahhh... that's better.",
                            "Fuuuck, I really needed that.",
                            "Finally feeling normal again...",
                            "Don't know what I'd do without my fix.",
                            "Warm ember... much better.",
                            "Smoke in my lungs clears the head.",
                            "I'm so pathetic...",
                            "I need to stop..."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    CUCoreUtils.PlaySoundAt(
                        AssetLoader.GetCachedAudioClip("subex.smoke"),
                        volume: 1f,
                        delay: 0.1f,
                        position: body.transform.position,
                        pitch: 1f
                    );
                    item.condition = 0f;
                }
            }, cigaretteSprite);

            ItemRegistry.Register("spincigarette", new ItemInfo
            {
                fullName = "Spin Cigarette",
                description = "A standard cigarette with a shrivelling filter, filled with a mix of cannabis and tobacco. The filter blocks a portion of the cannabinoids, leading to a lower total dose compared to joints. The tip features chemicals that remind you of matches. You could probably strike it with your claws.",
                category = "drug",
                value = 8,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(6),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    CBD statusCBD = body.GetStatus<CBD>();
                    THC statusTHC = body.GetStatus<THC>();
                    Nicotine statusNicotine = body.GetStatus<Nicotine>();
                    string[] lines = new string[0];
                    if (item.isWet)
                    {
                        if (statusNicotine.Tolerance < 8)
                        {
                            lines = new string[]
                            {
                                "Won't light...",
                                "Not dry enough.",
                                "Gotta wait for it to dry off...",
                                "It's completely soaked."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (statusNicotine.Tolerance < 20)
                        {
                            lines = new string[]
                            {
                                "It's wet... what a shame.",
                                "Can't this thing dry any faster?",
                                "Shouldn't have got it wet..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (statusNicotine.Tolerance < 35)
                        {
                            lines = new string[]
                            {
                                "Fuck... it's wet.",
                                "Wet... damn it, I need this..!",
                                "I can't afford to wait for it to dry.",
                                "Shit, it's soaked..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else
                        {
                            lines = new string[]
                            {
                                "no, no, dry off, dry off..!",
                                "Shit..! I need this, why is it soaked?!",
                                "DAMN IT! I NEED IT DRY!!",
                                "It's soaked? But I can't wait any longer..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        return;
                    }
                    System.Random rand = new System.Random();
                    int strain = rand.Next(1, 5);
                    float thc = 0;
                    float cbd = 0;
                    if (strain == 1)
                    {
                        thc = 70;
                        cbd = 5;
                    }
                    else if (strain == 2)
                    {
                        thc = 40;
                        cbd = 40;
                    }
                    else if (strain == 3)
                    {
                        thc = 5;
                        cbd = 70;
                    }
                    else if (strain == 4)
                    {
                        thc = 25;
                        cbd = 15;
                    }
                    statusTHC.Reception += thc * 0.75f;
                    statusCBD.Reception += cbd * 0.75f;
                    //MultiplayerApi.Apply(body, "happiness", 0.33f * (thc * 0.75f));
                    //MultiplayerApi.Apply(body, "hunger", -(0.25f * (thc * 0.75f)));
                    //MultiplayerApi.Apply(body, "traumaAmount", -(0.35f * (cbd * 0.75f)));
                    //MultiplayerApi.Apply(body, "stamina", -30f);
                    body.happiness += 0.33f * (thc * 0.75f);
                    body.hunger -= 0.25f * (thc * 0.75f);
                    body.traumaAmount -= 0.35f * (cbd * 0.75f);
                    body.stamina -= 30;
                    statusNicotine.Reception += 50f;
                    if (statusNicotine.Tolerance >= 15f && statusNicotine.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 50f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 50f);
                        body.happiness += 0.15f * 50f;
                        body.energy += 0.325f * 50f;
                    }
                    else if (statusNicotine.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 50f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 50f);
                        body.happiness += 0.25f * 50f;
                        body.energy += 0.725f * 50f;
                    }
                    statusNicotine.StaminaReduction += 0.02f;
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
                    body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    CUCoreUtils.PlaySoundAt(
                        AssetLoader.GetCachedAudioClip("subex.smoke"),
                        volume: 1f,
                        delay: 0.1f,
                        position: body.transform.position,
                        pitch: 1f
                    );
                    item.condition = 0f;
                }
            }, cigaretteSprite, 0);

            ItemRegistry.Register("emptycigarette", new ItemInfo
            {
                fullName = "Empty Cigarette",
                description = "A standard cigarette with a shrivelling filter, only empty.",
                category = "medical",
                weight = 0.025f,
                value = 1,
                usable = false,
                tags = "medicine,cangetwet",
                rec = new Recognition(3)
            }, cigaretteSprite, 0);

            Sprite cigarSprite = AssetLoader.LoadEmbeddedSprite("Images.cigar.png");
            ItemRegistry.Register("cigar", new ItemInfo
            {
                fullName = "Cigar",
                description = "A fancy-looking cigar with a visibly aged wrap. Provides more nicotine than a cigarette. The tip has a chemical that reminds you of matches. You could probably strike it with your claws.",
                category = "drug",
                weight = 0.2f,
                value = 23,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(4),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Nicotine status = body.GetStatus<Nicotine>();
                    string[] lines = new string[0];
                    if (item.isWet)
                    {
                        if (status.Tolerance < 8)
                        {
                            lines = new string[]
                            {
                                "Won't light...",
                                "Not dry enough.",
                                "Gotta wait for it to dry off...",
                                "It's completely soaked."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (status.Tolerance < 20)
                        {
                            lines = new string[]
                            {
                                "It's wet... what a shame.",
                                "Can't this thing dry any faster?",
                                "Shouldn't have got it wet..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (status.Tolerance < 35)
                        {
                            lines = new string[]
                            {
                                "Fuck... it's wet.",
                                "Wet... damn it, I need this..!",
                                "I can't afford to wait for it to dry.",
                                "Shit, it's soaked..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else
                        {
                            lines = new string[]
                            {
                                "no, no, dry off, dry off..!",
                                "Shit..! I need this, why is it soaked?!",
                                "DAMN IT! I NEED IT DRY!!",
                                "It's soaked? But I can't wait any longer..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        return;
                    }
                    status.Reception += 100f;
                    if (status.Tolerance >= 15f && status.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 100f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 50f);
                        body.happiness += 0.15f * 100f;
                        body.energy += 0.325f * 50f;
                    }
                    else if (status.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 100f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 100f);
                        body.happiness += 0.25f * 100f;
                        body.energy += 0.725f * 100f;
                    }
                    status.StaminaReduction += 0.02f * 100f;
                    //MultiplayerApi.Apply(body, "stamina", -50f);
                    body.stamina -= 50;
                    if (status.Tolerance < 8)
                    {
                        lines = new string[]
                        {
                            "*cough* *cough*",
                            "This tastes weird.",
                            "Augh.. that's way too hot!",
                            "Isn't this stuff bad for you..?",
                            "I think some of it blew out of my nose.",
                            "How do people do this every day?"
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else if (status.Tolerance < 20)
                    {
                        lines = new string[]
                        {
                            "Ohhh... I see why people like this!",
                            "I could get used to this.",
                            "So energising!",
                            "Whew... that's nice.",
                            "I'm staring to like this.",
                            "That's the good stuff..."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else
                    {
                        lines = new string[]
                        {
                            "Ahhh... that's better.",
                            "Fuuuck, I really needed that.",
                            "Finally feeling normal again...",
                            "Don't know what I'd do without my fix.",
                            "Warm ember... much better.",
                            "Smoke in my lungs clears the head.",
                            "I'm so pathetic...",
                            "I need to stop..."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    CUCoreUtils.PlaySoundAt(
                        AssetLoader.GetCachedAudioClip("subex.smoke"),
                        volume: 1f,
                        delay: 0.1f,
                        position: body.transform.position,
                        pitch: 1f
                    );
                    item.condition = 0f;
                }
            }, cigarSprite);

            ItemRegistry.Register("blunt", new ItemInfo
            {
                fullName = "Blunt",
                description = "A fancy-looking cigar with a green-tinted wrap. There’s cannabis inside it. Provides more nicotine than a cigarette and more cannabinoids than a joint. The tip features chemicals that remind you of matches. You could probably strike it with your claws.",
                category = "drug",
                weight = 0.2f,
                value = 25,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(7),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    CBD statusCBD = body.GetStatus<CBD>();
                    THC statusTHC = body.GetStatus<THC>();
                    Nicotine statusNicotine = body.GetStatus<Nicotine>();
                    string[] lines = new string[0];
                    if (item.isWet)
                    {
                        if (statusNicotine.Tolerance < 8)
                        {
                            lines = new string[]
                            {
                                "Won't light...",
                                "Not dry enough.",
                                "Gotta wait for it to dry off...",
                                "It's completely soaked."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (statusNicotine.Tolerance < 20)
                        {
                            lines = new string[]
                            {
                                "It's wet... what a shame.",
                                "Can't this thing dry any faster?",
                                "Shouldn't have got it wet..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else if (statusNicotine.Tolerance < 35)
                        {
                            lines = new string[]
                            {
                                "Fuck... it's wet.",
                                "Wet... damn it, I need this..!",
                                "I can't afford to wait for it to dry.",
                                "Shit, it's soaked..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        else
                        {
                            lines = new string[]
                            {
                                "no, no, dry off, dry off..!",
                                "Shit..! I need this, why is it soaked?!",
                                "DAMN IT! I NEED IT DRY!!",
                                "It's soaked? But I can't wait any longer..."
                            };
                            body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                        }
                        return;
                    }
                    System.Random rand = new System.Random();
                    int strain = rand.Next(1, 5);
                    float thc = 0;
                    float cbd = 0;
                    if (strain == 1)
                    {
                        thc = 70;
                        cbd = 5;
                    }
                    else if (strain == 2)
                    {
                        thc = 40;
                        cbd = 40;
                    }
                    else if (strain == 3)
                    {
                        thc = 5;
                        cbd = 70;
                    }
                    else if (strain == 4)
                    {
                        thc = 25;
                        cbd = 15;
                    }
                    statusTHC.Reception += thc * 1.5f;
                    statusCBD.Reception += cbd * 1.5f;
                    //MultiplayerApi.Apply(body, "happiness", 0.33f * (thc * 1.5f));
                    //MultiplayerApi.Apply(body, "hunger", -(0.25f * (thc * 1.5f)));
                    //MultiplayerApi.Apply(body, "traumaAmount", -(0.35f * (cbd * 1.5f)));
                    //MultiplayerApi.Apply(body, "stamina", -30f);
                    body.happiness += 0.33f * (thc * 1.5f);
                    body.hunger -= 0.25f * (thc * 1.5f);
                    body.traumaAmount -= 0.35f * (cbd * 1.5f);
                    body.stamina -= 50;
                    statusNicotine.Reception += 80f;
                    if (statusNicotine.Tolerance >= 15f && statusNicotine.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 80f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 80f);
                        body.happiness += 0.15f * 80f;
                        body.energy += 0.325f * 80f;
                    }
                    else if (statusNicotine.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 80f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 80f);
                        body.happiness += 0.25f * 80f;
                        body.energy += 0.725f * 80f;
                    }
                    statusNicotine.StaminaReduction += 0.02f * 80f;
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
                    body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    CUCoreUtils.PlaySoundAt(
                        AssetLoader.GetCachedAudioClip("subex.smoke"),
                        volume: 1f,
                        delay: 0.1f,
                        position: body.transform.position,
                        pitch: 1f
                    );
                    item.condition = 0f;
                }
            }, cigarSprite);

            Sprite gumSprite = AssetLoader.LoadEmbeddedSprite("Images.nicotinegum.png");
            ItemRegistry.Register("nicotinegumcontainer", new ItemInfo
            {
                fullName = "Nicotine Gum Container",
                description = "A small plastic container holding grey-tinged pieces of gum. Less intense than a cigarette, but more intense than a patch. Good for managing withdrawals, as it doesn't render you out of breath.",
                category = "medical",
                combineable = true,
                weight = 0.25f,
                scaleWeightWithCondition = true,
                value = 20,
                usable = true,
                tags = "medicine",
                rec = new Recognition(4),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    body.AutoPickUpItem(Utils.Create("nicotinegum", body.transform.position, 0f).GetComponent<Item>());
                    item.condition -= 1f / 6f;
                }
            }, gumSprite);

            ItemRegistry.Register("nicotinegum", new ItemInfo
            {
                fullName = "Nicotine Gum",
                description = "A grey-tinged piece of gum. Less intense than a cigarette, but more intense than a patch. Good for managing withdrawals, as it doesn't render you out of breath.",
                category = "drug",
                weight = 0.05f,
                value = 2,
                usable = true,
                tags = "medicine",
                rec = new Recognition(4),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Nicotine status = body.GetStatus<Nicotine>();
                    status.Reception += 25f;
                    if (status.Tolerance >= 15f && status.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 25f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 25f);
                        body.happiness += 0.15f * 25f;
                        body.energy += 0.325f * 25f;
                    }
                    else if (status.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 25f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 25f);
                        body.happiness += 0.25f * 25f;
                        body.energy += 0.725f * 25f;
                    }
                    if (status.Tolerance < 8)
                    {
                        string[] lines = new string[]
                        {
                            "Huh... this feels weird.",
                            "I feel funny...",
                            "Should I really be having this?",
                            "They do say this is healthier than smoking.",
                            "Maybe I'll get used to it?",
                            "I feel it..!"
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else if (status.Tolerance < 20)
                    {
                        string[] lines = new string[]
                        {
                            "Ohhh... I see why people like this!",
                            "I could get used to this.",
                            "So energising!",
                            "Whew... that's nice.",
                            "I'm starting to like this."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else
                    {
                        string[] lines = new string[]
                        {
                            "At least it's healthier than smoke...",
                            "That should hold off the withdrawals for a bit.",
                            "I wish I didn't need this.",
                            "I'm so pathetic...",
                            "That's better.",
                            "Don't know what I'd do without my fix.",
                            "Fuuuck, I really needed that."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("nicotinegumitem.png"), 0);

            Sprite patchSprite = AssetLoader.LoadEmbeddedSprite("Images.nicotinepatch.png");
            ItemRegistry.Register("nicotinepatches", new ItemInfo
            {
                fullName = "Nicotine Patches",
                description = "A small bag containing a number of sticky nicotine patches. Good for managing withdrawals, as it doesn't render you out of breath. Doesn't have much nicotine in it. Apply to the skin.",
                category = "medical",
                combineable = true,
                weight = 0.15f,
                scaleWeightWithCondition = true,
                value = 21,
                usable = true,
                tags = "medicine",
                rec = new Recognition(5),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    body.AutoPickUpItem(Utils.Create("nicotinepatch", body.transform.position, 0f).GetComponent<Item>());
                    item.condition -= 1f / 10f;
                }
            }, patchSprite);

            ItemRegistry.Register("nicotinepatch", new ItemInfo
            {
                fullName = "Nicotine Patch",
                description = "A sticky nicotie patch. Good for managing withdrawals, as it doesn't render you out of breath. Doesn't have much nicotine in it. Apply to the skin.",
                category = "drug",
                weight = 0.01f,
                value = 2,
                usable = true,
                tags = "medicine",
                rec = new Recognition(5),
                ignoreDepression = true,
                useLimbAction = (limb, item) =>
                {
                    CUCoreUtils.TryGetBody(out Body body);
                    Nicotine status = body.GetStatus<Nicotine>();
                    status.Reception += 10f;
                    if (status.Tolerance >= 15f && status.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 10f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 10f);
                        body.happiness += 0.15f * 10f;
                        body.energy += 0.325f * 10f;
                    }
                    else if (status.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 10f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 10f);
                        body.happiness += 0.25f * 10f;
                        body.energy += 0.725f * 10f;
                    }
                    if (status.Tolerance < 8)
                    {
                        string[] lines = new string[]
                        {
                            "Huh... this feels weird.",
                            "I feel funny...",
                            "Should I really be having this?",
                            "They do say this is healthier than smoking.",
                            "Maybe I'll get used to it?",
                            "I feel it..!"
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else if (status.Tolerance < 20)
                    {
                        string[] lines = new string[]
                        {
                            "Ohhh... I see why people like this!",
                            "I could get used to this.",
                            "So energising!",
                            "Whew... that's nice.",
                            "I'm starting to like this."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    else
                    {
                        string[] lines = new string[]
                        {
                            "At least it's healthier than smoke...",
                            "That should hold off the withdrawals for a bit.",
                            "I wish I didn't need this.",
                            "I'm so pathetic...",
                            "That's better.",
                            "Don't know what I'd do without my fix.",
                            "Fuuuck, I really needed that."
                        };
                        body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    }
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("nicotinepatchitem.png"), 0);

            ItemRegistry.Register("cigarettes", new ItemInfo
            {
                fullName = "Cigarette Pack",
                description = "A pack of cigarettes stored in a warning-riddled cardboard case. They advertise themselves as having chemicals in the tip that can ignite against a matchbox. You could probably do it with your claws.",
                category = "medical",
                combineable = true,
                weight = 1f,
                scaleWeightWithCondition = true,
                value = 36,
                usable = true,
                tags = "medicine",
                rec = new Recognition(3),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    body.AutoPickUpItem(Utils.Create("cigarette", body.transform.position, 0f).GetComponent<Item>());
                    item.condition -= 1f / 12f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.legalcigarettes.png"));

            ItemRegistry.Register("illegalcigarettes", new ItemInfo
            {
                fullName = "Illegal Cigarettes",
                description = "A pack of cigarettes. The box is promoting tobacco as a healthy alternative to harder drugs. The chemicals on the tip remind you of matches. You could probably strike it with your claws.",
                category = "medical",
                combineable = true,
                weight = 1f,
                scaleWeightWithCondition = true,
                value = 36,
                usable = true,
                tags = "medicine",
                rec = new Recognition(3),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    body.AutoPickUpItem(Utils.Create("cigarette", body.transform.position, 0f).GetComponent<Item>());
                    item.condition -= 1f / 12f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.illegalcigarettes.png"));

            ItemRegistry.Register("tobacco", new ItemInfo
            {
                fullName = "Tobacco",
                description = "A brown, dry herb, full of nicotine. Not very tasty, but it’ll give you a kick.",
                category = "medical",
                weight = 0.025f,
                value = 1,
                decayMinutes = 15,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(5),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Nicotine status = body.GetStatus<Nicotine>();
                    status.Reception += 35f;
                    //MultiplayerApi.Apply(body, "happiness", -4f);
                    body.happiness -= 4;
                    if (status.Tolerance >= 15f && status.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 35f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 35f);
                        body.happiness += 0.15f * 35f;
                        body.energy += 0.325f * 35f;
                    }
                    else if (status.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 35f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 35f);
                        body.happiness += 0.25f * 35f;
                        body.energy += 0.725f * 35f;
                    }
                    body.talker.Talk(Locale.GetCharacter("eatBad"));
                    Sound.Play("eatCrunch", body.transform.position);
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.tobacco.png"));

            ItemRegistry.Register("cannabis", new ItemInfo
            {
                fullName = "Cannabis",
                description = "A green, sticky herb containing cannabinoids. Useless as a drug unless smoked, though you can eat it in an emergency. Highly unpalatable, but notably more nutritious than standard foliage and grants a microscopic boost to immunity.",
                category = "medical",
                weight = 0.025f,
                value = 2,
                decayMinutes = 15,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(6),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    //MultiplayerApi.Apply(body, "hunger", 10f);
                    //MultiplayerApi.Apply(body, "happiness", -2f);
                    //MultiplayerApi.Apply(body, "immunity", 1f);
                    body.hunger += 10;
                    body.happiness -= 2;
                    body.immunity += 1;
                    if (UnityEngine.Random.value <= 0.66)
                    {
                        //MultiplayerApi.Apply(body, "sicknessAmount", UnityEngine.Random.Range(10, 30));
                        body.sicknessAmount += UnityEngine.Random.Range(10, 30);
                    }
                    body.talker.Talk(Locale.GetCharacter("eatBad"));
                    Sound.Play("eatCrunch", body.transform.position);
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.cannabis.png"));

            ItemRegistry.Register("spin", new ItemInfo
            {
                fullName = "Spin",
                description = "A yellow-green herb mix with dark spots. Made up of tobacco and cannabis, containing both nicotine and cannabinoids. The cannabis won't have any effect unless you smoke it, but chewing spin will provide a small amount of nicotine, nutrients, and a microscopic boost to immunity in an emergency.",
                category = "medical",
                weight = 0.025f,
                value = 3,
                decayMinutes = 15,
                usable = true,
                tags = "medicine,cangetwet",
                rec = new Recognition(7),
                ignoreDepression = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Nicotine status = body.GetStatus<Nicotine>();
                    status.Reception += 35f;
                    //MultiplayerApi.Apply(body, "hunger", 10f);
                    //MultiplayerApi.Apply(body, "happiness", -4f);
                    //MultiplayerApi.Apply(body, "immunity", 1f);
                    body.hunger += 10;
                    body.happiness -= 4;
                    body.immunity += 1;
                    if (status.Tolerance >= 15f && status.Tolerance < 35f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.15f * 35f);
                        //MultiplayerApi.Apply(body, "energy", 0.325f * 35f);
                        body.happiness += 0.15f * 35f;
                        body.energy += 0.325f * 35f;
                    }
                    else if (status.Tolerance < 15f)
                    {
                        //MultiplayerApi.Apply(body, "happiness", 0.25f * 35f);
                        //MultiplayerApi.Apply(body, "energy", 0.725f * 35f);
                        body.happiness += 0.25f * 50f;
                        body.energy += 0.725f * 50f;
                    }
                    body.talker.Talk(Locale.GetCharacter("eatBad"));
                    Sound.Play("eatCrunch", body.transform.position);
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("Images.spin.png"));

            ItemRegistry.Register("candyedible", new ItemInfo
            {
                fullName = "Cannabis Gummy",
                description = "A small piece of candy with cannabis infused into it. With barely any mass to mask the drug, it tastes horrible.",
                category = "drug",
                tags = "medicine,cangetwet",
                weight = 0.15f,
                value = 9,
                decayMinutes = 240,
                usable = true,
                rec = new Recognition(7),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    CBD statusCBD = body.GetStatus<CBD>();
                    THC statusTHC = body.GetStatus<THC>();
                    System.Random rand = new System.Random();
                    int strain = rand.Next(1, 5);
                    float thc = 0;
                    float cbd = 0;
                    if (strain == 1)
                    {
                        thc = 70;
                        cbd = 5;
                    }
                    else if (strain == 2)
                    {
                        thc = 40;
                        cbd = 40;
                    }
                    else if (strain == 3)
                    {
                        thc = 5;
                        cbd = 70;
                    }
                    else if (strain == 4)
                    {
                        thc = 25;
                        cbd = 15;
                    }
                    statusTHC.Reception += thc * 1.25f;
                    statusCBD.Reception += cbd * 1.25f;
                    //MultiplayerApi.Apply(body, "happiness", 0.33f * (thc * 1.25f));
                    //MultiplayerApi.Apply(body, "hunger", -(0.25f * (thc * 1.25f)));
                    //MultiplayerApi.Apply(body, "traumaAmount", -(0.35f * (cbd * 1.25f)));
                    body.happiness += 0.33f * (thc * 1.25f);
                    body.hunger -= 0.25f * (thc * 1.25f);
                    body.traumaAmount -= 0.35f * (cbd * 1.25f);
                    body.talker.Talk(Locale.GetCharacter("eatBad"));
                    Sound.Play("eatCrunch", body.transform.position);
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("candy.png", 60f));

            ItemRegistry.Register("cookieedible", new ItemInfo
            {
                fullName = "Cannabis Cookie",
                description = "A decent-sized cookie with cannabis infused into it. Tastes neutral.",
                category = "drug",
                tags = "medicine,cangetwet",
                weight = 0.25f,
                value = 11,
                decayMinutes = 120,
                usable = true,
                rec = new Recognition(7),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    CBD statusCBD = body.GetStatus<CBD>();
                    THC statusTHC = body.GetStatus<THC>();
                    System.Random rand = new System.Random();
                    int strain = rand.Next(1, 5);
                    float thc = 0;
                    float cbd = 0;
                    if (strain == 1)
                    {
                        thc = 70;
                        cbd = 5;
                    }
                    else if (strain == 2)
                    {
                        thc = 40;
                        cbd = 40;
                    }
                    else if (strain == 3)
                    {
                        thc = 5;
                        cbd = 70;
                    }
                    else if (strain == 4)
                    {
                        thc = 25;
                        cbd = 15;
                    }
                    statusTHC.Reception += thc * 1.25f;
                    statusCBD.Reception += cbd * 1.25f;
                    //MultiplayerApi.Apply(body, "happiness", 0.33f * (thc * 1.25f));
                    //MultiplayerApi.Apply(body, "hunger", -(0.25f * (thc * 1.25f)));
                    //MultiplayerApi.Apply(body, "traumaAmount", -(0.35f * (cbd * 1.25f)));
                    body.happiness += 0.33f * (thc * 1.25f);
                    body.hunger -= 0.25f * (thc * 1.25f);
                    body.traumaAmount -= 0.35f * (cbd * 1.25f);
                    body.talker.Talk(Locale.GetCharacter("eatMediocre"));
                    Sound.Play("eatCrunch", body.transform.position);
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("cookie.png"));

            ItemRegistry.Register("brownieedible", new ItemInfo
            {
                fullName = "Cannabis Brownie",
                description = "A large brownie with cannabis infused into it. Tastes great. While you can't digest chocolate, the cannabis prevents you from feeling sick.",
                category = "drug",
                tags = "medicine,cangetwet",
                weight = 0.2f,
                value = 12,
                decayMinutes = 60,
                usable = true,
                rec = new Recognition(7),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    CBD statusCBD = body.GetStatus<CBD>();
                    THC statusTHC = body.GetStatus<THC>();
                    System.Random rand = new System.Random();
                    int strain = rand.Next(1, 5);
                    float thc = 0;
                    float cbd = 0;
                    if (strain == 1)
                    {
                        thc = 70;
                        cbd = 5;
                    }
                    else if (strain == 2)
                    {
                        thc = 40;
                        cbd = 40;
                    }
                    else if (strain == 3)
                    {
                        thc = 5;
                        cbd = 70;
                    }
                    else if (strain == 4)
                    {
                        thc = 25;
                        cbd = 15;
                    }
                    statusTHC.Reception += thc * 1.25f;
                    statusCBD.Reception += cbd * 1.25f;
                    //MultiplayerApi.Apply(body, "happiness", 0.33f * (thc * 1.25f));
                    //MultiplayerApi.Apply(body, "hunger", -(0.25f * (thc * 1.25f)));
                    //MultiplayerApi.Apply(body, "traumaAmount", -(0.35f * (cbd * 1.25f)));
                    body.happiness += 0.33f * (thc * 1.25f);
                    body.hunger -= 0.25f * (thc * 1.25f);
                    body.traumaAmount -= 0.35f * (cbd * 1.25f);
                    body.talker.Talk(Locale.GetCharacter("eatGood"));
                    Sound.Play("eatCrunch", body.transform.position);
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("brownie.png"));

            ItemRegistry.Register("psilocybin", new ItemInfo
            {
                fullName = "Psilocybin",
                description = "A mild psychedelic. Looks like pieces of a mushroom ripped up and put back together. Lifts your mood slightly and distorts your senses. Looks a lot like brain damage but isn’t permanent.",
                category = "drug",
                tags = "medicine,cangetwet",
                weight = 0.3f,
                value = 10,
                decayMinutes = 180,
                usable = true,
                rec = new Recognition(4),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Psychedelic status = body.GetStatus<Psychedelic>();
                    status.Reception += 25f;
                    //MultiplayerApi.Apply(body, "happiness", 17.5f);
                    body.happiness += 17.5f;
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("psilocybin.png"));

            ItemRegistry.Register("lsd", new ItemInfo
            {
                fullName = "LSD",
                description = "A moderate psychedelic. Stored on a piece of paper. Lifts your mood and distorts your senses. Looks a lot like brain damage but isn’t permanent.",
                category = "drug",
                tags = "medicine,cangetwet",
                weight = 0.2f,
                value = 12,
                decayMinutes = 120,
                usable = true,
                rec = new Recognition(5),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Psychedelic status = body.GetStatus<Psychedelic>();
                    status.Reception += 50f;
                    //MultiplayerApi.Apply(body, "happiness", 25f);
                    body.happiness += 25f;
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("lsd.png"));

            ItemRegistry.Register("dmt", new ItemInfo
            {
                fullName = "Dimethyltryptamine",
                description = "An exceptionally strong psychedelic in powder form, also known as DMT. Nothing else comes close. Greatly lifts your mood and transports you to another dimension.",
                category = "drug",
                tags = "medicine,cangetwet",
                weight = 0.15f,
                value = 12,
                decayMinutes = 60,
                usable = true,
                rec = new Recognition(6),
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    Psychedelic status = body.GetStatus<Psychedelic>();
                    status.DMT += 25f;
                    //MultiplayerApi.Apply(body, "happiness", 35f);
                    body.happiness += 35f;
                    string[] lines = new string[]
                    {
                        "The walls... they just dissolved.",
                        "No more bunker... just pure light.",
                        "I'm out of the hole... finally.",
                        "Everything's connected... so still...",
                        "Blinding light... no more pain."
                    };
                    body.talker.Talk(lines[UnityEngine.Random.Range(0, lines.Length)], null, false, true);
                    item.condition = 0f;
                }
            }, AssetLoader.LoadEmbeddedSprite("dmt.png"));

            ItemRegistry.Register("glasswareset", new ItemInfo
            {
                fullName = "Glassware Set",
                description = "A set of beakers, flasks, and condensors. Useful in the production and processing of a variety of substances.",
                category = "utility",
                weight = 1f,
                value = 18,
                usable = false,
                rec = new Recognition(4)
            }, AssetLoader.LoadEmbeddedSprite("glasswareset.png"));

            ItemRegistry.Register("separationkit", new ItemInfo
            {
                fullName = "Separation Kit",
                description = "A set of funnels, filters, and pipettes. Useful in the refinement of a variety of substances.",
                category = "utility",
                weight = 0.65f,
                value = 15,
                usable = false,
                rec = new Recognition(5)
            }, AssetLoader.LoadEmbeddedSprite("separationkit.png"));
            
            //ItemRegistry.Register("precisionscale", new ItemInfo
            //{
            //    fullName = "Precision Scale",
            //    description = "A small scale accurate to the microgram. Useful for making things that need very specific ingredient amounts.",
            //    category = "utility",
            //    weight = 0.3f,
            //    value = 12,
            //    usable = false,
            //    rec = new Recognition(6)
            //}, AssetLoader.LoadEmbeddedSprite("precisionscale.png"), 0);

            ItemRegistry.Register("phstrips", new ItemInfo
            {
                fullName = "pH Strips",
                description = "A pack containing a number of small strips used to test pH levels.",
                category = "utility",
                weight = 0.125f,
                value = 10,
                usable = false,
                rec = new Recognition(6)
            }, AssetLoader.LoadEmbeddedSprite("phstrips.png"));

            ItemRegistry.Register("nailpolishremover", new LiquidItemInfo
            {
                fullName = "Nail Polish Remover",
                description = "A small bottle of nail polish remover. Contains acetone, a solvent.",
                category = "utility",
                weight = 0.1f,
                scaleWeightWithCondition = true,
                destroyAtZeroCondition = false,
                value = 7,
                usable = true,
                capacity = 50f,
                defaultContents = new List<LiquidStack>
                {
                    new LiquidStack("acetone", 50)
                },
                autoFill = false,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    item.GetComponent<WaterContainerItem>().Drink(body, 25);
                },
                rec = new Recognition(5)
            }, AssetLoader.LoadEmbeddedSprite("nailpolishremover.png"));

            ItemRegistry.Register("lye", new ItemInfo
            {
                fullName = "Lye",
                description = "A container with a white, chalky powder inside. Useful as a base.",
                category = "utility",
                weight = 0.3f,
                value = 9,
                usable = true,
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    //MultiplayerApi.Apply(body.limbs[0], "skinHealth", -60f);
                    //MultiplayerApi.Apply(body.limbs[0], "muscleHealth", -40f);
                    //MultiplayerApi.Apply(body.limbs[0], "pain", 100f);
                    body.limbs[0].skinHealth -= 60f;
                    body.limbs[0].muscleHealth -= 40f;
                    body.limbs[0].pain += 100f;
                    item.condition -= 0.1f;
                },
                rec = new Recognition(4)
            }, AssetLoader.LoadEmbeddedSprite("lye.png"));

            ItemRegistry.Register("foil", new ItemInfo
            {
                fullName = "Aluminum Foil",
                description = "A roll of metallic foil. Can act as a reducing agent.",
                category = "utility",
                weight = 0.2f,
                value = 4,
                usable = false,
                rec = new Recognition(3)
            }, AssetLoader.LoadEmbeddedSprite("foil.png"));

                                                                                                                                                                                                                                                
            ItemRegistry.Register("roadmarker", new ItemInfo
            {
                fullName = "Road Marker",
                description = "A strip of yellow paint made for road markings. Contains a precursor to clonidine.",
                category = "utility",
                weight = 0.8f,
                value = 4,
                usable = false,
                rec = new Recognition(2)
            }, AssetLoader.LoadEmbeddedSprite("roadmarker.png"));

            ItemRegistry.Register("perfume", new ItemInfo
            {
                fullName = "Perfume",
                description = "A nice-smelling fragrance used in fashion. Contains a precursor to phenobarbital.",
                category = "utility",
                weight = 0.1f,
                value = 2,
                usable = false,
                rec = new Recognition(2)
            }, AssetLoader.LoadEmbeddedSprite("perfume.png"));

            ItemRegistry.Register("nailpolish", new ItemInfo
            {
                fullName = "Nail Polish",
                description = "Paint meant to coat and adhere to keratin. Humans use these for their fingernails. Contains a precursor to diphenhydramine.",
                category = "utility",
                weight = 0.1f,
                value = 3,
                usable = false,
                rec = new Recognition(2)
            }, AssetLoader.LoadEmbeddedSprite("nailpolish.png"));

            ItemRegistry.Register("grabbergland", new ItemInfo
            {
                fullName = "Pituitary Gland",
                description = "The pituitary gland of a grabber plant. Contains hormones which can be used to produce anabolic steroids.",
                category = "utility",
                weight = 0.2f,
                value = 6,
                decayMinutes = 60,
                usable = false,
                rec = new Recognition(7)
            }, AssetLoader.LoadEmbeddedSprite("pituitarygland.png"), 0);

            ItemRegistry.Register("rootbeer", new LiquidItemInfo
            {
                fullName = "Root Beer",
                description = "A can holding 300ml of root beer.",
                category = "food",
                weight = 0.5f,
                scaleWeightWithCondition = true,
                destroyAtZeroCondition = false,
                value = 5,
                rec = new Recognition(3),
                capacity = 300f,
                defaultContents = new List<LiquidStack>
                {
                    new LiquidStack("rootbeer", 300)
                },
                useAction = (body, item) =>
                {
                    if (PlayerCamera.main == null || PlayerCamera.main.body != body)
                    {
                        return;
                    }
                    item.GetComponent<WaterContainerItem>().Drink(body);
                },
                autoFill = true
            }, AssetLoader.LoadEmbeddedSprite("rootbeercan.png"));

            RecipeRegistry.Register(new Recipe
            {
                INT = 4,
                result = new RecipeResult { id = "hydrochloricacid", isLiquid = true, resultCondition = 50 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.5f) { specificId = "internalorgans" },
                    new RecipeItem(0f) { quality = new CraftingQuality("cutting", 1), destroyItem = false }
                }
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 5,
                result = new RecipeResult { id = "hydrochloricacid", isLiquid = true, resultCondition = 50 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.5f) { specificId = "internalorgans" },
                    new RecipeItem(0f) { quality = new CraftingQuality("nails", 1), destroyItem = false }
                }
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 10,
                result = new RecipeResult { id = "diazepam", isLiquid = true, resultCondition = 30 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.2f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.2f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(50f) { specificId = "bleach", isLiquid = true },
                    new RecipeItem(0.5f) { specificId = "mushtail" },
                    new RecipeItem(0.5f) { specificId = "mushtail" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 13,
                result = new RecipeResult { id = "clonidine", isLiquid = true, resultCondition = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.2f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.2f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("acid", 25f), isLiquid = true },
                    new RecipeItem(0.2f) { specificId = "lye", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "foil", destroyItem = false },
                    new RecipeItem(50f) { specificId = "bleach", isLiquid = true },
                    new RecipeItem(0.5f) { specificId = "roadmarker", destroyItem = false },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 13,
                result = new RecipeResult { id = "clonidine", isLiquid = true, resultCondition = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.2f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.2f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("acid", 25f), isLiquid = true },
                    new RecipeItem(0.2f) { specificId = "lye", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "foil", destroyItem = false },
                    new RecipeItem(50f) { specificId = "saltpeter", isLiquid = true },
                    new RecipeItem(0.5f) { specificId = "roadmarker", destroyItem = false },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 10,
                result = new RecipeResult { id = "diphenhydramine" },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.1f) { specificId = "plasticchunk" },
                    new RecipeItem(0.1f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("acid", 25f), isLiquid = true },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(1f) { specificId = "nailpolish" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 10,
                result = new RecipeResult { id = "diphenhydramine" },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.1f) { specificId = "plasticchunk" },
                    new RecipeItem(0.1f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0.2f) { specificId = "lye", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(1f) { specificId = "nailpolish" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 11,
                result = new RecipeResult { id = "phenobarbital", isLiquid = true, resultCondition = 60 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.1f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("acid", 25f), isLiquid = true },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.5f) { specificId = "perfume", destroyItem = false },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 11,
                result = new RecipeResult { id = "phenobarbital", isLiquid = true, resultCondition = 60 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.1f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0.2f) { specificId = "lye", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.5f) { specificId = "perfume", destroyItem = false },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 10,
                result = new RecipeResult { id = "testosterone", isLiquid = true, resultCondition = 250 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.1f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("acid", 25f), isLiquid = true },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.05f) { specificId = "grabbergland" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 10,
                result = new RecipeResult { id = "testosterone", isLiquid = true, resultCondition = 250 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.1f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0.2f) { specificId = "lye" },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.05f) { specificId = "grabbergland" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 12,
                result = new RecipeResult { id = "mdma" },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.3f) { specificId = "plasticchunk" },
                    new RecipeItem(0.2f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.2f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("acid", 25f), isLiquid = true },
                    new RecipeItem(0.2f) { specificId = "lye", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "foil", destroyItem = false },
                    new RecipeItem(50f) { specificId = "bleach", isLiquid = true },
                    new RecipeItem(300f) { specificId = "rootbeer", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 12,
                result = new RecipeResult { id = "mdma" },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.3f) { specificId = "plasticchunk" },
                    new RecipeItem(0.2f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.2f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("acid", 25f), isLiquid = true },
                    new RecipeItem(0.2f) { specificId = "lye", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "foil", destroyItem = false },
                    new RecipeItem(50f) { specificId = "saltpeter", isLiquid = true },
                    new RecipeItem(300f) { specificId = "rootbeer", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 14,
                result = new RecipeResult { id = "semaglutide", isLiquid = true, resultCondition = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.2f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.2f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("acid", 25f), isLiquid = true },
                    new RecipeItem(0.2f) { specificId = "lye", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "foil", destroyItem = false },
                    new RecipeItem(50f) { specificId = "bleach", isLiquid = true },
                    new RecipeItem() { specificId = "digestioncrystalshard" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                }
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 14,
                result = new RecipeResult { id = "semaglutide", isLiquid = true, resultCondition = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.2f) { specificId = "glasswareset", destroyItem = false },
                    new RecipeItem(0.2f) { specificId = "separationkit", destroyItem = false },
                    new RecipeItem(10f) { specificId = "acetone", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("acid", 25f), isLiquid = true },
                    new RecipeItem(0.2f) { specificId = "lye", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "phstrips", destroyItem = false },
                    new RecipeItem(0.1f) { specificId = "foil", destroyItem = false },
                    new RecipeItem(50f) { specificId = "saltpeter", isLiquid = true },
                    new RecipeItem() { specificId = "digestioncrystalshard" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                }
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 5,
                result = new RecipeResult { id = "psilocybin", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.3f) { specificId = "browncap" },
                    new RecipeItem(0.3f) { specificId = "browncap" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 7,
                result = new RecipeResult { id = "lsd", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.5f) { specificId = "canvas" },
                    new RecipeItem(0.3f) { specificId = "bulbskin" },
                    new RecipeItem(0.3f) { specificId = "bulbskin" },
                    new RecipeItem(0.3f) { specificId = "bulbskin" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 8,
                result = new RecipeResult { id = "dmt", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.3f) { specificId = "browncap" },
                    new RecipeItem(0.3f) { specificId = "bulbskin" },
                    new RecipeItem(0.3f) { specificId = "bulbskin" },
                    new RecipeItem(0f) { specificId = "filterstraw", destroyItem = false },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 8,
                result = new RecipeResult { id = "candyedible", amount = 1, resultCondition = 1f },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.05f) { specificId = "cannabis" },
                    new RecipeItem(0.2f) { specificId = "hardcandy", destroyItem = false },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 8,
                result = new RecipeResult { id = "cookieedible", amount = 1, resultCondition = 1f },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.05f) { specificId = "cannabis" },
                    new RecipeItem(0.1f) { specificId = "cookies", destroyItem = false },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 8,
                result = new RecipeResult { id = "brownieedible", amount = 1, resultCondition = 1f },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.05f) { specificId = "cannabis" },
                    new RecipeItem(0.2f) { specificId = "ryeflour" },
                    new RecipeItem(0.33f) { specificId = "chocolatebar" },
                    new RecipeItem(20f) { specificId = "fat", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 1,
                result = new RecipeResult { id = "tobacco", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "cigarette" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 1,
                result = new RecipeResult { id = "Tobacco", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "tobaccojoint" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 1,
                result = new RecipeResult { id = "cannabis", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "joint" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 1,
                result = new RecipeResult { id = "cannabis", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "cannabiscigarette" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 2,
                result = new RecipeResult { id = "spin", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "tobacco" },
                    new RecipeItem { specificId = "cannabis" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 1,
                result = new RecipeResult { id = "spin", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "spliff" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 1,
                result = new RecipeResult { id = "spin", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "spincigarette" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 3,
                result = new RecipeResult { id = "emptycigarette", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "canvas" },
                    new RecipeItem { specificId = "rippeddressing" },
                    new RecipeItem(0.35f) { specificId = "flammablepowder" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 4,
                result = new RecipeResult { id = "emptyjoint", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "canvas" },
                    new RecipeItem { specificId = "canvas" },
                    new RecipeItem(0.35f) { specificId = "flammablepowder" },
                    new RecipeItem(10f) { specificId = "biochem", isLiquid = true }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 2,
                result = new RecipeResult { id = "cigarette", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "emptycigarette" },
                    new RecipeItem { specificId = "tobacco" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 2,
                result = new RecipeResult { id = "cannabiscigarette", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "emptycigarette" },
                    new RecipeItem { specificId = "cannabis" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 2,
                result = new RecipeResult { id = "spincigarette", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "emptycigarette" },
                    new RecipeItem { specificId = "spin" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 4,
                result = new RecipeResult { id = "cigar", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.2f) { specificId = "tobacco" },
                    new RecipeItem(0.2f) { specificId = "tobacco" },
                    new RecipeItem(0.35f) { specificId = "flammablepowder" },
                    new RecipeItem(10f) { specificId = "biochem", isLiquid = true }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 2,
                result = new RecipeResult { id = "joint", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "emptyjoint" },
                    new RecipeItem { specificId = "cannabis" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 2,
                result = new RecipeResult { id = "tobaccojoint", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "emptyjoint" },
                    new RecipeItem { specificId = "tobacco" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 2,
                result = new RecipeResult { id = "spliff", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem { specificId = "emptyjoint" },
                    new RecipeItem { specificId = "spin" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 2,
                result = new RecipeResult { id = "blunt", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.2f) { specificId = "cigar" },
                    new RecipeItem(0.2f) { specificId = "cannabis" }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 4,
                result = new RecipeResult { id = "blunt", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.2f) { specificId = "tobacco" },
                    new RecipeItem(0.2f) { specificId = "tobacco" },
                    new RecipeItem(0.2f) { specificId = "cannabis" },
                    new RecipeItem(0.35f) { specificId = "flammablepowder" },
                    new RecipeItem(10f) { specificId = "biochem", isLiquid = true }
                },
                category = Recipes.RecipeCategory.Medicine
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 3,
                result = new RecipeResult { id = "woodash", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem() { specificId = "woodscraps" },
                    new RecipeItem() { specificId = "woodscraps" },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Materials
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 5,
                result = new RecipeResult { id = "woodvinegar", isLiquid = true, resultCondition = 50 },
                items = new List<RecipeItem>
                {
                    new RecipeItem() { specificId = "woodscraps" },
                    new RecipeItem() { specificId = "woodscraps" },
                    new RecipeItem() { specificId = "woodscraps" },
                    new RecipeItem() { specificId = "carcass" },
                    new RecipeItem(10f) { specificId = "biochem", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Materials
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 8,
                result = new RecipeResult { id = "acetone", isLiquid = true, resultCondition = 20 },
                items = new List<RecipeItem>
                {
                    new RecipeItem() { specificId = "woodash" },
                    new RecipeItem(50f) { specificId = "woodvinegar", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Materials
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 7,
                result = new RecipeResult { id = "lye", resultCondition = 0.6f },
                items = new List<RecipeItem>
                {
                    new RecipeItem() { specificId = "woodash" },
                    new RecipeItem() { specificId = "canvas" },
                    new RecipeItem(10f) { specificId = "biochem", isLiquid = true }
                },
                category = Recipes.RecipeCategory.Materials
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 7,
                result = new RecipeResult { id = "saltpeter", isLiquid = true, resultCondition = 50 },
                items = new List<RecipeItem>
                {
                    new RecipeItem() { specificId = "droppings" },
                    new RecipeItem() { specificId = "woodash" },
                    new RecipeItem(0f) { quality = new CraftingQuality("water", 50f), isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Materials
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 10,
                result = new RecipeResult { id = "glasswareset", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem() { specificId = "flexiglass" },
                    new RecipeItem() { specificId = "flexiglass" },
                    new RecipeItem() { specificId = "plasticchunk" },
                    new RecipeItem() { specificId = "plasticchunk" },
                    new RecipeItem() { specificId = "scrappanel" },
                    new RecipeItem(20f) { specificId = "biochem", isLiquid = true }
                },
                category = Recipes.RecipeCategory.Utilities
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 10,
                result = new RecipeResult { id = "separationkit", amount = 1 },
                items = new List<RecipeItem>
                {
                    new RecipeItem() { specificId = "flexiglass" },
                    new RecipeItem() { specificId = "flexiglass" },
                    new RecipeItem() { specificId = "plasticchunk" },
                    new RecipeItem() { specificId = "plasticchunk" },
                    new RecipeItem() { specificId = "plasticchunk" },
                    new RecipeItem() { specificId = "scraptube" },
                    new RecipeItem() { specificId = "canvas" },
                    new RecipeItem(20f) { specificId = "biochem", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Utilities
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 6,
                result = new RecipeResult { id = "foil", amount = 1, resultCondition = 0.5f },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.9f) { specificId = "scrapmetal" },
                    new RecipeItem(0f) { quality = new CraftingQuality("hammering", 20f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Materials
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 9,
                result = new RecipeResult { id = "phstrips", amount = 1, resultCondition = 0.2f },
                items = new List<RecipeItem>
                {
                    new RecipeItem() { specificId = "canvas" },
                    new RecipeItem() { specificId = "glowplantfruit" },
                    new RecipeItem(0f) { quality = new CraftingQuality("cutting", 8f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Materials
            });

            RecipeRegistry.Register(new Recipe
            {
                INT = 8,
                result = new RecipeResult { id = "nitricacid", isLiquid = true, resultCondition = 50 },
                items = new List<RecipeItem>
                {
                    new RecipeItem(0.9f) { specificId = "scrapmetal" },
                    new RecipeItem(50f) { specificId = "saltpeter", isLiquid = true },
                    new RecipeItem(0f) { quality = new CraftingQuality("heatsource", 10f), destroyItem = false }
                },
                category = Recipes.RecipeCategory.Materials
            });
        }
    }
}
