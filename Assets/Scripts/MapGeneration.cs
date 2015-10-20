using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Assets.Scripts.Variables;
using Assets.Scripts.Weapons;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MapGeneration : NetworkBehaviour
    {

        #region Fields

        [Header("Surroundings")]
        public GameObject GroundPlane;
        public GameObject Light;

        [Header("Obstacles")]
        public GameObject Wall;
        public int NumberOfObstabcles;

        [Header("Tanks")]
        public GameObject Player;
        public GameObject Enemy;
        public GameObject Hangar;
        public int NumberOfEnemies;

        [Header("Waypoints")]
        public GameObject Waypoint;
        public int NumberOfWaypoints;

        [Header("Pick Ups")]
        public GameObject[] PickUps;
        public int NumberOfPickUps;
        private int _grained;
        private int[,] _map;
        private int _h;
        private int _w;
        private Dictionary<string, int> _tankRotations;

        private const bool DrawDebugGizmo = false;
        #endregion


        public override void OnStartServer()
        {
            base.OnStartServer();
            _tankRotations = new Dictionary<string, int>();
            GenerateMap();
            AddWaypoints();
            AddTanks();
            GenerateSurrondings();
            AddPickUps();
            DrawMap();
        }

        private void GenerateSurrondings()
        {
            GroundPlane = Instantiate(GroundPlane);
            NetworkServer.Spawn(GroundPlane);
            Instantiate(Light);
            NetworkServer.Spawn(Light);

            var gs = (GameObject)Instantiate(Resources.Load("GameScripts"));
            NetworkServer.Spawn(gs);
        }

        private void GenerateMap()
        {
            _grained = (int)(GroundPlane.transform.localScale.x / Wall.transform.localScale.x);

            _map = new int[10 * _grained, 10 * _grained];
            _h = _map.GetLength(0);
            _w = _map.GetLength(1);

            PlaceHangar(_h - 3, _w - 3, 3, 3);
            PlaceHangar(2, _w - 3, 3, 3);
            PlaceHangar(_h - 3, 2, 0, 3);
            PlaceHangar(2, 2, 0, 3);

            AddEdges();
            AddObstacles();
        }

        private void PlaceHangar(int x, int z, int dir, int id)
        {
            _map[x, z] = id;
            _tankRotations[x + ":" + z] = dir;
            switch (dir)
            {
                    case 0:
                        _map[x, z + 1] = 5;
                        _map[x, z + 2] = 5;
                        break;
                    case 1:
                        _map[x + 1, z] = 5;
                        _map[x + 2, z] = 5;
                        break;
                    case 2:
                        _map[x, z - 1] = 5;
                        _map[x, z - 2] = 5;
                        break;
                    case 3:
                        _map[x - 1, z] = 5;
                        _map[x - 2, z] = 5;
                        break;
            }
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
            for (var i = 0; i < NumberOfObstabcles; i++)
            {
                var dir = Random.Range(0, 3);

                var k = Random.Range(0, _h);
                var l = Random.Range(0, _w);
                while (Random.value < 0.8f && _map[k, l] == 0)
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
            for (var i = 0; i < NumberOfWaypoints; i++)
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

        private void AddPickUps()
        {
            for (var i = 0; i < NumberOfPickUps; i++)
            {
                int x = 0, z = 0;
                while (_map[x, z] != 0)
                {
                    x = Random.Range(1, _h - 1);
                    z = Random.Range(1, _w - 1);
                }
                _map[x, z] = 6;
            }
        }

        private void AddTanks()
        {
            
            for (var i = 0; i < NumberOfEnemies; i++)
            {
                var dir = Random.Range(0, 3);
                int x = 0, z = 0, front = -1;
                while (_map[x, z] != 0 || front != 0)
                {
                    x = Random.Range(1, _h - 1);
                    z = Random.Range(1, _w - 1);

                    // Makes sure the hangar is not ganna be blocked 
                    try
                    {
                        switch (dir)
                        {
                            case 0:
                                front = _map[x, z + 1] + _map[x, z + 2];
                                break;
                            case 1:
                                front = _map[x + 1, z] + _map[x + 2, z];
                                break;
                            case 2:
                                front = _map[x, z - 1] + _map[x, z - 2];
                                break;
                            case 3:
                                front = _map[x - 1, z] + _map[x - 2, z];
                                break;
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        front = -1;
                    }
                }

                
                PlaceHangar(z, x, dir, 4);
            }
        }

        private void DrawMap()
        {
            // Gameobjects for groupin objects in the game hierachy
            var waypointParrent = new GameObject("Waypoints").transform;
            waypointParrent.parent = transform;
            var wallParrent = new GameObject("Walls").transform;
            wallParrent.parent = transform;
            var hangarParrent = new GameObject("Hangars").transform;


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
                            wall.transform.parent = wallParrent;
                            NetworkServer.Spawn(wall);
                            break;
                        case 2:
                            var wp = Instantiate(Waypoint);
                            wp.transform.position = new Vector3(x, 0, z);
                            wp.transform.parent = waypointParrent;
                            NetworkServer.Spawn(wp);
                            break;
                        case 3:
                            var pyRotation = _tankRotations[i + ":" + j] * 90;
                            var proration = new Quaternion(0, pyRotation, 0, 0);
                            var pHanger = Instantiate(Hangar).transform;
                            pHanger.position = new Vector3(x, 0, z);;
                            pHanger.rotation = proration;
                            pHanger.parent = hangarParrent;
                            NetworkServer.Spawn(pHanger.gameObject);
                            break;
                        case 4:
                            var yRotation = _tankRotations[i + ":" + j] * 90;
                            var roration = new Quaternion(0,yRotation, 0,0);

                            var tank = Instantiate(_map[i,j] == 3 ? Player : Enemy).transform;
                            tank.position = new Vector3(x, 0, z);
                            tank.rotation = roration;
                            
                            NetworkServer.Spawn(tank.gameObject);
                            var hangar = Instantiate(Hangar).transform;
                            hangar.position = tank.transform.position;
                            hangar.rotation = roration;

                            hangar.parent = hangarParrent;
                            NetworkServer.Spawn(hangar.gameObject);
                            
                            break;
                        case 6:
                            var pu = Instantiate(PickUps[Random.Range(0,PickUps.Length)]);
                            pu.transform.position = new Vector3(x, 0, z);
                            NetworkServer.Spawn(pu);
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


            if (!DrawDebugGizmo || _map == null) return;

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
