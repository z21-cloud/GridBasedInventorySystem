using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Player.Status
{
    public class PlayerStatus : MonoBehaviour
    {
        #region SINGLETON
        private static PlayerStatus instance = null;
        public static PlayerStatus Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("GameManager").AddComponent<PlayerStatus>();
                }
                return instance;
            }
        }

        private void OnEnable()
        {
            instance = this;
        }
        #endregion

        [SerializeField] Image healthImage; 
        [SerializeField] Image energyImage; 
        [SerializeField] Image waterImage; 
        [SerializeField] TextMeshProUGUI healthText; 
        [SerializeField] TextMeshProUGUI energyText; 
        [SerializeField] TextMeshProUGUI waterText;

        public int health;
        public int energy;
        public int water;

        public int energyFactor = 2;
        public float energyDecreaseTime = 2f;
        public int waterFactor;
        public float waterDecreaseTime = 1f;

        void Start()
        {
            healthText.text = health.ToString();
            energyText.text = energy.ToString();
            waterText.text = water.ToString();

            healthImage.fillAmount = health / 100f;
            energyImage.fillAmount = energy / 100f;
            waterImage.fillAmount = water / 100f;

            StartCoroutine(DecreaseEnergy());
            StartCoroutine(DecreaseWater());
        }

        IEnumerator DecreaseEnergy()
        {
            while(true)
            {
                yield return new WaitForSeconds(energyDecreaseTime);
                energy = energy - energyFactor;
                energy = Mathf.Clamp(energy, 0, 100);
                energyText.text = energy.ToString();
                Color(energyImage, energy);
            }
        }

        IEnumerator DecreaseWater()
        {
            while (true)
            {
                yield return new WaitForSeconds(waterDecreaseTime);
                water = water - waterFactor;
                water = Mathf.Clamp(water, 0, 100);
                waterText.text = water.ToString();
                Color(waterImage, water);
            }
        }

        private void Color(Image image, float value)
        {
            byte x = (byte)(value * 255f / 100f);
            image.color = new Color32(255, x, x, 255);
            image.fillAmount = value / 100f;
        }

        public void SetEnergy(int value)
        {
            StartCoroutine(ISetEnergy(value));
        }

        public void SetWater(int value)
        {
            StartCoroutine(ISetWater(value));
        }

        public void SetHealth(int value)
        {
            health = value;
            healthText.text = health.ToString();
            Color(healthImage, health);
        }

        private IEnumerator ISetEnergy(int value)
        {
            //1-2-3 = 6
            int factor = value / 6;

            energy = Mathf.Clamp(energy + factor * 3, 0, 100);
            energyText.text = energy.ToString();
            Color(energyImage, energy);
            yield return new WaitForSeconds(.5f);

            energy = Mathf.Clamp(energy + factor * 2, 0, 100);
            energyText.text = energy.ToString();
            Color(energyImage, energy);
            yield return new WaitForSeconds(.5f);

            energy = Mathf.Clamp(energy + factor * 1, 0, 100);
            energyText.text = energy.ToString();
            Color(energyImage, energy);
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator ISetWater(int value)
        {
            //1-2-3 = 6
            int factor = value / 6;

            water = Mathf.Clamp(water + factor * 3, 0, 100);
            waterText.text = water.ToString();
            Color(waterImage, water);
            yield return new WaitForSeconds(.5f);

            water = Mathf.Clamp(water + factor * 2, 0, 100);
            waterText.text = water.ToString();
            Color(waterImage, water);
            yield return new WaitForSeconds(.5f);

            water = Mathf.Clamp(water + factor * 1, 0, 100);
            waterText.text = water.ToString();
            Color(waterImage, water);
            yield return new WaitForSeconds(1f);
        }
    }
}
