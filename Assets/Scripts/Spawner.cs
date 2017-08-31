using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Button spawnButton;

    [SerializeField]
    private Slider speedSlider;

    [SerializeField]
    private Text speedSliderText;

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private Transform spawnTransform;

    private void Awake()
    {
        spawnButton.onClick.AddListener(OnSpawn);
        speedSlider.onValueChanged.AddListener(OnSpeedSliderValueChanged);
    }

    private void OnSpeedSliderValueChanged(float newSpeed)
    {
        speedSliderText.text = "Velocity: " + newSpeed.ToString("00.00");
    }

    private void OnSpawn()
    {
        GameObject newSpawnedObject = Instantiate(prefab, spawnTransform.position, Quaternion.identity);

        newSpawnedObject.GetComponent<Rigidbody>().velocity = spawnTransform.forward * speedSlider.value;
    }
}
