using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ForsakenLegacy
{
    public class Ability
    {
        public AbilityType type;
        public bool unlocked;

        // Method to unlock the ability
        public void UnlockAbility()
        {
            unlocked = true;
            Debug.Log("Unlocked ability: " + type);
            
            // Invoke the event when the ability is unlocked
            // OnAbilityUnlocked?.Invoke(type);
        }

        // Method to check if the ability is unlocked
        public bool IsUnlocked()
        {
            return unlocked;
        }
    }

    public class StunAbility : MonoBehaviour
    {
        public AbilityType type = AbilityType.Stun;
        public float stunDuration = 3f;
        public float stunRadius = 5f;

        private LayerMask enemyLayer;

        private PlayerInput _playerInput;
        private InputAction stunAction;

        public float stunCooldown = 7f;
        public Image stunCooldownImage; 
        private bool canstun = true;

        public MMFeedbacks stunFeedback;
    
        private void Start()
        {
            // Set the layer mask to the enemy layer
            enemyLayer = LayerMask.GetMask("Enemy");

            // Initialize the input system to check for the key
            _playerInput = GetComponent<PlayerInput>();
            stunAction = _playerInput.actions.FindAction("Stun");
            stunAction.performed += OnStunPerformed;
        }

        private void OnStunPerformed(InputAction.CallbackContext context)
        {
            // Ability stunAbility = AbilityManager.Instance.GetAbilityByType(type);

            // if (stunAbility != null && stunAbility.IsUnlocked())
            // {
                bool isAttacking = GetComponent<AttackMelee>().isAttacking;

                if (!GetComponent<PlayerController>().isInAbility && !isAttacking && canstun)
                {
                    StartCoroutine("PerformStun");
                }
            // }
        }


        private IEnumerator PerformStun()
        {
            canstun = false;
            GetComponent<PlayerController>().isInAbility = true;

            // Play the stun feedback
            stunFeedback.PlayFeedbacks();

            yield return new WaitForSeconds(1);

            // Get all colliders in the stun radius that belong to the enemy layer
            Collider[] colliders = Physics.OverlapSphere(transform.position, stunRadius, enemyLayer);

            // Apply the stun effect to each enemy
            foreach (Collider collider in colliders)
            {
                Stunnable stunnable = collider.GetComponent<Stunnable>();
                if (stunnable != null)
                {
                    stunnable.Stun(stunDuration);
                }
            }

            GetComponent<PlayerController>().isInAbility = false;
            StartCoroutine(StunCooldown());
        }

        private IEnumerator StunCooldown()
        {
            float elapsedTime = 0f;
    
            while (elapsedTime < stunCooldown)
            {
                // Calculate the fill amount based on the remaining cooldown time
                float fillAmount = 1 - (elapsedTime / stunCooldown);
                // Update the UI image's fill amount
                stunCooldownImage.fillAmount = fillAmount;
                // Increment the elapsed time
                elapsedTime += Time.deltaTime;
                // Wait for the next frame
                yield return null;
            }
            // Allow stunning again after the cooldown
            canstun = true;
        }
    }
}

