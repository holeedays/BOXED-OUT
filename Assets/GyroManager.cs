using System.Linq.Expressions;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;
using AttitudeSensor = UnityEngine.InputSystem.AttitudeSensor;

public class GyroManager : MonoBehaviour
{
    /// <summary>
    /// Before you get to actually doing anything this is the workflow to get your iphone gyroscope working on windows:
    /// 
    /// INSTRUCTIONS (The Setup)
    /// ----------------------------------------
    /// 1. Download iTunes, though since it is defunct. Download apple devices (especially for Windows 11 pcs). Both apps can be found in the microsoft store.
    /// 2. Be sure to set those up so that they rocognize your app.
    /// 3. Probably restart your computer afterwards so everything initializes.
    /// 3. After that go to (in your Unity project): Project Settings -> Editor -> Devices (under Remote Play) and change the device to your ios device OR any ios device
    /// 4. To make sure it works. Make sure Apple Devices (or iTunes though I haven't tested iTunes) is turned on and detects your phone. If it still doesn't appear, unplug and replug.
    /// 5. Install Unity Remote 5 from the ios store.
    /// 6. So the tricky part is getting Remote 5 to execute. I would maybe suggest restarting your phone just in case.
    /// 7. The method of successfully executing the app is to open the app when you're loading the game after pressing play.
    /// 8. If your phone fell asleep, probably refresh the app before setting it up again.
    /// 9. If you did it successfully, the app on your phone should now be streaming your game after it successfully compiled to playmode.
    /// 
    /// 
    /// BENEFITS (The Benefits)
    /// ----------------------------------------
    ///     You can access the new UnityEngine.InputSystem module which allows you to access mobile features 
    ///     like accelerometers, touchscreen, attitude sensors (gyroscopes).
    ///     
    ///     It seems to work over using the legacy Input module, which seems to be finnicky when interacting with features like Input.gyro (which is not null but doesn't return my phone
    ///     gyro when I connect it so idk why tf it's doing that)
    ///     
    /// ADDITIONAL NOTES (Miscellaneous Bugs and Crap)
    /// ----------------------------------------
    /// By default, all inputdevices are disabled by default so you have to enable them. HOWEVER, the devices take a small amount of time to be found so you have to check
    /// if the device you're looking for is not null. After enabling the device, the device disables shortly after for some reason, you can check this by trying to read the 
    /// value of a sensor. If it debugs to a default value, then it probably means the device is off again. ENABLE IT AGAIN in update or something.
    /// 
    /// </summary>

    #region Init The Script
    public static GyroManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject().AddComponent<GyroManager>();

            return instance;
        }
        set
        {
            ;
        }
    }
    private static GyroManager instance;
    #endregion Init The Script

    #region Other Misc Vars
    private bool gyroEnabled;
    #endregion Other Misc Vars

    // activate gyroscope
    public async void EnableGyroscope()
    {
        // waiting for a viable device (if any)
        await AwaitGyroscopeDetection();

        // then enable that device 
        InputSystem.EnableDevice(AttitudeSensor.current);
        //InputSystem.EnableDevice(Gyroscope.current);
        gyroEnabled = true;
        Debug.Log("Gyroscope ready to use");
    }

    // first check if the gyroscope exists
    public async Task AwaitGyroscopeDetection()
    {
        Debug.Log("Awaiting connection...");

        while (AttitudeSensor.current == null)
        {
            await Task.Yield();;
        }

        Debug.Log("Gyroscope connected!");
    }

    private void Update()
    {
        // this is extremely strange but it seems that after initially checking if the sensor is enabled and enabling it; it turns off again
        // though the device is not null, but the device is deactivated
        if (gyroEnabled && !AttitudeSensor.current.enabled)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
            //InputSystem.EnableDevice(Gyroscope.current);
        }
    }


    public Quaternion GetGyroRot()
    {
        if (gyroEnabled)
        {
            // Note attitude sensor is a quaternion version of the default gyroscope, which is a Vector3 variant

            Debug.Log(AttitudeSensor.current.attitude.ReadValue());
            //Debug.Log(Gyroscope.current.angularVelocity.ReadValue());
            return AttitudeSensor.current.attitude.ReadValue();
        }
        else
            return Quaternion.identity;
    }
}

