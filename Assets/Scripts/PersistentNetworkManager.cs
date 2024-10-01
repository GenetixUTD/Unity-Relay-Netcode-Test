using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PersistentNetworkManager : MonoBehaviour
{
    private void Awake()
    {
        
        // Check if there's already a NetworkManager in the scene
        if (NetworkManager.Singleton != null && NetworkManager.Singleton != this)
        {
            // Destroy this instance if a NetworkManager already exists
            Destroy(gameObject);
            return;
        }

        // Make sure the NetworkManager persists between scenes
        DontDestroyOnLoad(gameObject);
    }
}
