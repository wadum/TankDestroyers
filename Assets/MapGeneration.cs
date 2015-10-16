using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneration : MonoBehaviour
{


    public GameObject GroundPlane;
    public GameObject Wall;

    [Range(2,100)]
    public int FineGrained;

    private int[,] _map;

	// Use this for initialization
    private void Start()
    {
        _map = new int[10*FineGrained,10*FineGrained];
        Generate();
        DrawMap();
    }


    // Update is called once per frame
	void Update () {
	
	}

    public void Generate()
    {
        var h = _map.GetLength(0);
        var w = _map.GetLength(1);
        var count = _map.Length/50;


        for (int i = 0; i < count; i++)
        {

            Direction dir = (Direction) Random.Range(0, 3);

            var k = Random.Range(0, h);
            var l = Random.Range(0, w);
            while (Random.value < 0.8f)
            {
                _map[k, l] = 1;
                if(Random.value > 0.8f)
                    dir = (Direction) Random.Range(0, 3);

                switch (dir)
                {
                    case Direction.Norh:
                        if (k < w -2)
                            k++;
                        break;
                    case Direction.West:
                        if(l < w -2)
                            l++;
                        break;
                    case Direction.East:
                        if(k > 2)
                            k--;
                        break;
                    case Direction.South:
                        if(l > 2)
                            l--;
                        break;
                }
            }
        }
    }

    private void DrawMap()
    {
        var vertivces = GroundPlane.GetComponent<MeshFilter>().mesh.vertices;
        var ordered = vertivces.OrderBy(v => v.x).ThenBy(v => v.z);

        var min = GroundPlane.transform.TransformPoint(ordered.FirstOrDefault());
        var max = GroundPlane.transform.TransformPoint(ordered.LastOrDefault());

        var h = (max.x - min.x)/_map.GetLength(0);
        var w = (max.z - min.z)/_map.GetLength(1);

        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if (_map[i, j] == 1)
                {
                    var x = min.x + (h*i);
                    var z = min.z + (w*j);
                    var wall = Instantiate(Wall);
                    wall.transform.position = new Vector3(x, 0, z);
                }
            }
        }
    }

    enum Direction
    {
        Norh, South, East, West
    }


    #region GizmoDebug

    void OnDrawGizmos()
    {
        if (_map == null) return;

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
