using UnityEngine;

public class PlayerMoneyManager : MonoBehaviour
{
    [SerializeField]
    private int playerBalance = 0;
    public int GetPlayerBalance() { return playerBalance; }
    public bool CanAffordPrice(int price) { return playerBalance >= price; }
    public void DeductMoney(int money) { playerBalance -= money; }
    public void AddMoney(int money) { playerBalance += money; }

    private float playerPassiveMoneyTimer = 0;

    void Update()
    {
        playerPassiveMoneyTimer += Time.deltaTime;

        if (playerPassiveMoneyTimer < GameManager.instance.GetPlayerPassiveMoneyTimerDuration()) { return; }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>().AddMoney(1);

        playerPassiveMoneyTimer = 0;
    }
}