using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Broken Soul/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("Core Stats")]
    public int maxHealth = 4;
    public int maxNostalgia = 100;

    [Header("Sanity Drain")]
    public int drainAmount = 1; // Quanta Sanity/Vita perde
    public float drainTickRate = 5f; // Ogni quanti secondi

    [Header("Combat")]
    public int attackDamage = 1;
    public float knockbackForce = 5f;
}