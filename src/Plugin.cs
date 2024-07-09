using System.Drawing.Drawing2D;

namespace PupBase
{
    [BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
    class Plugin : BaseUnityPlugin
    {
        public const string MOD_ID = "Antoneeee.PupBase";

        public const string MOD_NAME = "PupBase";

        public const string VERSION = "1.1.3";

        public const string AUTHORS = "Antoneeee";

        private bool IsInit = false;

        private bool PostIsInit = false;

        public static bool BeastMasterPupExtras = false;

        public static bool Pearlcat = false;

        public static bool SlugpupStuff = false;

        public static BepInEx.Logging.ManualLogSource ModLogger;

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

        //code from Pups+, modified by Antoneeee
        public static void DevConsoleCommand()
        {
            string[] tags = ["Voidsea", "Winter", "Ignorecycle", "TentacleImmune", "Lavasafe", "AlternateForm", "PreCycle", "Night"];
            string[] types = [..PupManager.GetPupTypeListString().ToArray(), "Random"];
            new CommandBuilder("spawn_slugNPC")
                .RunGame((game, arguments) =>
                {
                    try
                    {
                        if (arguments.Length > 0)
                        {
                            EntityID? id = null;
                            PupType pupType = null;
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
                                else if (PupManager.TryGetPupTypeFromString(argument, out var type))
                                {
                                    pupType = type;
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

                            AbstractCreature abstractPup = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, GameConsole.TargetPos.Room.realizedRoom.GetWorldCoordinate(GameConsole.TargetPos.Pos), id ?? game.GetNewID());
                            if (pupType != null && abstractPup.state is PlayerState npcState)
                            {
                                npcState.PupState().pupType = pupType;
                                ModLogger.LogInfo("Assigned " + abstractPup.ID.ToString() + " Type " + npcState.PupType().name);
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
                .Help("spawn_slugNPC [type?] [ID?] [args...]")
                .AutoComplete(arguments =>
                {
                    bool type = false;
                    foreach (string argument in arguments)
                    {
                        if (PupManager.TryGetPupTypeFromString(argument))
                        {
                            type = true;
                        }
                    }
                    if (!type) return types;
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
            List<string> tempList = PupManager.GetPupTypeListString();

            foreach (string str in list)
            {
                tempList.Remove(str);
            }

            list.AddRange(tempList);

            return list;
        }
    }
}