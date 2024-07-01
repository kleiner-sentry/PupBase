namespace PupBase
{
    public class PupType
    {
        public class SpawnModifiers
        {
            public string str;
            public int type;
            public float multiplier;
            public bool exclusive;

            /// <summary>
            /// Allows you to choose if your Puptype will be chosen more or less freqently in specific regions/campaigns.
            /// </summary>
            /// <param name="str">Can represent either the region ID or the name of the campaign the pup is in.</param>
            /// <param name="type">0 = Campaign, 1 = Region</param>
            /// <param name="multiplier">How much this modifier will multiply its spawn weight.</param>
            /// <param name="exclusive">Will this pup only spawn in this region/campaign?</param>
            public SpawnModifiers(string str, int type, float multiplier, bool exclusive = false)
            {
                this.str = str;
                this.type = type;
                this.multiplier = multiplier;
                this.exclusive = exclusive;
            }

            public override string ToString()
            {
                return "{ " + str + ", " + multiplier + (exclusive ? ", Exclusive" : "") + " }";
            }
        }

        public string modName;
        public SlugcatStats.Name name;

        public List<SpawnModifiers> spawnModifiersList;

        public int foodToHibernate;
        public int maxFood;
        public bool hideInMenu;

        public int defaultSpawnWeight;
        public Configurable<int> config;
        public int spawnWeight
        {
            get { return config != null ? config.Value : defaultSpawnWeight; }
        }

        /// <summary>
        /// Creates a new PupType.
        /// </summary>
        /// <param name="modName">Your mod name.</param>
        /// <param name="name">The name to identify the PupType by. It's recommended that you register your own names.</param>
        /// <param name="spawnWeight">The weighted probability of this Slugpup spawning. Default is 100.</param>
        /// <param name="foodToHibernate">(OPTIONAL) The amount of food needed to hibernate.</param>
        /// <param name="maxFood">(OPTIONAL) The maximum food this pup can hold.</param>
        /// <param name="hideInMenu">(OPTIONAL) Hide this pup in the story menu.</param>
        /// <param name="disableCustomSpawnWeight">(OPTIONAL) Intended for those who use a custom campaign, and wish to prevent users from modifying its spawn weight.</param>
        /// <param name="spawnModifiersList">(OPTIONAL) Allows you to choose which region/campaign this type will be chosen more or less freqently in.</param>
        public PupType(string modName, SlugcatStats.Name name, int spawnWeight = 100, int foodToHibernate = 2, int maxFood = 3, bool hideInMenu = true, bool disableCustomSpawnWeight = false, List<SpawnModifiers> spawnModifiersList = null)
        {
            this.modName = string.IsNullOrEmpty(modName) ? this.modName = "???" : modName;
            if (name != null)
            {
                this.name = name;
            }
            else
            {
                Plugin.ModLogger.LogWarning(this.modName + ", Please assign a name!!");
                this.name = new SlugcatStats.Name("Testpup");
            }
            defaultSpawnWeight = spawnWeight;

            this.spawnModifiersList = spawnModifiersList;

            this.foodToHibernate = foodToHibernate;
            this.maxFood = maxFood;
            this.hideInMenu = hideInMenu;

            config = disableCustomSpawnWeight ? null : ModOptions.Instance.config.Bind("SpawnWeight" + this.name, defaultSpawnWeight, new ConfigurableInfo("Set how common this type of pup will be.", new ConfigAcceptableRange<int>(0, 1000)));
        }

        /// <summary>
        /// Calculates its actual spawn weight if it's in a region that it's supposed to spawn more/less in.
        /// </summary>
        /// <param name="world">Used to calculate if the pup is currently in a region.</param>
        /// <param name="debug">Outputs the result to the log.</param>
        /// <returns>Returns the proper spawn weight.</returns>
        public float CalculateWeight(World world, bool debug = false)
        {
            // Check if the pup is in a region. If so, then compare. Otherwise just return spawnWeight.
            if (world != null && world.game.IsStorySession && spawnModifiersList != null && spawnModifiersList.Count > 0)
            {
                float tempWeight = spawnWeight;
                bool isExclusiveCampaign = false;
                bool inExclusiveCampaign = false;
                bool isExclusiveRegion = false;
                bool inExclusiveRegion = false;
                foreach (SpawnModifiers spawnModifiers in spawnModifiersList)
                {
                    if (spawnModifiers.exclusive)
                    {
                        isExclusiveCampaign = spawnModifiers.type == 0 ? true : isExclusiveCampaign;
                        isExclusiveRegion = spawnModifiers.type == 1 ? true : isExclusiveRegion;
                    }
                    if (world.game.StoryCharacter.value.Equals(spawnModifiers.str) && spawnModifiers.type == 0)
                    {
                        tempWeight *= spawnModifiers.multiplier;
                        inExclusiveCampaign = spawnModifiers.exclusive ? true : inExclusiveCampaign;

                        if (debug)
                        {
                            Plugin.ModLogger.LogDebug("Spawn weight for " + name + " in " + world.game.StoryCharacter + "'s campaign is " + tempWeight);
                        }
                    }
                    if (world.region.name.Equals(spawnModifiers.str) && spawnModifiers.type == 1)
                    {
                        tempWeight *= spawnModifiers.multiplier;
                        inExclusiveRegion = spawnModifiers.exclusive ? true : inExclusiveRegion;
                        
                        if (debug)
                        {
                            Plugin.ModLogger.LogDebug("Spawn weight for " + name + " in " + world.region.name + " is " + tempWeight);
                        }
                    }
                }
                if (isExclusiveCampaign && !inExclusiveCampaign)
                {
                    return 0;
                }
                if (isExclusiveRegion && !inExclusiveCampaign)
                {
                    return 0;
                }
                return tempWeight;
            }
            if (debug)
            {
                Plugin.ModLogger.LogDebug("Spawn weight for " + name + " is " + spawnWeight);
            }
            return spawnWeight;
        }

        public string SpawnModifiersToString()
        {
            if (spawnModifiersList != null && spawnModifiersList.Count > 0)
            {
                string str = "{ ";
                int i = 1;
                foreach (SpawnModifiers spawnModifiers in spawnModifiersList)
                {
                    str += spawnModifiers.ToString() + (i < spawnModifiersList.Count ? ", " : "");
                    i++;
                }
                str += " }";
                return str;
            }
            return null;
        }
    }
}