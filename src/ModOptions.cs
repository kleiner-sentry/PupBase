namespace PupBase
{
    public class ModOptions : OptionsTemplate
    {
        public static ModOptions Instance { get; } = new();
        public static void RegisterOI()
        {
            if (MachineConnector.GetRegisteredOI(Plugin.MOD_ID) != Instance)
            {
                MachineConnector.SetRegisteredOI(Plugin.MOD_ID, Instance);
            }
        }

        public static Configurable<bool> inheritCamo = Instance.config.Bind(nameof(inheritCamo), true, new ConfigurableInfo("Allow non-stalker slugcats to inherit the camo ability when mounted on stalker's back.", null, "", "Inherit Camouflage"));

        private const int TAB_COUNT = 1;

        public override void Initialize()
        {
            base.Initialize();

            Tabs = new OpTab[TAB_COUNT];
            int tabIndex = -1;

            InitSettings(ref tabIndex);
        }

        public static readonly Color WarnRed = new(0.85f, 0.35f, 0.4f);

        private void InitSettings(ref int tabIndex)
        {
            AddTab(ref tabIndex, "Settings");

            AddCheckBox(inheritCamo);
            DrawCheckBoxes(ref Tabs[tabIndex]);
            AddNewLine(1);
        }
    }
}