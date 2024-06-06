using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class EmojiController : MonoBehaviour
{
    [SerializeField] private GameObject emojiPrefab;
    [SerializeField] private string emojiAnimation;
    [SerializeField] private Transform spawnTransform; // This should be the players position 
    [SerializeField] private bool canSpawnEmoji = true;
    [SerializeField] private float emojiLifeTime = 5f;

    [Header("Player data")]
    [SerializeField] private int _userIndex;
    [SerializeField] private string _userName;

    GamePlayManager gamePlayManager;

    private void Awake()
    {
        spawnTransform = GameObject.FindGameObjectWithTag("EmojiSpawn").transform;
        canSpawnEmoji = true;

        gamePlayManager = FindAnyObjectByType<GamePlayManager>();

        //PhotonNetwork.LocalPlayer.
    }

    public void BTPlayEmoji()
    {
        SpawnEmoji();
    }
    
    public void SpawnEmoji()               
    {
        gamePlayManager = FindAnyObjectByType<GamePlayManager>();
        if (canSpawnEmoji)
        {
            Player player = gamePlayManager.players[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            if (player.playerIndex == PhotonNetwork.LocalPlayer.ActorNumber - 1)
            {
                _userIndex = player.playerIndex;
                _userName = player.playerName;

                canSpawnEmoji = false;
                StartCoroutine(EmojiCooldown());
                // Instatiates an emoji at the player's emoji transform
                    
                GameObject emojiInstance = PhotonNetwork.Instantiate(emojiPrefab.name, player.gameObject.transform.position, emojiPrefab.transform.rotation);
                emojiInstance.transform.SetParent(FindAnyObjectByType<UIManager>().transform);
                emojiInstance.GetComponentInChildren<Animator>().Play(emojiAnimation);

                // Update emoji username
                emojiInstance.GetComponentInChildren<TextMeshProUGUI>().text = _userName;

                StartCoroutine(NetworkDestroy(emojiInstance));
            }
        }     
    }

    IEnumerator NetworkDestroy(GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(emojiLifeTime);

        if (objectToDestroy.GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Destroy(objectToDestroy);
        }
    }

    IEnumerator EmojiCooldown()
    {
        yield return new WaitForSeconds(emojiLifeTime);
        canSpawnEmoji = true;
    }
}
