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

    // Start is called before the first frame update
    async void Start()
    {
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
        if(Input.GetKeyDown(KeyCode.F5) && Input.GetKeyDown(KeyCode.H))
        {
            if (nameText.text != "" && NetworkManager.Singleton != null)
            {
                Debug.Log("Starting Host");
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
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void createRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            PlayerPrefs.SetString("joinCode", joinCode);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartServer();

            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
