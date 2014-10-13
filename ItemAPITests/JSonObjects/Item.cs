using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemAPITests.JSonObjects
{
    public class Item
    {
        public int id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public List<Spell> itemSpells { get; set; }
        public int sellPrice { get; set; }
        public int requiredSkill { get; set; }
        public int requiredSkillRank { get; set; }
        public int itemLevel { get; set; }
        public int requiredLevel { get; set; }
        public int itemBind { get; set; }

        public List<int> allowableClasses { get; set; }
        public List<BonusStat> bonusStats { get; set; }
    }
}
