﻿namespace PupBase
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

        public static Configurable<int> cyclesTillGrown = Instance.config.Bind(nameof(cyclesTillGrown), 10, new ConfigurableInfo("How many cycles must the pup survive with a full stomach until it grows up.", new ConfigAcceptableRange<int>(1, 100000), "", ""));

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
                new OpLabel(new Vector2(400f, 475f), new Vector2(7, 20), text: "---Aging---", bigText: false),
                new OpLabel(new Vector2(400f, 445f), new Vector2(5, 15), text: "If your Slugpup can hibernate with a full stomach,\n they will grow older. After enough cycles has passed,\n they'll become an adult.", bigText: false),
                new OpLabel(300f, 390f, "Enable Aging: "),
                new OpCheckBox(enableAging, new Vector2(450f, 390f)),
                new OpLabel(300f, 340f, "Cycles until Adult: "),
                new OpUpdown(cyclesTillGrown, new Vector2(450f, 340f), 75)
            ];

            Tabs[0].AddItems(opts);

            // Initialize Registry
            var vsp = new OpScrollBox(Tabs[1], 2000);

            List<UIelement> PupUIList = new List<UIelement> { };

            int interval = 30;
            int intervalBetweenPTs = 45;
            int i = 0;
            float vSize = (interval * 5 + intervalBetweenPTs) * PupManager.GetPupTypeList().Count + 100;

            PupUIList.Add(new OpLabel(new Vector2(200f, 550f), new Vector2(200, 30), text: "Registry", bigText: true));
            PupUIList.Add(new OpLabel(new Vector2(200f, 525f), new Vector2(200, 30), text: "----------------------------------------------------", bigText: true));
            PupUIList.Add(new OpLabel(50f, 500f, PupManager.GetPupTypeList().Count + " Pup type" + (PupManager.GetPupTypeList().Count > 1 ? "s registered." : " registered.") + " Note: any changes made outside Rainworld requires a restart!"));

            foreach (PupType type in PupManager.GetPupTypeList())
            {
                float vpos = vSize - 100 - i * (interval * (type.hasAdultModule ? 6 : 5) + intervalBetweenPTs);

                PupUIList.Add(new OpLabel(50, 625 - vpos, "Mod:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos, type.modName));
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval, "Name:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos - interval, type.name.value));
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval * 2, "Spawn Weight:"));
                if (type.customSpawnWeight)
                {
                    PupUIList.Add(new OpUpdown(type.spawnWeightConfig, new Vector2(175, 620 - vpos - interval * 2), 75));
                    PupUIList.Add(new OpLabel(255, 625 - vpos - interval * 2, "Default: " + type.defaultSpawnWeight.ToString()));
                }
                else
                {
                    PupUIList.Add(new OpLabel(175, 625 - vpos - interval * 2, type.spawnWeight.ToString()));
                }
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval * (type.hasAdultModule ? 4 : 3), "Food:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos - interval * (type.hasAdultModule ? 4 : 3), type.foodToHibernate + " - " + type.maxFood));
                PupUIList.Add(new OpLabel(50, 625 - vpos - interval * (type.hasAdultModule ? 5 : 4), "Spawn Modifiers:"));
                PupUIList.Add(new OpLabel(175, 625 - vpos - interval * (type.hasAdultModule ? 5 : 4), (type.regionModifiers == null && type.campaignModifiers == null ) ? "N/A" : type.ModifiersToString()));

                if (type.hasAdultModule)
                {
                    PupUIList.Add(new OpLabel(325, 625 - vpos - interval, type.adultModule.name.value));
                    PupUIList.Add(new OpLabel(50, 625 - vpos - interval * 3, "Adult Chance:"));
                    if (type.adultModule.customAdultChance)
                    {
                        PupUIList.Add(new OpUpdown(type.adultModule.adultChanceConfig, new Vector2(175, 620 - vpos - interval * 3), 75));
                        PupUIList.Add(new OpLabel(255, 625 - vpos - interval * 3, "Default: " + type.adultModule.defaultAdultChance.ToString()));
                    }
                    else
                    {
                        PupUIList.Add(new OpLabel(175, 625 - vpos - interval * 3, type.spawnWeight.ToString()));
                    }
                    PupUIList.Add(new OpLabel(325, 625 - vpos - interval * 4, type.adultModule.foodToHibernate + " - " + type.adultModule.maxFood));
                }

                i++;
            }

            vsp.contentSize = vSize;
            vsp.AddItems(PupUIList.ToArray());
        }
    }
}