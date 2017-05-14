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
}

[System.Serializable]
public class AccessoryDescription
{
    public Sprite Sprite;
    public Vector2 Offset;
}