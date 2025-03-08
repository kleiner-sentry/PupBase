using System.Drawing.Drawing2D;
using System.Dynamic;
using UnityEngine.Assertions.Must;

namespace PupBase
{
    public static class PupManager
    {
        private static List<PupType> pupTypeList = [new(Plugin.MOD_NAME, MoreSlugcatsEnums.SlugcatStatsName.Slugpup) { regionModifiers = [new("SU") { spawnMultiplier = 1.2f }], adultModule = new(SlugpupNames.SlugpupAdult) }];

        public static List<int> PupIDBlacklist = [1000, 1001, 2220, 3118, 4118, 765];

        /// <summary>
        /// Register a new PupType. Will return the PupType given. It is recommended that you assign this while your mod is initializing.
        /// </summary>
        /// <param name="pupType">The PupType that is to registered with PupBase.</param>
        /// <returns>Returns the newly registered PupType.</returns>
        public static PupType Register(PupType pupType)
        {
            pupTypeList.Add(pupType);
            Plugin.ModLogger.LogInfo("Registered: " + pupType.name);
            return pupType;
        }

        /// <summary>
        /// Grabs the current list of PupTypes registered to PupManager, and returns them in the form of a list.
        /// </summary>
        /// <returns>Returns a list of PupTypes.</returns>
        public static List<PupType> GetPupTypeList() => pupTypeList;

        /// <summary>
        /// Grabs the current list of PupTypes registered to PupManager, and returns them in the form of a list of its names.
        /// </summary>
        /// <param name="includeAdultName">Will this method also include the adult names?</param>
        /// <returns>Returns a list of PupType names.</returns>
        public static List<SlugcatStats.Name> GetPupTypeListName(bool includeAdultName = false)
        {
            List<SlugcatStats.Name> tempList = [];

            foreach (PupType type in GetPupTypeList())
            {
                tempList.Add(type.name);
                if (includeAdultName && type.HasAdultModule)
                {
                    tempList.Add(type.adultModule.name);
                }
            }
            return tempList;
        }

