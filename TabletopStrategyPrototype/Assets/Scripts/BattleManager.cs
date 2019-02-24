using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public List<Unit> units;

    public List<KeyValuePair<int, Unit>> initiativeList;
    public int initiativeListIndex = 0;
    public int currentRound = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(units.Count == 0)
        {
            Debug.LogError("No units added to battle manager list!");
            return;
        }

        // Create the initiative list
        initiativeList = new List<KeyValuePair<int, Unit>>();

        foreach(Unit u in units)
        {
            int initiative = Random.Range(1, 21) + u.initiative;
            KeyValuePair<int, Unit> initEntry = new KeyValuePair<int, Unit>(initiative, u);
            initiativeList.Add(initEntry);
        }

        initiativeList.Sort(delegate (KeyValuePair<int, Unit> x, KeyValuePair<int, Unit> y)
        {
            return x.Key.CompareTo(y.Key);
        });

        BattleUIController.instance.selectedUnit = initiativeList[0].Value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void nextTurn()
    {
        initiativeListIndex++;
        if(initiativeListIndex >= initiativeList.Count)
        {
            initiativeListIndex = 0;
            currentRound++;
        }

        BattleUIController.instance.selectedUnit = initiativeList[initiativeListIndex].Value;
    }
}
