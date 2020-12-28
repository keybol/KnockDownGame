using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KEntity : MonoBehaviour
{
	[SerializeField] KinematicCharacterMotor Motor;
	private Vector3 spawnPoint;
	private Quaternion spawnRotation;

	private void Awake()
	{
		spawnPoint = Motor.TransientPosition;
		spawnRotation = Motor.TransientRotation;
	}

	private void Update()
	{
		if (transform.position.y < -5)
			Motor.SetPositionAndRotation(spawnPoint, spawnRotation);
	}
}
