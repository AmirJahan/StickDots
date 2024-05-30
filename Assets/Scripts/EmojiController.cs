using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiController : MonoBehaviour
{
    [SerializeField] private GameObject emojiPrefab;
    [SerializeField] private string emojiAnimation;
    [SerializeField] private Transform spawnTransform; // This should be the players position 
    [SerializeField] private bool canSpawnEmoji = true;
    [SerializeField] private float emojiLifeTime = 5f;

    private void Awake()
    {
        spawnTransform = GameObject.FindGameObjectWithTag("EmojiSpawn").transform;
        canSpawnEmoji = true;
    }

    public void SpawnEmoji()
    {
        if(canSpawnEmoji)
        {
            canSpawnEmoji = false;
            StartCoroutine(EmojiCooldown());
            // Instatiates an emoji at the player's emoji transform
            GameObject emojiInstance = Instantiate(emojiPrefab, spawnTransform);
            emojiInstance.GetComponentInChildren<Animator>().Play(emojiAnimation);
            Destroy(emojiInstance, emojiLifeTime);
        }     
    }

    IEnumerator EmojiCooldown()
    {
        yield return new WaitForSeconds(emojiLifeTime);
        canSpawnEmoji = true;
    }
}
