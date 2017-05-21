using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NinjaBank", menuName = "NinjaData/NinjaBank")]
public class NinjaBank : ScriptableObject
{
    [SerializeField]
    protected NinjaDescription[] ninjaDescriptions;

    public int Count
    {
        get { return ninjaDescriptions.Length; }
    }

    public List<NinjaDescription> GetRandomNinjas(int count)
    {
        var randoms = new List<int>();
        for (var i = 0; i < Count; i++)
        {
            randoms.Add(i);
        }
        randoms.ShuffleInPlace();

        var ninjas = new List<NinjaDescription>();
        for (var i = 0; i < count; i++)
        {
            ninjas.Add(ninjaDescriptions[randoms[i]]);
        }

        return ninjas;
    }

    public NinjaDescription GetNextDescription(NinjaDescription current)
    {
        var index = Array.IndexOf(ninjaDescriptions, current);
        return ninjaDescriptions[(index + 1) % Count];
    }

    public NinjaDescription GetPreviousDescription(NinjaDescription current)
    {
        var index = Array.IndexOf(ninjaDescriptions, current);
        return index == 0 ? ninjaDescriptions[Count - 1] : ninjaDescriptions[index - 1];
    }

    public NinjaDescription GetRandomNinja(List<NinjaDescription> inUse)
    {
        var ninja = ninjaDescriptions[Random.Range(0, Count)];
        while (inUse.Contains(ninja))
        {
            ninja = ninjaDescriptions[Random.Range(0, Count)];
        }
        return ninja;
    }
}

[System.Serializable]
public class NinjaDescription
{
    public string Name;
    public Color Color;
    public Sprite Icon;
}