using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
/// <summary>
/// Script handling player health. Not fully implemented.
/// </summary>
public class PlayerHealth : MonoBehaviour {

	public int startingHealth = 100;
	public int currentHealth;
	public Slider healthSlider;
	public Image damageImage;
	public AudioClip deathClip;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

	Animator anim;
	AudioSource playerAudio;
	PlayerMovement playerMovement;
	//PlayerShooting playerShooting;
	bool isDead;
	bool damaged;


    /// <summary>
    /// Awake method. Initializes the player's health.
    /// </summary>
    void Awake ()
	{
		anim = GetComponent <Animator> ();
		playerAudio = GetComponent <AudioSource> ();
		playerMovement = GetComponent <PlayerMovement> ();
		//playerShooting = GetComponentInChildren <PlayerShooting> ();
		currentHealth = startingHealth;
	}
    /// <summary>
    /// Update method. Updates the UI when the player is damaged.
    /// </summary>
	void Update ()
	{
		if(damaged)
		{
			//damageImage.color = flashColour;
		}
		else
		{
			//damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;
	}
    /// <summary>
    /// Applies damage to the player and checks if the player is dead.
    /// </summary>
    /// <param name="amount"></param>
	public void TakeDamage (int amount)
	{
		damaged = true;

		currentHealth -= amount;

		//healthSlider.value = currentHealth;

		playerAudio.Play ();

		if(currentHealth <= 0 && !isDead)
		{
			Death ();
		}
	}
    /// <summary>
    /// Plays the death animation and disables the player's movement.
    /// </summary>
	void Death ()
	{
		isDead = true;

		//playerShooting.DisableEffects ();

		anim.SetTrigger ("Die");

		playerAudio.clip = deathClip;
		playerAudio.Play ();

		//playerMovement.enabled = false;
		//playerShooting.enabled = false;
	}
}
