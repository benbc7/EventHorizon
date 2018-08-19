/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {

	[Header ("Weapon")]
	public WeaponType weaponType;
	public Sprite icon;
	public Transform projectileSpawn;
	public Projectile projectile;

	[Range (1, 1000)]
	public float msBetweenShots = 100;
	public float spawnVelocity = 35;

	protected WeaponController weaponController;
	protected float coolDown;
	protected bool canShoot = true;
	protected float nextShotTime;
	protected Image coolDownBar;

	private void LateUpdate () {

		if (coolDown >= 100) {
			canShoot = false;
			coolDown = 100;
		}

		if (coolDown > 0) {
			coolDown -= Time.fixedDeltaTime * 10f;
		}

		if (coolDown < 0) {
			coolDown = 0;
			canShoot = true;
		}

		if (coolDownBar != null) {
			coolDownBar.fillAmount = coolDown / 100f;
		}
	}

	public void Initialize (WeaponController _weaponController, Image _coolDownBar, bool primary) {
		weaponController = _weaponController;
		projectileSpawn = weaponController.projectileSpawn;

		if (primary) {
			weaponController.EventShootPrimary += Shoot;
		} else {
			weaponController.EventShootSecondary += Shoot;
		}

		coolDownBar = _coolDownBar;
		coolDownBar.enabled = true;
		PoolManager.instance.CreatePool (projectile.gameObject, 30);
		PoolManager.instance.CreatePool (projectile.sparkParticle.gameObject, 15);
	}

	public virtual void Shoot () {
	}

	protected virtual void OnValidate () {
		if (spawnVelocity < 1f) {
			spawnVelocity = 1f;
		}
	}

	private void OnDisable () {
		weaponController.EventShootPrimary -= Shoot;
		if (coolDownBar != null) {
			coolDownBar.enabled = false;
		}
	}
}