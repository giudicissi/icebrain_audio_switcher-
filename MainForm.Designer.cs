using System.Drawing;
using System.Windows.Forms;

namespace AudioSwitcherApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            pnlTopBar = new GlassPanel();
            lblHeader = new Label();
            btnMinimize = new Button();
            btnClose = new Button();

            pnlProfile1 = new GlassPanel();
            txtName1 = new TextBox();
            lblInput1 = new Label();
            cmbInput1 = new ComboBox();
            lblOutput1 = new Label();
            cmbOutput1 = new ComboBox();
            lblHotkey1 = new Label();
            txtHotkey1 = new TextBox();
            btnRecord1 = new Button();
            btnActivate1 = new Button();
            btnColor1 = new Button();
            lblInputVol1 = new Label();
            trkInputVol1 = new TrackBar();
            chkApplyInputVol1 = new CheckBox();
            lblOutputVol1 = new Label();
            trkOutputVol1 = new TrackBar();
            chkApplyOutputVol1 = new CheckBox();

            pnlProfile2 = new GlassPanel();
            txtName2 = new TextBox();
            lblInput2 = new Label();
            cmbInput2 = new ComboBox();
            lblOutput2 = new Label();
            cmbOutput2 = new ComboBox();
            lblHotkey2 = new Label();
            txtHotkey2 = new TextBox();
            btnRecord2 = new Button();
            btnActivate2 = new Button();
            btnColor2 = new Button();
            lblInputVol2 = new Label();
            trkInputVol2 = new TrackBar();
            chkApplyInputVol2 = new CheckBox();
            lblOutputVol2 = new Label();
            trkOutputVol2 = new TrackBar();
            chkApplyOutputVol2 = new CheckBox();

            pnlGlobal = new GlassPanel();
            lblGlobal = new Label();
            txtHotkeyGlobal = new TextBox();
            btnRecordGlobal = new Button();
            lblRefreshHeader = new Label();
            btnRefresh = new Button();

            chkStartWithWindows = new CheckBox();
            chkStartMinimized = new CheckBox();
            chkTransparentPanels = new CheckBox();
            lblLanguage = new Label();
            picFlagBR = new PictureBox();
            picFlagUS = new PictureBox();
            picLogo = new PictureBox();

            ((System.ComponentModel.ISupportInitialize)trkInputVol1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkOutputVol1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkInputVol2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkOutputVol2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picFlagBR).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picFlagUS).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();

            BackColor = Color.FromArgb(4, 6, 12);
            TransparencyKey = Color.FromArgb(4, 6, 12);
            AllowTransparency = true;
            ClientSize = new Size(880, 1210);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(440, 605);
            MaximumSize = new Size(1760, 2420);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ICEBRAIN SWITCHER";

            pnlTopBar.BackColor = Color.Transparent;
            pnlTopBar.BorderColor = Color.Transparent;
            pnlTopBar.Location = new Point(0, 0);
            pnlTopBar.Size = new Size(1, 1);
            pnlTopBar.Visible = false;

            lblHeader.AutoSize = true;
            lblHeader.BackColor = Color.Transparent;
            lblHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            lblHeader.ForeColor = Color.Transparent;
            lblHeader.Location = new Point(0, 0);
            lblHeader.Text = "ICEBRAIN SWITCHER";
            lblHeader.Visible = false;

            ConfigureWindowButton(btnMinimize, "", new Point(706, 7), new Size(36, 26));
            ConfigureWindowButton(btnClose, "", new Point(825, 6), new Size(36, 26));
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(160, 220, 50, 50);
            btnClose.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 180, 30, 30);
            btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            pnlTopBar.Controls.AddRange(new Control[] { lblHeader, btnMinimize, btnClose });

            ConfigureProfilePanel(pnlProfile1, new Point(40, 76), new Size(800, 280));
            ConfigureProfilePanel(pnlProfile2, new Point(40, 386), new Size(800, 280));
            pnlProfile1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlProfile2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ConfigureTextTitle(txtName1, new Point(18, 18), new Size(340, 32));
            ConfigureTextTitle(txtName2, new Point(18, 18), new Size(340, 32));
            txtName1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtName2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtName1.Text = "oculos VR";
            txtName2.Text = "pc padraozimmm";

            ConfigureLabel(lblInput1, "ENTRADA", new Point(18, 70), Color.Gainsboro);
            ConfigureLabel(lblOutput1, "SAIDA", new Point(18, 108), Color.Gainsboro);
            ConfigureLabel(lblHotkey1, "ATALHO:", new Point(18, 146), Color.Gainsboro);
            ConfigureLabel(lblInput2, "ENTRADA", new Point(18, 70), Color.Gainsboro);
            ConfigureLabel(lblOutput2, "SAIDA", new Point(18, 108), Color.Gainsboro);
            ConfigureLabel(lblHotkey2, "ATALHO:", new Point(18, 146), Color.Gainsboro);

            ConfigureCombo(cmbInput1, new Point(102, 66), new Size(680, 25));
            ConfigureCombo(cmbOutput1, new Point(102, 104), new Size(680, 25));
            ConfigureCombo(cmbInput2, new Point(102, 66), new Size(680, 25));
            ConfigureCombo(cmbOutput2, new Point(102, 104), new Size(680, 25));
            cmbInput1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbOutput1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbInput2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbOutput2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            ConfigureHotkeyBox(txtHotkey1, new Point(86, 142), new Size(205, 25));
            ConfigureHotkeyBox(txtHotkey2, new Point(86, 142), new Size(205, 25));

            ConfigureButton(btnRecord1, "GRAVAR", new Point(305, 140), new Size(94, 30), Color.FromArgb(65, 65, 65));
            ConfigureButton(btnActivate1, "ATIVAR", new Point(410, 140), new Size(110, 30), Color.FromArgb(65, 65, 65));
            ConfigureButton(btnColor1, "COR DO", new Point(532, 140), new Size(110, 30), Color.FromArgb(65, 65, 65));
            btnRecord1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnActivate1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnColor1.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            ConfigureButton(btnRecord2, "GRAVAR", new Point(305, 140), new Size(94, 30), Color.FromArgb(65, 65, 65));
            ConfigureButton(btnActivate2, "ATIVAR", new Point(410, 140), new Size(110, 30), Color.FromArgb(65, 65, 65));
            ConfigureButton(btnColor2, "COR DO", new Point(532, 140), new Size(110, 30), Color.FromArgb(65, 65, 65));
            btnRecord2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnActivate2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnColor2.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            ConfigureVolumeLabel(lblInputVol1, "VOL ENTRADA: 100%", new Point(18, 196), Color.FromArgb(170, 220, 255));
            ConfigureVolumeLabel(lblOutputVol1, "VOL SAIDA: 100%", new Point(18, 236), Color.FromArgb(160, 255, 200));
            ConfigureVolumeLabel(lblInputVol2, "VOL ENTRADA: 100%", new Point(18, 196), Color.FromArgb(170, 220, 255));
            ConfigureVolumeLabel(lblOutputVol2, "VOL SAIDA: 100%", new Point(18, 236), Color.FromArgb(160, 255, 200));

            ConfigureTrackBar(trkInputVol1, new Point(230, 188), new Size(430, 45), Color.FromArgb(24, 30, 42));
            ConfigureTrackBar(trkOutputVol1, new Point(230, 228), new Size(430, 45), Color.FromArgb(24, 30, 42));
            ConfigureTrackBar(trkInputVol2, new Point(230, 188), new Size(430, 45), Color.FromArgb(24, 30, 42));
            ConfigureTrackBar(trkOutputVol2, new Point(230, 228), new Size(430, 45), Color.FromArgb(24, 30, 42));
            trkInputVol1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkOutputVol1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkInputVol2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkOutputVol2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            ConfigureApplyCheckbox(chkApplyInputVol1, new Point(680, 196), Color.FromArgb(170, 220, 255));
            ConfigureApplyCheckbox(chkApplyOutputVol1, new Point(680, 236), Color.FromArgb(160, 255, 200));
            ConfigureApplyCheckbox(chkApplyInputVol2, new Point(680, 196), Color.FromArgb(170, 220, 255));
            ConfigureApplyCheckbox(chkApplyOutputVol2, new Point(680, 236), Color.FromArgb(160, 255, 200));

            pnlProfile1.Controls.AddRange(new Control[]
            {
                txtName1, lblInput1, cmbInput1, lblOutput1, cmbOutput1, lblHotkey1, txtHotkey1,
                btnRecord1, btnActivate1, btnColor1, lblInputVol1, trkInputVol1, chkApplyInputVol1,
                lblOutputVol1, trkOutputVol1, chkApplyOutputVol1
            });

            pnlProfile2.Controls.AddRange(new Control[]
            {
                txtName2, lblInput2, cmbInput2, lblOutput2, cmbOutput2, lblHotkey2, txtHotkey2,
                btnRecord2, btnActivate2, btnColor2, lblInputVol2, trkInputVol2, chkApplyInputVol2,
                lblOutputVol2, trkOutputVol2, chkApplyOutputVol2
            });

            pnlGlobal.BackColor = Color.FromArgb(120, 10, 18, 30);
            pnlGlobal.BorderColor = Color.FromArgb(185, 214, 237, 255);
            pnlGlobal.Location = new Point(40, 696);
            pnlGlobal.Size = new Size(800, 96);
            pnlGlobal.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            lblGlobal.AutoSize = true;
            lblGlobal.BackColor = Color.Transparent;
            lblGlobal.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            lblGlobal.ForeColor = Color.White;
            lblGlobal.Location = new Point(18, 14);
            lblGlobal.Text = "ATALHO TROCAR TUDO";

            ConfigureHotkeyBox(txtHotkeyGlobal, new Point(18, 48), new Size(262, 25));
            txtHotkeyGlobal.ForeColor = Color.Gold;
            ConfigureButton(btnRecordGlobal, "GRAVAR", new Point(294, 45), new Size(94, 30), Color.FromArgb(65, 65, 65));
            txtHotkeyGlobal.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            btnRecordGlobal.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            lblRefreshHeader.AutoSize = true;
            lblRefreshHeader.BackColor = Color.Transparent;
            lblRefreshHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            lblRefreshHeader.ForeColor = Color.White;
            lblRefreshHeader.Location = new Point(570, 14);
            lblRefreshHeader.Text = "DISPOSITIVOS DE ÁUDIO";
            lblRefreshHeader.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            ConfigureButton(btnRefresh, "ATUALIZAR", new Point(570, 45), new Size(204, 30), Color.FromArgb(0, 122, 204));
            btnRefresh.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            pnlGlobal.Controls.AddRange(new Control[] { lblGlobal, txtHotkeyGlobal, btnRecordGlobal, lblRefreshHeader, btnRefresh });

            ConfigureToggle(chkStartWithWindows, "INICIAR COM O WINDOWS", new Point(45, 820));
            ConfigureToggle(chkStartMinimized, "INICIAR MINIMIZADO", new Point(45, 856));
            ConfigureToggle(chkTransparentPanels, "MODO TRANSPARENTE", new Point(45, 892));
            chkStartWithWindows.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            chkStartMinimized.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            chkTransparentPanels.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            lblLanguage.AutoSize = true;
            lblLanguage.BackColor = Color.FromArgb(110, 10, 18, 30);
            lblLanguage.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            lblLanguage.ForeColor = Color.White;
            lblLanguage.Location = new Point(45, 930);
            lblLanguage.Text = "IDIOMA:";
            lblLanguage.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            picFlagBR.BackColor = Color.Transparent;
            picFlagBR.Cursor = Cursors.Hand;
            picFlagBR.Location = new Point(135, 926);
            picFlagBR.Size = new Size(44, 28);
            picFlagBR.SizeMode = PictureBoxSizeMode.StretchImage;
            picFlagBR.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            picFlagUS.BackColor = Color.Transparent;
            picFlagUS.Cursor = Cursors.Hand;
            picFlagUS.Location = new Point(190, 926);
            picFlagUS.Size = new Size(44, 28);
            picFlagUS.SizeMode = PictureBoxSizeMode.StretchImage;
            picFlagUS.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            picLogo.BackColor = Color.Transparent;
            picLogo.Location = new Point(630, 810);
            picLogo.Size = new Size(200, 200);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            Controls.AddRange(new Control[]
            {
                pnlTopBar, pnlProfile1, pnlProfile2, pnlGlobal,
                chkStartWithWindows, chkStartMinimized, chkTransparentPanels,
                lblLanguage, picFlagBR, picFlagUS, picLogo
            });
            Controls.Add(btnMinimize);
            Controls.Add(btnClose);

            ((System.ComponentModel.ISupportInitialize)trkInputVol1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkOutputVol1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkInputVol2).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkOutputVol2).EndInit();
            ((System.ComponentModel.ISupportInitialize)picFlagBR).EndInit();
            ((System.ComponentModel.ISupportInitialize)picFlagUS).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private static void ConfigureProfilePanel(GlassPanel panel, Point location, Size size)
        {
            panel.BackColor = Color.FromArgb(185, 44, 44, 48);
            panel.BorderColor = Color.FromArgb(210, 255, 255, 255);
            panel.Location = location;
            panel.Size = size;
        }

        private static void ConfigureTextTitle(TextBox textBox, Point location, Size size)
        {
            textBox.BackColor = Color.FromArgb(28, 28, 32);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
            textBox.ForeColor = Color.White;
            textBox.Location = location;
            textBox.Size = size;
        }

        private static void ConfigureLabel(Label label, string text, Point location, Color color)
        {
            label.AutoSize = true;
            label.BackColor = Color.Transparent;
            label.ForeColor = color;
            label.Location = location;
            label.Text = text;
        }

        private static void ConfigureCombo(ComboBox comboBox, Point location, Size size)
        {
            comboBox.BackColor = Color.White;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.ForeColor = Color.Black;
            comboBox.Location = location;
            comboBox.Size = size;
        }

        private static void ConfigureHotkeyBox(TextBox textBox, Point location, Size size)
        {
            textBox.BackColor = Color.FromArgb(22, 22, 24);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.ForeColor = Color.Lime;
            textBox.Location = location;
            textBox.ReadOnly = true;
            textBox.Size = size;
        }

        private static void ConfigureButton(Button button, string text, Point location, Size size, Color backColor)
        {
            button.BackColor = backColor;
            button.FlatAppearance.BorderColor = Color.White;
            button.FlatAppearance.BorderSize = 1;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button.ForeColor = Color.White;
            button.Location = location;
            button.Size = size;
            button.Text = text;
            button.UseVisualStyleBackColor = false;
        }

        private static void ConfigureWindowButton(Button button, string text, Point location, Size size)
        {
            button.BackColor = Color.Transparent;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button.ForeColor = Color.Transparent;
            button.Location = location;
            button.Size = size;
            button.Text = text;
            button.UseVisualStyleBackColor = false;
            button.TabStop = false;
        }

        private static void ConfigureVolumeLabel(Label label, string text, Point location, Color color)
        {
            label.AutoSize = false;
            label.BackColor = Color.Transparent;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label.ForeColor = color;
            label.Location = location;
            label.Size = new Size(185, 20);
            label.Text = text;
        }

        private static void ConfigureTrackBar(TrackBar trackBar, Point location, Size size, Color backColor)
        {
            trackBar.BackColor = Color.FromArgb(backColor.R, backColor.G, backColor.B);
            trackBar.LargeChange = 10;
            trackBar.Location = location;
            trackBar.Maximum = 100;
            trackBar.Minimum = 0;
            trackBar.Size = size;
            trackBar.SmallChange = 1;
            trackBar.TickFrequency = 10;
            trackBar.Value = 100;
        }

        private static void ConfigureApplyCheckbox(CheckBox checkBox, Point location, Color color)
        {
            checkBox.AutoSize = true;
            checkBox.BackColor = Color.Transparent;
            checkBox.Checked = true;
            checkBox.CheckState = CheckState.Checked;
            checkBox.Font = new Font("Segoe UI", 8F, FontStyle.Bold, GraphicsUnit.Point);
            checkBox.ForeColor = color;
            checkBox.Location = location;
            checkBox.Text = "APLICAR";
        }

        private static void ConfigureToggle(CheckBox checkBox, string text, Point location)
        {
            checkBox.AutoSize = true;
            checkBox.BackColor = Color.FromArgb(110, 10, 18, 30);
            checkBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            checkBox.ForeColor = Color.White;
            checkBox.Location = location;
            checkBox.Text = text;
        }

        private GlassPanel pnlTopBar;
        private Label lblHeader;
        private Button btnMinimize;
        private Button btnClose;

        private GlassPanel pnlProfile1;
        private TextBox txtName1;
        private Label lblInput1;
        private ComboBox cmbInput1;
        private Label lblOutput1;
        private ComboBox cmbOutput1;
        private Label lblHotkey1;
        private TextBox txtHotkey1;
        private Button btnRecord1;
        private Button btnColor1;

        private GlassPanel pnlProfile2;
        private TextBox txtName2;
        private Label lblInput2;
        private ComboBox cmbInput2;
        private Label lblOutput2;
        private ComboBox cmbOutput2;
        private Label lblHotkey2;
        private TextBox txtHotkey2;
        private Button btnRecord2;
        private Button btnColor2;

        private GlassPanel pnlGlobal;
        private Label lblGlobal;
        private TextBox txtHotkeyGlobal;
        private Button btnRecordGlobal;
        private Label lblRefreshHeader;
        private Button btnRefresh;

        private Button btnActivate1;
        private Button btnActivate2;
        private CheckBox chkStartWithWindows;
        private CheckBox chkStartMinimized;
        private CheckBox chkTransparentPanels;
        private Label lblLanguage;
        private PictureBox picFlagBR;
        private PictureBox picFlagUS;
        private PictureBox picLogo;

        private TrackBar trkInputVol1;
        private Label lblInputVol1;
        private CheckBox chkApplyInputVol1;
        private TrackBar trkOutputVol1;
        private Label lblOutputVol1;
        private CheckBox chkApplyOutputVol1;

        private TrackBar trkInputVol2;
        private Label lblInputVol2;
        private CheckBox chkApplyInputVol2;
        private TrackBar trkOutputVol2;
        private Label lblOutputVol2;
        private CheckBox chkApplyOutputVol2;
    }
}
