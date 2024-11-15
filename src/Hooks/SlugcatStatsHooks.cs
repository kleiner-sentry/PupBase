namespace PupBase.Hooks
{
    public class SlugcatStatsHooks
    {
        public static void Init()
        {
            On.SlugcatStats.SlugcatFoodMeter += SlugcatStats_SlugcatFoodMeter;
            On.SlugcatStats.HiddenOrUnplayableSlugcat += SlugcatStats_HiddenOrUnplayableSlugcat;

        }

        public static IntVector2 SlugcatStats_SlugcatFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
        {
            if (PupManager.TryGetAdultType(slugcat, out var adultType))
            {
                return new IntVector2(adultType.adultMaxFood, adultType.adultFoodToHibernate);
            }
            else if (PupManager.TryGetPupType(slugcat, out var pupType))
            {
                return new IntVector2(pupType.maxFood, pupType.foodToHibernate);
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