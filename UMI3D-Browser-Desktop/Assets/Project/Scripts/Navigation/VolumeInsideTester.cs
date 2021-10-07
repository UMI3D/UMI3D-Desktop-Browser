using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.cdk.volumes;

public class VolumeInsideTester : MonoBehaviour
{
    public GameObject prefab;
    public Bounds bounds;
    public Vector3 subdiv = Vector3.one * 3;
    [Range(0f, 1f)]
    public float gizmosSize = 1;


    List<List<List<bool>>> isInside = new List<List<List<bool>>>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        ResetMapping();
    }

    [ContextMenu("Reset mapping")]
    public void ResetMapping()
    {
        for (int i = 0; i < subdiv.x; i++)
        {
            List<List<bool>> line = new List<List<bool>>();
            for (int j = 0; j < subdiv.y; j++)
            {
                List<bool> values = new List<bool>();
                for (int k = 0; k < subdiv.z; k++)
                {
                    values.Add(false);
                }
                line.Add(values);
            }
            isInside.Add(line);
        }
    }

    [ContextMenu("Map")]
    public void Map()
    {
        for (int i = 0; i < subdiv.x; i++)
        {
            for (int j = 0; j < subdiv.y; j++)
            {
                for (int k = 0; k < subdiv.z; k++)
                {
                   Vector3 pos = bounds.min + Vector3.Scale(
                                bounds.size,
                                new Vector3(
                                    i / ((float)subdiv.x - 1),
                                    j / ((float)subdiv.y - 1),
                                    k / ((float)subdiv.z - 1)
                                    )
                                );
                    isInside[i][j][k] = VolumePrimitiveManager.GetPrimitives().Exists(p => p.IsInside(pos));
                }
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (subdiv.x <= 0)
            return;
        if (subdiv.y <= 0)
            return;
        if (subdiv.z <= 0)
            return;

        for (int i=0; i< isInside.Count; i++)
        {
            for (int j = 0; j < isInside[i].Count; j++)
            {
                for (int k = 0; k < isInside[j].Count; k++)
                {
                    if (isInside[i][j][k])
                    {
                        Gizmos.DrawWireCube(
                            bounds.min + Vector3.Scale(
                                bounds.size,
                                new Vector3(
                                    i / ((float)subdiv.x - 1),
                                    j / ((float)subdiv.y - 1),
                                    k / ((float)subdiv.z - 1)
                                    )
                                )
                           , bounds.size / 50f * gizmosSize);
                    }
                }           
            }
        }
    }
}
