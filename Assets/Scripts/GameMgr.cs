using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum RESOURCE
{
    IRON
}

public class GameMgr : MonoBehaviour
{
    public static GameMgr inst;

    // Start is called before the first frame update
    void Start()
    {
        inst = this; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
