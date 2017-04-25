using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NinjaBank", menuName = "NinjaData/NinjaBank")]
public class NinjaBank : ScriptableObject
{
    [SerializeField]
    protected NinjaDescription[] ninjaDescriptions;

    public int Count
    {
        get { return ninjaDescriptions.Length; }
    }

    public NinjaDescription[] GetRandomNinjas(int count)
    {
        var randoms = new List<int>();
        for (var i = 0; i < count; i++)
        {
            randoms.Add(i);
        }
        randoms.ShuffleInPlace();

        var ninjas = new NinjaDescription[count];
        for (var i = 0; i < count; i++)
        {
            ninjas[i] = ninjaDescriptions[randoms[i]];
        }

        return ninjas;
    }
}

[System.Serializable]
public class NinjaDescription
{
    public string Name;
    public Color Color;
    public Sprite Icon;
}