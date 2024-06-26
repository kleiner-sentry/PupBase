namespace PupBase
{
    public static class PupManager
    {
        private static List<PupType> pupTypeList = [new PupType(Plugin.MOD_NAME, MoreSlugcatsEnums.SlugcatStatsName.Slugpup, regionMultList: new List<PupType.RegionMult> { new PupType.RegionMult("SU", 1.2f) })];

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
        public static List<PupType> GetPupTypeList()
        {
            return pupTypeList;
        }

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
                    if (str.Contains(type.name.value))
                    {
                        pupType = type;
                        break;
                    }
                }
            }
            return pupType != null;
        }

        /// <summary>
        /// Generates a new PupType. Outputs the assigned PupType.
        /// </summary>
        /// <param name="player">Used to gather all necessary data.</param>
        /// <returns>Outputs the assigned PupType.</returns>
        public static PupType GenerateType(Player player)
        {
            // Calculate total weight.
            float totalWeight = 0;
            foreach (PupType type in pupTypeList)
            {
                totalWeight += type.CalculateWeight(player.abstractCreature.world);
            }

            // Generate random number based on ID
            Random.State state = Random.state;
            Random.InitState(player.abstractCreature.ID.RandomSeed);

            float probability = Random.value * totalWeight;

            Random.state = state;

            // Assign PupType based on weighted probability
            float sum = 0;
            foreach (PupType type in pupTypeList)
            {
                sum += type.CalculateWeight(player.abstractCreature.world, ModOptions.enableDebug.Value);

                if (sum >= probability)
                {
                    return type;
                }
            }
            return null;
        }

    }
}
