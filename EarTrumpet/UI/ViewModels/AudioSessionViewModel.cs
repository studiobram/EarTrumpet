using EarTrumpet.DataModel.Audio;
using EarTrumpet.DataModel.WindowsAudio.Internal;
using EarTrumpet.Extensions;
using EarTrumpet.UI.Helpers;
using System.Linq;
using System.Windows.Input;

namespace EarTrumpet.UI.ViewModels
{
    public class AudioSessionViewModel : BindableBase
    {
        private readonly IStreamWithVolumeControl _stream;

        public AudioSessionViewModel(IStreamWithVolumeControl stream)
        {
            _stream = stream;
            _stream.PropertyChanged += Stream_PropertyChanged;

            ToggleMute = new RelayCommand(() => IsMuted = !IsMuted);
        }

        ~AudioSessionViewModel()
        {
            _stream.PropertyChanged -= Stream_PropertyChanged;
        }

        private void Stream_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        public string Id => _stream.Id;
        public ICommand ToggleMute { get; } 
        public bool IsMuted
        {
            get => _stream.IsMuted;
            set => _stream.IsMuted = value;
        }
        public int Volume
        {
            get => _stream.Volume.ToVolumeInt();
            set
            {
                var volume = value / 100f;
                _stream.Volume = volume;

                if (volume.ToVolumeInt() < SyncVolumeLevel)
                {
                    SyncVolumeLevel = volume.ToVolumeInt();
                }
            }
        }
        public int SyncVolumeLevel
        {
            get
            {
                var volume = _stream.SyncVolumeLevel.ToVolumeInt();
                // Extra safety check
                if (volume > Volume)
                {
                    SyncVolumeLevel = Volume;
                }
                return volume;
            }
            set {
                var volume = value / 100f;

                if (volume.ToVolumeInt() > Volume)
                {
                    volume = Volume / 100f;
                }

                _stream.SyncVolumeLevel = volume;
                SyncSettings.SetSyncVolumeLevel(_stream, volume);

                if (_stream.GetType() == typeof(AudioDevice))
                {
                    foreach (var item in ((AudioDevice)_stream).Groups.Where(e => e.SyncVolume))
                    {
                        item.Volume = _stream.SyncVolumeLevel;
                    }
                }
            }
        }
        public virtual float PeakValue1 => _stream.PeakValue1;
        public virtual float PeakValue2 => _stream.PeakValue2;

        public virtual void UpdatePeakValueForeground()
        {
            RaisePropertyChanged(nameof(PeakValue1));
            RaisePropertyChanged(nameof(PeakValue2));
        }
    }
}
