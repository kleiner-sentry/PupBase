using static PupBase.PupType;

namespace PupBase
{
    public class PupType
    {
        public class SpawnModifiers
        {
            public PupType pupType;
            public string str;
            public int type;
            public float multiplier;
            public bool exclusive;
            public bool unique;

            /// <summary>
            /// Allows you to choose if your Puptype will be chosen more or less freqently in specific regions/campaigns.
            /// </summary>
            /// <param name="str">Can represent either the region ID or the name of the campaign the pup is in.</param>
            /// <param name="type">0 = Campaign, 1 = Region</param>
            /// <param name="multiplier">How much this modifier will multiply its spawn weight.</param>
            /// <param name="exclusive">A tag used to make this specific campaign/region only allow your kind of pup. If another PupType has this exclusive tag in the same region as this is used in, they'll both spawn.</param>
            /// <param name="unique">Make this kind of pup only spawn in this campaign/region.</param>
            public SpawnModifiers(string str, int type, float multiplier, bool exclusive = false, bool unique = false)
            {
                this.str = str;
                this.type = type;
                this.multiplier = multiplier;
                this.exclusive = exclusive;
                this.unique = unique;
            }

            public string ToStringCustom()
            {
                return "{ " + str + ", " + multiplier + (exclusive ? ", Exclusive" : "") + (unique ? ", Unique" : "") + " }";
            }
        }

        public string modName;
        public SlugcatStats.Name name;

        public List<SpawnModifiers> spawnModifiersList;

        public int foodToHibernate;
        public int maxFood;
        public bool mature;

        public bool allowSpawningInArena;
        public bool hideInMenu;
        public bool pioritize = false; // let this instance spawn over pups+ variants

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
        /// <param name="mature">(OPTIONAL) Will this pup be an adult or </param>
        /// <param name="hideInMenu">(OPTIONAL) Hide this pup in the story menu.</param>
        /// <param name="allowSpawningInArena">(OPTIONAL) Intended for those who use a custom campaign, and wish to prevent users from allowing this pup from spawning in Arena.</param>
        /// <param name="disableCustomSpawnWeight">(OPTIONAL) Intended for those who use a custom campaign, and wish to prevent users from modifying its spawn weight.</param>
        /// <param name="spawnModifiersList">(OPTIONAL) Allows you to choose which region/campaign this type will be chosen more or less freqently in.</param>
        public PupType(string modName, SlugcatStats.Name name, int spawnWeight = 100, int foodToHibernate = 2, int maxFood = 3, bool mature = false, bool hideInMenu = true, bool allowSpawningInArena = true, bool disableCustomSpawnWeight = false, List<SpawnModifiers> spawnModifiersList = null)
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

            this.foodToHibernate = foodToHibernate;
            this.maxFood = maxFood;
            this.mature = mature;

            this.allowSpawningInArena = allowSpawningInArena;
            this.hideInMenu = hideInMenu;

            defaultSpawnWeight = spawnWeight;

            if (spawnModifiersList != null)
            {
                this.spawnModifiersList = spawnModifiersList;
                foreach (SpawnModifiers modifiers in this.spawnModifiersList)
                {
                    modifiers.pupType = this;
                }
            }

            config = disableCustomSpawnWeight ? null : ModOptions.Instance.config.Bind("SpawnWeight" + this.name, defaultSpawnWeight, new ConfigurableInfo("Set how common this type of pup will be.", new ConfigAcceptableRange<int>(0, 1000)));
        }

        /// <summary>
        /// Calculates its actual spawn weight if it's in a region that it's supposed to spawn more/less in.
        /// </summary>
        /// <param name="world">Used to calculate if the pup is currently in a region.</param>
        /// <param name="debug">Outputs the results to the log.</param>
        /// <returns>Returns the proper spawn weight.</returns>
        public float CalculateWeight(World world, bool debug = false)
        {
            if (world != null && !world.game.IsArenaSession)
            {
                // Exclusives
                bool otherExclusiveCampaign = false;
                bool inExclusiveCampaign = false;
                bool otherExclusiveRegion = false;
                bool inExclusiveRegion = false;
                // Uniques
                bool isUniqueCampaign = false;
                bool inUniqueCampaign = false;
                bool isUniqueRegion = false;
                bool inUniqueRegion = false;
                // Calculate
                float tempWeight = spawnWeight;
                // Exclusives
                foreach (SpawnModifiers modifiers in PupManager.spawnModifiersList)
                {
                    if (modifiers.pupType == this)
                    {
                        if (modifiers.type == 0)
                        {
                            if (modifiers.unique) isUniqueCampaign = true;
                            if (world.game.StoryCharacter.value == modifiers.str)
                            {
                                if (modifiers.exclusive) inExclusiveCampaign = true;
                                inUniqueCampaign = true;
                                tempWeight *= modifiers.multiplier;
                            }
                        }
                        if (modifiers.type == 1)
                        {
                            if (modifiers.unique) isUniqueRegion = true;
                            if (world.region.name == modifiers.str)
                            {
                                if (modifiers.exclusive) inExclusiveRegion = true;
                                inUniqueRegion = true;
                                tempWeight *= modifiers.multiplier;
                            }
                        }
                    }

                    if (modifiers.pupType != this && modifiers.exclusive)
                    {
                        if (modifiers.type == 0 && modifiers.str == world.game.StoryCharacter.value)
                        {
                            otherExclusiveCampaign = true;
                        }
                        if (modifiers.type == 1 && modifiers.str == world.region.name)
                        {
                            otherExclusiveRegion = true;
                        }
                    }
                }
                bool disabled = false;
                if ((otherExclusiveCampaign && !inExclusiveCampaign) || (otherExclusiveRegion && !inExclusiveRegion) || (isUniqueCampaign && !inUniqueCampaign) || (isUniqueRegion && !inUniqueRegion))
                {
                    disabled = true;
                }
                if (debug) Plugin.ModLogger.LogDebug(disabled ? name + " Will not spawn. Because: " + ((otherExclusiveCampaign && !inExclusiveCampaign) ? "Spawned in an excluded campaign." : (otherExclusiveRegion && !inExclusiveRegion) ? "Spawned in an excluded region." : (isUniqueCampaign && !inUniqueCampaign) ? "Didn't spawn in its unique campaign." : (isUniqueRegion && !inUniqueRegion) ? "Didn't spawn in its unique region." : "No reason...How did this happen?") : name + "s spawnWeight is: " + tempWeight);
                return disabled ? 0 : tempWeight;
            }
            if (debug) Plugin.ModLogger.LogDebug((!allowSpawningInArena && world.game.IsArenaSession) ? name + " Will not spawn in Arena mode." : name + "s spawnWeight is: " + spawnWeight);
            return (!allowSpawningInArena && world.game.IsArenaSession) ? 0 : spawnWeight;
        }

        public string SpawnModifiersToString()
        {
            if (spawnModifiersList != null && spawnModifiersList.Count > 0)
            {
                string str = "{ ";
                int i = 1;
                foreach (SpawnModifiers spawnModifiers in spawnModifiersList)
                {
                    str += spawnModifiers.ToStringCustom() + (i < spawnModifiersList.Count ? ", " : "");
                    i++;
                }
                str += " }";
                return str;
            }
            return null;
        }
    }
}