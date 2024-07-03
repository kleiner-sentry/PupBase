using System.Drawing.Drawing2D;

namespace PupBase
{
    [BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
    class Plugin : BaseUnityPlugin
    {
        public const string MOD_ID = "Antoneeee.PupBase";

        public const string MOD_NAME = "PupBase";

        public const string VERSION = "1.0.0";

        public const string AUTHORS = "Antoneeee";

        private bool IsInit = false;

        private bool PostIsInit = false;

        public static bool BeastMasterPupExtras = false;

        public static bool Pearlcat = false;

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
                    SetupDMSSprites();
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "NoirCatto.BeastMasterPupExtras"))
                {
                    BeastMasterPupExtras = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "pearlcat"))
                {
                    Pearlcat = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "slime-cubed.devconsole"))
                {
                    RegisterSpawnPupCommand();
                     ModLogger.LogInfo("spawn_pup command registered");
                }

                PostIsInit = true;
            }
            catch (Exception e)
            {
                ModLogger.LogError(e);
                throw;
            }
        }

        public static void RegisterSpawnPupCommand()
        {
            string[] tags = ["Voidsea", "Winter", "Ignorecycle", "TentacleImmune", "Lavasafe", "AlternateForm", "PreCycle", "Night"];
            string[] types = [..PupManager.GetPupTypeListString().ToArray(), "Random"];
            string[] arguments = null;
            new CommandBuilder("spawn_pup")
                .RunGame((game, args) =>
                {
                    try
                    {
                        arguments = args;
                        if (args.Length != 0)
                        {
                            EntityID? id = null;
                            PupType pupType = null;
                            foreach (string arg in args)
                            {
                                if (arg.Contains('.'))
                                {
                                    try
                                    {
                                        id = EntityID.FromString(arg);
                                    }
                                    catch
                                    {
                                        if (int.TryParse(arg, out int idNum))
                                            id = new EntityID(0, idNum);
                                    }
                                }

                                else if (PupManager.TryGetPupTypeFromString(arg, out var type))
                                {
                                    pupType = type;
                                }
                            }

                            var abstractPup = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, GameConsole.TargetPos.Room.realizedRoom.GetWorldCoordinate(GameConsole.TargetPos.Pos), id ?? game.GetNewID());
                            (abstractPup.state as PlayerState).PupState().pupType = pupType ?? PupManager.GenerateType(abstractPup);
                            abstractPup.spawnData = "{" + string.Join(",", args.Select((string tag) => tags.FirstOrDefault((string testTag) => tag.Equals(testTag, StringComparison.OrdinalIgnoreCase)) ?? tag)) + "}";
                            abstractPup.setCustomFlags();

                            GameConsole.TargetPos.Room.AddEntity(abstractPup);
                            if (GameConsole.TargetPos.Room.realizedRoom != null)
                            {
                                abstractPup.RealizeInRoom();
                            }
                        }
                        else
                        {
                            var abstractPup = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, GameConsole.TargetPos.Room.realizedRoom.GetWorldCoordinate(GameConsole.TargetPos.Pos), game.GetNewID());
                            (abstractPup.state as PlayerState).PupState().pupType = PupManager.GenerateType(abstractPup);

                            GameConsole.TargetPos.Room.AddEntity(abstractPup);
                            if (GameConsole.TargetPos.Room.realizedRoom != null)
                            {
                                abstractPup.RealizeInRoom();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GameConsole.WriteLine("Failed to spawn pup! See console log for more info.");
                        ModLogger.LogWarning("pup failed:" + ex.ToString());
                    }
                })
                .Help("spawn_pup [ID?] [type?] [args...]")
                .AutoComplete(arguments =>
                {
                    if (arguments.Length == 0) return types;
                    else if (arguments.Length == 1 && arguments[0].Contains('.')) return types;
                    else if (arguments.Length == 1 && !arguments[0].Contains('.') || arguments.Length == 2) return tags;
                    else return null;
                })
                .Register();
        }

        private void SetupDMSSprites()
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