namespace PupBase.Hooks
{
    public class SlugcatStatsHooks
    {
        public static void Init()
        {
            On.SlugcatStats.ctor += SlugcatStats_ctor;
            On.SlugcatStats.SlugcatFoodMeter += SlugcatStats_SlugcatFoodMeter;
            On.SlugcatStats.HiddenOrUnplayableSlugcat += SlugcatStats_HiddenOrUnplayableSlugcat;

        }

        public static void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
        {
            orig(self, slugcat, malnourished);

            if (ModOptions.enableDebug.Value)
            {
                Plugin.ModLogger.LogDebug(slugcat.value);
            }
            /*
            StackTrace st = new StackTrace(true);
            for (int i = 0; i < st.FrameCount; i++)
            {
                // Note that at this level, there are four
                // stack frames, one for each method invocation.
                StackFrame sf = st.GetFrame(i);
                Plugin.ModLogger.LogDebug(" Method: {0}" + sf.GetMethod());
                Plugin.ModLogger.LogDebug(" File: {0}" + sf.GetFileName());
                Plugin.ModLogger.LogDebug(" Line Number: {0}" + sf.GetFileLineNumber());
            }
            */

            /*
            if (self.PupStats().player != null && ModOptions.enableVariations.Value && PupManager.TryGetPupType(slugcat, out var puptype) && puptype.enableVariations)
            {
                Player.NPCStats stats = self.PupStats().player.npcStats;
                self.runspeedFac *= (0.85f + 0.15f * stats.Met + 0.15f * (1f - stats.Bal) + 0.1f * (1f - stats.Stealth));
                self.bodyWeightFac *= (0.85f + 0.15f * stats.Wideness + 0.1f * stats.Met);
                self.generalVisibilityBonus *= (0.8f + 0.2f * (1f - stats.Stealth) + 0.2f * stats.Met);
                self.visualStealthInSneakMode *= (0.75f + 0.35f * stats.Stealth + 0.15f * (1f - stats.Met));
                self.loudnessFac *= (0.8f + 0.2f * stats.Wideness + 0.2f * (1f - stats.Stealth));
                self.lungsFac *= (0.8f + 0.2f * (1f - stats.Met) + 0.2f * (1f - stats.Stealth));
                self.poleClimbSpeedFac *= (0.85f + 0.15f * stats.Met + 0.15f * stats.Bal + 0.1f * (1f - stats.Stealth));
                self.corridorClimbSpeedFac *= (0.85f + 0.15f * stats.Met + 0.15f * (1f - stats.Bal) + 0.1f * (1f - stats.Stealth));
            }
            */
        }

        public static IntVector2 SlugcatStats_SlugcatFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
        {
            if (PupManager.TryGetPupType(slugcat, out var type))
            {
                if (ModOptions.enableDebug.Value)
                {
                    Plugin.ModLogger.LogDebug("returning " + type.name + "'s food: " + type.maxFood + ", " + type.foodToHibernate);
                }
                return new IntVector2(type.maxFood, type.foodToHibernate);
            }
            return orig(slugcat);
        }

        public static bool SlugcatStats_HiddenOrUnplayableSlugcat(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if (PupManager.TryGetPupType(i, out var type) && type.hideInMenu)
            {
                return true;
            }
            return orig(i);
        }
    }
}   