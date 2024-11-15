using System.Drawing.Drawing2D;
using System.Dynamic;
using UnityEngine.Assertions.Must;

namespace PupBase
{
    public static class PupManager
    {
        private static List<PupType> pupTypeList = [new AdultType(Plugin.MOD_NAME, MoreSlugcatsEnums.SlugcatStatsName.Slugpup, SlugpupNames.SlugpupAdult) { regionModifiers = [new("SU", 1.2f)] }];

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
        /// Register a new AdultType. Will return the AdultType given. It is recommended that you assign this while your mod is initializing.
        /// </summary>
        /// <param name="adultType">The PupType that is to registered with PupBase.</param>
        /// <returns>Returns the newly registered PupType.</returns>
        public static AdultType Register(AdultType adultType)
        {
            pupTypeList.Add(adultType);
            Plugin.ModLogger.LogInfo("Registered: " + adultType.name);
            return adultType;
        }

        /// <summary>
        /// Grabs the current list of PupTypes registered to PupManager, and returns them in the form of a list.
        /// </summary>
        /// <returns>Returns a list of PupTypes.</returns>
        public static List<PupType> GetPupTypeList() => pupTypeList;

        /// <summary>
        /// Grabs the current list of PupTypes registered to PupManager, and returns them in the form of a list of its names.
        /// </summary>
        /// <returns>Returns a list of PupType names.</returns>
        public static List<SlugcatStats.Name> GetPupTypeListName(bool includeAdultName = false)
        {
            List<SlugcatStats.Name> tempList = [];

            foreach (PupType type in pupTypeList)
            {
                tempList.Add(type.name);
                if (includeAdultName && type.adultType != null)
                {
                    tempList.Add(type.adultType.adultName);
                }
            }
            return tempList;
        }

