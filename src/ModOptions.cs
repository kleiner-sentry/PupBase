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

        public static Configurable<bool> enableAging = Instance.config.Bind(nameof(enableAging), false, new ConfigurableInfo("Allow pups to grow up after a few cycles with a full stomach.", null, "", "Enable Aging"));

        public static Configurable<bool> nonBaseAging = Instance.config.Bind(nameof(nonBaseAging), false, new ConfigurableInfo("Allow non-pupbase pups to age. Use with caution.", null, "", "Aging Compatibility"));

        public static Configurable<int> cyclesTillGrown = Instance.config.Bind(nameof(cyclesTillGrown), 10, new ConfigurableInfo("How many cycles must the pup survive with a full stomach until it grows up.", new ConfigAcceptableRange<int>(1, 100000), "", ""));

        private static UIelement[] list = [];

        public override void Initialize()
        {
            base.Initialize();

            OpTab settingsTab = new OpTab(this, "Settings");
            OpTab registryTab = new OpTab(this, "Registry");

            Tabs = [ settingsTab, registryTab ];

            OpContainer settingsContainer = new(new Vector2(0, 0));
            settingsTab.AddItems(settingsContainer);

            OpContainer registryContainer = new(new Vector2(0, 0));
            registryTab.AddItems(registryContainer);

            // Initialize Settings
            UIelement[] opts =
            [
                new OpLabel(new Vector2(200f, 550f), new Vector2(200, 30), text: "Settings", bigText: true),
                new OpLabel(new Vector2(200f, 525f), new Vector2(200, 30), text: "----------------------------------------------------", bigText: true),
                new OpLabel(50f, 475f, "Enable Debug Mode: "),
                new OpCheckBox(enableDebug, new Vector2(200f, 475f)),
                new OpLabel(new Vector2(400f, 475f), new Vector2(7, 20), text: "---Aging---", bigText: false),
                new OpLabel(new Vector2(400f, 445f), new Vector2(5, 15), text: "If your Slugpup can hibernate with a full stomach,\n they will grow older. After enough cycles has passed,\n they'll become an adult.", bigText: false),
                new OpLabel(300f, 390f, "Enable Aging: "),
                new OpCheckBox(enableAging, new Vector2(450f, 390f)),
                new OpLabel(300f, 340f, "Aging Compatibility: "),
                new OpCheckBox(nonBaseAging, new Vector2(450f, 340f)),
                new OpLabel(300f, 290f, "Cycles until Adult: "),
                new OpUpdown(cyclesTillGrown, new Vector2(450f, 290f), 75)
            ];

            settingsTab.AddItems(opts);

            // Initialize Registry
            OpScrollBox scrollBox = new(Vector2.zero, new Vector2(600, 450), 2000);
            registryTab.AddItems(scrollBox); // create a scrollbox and add it to the registry tab

            OpSimpleButton refreshButton = new(new Vector2(450, 480), new Vector2(70, 30), "Refresh") { description = "Refresh the list" };
            OpLabel countLabel = new(50f, 490f, PupManager.GetPupTypeList().Count + " Pup type" + (PupManager.GetPupTypeList().Count > 1 ? "s registered." : " registered."));

            refreshButton.OnClick += (UIfocusable e) =>
            {
                foreach (UIelement item in list)
                {
                    if (registryTab.items.TryGetValue(item, out UIelement tabItem))
                    {
                        if (tabItem is OpUpdown)
                        {
                            (tabItem as OpUpdown).Unload();
                        }
                        registryTab._RemoveItem(tabItem);
                    }
                } // remove every item in the scrollbox

                (list, float vertSize) = GenerateList(); // create a new list of items and store it in the private variable to access later

                scrollBox.contentSize = vertSize;
                scrollBox.AddItems(list); // fill the scrollbox with a new list 

                countLabel.text = PupManager.GetPupTypeList().Count + " Pup type" + (PupManager.GetPupTypeList().Count > 1 ? "s registered." : " registered.");
            };

            UIelement[] registryHeader =
            [
                new OpLabel(new Vector2(200f, 550f), new Vector2(200, 30), text: "Registry", bigText: true),
                new OpLabel(new Vector2(200f, 525f), new Vector2(200, 30), text: "----------------------------------------------------", bigText: true),
                countLabel,
                refreshButton
            ];

            registryTab.AddItems(registryHeader); // add the header to the registry tab

            (list, float vertSize) = GenerateList(); // create a new list of items and store it in the private variable to access later

            scrollBox.contentSize = vertSize;
            scrollBox.AddItems(list); // fill the scrollbox with a new list 
        }

        private (UIelement[], float) GenerateList()
        {
            int interval = 30;
            int intervalBetweenPTs = 45;
            int i = 0;
            float vSize = (interval * 5 + intervalBetweenPTs) * PupManager.GetPupTypeList().Count + 100;

            List<UIelement> PupUIList = [];

            foreach (PupType type in PupManager.GetPupTypeList())
            {
                float vpos = vSize - 100 - i * (interval * (type.HasAdultModule ? 6 : 5) + intervalBetweenPTs);

                PupUIList.Add(new OpLabel(50, 625 - vpos, "Mod:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos, type.modName));
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval, "Name:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos - interval, type.name.value));
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval * 2, "Spawn Weight:"));
                if (type.customSpawnWeight)
                {
                    PupUIList.Add(new OpUpdown(type.spawnWeightConfig, new Vector2(175, 620 - vpos - interval * 2), 75) { description = "Change the base chance this puptype can spawn over other puptypes, from 0-1000. Default: " + type.defaultSpawnWeight.ToString() });
                }
                else
                {
                    PupUIList.Add(new OpLabel(175, 625 - vpos - interval * 2, type.SpawnWeight.ToString()));
                }
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval * (type.HasAdultModule ? 4 : 3), "Food:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos - interval * (type.HasAdultModule ? 4 : 3), type.foodToHibernate + " - " + type.maxFood));
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval * (type.HasAdultModule ? 5 : 4), "Spawn Modifiers:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos - interval * (type.HasAdultModule ? 5 : 4), (type.regionModifiers == null && type.campaignModifiers == null) ? "N/A" : type.ModifiersToString()));

                if (type.HasAdultModule)
                {
                    PupUIList.Add(new OpLabel(325, 625 - vpos - interval, type.adultModule.name.value));
                    PupUIList.Add(new OpLabel(50, 625 - vpos - interval * 3, "Adult Chance:"));
                    if (type.adultModule.customAdultChance)
                    {
                        PupUIList.Add(new OpUpdown(type.adultModule.adultChanceConfig, new Vector2(175, 620 - vpos - interval * 3), 75) { description = "Change the chance a newly spawned pup will be an adult, from 0-100%. Default: " + type.adultModule.defaultAdultChance.ToString() });
                    }
                    else
                    {
                        PupUIList.Add(new OpLabel(175, 625 - vpos - interval * 3, type.adultModule.defaultAdultChance.ToString()));
                    }
                    PupUIList.Add(new OpLabel(325, 625 - vpos - interval * 4, type.adultModule.foodToHibernate + " - " + type.adultModule.maxFood));
                }

                i++;
            }

            return (PupUIList.ToArray(), vSize);
        }
    }
}