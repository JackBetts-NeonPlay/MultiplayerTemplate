using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PooledParticle : PooledObjectBase
    {
        private ParticleSystem _particle; 

        private void OnEnable() 
        {
            _particle.Play(); 
            StartCoroutine(CoRecycleAfterTime()); 
        }

        private void Awake() 
        {
            _particle = GetComponentInChildren<ParticleSystem>(); 
        }

        IEnumerator CoRecycleAfterTime()
        {
            yield return new WaitForSeconds(_particle.main.duration); 
            Recycle(); 
        }
    }
}