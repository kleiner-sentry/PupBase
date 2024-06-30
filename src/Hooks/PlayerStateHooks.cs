namespace PupBase.Hooks
{
    public class PlayerStateHooks
    {
        public static void Init()
        {
            On.MoreSlugcats.PlayerNPCState.ToString += PlayerNPCState_ToString;
            On.MoreSlugcats.PlayerNPCState.LoadFromString += PlayerNPCState_LoadFromString;

            IL.MoreSlugcats.PlayerNPCState.CycleTick += IL_PlayerNPCState_CycleTick;

        }

        public static string PlayerNPCState_ToString(On.MoreSlugcats.PlayerNPCState.orig_ToString orig, PlayerNPCState self)
        {
            string text = orig(self);
            text += "Type<cC>" + (self.PupType() != null ? self.PupType().name.value : "NULL") + "<cB>";
            return text;
        }

        public static void PlayerNPCState_LoadFromString(On.MoreSlugcats.PlayerNPCState.orig_LoadFromString orig, PlayerNPCState self, string[] s)
        {
            orig(self, s);
            for (int i = 0; i < s.Length - 1; i++)
            {
                string[] array = Regex.Split(s[i], "<cC>");
                switch (array[0])
                {
                    case "Type":
                        if (PupManager.TryGetPupTypeFromString(array[1], out var type))
                        {
                            self.PupState().pupType = type;
                            Plugin.ModLogger.LogInfo("Assigned from save " + self.player.ID.ToString() + " Type " + type.name);
                        }
                        break;
                    case "SlugcatCharacter":
                        if (Plugin.BeastMasterPupExtras && !array[1].Equals("Slugpup"))
                        {
                            self.PupState().pupType = PupManager.GetPupType(MoreSlugcatsEnums.SlugcatStatsName.Slugpup);
                        }
                        break;
                }
            }
            self.unrecognizedSaveStrings.Remove("Type");
        }

        public static void IL_PlayerNPCState_CycleTick(ILContext il)
        {
            ILCursor foodCurs = new(il);    

            while (foodCurs.TryGotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Slugpup))))
            {
                /* WHILE TRYGOTO AFTER ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Slugpup
                 * 	IL_****: ldarg.0
	             *  IL_****: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Slugpup
                 */
                foodCurs.Emit(OpCodes.Ldarg_0); // self
                foodCurs.EmitDelegate((SlugcatStats.Name slugpup, PlayerNPCState self) =>   // If pupNPCState.variant != null, return variant, else return slugpup
                {
                    return self.PupType() != null ? self.PupType().name : slugpup;
                }); 
            }
        }
    }
}