﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SanAndreasUnity.Utilities
{

	public class DamageInfo
	{
		public float amount = 0f;
		public Transform raycastHitTransform = null;
		public object attacker = null;
		public object data = null;
	}

	public class Damageable : MonoBehaviour
	{

		[SerializeField] private float m_health = 0f;
		public float Health { get { return m_health; } set { m_health = value; } }

		[SerializeField] private UnityEvent m_onDamage = new UnityEvent ();
		public UnityEvent OnDamageEvent => m_onDamage;

		public DamageInfo LastDamageInfo { get; private set; }



		public void Damage (DamageInfo info)
		{
			this.LastDamageInfo = info;

			F.RunExceptionSafe(() => m_onDamage.Invoke());
		}

		public void HandleDamageByDefault ()
		{
			DamageInfo info = this.LastDamageInfo;

			this.Health -= info.amount;

			if (this.Health <= 0f) {
				Destroy (this.gameObject);
			}
		}

		public static void InflictDamageToObjectsInArea(Vector3 center, float radius, float damageAmount)
		{
			Collider[] colliders = Physics.OverlapSphere(center, radius);

			var damagables = new HashSet<Damageable>();

			foreach (var collider in colliders)
			{
				var damagable = collider.GetComponentInParent<Damageable>();
				if (damagable != null && !damagables.Contains(damagable))
				{
					damagables.Add(damagable);
				}
			}

			foreach (var damageable in damagables)
			{
				//Collider collider = pair.Value;

				//Vector3 closestPointOnCollider = collider.ClosestPoint(center);
				//float distanceToPointOnCollider = Vector3.Distance(center, closestPointOnCollider);
				//float distanceToCollider = Vector3.Distance(center, collider.transform.position);

				float distance = Vector3.Distance(center, damageable.transform.position);
				float distanceFactor = 1.0f - Mathf.Clamp01(distance / radius);
				float damageAmountBasedOnDistance = damageAmount * distanceFactor;

				F.RunExceptionSafe(() => damageable.Damage(new DamageInfo() { amount = damageAmountBasedOnDistance }));
			}

		}

	}

}