        /// <summary>
        /// Grabs the current list of PupTypes registered to PupManager, and returns them in the form of a list of strings.
        /// </summary>
        /// <param name="includeAdultName">Will this method also include the adult names?</param>
        /// <returns>Returns a list of PupType names in string form.</returns>
        public static List<string> GetPupTypeListString(bool includeAdultName = false)
        {
            List<string> tempList = [];

            foreach (PupType type in GetPupTypeList())
            {
                tempList.Add(type.name.value);
                if (includeAdultName && type.HasAdultModule)
                {
                    tempList.Add(type.adultModule.name.value);
                }
            }
            return tempList;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns a Puptype if a match is found.
        /// </summary>
        /// <param name="name">The name to compare against.</param>
        /// <returns>Returns the PupType that it found.</returns>
        public static PupType GetPupType(SlugcatStats.Name name)
        {
            if (GetPupTypeList() != null)
            {
                foreach (PupType type in GetPupTypeList())
                {
                    if (type.name == name || (type.HasAdultModule && type.adultModule.name == name))
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns true if the puptypes' name matches the given name. Outputs the PupType found.
        /// </summary>
        /// <param name="name">The name to compare against.</param>
        /// <param name="pupType">The output if a match is found.</param>
        /// <returns>Returns true if name matches with a puptypes name, and outputs the puptype it was found in.</returns>
        public static bool TryGetPupType(SlugcatStats.Name name, out PupType pupType)
        {
            pupType = null;
            if (GetPupTypeList() != null)
            {
                foreach (PupType type in GetPupTypeList())
                {
                    if (type.name == name || (type.HasAdultModule && type.adultModule.name == name))
                    {
                        pupType = type;
                        break;
                    }
                }
            }
            return pupType != null;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns the PupType Found if the puptypes' name is in the given string.
        /// </summary>
        /// <param name="str">The string to compare against.</param>
        /// <returns>Returns the PupType found if str equals a puptypes name.</returns>
        public static PupType GetPupTypeFromString(string str)
        {
            PupType pupType = null;
            if (GetPupTypeList() != null)
            {
                foreach (PupType type in GetPupTypeList())
                {
                    if (str.Equals(type.name.value, StringComparison.OrdinalIgnoreCase) || (type.HasAdultModule && str.Equals(type.adultModule.name.value, StringComparison.OrdinalIgnoreCase)))
                    {
                        pupType = type;
                        break;
                    }
                }
            }
            return pupType;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns true if the puptypes' name is in the given string. Outputs the PupType found.
        /// </summary>
        /// <param name="str">The string to compare against.</param>
        /// <param name="pupType">The output if a match is found.</param>
        /// <returns>Returns true if str equals a puptypes name, and outputs the puptype it was found in.</returns>
        public static bool TryGetPupTypeFromString(string str, out PupType pupType)
        {
            pupType = null;
            if (pupTypeList != null)
            {
                foreach (PupType type in GetPupTypeList())
                {
                    if (str.Equals(type.name.value, StringComparison.OrdinalIgnoreCase) || (type.HasAdultModule && str.Equals(type.adultModule.name.value, StringComparison.OrdinalIgnoreCase)))
                    {
                        pupType = type;
                        break;
                    }
                }
            }
            return pupType != null;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns true if the name given matches with a PupTypes adult name.
        /// </summary>
        /// <param name="name">The name to compare against.</param>\
        /// <returns>Returns true if the name given matches with a PupTypes adult name.</returns>
        public static bool isAdultName(SlugcatStats.Name name)
        {
            if (GetPupTypeList() != null)
            {
                foreach (PupType type in GetPupTypeList())
                {
                    if (type.HasAdultModule && type.adultModule.name == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Generates a new PupType. Outputs the assigned PupType. If no pup is generated, it'll output a regular Slugpup instead.
        /// </summary>
        /// <param name="abstractCreature">Used to gather all necessary data.</param>
        /// <param name="adultTypeOnly">Will only generate pups that have the Adult module.</param>
        /// <param name="info">Outputs info to the log</param>
        /// <returns>Outputs the newly generated PupType, and if applicable, if its an adult or not.</returns>
        public static (PupType, bool) GenerateType(AbstractCreature abstractCreature, bool adultTypeOnly = false, bool info = true)
        {
            // Calculate total weight.
            float totalWeight = 0;
            List<object[]> listedWeights = new List<object[]>();
            foreach (PupType type in pupTypeList)
            {
                if (!adultTypeOnly || (adultTypeOnly && type.HasAdultModule))
                {
                    (float w, float a) = type.CalculateWeight(abstractCreature.world, info && ModOptions.enableDebug.Value);
                    listedWeights.Add([type, w, a]);
                    totalWeight += (float)listedWeights.Last()[1];
                }
            }

            // Generate random number based on ID
            Random.State state = Random.state;
            Random.InitState(abstractCreature.ID.RandomSeed);

            float seedWeight = Random.value * totalWeight;
            float seed = Random.value;

            Random.state = state;

            // Assign PupType based on weighted probability
            float sum = 0;
            foreach (object[] obj in listedWeights)
            {
                sum += (float)obj[1];

                if (sum >= seedWeight)
                {
                    bool roll = ((float)obj[2] / 100f) >= seed;
                    if (info) Plugin.ModLogger.LogInfo("Generated " + abstractCreature.ID.ToString() + " Type " + ((PupType)obj[0]).name + " As " + (roll ? "an adult." : "a child."));
                    return ((PupType)obj[0], roll);
                }
            }
            if (info) Plugin.ModLogger.LogInfo("Failed to generate a PupType. Defaulting to Slugpup.");
            return (GetPupType(MoreSlugcatsEnums.SlugcatStatsName.Slugpup), false);
        }

        /// <summary>
        /// determines if the pup ID given will be an adult or not.
        /// </summary>
        /// <param name="abstractCreature">Used to gather all necessary data.</param>
        /// <param name="type">The puptypes' module used to determine the likelyhood of it being an adult.</param>
        /// <param name="info">Outputs info to the log</param>
        /// <returns>Outputs true or false if it successfully rolled for an adult.</returns>
        public static bool GenerateAdult(AbstractCreature abstractCreature, PupType type, bool info = true)
        {
            if (!type.HasAdultModule)
            {
                return false;
            }

            // Generate random number based on ID
            Random.State state = Random.state;
            Random.InitState(abstractCreature.ID.RandomSeed);

            float seed = Random.value;

            Random.state = state;

            (_,float adultChance) = type.CalculateWeight(abstractCreature.world, info && ModOptions.enableDebug.Value);

            bool roll = (adultChance / 100f) >= seed;
            if (info) Plugin.ModLogger.LogInfo(abstractCreature.ID.ToString() + (roll ? " generated as an adult." : " generated as a child.") );
            return roll;
        }

        /// <summary>
        /// Returns true if Pups+ is enabled, and the mod is currently using this pup.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>True if Variant isn't null.</returns>
        public static bool IsPupInUseBySlugpupStuff(CreatureState state)
        {
            try
            {
                return !SlugpupCWTs.TryGetPupState(state as PlayerNPCState, out _);
            }
            catch (Exception ex)
            {
                Plugin.ModLogger.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Forces a Pups+ variant to be what you've given. Not recommended.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="name"></param>
        public static void OverrideSlugpupStuffVariant(CreatureState state, SlugcatStats.Name name)
        {
            try
            {
                if (state is PlayerNPCState npcState && SlugpupCWTs.TryGetPupState(npcState, out var pupState))
                {
                    pupState.Variant = name;
                }
            }
            catch (Exception ex)
            {
                Plugin.ModLogger.LogError(ex);
            }
        }

        internal static bool IsPearlpup(AbstractCreature abstractCreature) => Pearlcat.Hooks.IsPearlpup(abstractCreature);
    }
}
