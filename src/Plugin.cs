using System.Drawing.Drawing2D;

namespace PupBase
{
    [BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
    class Plugin : BaseUnityPlugin
    {
        public const string MOD_ID = "Antoneeee.PupBase";

        public const string MOD_NAME = "PupBase";

        public const string VERSION = "1.2.7";

        public const string AUTHORS = "Antoneeee";

        internal bool IsInit = false;

        internal bool PostIsInit = false;

        internal static bool BeastMasterPupExtras = false;

        internal static bool Pearlcat = false;

        internal static bool SlugpupStuff = false;

        internal static BepInEx.Logging.ManualLogSource ModLogger;

        Plugin()
        {
            ModLogger = Logger;
        }

        public void OnEnable()
        {
            ModLogger = Logger;
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
            ModLogger.LogInfo("Initialized " + VERSION);
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsInit) return;

                SlugpupNames.RegisterValues();

                ModOptions.RegisterOI();
                Hooks.PlayerHooks.Init();
                Hooks.PlayerStateHooks.Init();
                Hooks.SlugcatStatsHooks.Init();

                IsInit = true;
            }
            catch (Exception e)
            {
                ModLogger.LogError(e);
                throw;
            }
        }

        private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (PostIsInit) return;
                
                if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
                {
                    DMSHook();
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "NoirCatto.BeastMasterPupExtras"))
                {
                    BeastMasterPupExtras = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "pearlcat"))
                {
                    Pearlcat = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "iwantbread.slugpupstuff"))
                {
                    SlugpupStuff = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "slime-cubed.devconsole"))
                {
                    DevConsoleCommand();
                }

                PostIsInit = true;
            }
            catch (Exception e)
            {
                ModLogger.LogError(e);
                throw;
            }
        }

        public static void DevConsoleCommand()
        {
            string[] tags = ["Voidsea", "Winter", "Ignorecycle", "TentacleImmune", "Lavasafe", "AlternateForm", "PreCycle", "Night"];
            string[] typeTags = [..PupManager.GetPupTypeListString().ToArray(), "Random"];
            string[] ageTags = ["Adult", "Child"];
            new CommandBuilder("spawn_slugNPC")
                .RunGame((game, arguments) =>
                {
                    try
                    {
                        if (arguments.Length > 0)
                        {
                            EntityID? id = null;
                            string pupType = null;
                            string ageType = null;
                            bool? prioritize = null;
                            string[] tempTags = [];
                            foreach (string argument in arguments)
                            {
                                if (argument.Contains('.'))
                                {
                                    try
                                    {
                                        id = EntityID.FromString(argument);
                                    }
                                    catch
                                    {
                                        if (int.TryParse(argument, out int idNum))
                                            id = new EntityID(0, idNum);
                                    }
                                }
                                else
                                {
                                    if (argument.Equals("true", StringComparison.OrdinalIgnoreCase))
                                    {
                                        prioritize = true;
                                    }
                                    else if (argument.Equals("false", StringComparison.OrdinalIgnoreCase))
                                    {
                                        prioritize = false;
                                    }

                                    if (pupType == null && typeTags.Contains(argument))
                                    {
                                        pupType = argument;
                                    }
                                    if (ageType == null && ageTags.Contains(argument))
                                    {
                                        ageType = argument;
                                    }

                                    foreach (string testTag in tags)
                                    {
                                        if (argument.Equals(testTag, StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (tempTags.Length > 0)
                                            {
                                                tempTags.Append("," + testTag);
                                            }
                                            else
                                            {
                                                tempTags.Append(testTag);
                                            }
                                        }
                                    }
                                }
                            }

                            AbstractCreature abstractPup = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, GameConsole.TargetPos.Room.realizedRoom.GetWorldCoordinate(GameConsole.TargetPos.Pos), id ?? game.GetNewID());
                            if (abstractPup.state is PlayerState npcState)
                            {
                                npcState.PupState().prioritize = prioritize != false;
                                if (pupType != null && PupManager.TryGetPupTypeFromString(pupType, out PupType type))
                                {
                                    npcState.PupState().pupType = type;
                                    if (ageType == null) npcState.forceFullGrown = PupManager.GenerateAdult(abstractPup, npcState.PupType(), false);
                                    npcState.PupState().prioritize = prioritize == true;
                                }
                                if (ageType != null)
                                {
                                    switch (ageType)
                                    {
                                        case "Adult":
                                            if (npcState.PupType() == null || (npcState.PupType() != null && !npcState.PupType().HasAdultModule))
                                            {
                                                (npcState.PupState().pupType, _) = PupManager.GenerateType(abstractPup, true, info: false);
                                            }
                                            npcState.forceFullGrown = true;
                                            npcState.PupState().prioritize = prioritize == true;
                                            break;
                                        case "Child":
                                            if (npcState.PupType() == null)
                                            {
                                                (npcState.PupState().pupType, _) = PupManager.GenerateType(abstractPup, false, info: false);
                                            }
                                            npcState.forceFullGrown = false;
                                            npcState.PupState().prioritize = prioritize == true;
                                            break;
                                    }
                                }
                                if (pupType != null || ageType != null)
                                {
                                    ModLogger.LogInfo("Assigned " + abstractPup.ID.ToString() + (npcState.PupType() != null ? " Type " + npcState.PupType().name : "") + (ageType != null ? " As " + (npcState.forceFullGrown ? "an adult" : "a child") : ""));
                                }
                            }
                            if (tempTags.Length > 0)
                            {
                                abstractPup.spawnData = "{" + tempTags.ToString() + "}";
                                abstractPup.setCustomFlags();
                            }

                            GameConsole.TargetPos.Room.AddEntity(abstractPup);
                            if (GameConsole.TargetPos.Room.realizedRoom != null)
                            {
                                abstractPup.RealizeInRoom();
                            }
                        }
                        else
                        {
                            AbstractCreature abstractPup = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, GameConsole.TargetPos.Room.realizedRoom.GetWorldCoordinate(GameConsole.TargetPos.Pos), game.GetNewID());

                            GameConsole.TargetPos.Room.AddEntity(abstractPup);
                            if (GameConsole.TargetPos.Room.realizedRoom != null)
                            {
                                abstractPup.RealizeInRoom();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GameConsole.WriteLine(ex.Message);
                        ModLogger.LogWarning(ex.ToString());
                    }
                })
                .Help("spawn_slugNPC [type?] [maturity?] [prioritize: true] [ID?] [args...]")
                .AutoComplete(arguments =>
                {
                    bool type = false;
                    bool mat = false;
                    foreach (string argument in arguments)
                    {
                        if (typeTags.Contains(argument))
                        {
                            type = true;
                        }
                        if (ageTags.Contains(argument))
                        {
                            mat = true;
                        }
                    }
                    if (!type) return typeTags;
                    else if (!mat) return ageTags;
                    else return tags;
                })
                .Register();
        }

        private void DMSHook()
        {
            new Hook(typeof(DressMySlugcat.Utils).GetProperty("ValidSlugcatNames", BindingFlags.Static | BindingFlags.Public).GetGetMethod(), new Func<Func<List<string>>, List<string>>(DMSValidPupNames));
        }

        private List<string> DMSValidPupNames(Func<List<string>> orig)
        {
            List<string> list = orig();
            List<string> tempList = PupManager.GetPupTypeListString(true);

            foreach (string str in list)
            {
                tempList.Remove(str);
            }

            list.AddRange(tempList);

            return list;
        }
    }
}