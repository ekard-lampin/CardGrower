using UnityEngine;

public class PlayerMoneyManager : MonoBehaviour
{
    [SerializeField]
    private int playerBalance = 0;
    public int GetPlayerBalance() { return playerBalance; }
    public bool CanAffordPrice(int price) { return playerBalance >= price; }
    public void DeductMoney(int money) { playerBalance -= money; }
    public void AddMoney(int money) { playerBalance += money; }
}