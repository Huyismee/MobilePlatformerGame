using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : SerializedMonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MovePlayer(Transform position) {  
        PlayerController.Instance.MoveToDestination(position);
    }

    public void GoToNextLevel(int level) 
    {
        GameManager.Instance.LoadGameplay(level);
    }
}
