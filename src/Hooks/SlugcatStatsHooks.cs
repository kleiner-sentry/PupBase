using static Player;

namespace PupBase.Hooks
{
    public class SlugcatStatsHooks
    {
        public static void Init()
        {
            On.SlugcatStats.SlugcatFoodMeter += SlugcatStats_SlugcatFoodMeter;
            On.SlugcatStats.HiddenOrUnplayableSlugcat += SlugcatStats_HiddenOrUnplayableSlugcat;
            On.SlugcatStats.ctor += SlugcatStats_ctor;
            On.Player.Grabability += Player_Grabability;

            IL.Player.NPCStats.ctor += Player_NPCStats_ctorIL;
            IL.SlugcatHand.Update += IL_SlugcatHand_Update;
        }

        public static IntVector2 SlugcatStats_SlugcatFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
        {
            if (PupManager.TryGetPupType(slugcat, out PupType pupType))
            {
                if (PupManager.isAdultName(slugcat))
                {
                    return new IntVector2(pupType.adultModule.maxFood, pupType.adultModule.foodToHibernate);
                }
                return new IntVector2(pupType.maxFood, pupType.foodToHibernate);
            }
            return orig(slugcat);
        }

        public static bool SlugcatStats_HiddenOrUnplayableSlugcat(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if (PupManager.TryGetPupType(i, out PupType type) && type.hideInMenu)
            {
                return true;
            }
            return orig(i);
        }

        public static void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
        {
            orig(self, slugcat, malnourished);

            if (slugcat == SlugpupNames.SlugpupAdult)
            {
                self.bodyWeightFac = malnourished ? 0.9f : 1f;
                self.visualStealthInSneakMode = 0.5f;
                self.loudnessFac = 1f;
                self.lungsFac = 1f;
                self.throwingSkill = 1;
                self.poleClimbSpeedFac = malnourished ? 0.8f : 1f;
                self.corridorClimbSpeedFac = malnourished ? 0.86f : 1f;
                self.runspeedFac = malnourished ? 0.875f : 1f;
            }
        }

        public static ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (obj is Player && obj != self && !self.isSlugpup && (obj as Player).SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Slugpup && (obj as Player).playerState.forceFullGrown)
            {
                return ObjectGrabability.OneHand;
            }
            return orig(self, obj);
        }

        public static void Player_NPCStats_ctorIL(ILContext il)
        {
            ILCursor statsCurs = new ILCursor(il);
            statsCurs.GotoNext(MoveType.After, (Instruction x) => x.MatchStfld<NPCStats>("EyeColor"));
            statsCurs.Emit(OpCodes.Ldarg_0);
            statsCurs.Emit(OpCodes.Ldarg_1);
            statsCurs.EmitDelegate(delegate (NPCStats self, Player player)
            {
                if (player.PupType() != null && player.PupType().hasAdultModule && player.PupType().adultModule?.name == SlugpupNames.SlugpupAdult)
                {
                    player.abstractCreature.personality.sympathy = Random.value;
                    player.abstractCreature.personality.bravery = Random.value;
                    player.abstractCreature.personality.sympathy = Mathf.Clamp(Custom.PushFromHalf(player.abstractCreature.personality.sympathy / 1.2f - 0.15f, 0.6f), 0f, 1f);
                    player.abstractCreature.personality.bravery = Mathf.Clamp(Custom.PushFromHalf(player.abstractCreature.personality.bravery + 0.01f * (1.5f * player.abstractCreature.personality.bravery), 1.05f), 0f, 1f);
                    player.abstractCreature.personality.nervous = Mathf.Lerp(Random.value, Mathf.Lerp(player.abstractCreature.personality.energy, 1f - player.abstractCreature.personality.bravery, 0.5f), Mathf.Pow(Random.value, 0.25f));
                    player.abstractCreature.personality.aggression = Mathf.Lerp(Random.value, (player.abstractCreature.personality.energy + player.abstractCreature.personality.bravery) / 2f * (1f - player.abstractCreature.personality.sympathy), Mathf.Pow(Random.value, 0.25f));
                    player.abstractCreature.personality.dominance = Mathf.Lerp(Random.value, (player.abstractCreature.personality.energy + player.abstractCreature.personality.bravery + player.abstractCreature.personality.aggression) / 3f, Mathf.Pow(Random.value, 0.25f));
                    player.abstractCreature.personality.nervous = Custom.PushFromHalf(player.abstractCreature.personality.nervous, 2.5f);
                    player.abstractCreature.personality.aggression = Mathf.Clamp(Custom.PushFromHalf(player.abstractCreature.personality.aggression + 0.1f + 0.05f * (0.25f * player.abstractCreature.personality.aggression), 1.35f), 0f, 1f);
                }
            });
        }

        public static void IL_SlugcatHand_Update(ILContext il)
        {
            ILCursor statsCurs = new ILCursor(il);
            statsCurs.GotoNext((Instruction x) => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Slugpup"));
            statsCurs.GotoNext(MoveType.Before, (Instruction x) => x.MatchBrfalse(out _));
            statsCurs.Emit(OpCodes.Ldarg, 0);
            statsCurs.EmitDelegate(delegate (bool isPup, SlugcatHand self)
            {
                return isPup && !(self.owner.owner as Player).playerState.forceFullGrown;
            });
        }
    }
}   