        /// <summary>
        /// Grabs the current list of PupTypes registered to PupManager, and returns them in the form of a list of strings.
        /// </summary>
        /// <returns>Returns a list of PupType names in string form.</returns>
        public static List<string> GetPupTypeListString(bool includeAdultName = false)
        {
            List<string> tempList = [];

            foreach (PupType type in pupTypeList)
            {
                tempList.Add(type.name.value);
                if (includeAdultName && type.adultType != null)
                {
                    tempList.Add(type.adultType.adultName.value);
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
            if (pupTypeList != null)
            {
                foreach (PupType type in pupTypeList)
                {
                    if (type.name == name)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        public static AdultType GetAdultType(SlugcatStats.Name name)
        {
            if (pupTypeList != null)
            {
                foreach (AdultType type in pupTypeList)
                {
                    if (type.adultType != null && type.adultType.adultName == name)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns true if the puptypes' name matches the given name.
        /// </summary>
        /// <param name="name">The name to compare against.</param>
        /// <returns>Returns true if name matches with a puptypes name.</returns>
        public static bool TryGetPupType(SlugcatStats.Name name)
        {
            if (pupTypeList != null)
            {
                foreach (PupType type in pupTypeList)
                {
                    if (type.name == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool TryGetAdultType(SlugcatStats.Name name)
        {
            if (pupTypeList != null)
            {
                foreach (AdultType type in pupTypeList)
                {
                    if (type.adultType != null && type.adultType.adultName == name)
                    {
                        return true;
                    }
                }
            }
            return false;
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
            if (pupTypeList != null)
            {
                foreach (PupType type in pupTypeList)
                {
                    if (type.name == name)
                    {
                        pupType = type;
                        break;
                    }
                }
            }
            return pupType != null;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns true if the puptypes' name matches the given name. Outputs the PupType found.
        /// </summary>
        /// <param name="name">The name to compare against.</param>
        /// <param name="pupType">The output if a match is found.</param>
        /// <returns>Returns true if name matches with a puptypes name, and outputs the puptype it was found in.</returns>
        public static bool TryGetAdultType(SlugcatStats.Name name, out AdultType pupType)
        {
            pupType = null;
            if (pupTypeList != null)
            {
                foreach (AdultType type in pupTypeList)
                {
                    if (type.adultType != null && type.adultType.adultName == name)
                    {
                        pupType = type;
                        break;
                    }
                }
            }
            return pupType != null;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns the PupType Found if the puptypes' name is mentioned in the given string.
        /// </summary>
        /// <param name="str">The string to compare against.</param>
        /// <returns>Returns the PupType found if str conmtains a puptypes name.</returns>
        public static PupType GetPupTypeFromString(string str)
        {
            PupType pupType = null;
            if (pupTypeList != null)
            {
                foreach (PupType type in pupTypeList)
                {
                    if ((type.adultType != null && str.Equals(type.adultType.adultName.value, StringComparison.OrdinalIgnoreCase)) || str.Equals(type.name.value, StringComparison.OrdinalIgnoreCase))
                    {
                        pupType = type;
                        break;
                    }
                }
            }
            return pupType;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns true if the puptypes' name is mentioned in the given string.
        /// </summary>
        /// <param name="str">The string to compare against.</param>
        /// <returns>Returns true if str conmtains a puptypes name.</returns>
        public static bool TryGetPupTypeFromString(string str)
        {
            if (pupTypeList != null)
            {
                foreach (PupType type in pupTypeList)
                {
                    if ((type.adultType != null && str.Equals(type.adultType.adultName.value, StringComparison.OrdinalIgnoreCase)) || str.Equals(type.name.value, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Searches through all available PupTypes and returns true if the puptypes' name is mentioned in the given string. Outputs the PupType found.
        /// </summary>
        /// <param name="str">The string to compare against.</param>
        /// <param name="pupType">The output if a match is found.</param>
        /// <returns>Returns true if str conmtains a puptypes name, and outputs the puptype it was found in.</returns>
        public static bool TryGetPupTypeFromString(string str, out PupType pupType)
        {
            pupType = null;
            if (pupTypeList != null)
            {
                foreach (PupType type in pupTypeList)
                {
                    if ((type.adultType != null && str.Equals(type.adultType.adultName.value, StringComparison.OrdinalIgnoreCase)) || str.Equals(type.name.value, StringComparison.OrdinalIgnoreCase))
                    {
                        pupType = type;
                        break;
                    }
                }
            }
            return pupType != null;
        }

        /// <summary>
        /// Generates a new PupType. Outputs the assigned PupType. If no pup is generated, it'll output a regular Slugpup instead.
        /// </summary>
        /// <param name="abstractCreature">Used to gather all necessary data.</param>
        /// <param name="maturity">1 = only children spawn, 2 = only adults spawn</param>
        /// <param name="info">Outputs info in the log</param>
        /// <returns>Outputs the newly generated PupType.</returns>
        public static PupType GenerateType(AbstractCreature abstractCreature, bool adultTypeOnly = false, bool info = true)
        {
            // Calculate total weight.
            float totalWeight = 0;
            List<object[]> listedWeights = new List<object[]>();
            foreach (PupType type in pupTypeList)
            {
                if (!adultTypeOnly || (adultTypeOnly && type.adultType != null))
                {
                    listedWeights.Add([type, type.CalculateWeight(abstractCreature.world, info && ModOptions.enableDebug.Value)]);
                    totalWeight += (float)listedWeights.Last()[1];
                }
            }

            // Generate random number based on ID
            Random.State state = Random.state;
            Random.InitState(abstractCreature.ID.RandomSeed);

            float seed = Random.value * totalWeight;

            Random.state = state;

            // Assign PupType based on weighted probability
            float sum = 0;
            foreach (object[] obj in listedWeights)
            {
                sum += (float)obj[1];

                if (sum >= seed)
                {
                    if (info) Plugin.ModLogger.LogInfo("Generated " + abstractCreature.ID.ToString() + " Type " + ((PupType)obj[0]).name);
                    return (PupType)obj[0];
                }
            }
            if (info) Plugin.ModLogger.LogInfo("Failed to generate a PupType. Defaulting to Slugpup.");
            return GetPupType(MoreSlugcatsEnums.SlugcatStatsName.Slugpup);
        }

        public static bool GenerateAdult(AbstractCreature abstractCreature, AdultType type)
        {
            // Generate random number based on ID
            Random.State state = Random.state;
            Random.InitState(abstractCreature.ID.RandomSeed);

            float seed = Random.value;

            Random.state = state;

            return (type.adultChance / 100) >= seed;
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
                if (state is PlayerNPCState npcState && SlugpupCWTs.GetPupState(npcState).Variant != null)
                {
                    return true;
                }
                return false;
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
