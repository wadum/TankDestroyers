﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Variables;
using Assets.Scripts.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MapGeneration : MonoBehaviour
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
        public int NumberOfPickUps;
        public GameObject[] PickUpTypes;

        private int _grained;
        private int[,] _map;
        private int _h;
        private int _w;
        private Dictionary<string, int> _tankRotations;

        private const bool DrawDebugGizmo = false;
        #endregion

        private void Start()
        {
            GenerateSurrondings();
            GenerateMap();
            AddWaypoints();
            AddTanks();
            AddPickUps();
            DrawMap();
        }

        private void GenerateSurrondings()
        {
            GroundPlane = Instantiate(GroundPlane);
            Instantiate(Light);

            var gs = new GameObject("GameScripts");
            gs.tag = Constants.Tags.GameScripts;
            gs.AddComponent<BulletManager>().BulletSpeed = 0.5f;
            gs.AddComponent<MissileManager>().MissileSpeed = 0.5f;
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

        private void AddPickUps()
        {
            for (int i = 0; i < NumberOfPickUps; i++)
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
            _tankRotations = new Dictionary<string, int>();
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

                _tankRotations.Add(x + ":" + z, dir);
                _map[x, z] = i == 0 ? 3 : 4;

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
                            break;
                        case 2:
                            var wp = Instantiate(Waypoint);
                            wp.transform.position = new Vector3(x, 0, z);
                            wp.transform.parent = waypointParrent;
                            break;
                        case 3:
                        case 4:

                            var yRotation = _tankRotations[i + ":" + j] * 90;
                            var roration = new Quaternion(0,yRotation, 0,0);

                            var tank = Instantiate(_map[i,j] == 3 ? Player : Enemy).transform;
                            tank.position = new Vector3(x, 0, z);
                            tank.rotation = roration;
                            
                            var hangar = Instantiate(Hangar).transform;
                            hangar.position = tank.transform.position;
                            hangar.rotation = roration;

                            hangar.parent = hangarParrent;
                            
                            break;
                        case 6:
                            var pu = Instantiate(PickUpTypes[Random.Range(0,PickUpTypes.Length)]);
                            pu.transform.position = new Vector3(x, 0, z);
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
