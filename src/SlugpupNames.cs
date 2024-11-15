using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PupBase
{
    public class SlugpupNames
    {
        public static SlugcatStats.Name SlugpupAdult;

        public static void RegisterValues()
        {
            SlugpupAdult = new SlugcatStats.Name("SlugpupAdult", register: true);
        }

        public static void UnregisterValues()
        {
            SlugpupAdult?.Unregister();
            SlugpupAdult = null;
        }
    }
}
