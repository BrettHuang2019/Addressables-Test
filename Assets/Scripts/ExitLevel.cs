using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{
    public void Exit()
    {
        GameManager.Instance.UnloadScene();
    }
    
}
