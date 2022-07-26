using System.Collections.Generic;
using UnityEngine;

public sealed class MapGenerator : MonoBehaviour
{
    [Range(5, 20)]
    [SerializeField] int x = 5, y = 5;
    [Range(0, 100)]
    [SerializeField] int possibilityOfBrick = 50;
    [SerializeField] CameraHandler cameraHandler;
    [SerializeField] Transform outWallPrefab, inWallPrefab, brickPrefab;
    internal List<Vector2> EmptyPlace { get; private set; }
    MapGenerator() { }
    internal void GenerateMap()
    {
        DeleteOldMap();
        GameObject tempGeneratedObj = null;
        Vector3 tempPos = new Vector3();
        int enemyGenDone = 0;
        EmptyPlace = new List<Vector2>();
        for (int i = 0; i < x; ++i)
        {
            for (int j = 0; j < y; ++j)
            {
                if (i == 0 || j == 0 || i == x - 1 || j == y - 1)
                {
                    tempGeneratedObj = outWallPrefab.gameObject;
                    tempPos = new Vector3(i, j, 0);
                }
                else if (i % 2 == 0 && j % 2 == 0)
                {
                    tempGeneratedObj = inWallPrefab.gameObject;
                    tempPos = new Vector3(i, j, 0);
                }
                else if ((i == 1 && (j == y - 2 || j == y - 3)) || (j == y - 2 && i == 2) || (i == 2 && j == y - 3))
                {
                    if (i == 1 && j == y - 2)
                    {
                        GameManager.Instance.GeneratePlayer(transform, new Vector2(i, j));
                    }
                }
                else
                {
                    int range = Random.Range(0, 100);
                    if (range < possibilityOfBrick)
                    {
                        tempGeneratedObj = brickPrefab.gameObject;
                        tempPos = new Vector3(i, j, 0);
                    }
                    else
                    {
                        EmptyPlace.Add(new Vector2(i, j));
                    }
                }
                Instantiate(tempGeneratedObj, transform).transform.position = tempPos;
            }
        }
        while (enemyGenDone < GameManager.Instance.MaxEnemyAllow && EmptyPlace.Count > 0)
        {
            int randomEnemyPlace = Random.Range(0, EmptyPlace.Count);
            Vector2 enemyPlace = EmptyPlace[randomEnemyPlace];
            EmptyPlace.RemoveAt(randomEnemyPlace);
            //Debug.Log($"enemy pos {enemyPlace.x} {enemyPlace.y}");
            GameManager.Instance.GenerateEnemy(transform, enemyPlace);
            ++enemyGenDone;
        }
        cameraHandler.CenterMe((float)x / 2, (float)y / 2);
    }
    internal void DeleteOldMap()
    {
        if (transform.childCount > 0)
        {
            var childTransform = GetComponentsInChildren<Transform>();
            for (int i = 1; i < childTransform.Length; ++i)
            {
                if (childTransform[i] != null)
                {
                    DestroyImmediate(childTransform[i].gameObject);
                }
            }
        }
    }
}
