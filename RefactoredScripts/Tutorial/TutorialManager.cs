using Sigtrap.VrTunnellingPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
    [SerializeField] 
    private TutorialLevel[] tutorials;

    [SerializeField]
    private TutorialLevel transitionTutorial;

    [SerializeField]
    private Transform teleportAnchor;

    [SerializeField]
    private Tunnelling tunnelingPro;

    private int tutorialIndex;
    private CharacterController characterController;
    private Transform playerTransform;

    private void Start() {
        GameObject player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        characterController = player.GetComponent<CharacterController>();

        tutorials[0].enabled = true;
        TutorialLevel.TutorialEndEvent += OnEndEvent;
    }

    private void OnEndEvent() {
        transitionTutorial.enabled = false;
        tutorials[tutorialIndex].enabled = false;

        tutorialIndex++;
        TeleportPlayer(teleportAnchor);
        
        // Default behaviour is just to swap between tutorials
        if (tutorialIndex < tutorials.Length) {
            tutorials[tutorialIndex].enabled = true;
        }
        
        // If the last tutorial is reached, it will enable tunneling 
        else if(tunnelingPro.enabled == false) {
            transitionTutorial.enabled = true;
            tunnelingPro.enabled = true;
            tutorialIndex = 0;
        }
        
        // If tunnelling tutorial has been done, it will launch the experiment
        else { 
            SceneManager.LoadScene("Scenes/TunnelingSnapping");
        }
    }
    
    /// <summary>
    /// Teleport the player to the given point
    /// BUG: characterController has to be disabled & enabled to let the player be teleported
    /// </summary>
    private void TeleportPlayer(Transform anchor) {
        characterController.enabled = false;
        playerTransform.position = anchor.position;
        playerTransform.rotation = anchor.rotation;
        characterController.enabled = true;
    }
}
