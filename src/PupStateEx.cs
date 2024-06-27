using Newtonsoft.Json.Serialization;

namespace PupBase
{
    /// <summary>
    /// Holds custom variables in PlayerState.
    /// </summary>
    public class PupStateEx
    {
        public PupType pupType;
        public SlugcatStats.Name type;
    }

    public class PupStatsEx
    {
        public delegate void PostStatsModificationDelegate();
        public event PostStatsModificationDelegate PostStatsModification;

        public delegate void PostStatsConstructionDelegate(SlugcatStats stats, bool malnourished);
        public event PostStatsConstructionDelegate PostStatsConstruction;

        internal void StatsConstructed(SlugcatStats stats, bool malnourished)
        {
            PostStatsConstruction?.Invoke(stats, malnourished);
            PostStatsModification?.Invoke();
        }
    }

    public static class PlayerStateExtention
    {
        private static readonly ConditionalWeakTable<PlayerState, PupStateEx> cwtState = new();
        private static readonly ConditionalWeakTable<SlugcatStats, PupStatsEx> cwtStats = new();

        /// <summary>
        /// Gramts access to PupStateEx
        /// </summary>
        public static PupStateEx PupState(this PlayerState self) => cwtState.GetValue(self, _ => new PupStateEx());

        /// <summary>
        /// Gramts access to PupStateEx
        /// </summary>
        public static PupStateEx PupState(this Player self) => cwtState.GetValue(self.State as PlayerState, _ => new PupStateEx());

        /// <summary>
        /// Returns the type variable stored in PupStateEx
        /// </summary>
        public static SlugcatStats.Name PupType(this PlayerState self) => self.PupState().type;

        /// <summary>
        /// Returns the type variable stored in PupStateEx
        /// </summary>
        public static SlugcatStats.Name PupType(this Player self) => self.PupState().type;

        public static PupStatsEx PupStats(this SlugcatStats self) => cwtStats.GetValue(self, _ => new PupStatsEx());

        /// <summary>
        /// Allows you to modify the pups stats after its personality and pup stats have been defined. Good for creating variation between your pups. Highly recommended you use this after NPCStats is defined.
        /// </summary>
        /// <param name="whenMalnourished"></param>
        /// <param name="runSpeedFac"></param>
        /// <param name="bodyWeightFac"></param>
        /// <param name="generalVisibilityBonus"></param>
        /// <param name="visualStealthInSneakMode"></param>
        /// <param name="loudnessFac"></param>
        /// <param name="lungsFac"></param>
        /// <param name="throwingSkill"></param>
        /// <param name="poleClimbSpeedFac"></param>
        /// <param name="corridorClimbSpeedFac"></param>
        public static void PupBase_SetPhysicalStats(
            this Player self,
            float? runSpeedFac = null,
            float? bodyWeightFac = null,
            float? generalVisibilityBonus = null,
            float? visualStealthInSneakMode = null,
            float? loudnessFac = null,
            float? lungsFac = null,
            int? throwingSkill = null,
            float? poleClimbSpeedFac = null,
            float? corridorClimbSpeedFac = null)
        {
            if (ModOptions.enableDebug.Value)
            {
                void Delegate()
                {
                    SlugcatStats stats = self.slugcatStats;
                    stats.runspeedFac *= runSpeedFac ?? (0.85f + 0.15f * self.npcStats.Met + 0.15f * (1f - self.npcStats.Bal) + 0.1f * (1f - self.npcStats.Stealth));
                    stats.bodyWeightFac *= bodyWeightFac ?? (0.85f + 0.15f * self.npcStats.Wideness + 0.1f * self.npcStats.Met);
                    stats.generalVisibilityBonus *= generalVisibilityBonus ?? (0.8f + 0.2f * (1f - self.npcStats.Stealth) + 0.2f * self.npcStats.Met);
                    stats.visualStealthInSneakMode *= visualStealthInSneakMode ?? (0.75f + 0.35f * self.npcStats.Stealth + 0.15f * (1f - self.npcStats.Met));
                    stats.loudnessFac *= loudnessFac ?? (0.8f + 0.2f * self.npcStats.Wideness + 0.2f * (1f - self.npcStats.Stealth));
                    stats.lungsFac *= lungsFac ?? (0.8f + 0.2f * (1f - self.npcStats.Met) + 0.2f * (1f - self.npcStats.Stealth));
                    stats.throwingSkill *= throwingSkill ?? 0;
                    stats.poleClimbSpeedFac *= poleClimbSpeedFac ?? (0.85f + 0.15f * self.npcStats.Met + 0.15f * self.npcStats.Bal + 0.1f * (1f - self.npcStats.Stealth));
                    stats.corridorClimbSpeedFac *= corridorClimbSpeedFac ?? (0.85f + 0.15f * self.npcStats.Met + 0.15f * (1f - self.npcStats.Bal) + 0.1f * (1f - self.npcStats.Stealth));
                }
                self.slugcatStats.PupStats().PostStatsModification += Delegate;
            }
        }
    }
}