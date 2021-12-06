using UnityEngine;

namespace Epitome.Hardware
{
    public static class Mike
    {
        public static void OpenMicrophone(AudioSource audioSource,string deviceName = null, int duration = 60, bool loop = false, int frequency= 44100)
        {
            string[] tempDevices = Microphone.devices;

            CloseMicrophone();

            if (tempDevices.Length != 0) { audioSource.clip = Microphone.Start(deviceName, loop, duration, frequency); }
            else { Debug.Log("没有找到录音设备"); }
        }

        public static void CloseMicrophone()
        {
            Microphone.End(null);
        }
    }
}
