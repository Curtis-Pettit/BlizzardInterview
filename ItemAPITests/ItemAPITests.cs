using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using RestSharp.Deserializers;
using ItemAPITests.JSonObjects;

namespace ItemAPITests
{
    [TestFixture]
    public class ItemAPITests
    {
        private string baseUri = "http://us.battle.net/api/wow/";
        private string itemResourceFormat = "item/{0}";

        [TestCase]
        public void GetRecipeItem()
        {
            //There are more properties than this, but these are the ones that can be confirmed in WoWHead.
            int requestedItemId = 34109;
            string description = "Teaches you the fine art of fish finding.";
            string name = "Weather-Beaten Journal";
            //spells aren't the purpose of this test so ids is enough.
            int[] spells = new int[] { 483/*Leaning */, 43308 /*Find Fish */ };
            int sellPrice = 625;
            int requiredSkillRank = 100;
            int requiredSkill = 356;
            int itemLevel = 40;
            int requiredLevel = 0;

            var response = GetItem(requestedItemId);

            Assert.AreEqual(description, response.Data.description, "Items decription does not match expected.");
            Assert.AreEqual(name, response.Data.name, "Items Name does not match expected");
            Assert.AreEqual(sellPrice, response.Data.sellPrice, "Items sell price does not match expected.");
            Assert.AreEqual(requiredSkillRank, response.Data.requiredSkillRank, "Items required skill rank does not match expected.");
            Assert.AreEqual(requiredSkill, response.Data.requiredSkill, "Items skill requirement does not match expected.");
            Assert.AreEqual(itemLevel, response.Data.itemLevel, "Items item level does not match expected.");
            Assert.AreEqual(requiredLevel, response.Data.requiredLevel, "Items level requirement does not match expected.");

            Assert.AreEqual(spells[0], response.Data.itemSpells[0].spellId, "Learn Spell had the wrong id");
            Assert.AreEqual(spells[1], response.Data.itemSpells[1].spellId, "Find Fish Spell had the wrong id");
        }

        [TestCase]
        public void GetNoLongerAvailableArmorItem()
        {
            int requestedItemId = 18470;
            string description = "Blessed by the Shen'dralar Ancients.";
            string name = "Royal Seal of Eldre'Thalas";
            int[] allowableClasses = new int[] { 11 }; //Druid
            int itemBind = 1; //Bind on pickup
            List<BonusStat> bonusStats = new List<BonusStat>();
            bonusStats.Add(new BonusStat { stat = 51, amount = 10 }); //Fire Resistance
            bonusStats.Add(new BonusStat { stat = 5, amount = 20 }); //Intellect
            int itemLevel = 62;



            var response = GetItem(requestedItemId);

            Assert.AreEqual(description, response.Data.description, "Items decription does not match expected.");
            Assert.AreEqual(name, response.Data.name, "Items Name does not match expected");
            Assert.AreEqual(allowableClasses.Length, response.Data.allowableClasses.Count, "Unexpected class allowed");
            Assert.AreEqual(allowableClasses[0], response.Data.allowableClasses[0], "Unexpected class allowed");
            Assert.AreEqual(itemBind, response.Data.itemBind, "Item does not bind as expected");
            Assert.AreEqual(itemLevel, response.Data.itemLevel, "Items item level does not match expected.");

            Assert.AreEqual(bonusStats.Count, response.Data.bonusStats.Count, "Unexpected number of stat boosts");
            Assert.AreEqual(bonusStats[0].stat, response.Data.bonusStats[0].stat, "Fire Resistance stat was not boosted correctly");
            Assert.AreEqual(bonusStats[0].amount, response.Data.bonusStats[0].amount, "Fire Resistance stat was not boosted correctly");
            Assert.AreEqual(bonusStats[1].stat, response.Data.bonusStats[1].stat, "Int stat was not boosted correctly");
            Assert.AreEqual(bonusStats[1].amount, response.Data.bonusStats[1].amount, "Int stat was not boosted correctly");
        }

        [TestCase]
        public void GetItemZero()
        {
            string status = "nok";
            string reason = "unable to get item information.";
            IRestResponse<Error> response = GetItemError(0);

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NotFound, "Unexpected error code");
            Assert.AreEqual(status, response.Data.status, "Unexpected status returned");
            Assert.AreEqual(reason, response.Data.reason, "Unexpected error reason");
        }

        [TestCase]
        public void GetNegativeItem()
        {
            string status = "nok";
            string reason = "unable to get item information.";
            IRestResponse<Error> response = GetItemError(0);

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NotFound, "Unexpected error code");
            Assert.AreEqual(status, response.Data.status, "Unexpected status returned");
            Assert.AreEqual(reason, response.Data.reason, "Unexpected error reason");
        }

        [TestCase]
        public void GetMaxIntItem()
        {
            string status = "nok";
            string reason = "unable to get item information.";
            IRestResponse<Error> response = GetItemError(Int32.MaxValue);

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NotFound, "Unexpected error code");
            Assert.AreEqual(status, response.Data.status, "Unexpected status returned");
            Assert.AreEqual(reason, response.Data.reason, "Unexpected error reason");
        }

        [TestCase]
        public void GetMaxIntPlusOneItem()
        {
            string status = "nok";
            string reason = "Item id should be integer.";
            long tooLargeItemId = ((long)Int32.MaxValue) + 1;
            IRestResponse<Error> response = GetItemError(tooLargeItemId);

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.InternalServerError, "Unexpected error code");
            Assert.AreEqual(status, response.Data.status, "Unexpected status returned");
            Assert.AreEqual(reason, response.Data.reason, "Unexpected error reason");
        }

        [TestCase]
        public void GetUnreleasedItem()
        {
            string status = "nok";
            string reason = "unable to get item information.";
            IRestResponse<Error> response = GetItemError(118306); //Spellbound Runic Band of the All-Seeing Eye from Warlords of Dreanor (according to WoWHead)

            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NotFound, "Unexpected error code");
            Assert.AreEqual(status, response.Data.status, "Unexpected status returned");
            Assert.AreEqual(reason, response.Data.reason, "Unexpected error reason");
        }

        private IRestResponse<Error> GetItemError(long requestedItemId)
        {
            RestClient client = new RestClient();
            client.BaseUrl = baseUri;

            RestRequest request = new RestRequest();
            request.Resource = string.Format(itemResourceFormat, requestedItemId);

            var response = client.Execute<Error>(request);
            return response;
        }

        private IRestResponse<Item> GetItem(int requestedItemId)
        {
            RestClient client = new RestClient();
            client.BaseUrl = baseUri;

            RestRequest request = new RestRequest();
            request.Resource = string.Format(itemResourceFormat, requestedItemId);

            var response = client.Execute<Item>(request);
            Assert.AreEqual(requestedItemId, response.Data.id, "Api returned an item with an id different from the one requested.");
            return response;
        }
    }
}