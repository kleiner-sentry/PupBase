namespace PupBase.Hooks
{
    public class PlayerHooks
    {
        public static void Init()
        {
            IL.Player.SetMalnourished += IL_Player_SetMalnourished;
            IL.Player.NPCStats.ctor += IL_NPCStats_ctor;
        }

        public static void IL_Player_SetMalnourished(ILContext il)
        {
            try
            {
                ILCursor statsCurs = new ILCursor(il);
                statsCurs.GotoNext((Instruction x) => x.MatchLdarg(0));
                statsCurs.GotoNext(MoveType.After, (Instruction x) => x.MatchLdarg(0));
                statsCurs.GotoNext(MoveType.After, (Instruction x) => x.MatchLdarg(0));
                statsCurs.GotoNext(MoveType.After, (Instruction x) => x.MatchLdfld(typeof(Player).GetField(nameof(Player.SlugCatClass))));
                statsCurs.Emit(OpCodes.Ldarg_0);
                statsCurs.EmitDelegate((SlugcatStats.Name name, Player player) =>
                {
                    return player.PupState().pupType != null ? player.PupType().hasAdultModule && player.playerState.forceFullGrown ? player.PupType().adultModule.name : player.PupType().name : name;
                });
            }
            catch (Exception e)
            {
                Plugin.ModLogger.LogError(e);
            }
        }

        public static void IL_NPCStats_ctor(ILContext il)
        {
            try
            {
                ILCursor statsCurs = new ILCursor(il);
                statsCurs.GotoNext((Instruction x) => x.MatchStloc(0));
                statsCurs.GotoNext(MoveType.After, (Instruction x) => x.MatchStloc(0));
                statsCurs.Emit(OpCodes.Ldarg_1);
                statsCurs.EmitDelegate(delegate (Player player)
                {
                    if (Plugin.SlugpupStuff && PupManager.IsPupInUseBySlugpupStuff(player.playerState) && player.PupState() != null)
                    {
                        if (player.PupState().prioritize)
                        {
                            PupManager.OverrideSlugpupStuffVariant(player.playerState, null);
                            Plugin.ModLogger.LogInfo("Pups+ variant detected. Setting Variant to null.");
                        }
                        else
                        {
                            player.PupState().pupType = null;
                            Plugin.ModLogger.LogInfo("Pups+ variant detected. Setting PupType to null.");
                        }
                    }
                    else if (player.isSlugpup && player.isNPC && player.PupType() == null && !((Plugin.Pearlcat && PupManager.IsPearlpup(player.abstractCreature)) || PupManager.PupIDBlacklist.Contains(player.abstractCreature.ID.RandomSeed)))
                    {
                        player.PupState().pupType = PupManager.GenerateType(player.abstractCreature, player.playerState.forceFullGrown, info: true);
                        if (!player.playerState.forceFullGrown && player.PupType().hasAdultModule)
                        {
                            player.playerState.forceFullGrown = PupManager.GenerateAdult(player.abstractCreature, player.PupType().adultModule);

                        }
                    }
                });
            
                statsCurs.GotoNext(MoveType.After, (Instruction x) => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Slugpup"));
                statsCurs.Emit(OpCodes.Ldarg_1);
                statsCurs.EmitDelegate((SlugcatStats.Name slugpup, Player player) => 
                {
                    return player.PupState().pupType != null ? player.PupType().hasAdultModule && player.playerState.forceFullGrown ? player.PupType().adultModule.name : player.PupType().name : slugpup;
                });
            }
            catch (Exception e)
            {
                Plugin.ModLogger.LogError(e);
            }
        }
    }
}
