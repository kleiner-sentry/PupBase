using System.Drawing.Drawing2D;
using UnityEngine.Assertions.Must;

namespace PupBase
{
    public static class PupManager
    {
        private static List<PupType> pupTypeList = [new PupType(Plugin.MOD_NAME, MoreSlugcatsEnums.SlugcatStatsName.Slugpup, spawnModifiersList: new List<PupType.SpawnModifiers> { new PupType.SpawnModifiers("SU", 1, 1.2f) })];

        internal static List<PupType.SpawnModifiers> spawnModifiersList = new List<PupType.SpawnModifiers>();

        public static List<int> PupIDBlacklist = [ 1000, 1001, 2220, 3118, 4118, 765];

        /// <summary>
        /// Register a new PupType. Will return the PupType given. It is recommended that you assign this while your mod is initializing.
        /// </summary>
        /// <param name="pupType">The PupType that is to registered with PupBase.</param>
        /// <returns>Returns the newly registered PupType.</returns>
        public static PupType Register(PupType pupType)
        {
            pupTypeList.Add(pupType);
            if (pupType.spawnModifiersList != null)
            {
                spawnModifiersList.AddRange(pupType.spawnModifiersList);
            }
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
        /// <returns>Returns a list of PupType names.</returns>
        public static List<SlugcatStats.Name> GetPupTypeListName()
        {
            List<SlugcatStats.Name> tempList = [];

            foreach (PupType pupType in pupTypeList)
            {
                tempList.Add(pupType.name);
            }
            return tempList;
        }
        
        /// <summary>
        /// Grabs the current list of PupTypes registered to PupManager, and returns them in the form of a list of strings.
        /// </summary>
        /// <returns>Returns a list of PupType names in string form.</returns>
        public static List<string> GetPupTypeListString()
        {
            List<string> tempList = [];

            foreach (PupType pupType in pupTypeList)
            {
                tempList.Add(pupType.name.value);
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
                foreach (PupType pupType in pupTypeList)
                {
                    if (pupType.name == name)
                    {
                        return pupType;
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
                    if (str.Contains(type.name.value) || str.Equals(type.name.value, StringComparison.OrdinalIgnoreCase))
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
                    if (str.Contains(type.name.value) || str.Equals(type.name.value, StringComparison.OrdinalIgnoreCase))
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
                    if (str.Contains(type.name.value) || str.Equals(type.name.value, StringComparison.OrdinalIgnoreCase))
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
        /// <returns>Outputs the assigned PupType.</returns>
        public static PupType GenerateType(AbstractCreature abstractCreature)
        {
            if ((Plugin.Pearlcat && IsPearlpup(abstractCreature)) || PupIDBlacklist.Contains(abstractCreature.ID.RandomSeed))
            {
                Plugin.ModLogger.LogInfo("This particular pup is excluded from generating a PupType. Defaulting to Slugpup.");
                return GetPupType(MoreSlugcatsEnums.SlugcatStatsName.Slugpup);
            }

            // Calculate total weight.
            float totalWeight = 0;
            List<float> listedWeights = new List<float>();
            foreach (PupType type in pupTypeList)
            {
                listedWeights.Add(type.CalculateWeight(abstractCreature.world, ModOptions.enableDebug.Value));
                totalWeight += listedWeights.Last();
            }

            // Generate random number based on ID
            Random.State state = Random.state;
            Random.InitState(abstractCreature.ID.RandomSeed);

            float probability = Random.value * totalWeight;

            Random.state = state;

            // Assign PupType based on weighted probability
            float sum = 0;
            int i = 0;
            foreach (PupType type in pupTypeList)
            {
                sum += listedWeights.ElementAt(i);

                if (sum >= probability)
                {
                    return type;
                }
                i++;
            }
            Plugin.ModLogger.LogInfo("Failed to generate a PupType. Defaulting to Slugpup.");
            return GetPupType(MoreSlugcatsEnums.SlugcatStatsName.Slugpup);
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
                if (Plugin.SlugpupStuff && state is PlayerNPCState npcState && npcState.GetPupState().Variant != null)
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
        public static void OverrideSlugpupStuffVariant(CreatureState state, SlugcatStats.Name? name)
        {
            try
            {
                if (Plugin.SlugpupStuff && state is PlayerNPCState npcState && npcState.GetPupState() != null)
                {
                    npcState.GetPupState().Variant = name;
                }
            }
            catch (Exception ex)
            {
                Plugin.ModLogger.LogError(ex);
            }
        }

        private static bool IsPearlpup(AbstractCreature abstractCreature) => Pearlcat.Hooks.IsPearlpup(abstractCreature);
    }
}
