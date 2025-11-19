using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AcidDamage : MonoBehaviour
{
    public float damagePerTick = 5f;
    public float damageInterval = 1f;
    public float knockbackForce = 10f;

    public Slider healthSlider;

    private bool playerInside = false;
    private Transform player;
    private Rigidbody playerRb;

    private void Start()
    {
        if (healthSlider == null)
        {
            healthSlider = FindFirstObjectByType<Slider>();
            Debug.Log("Health slider auto-assigned: " + healthSlider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter hit: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered acid!");

            playerInside = true;
            player = other.transform;
            playerRb = other.GetComponent<Rigidbody>();

            StartCoroutine(DamageLoop());
        }
        else
        {
            Debug.Log("Object was not tagged Player");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Player left acid.");
        }
    }

    IEnumerator DamageLoop()
    {
        while (playerInside)
        {
            Debug.Log("Dealing damage tick!");

            if (healthSlider != null)
                healthSlider.value -= damagePerTick;
            else
                Debug.LogError("No health slider found!");

            if (playerRb != null)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                playerRb.AddForce(direction * knockbackForce, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }
}
