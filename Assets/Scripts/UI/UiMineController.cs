using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UiMineController : MonoBehaviour
    {

        public GameObject[] Mines;

        public void SetAvailableMines(int amount)
        {
            for (var i = 0; i < Mines.Length; i++)
            {
                Mines[i].SetActive(i < amount);
            }
        }
    }
}
