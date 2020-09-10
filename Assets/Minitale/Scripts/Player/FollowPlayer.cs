using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : NetworkBehaviour
{

    public static GameObject player;
    public float yOffset = 0f;

    // Update is called once per frame
    void Update()
    {
        if(player == null && GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player != null)
            transform.position = new Vector3(player.transform.position.x, transform.position.y + yOffset, player.transform.position.z);
    }
}
