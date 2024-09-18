using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerNetcode : NetworkBehaviour
{
    public Camera myCamera;
    public AudioListener myAudioListener;

    public TMP_Text myNameTag;
    public Vector3 spawnLocation;

    public TMP_Text myJoinCode;

    public override void OnNetworkSpawn()
    {
        myCamera.gameObject.SetActive(IsOwner);
        myAudioListener.enabled = IsOwner;

        myNameTag.text = PlayerPrefs.GetString("clientName");

        

        if(IsOwner)
        {
            transform.position = spawnLocation;
        }
    }

    private void Update()
    {
        myJoinCode.text = PlayerPrefs.GetString("joinCode");
    }
}
