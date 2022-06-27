using System.Collections.Generic;

using UnityEngine;

public class Randomizer : MonoBehaviour
{
    System.Random _random;

    [Range(0, 1f)]
    public float pourcentageChance;

    public int hitcounter;

    public List<bool> hitChance = new List<bool>();

    // Start is called before the first frame update
    void Start()
    {
        _random = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        if (hitChance.Count < 100)
            Randomize();
            
    }

    void Randomize() 
    {
        if (_random.Next(1, 101) <= 100f * pourcentageChance)
        {
            hitChance.Add(true);
            return;
        }

        hitChance.Add(false);
    }
}
