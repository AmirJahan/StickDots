using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emoji : MonoBehaviour
{
    [SerializeField] private float moveUpSpeed = 10f;
    
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * moveUpSpeed);
    }
}
