using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UiMineController : MonoBehaviour
    {

        public GameObject[] Mines;

        public void SetAvailableMines(int amount)
        {
            amount = Mathf.Min(amount, Mines.Length);
            for (var i = 0; i < Mines.Length; i++)
            {
                Mines[i].SetActive(i < amount);
            }
        }
    }
}
