using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {

        public int Health = 3;

        [Header("Controls")]
        public float RotationMagnitude = 0.5f;
        public float MovementMagnitude = 0.1f;

        [Space(10)]
        [Header("Tank Shot")]
        public AudioClip TankShotSound;
        public float TankShotVolume = 0.3f;

        [Space(10)]
        [Header("Tank Shot")]
        public GameObject MinePrefab;
        public int MineStartAmount = 3;


        private Rigidbody _rb;
        private AudioSource _shotSource;
        private int _health;
        private int _mineAmount;

        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody>();
            _shotSource = gameObject.AddComponent<AudioSource>();
            _shotSource.clip = TankShotSound;
            _shotSource.loop = false;
            _shotSource.playOnAwake = false;
            _shotSource.volume = TankShotVolume;
            _shotSource.minDistance = 5;
            _shotSource.maxDistance = 20;
            _shotSource.rolloffMode = AudioRolloffMode.Linear;
            _shotSource.spatialBlend = 1;
            _health = Health;
            _mineAmount = MineStartAmount;
        }

        // Update is called once per frame
        void Update () {
            if (Input.GetKey("a"))
                _rb.AddTorque(new Vector3(0, -1*RotationMagnitude,0));
            if (Input.GetKey("d"))
                _rb.AddTorque(new Vector3(0, 1*RotationMagnitude, 0));
            if (Input.GetKey("w"))
                _rb.AddForce(transform.forward * MovementMagnitude);
            if (Input.GetKey("s"))
                _rb.AddForce(transform.forward * -MovementMagnitude);
            if (Input.GetKeyDown("space"))
            {
                GameObject.FindGameObjectWithTag("GameScripts").GetComponent<BulletController>().FireBullet(transform.position, transform.forward);
                _shotSource.Play();
            }
            if (Input.GetKeyDown("left ctrl"))
            {
                var mine = (GameObject)Instantiate(MinePrefab);
                mine.transform.position = transform.position;
                mine.transform.Translate(transform.forward*-3);
            }
        }

        public void HitByBullet()
        {
            _health--;
            if (_health < 1)
                Application.LoadLevel(Application.loadedLevel);
        }

        public void AddMines(int amount)
        {
            _mineAmount += amount;
        }
    }
}
