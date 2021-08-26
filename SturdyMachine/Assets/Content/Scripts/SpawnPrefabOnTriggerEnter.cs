using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabOnTriggerEnter : MonoBehaviour
{
	public GameObject prefabToSpawn;
	public Transform targetTransform;

	public float waitTimeBetweenSpawns = 1f;

	public float killTimeOnTriggerExit = 1f;


	private bool _isPlayerVehiculeInTrigger;

	private Coroutine _playerInTriggerCoroutine;


	public void OnTriggerEnter(Collider other)
	{
			_isPlayerVehiculeInTrigger = true;
			
			if (_playerInTriggerCoroutine != null)
			{
				StopCoroutine(_playerInTriggerCoroutine);

			}

			_playerInTriggerCoroutine = StartCoroutine(PlayerInTriggerCoroutine());

		

	}

	public void OnTriggerExit(Collider other)
	{
			_isPlayerVehiculeInTrigger = false;
	}

	private IEnumerator PlayerInTriggerCoroutine()
	{
		List<GameObject> spawnedGOs = new List<GameObject>();
		while (_isPlayerVehiculeInTrigger)
		{
			if (prefabToSpawn != null)
			{
				Vector3 position = targetTransform != null ? targetTransform.position : this.transform.position;
				Quaternion rotation = targetTransform != null ? targetTransform.rotation : this.transform.rotation;
				GameObject go = GameObject.Instantiate(prefabToSpawn, position, rotation);
				spawnedGOs.Add(go);

			}

			yield return new WaitForSeconds(waitTimeBetweenSpawns);

		}

		yield return new WaitForSeconds(killTimeOnTriggerExit);

		foreach (GameObject go in spawnedGOs)
		{
			Destroy(go);

		}

		yield return null;

	}

}
