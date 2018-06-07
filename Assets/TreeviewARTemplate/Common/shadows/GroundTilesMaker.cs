using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTilesMaker : MonoBehaviour {

    public GameObject tilePrefab;
    
    public Vector2 tilesCount;
    public Vector2 tilesGap;

    void Awake() {

        DestroyTiles();
        MakeTiles();
	}

    void DestroyTiles()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }

    void MakeTiles()
    {
        Vector3 startPos = new Vector3(
            -(tilesCount.x / 2f - 0.5f) * tilesGap.x,
            0f,
            -(tilesCount.y / 2f - 0.5f) * tilesGap.y
        );
        for(int x=0; x<tilesCount.x; x++)
        {
            for (int z=0; z<tilesCount.y; z++)
            {
                Vector3 pos = startPos + new Vector3(
                    x * tilesGap.x,
                    0f,
                    z * tilesGap.y
                );
                Instantiate(tilePrefab, pos, Quaternion.identity, transform);
            }
        }
    }
	
}
