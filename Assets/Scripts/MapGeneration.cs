using System.Linq;
using Assets.Scripts.Variables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets
{
    public class MapGeneration : MonoBehaviour
    {

        #region Fields
        public GameObject GroundPlane;

        [Header("Obstacles")]
        public GameObject Wall;
        public int NumberOfObstabcles;

        [Header("Tanks")]
        public GameObject Player;
        public GameObject Enemy;
        public int NumberOfEnemies;

        [Header("Waypoints")]
        public GameObject Waypoint;
        public int NumberOfWaypoints;

        private int _grained;
        private int[,] _map;
        private int _h;
        private int _w;

        private Transform _waypointsParrent;
        private Transform _wallsParrent;

        private const bool DrawDebugGizmo = false;
        #endregion

        private void Start()
        {
            _waypointsParrent = transform.FindChild(Constants.MapGeneration.WaypointsGo);
            _wallsParrent = transform.FindChild(Constants.MapGeneration.WallsGo);

            GenerateMap();
            AddWaypoints();
            AddTanks();
            DrawMap();
        }

        private void GenerateMap()
        {
            _grained = (int)(GroundPlane.transform.localScale.x / Wall.transform.localScale.x);

            _map = new int[10 * _grained, 10 * _grained];
            _h = _map.GetLength(0);
            _w = _map.GetLength(1);

            AddEdges();
            AddObstacles();
        }

        private void AddEdges()
        {
            for (int i = 0; i < _h; i++)
            {
                _map[i, 0] = 1;
                _map[i, _w - 1] = 1;
            }

            for (int i = 0; i < _w; i++)
            {
                _map[0, i] = 1;
                _map[_h - 1, i] = 1;
            }
        }

        private void AddObstacles()
        {
            for (int i = 0; i < NumberOfObstabcles; i++)
            {
                int dir = Random.Range(0, 3);

                var k = Random.Range(0, _h);
                var l = Random.Range(0, _w);
                while (Random.value < 0.8f)
                {
                    _map[k, l] = 1;
                    if (Random.value > 0.8f)
                        dir = Random.Range(0, 3);

                    switch (dir)
                    {
                        case 0:
                            if (k < _w - 2)
                                k++;
                            break;
                        case 1:
                            if (l < _w - 2)
                                l++;
                            break;
                        case 2:
                            if (k > 2)
                                k--;
                            break;
                        default:
                            if (l > 2)
                                l--;
                            break;
                    }
                }
            }
        }

        private void AddWaypoints()
        {
            for (int i = 0; i < NumberOfWaypoints; i++)
            {
                int x = 0, z = 0;
                while (_map[x, z] != 0)
                {
                    x = Random.Range(1, _h - 1);
                    z = Random.Range(1, _w - 1);
                }
                _map[x, z] = 2;
            }
        }

        private void AddTanks()
        {
            for (var i = 0; i < NumberOfEnemies; i++)
            {
                int x = 0, z = 0;
                while (_map[x, z] != 0)
                {
                    x = Random.Range(1, _h - 1);
                    z = Random.Range(1, _w - 1);
                }
                _map[x, z] = i == 0 ? 3 : 4;
            }
        }

        private void DrawMap()
        {
            var vertivces = GroundPlane.GetComponent<MeshFilter>().mesh.vertices;
            var ordered = vertivces.OrderBy(v => v.x).ThenBy(v => v.z);

            var min = GroundPlane.transform.TransformPoint(ordered.FirstOrDefault());
            var max = GroundPlane.transform.TransformPoint(ordered.LastOrDefault());

            var h = (max.x - min.x) / _h;
            var w = (max.z - min.z) / _w;

            for (var i = 0; i < _map.GetLength(0); i++)
            {
                for (var j = 0; j < _map.GetLength(1); j++)
                {
                    var x = min.x + (h*i);
                    var z = min.z + (w*j);


                    switch (_map[i, j])
                    {
                        case 1:
                            var wall = Instantiate(Wall);
                            wall.transform.position = new Vector3(x, 0, z);
                            wall.transform.parent = _wallsParrent;
                            break;
                        case 2:
                            var wp = Instantiate(Waypoint);
                            wp.transform.position = new Vector3(x, 0, z);
                            wp.transform.parent = _waypointsParrent;
                            break;
                        case 3:
                            var player = Instantiate(Player);
                            player.transform.position = new Vector3(x, 0, z);
                            break;
                        case 4:
                            var enemy = Instantiate(Enemy);
                            enemy.transform.position = new Vector3(x, 0, z);
                            break;
                        default:
                            continue;
                    }
                }
            }
        }

        #region GizmoDebug

        void OnDrawGizmos()
        {


            if (!DrawDebugGizmo) return;

            var h = _map.GetLength(0);
            var w = _map.GetLength(1);

            for (var x = 0; x < w; x++)
                for (var y = 0; y < h; y++)
                {
                    Gizmos.color = (_map[x, y] == 1) ? Color.black : Color.white;
                    var pos = new Vector3(-w / 2 + x + .5f, 0, -h / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
        }

        #endregion
    }
}
