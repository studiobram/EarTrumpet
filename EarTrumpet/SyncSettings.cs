using EarTrumpet.DataModel.Audio;
using EarTrumpet.DataModel.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrumpet
{
    internal static class SyncSettings
    {
        private static ISettingsBag _settings = StorageFactory.GetSettings();

        public static void SyncVolumeLevel(IAudioDevice device)
        {
            var syncVolumeLevel = _settings.Get($"SyncVolumeLevel|{device.Id}", default(float));
            if (syncVolumeLevel == default(float))
            {
                syncVolumeLevel = device.Volume;
                _settings.Set($"SyncVolumeLevel|{device.Id}", syncVolumeLevel);
            }

            device.SyncVolumeLevel = syncVolumeLevel;
        }

        public static void SetSyncVolumeLevel(IStreamWithVolumeControl device, float syncVolumeLevel)
        {
            _settings.Set($"SyncVolumeLevel|{device.Id}", syncVolumeLevel);
        }

        public static void SyncVolume(IAudioDeviceSession device)
        {
            device.SyncVolume = _settings.Get($"SyncVolume|{GetCleanAppItemId(device.Id)}", true);
            if (device.SyncVolume)
            {
                device.Volume = device.Parent.SyncVolumeLevel;
            }
        }

        public static void SetSyncVolume(IStreamWithVolumeControl device, bool syncVolume)
        {
            _settings.Set($"SyncVolume|{GetCleanAppItemId(device.Id)}", syncVolume);
        }

        private static string GetCleanAppItemId(string id)
        {
            int idx = id.LastIndexOf('|');

            if (idx != -1 && idx + 1 < id.Length)
            {
                return id.Substring(0, idx);
            }

            return id;
        }
    }
}
