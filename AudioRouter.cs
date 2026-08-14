using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AudioSwitcherApp
{
    /// <summary>
    /// Roteamento de áudio via WasapiLoopbackCapture.
    /// Agora usa o MMDeviceEnumerator padrão do NAudio (sem conflitos).
    /// </summary>
    public class AudioRouter
    {
        private WasapiLoopbackCapture? capture;
        private WasapiOut?             playback;
        private BufferedWaveProvider?  provider;
        
        public static string LastError { get; set; } = "";
        public bool IsRunning { get; private set; } = false;

        public void StartRouting(string cableInputFullName, string physicalOutputFullName)
        {
            StopRouting();
            LastError = "";

            try
            {
                var enumerator = new MMDeviceEnumerator();
                
                MMDevice? cableIn = FindDevice(enumerator, cableInputFullName, DataFlow.Render);
                MMDevice? physicalOut = FindDevice(enumerator, physicalOutputFullName, DataFlow.Render);

                if (cableIn == null) { LastError = $"CABLE ({cableInputFullName}) não encontrado."; return; }
                if (physicalOut == null) { LastError = $"Fone/Caixa ({physicalOutputFullName}) não encontrado."; return; }

                // 1. Inicia Captura Loopback do CABLE
                capture = new WasapiLoopbackCapture(cableIn);
                
                provider = new BufferedWaveProvider(capture.WaveFormat)
                {
                    DiscardOnBufferOverflow = true,
                    BufferDuration = TimeSpan.FromMilliseconds(200)
                };

                capture.DataAvailable += (s, a) =>
                {
                    provider?.AddSamples(a.Buffer, 0, a.BytesRecorded);
                };

                // 2. Cadeia de Conversão: Float -> Stereo -> 48kHz
                var sourceSampleProvider = provider.ToSampleProvider();
                
                ISampleProvider stereoProvider;
                if (capture.WaveFormat.Channels > 2)
                    stereoProvider = new MixingSampleProvider(new[] { sourceSampleProvider });
                else if (capture.WaveFormat.Channels == 1)
                    stereoProvider = new MonoToStereoSampleProvider(sourceSampleProvider);
                else
                    stereoProvider = sourceSampleProvider;

                var resampler = new WdlResamplingSampleProvider(stereoProvider, 48000);

                // 3. Inicia Saída
                playback = new WasapiOut(physicalOut, AudioClientShareMode.Shared, true, 50);
                playback.Init(resampler);

                capture.StartRecording();
                playback.Play();
                IsRunning = true;
            }
            catch (Exception ex)
            {
                LastError = $"Erro router: {ex.Message}";
                StopRouting();
            }
        }

        private MMDevice? FindDevice(MMDeviceEnumerator enumerator, string name, DataFlow flow)
        {
            var collection = enumerator.EnumerateAudioEndPoints(flow, DeviceState.Active);
            foreach (var d in collection)
            {
                if (d.FriendlyName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    d.FriendlyName.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    return d;
                }
            }
            return null;
        }

        public void StopRouting()
        {
            IsRunning = false;
            try { capture?.StopRecording(); capture?.Dispose(); capture = null; } catch { }
            try { playback?.Stop(); playback?.Dispose(); playback = null; } catch { }
            provider = null;
        }

        public string GetDebugStatus()
        {
            if (!IsRunning) return $"Status: Parado. Erro: {LastError}";
            return $"Status: RODANDO. {capture?.WaveFormat.SampleRate}Hz/{capture?.WaveFormat.Channels}ch → 48000Hz/2ch.";
        }
    }
}
