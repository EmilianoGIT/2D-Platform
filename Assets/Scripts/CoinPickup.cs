using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int points = 100;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        /*
         * I perform this check because given that my player has
         * 2 colliders: 
         * - CapsuleCollider for the body
         * - BoxCollider for the feet
         * I can sometimes collide 2 times and that make so that my score is increased 2 times..
         */

        if (!(collider is CapsuleCollider2D)) return;
        FindObjectOfType<GameSession>().AddToScore(points);
        AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
        Destroy(gameObject);

    }
}
