using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public event Action OnDeath;
    public int health;
    VerticalLayoutGroup layout;
    private List<GameObject> healthContainers = new List<GameObject>();
    [SerializeField] private GameObject HeathContainer;

    private void Awake()
    {
        layout = GetComponent<VerticalLayoutGroup>();
        for (int i = 0; i < health; i++)
        {
            healthContainers.Add(Instantiate(HeathContainer, transform));
        }
    }

    public void TakeDamage()
    {
        layout.enabled = false;
        health--;
        
        if (health <= 0)
        {
            OnDeath?.Invoke();
            health = 0;
        }
        healthContainers[health].SetActive(false);
    }
}
