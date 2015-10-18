using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class RadialSlider: MonoBehaviour
    {

        public float StartValue = 100;

        private Text _text;

        void Start()
        {
            _text = GetComponentInChildren<Text>();
            SetValue(StartValue);
        }

        public float GetValue()
        {
            return GetComponent<Image>().fillAmount*400;
        }

        public void SetValue(float value)
        {
            var angle = Mathf.Max(Mathf.Min(value,100) / 400, 0.01f);

            GetComponent<Image>().fillAmount = angle;

            var col = Color.Lerp(Color.red, Color.green, angle * 4);
            col.a = 0.5f;
            GetComponent<Image>().color = col;

            if (!_text) return;
            _text.text = Mathf.FloorToInt(angle*400) + "%";
            _text.color = col;
        }
    
    }
}
