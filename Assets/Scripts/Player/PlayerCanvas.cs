using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class PlayerCanvas : NetworkBehaviour
{
    public PlayerHealthBar playerHealthBar;
    public GameOverScreen GOS;
    public TextMeshProUGUI goldCounter;
    public TextMeshProUGUI playersOnlineText;
    public ShopUI shopUI;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //GOS is gameoverscreen just if i forget somehow

    // Start is called before the first frame update
    void Start()
    {
        playerCount.OnValueChanged += OnPlayersOnlineChanged;
        GOS.gameObject.SetActive(false);
    }

    void OnPlayersOnlineChanged(int previous, int next)
    {
        playersOnlineText.SetText(playerCount.ToString() + " Players Online");
    }

    // Update is called once per frame
    void Update()
    {
        playerCount.Value = GameManager.instance.noOfPlayers;
    }

    public void SetHealthBar(float health)
    {
        playerHealthBar.SetHealthBar(health);
    }

    public void ShowGOS()
    {
        GOS.gameObject.SetActive(true);
    }

    public void UpdateGoldCounter(int count)
    {
        goldCounter.SetText(count.ToString());
    }

    public void ShowShop()
    {
        shopUI.gameObject.SetActive(true);
    }

    public void HideShop()
    {
        shopUI.gameObject.SetActive(false);
    }
}
