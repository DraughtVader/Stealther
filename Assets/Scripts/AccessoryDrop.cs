using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoryDrop : MonoBehaviour
{
    public void SetUp(Sprite hatSprite)
    {
       GetComponent<SpriteRenderer>().sprite = hatSprite;
       GameManager.Instance.MatchFinished += Destroy;
    }

   private void Destroy()
   {
      GameManager.Instance.MatchFinished -= Destroy;
      Destroy(gameObject);
   }

}
