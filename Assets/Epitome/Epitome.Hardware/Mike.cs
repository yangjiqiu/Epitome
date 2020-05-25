using UnityEngine;

namespace Epitome.Hardware
{
    public static class Mike
    {
        public static void OpenMicrophone(AudioSource audioSource, int duration = 60, bool loop = false, int frequency= 44100)
        {
            string[] tempDevices = Microphone.devices;

            CloseMicrophone();

            if (tempDevices.Length != 0) { audioSource.clip = Microphone.Start(null, loop, duration, frequency); }
        }

        public static void CloseMicrophone()
        {
            Microphone.End(null);
        }
    }
}
