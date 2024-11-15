namespace PupBase
{
    /// <summary>
    /// Holds custom variables in PlayerState.
    /// </summary>
    public class PupStateEx
    {
        public PupType pupType;
        public bool pioritize = false; // let this instance spawn over pups+ variants
    }

    public static class PlayerStateExtension
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
        /// Returns the name of the PupType stored in PupStateEx
        /// </summary>
        public static PupType PupType(this PlayerState self) => self.PupState().pupType;

        /// <summary>
        /// Returns the name of the PupType stored in PupStateEx
        /// </summary>
        public static PupType PupType(this Player self) => self.PupState().pupType;

    }
}