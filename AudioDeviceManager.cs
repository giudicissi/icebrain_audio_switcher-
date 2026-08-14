using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;

namespace AudioSwitcherApp
{
    [Guid("f8679f50-850a-41cf-9c72-430f290290c8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPolicyConfig
    {
        [PreserveSig] int GetMixFormat(string pszDeviceName, IntPtr ppFormat);
        [PreserveSig] int GetDeviceFormat(string pszDeviceName, bool bDefault, IntPtr ppFormat);
        [PreserveSig] int ResetDeviceFormat(string pszDeviceName);
        [PreserveSig] int SetDeviceFormat(string pszDeviceName, IntPtr pEndpointFormat, IntPtr MixFormat);
        [PreserveSig] int GetProcessingPeriod(string pszDeviceName, bool bDefault, IntPtr pmftDefaultPeriod, IntPtr pmftMinimumPeriod);
        [PreserveSig] int SetProcessingPeriod(string pszDeviceName, IntPtr pmftPeriod);
        [PreserveSig] int GetShareMode(string pszDeviceName, IntPtr pMode);
        [PreserveSig] int SetShareMode(string pszDeviceName, IntPtr mode);
        [PreserveSig] int GetPropertyValue(string pszDeviceName, bool bFxStore, IntPtr key, IntPtr pv);
        [PreserveSig] int SetPropertyValue(string pszDeviceName, bool bFxStore, IntPtr key, IntPtr pv);
        [PreserveSig] int SetDefaultEndpoint(string pszDeviceName, Role role);
        [PreserveSig] int SetEndpointVisibility(string pszDeviceName, bool bVisible);
    }

    [ComImport, Guid("870af99c-171d-4f9e-af0d-e63df40c2bc9")]
    internal class PolicyConfigClient
    {
    }

    public class AudioDeviceManager
    {
        private readonly MMDeviceEnumerator enumerator;

        public AudioDeviceManager()
        {
            enumerator = new MMDeviceEnumerator();
        }

        public List<MMDevice> GetDevices(DataFlow flow)
        {
            var list = new List<MMDevice>();

            try
            {
                var collection = enumerator.EnumerateAudioEndPoints(flow, DeviceState.Active);
                foreach (var device in collection)
                {
                    list.Add(device);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao listar dispositivos: " + ex.Message);
            }

            return list;
        }

        public bool SetDefaultDevice(MMDevice device)
        {
            if (device == null)
            {
                return false;
            }

            try
            {
                var policyConfig = (IPolicyConfig)new PolicyConfigClient();

                // Define o mesmo dispositivo para o padrao geral e para comunicacao.
                policyConfig.SetDefaultEndpoint(device.ID, Role.Console);
                policyConfig.SetDefaultEndpoint(device.ID, Role.Multimedia);
                policyConfig.SetDefaultEndpoint(device.ID, Role.Communications);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao definir dispositivo padrao: " + ex.Message);
                return false;
            }
        }

        public void SetVolume(MMDevice device, int volumePercent)
        {
            if (device == null) return;

            try
            {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = volumePercent / 100f;
            }
            catch
            {
            }
        }

        public MMDevice? FindDevice(DataFlow flow, string? id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            try
            {
                return enumerator.GetDevice(id);
            }
            catch
            {
                return null;
            }
        }

        public MMDevice? FindDeviceByName(DataFlow flow, string name)
        {
            foreach (var device in GetDevices(flow))
            {
                if (device.FriendlyName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    device.FriendlyName.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    return device;
                }
            }

            return null;
        }
    }
}
