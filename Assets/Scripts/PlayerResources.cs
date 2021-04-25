using UnityEngine;
using TMPro;
using System;

namespace Assets.Scripts
{
    public class PlayerResources : MonoBehaviour
    {
        public TMP_Text moneyValue;
        public TMP_Text ladderValue;
        public TMP_Text silverValue;
        public TMP_Text saphireValue;
        public TMP_Text emeraldValue;
        public TMP_Text amethystValue;
        public TMP_Text rubyValue;
        public TMP_Text diamondValue;

        public OreData silverData;
        public OreData saphireData;
        public OreData emeraldData;
        public OreData amethystData;
        public OreData rubyData;
        public OreData diamondData;

        public int startMoney = 0;
        public int drillTier = 1;
        public int energyTier = 1;
        public bool lamp = false;

        [Header("Notifications")]
        public Animator moneyNotifyAnim;
        public TMP_Text moneyNotifyText;

        public Animator ladderNotifyAnim;
        public TMP_Text ladderNotifyText;

        public Animator silverNotifyAnim;
        public TMP_Text silverNotifyText;

        public Animator saphireNotifyAnim;
        public TMP_Text saphireNotifyText;

        public Animator emeraldNotifyAnim;
        public TMP_Text emeraldNotifyText;

        public Animator amethystNotifyAnim;
        public TMP_Text amethystNotifyText;

        public Animator rubyNotifyAnim;
        public TMP_Text rubyNotifyText;

        public Animator diamondNotifyAnim;
        public TMP_Text diamondNotifyText;

        private int money = 0;
        private int ladders = 5;
        private int silvers = 0;
        private int saphires = 0;
        private int emeralds = 0;
        private int amethysts = 0;
        private int rubys = 0;
        private int diamonds = 0;

        public int Money { get => money; private set => money = value; }
        public int Silvers { get => silvers; private set => silvers = value; }
        public int Saphires { get => saphires; private set => saphires = value; }
        public int Emeralds { get => emeralds; private set => emeralds = value; }
        public int Amethysts { get => amethysts; private set => amethysts = value; }
        public int Rubys { get => rubys; private set => rubys = value; }
        public int Diamonds { get => diamonds; private set => diamonds = value; }
        public int Ladders { get => ladders; set => ladders = value; }

    

        private void Start()
        {
            money = startMoney;
            UpdateAllResourceUIs();
        }

        private void Update()
        {
            if (!Application.isEditor)
                return;

            if(Input.GetKeyDown(KeyCode.F1))
            {
                AddOre(Ore.Silver);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                AddOre(Ore.Saphire);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                AddOre(Ore.Emerald);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                AddOre(Ore.Amethyst);
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                AddOre(Ore.Ruby);
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                AddOre(Ore.Diamond);
            }
        }

        public bool SpendMoney(int amount)
        {
            if(money >= amount)
            {
                money -= amount;

                moneyNotifyText.text = "-" + amount;
                moneyNotifyAnim.Play("ResourceNotification");

                UpdateMoneyUI();
                return true;
            }
            return false;
        }

        public void SellAllOres()
        {
            int value = 0;

            value += silvers * silverData.value;
            value += saphires * saphireData.value;
            value += emeralds * emeraldData.value;
            value += amethysts * amethystData.value;
            value += rubys * rubyData.value;
            value += diamonds * diamondData.value;

            money += value;
            if(value > 0)
            {
                moneyNotifyText.text = "+" + value;
                moneyNotifyAnim.Play("ResourceNotification", -1, 0);
            }
            
            ClearAllOres();
        }

        public bool UseLadder()
        {
            if(ladders > 0)
            {
                ladders--;
                ladderNotifyText.text = "-1";
                ladderNotifyAnim.Play("ResourceNotification", -1, 0);
                UpdateLadderUI();
                return true;
            }
            return false;
        }

        public void AddLadder()
        {
            ladders++;
            ladderNotifyText.text = "+1";
            ladderNotifyAnim.Play("ResourceNotification", -1, 0);
            UpdateLadderUI();
        }

        public void AddOre(Ore ore)
        {
            switch (ore)
            {
                case Ore.Silver:    silvers++;  UpdateSilverUI();   break;
                case Ore.Saphire:   saphires++; UpdateSaphireUI();  break;
                case Ore.Emerald:   Emeralds++; UpdateEmeraldUI();  break;
                case Ore.Amethyst:  amethysts++;UpdateAmethystUI(); break;
                case Ore.Ruby:      rubys++;    UpdateRubyUI();     break;
                case Ore.Diamond:   diamonds++; UpdateDiamondUI();  break;
            }
        }

        void UpdateAllResourceUIs()
        {
            UpdateMoneyUI();
            UpdateLadderUI();
            UpdateSilverUI();
            UpdateSaphireUI();
            UpdateEmeraldUI();
            UpdateAmethystUI();
            UpdateRubyUI();
            UpdateDiamondUI();
        }

        private void UpdateLadderUI()
        {
            ladderValue.text = ladders.ToString();
        }

        private void UpdateDiamondUI()
        {
            int previous = int.Parse(diamondValue.text);
            if (diamonds != previous)
            {
                int diff = (diamonds - previous);
                diamondNotifyText.text = (diff > 0 ? "+" : "") + diff;
                diamondNotifyAnim.Play("ResourceNotification", -1, 0);
            }

            diamondValue.text = diamonds.ToString();
        }

        private void UpdateRubyUI()
        {
            int previous = int.Parse(rubyValue.text);
            if (rubys != previous)
            {
                int diff = (rubys - previous);
                rubyNotifyText.text = (diff > 0 ? "+" : "") + diff;
                rubyNotifyAnim.Play("ResourceNotification", -1, 0);
            }

            rubyValue.text = rubys.ToString();
        }

        private void UpdateAmethystUI()
        {
            int previous = int.Parse(amethystValue.text);
            if (amethysts != previous)
            {
                int diff = (amethysts - previous);
                amethystNotifyText.text = (diff > 0 ? "+" : "") + diff;
                amethystNotifyAnim.Play("ResourceNotification", -1, 0);
            }

            amethystValue.text = amethysts.ToString();
        }

        private void UpdateEmeraldUI()
        {
            int previous = int.Parse(emeraldValue.text);
            if (emeralds != previous)
            {
                int diff = (emeralds - previous);
                emeraldNotifyText.text = (diff > 0 ? "+" : "") + diff;
                emeraldNotifyAnim.Play("ResourceNotification", -1, 0);
            }

            emeraldValue.text = emeralds.ToString();
        }

        private void UpdateSaphireUI()
        {
            int previous = int.Parse(saphireValue.text);
            if (saphires != previous)
            {
                int diff = (saphires - previous);
                saphireNotifyText.text = (diff > 0 ? "+" : "") + diff;
                saphireNotifyAnim.Play("ResourceNotification", -1, 0);
            }

            saphireValue.text = saphires.ToString();
        }

        private void UpdateSilverUI()
        {
            int previous = int.Parse(silverValue.text);
            if (silvers != previous)
            {
                int diff = (silvers - previous);
                silverNotifyText.text = (diff>0?"+":"") + diff;
                silverNotifyAnim.Play("ResourceNotification", -1, 0);
            }

            silverValue.text = silvers.ToString();
        }

        private void UpdateMoneyUI()
        {
            moneyValue.text = money.ToString();
        }

        internal void ClearAllOres()
        {
            silvers = 0;
            saphires = 0;
            emeralds = 0;
            amethysts = 0;
            rubys = 0;
            diamonds = 0;
            UpdateAllResourceUIs();
        }
    }
}

