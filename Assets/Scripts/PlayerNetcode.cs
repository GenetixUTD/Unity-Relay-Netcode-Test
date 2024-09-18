using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class PlayerNetcode : NetworkBehaviour
{
    public Camera myCamera;
    public AudioListener myAudioListener;
    public TMP_Text myNameTag;

    private NetworkVariable<FixedString32Bytes> myPlayerName = new NetworkVariable<FixedString32Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    public Vector3 spawnLocation;

    public override void OnNetworkSpawn()
    {
        myCamera.gameObject.SetActive(IsOwner);
        myAudioListener.enabled = IsOwner;

        

        if(IsOwner)
        {
            transform.position = spawnLocation;
            myPlayerName.Value = NameManager.Instance.PlayerName;
            myCamera.gameObject.SetActive(true);
        }
        else
        {
            myCamera.gameObject.SetActive(false);
        }

        myPlayerName.OnValueChanged += UpdatePlayerNameDisplay;

        SetupPlayerName();
    }

    private void SetupPlayerName()
    {
        myNameTag.text = myPlayerName.Value.ToString();
    }

    private void UpdatePlayerNameDisplay(FixedString32Bytes oName, FixedString32Bytes nName)
    {
        if (myNameTag.text != null)
        {
            myNameTag.text = nName.ToString();
        }
    }
}
