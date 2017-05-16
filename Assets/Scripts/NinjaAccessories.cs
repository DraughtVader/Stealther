using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NinjaAccessories", menuName = "NinjaData/NinjaAccessories")]
public class NinjaAccessories : ScriptableObject
{
    [SerializeField]
    protected AccessoryDescription[] ninjaAccessories;

    public int Count
    {
        get { return ninjaAccessories.Length; }
    }

    public AccessoryDescription GetRandomAccessory()
    {
        return ninjaAccessories[Random.Range(0, Count)];
    }

    public AccessoryDescription[] GetRandomAccessories(int count)
    {
        var randoms = new List<int>();
        for (var i = 0; i < Count; i++)
        {
            randoms.Add(i);
        }
        randoms.ShuffleInPlace();

        var ninjas = new AccessoryDescription[count];
        for (var i = 0; i < count; i++)
        {
            ninjas[i] = ninjaAccessories[randoms[i]];
        }

        return ninjas;
    }
}

[System.Serializable]
public class AccessoryDescription
{
    public Sprite Sprite;
    public Vector2 Offset;
}