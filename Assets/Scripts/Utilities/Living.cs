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

public class Living : MonoBehaviour, IDamagable {

	[Header ("Living Entity")]
	public float startingHealth;

	[Tooltip ("If the entity has a healthbar")]
	public Image healthBar;

	[Tooltip ("For flashing the material red when hit"), Header ("Only use one")]
	public MeshRenderer meshRenderer;
	public SkinnedMeshRenderer skinnedMeshRenderer;
	public Color hurtColor = new Color (103 / 244, 0, 0, 1);

	private bool flashing;
	private float endFlashingTime;
	private Material material;
	private Color originalColor;
	private Color currentColor;

	protected float health;
	private bool _dead = false;

	public bool dead {
		get {
			return _dead;
		}
		protected set {
			_dead = value;
		}
	}

	protected virtual void Start () {
		SetDefaults ();
	}

	private void LateUpdate () {
		if (flashing) {
			if (Time.time < endFlashingTime) {
				currentColor = Color.Lerp (originalColor, hurtColor, (Mathf.Sin (Time.time * 20f) + 1f) / 2f);
				material.color = currentColor;
			} else {
				flashing = false;
				material.color = currentColor = originalColor;
			}
		}
	}

	public virtual void SetDefaults () {
		health = startingHealth;
		dead = false;
		if (meshRenderer != null) {
			material = meshRenderer.material;
		} else if (skinnedMeshRenderer != null) {
			material = skinnedMeshRenderer.material;
		}
		if (material != null) {
			originalColor = material.color;
		}
	}

	public void TakeDamage (float damage) {
		if (!dead) {
			health -= damage;
			if (healthBar != null) {
				healthBar.fillAmount = health / startingHealth;
			}

			if (material != null) {
				flashing = true;
				endFlashingTime = Time.time + 0.25f;
			}

			if (health <= 0) {
				Die ();
			}
		}
	}

	protected virtual void Die () {
		dead = true;
	}
}