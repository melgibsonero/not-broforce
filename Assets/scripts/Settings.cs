using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace not_broforce {
    public class Settings: MonoBehaviour {

        [SerializeField]
        private Text musicVolume;

        [SerializeField]
        private Text effectVolume;

        [SerializeField]
        private Slider musicSlider;

        [SerializeField]
        private Slider effectSlider;

        private void Start()
        {
            musicSlider.value = GameManager.Instance.musicVolume;
            effectSlider.value = GameManager.Instance.effectVolume;
        }
        // Update is called once per frame
        void Update() {
            musicVolume.text = "Music Volume: " + (int)(musicSlider.value * 100) + "%";
            effectVolume.text = "Effect Volue: " + (int)(effectSlider.value * 100) + "%";
            GameManager.Instance.effectVolume = effectSlider.value;
            GameManager.Instance.musicVolume = musicSlider.value;
        }
    }
}
