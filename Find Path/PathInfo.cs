using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PathInfo 
{
    public List<int> Path = new List<int>();

    public PathInfo(List<int> path)
    {
        Path = path;
    }
    public PathInfo() { }
   

    public void PrintPath()
    {
        Debug.Log($"Path: {string.Join(" -> ", Path)}");
    }
}
