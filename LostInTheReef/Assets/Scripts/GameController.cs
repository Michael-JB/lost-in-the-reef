using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum CurrentDirection {
    UP,
    DOWN,
}

public class GameController : MonoBehaviour {
    public GameObject[] mazeLevels;
    public Light2D playerLight;
    public GameObject artifactTile;

    public Sprite tick;
    public Sprite cross;
    public Image artifactImage;
    public Image gaugeNeedle;
    public Image compassNeedle;
    public Slider oxygenSlider;
    public TextMeshProUGUI interactionPrompt;

    public bool enableCheats = false;
    public float oxygenDrainRate = 3f;
    public float oxygenGainRate = 20f;
    public float gaugeNeedleRotationSpeed = 0.5f;
    public float compassNeedleRotationSpeed = 3f;
    public float lightRadius = 7f;

    private int currentMaze = 0;
    private bool hasArtifact = false;
    private float oxygenPercentage = 100f;
    private bool hasShownCurrentPrompt = false;

    private bool onDownCurrent = false;
    private bool onUpCurrent = false;
    private bool onArtifact = false;
    private bool onStartingPoint = false;
    private bool onOxygen = false;

    // Start is called before the first frame update
    void Start() {
        UpdateArtifactImageSprite();
        HideInteractionPrompt();
        SetCurrentMazeActive();
        SetLightRadius();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        // Cheats
        if (enableCheats) {
            if (Input.GetKeyDown(KeyCode.O)) OnMazeChange(CurrentDirection.UP);
            if (Input.GetKeyDown(KeyCode.P)) OnMazeChange(CurrentDirection.DOWN);
        }

        // Interactions
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (onUpCurrent) OnMazeChange(CurrentDirection.UP);
            if (onDownCurrent) OnMazeChange(CurrentDirection.DOWN);
            if (onArtifact) OnArtifactPickUp();
            if (onStartingPoint) OnArtifactPutDown();
        }

        // Interaction prompts
        if (onUpCurrent) {
            if (!hasShownCurrentPrompt) {
                ShowInteractionPrompt("Press 'SPACE' to follow the upwards current");
            } else {
                ShowInteractionPrompt("Upwards current");
            }
        } else if (onDownCurrent) {
            if (!hasShownCurrentPrompt) {
                ShowInteractionPrompt("Press 'SPACE' to follow the downwards current");
            } else {
                ShowInteractionPrompt("Downwards current");
            }
        } else if (onArtifact && !hasArtifact) {
            ShowInteractionPrompt("Press 'SPACE' to pick up the artefact");
        } else if (onStartingPoint && hasArtifact) {
            ShowInteractionPrompt("Press 'SPACE' to put down the artefact");
        } else {
            ShowInteractionPrompt("");
        }

        // UI
        UpdateOxygenPercentage();
        UpdateGaugeNeedle();
        UpdateCompassNeedle();
    }

    private void UpdateGaugeNeedle() {
        Quaternion target = Quaternion.Euler(0, 0,  transform.rotation.eulerAngles.z + 55 - (55 * currentMaze));
        gaugeNeedle.transform.rotation = Quaternion.Slerp(gaugeNeedle.transform.rotation, target, Time.deltaTime * gaugeNeedleRotationSpeed);
    }

    private void UpdateCompassNeedle() {
        Quaternion target = Quaternion.Euler(0, 0, 180);
        compassNeedle.transform.rotation = Quaternion.Slerp(compassNeedle.transform.rotation, target, Time.deltaTime * compassNeedleRotationSpeed);
    }

    private void UpdateOxygenPercentage() {
        if (onOxygen) oxygenPercentage = Mathf.Min(oxygenPercentage + (oxygenGainRate * Time.deltaTime), 100);
        else oxygenPercentage = Mathf.Max(oxygenPercentage - (oxygenDrainRate * Time.deltaTime), 0);
        oxygenSlider.value = oxygenPercentage;

        if (oxygenPercentage == 0) LoseGame();
    }

    private void UpdateArtifactImageSprite() {
        artifactImage.sprite = hasArtifact ? tick : cross;
    }

    private void HideInteractionPrompt() {
        interactionPrompt.enabled = false;
    }

    private void ShowInteractionPrompt(string prompt) {
        interactionPrompt.text = prompt;
        interactionPrompt.enabled = true;
    }

    private void OnMazeChange(CurrentDirection current) {
        hasShownCurrentPrompt = true;
        HideInteractionPrompt();
        currentMaze = (currentMaze + (current == CurrentDirection.DOWN ? 1 : mazeLevels.Length - 1)) % mazeLevels.Length;
        SetCurrentMazeActive();
        SetLightRadius();
    }

    private void SetCurrentMazeActive() {
        for (int i = 0; i < mazeLevels.Length; i++) mazeLevels[i].SetActive(i == currentMaze);
    }

    private void SetLightRadius() {
        playerLight.pointLightOuterRadius = GetBrightnessForLevel();
    }

    private float GetBrightnessForLevel() {
        return lightRadius - currentMaze;
    }

    private void OnArtifactPickUp() {
        hasArtifact = true;
        HideInteractionPrompt();
        artifactTile.SetActive(false);
        UpdateArtifactImageSprite();
        SetLightRadius();
    }

    private void OnArtifactPutDown() {
        if (hasArtifact) WinGame();
    }

    private void WinGame() {
        SceneManager.LoadScene("VictoryScene");
    }

    private void LoseGame() {
        SceneManager.LoadScene("FailScene");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("DownCurrent")) onDownCurrent = true;
        if (other.CompareTag("UpCurrent")) onUpCurrent = true;
        if (other.CompareTag("Artifact")) onArtifact = true;
        if (other.CompareTag("StartingPoint")) onStartingPoint = true;
        if (other.CompareTag("Oxygen")) onOxygen = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("DownCurrent")) onDownCurrent = false;
        if (other.CompareTag("UpCurrent")) onUpCurrent = false;
        if (other.CompareTag("Artifact")) onArtifact = false;
        if (other.CompareTag("StartingPoint")) onStartingPoint = false;
        if (other.CompareTag("Oxygen")) onOxygen = false;
    }
}
