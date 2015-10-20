using System.Collections;
using UnityEngine;

namespace Assets.Scripts.PickUps
{
    public class PickUpManager : MonoBehaviour
    {

        public GameObject PickUpModel;

        void OnEnable()
        {
            PickUpModel.transform.localPosition = Vector3.up*2;
            PickUpModel.transform.localScale = Vector3.one * 4;
            StartCoroutine(Rotate());
        }

        IEnumerator Rotate()
        {
            while (true)
            {
                PickUpModel.transform.Rotate(0,1,0);
                yield return null;
            }
        }
    }
}
