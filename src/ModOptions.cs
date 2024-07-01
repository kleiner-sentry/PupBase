namespace PupBase
{
    public class ModOptions : OptionInterface
    {
        public static ModOptions Instance { get; } = new();
        public static void RegisterOI()
        {
            if (MachineConnector.GetRegisteredOI(Plugin.MOD_ID) != Instance)
            {
                MachineConnector.SetRegisteredOI(Plugin.MOD_ID, Instance);
            }
        }

        public static Configurable<bool> enableDebug = Instance.config.Bind(nameof(enableDebug), false, new ConfigurableInfo("Allow the mod to send information to the console.", null, "", "Enable Debug mode"));

        public override void Initialize()
        {
            base.Initialize();

            OpTab settingsTab = new OpTab(this, "Settings");
            OpTab registryTab = new OpTab(this, "Registry");

            Tabs = [ settingsTab, registryTab ];

            // Initialize Settings
            UIelement[] opts =
            [
                new OpLabel(new Vector2(200f, 550f), new Vector2(200, 30), text: "Settings", bigText: true),
                new OpLabel(new Vector2(200f, 525f), new Vector2(200, 30), text: "----------------------------------------------------", bigText: true),
                new OpLabel(50f, 475f, "Enable Debug Mode: "),
                new OpCheckBox(enableDebug, new Vector2(200f, 475f)),
            ];

            Tabs[0].AddItems(opts);

            // Initialize Registry
            var vsp = new OpScrollBox(Tabs[1], 2000);

            List<UIelement> PupUIList = new List<UIelement> { };

            int interval = 25;
            int intervalBetweenPTs = 45;
            int i = 0;
            float vSize = (interval * 5 + intervalBetweenPTs) * PupManager.GetPupTypeList().Count + 100;

            PupUIList.Add(new OpLabel(new Vector2(200f, 550f), new Vector2(200, 30), text: "Registry", bigText: true));
            PupUIList.Add(new OpLabel(new Vector2(200f, 525f), new Vector2(200, 30), text: "----------------------------------------------------", bigText: true));
            PupUIList.Add(new OpLabel(50f, 500f, PupManager.GetPupTypeList().Count + " Pup type" + (PupManager.GetPupTypeList().Count > 1 ? "s registered." : " registered.") + " Note: any changes made outside Rainworld requires a restart!"));

            foreach (PupType type in PupManager.GetPupTypeList())
            {
                float vpos = vSize - 100 - i * (interval * 5 + intervalBetweenPTs);

                PupUIList.Add(new OpLabel(50, 625 - vpos, "Mod:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos, type.modName));
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval, "Name:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos - interval, type.name.value));
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval * 2, "Spawn Weight:"));
                if (type.config != null)
                {
                    PupUIList.Add(new OpUpdown(type.config, new Vector2(175, 620 - vpos - interval * 2), 75));
                    PupUIList.Add(new OpLabel(255, 625 - vpos - interval * 2, "Default: " + type.defaultSpawnWeight.ToString()));
                }
                else
                {
                    PupUIList.Add(new OpLabel(175, 625 - vpos - interval * 2, type.spawnWeight.ToString()));
                }
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval * 3, "Food:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos - interval * 3, type.foodToHibernate + " - " + type.maxFood));
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval * 4, "Spawn Modifiers:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos - interval * 4, (type.SpawnModifiersToString() != null ? type.SpawnModifiersToString() : "N/A")));

                i++;
            }

            vsp.contentSize = vSize;
            vsp.AddItems(PupUIList.ToArray());
        }
    }
}