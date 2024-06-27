namespace PupBase.Hooks
{
    public class PlayerHooks
    {
        public static void Init()
        {
            IL.Player.SetMalnourished += IL_Player_SetMalnourished;
            IL.Player.NPCStats.ctor += IL_NPCStats_ctor;
            On.Player.ctor += Player_ctor;
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

        public static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            try
            {
                if (self.isSlugpup && self.isNPC && self.PupState() is { } state)
                {
                    state.pupType.PlayerConstructed(self);
                }
            }
            catch (Exception e)
            {
                Plugin.ModLogger.LogError(e);
            }
        }
    }
}
