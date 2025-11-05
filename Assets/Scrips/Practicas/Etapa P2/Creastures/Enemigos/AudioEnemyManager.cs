using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnemyManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void DamageSound(AudioClip _damageSound)
    {
        if (audioSource != null && _damageSound != null)
            audioSource.PlayOneShot(_damageSound);
    }
}
