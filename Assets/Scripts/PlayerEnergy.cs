using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerEnergy : MonoBehaviour
    {
        public int maxEnergy = 20;
        public Image fill;

        private PlayerResources playerResources;
        private PlayerController playerController;
        private SceneFade sceneFade;
        private float currentEnergy;

        private void Start()
        {
            playerController = FindObjectOfType<PlayerController>();
            RefillEnergy();
            UpdateFillbar();
        }

        public void DecreaseEnergy(float value)
        {
            currentEnergy -= value;

            if (currentEnergy <= 0)
            {
                StartCoroutine(playerController.Faint());
            }
            UpdateFillbar();
        }

        public void RefillEnergy()
        {
            currentEnergy = maxEnergy;
        }

        void UpdateFillbar()
        {
            fill.fillAmount = currentEnergy / (float)maxEnergy;
        }

        public void IncreaseMaxEnergy(int tier)
        {
            int amount = 0;
            switch (tier)
            {
                case 1: amount = 10; break;
                case 2: amount = 11; break;
                case 3: amount = 15; break;
                case 4: amount = 15; break;
                case 5: amount = 20; break;
            }

            maxEnergy += amount;
            currentEnergy = maxEnergy;
            UpdateFillbar();
        }

    }

}