using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : NetworkBehaviour
{
   
    public void Destroy()
    {
        Debug.Log($"Destroying {gameObject}");
        NetworkServer.Destroy(gameObject);
        //Destroy(gameObject);
    }

}
