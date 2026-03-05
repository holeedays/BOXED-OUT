using UnityEngine;

public class Gyro : MonoBehaviour
{
    private void Start()
    {
        GyroManager.Instance.EnableGyroscope();
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.transform != null)
            this.transform.rotation = GyroManager.Instance.GetGyroRot();
    }
}
