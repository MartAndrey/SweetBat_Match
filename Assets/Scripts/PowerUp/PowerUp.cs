using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TypePowerUp { InfiniteLife, Bomb, Lightning, Potion }

public class PowerUp : MonoBehaviour
{
    public TypePowerUp TypePowerUp { get { return typePowerUp; } }
    public TMP_Text TextAmount { get { return textAmount; } set { textAmount = value; } }

    [SerializeField] TypePowerUp typePowerUp;
    [SerializeField] TMP_Text textAmount;
}