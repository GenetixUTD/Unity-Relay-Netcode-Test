using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(IsServer)
        {
            Vector3 push = collision.gameObject.GetComponent<CharacterController>().velocity * rb.mass;
            rb.AddForce(push, ForceMode.Impulse);
            //SetBallPositionClientRpc(transform.position, rb.velocity, transform.rotation);
        }
    }

    [ClientRpc]
    public void SetBallPositionClientRpc(Vector3 position, Vector3 velocity, Quaternion rotaion)
    {
        rb.position = position;
        rb.velocity = velocity;
        rb.rotation = rotaion;
    }
}
