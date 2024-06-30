﻿namespace PupBase.Hooks
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