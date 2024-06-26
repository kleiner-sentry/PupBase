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

        public static Configurable<bool> enableDebug = Instance.config.Bind(nameof(enableDebug), false, new ConfigurableInfo("Allow the mod to send information to the console.", null, "", "Enable Debug mode"));

        public override void Initialize()
        {
            base.Initialize();

            OpTab settingsTab = new OpTab(this, "Settings");
            OpTab registryTab = new OpTab(this, "Registry");

            Tabs = new OpTab[] { settingsTab, registryTab };

            InitSettings(0);
            InitRegistry(1);
        }

        private void InitSettings(int tabIndex)
        {
            UIelement[] opts = new UIelement[] 
            {
                new OpLabel(new Vector2(200f, 550f), new Vector2(200, 30), text: "Settings", bigText: true),
                new OpLabel(new Vector2(200f, 525f), new Vector2(200, 30), text: "----------------------------------------------------", bigText: true),
                new OpLabel(50f, 475f, "Enable Debug Mode: "),
                new OpCheckBox(enableDebug, new Vector2(175f, 475f))
            };

            Tabs[tabIndex].AddItems(opts);
        }

        private void InitRegistry(int tabIndex)
        {
            var vsp = new OpScrollBox(Tabs[tabIndex], 2000);

            List<UIelement> PupUIList = new List<UIelement> { };

            int interval = 14;
            int intervalBetweenCMs = 30;
            int i = 0;
            float vSize = (interval * 5 + intervalBetweenCMs) * PupManager.GetPupTypeList().Count + 100;

            PupUIList.Add(new OpLabel(new Vector2(200f, 550f), new Vector2(200, 30), text: "Registry", bigText: true));
            PupUIList.Add(new OpLabel(new Vector2(200f, 525f), new Vector2(200, 30), text: "----------------------------------------------------", bigText: true));
            PupUIList.Add(new OpLabel(50f, 500f, PupManager.GetPupTypeList().Count + " Pup type" + (PupManager.GetPupTypeList().Count > 1 ? "s registered." : " registered.")));

            foreach (PupType type in PupManager.GetPupTypeList())
            {
                float vpos = vSize - 100 - i * (interval * 4 + intervalBetweenCMs);

                /*
                customSpawnWeightArray[i, 0] = Instance.config.Bind("", Mathf.Clamp(type.spawnWeight, int.MinValue, int.MaxValue));
                customSpawnWeightArray[i, 1] = type;
                */

                /*
                AddTextLabel("ID: " + type.name.value);
                DrawTextLabels(ref Tabs[tabIndex]);
                AddTextLabel("Default Spawn Weight: " + type.spawnWeight.ToString());
                DrawTextLabels(ref Tabs[tabIndex]);
                AddSlider((Configurable<int>)customSpawnWeightArray[i, 0], "Spawn Weight");
                DrawFloatSliders(ref Tabs[tabIndex]);  
                */

                PupUIList.Add(new OpLabel(50, 575 - vpos, "Mod:"));
                PupUIList.Add(new OpLabel(175, 575 - vpos, type.modName));
                PupUIList.Add(new OpLabel(50, 575 - vpos - interval, "Name:"));
                PupUIList.Add(new OpLabel(175, 575 - vpos - interval, type.name.value));
                PupUIList.Add(new OpLabel(50, 575 - vpos - interval * 2, "Spawn Weight:"));
                PupUIList.Add(new OpLabel(175, 575 - vpos - interval * 2, type.spawnWeight.ToString()));
                PupUIList.Add(new OpLabel(50, 575 - vpos - interval * 3, "Food:"));
                PupUIList.Add(new OpLabel(175, 575 - vpos - interval * 3, type.foodToHibernate + " - " + type.maxFood));
                PupUIList.Add(new OpLabel(50, 575 - vpos - interval * 4, "Region Modifiers:"));
                PupUIList.Add(new OpLabel(175, 575 - vpos - interval * 4, (type.RegionMultToString() != null ? type.RegionMultToString() : "N/A")));
                //customMeowList.Add(weightUpDown);

                i++;
            }

            vsp.contentSize = vSize;
            vsp.AddItems(PupUIList.ToArray());

        }
    }
}