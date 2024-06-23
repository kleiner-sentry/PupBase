namespace PupBase
{
    public class PupType
    {
        public SlugcatStats.Name name;
        public int spawnWeight;

        public int foodToHibernate;
        public int maxFood;

        /// <summary>
        /// Creates a new PupType.
        /// </summary>
        /// <param name="name">The name to identify the PupType by.</param>
        /// <param name="spawnWeight">The weighted probability of this Slugpup spawning. Default is 100.</param>
        /// <param name="foodToHibernate">(OPTIONAL) The amount of food needed to hibernate.</param>
        /// <param name="maxFood">(OPTIONAL) The maximum food this pup can hold.</param>
        public PupType(SlugcatStats.Name name, int spawnWeight = 100, int foodToHibernate = 2, int maxFood = 3)
        {
            this.name = name;
            this.spawnWeight = spawnWeight;

            this.foodToHibernate = foodToHibernate;
            this.maxFood = maxFood;
        }
    }
}
