using static PupBase.PupType;

namespace PupBase
{
    public class PupType
    {
        public class RegionModifier
        {
            public string str;
            public float multiplier;
            public bool exclusive;
            public bool unique;

            /// <summary>
            /// Allows you to choose if your Puptype will be chosen more or less freqently in specific regions.
            /// </summary>
            /// <param name="str">Can represent either the region ID or the name of a campaign. If this is for Arena mode, then put "Arena".</param>
            /// <param name="multiplier">How much this modifier will multiply its spawn weight.</param>
            /// <param name="exclusive">A tag used to make this specific campaign/region only allow your kind of pup. If another PupType has this exclusive tag in the same region as this is used in, they'll both spawn.</param>
            /// <param name="unique">Make this kind of pup only spawn in this campaign/region.</param>
            public RegionModifier(string str, float multiplier, bool exclusive = false, bool unique = false)
            {
                this.str = str;
                this.multiplier = multiplier;
                this.exclusive = exclusive;
                this.unique = unique;
            }

            public override string ToString()
            {
                return "{ " + str + ", " + multiplier + (exclusive ? ", Exclusive" : "") + (unique ? ", Unique" : "") + " }";
            }
        }

        public class CampaignModifier
        {
            public string str;
            public float multiplier;
            public bool exclusive;
            public bool unique;

            /// <summary>
            /// Allows you to choose if your Puptype will be chosen more or less freqently in specific campaigns.
            /// </summary>
            /// <param name="str">Can represent either the region ID or the name of a campaign. If this is for Arena mode, then put "Arena".</param>
            /// <param name="multiplier">How much this modifier will multiply its spawn weight.</param>
            /// <param name="exclusive">A tag used to make this specific campaign/region only allow your kind of pup. If another PupType has this exclusive tag in the same region as this is used in, they'll both spawn.</param>
            /// <param name="unique">Make this kind of pup only spawn in this campaign/region.</param>
            public CampaignModifier(string str, float multiplier, bool exclusive = false, bool unique = false)
            {
                this.str = str;
                this.multiplier = multiplier;
                this.exclusive = exclusive;
                this.unique = unique;
            }

            public override string ToString()
            {
                return "{ " + str + ", " + multiplier + (exclusive ? ", Exclusive" : "") + (unique ? ", Unique" : "") + " }";
            }
        }

        public class AdultModule
        {
            public SlugcatStats.Name name;

            public int foodToHibernate = 4;
            public int maxFood = 7;

            public bool disableAging = false;

            public bool customAdultChance = true;
            internal Configurable<int> adultChanceConfig;
            public readonly int defaultAdultChance;
            public int adultChance
            {
                get { return (customAdultChance && adultChanceConfig != null) ? adultChanceConfig.Value : defaultAdultChance; }
            }

            public AdultModule(SlugcatStats.Name name, int adultChance = 10)
            {
                if (name == null)
                {
                    Plugin.ModLogger.LogWarning("Please assign a name!!");
                    this.name = new SlugcatStats.Name("TestpupAdult");
                }
                else
                {
                    this.name = name;
                }

                defaultAdultChance = Mathf.Clamp(adultChance, 0, 100);

                adultChanceConfig = ModOptions.Instance.config.Bind("AdultChance" + this.name, defaultAdultChance, new ConfigurableInfo("Set the percentage chance this pup will spawn as an adult.", new ConfigAcceptableRange<int>(0, 100)));
            }
        }

        public string modName;
        public SlugcatStats.Name name;

        public AdultModule adultModule;
        public bool hasAdultModule {  get { return adultModule != null; } }

        public List<RegionModifier> regionModifiers;
        public List<CampaignModifier> campaignModifiers;

        public int foodToHibernate = 2;
        public int maxFood = 3;
        //public bool mature;

        public bool hideInMenu = true; // let this show up in the story select menu.

        public bool customSpawnWeight = true;
        internal Configurable<int> spawnWeightConfig;
        public readonly int defaultSpawnWeight;
        public int spawnWeight { get { return (customSpawnWeight && spawnWeightConfig != null) ? spawnWeightConfig.Value : defaultSpawnWeight; } }

        /// <summary>
        /// Creates a new PupType. This constructor has every possible variable that you can customize.
        /// </summary>
        /// <param name="modName">Your mod name.</param>
        /// <param name="name">The name to identify the PupType by. Be sure to register your own names.</param>
        /// <param name="spawnWeight">The weighted chance that this puptype will be selected over other puptypes.</param>
        public PupType(string modName, SlugcatStats.Name name, int spawnWeight = 100)
        {
            this.modName = string.IsNullOrEmpty(modName) ? this.modName = "???" : modName;
            if (name == null)
            {
                Plugin.ModLogger.LogWarning(this.modName + ", Please assign a name!!");
                this.name = new SlugcatStats.Name("Testpup");
            }
            else
            {
                this.name = name;
            }

            defaultSpawnWeight = Mathf.Clamp(spawnWeight, 0, 1000);

            spawnWeightConfig = ModOptions.Instance.config.Bind("SpawnWeight" + this.name, defaultSpawnWeight, new ConfigurableInfo("Set how common this type of pup will be.", new ConfigAcceptableRange<int>(0, 1000)));
        }

