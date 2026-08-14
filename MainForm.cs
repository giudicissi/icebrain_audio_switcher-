using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace AudioSwitcherApp
{
    public partial class MainForm : Form
    {
        private const int WmNchittest = 0x84;
        private const int WmNclButtonDown = 0xA1;
        private const int WmSizing = 0x214;
        private const int HtCaption = 0x2;
        private const int HtClient = 0x1;
        private const int HtLeft = 0xA;
        private const int HtRight = 0xB;
        private const int HtTop = 0xC;
        private const int HtTopLeft = 0xD;
        private const int HtTopRight = 0xE;
        private const int HtBottom = 0xF;
        private const int HtBottomLeft = 0x10;
        private const int HtBottomRight = 0x11;
        private const int ResizeBorder = 10;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private class ControlBoundsState
        {
            public Control Control { get; set; } = null!;
            public Rectangle BaseBounds { get; set; }
            public float BaseFontSize { get; set; }
        }

        private readonly List<ControlBoundsState> layoutStates = new();

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private AppSettings settings = new AppSettings();
        private readonly AudioDeviceManager audioManager;

        private List<MMDevice> inputDevices = new();
        private List<MMDevice> outputDevices = new();

        private GlobalHotkey? hotkeyProfile1;
        private GlobalHotkey? hotkeyProfile2;
        private GlobalHotkey? hotkeyGlobalToggle;

        private bool isRecording1;
        private bool isRecording2;
        private bool isRecordingGlobal;
        private bool isInitializingUi;
        private bool backgroundLoaded;

        private int activeProfileIndex = 1;
        private int currentToggleIndex;

        private readonly NotifyIcon trayIcon;
        private readonly ContextMenuStrip trayContextMenu;
        private System.Windows.Forms.Timer? saveTimer;

        public MainForm()
        {
            InitializeComponent();

            DoubleBuffered = true;
            audioManager = new AudioDeviceManager();

            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
            Resize += MainForm_Resize;

            trayContextMenu = new ContextMenuStrip
            {
                BackColor = Color.FromArgb(20, 24, 36),
                ForeColor = Color.White,
                ShowImageMargin = false
            };

            var itemOpen = new ToolStripMenuItem("Abrir App")
            {
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            itemOpen.Click += (s, e) => RestoreFromTray();

            var itemClose = new ToolStripMenuItem("Fechar App")
            {
                ForeColor = Color.White
            };
            itemClose.Click += (s, e) => Close();

            trayContextMenu.Items.Add(itemOpen);
            trayContextMenu.Items.Add(new ToolStripSeparator());
            trayContextMenu.Items.Add(itemClose);

            trayIcon = new NotifyIcon
            {
                Text = "ICEBRAIN SWITCHER",
                ContextMenuStrip = trayContextMenu
            };
            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            KeyPreview = true;
            KeyDown += MainForm_KeyDown;

            btnClose.Click += (s, e) => Close();
            btnMinimize.Click += (s, e) => WindowState = FormWindowState.Minimized;

            WireWindowDrag(this);

            btnColor1.Click += BtnColor1_Click;
            btnColor2.Click += BtnColor2_Click;

            btnRecord1.Click += (s, e) => ToggleRecording(1);
            btnRecord2.Click += (s, e) => ToggleRecording(2);
            btnRecordGlobal.Click += (s, e) => ToggleRecording(3);
            btnRefresh.Click += BtnRefresh_Click;

            btnActivate1.Click += (s, e) => ActivateProfile(settings.SettingsProfile1, 1);
            btnActivate2.Click += (s, e) => ActivateProfile(settings.SettingsProfile2, 2);

            txtName1.TextChanged += (s, e) =>
            {
                if (isInitializingUi) return;
                settings.SettingsProfile1.Name = txtName1.Text.Trim();
                SaveSettingsDelayed();
            };

            txtName2.TextChanged += (s, e) =>
            {
                if (isInitializingUi) return;
                settings.SettingsProfile2.Name = txtName2.Text.Trim();
                SaveSettingsDelayed();
            };

            cmbInput1.SelectedIndexChanged += CmbDevice_SelectedIndexChanged;
            cmbOutput1.SelectedIndexChanged += CmbDevice_SelectedIndexChanged;
            cmbInput2.SelectedIndexChanged += CmbDevice_SelectedIndexChanged;
            cmbOutput2.SelectedIndexChanged += CmbDevice_SelectedIndexChanged;

            trkInputVol1.ValueChanged += (s, e) => HandleVolumeChanged(settings.SettingsProfile1, trkInputVol1, lblInputVol1, true, 1);
            trkOutputVol1.ValueChanged += (s, e) => HandleVolumeChanged(settings.SettingsProfile1, trkOutputVol1, lblOutputVol1, false, 1);
            trkInputVol2.ValueChanged += (s, e) => HandleVolumeChanged(settings.SettingsProfile2, trkInputVol2, lblInputVol2, true, 2);
            trkOutputVol2.ValueChanged += (s, e) => HandleVolumeChanged(settings.SettingsProfile2, trkOutputVol2, lblOutputVol2, false, 2);

            chkApplyInputVol1.CheckedChanged += (s, e) => HandleApplyVolumeChanged(settings.SettingsProfile1, chkApplyInputVol1.Checked, true, 1);
            chkApplyOutputVol1.CheckedChanged += (s, e) => HandleApplyVolumeChanged(settings.SettingsProfile1, chkApplyOutputVol1.Checked, false, 1);
            chkApplyInputVol2.CheckedChanged += (s, e) => HandleApplyVolumeChanged(settings.SettingsProfile2, chkApplyInputVol2.Checked, true, 2);
            chkApplyOutputVol2.CheckedChanged += (s, e) => HandleApplyVolumeChanged(settings.SettingsProfile2, chkApplyOutputVol2.Checked, false, 2);

            chkStartWithWindows.CheckedChanged += ChkStartWithWindows_CheckedChanged;
            chkStartMinimized.CheckedChanged += ChkStartMinimized_CheckedChanged;
            chkTransparentPanels.CheckedChanged += ChkTransparentPanels_CheckedChanged;

            picFlagBR.Click += (s, e) => SetLanguage("pt-BR");
            picFlagUS.Click += (s, e) => SetLanguage("en-US");
            picFlagBR.Paint += PicFlagBR_Paint;
            picFlagUS.Paint += PicFlagUS_Paint;

            CaptureBaseLayout();
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            settings = SettingsManager.Load();
            activeProfileIndex = settings.LastActiveProfileIndex;
            currentToggleIndex = activeProfileIndex == 1 ? 0 : 1;

            LoadBackgroundArt();
            LoadLogoAndIcon();
            LoadFlagImages();
            LoadDevices();
            ApplySettingsToUI();
            RegisterAllHotkeys();
            UpdateActivateButtons();

            // Set comfortable default window size (75% of 880x1210 base)
            ClientSize = new Size(660, 908);
            LayoutProportionally();

            if (settings.StartMinimized)
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void CaptureBaseLayout()
        {
            layoutStates.Clear();
            RecordControlBounds(this);
        }

        private void RecordControlBounds(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                c.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                layoutStates.Add(new ControlBoundsState
                {
                    Control = c,
                    BaseBounds = c.Bounds,
                    BaseFontSize = c.Font.Size
                });

                if (c.HasChildren)
                {
                    RecordControlBounds(c);
                }
            }
        }

        private void LayoutProportionally()
        {
            if (layoutStates.Count == 0 || ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }

            float scaleX = (float)ClientSize.Width / 880f;
            float scaleY = (float)ClientSize.Height / 1210f;
            float scaleFont = Math.Min(scaleX, scaleY);

            SuspendLayout();
            foreach (var state in layoutStates)
            {
                int newX = (int)Math.Round(state.BaseBounds.X * scaleX);
                int newY = (int)Math.Round(state.BaseBounds.Y * scaleY);
                int newW = (int)Math.Round(state.BaseBounds.Width * scaleX);
                int newH = (int)Math.Round(state.BaseBounds.Height * scaleY);

                state.Control.SetBounds(newX, newY, newW, newH);

                if (state.BaseFontSize > 0)
                {
                    float newFontSize = Math.Max(6.5f, state.BaseFontSize * scaleFont);
                    if (Math.Abs(state.Control.Font.Size - newFontSize) > 0.2f)
                    {
                        try
                        {
                            state.Control.Font = new Font(state.Control.Font.FontFamily, newFontSize, state.Control.Font.Style);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            ResumeLayout(true);
        }

        private void WireWindowDrag(Control control)
        {
            control.MouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Left)
                {
                    return;
                }

                ReleaseCapture();
                SendMessage(Handle, WmNclButtonDown, HtCaption, 0);
            };
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                return;
            }

            LayoutProportionally();
        }

        private void TrayIcon_DoubleClick(object? sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void RestoreFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
            BringToFront();
        }

        private void LoadBackgroundArt()
        {
            try
            {
                string[] backgroundCandidates =
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bgggg.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app-background.png")
                };

                string? backgroundPath = backgroundCandidates.FirstOrDefault(File.Exists);
                if (string.IsNullOrWhiteSpace(backgroundPath))
                {
                    backgroundLoaded = false;
                    return;
                }

                BackgroundImage = Image.FromFile(backgroundPath);
                BackgroundImageLayout = ImageLayout.Stretch;
                backgroundLoaded = true;
            }
            catch
            {
                backgroundLoaded = false;
            }
        }

        private void LoadLogoAndIcon()
        {
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "iceheadset.ico");
                string imgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "iceheadset.png");

                if (File.Exists(iconPath))
                {
                    Icon appIcon = new Icon(iconPath);
                    Icon = appIcon;
                    trayIcon.Icon = appIcon;
                    trayIcon.Visible = true;
                }

                if (backgroundLoaded)
                {
                    picLogo.Visible = false;
                    btnMinimize.Text = "";
                    btnClose.Text = "";
                    btnMinimize.ForeColor = Color.Transparent;
                    btnClose.ForeColor = Color.Transparent;
                }
                else
                {
                    btnMinimize.Text = "─";
                    btnClose.Text = "✕";
                    btnMinimize.ForeColor = Color.White;
                    btnClose.ForeColor = Color.White;

                    if (File.Exists(imgPath))
                    {
                        picLogo.Image = Image.FromFile(imgPath);
                        picLogo.Visible = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void LoadFlagImages()
        {
            try
            {
                string[] brCandidates =
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "br.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "br.png"),
                    Path.GetFullPath("br.png")
                };

                string[] usCandidates =
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "us.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "us.png"),
                    Path.GetFullPath("us.png")
                };

                string? brPath = brCandidates.FirstOrDefault(File.Exists);
                if (!string.IsNullOrWhiteSpace(brPath))
                {
                    picFlagBR.Image = Image.FromFile(brPath);
                    picFlagBR.SizeMode = PictureBoxSizeMode.Zoom;
                }

                string? usPath = usCandidates.FirstOrDefault(File.Exists);
                if (!string.IsNullOrWhiteSpace(usPath))
                {
                    picFlagUS.Image = Image.FromFile(usPath);
                    picFlagUS.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch
            {
            }
        }

        private void SetLanguage(string lang)
        {
            settings.Language = lang;
            ApplyLanguageToUI();
            picFlagBR.Invalidate();
            picFlagUS.Invalidate();
            SaveSettingsDelayed();
        }

        private void PicFlagBR_Paint(object? sender, PaintEventArgs e)
        {
            if (settings.Language == "pt-BR")
            {
                using var pen = new Pen(Color.Gold, 3);
                e.Graphics.DrawRectangle(pen, 1, 1, picFlagBR.Width - 2, picFlagBR.Height - 2);
            }
        }

        private void PicFlagUS_Paint(object? sender, PaintEventArgs e)
        {
            if (settings.Language == "en-US")
            {
                using var pen = new Pen(Color.Gold, 3);
                e.Graphics.DrawRectangle(pen, 1, 1, picFlagUS.Width - 2, picFlagUS.Height - 2);
            }
        }

        private void UpdateTrayContextMenu()
        {
            bool isEn = settings.Language == "en-US";
            if (trayContextMenu.Items.Count >= 3)
            {
                trayContextMenu.Items[0].Text = isEn ? "Open App" : "Abrir App";
                trayContextMenu.Items[2].Text = isEn ? "Close App" : "Fechar App";
            }
        }

        private void ApplyLanguageToUI()
        {
            bool isEn = settings.Language == "en-US";

            lblInput1.Text = isEn ? "INPUT" : "ENTRADA";
            lblOutput1.Text = isEn ? "OUTPUT" : "SAIDA";
            lblHotkey1.Text = isEn ? "HOTKEY:" : "ATALHO:";

            lblInput2.Text = isEn ? "INPUT" : "ENTRADA";
            lblOutput2.Text = isEn ? "OUTPUT" : "SAIDA";
            lblHotkey2.Text = isEn ? "HOTKEY:" : "ATALHO:";

            lblGlobal.Text = isEn ? "GLOBAL TOGGLE HOTKEY" : "ATALHO TROCAR TUDO";
            lblRefreshHeader.Text = isEn ? "AUDIO DEVICES" : "DISPOSITIVOS DE ÁUDIO";

            chkStartWithWindows.Text = isEn ? "START WITH WINDOWS" : "INICIAR COM O WINDOWS";
            chkStartMinimized.Text = isEn ? "START MINIMIZED" : "INICIAR MINIMIZADO";
            chkTransparentPanels.Text = isEn ? "TRANSPARENT MODE" : "MODO TRANSPARENTE";
            lblLanguage.Text = isEn ? "LANGUAGE:" : "IDIOMA:";

            btnColor1.Text = isEn ? "COLOR" : "COR DO";
            btnColor2.Text = isEn ? "COLOR" : "COR DO";

            chkApplyInputVol1.Text = isEn ? "APPLY" : "APLICAR";
            chkApplyOutputVol1.Text = isEn ? "APPLY" : "APLICAR";
            chkApplyInputVol2.Text = isEn ? "APPLY" : "APLICAR";
            chkApplyOutputVol2.Text = isEn ? "APPLY" : "APLICAR";

            if (!isRecording1) btnRecord1.Text = isEn ? "RECORD" : "GRAVAR";
            else btnRecord1.Text = isEn ? "STOP" : "PARAR";

            if (!isRecording2) btnRecord2.Text = isEn ? "RECORD" : "GRAVAR";
            else btnRecord2.Text = isEn ? "STOP" : "PARAR";

            if (!isRecordingGlobal) btnRecordGlobal.Text = isEn ? "RECORD" : "GRAVAR";
            else btnRecordGlobal.Text = isEn ? "STOP" : "PARAR";

            if (btnRefresh.Enabled)
            {
                btnRefresh.Text = isEn ? "REFRESH" : "ATUALIZAR";
            }

            txtHotkey1.Text = settings.SettingsProfile1.Hotkey.ToString(isEn);
            txtHotkey2.Text = settings.SettingsProfile2.Hotkey.ToString(isEn);
            txtHotkeyGlobal.Text = settings.GlobalToggleHotkey.ToString(isEn);

            UpdateVolumeLabels();
            UpdateActivateButtons();
            UpdateTrayContextMenu();
        }

        private void LoadDevices()
        {
            try
            {
                string? selectedInput1 = (cmbInput1.SelectedItem as DeviceItem)?.Id ?? settings.SettingsProfile1.InputDeviceId;
                string? selectedOutput1 = (cmbOutput1.SelectedItem as DeviceItem)?.Id ?? settings.SettingsProfile1.OutputDeviceId;
                string? selectedInput2 = (cmbInput2.SelectedItem as DeviceItem)?.Id ?? settings.SettingsProfile2.InputDeviceId;
                string? selectedOutput2 = (cmbOutput2.SelectedItem as DeviceItem)?.Id ?? settings.SettingsProfile2.OutputDeviceId;

                inputDevices = audioManager.GetDevices(DataFlow.Capture).ToList();
                outputDevices = audioManager.GetDevices(DataFlow.Render).ToList();

                var inputItems = inputDevices.Select(d => new DeviceItem { Id = d.ID, Name = d.FriendlyName }).ToArray();
                var outputItems = outputDevices.Select(d => new DeviceItem { Id = d.ID, Name = d.FriendlyName }).ToArray();

                cmbInput1.Items.Clear();
                cmbInput2.Items.Clear();
                cmbOutput1.Items.Clear();
                cmbOutput2.Items.Clear();

                cmbInput1.Items.AddRange(inputItems);
                cmbInput2.Items.AddRange(inputItems);
                cmbOutput1.Items.AddRange(outputItems);
                cmbOutput2.Items.AddRange(outputItems);

                SelectComboBoxItem(cmbInput1, selectedInput1);
                SelectComboBoxItem(cmbOutput1, selectedOutput1);
                SelectComboBoxItem(cmbInput2, selectedInput2);
                SelectComboBoxItem(cmbOutput2, selectedOutput2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao listar dispositivos de audio: " + ex.Message);
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            bool isEn = settings.Language == "en-US";
            btnRefresh.Enabled = false;
            btnRefresh.Text = isEn ? "REFRESHING..." : "ATUALIZANDO...";
            Application.DoEvents();

            LoadDevices();

            btnRefresh.Text = isEn ? "REFRESHED!" : "ATUALIZADO!";
            var timer = new System.Windows.Forms.Timer { Interval = 1400 };
            timer.Tick += (s, ev) =>
            {
                btnRefresh.Text = isEn ? "REFRESH" : "ATUALIZAR";
                btnRefresh.Enabled = true;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ApplySettingsToUI()
        {
            isInitializingUi = true;
            settings.Normalize();

            txtName1.Text = settings.SettingsProfile1.Name;
            txtName2.Text = settings.SettingsProfile2.Name;

            SelectComboBoxItem(cmbInput1, settings.SettingsProfile1.InputDeviceId);
            SelectComboBoxItem(cmbOutput1, settings.SettingsProfile1.OutputDeviceId);
            SelectComboBoxItem(cmbInput2, settings.SettingsProfile2.InputDeviceId);
            SelectComboBoxItem(cmbOutput2, settings.SettingsProfile2.OutputDeviceId);

            chkStartWithWindows.Checked = settings.StartWithWindows;
            chkStartMinimized.Checked = settings.StartMinimized;
            chkTransparentPanels.Checked = settings.TransparentPanelsMode;

            trkInputVol1.Value = settings.SettingsProfile1.InputVolume;
            trkOutputVol1.Value = settings.SettingsProfile1.OutputVolume;
            trkInputVol2.Value = settings.SettingsProfile2.InputVolume;
            trkOutputVol2.Value = settings.SettingsProfile2.OutputVolume;

            chkApplyInputVol1.Checked = settings.SettingsProfile1.ApplyInputVolume;
            chkApplyOutputVol1.Checked = settings.SettingsProfile1.ApplyOutputVolume;
            chkApplyInputVol2.Checked = settings.SettingsProfile2.ApplyInputVolume;
            chkApplyOutputVol2.Checked = settings.SettingsProfile2.ApplyOutputVolume;

            ApplyLanguageToUI();
            UpdatePanelVisuals();
            isInitializingUi = false;
        }

        private void UpdatePanelVisuals()
        {
            if (settings.TransparentPanelsMode)
            {
                ApplyTransparentStyle(pnlProfile1);
                ApplyTransparentStyle(pnlProfile2);
                ApplyTransparentStyle(pnlGlobal, 105);
                ApplyTransparentStyle(pnlTopBar, 120);
                return;
            }

            ApplyColoredStyle(pnlProfile1, settings.SettingsProfile1.ColorHex);
            ApplyColoredStyle(pnlProfile2, settings.SettingsProfile2.ColorHex);
            pnlGlobal.BackColor = Color.FromArgb(165, 15, 19, 30);
            pnlGlobal.BorderColor = Color.FromArgb(170, 225, 237, 255);
            pnlTopBar.BackColor = Color.FromArgb(120, 8, 10, 16);
            pnlTopBar.BorderColor = Color.FromArgb(110, 220, 235, 255);
        }

        private static void ApplyTransparentStyle(GlassPanel panel, int alpha = 78)
        {
            panel.BackColor = Color.FromArgb(alpha, 10, 18, 30);
            panel.BorderColor = Color.FromArgb(185, 214, 237, 255);
        }

        private static void ApplyColoredStyle(GlassPanel panel, string colorHex)
        {
            try
            {
                Color baseColor = ColorTranslator.FromHtml(colorHex);
                panel.BackColor = Color.FromArgb(205, baseColor.R, baseColor.G, baseColor.B);
                panel.BorderColor = ControlPaint.Light(baseColor, 0.25f);
            }
            catch
            {
                panel.BackColor = Color.FromArgb(200, 60, 60, 60);
                panel.BorderColor = Color.FromArgb(210, 255, 255, 255);
            }
        }

        private void UpdateVolumeLabels()
        {
            lblInputVol1.Text = $"VOL ENTRADA: {trkInputVol1.Value}%";
            lblOutputVol1.Text = $"VOL SAIDA: {trkOutputVol1.Value}%";
            lblInputVol2.Text = $"VOL ENTRADA: {trkInputVol2.Value}%";
            lblOutputVol2.Text = $"VOL SAIDA: {trkOutputVol2.Value}%";
        }

        private void ChkStartWithWindows_CheckedChanged(object? sender, EventArgs e)
        {
            if (isInitializingUi) return;

            settings.StartWithWindows = chkStartWithWindows.Checked;
            SetStartupRegistry(settings.StartWithWindows);
            SaveSettingsDelayed();
        }

        private void ChkStartMinimized_CheckedChanged(object? sender, EventArgs e)
        {
            if (isInitializingUi) return;

            settings.StartMinimized = chkStartMinimized.Checked;
            SaveSettingsDelayed();
        }

        private void ChkTransparentPanels_CheckedChanged(object? sender, EventArgs e)
        {
            if (isInitializingUi) return;

            settings.TransparentPanelsMode = chkTransparentPanels.Checked;
            UpdatePanelVisuals();
            SaveSettingsDelayed();
        }

        private void SetStartupRegistry(bool enable)
        {
            try
            {
                using var rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (rk == null)
                {
                    return;
                }

                if (enable)
                {
                    rk.SetValue("IcebrainSwitcher", $"\"{Application.ExecutablePath}\"");
                }
                else
                {
                    rk.DeleteValue("IcebrainSwitcher", false);
                }
            }
            catch
            {
            }
        }

        private void SelectComboBoxItem(ComboBox comboBox, string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                comboBox.SelectedIndex = -1;
                return;
            }

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i] is DeviceItem item && item.Id == id)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }

            comboBox.SelectedIndex = -1;
        }

        private void RegisterAllHotkeys()
        {
            bool isEn = settings.Language == "en-US";
            hotkeyProfile1?.Dispose();
            hotkeyProfile2?.Dispose();
            hotkeyGlobalToggle?.Dispose();

            if (settings.SettingsProfile1.Hotkey.KeyCode != Keys.None)
            {
                hotkeyProfile1 = new GlobalHotkey(settings.SettingsProfile1.Hotkey.GetWin32Modifiers(), settings.SettingsProfile1.Hotkey.KeyCode, this, 1);
                if (!hotkeyProfile1.Register())
                {
                    MessageBox.Show(isEn ? "Warning: Failed to register Profile 1 hotkey." : "Aviso: Falha ao registrar atalho do Perfil 1.");
                }
            }

            if (settings.SettingsProfile2.Hotkey.KeyCode != Keys.None)
            {
                hotkeyProfile2 = new GlobalHotkey(settings.SettingsProfile2.Hotkey.GetWin32Modifiers(), settings.SettingsProfile2.Hotkey.KeyCode, this, 2);
                if (!hotkeyProfile2.Register())
                {
                    MessageBox.Show(isEn ? "Warning: Failed to register Profile 2 hotkey." : "Aviso: Falha ao registrar atalho do Perfil 2.");
                }
            }

            if (settings.GlobalToggleHotkey.KeyCode != Keys.None)
            {
                hotkeyGlobalToggle = new GlobalHotkey(settings.GlobalToggleHotkey.GetWin32Modifiers(), settings.GlobalToggleHotkey.KeyCode, this, 3);
                if (!hotkeyGlobalToggle.Register())
                {
                    MessageBox.Show(isEn ? "Warning: Failed to register Global Toggle hotkey." : "Aviso: Falha ao registrar atalho de Troca Global.");
                }
            }
        }

        private void ToggleRecording(int profile)
        {
            bool isEn = settings.Language == "en-US";
            string recordText = isEn ? "RECORD" : "GRAVAR";
            string stopText = isEn ? "STOP" : "PARAR";
            string pressText = isEn ? "Press hotkey..." : "Pressione a tecla...";

            if (profile == 1)
            {
                isRecording1 = !isRecording1;
                isRecording2 = false;
                isRecordingGlobal = false;
                btnRecord1.Text = isRecording1 ? stopText : recordText;
                btnRecord2.Text = recordText;
                btnRecordGlobal.Text = recordText;
                txtHotkey1.Text = isRecording1 ? pressText : settings.SettingsProfile1.Hotkey.ToString(isEn);
            }
            else if (profile == 2)
            {
                isRecording2 = !isRecording2;
                isRecording1 = false;
                isRecordingGlobal = false;
                btnRecord1.Text = recordText;
                btnRecord2.Text = isRecording2 ? stopText : recordText;
                btnRecordGlobal.Text = recordText;
                txtHotkey2.Text = isRecording2 ? pressText : settings.SettingsProfile2.Hotkey.ToString(isEn);
            }
            else
            {
                isRecordingGlobal = !isRecordingGlobal;
                isRecording1 = false;
                isRecording2 = false;
                btnRecord1.Text = recordText;
                btnRecord2.Text = recordText;
                btnRecordGlobal.Text = isRecordingGlobal ? stopText : recordText;
                txtHotkeyGlobal.Text = isRecordingGlobal ? pressText : settings.GlobalToggleHotkey.ToString(isEn);
            }

            ActiveControl = null;
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!isRecording1 && !isRecording2 && !isRecordingGlobal)
            {
                return;
            }

            if (e.KeyCode is Keys.ShiftKey or Keys.ControlKey or Keys.Menu)
            {
                return;
            }

            var hotkey = new HotkeyConfig
            {
                KeyCode = e.KeyCode,
                Alt = e.Alt,
                Ctrl = e.Control,
                Shift = e.Shift
            };

            if (isRecording1)
            {
                settings.SettingsProfile1.Hotkey = hotkey;
                ToggleRecording(1);
            }
            else if (isRecording2)
            {
                settings.SettingsProfile2.Hotkey = hotkey;
                ToggleRecording(2);
            }
            else if (isRecordingGlobal)
            {
                settings.GlobalToggleHotkey = hotkey;
                ToggleRecording(3);
            }

            bool isEn = settings.Language == "en-US";
            txtHotkey1.Text = settings.SettingsProfile1.Hotkey.ToString(isEn);
            txtHotkey2.Text = settings.SettingsProfile2.Hotkey.ToString(isEn);
            txtHotkeyGlobal.Text = settings.GlobalToggleHotkey.ToString(isEn);

            RegisterAllHotkeys();
            SaveSettingsDelayed();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmNchittest)
            {
                base.WndProc(ref m);
                if ((int)m.Result == HtClient)
                {
                    Point point = PointToClient(new Point(m.LParam.ToInt32()));
                    bool left = point.X <= ResizeBorder;
                    bool right = point.X >= ClientSize.Width - ResizeBorder;
                    bool top = point.Y <= ResizeBorder;
                    bool bottom = point.Y >= ClientSize.Height - ResizeBorder;

                    if (left && top) m.Result = (IntPtr)HtTopLeft;
                    else if (right && top) m.Result = (IntPtr)HtTopRight;
                    else if (left && bottom) m.Result = (IntPtr)HtBottomLeft;
                    else if (right && bottom) m.Result = (IntPtr)HtBottomRight;
                    else if (left) m.Result = (IntPtr)HtLeft;
                    else if (right) m.Result = (IntPtr)HtRight;
                    else if (top) m.Result = (IntPtr)HtTop;
                    else if (bottom) m.Result = (IntPtr)HtBottom;
                }
                return;
            }

            if (m.Msg == WmSizing)
            {
                base.WndProc(ref m);
                if (m.LParam != IntPtr.Zero)
                {
                    RECT rect = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT))!;
                    int width = rect.Right - rect.Left;
                    int height = rect.Bottom - rect.Top;
                    float targetRatio = 880f / 1210f;
                    int side = m.WParam.ToInt32();

                    if (side == 1 || side == 2 || side == 4 || side == 5)
                    {
                        int newHeight = (int)Math.Round(width / targetRatio);
                        rect.Bottom = rect.Top + newHeight;
                    }
                    else
                    {
                        int newWidth = (int)Math.Round(height * targetRatio);
                        rect.Right = rect.Left + newWidth;
                    }

                    Marshal.StructureToPtr(rect, m.LParam, false);
                }
                return;
            }

            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                int id = m.WParam.ToInt32();
                if (id == 1)
                {
                    ActivateProfile(settings.SettingsProfile1, 1);
                }
                else if (id == 2)
                {
                    ActivateProfile(settings.SettingsProfile2, 2);
                }
                else if (id == 3)
                {
                    ToggleGlobalProfile();
                }
            }

            base.WndProc(ref m);
        }

        private void ActivateProfile(Profile profile, int profileIndex)
        {
            var errors = new List<string>();

            if (!string.IsNullOrWhiteSpace(profile.InputDeviceId))
            {
                var inputDevice = inputDevices.FirstOrDefault(x => x.ID == profile.InputDeviceId);
                if (inputDevice == null || !audioManager.SetDefaultDevice(inputDevice))
                {
                    errors.Add("entrada");
                }
            }

            if (!string.IsNullOrWhiteSpace(profile.OutputDeviceId))
            {
                var outputDevice = outputDevices.FirstOrDefault(x => x.ID == profile.OutputDeviceId);
                if (outputDevice == null || !audioManager.SetDefaultDevice(outputDevice))
                {
                    errors.Add("saida");
                }
            }

            activeProfileIndex = profileIndex;
            currentToggleIndex = profileIndex == 1 ? 0 : 1;
            settings.LastActiveProfileIndex = profileIndex;

            UpdateActivateButtons();
            ApplyProfileVolumes(profile);
            FlashWindowForProfile(profile);
            SaveSettingsDelayed();

            if (errors.Count > 0)
            {
                MessageBox.Show(
                    "Nao foi possivel definir corretamente o dispositivo de " + string.Join(" e ", errors) + ".",
                    "Falha ao aplicar perfil",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void FlashWindowForProfile(Profile profile)
        {
            if (settings.TransparentPanelsMode)
            {
                return;
            }

            try
            {
                Color flashColor = ColorTranslator.FromHtml(profile.ColorHex);
                Color originalBack = BackColor;
                BackColor = flashColor;

                var timer = new System.Windows.Forms.Timer { Interval = 140 };
                timer.Tick += (s, e) =>
                {
                    BackColor = originalBack;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            catch
            {
            }
        }

        private void ToggleGlobalProfile()
        {
            if (currentToggleIndex == 0)
            {
                ActivateProfile(settings.SettingsProfile2, 2);
            }
            else
            {
                ActivateProfile(settings.SettingsProfile1, 1);
            }
        }

        private void ApplyProfileVolumes(Profile profile)
        {
            try
            {
                if (profile.ApplyInputVolume && !string.IsNullOrWhiteSpace(profile.InputDeviceId))
                {
                    var inputDevice = inputDevices.FirstOrDefault(x => x.ID == profile.InputDeviceId);
                    if (inputDevice != null)
                    {
                        audioManager.SetVolume(inputDevice, profile.InputVolume);
                    }
                }

                if (profile.ApplyOutputVolume && !string.IsNullOrWhiteSpace(profile.OutputDeviceId))
                {
                    var outputDevice = outputDevices.FirstOrDefault(x => x.ID == profile.OutputDeviceId);
                    if (outputDevice != null)
                    {
                        audioManager.SetVolume(outputDevice, profile.OutputVolume);
                    }
                }
            }
            catch
            {
            }
        }

        private void HandleVolumeChanged(Profile profile, TrackBar trackBar, Label label, bool isInput, int profileIndex)
        {
            if (isInitializingUi) return;

            bool isEn = settings.Language == "en-US";
            if (isInput)
            {
                profile.InputVolume = trackBar.Value;
                label.Text = isEn ? $"INPUT VOL: {trackBar.Value}%" : $"VOL ENTRADA: {trackBar.Value}%";
            }
            else
            {
                profile.OutputVolume = trackBar.Value;
                label.Text = isEn ? $"OUTPUT VOL: {trackBar.Value}%" : $"VOL SAIDA: {trackBar.Value}%";
            }

            if (activeProfileIndex == profileIndex)
            {
                ApplyProfileVolumes(profile);
            }

            SaveSettingsDelayed();
        }

        private void HandleApplyVolumeChanged(Profile profile, bool isChecked, bool isInput, int profileIndex)
        {
            if (isInitializingUi) return;

            if (isInput)
            {
                profile.ApplyInputVolume = isChecked;
            }
            else
            {
                profile.ApplyOutputVolume = isChecked;
            }

            if (activeProfileIndex == profileIndex)
            {
                ApplyProfileVolumes(profile);
            }

            SaveSettingsDelayed();
        }

        private void UpdateActivateButtons()
        {
            StyleActivateButton(btnActivate1, activeProfileIndex == 1, settings.SettingsProfile1.ColorHex);
            StyleActivateButton(btnActivate2, activeProfileIndex == 2, settings.SettingsProfile2.ColorHex);
        }

        private void StyleActivateButton(Button button, bool isActive, string profileColor)
        {
            Color accent;

            try
            {
                accent = ColorTranslator.FromHtml(profileColor);
            }
            catch
            {
                accent = Color.FromArgb(0, 120, 215);
            }

            bool isEn = settings.Language == "en-US";
            string activeText = isEn ? "ACTIVE" : "ATIVO";
            string inactiveText = isEn ? "ACTIVATE" : "ATIVAR";

            if (settings.TransparentPanelsMode)
            {
                button.Text = isActive ? activeText : inactiveText;
                button.BackColor = isActive ? Color.FromArgb(190, 220, 245, 255) : Color.FromArgb(95, 20, 26, 38);
                button.ForeColor = isActive ? Color.Black : Color.White;
                button.FlatAppearance.BorderColor = Color.FromArgb(190, 220, 245, 255);
                return;
            }

            button.Text = isActive ? activeText : inactiveText;
            button.BackColor = isActive ? accent : Color.FromArgb(90, 18, 24, 36);
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderColor = ControlPaint.Light(accent, isActive ? 0.2f : 0f);
        }

        private void CmbDevice_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isInitializingUi) return;

            if (cmbInput1.SelectedItem is DeviceItem input1)
            {
                settings.SettingsProfile1.InputDeviceId = input1.Id;
            }

            if (cmbOutput1.SelectedItem is DeviceItem output1)
            {
                settings.SettingsProfile1.OutputDeviceId = output1.Id;
            }

            if (cmbInput2.SelectedItem is DeviceItem input2)
            {
                settings.SettingsProfile2.InputDeviceId = input2.Id;
            }

            if (cmbOutput2.SelectedItem is DeviceItem output2)
            {
                settings.SettingsProfile2.OutputDeviceId = output2.Id;
            }

            SaveSettingsDelayed();
        }

        private void BtnColor1_Click(object? sender, EventArgs e)
        {
            using ColorDialog dialog = new ColorDialog { Color = pnlProfile1.BackColor };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                settings.SettingsProfile1.ColorHex = ColorTranslator.ToHtml(dialog.Color);
                UpdatePanelVisuals();
                UpdateActivateButtons();
                SaveSettingsDelayed();
            }
        }

        private void BtnColor2_Click(object? sender, EventArgs e)
        {
            using ColorDialog dialog = new ColorDialog { Color = pnlProfile2.BackColor };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                settings.SettingsProfile2.ColorHex = ColorTranslator.ToHtml(dialog.Color);
                UpdatePanelVisuals();
                UpdateActivateButtons();
                SaveSettingsDelayed();
            }
        }

        private void SaveSettingsDelayed()
        {
            saveTimer ??= new System.Windows.Forms.Timer { Interval = 450 };
            saveTimer.Tick -= SaveTimer_Tick;
            saveTimer.Tick += SaveTimer_Tick;
            saveTimer.Stop();
            saveTimer.Start();
        }

        private void SaveTimer_Tick(object? sender, EventArgs e)
        {
            saveTimer?.Stop();
            settings.LastActiveProfileIndex = activeProfileIndex;
            SettingsManager.Save(settings);
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            saveTimer?.Stop();
            settings.LastActiveProfileIndex = activeProfileIndex;
            SettingsManager.Save(settings);

            trayIcon.Visible = false;
            trayIcon.Dispose();
            hotkeyProfile1?.Dispose();
            hotkeyProfile2?.Dispose();
            hotkeyGlobalToggle?.Dispose();
        }
    }

    public class DeviceItem
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public override string ToString()
        {
            return Name;
        }
    }
}
