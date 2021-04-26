using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Shop : MonoBehaviour
    {
        public GameObject interactObject;
        public GameObject shopWindow;

        public TMP_Text drillCostText;
        public TMP_Text energyCostText;
        public TMP_Text lampCostText;
        public TMP_Text ladderCostText;

        public TMP_Text drillTierText;
        public TMP_Text energyTierText;

        public Button drillButton;
        public Button energyButton;
        public Button lampButton;
        public Button ladderButton;

        public int maxTier = 5;

        public int drillCost;
        public float drillCostModifier;

        public int energyCost;
        public float energyCostModifier;

        public int lampCost;
        public int ladderCost;

        PlayerEnergy playerEnergy;
        PlayerResources playerResources;
        PlayerController playerController;
        SoundManager soundManager;
        bool isClose = false;

        private void Start()
        {
            playerEnergy = FindObjectOfType<PlayerEnergy>();
            playerResources = FindObjectOfType<PlayerResources>();
            playerController = FindObjectOfType<PlayerController>();
            soundManager = FindObjectOfType<SoundManager>();
        }

        private void Update()
        {
            if(isClose && !shopWindow.activeSelf && Input.GetKey(KeyCode.E))
            {
                OpenShop();
            }

            if(shopWindow.activeSelf && Input.GetKey(KeyCode.Escape))
            {
                CloseShop();
            }
        }

        public void SellOres()
        {
            playerResources.SellAllOres();
            soundManager.PlaySfx("Coin", 1f);
            UpdateAllUI();
        }

        private void OpenShop()
        {
            UpdateAllUI();
            soundManager.PlaySfx("Shop", 1f);
            shopWindow.SetActive(true);
            playerController.CanMove = false;
        }

        void UpdateAllUI()
        {
            SetDrillUI();
            SetEnergyUI();
            SetLampUI();
            SetLadderUI();
        }

        public void BuyDrill()
        {
            int cost = Mathf.RoundToInt(drillCost * drillCostModifier * playerResources.drillTier);
            playerResources.drillTier++;
            playerResources.SpendMoney(cost);
            soundManager.PlaySfx("Coin", 1f);
            UpdateAllUI();
        }

        public void BuyEnergy()
        {
            int cost = Mathf.RoundToInt(energyCost * energyCostModifier * playerResources.energyTier);
            playerResources.energyTier++;
            playerResources.SpendMoney(cost);
            playerEnergy.IncreaseMaxEnergy(playerResources.energyTier);
            soundManager.PlaySfx("Coin", 1f);
            UpdateAllUI();
        }

        public void BuyLamp()
        {
            playerController.ActivateLamp();
            playerResources.lamp = true;
            playerResources.SpendMoney(lampCost);
            soundManager.PlaySfx("Coin", 1f);
            UpdateAllUI();
        }

        public void BuyLadder()
        {
            playerResources.AddLadder();
            playerResources.SpendMoney(ladderCost);
            soundManager.PlaySfx("Coin", 1f);
            UpdateAllUI();
        }

        void SetDrillUI()
        {
            drillTierText.text = "Tier" + playerResources.drillTier;
            if (playerResources.drillTier >= maxTier)
            {
                drillButton.interactable = false;
                drillCostText.text = "<color=orange>MAX</color>";
            }
            else
            {
                int cost = Mathf.RoundToInt(drillCost * drillCostModifier * playerResources.drillTier);
                if (playerResources.Money >= cost)
                {
                    drillButton.interactable = true;
                    drillCostText.text = "<color=green>cost " + cost + "</color>";
                }
                else
                {
                    drillButton.interactable = false;
                    drillCostText.text = "<color=red>cost " + cost + "</color>";
                }
            }
        }

        void SetEnergyUI()
        {
            energyTierText.text = "Tier" + playerResources.energyTier;

            if (playerResources.energyTier >= maxTier)
            {
                energyButton.interactable = false;
                energyCostText.text = "<color=orange>MAX</color>";
            }
            else
            {
                int cost = Mathf.RoundToInt(energyCost * energyCostModifier * playerResources.energyTier);
                if (playerResources.Money >= cost)
                {
                    energyButton.interactable = true;
                    energyCostText.text = "<color=green>cost " + cost + "</color>";
                }
                else
                {
                    energyButton.interactable = false;
                    energyCostText.text = "<color=red>cost " + cost + "</color>";
                }
            }
        }

        void SetLampUI()
        {
            if (playerResources.lamp)
            {
                lampButton.interactable = false;
                lampCostText.text = "<color=orange>MAX</color>";
            }
            else
            {
                if (playerResources.Money >= lampCost)
                {
                    lampButton.interactable = true;
                    lampCostText.text = "<color=green>cost " + lampCost + "</color>";
                }
                else
                {
                    lampButton.interactable = false;
                    lampCostText.text = "<color=red>cost " + lampCost + "</color>";
                }
            }
        }

        void SetLadderUI()
        {
            if (playerResources.Money >= ladderCost)
            {
                ladderButton.interactable = true;
                ladderCostText.text = "<color=green>cost " + ladderCost + "</color>";
            }
            else
            {
                ladderButton.interactable = false;
                ladderCostText.text = "<color=red>cost " + ladderCost + "</color>";
            }
        }

        public void CloseShop()
        {
            shopWindow.SetActive(false);
            playerController.CanMove = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            isClose = true;
            interactObject.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            isClose = false;
            interactObject.SetActive(false);
        }
    }
}

