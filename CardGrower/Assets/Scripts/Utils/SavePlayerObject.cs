using System;

[Serializable]
public class SavePlayerObject
{
    public int balance;
    public int GetBalance() { return balance; }
    public void SetBalance(int balance) { this.balance = balance; }

    public SaveCardObject[] cards;
    public SaveCardObject[] GetCards() { return cards; }
    public void SetCards(SaveCardObject[] cards) { this.cards = cards; }
}