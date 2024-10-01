using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NameManager : MonoBehaviour
{
    public static NameManager Instance {  get; private set; }

    public string PlayerName { get; set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
