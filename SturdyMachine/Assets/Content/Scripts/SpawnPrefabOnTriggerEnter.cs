using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabOnTriggerEnter : MonoBehaviour
{
	[SerializeField]
	GameObject _fusionWeaponImpact;

	Vector3 _contactPosition;

    void OnCollisionEnter(Collision pCollision)
    {
        if (_contactPosition != pCollision.GetContact(0).point)
        {
			_contactPosition = transform.InverseTransformPoint(pCollision.transform.position);

			_fusionWeaponImpact.transform.localPosition =  _contactPosition;

			if (!_fusionWeaponImpact.activeSelf)
			{
				_fusionWeaponImpact.SetActive(true);
				_fusionWeaponImpact.GetComponent<ParticleSystem>().Play();
			}
		}
    }

    void OnCollisionExit(Collision pCollision)
    {
		if (_fusionWeaponImpact.transform.position != Vector3.zero)
		{
			_fusionWeaponImpact.transform.position = Vector3.zero;

			_contactPosition = _fusionWeaponImpact.transform.position;

			if (_fusionWeaponImpact.activeSelf)
				_fusionWeaponImpact.SetActive(false);
		}
	}
}