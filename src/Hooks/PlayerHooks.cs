namespace PupBase.Hooks
{
    public class PlayerHooks
    {
        public static void Init()
        {
            IL.Player.SetMalnourished += IL_Player_SetMalnourished;
            IL.Player.NPCStats.ctor += IL_NPCStats_ctor;
            On.SlugcatStats.ctor += SlugcatStats_ctor;
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
                statsCurs.EmitDelegate((SlugcatStats.Name name, Player player) => player.PupType() != null ? player.PupType() : name); // player.PupState().type != null ? player.PupState().type : slugpup // player.PupState().type
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
                    if (player.isSlugpup && player.isNPC && player.PupType() == null)
                    {
                        PupType type = PupManager.GenerateType(player);
                        player.PupState().pupType = type;
                        player.PupState().type = type.name;
                        Plugin.ModLogger.LogInfo("Generated " + player.abstractCreature.ID.ToString() + " Type " + type!.name);
                    }
                });
            
                statsCurs.GotoNext(MoveType.After, (Instruction x) => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Slugpup"));
                statsCurs.Emit(OpCodes.Ldarg_1);
                statsCurs.EmitDelegate((SlugcatStats.Name slugpup, Player player) => player.PupType() != null ? player.PupType() : slugpup); // player.PupState().type != null ? player.PupState().type : slugpup // player.PupState().type
            }
            catch (Exception e)
            {
                Plugin.ModLogger.LogError(e);
            }

        }

        public static void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
        {
            orig(self, slugcat, malnourished);
            try
            {
                if (PupManager.GetPupType(slugcat) is PupType pupType)
                {
                    pupType.StatsConstructed(self, malnourished);
                }
            }
            catch (Exception e)
            {
                Plugin.ModLogger.LogError(e);
            }
        }
    }
}
