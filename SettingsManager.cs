using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AudioSwitcherApp
{
    public class AppSettings
    {
        public Profile SettingsProfile1 { get; set; } = new Profile
        {
            Name = "oculos VR",
            ColorHex = "#400080"
        };

        public Profile SettingsProfile2 { get; set; } = new Profile
        {
            Name = "pc padraozimmm",
            ColorHex = "#0080C0"
        };

        public HotkeyConfig GlobalToggleHotkey { get; set; } = new HotkeyConfig();
        public bool StartWithWindows { get; set; }
        public bool StartMinimized { get; set; }
        public bool TransparentPanelsMode { get; set; }
        public int LastActiveProfileIndex { get; set; } = 1;
        public string Language { get; set; } = "pt-BR";

        public void Normalize()
        {
            SettingsProfile1 ??= new Profile();
            SettingsProfile2 ??= new Profile();
            GlobalToggleHotkey ??= new HotkeyConfig();

            SettingsProfile1.Normalize("oculos VR", "#400080");
            SettingsProfile2.Normalize("pc padraozimmm", "#0080C0");

            if (LastActiveProfileIndex is < 1 or > 2)
            {
                LastActiveProfileIndex = 1;
            }

            if (Language != "en-US")
            {
                Language = "pt-BR";
            }
        }
    }

    public class Profile
    {
        public string Name { get; set; } = "";
        public string InputDeviceId { get; set; } = "";
        public string OutputDeviceId { get; set; } = "";
        public string ColorHex { get; set; } = "#333333";
        public HotkeyConfig Hotkey { get; set; } = new HotkeyConfig();
        public int InputVolume { get; set; } = 100;
        public int OutputVolume { get; set; } = 100;
        public bool ApplyInputVolume { get; set; } = true;
        public bool ApplyOutputVolume { get; set; } = true;

        public void Normalize(string fallbackName, string fallbackColor)
        {
            Name = string.IsNullOrWhiteSpace(Name) ? fallbackName : Name.Trim();
            ColorHex = string.IsNullOrWhiteSpace(ColorHex) ? fallbackColor : ColorHex;
            InputDeviceId ??= "";
            OutputDeviceId ??= "";
            Hotkey ??= new HotkeyConfig();
            InputVolume = Math.Clamp(InputVolume, 0, 100);
            OutputVolume = Math.Clamp(OutputVolume, 0, 100);
        }
    }

    public class HotkeyConfig
    {
        public Keys KeyCode { get; set; } = Keys.None;
        public bool Alt { get; set; }
        public bool Ctrl { get; set; }
        public bool Shift { get; set; }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool isEnglish)
        {
            if (KeyCode == Keys.None)
            {
                return isEnglish ? "None" : "Nenhum";
            }

            var parts = new List<string>();
            if (Ctrl) parts.Add("Ctrl");
            if (Alt) parts.Add("Alt");
            if (Shift) parts.Add("Shift");
            parts.Add(KeyCode.ToString());
            return string.Join(" + ", parts);
        }

        public int GetWin32Modifiers()
        {
            int mod = Constants.NOMOD;
            if (Alt) mod |= Constants.ALT;
            if (Ctrl) mod |= Constants.CTRL;
            if (Shift) mod |= Constants.SHIFT;
            return mod;
        }
    }

    public static class SettingsManager
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static AppSettings Load()
        {
            AppSettings loadedSettings;

            if (File.Exists(FilePath))
            {
                try
                {
                    string json = File.ReadAllText(FilePath);
                    loadedSettings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                }
                catch
                {
                    loadedSettings = new AppSettings();
                }
            }
            else
            {
                loadedSettings = new AppSettings();
            }

            loadedSettings.Normalize();
            return loadedSettings;
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                settings.Normalize();
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar config: " + ex.Message);
            }
        }
    }
}
