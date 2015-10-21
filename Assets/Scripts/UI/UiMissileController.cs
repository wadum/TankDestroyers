using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UiMissileController : MonoBehaviour
    {

        public GameObject[] Missiles;

        public void SetAvailableMissiles(int amount)
        {
            for (var i = 0; i < Missiles.Length; i++)
            {
                Missiles[i].SetActive(i < amount);
            }
        }
    }
}
