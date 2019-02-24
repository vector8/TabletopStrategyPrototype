using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage
{
    public enum DamageType
    {
        Acid,
        Bludgeoning,
        Cold,
        Fire,
        Force,
        Lightning,
        Necrotic,
        Piercing,
        Poison,
        Psychic,
        Radiant,
        Slashing,
        Thunder
    }

    public Damage()
    {
        type = DamageType.Bludgeoning;
        dice = new int[6];
        DICE_VALUES = new int[] { 4, 6, 8, 10, 12, 20 };
    }

    public Damage(DamageType t, int d4, int d6, int d8, int d10, int d12, int d20, int modifier)
    {
        type = t;
        dice[0] = d4;
        dice[1] = d6;
        dice[2] = d8;
        dice[3] = d10;
        dice[4] = d12;
        dice[5] = d20;
        damageModifier = modifier;
        DICE_VALUES = new int[] { 4, 6, 8, 10, 12, 20 };
    }

    public DamageType type = DamageType.Bludgeoning;
    // number of each type of dice: d4, d6, d8, d10, d12, d20
    public int[] dice = new int[6];
    private int[] DICE_VALUES = new int[] { 4, 6, 8, 10, 12, 20 };
    public int damageModifier = 0;

    public int rollDamage()
    {
        int damage = damageModifier;
        for (int i = 0; i < dice.Length; i++)
        {
            for (int j = 0; j < dice[i]; j++)
            {
                damage += Random.Range(1, DICE_VALUES[i] + 1);
            }
        }

        Debug.Log(damage + " damage dealt.");
        return damage;
    }
}

public class Unit : MonoBehaviour
{
    public float movementRange, movementRemaining;
    public float attackRange;
    public int initiative;
    public int hitModifier;
    public int numberOfAttacks, attacksRemaining;
    public int defense;
    public int hp;
    public int tempHP;

    public Damage damage;

    private Vector3 moveStart, moveTarget;
    private float moveTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (moveTime < 1f)
        {
            moveTime += Time.deltaTime;
            if (moveTime > 1f)
            {
                moveTime = 1f;
            }

            transform.position = Vector3.Lerp(moveStart, moveTarget, moveTime);
        }
    }

    public bool hitRoll(Unit other)
    {
        if (attacksRemaining == 0)
            return false;

        attacksRemaining--;

        int roll = Random.Range(1, 21);
        
        if(other.defense > roll + hitModifier)
        {
            // attack dodged
            print("Miss!");
            return false;
        }
        else
        {
            // attack hit, other should take damage // TODO: remove this? only make this method determine IF a hit lands?
            print("Hit!");
            other.takeDamage(damage.rollDamage());
            return true;
        }
    }

    public void takeDamage(int damage)
    {
        if(tempHP > 0)
        {
            int t = tempHP;
            tempHP = Mathf.Max(tempHP - damage, 0); // deal damage to tempHP first
            damage = Mathf.Max(damage - tempHP, 0); // get remainder of damage
        }

        hp -= damage;

        if(hp <= 0)
        {
            // unit is dead
            gameObject.SetActive(false);
            // TODO: notify battlemanager
        }
    }

    public void moveTo(Vector3 point)
    {
        moveStart = transform.position;
        moveTarget = point;

        movementRemaining = Mathf.Max(movementRemaining - Vector3.Distance(moveTarget, moveStart), 0f);

        moveTime = 0f;
    }

    public void endTurn()
    {
        movementRemaining = movementRange;
        attacksRemaining = numberOfAttacks;
    }
}
