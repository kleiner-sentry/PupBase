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

    public static class PlayerStateExtention
    {
        private static readonly ConditionalWeakTable<PlayerState, PupStateEx> cwtState = new();

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

    }
}