using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class SliderScripts : MonoBehaviour
    {
        public Slider slider;
        public Image fill;

        private void Start()
        {
            FillSlider();
        }

        public void FillSlider()
        {
            fill.fillAmount = slider.value;
        }
    }
}
