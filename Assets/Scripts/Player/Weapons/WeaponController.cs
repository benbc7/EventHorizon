using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType {
	SemiAutomatic, FullyAutomatic, ChargedShot, Spread, Grapple
}

public class WeaponController : MonoBehaviour {

	public Transform weaponHold;
	public Transform projectileSpawn;
	public Weapon startingWeapon;
	public Weapon startingSecondary;

	[Header ("UI")]
	public Animator HUDAnimator;
	public Image primaryCoolDownBar;
	public Image primaryIcon;
	public Image secondaryCoolDownBar;
	public Image secondaryIcon;

	private Weapon primaryWeapon;
	private Weapon secondaryWeapon;

	private bool paused;

	public delegate void ShootDelegate ();

	public event ShootDelegate EventShootPrimary;

	public event ShootDelegate EventShootSecondary;

	private void Start () {
		GameManager.instance.PauseEvent += Pause;
		if (startingWeapon != null) {
			EquipWeapon (startingWeapon, true);
		}
		if (startingSecondary != null) {
			EquipWeapon (startingSecondary, false);
		}
	}

	public void Pause () {
		paused = !paused;
	}

	public void EquipWeapon (Weapon weaponToEquip, bool primary) {
		if (primary) {
			if (primaryWeapon != null) {
				Destroy (primaryWeapon.gameObject);
			}
			primaryWeapon = Instantiate (weaponToEquip, weaponHold.position, weaponHold.rotation) as Weapon;
			primaryWeapon.transform.parent = weaponHold;
			primaryIcon.sprite = primaryWeapon.icon;
			AudioManager.instance.PlaySound ("EquipWeapon");
			primaryWeapon.Initialize (this, primaryCoolDownBar, true);
		} else {
			if (secondaryWeapon != null) {
				Destroy (secondaryWeapon.gameObject);
			}
			secondaryWeapon = Instantiate (weaponToEquip, weaponHold.position, weaponHold.rotation) as Weapon;
			secondaryWeapon.transform.parent = weaponHold;
			secondaryIcon.sprite = secondaryWeapon.icon;
			HUDAnimator.SetBool ("SecondaryEquipped", true);
			AudioManager.instance.PlaySound ("EquipWeapon");
			secondaryWeapon.Initialize (this, secondaryCoolDownBar, false);
		}
	}

	public void Shoot (bool primary) {
		if (!paused) {
			if (primary) {
				if (EventShootPrimary != null) {
					EventShootPrimary ();
				}
			} else {
				if (EventShootSecondary != null) {
					EventShootSecondary ();
				}
			}
		}
	}
}