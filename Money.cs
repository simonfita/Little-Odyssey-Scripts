using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Money
{
    public System.Action<Money> amountChanged;

    public int bronze { get; private set; }
    public int silver { get; private set; }
    public int gold { get; private set; }

    public int amount { get { return bronze + silver * 100 + gold * 10000; } set { bronze = value % 100; silver = value % 10000 / 100; gold = value / 10000; amountChanged?.Invoke(this); } }

    public Money(int _amount)
    {
        bronze = 0;
        silver = 0;
        gold = 0;
        amountChanged = null;
        amount = _amount;
    }
}
