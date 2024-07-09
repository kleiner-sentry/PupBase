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
                statsCurs.EmitDelegate((SlugcatStats.Name name, Player player) => player.PupType() != null ? player.PupType().name : name);
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
                    // If a Puptype has been detected prior to its generation, that means it's been assigned manually or was assigned by a save file. If so, ensure that this PupType remains as it is.
                    if (Plugin.SlugpupStuff && player.PupType() != null && PupManager.IsPupInUseBySlugpupStuff(player.playerState))
                    {
                        PupManager.OverrideSlugpupStuffVariant(player.playerState, null);
                        Plugin.ModLogger.LogInfo("Pups+ variant detected. Setting Variant to null.");
                    }
                    // If the above isn't true, then generate a PupType.
                    else if (player.isSlugpup && player.isNPC && player.PupType() == null && !(Plugin.SlugpupStuff && PupManager.IsPupInUseBySlugpupStuff(player.playerState)))
                    {
                        player.PupState().pupType = PupManager.GenerateType(player.abstractCreature);
                        Plugin.ModLogger.LogInfo("Generated " + player.abstractCreature.ID.ToString() + " Type " + player.PupType().name);
                    }
                    // If a Pup Variant is detected in this process, then override the PupType to be null.
                    else if (Plugin.SlugpupStuff && player.PupType() != null && PupManager.IsPupInUseBySlugpupStuff(player.playerState))
                    {
                        player.PupState().pupType = null;
                        Plugin.ModLogger.LogInfo("Pups+ variant detected. Setting PupType to null.");
                    }
                });
            
                statsCurs.GotoNext(MoveType.After, (Instruction x) => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Slugpup"));
                statsCurs.Emit(OpCodes.Ldarg_1);
                statsCurs.EmitDelegate((SlugcatStats.Name slugpup, Player player) => player.PupType() != null ? player.PupType().name : slugpup);
            }
            catch (Exception e)
            {
                Plugin.ModLogger.LogError(e);
            }
            
        }
    }
}
