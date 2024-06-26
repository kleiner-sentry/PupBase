using static PupBase.PupType;

namespace PupBase
{
    public class PupType
    {
        public class SpawnModifiers
        {
            public string regionID;
            public float multiplier;

            /// <summary>
            /// Allows you to choose if your Puptype will be chosen more or less freqently in specific regions.
            /// </summary>
            /// <param name="regionID"></param>
            /// <param name="multiplier"></param>
            public SpawnModifiers(string regionID, float multiplier)
            {
                this.regionID = regionID;
                this.multiplier = multiplier;
            }

            public override string ToString()
            {
                return "{ " + regionID + ", " + multiplier + " }";
            }
        }

        public string modName;
        public SlugcatStats.Name name;
        public int spawnWeight;

        public List<SpawnModifiers> spawnModifiersList;

        public int foodToHibernate;
        public int maxFood;
        public bool hideInMenu;

        /// <summary>
        /// Creates a new PupType.
        /// </summary>
        /// <param name="modName">Your mod name.</param>
        /// <param name="name">The name to identify the PupType by. It's recommended that you register your own names.</param>
        /// <param name="spawnWeight">The weighted probability of this Slugpup spawning. Default is 100.</param>
        /// <param name="foodToHibernate">(OPTIONAL) The amount of food needed to hibernate.</param>
        /// <param name="maxFood">(OPTIONAL) The maximum food this pup can hold.</param>
        /// <param name="hideInMenu">(OPTIONAL) Hide this pup in the story menu.</param>
        /// <param name="regionMultList">(OPTIONAL) Allows you to choose which region this type will be chosen more or less freqently in.</param>
        public PupType(string modName, SlugcatStats.Name name, int spawnWeight = 100, int foodToHibernate = 2, int maxFood = 3, bool hideInMenu = true, List<SpawnModifiers> spawnModifiersList = null)
        {
            this.modName = string.IsNullOrEmpty(modName) ? this.modName = "???" : modName;
            if (name != null)
            {
                this.name = name;
            }
            else
            {
                Plugin.ModLogger.LogWarning(this.modName + ", Please assign a name!!");
                this.name = MoreSlugcatsEnums.SlugcatStatsName.Slugpup;
            }
            this.spawnWeight = spawnWeight;

            this.spawnModifiersList = spawnModifiersList;

            this.foodToHibernate = foodToHibernate;
            this.maxFood = maxFood;
            this.hideInMenu = hideInMenu;
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
                foreach (SpawnModifiers spawnModifiers in spawnModifiersList)
                {
                    if (world.region.name == spawnModifiers.regionID)
                    {
                        tempWeight *= spawnModifiers.multiplier;
                        if (debug)
                        {
                            Plugin.ModLogger.LogDebug("Spawn weight for " + name + " in " + world.region.name + " is " + tempWeight);
                        }
                    }
                    if (world.game.StoryCharacter.value.Equals(name))
                    {
                        tempWeight *= spawnModifiers.multiplier;
                        if (debug)
                        {
                            Plugin.ModLogger.LogDebug("Spawn weight for " + name + " in " + world.game.StoryCharacter + "'s campaign is " + tempWeight);
                        }
                    }
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