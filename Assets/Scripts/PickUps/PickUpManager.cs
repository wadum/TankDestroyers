using System.Collections;
using UnityEngine;

namespace Assets.Scripts.PickUps
{
    public class PickUpManager : MonoBehaviour
    {

        public GameObject PickUpModel;

        void Start()
        {
            PickUpModel.transform.localPosition = Vector3.up*2;
            PickUpModel.transform.localScale = Vector3.one * 4;
            PickUpModel.transform.localRotation = Quaternion.Euler(0,0,45);
            StartCoroutine(Rotate());
        }

        IEnumerator Rotate()
        {
            while (true)
            {
                PickUpModel.transform.Rotate(1,1,0);
                yield return null;
            }
        }
    }
}