        /// <summary>
        /// Calculates its proper spawnweight based on factors determined by its spawn modifiers, as well as other pups spawn modifiers. 
        /// </summary>
        /// <param name="world">Used to calculate if the pup is currently in a region.</param>
        /// <param name="debug">Outputs the results to the log.</param>
        /// <returns>Returns the proper spawn weight.</returns>
        public float CalculateWeight(World world, bool debug = false)
        {
            if (world != null && !world.game.IsArenaSession)
            {
                // Exclusives
                bool otherExclusiveRegion = false;
                bool inExclusiveRegion = false;
                bool otherExclusiveCampaign = false;
                bool inExclusiveCampaign = false;
                // Uniques
                bool isUniqueRegion = false;
                bool inUniqueRegion = false;
                bool isUniqueCampaign = false;
                bool inUniqueCampaign = false;
                // Calculate
                float tempWeight = spawnWeight;
                foreach (PupType type in PupManager.GetPupTypeList())
                {
                    if (type.regionModifiers != null)
                    {
                        foreach (RegionModifier regionModifier in type.regionModifiers)
                        {
                            if (type == this)
                            {
                                if (regionModifier.unique) isUniqueRegion = true;
                                if (world.region.name == regionModifier.str)
                                {
                                    if (regionModifier.exclusive) inExclusiveRegion = true;
                                    inUniqueRegion = true;
                                    tempWeight *= regionModifier.multiplier;
                                }
                            }
                            else if (regionModifier.exclusive && regionModifier.str == world.region.name)
                            {
                                otherExclusiveRegion = true;
                            }
                        }
                    }
                    if (type.campaignModifiers != null)
                    {
                        foreach (CampaignModifier campaignModifier in type.campaignModifiers)
                        {
                            if (type == this)
                            {
                                if (campaignModifier.unique) isUniqueCampaign = true;
                                if (world.game.StoryCharacter.value == campaignModifier.str)
                                {
                                    if (campaignModifier.exclusive) inExclusiveCampaign = true;
                                    inUniqueCampaign = true;
                                    tempWeight *= campaignModifier.multiplier;
                                }
                            }
                            else if (campaignModifier.exclusive && campaignModifier.str == world.game.StoryCharacter.value)
                            {
                                otherExclusiveCampaign = true;
                            }
                        }
                    }
                }
                bool disabled = false;
                if ((otherExclusiveRegion && !inExclusiveRegion) || (otherExclusiveCampaign && !inExclusiveCampaign) || (isUniqueRegion && !inUniqueRegion) || (isUniqueCampaign && !inUniqueCampaign))
                {
                    disabled = true;
                }
                if (debug) Plugin.ModLogger.LogDebug(disabled ? name + " Will not spawn. Because: " + ((otherExclusiveCampaign && !inExclusiveCampaign) ? "Spawned in an excluded campaign." : (otherExclusiveRegion && !inExclusiveRegion) ? "Spawned in an excluded region." : (isUniqueCampaign && !inUniqueCampaign) ? "Didn't spawn in its unique campaign." : (isUniqueRegion && !inUniqueRegion) ? "Didn't spawn in its unique region." : "No reason...How did this happen?") : name + "s spawnWeight is: " + tempWeight);
                return disabled ? 0 : tempWeight;
            }
            if (debug) Plugin.ModLogger.LogDebug(name + "s spawnWeight is: " + spawnWeight);
            return spawnWeight;
        }

        public string ModifiersToString()
        {
            string str = "";
            str += RegionsToString();
            if (!string.IsNullOrEmpty(str) && campaignModifiers != null)
            {
                str += ", " + CampaignsToString();
            }
            return str;
        }

        public string RegionsToString()
        {
            string str = "";
            if (regionModifiers != null && regionModifiers.Count > 0)
            {
                for (int i = 0; i < regionModifiers.Count; i++)
                {
                    str += regionModifiers[i].ToString() + (i + 1 == regionModifiers.Count ? "" : ", ");
                }
            }
            return str;
        }

        public string CampaignsToString()
        {
            string str = "";
            if (campaignModifiers != null && campaignModifiers.Count > 0)
            {
                for (int i = 0; i < campaignModifiers.Count; i++)
                {
                    str += campaignModifiers[i].ToString() + (i + 1 == campaignModifiers.Count ? "" : ", ");
                }
            }
            return str;
        }
    }
}