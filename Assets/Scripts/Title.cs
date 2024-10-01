using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;

public class Title : MonoBehaviour
{

    public TMP_InputField nameText;
    public TMP_InputField joinCodeText;

    private bool startingHost;

    // Start is called before the first frame update
    async void Start()
    {
        startingHost = false;
        if(PlayerPrefs.HasKey("clientName"))
        {
            nameText.text = PlayerPrefs.GetString("clientName");
        }

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.F5) && Input.GetKey(KeyCode.H) && startingHost != true)
        {
            if (nameText.text != "" && NetworkManager.Singleton != null)
            {
                Debug.Log("Starting Host");
                startingHost = true;
                createRelay();
            }
            else
            {
                Debug.Log("NetWork Manager is Null");
            }
        }
    }

    public void joinButton()
    {
        if(nameText.text != "" && joinCodeText.text != "")
        {
            NameManager.Instance.PlayerName = nameText.text;
            joinRelay(joinCodeText.text);
        }
    }

    

    public void updateSavedName()
    {
        PlayerPrefs.SetString("clientName", nameText.text);
    }

    private async void joinRelay(string joinCode)
    {
        try
        {
            //Attempt join with given join code
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            //Create Relay Server Data
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            //Set Relay Server Data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            //Join multiplayer world and start network client
            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        catch (RelayServiceException e)
        {
            //Catch exception from attempting to join
            Debug.Log(e);
        }
    }

    private async void createRelay()
    {
        try
        {
            //Create allocation
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            //Grab join code from the created allocation
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            PlayerPrefs.SetString("joinCode", joinCode);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);


            //Join multiplayer world as network server
            NetworkManager.Singleton.StartServer();

            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
