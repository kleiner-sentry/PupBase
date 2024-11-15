using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PupBase
{
    public class AdultType : PupType
    {
        public SlugcatStats.Name adultName;

        public int adultFoodToHibernate = 4;
        public int adultMaxFood = 7;

        public bool customAdultChance = true;
        internal Configurable<int> adultChanceConfig;
        public readonly float defaultAdultChance;
        public float adultChance
        {
            get { return (customAdultChance && adultChanceConfig != null) ? adultChanceConfig.Value : defaultAdultChance; }
        }

        public AdultType(string modName, SlugcatStats.Name name, SlugcatStats.Name adultName = null, int spawnWeight = 100, float adultChance = 0.1f)
            : base(modName, name, spawnWeight)
        {
            adultType = this;

            if (adultName == null)
            {
                this.adultName = new SlugcatStats.Name(name.value + "Adult");
            }
            else
            {
                this.adultName = adultName;
            }

            defaultAdultChance = Mathf.Clamp(adultChance, 0, 100);

            adultChanceConfig = ModOptions.Instance.config.Bind("AdultChance" + this.name, defaultSpawnWeight, new ConfigurableInfo("Set the percentage chance this pup will spawn as an adult.", new ConfigAcceptableRange<float>(0, 100)));
        }
    }
}
