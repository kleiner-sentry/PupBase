using System.Globalization;

namespace PupBase.Hooks
{
    public class PlayerStateHooks
    {
        public static void Init()
        {
            On.MoreSlugcats.PlayerNPCState.ToString += PlayerNPCState_ToString;
            On.MoreSlugcats.PlayerNPCState.LoadFromString += PlayerNPCState_LoadFromString;
            On.MoreSlugcats.PlayerNPCState.CycleTick += PlayerNPCState_CycleTick;

            IL.MoreSlugcats.PlayerNPCState.CycleTick += IL_PlayerNPCState_CycleTick;

        }

        public static string PlayerNPCState_ToString(On.MoreSlugcats.PlayerNPCState.orig_ToString orig, PlayerNPCState self)
        {
            string text = orig(self);
            text += "Type<cC>" + (self.PupType() != null ? self.PupType().name.value : "NULL") + "<cB>";
            text += "Age<cC>" + self.PupState().age + "<cB>";
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
                            self.PupState().prioritize = true;
                            Plugin.ModLogger.LogInfo("Assigned from save " + self.player.ID.ToString() + " Type " + type.name);
                        }
                        break;
                    case "Age":
                        self.PupState().age = int.Parse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                        break;
                    case "Prioritize":
                        self.PupState().prioritize = bool.Parse(array[1]);
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
            self.unrecognizedSaveStrings.Remove("Age");
        }

        public static void PlayerNPCState_CycleTick(On.MoreSlugcats.PlayerNPCState.orig_CycleTick orig, PlayerNPCState self)
        {
            if (self.player.world.game.IsStorySession && ModOptions.enableAging.Value && self.PupType() != null && self.PupType().hasAdultModule && !self.PupType().adultModule.disableAging)
            {
                if (!self.Malnourished && self.foodInStomach >= self.PupType().maxFood)
                {
                    self.PupState().age++;
                }
                if (!self.forceFullGrown && self.PupState().age >= Mathf.Clamp(Mathf.RoundToInt(ModOptions.cyclesTillGrown.Value * self.PupType().adultModule.agingMultiplier), 1, 100000))
                {
                    self.forceFullGrown = true;
                    self.foodInStomach = self.PupType().adultModule.foodToHibernate;
                }
            }

            orig(self);
        }

        public static void IL_PlayerNPCState_CycleTick(ILContext il)
        {
            ILCursor foodCurs = new(il);    

            while (foodCurs.TryGotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Slugpup))))
            {
                foodCurs.Emit(OpCodes.Ldarg_0);
                foodCurs.EmitDelegate((SlugcatStats.Name slugpup, PlayerNPCState self) =>
                {
                    return self.PupState().pupType != null ? self.PupType().hasAdultModule && self.forceFullGrown ? self.PupType().adultModule.name : self.PupType().name : slugpup;
                }); 
            }
        }
    }
}