<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.LogsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ChangeUserToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OPenDebuggerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tmReadData = New System.Windows.Forms.Timer(Me.components)
        Me.btnSave = New DevExpress.XtraEditors.SimpleButton()
        Me.tbRSSettings = New DevExpress.XtraTab.XtraTabControl()
        Me.XtraTabPage1 = New DevExpress.XtraTab.XtraTabPage()
        Me.chkReceiving = New System.Windows.Forms.CheckBox()
        Me.cboDataBits = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.LabelControl7 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl8 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl9 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl10 = New DevExpress.XtraEditors.LabelControl()
        Me.cboStopBits = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.cboBaudrate = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.ComPortsBox = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.XtraTabPage3 = New DevExpress.XtraTab.XtraTabPage()
        Me.LabelControl16 = New DevExpress.XtraEditors.LabelControl()
        Me.cboLocation = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.txtMachine = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl11 = New DevExpress.XtraEditors.LabelControl()
        Me.txtSerial = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl6 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.txtServerName = New DevExpress.XtraEditors.TextEdit()
        Me.txtRoot = New DevExpress.XtraEditors.TextEdit()
        Me.txtPassword = New DevExpress.XtraEditors.TextEdit()
        Me.LabelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl5 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.txtDBName = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.ReceivedText = New System.Windows.Forms.RichTextBox()
        Me.tmConnect = New System.Windows.Forms.Timer(Me.components)
        Me.tbDbaseSettings = New DevExpress.XtraTab.XtraTabControl()
        Me.XtraTabPage5 = New DevExpress.XtraTab.XtraTabPage()
        Me.XtraTabControl3 = New DevExpress.XtraTab.XtraTabControl()
        Me.XtraTabPage7 = New DevExpress.XtraTab.XtraTabPage()
        Me.btnEdit = New DevExpress.XtraEditors.SimpleButton()
        Me.lblConnection = New DevExpress.XtraEditors.LabelControl()
        Me.btnDisconnect = New DevExpress.XtraEditors.SimpleButton()
        Me.btnConnect = New DevExpress.XtraEditors.SimpleButton()
        Me.btnClear = New DevExpress.XtraEditors.SimpleButton()
        Me.tmACK = New System.Windows.Forms.Timer(Me.components)
        Me.btnRefresh = New DevExpress.XtraEditors.SimpleButton()
        Me.lblVersion = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.mnuStrip.SuspendLayout()
        CType(Me.tbRSSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbRSSettings.SuspendLayout()
        Me.XtraTabPage1.SuspendLayout()
        CType(Me.cboDataBits.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cboStopBits.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cboBaudrate.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ComPortsBox.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.XtraTabPage3.SuspendLayout()
        CType(Me.cboLocation.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtMachine.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtSerial.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtServerName.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtRoot.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtPassword.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtDBName.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbDbaseSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbDbaseSettings.SuspendLayout()
        Me.XtraTabPage5.SuspendLayout()
        CType(Me.XtraTabControl3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.XtraTabControl3.SuspendLayout()
        Me.XtraTabPage7.SuspendLayout()
        Me.SuspendLayout()
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "SBSI LIS"
        Me.NotifyIcon1.Visible = True
        '
        'LogsToolStripMenuItem
        '
        Me.LogsToolStripMenuItem.Name = "LogsToolStripMenuItem"
        Me.LogsToolStripMenuItem.Size = New System.Drawing.Size(185, 24)
        Me.LogsToolStripMenuItem.Text = "Logs"
        '
        'ChangeUserToolStripMenuItem
        '
        Me.ChangeUserToolStripMenuItem.Name = "ChangeUserToolStripMenuItem"
        Me.ChangeUserToolStripMenuItem.Size = New System.Drawing.Size(185, 24)
        Me.ChangeUserToolStripMenuItem.Text = "Change User"
        '
        'OPenDebuggerToolStripMenuItem
        '
        Me.OPenDebuggerToolStripMenuItem.Name = "OPenDebuggerToolStripMenuItem"
        Me.OPenDebuggerToolStripMenuItem.Size = New System.Drawing.Size(185, 24)
        Me.OPenDebuggerToolStripMenuItem.Text = "Open Debugger"
        '
        'mnuStrip
        '
        Me.mnuStrip.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.mnuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OPenDebuggerToolStripMenuItem, Me.ChangeUserToolStripMenuItem, Me.LogsToolStripMenuItem})
        Me.mnuStrip.Name = "ContextMenuStrip1"
        Me.mnuStrip.Size = New System.Drawing.Size(186, 76)
        '
        'tmReadData
        '
        Me.tmReadData.Interval = 500
        '
        'btnSave
        '
        Me.btnSave.Enabled = False
        Me.btnSave.ImageOptions.Image = CType(resources.GetObject("btnSave.ImageOptions.Image"), System.Drawing.Image)
        Me.btnSave.Location = New System.Drawing.Point(869, 735)
        Me.btnSave.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(112, 34)
        Me.btnSave.TabIndex = 100
        Me.btnSave.Text = "&Save"
        '
        'tbRSSettings
        '
        Me.tbRSSettings.AppearancePage.Header.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.tbRSSettings.AppearancePage.Header.Options.UseFont = True
        Me.tbRSSettings.Enabled = False
        Me.tbRSSettings.Location = New System.Drawing.Point(14, 16)
        Me.tbRSSettings.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.tbRSSettings.Name = "tbRSSettings"
        Me.tbRSSettings.SelectedTabPage = Me.XtraTabPage1
        Me.tbRSSettings.Size = New System.Drawing.Size(484, 241)
        Me.tbRSSettings.TabIndex = 101
        Me.tbRSSettings.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.XtraTabPage1, Me.XtraTabPage3})
        '
        'XtraTabPage1
        '
        Me.XtraTabPage1.Controls.Add(Me.chkReceiving)
        Me.XtraTabPage1.Controls.Add(Me.cboDataBits)
        Me.XtraTabPage1.Controls.Add(Me.LabelControl7)
        Me.XtraTabPage1.Controls.Add(Me.LabelControl8)
        Me.XtraTabPage1.Controls.Add(Me.LabelControl9)
        Me.XtraTabPage1.Controls.Add(Me.LabelControl10)
        Me.XtraTabPage1.Controls.Add(Me.cboStopBits)
        Me.XtraTabPage1.Controls.Add(Me.cboBaudrate)
        Me.XtraTabPage1.Controls.Add(Me.ComPortsBox)
        Me.XtraTabPage1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.XtraTabPage1.Name = "XtraTabPage1"
        Me.XtraTabPage1.Size = New System.Drawing.Size(482, 202)
        Me.XtraTabPage1.Text = "RS 232 Settings"
        '
        'chkReceiving
        '
        Me.chkReceiving.AutoSize = True
        Me.chkReceiving.Checked = True
        Me.chkReceiving.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkReceiving.Location = New System.Drawing.Point(142, 162)
        Me.chkReceiving.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.chkReceiving.Name = "chkReceiving"
        Me.chkReceiving.Size = New System.Drawing.Size(179, 23)
        Me.chkReceiving.TabIndex = 133
        Me.chkReceiving.Text = "Enable Two Way Process"
        Me.chkReceiving.UseVisualStyleBackColor = True
        '
        'cboDataBits
        '
        Me.cboDataBits.Location = New System.Drawing.Point(142, 127)
        Me.cboDataBits.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.cboDataBits.Name = "cboDataBits"
        Me.cboDataBits.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cboDataBits.Properties.Items.AddRange(New Object() {"5", "8"})
        Me.cboDataBits.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        Me.cboDataBits.Size = New System.Drawing.Size(315, 24)
        Me.cboDataBits.TabIndex = 125
        '
        'LabelControl7
        '
        Me.LabelControl7.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl7.Appearance.Options.UseForeColor = True
        Me.LabelControl7.Location = New System.Drawing.Point(33, 30)
        Me.LabelControl7.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl7.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl7.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl7.Name = "LabelControl7"
        Me.LabelControl7.Size = New System.Drawing.Size(51, 17)
        Me.LabelControl7.TabIndex = 120
        Me.LabelControl7.Text = "ComPort"
        '
        'LabelControl8
        '
        Me.LabelControl8.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl8.Appearance.Options.UseForeColor = True
        Me.LabelControl8.Location = New System.Drawing.Point(33, 64)
        Me.LabelControl8.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl8.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl8.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl8.Name = "LabelControl8"
        Me.LabelControl8.Size = New System.Drawing.Size(55, 17)
        Me.LabelControl8.TabIndex = 122
        Me.LabelControl8.Text = "Baudrate:"
        '
        'LabelControl9
        '
        Me.LabelControl9.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl9.Appearance.Options.UseForeColor = True
        Me.LabelControl9.Location = New System.Drawing.Point(33, 132)
        Me.LabelControl9.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl9.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl9.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl9.Name = "LabelControl9"
        Me.LabelControl9.Size = New System.Drawing.Size(54, 17)
        Me.LabelControl9.TabIndex = 123
        Me.LabelControl9.Text = "Data Bits:"
        '
        'LabelControl10
        '
        Me.LabelControl10.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl10.Appearance.Options.UseForeColor = True
        Me.LabelControl10.Location = New System.Drawing.Point(33, 98)
        Me.LabelControl10.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl10.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl10.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl10.Name = "LabelControl10"
        Me.LabelControl10.Size = New System.Drawing.Size(54, 17)
        Me.LabelControl10.TabIndex = 121
        Me.LabelControl10.Text = "Stop Bits:"
        '
        'cboStopBits
        '
        Me.cboStopBits.Location = New System.Drawing.Point(142, 93)
        Me.cboStopBits.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.cboStopBits.Name = "cboStopBits"
        Me.cboStopBits.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cboStopBits.Properties.Items.AddRange(New Object() {"1", "2"})
        Me.cboStopBits.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        Me.cboStopBits.Size = New System.Drawing.Size(315, 24)
        Me.cboStopBits.TabIndex = 103
        '
        'cboBaudrate
        '
        Me.cboBaudrate.Location = New System.Drawing.Point(142, 59)
        Me.cboBaudrate.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.cboBaudrate.Name = "cboBaudrate"
        Me.cboBaudrate.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cboBaudrate.Properties.Items.AddRange(New Object() {"110", "300", "600", "1200", "1800", "2400", "4800", "7200", "9600", "14400", "19200", "38400", "57600", "115200", "230400", "460800", "921600"})
        Me.cboBaudrate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        Me.cboBaudrate.Size = New System.Drawing.Size(315, 24)
        Me.cboBaudrate.TabIndex = 102
        '
        'ComPortsBox
        '
        Me.ComPortsBox.Location = New System.Drawing.Point(142, 25)
        Me.ComPortsBox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.ComPortsBox.Name = "ComPortsBox"
        Me.ComPortsBox.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.ComPortsBox.Size = New System.Drawing.Size(315, 24)
        Me.ComPortsBox.TabIndex = 101
        '
        'XtraTabPage3
        '
        Me.XtraTabPage3.Controls.Add(Me.LabelControl16)
        Me.XtraTabPage3.Controls.Add(Me.cboLocation)
        Me.XtraTabPage3.Controls.Add(Me.txtMachine)
        Me.XtraTabPage3.Controls.Add(Me.LabelControl11)
        Me.XtraTabPage3.Controls.Add(Me.txtSerial)
        Me.XtraTabPage3.Controls.Add(Me.LabelControl6)
        Me.XtraTabPage3.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.XtraTabPage3.Name = "XtraTabPage3"
        Me.XtraTabPage3.Size = New System.Drawing.Size(482, 202)
        Me.XtraTabPage3.Text = "Machine Settings"
        '
        'LabelControl16
        '
        Me.LabelControl16.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl16.Appearance.Options.UseForeColor = True
        Me.LabelControl16.Location = New System.Drawing.Point(31, 102)
        Me.LabelControl16.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl16.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl16.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl16.Name = "LabelControl16"
        Me.LabelControl16.Size = New System.Drawing.Size(52, 17)
        Me.LabelControl16.TabIndex = 131
        Me.LabelControl16.Text = "Location:"
        '
        'cboLocation
        '
        Me.cboLocation.Location = New System.Drawing.Point(141, 97)
        Me.cboLocation.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.cboLocation.Name = "cboLocation"
        Me.cboLocation.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cboLocation.Properties.Items.AddRange(New Object() {"OPD", "ER", "Laboratory"})
        Me.cboLocation.Size = New System.Drawing.Size(315, 24)
        Me.cboLocation.TabIndex = 132
        '
        'txtMachine
        '
        Me.txtMachine.EditValue = "PentraC400"
        Me.txtMachine.Location = New System.Drawing.Point(141, 27)
        Me.txtMachine.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.txtMachine.Name = "txtMachine"
        Me.txtMachine.Size = New System.Drawing.Size(315, 24)
        Me.txtMachine.TabIndex = 126
        '
        'LabelControl11
        '
        Me.LabelControl11.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl11.Appearance.Options.UseForeColor = True
        Me.LabelControl11.Location = New System.Drawing.Point(31, 67)
        Me.LabelControl11.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl11.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl11.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl11.Name = "LabelControl11"
        Me.LabelControl11.Size = New System.Drawing.Size(60, 17)
        Me.LabelControl11.TabIndex = 127
        Me.LabelControl11.Text = "Serial No.:"
        '
        'txtSerial
        '
        Me.txtSerial.Location = New System.Drawing.Point(141, 61)
        Me.txtSerial.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.txtSerial.Name = "txtSerial"
        Me.txtSerial.Size = New System.Drawing.Size(315, 24)
        Me.txtSerial.TabIndex = 128
        '
        'LabelControl6
        '
        Me.LabelControl6.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl6.Appearance.Options.UseForeColor = True
        Me.LabelControl6.Location = New System.Drawing.Point(31, 33)
        Me.LabelControl6.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl6.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl6.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl6.Name = "LabelControl6"
        Me.LabelControl6.Size = New System.Drawing.Size(91, 17)
        Me.LabelControl6.TabIndex = 124
        Me.LabelControl6.Text = "Machine Name:"
        '
        'LabelControl1
        '
        Me.LabelControl1.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl1.Appearance.Options.UseForeColor = True
        Me.LabelControl1.Location = New System.Drawing.Point(16, 39)
        Me.LabelControl1.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl1.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl1.Name = "LabelControl1"
        Me.LabelControl1.Size = New System.Drawing.Size(79, 17)
        Me.LabelControl1.TabIndex = 114
        Me.LabelControl1.Text = "Server Name:"
        '
        'txtServerName
        '
        Me.txtServerName.Location = New System.Drawing.Point(154, 31)
        Me.txtServerName.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.txtServerName.Name = "txtServerName"
        Me.txtServerName.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.txtServerName.Properties.Appearance.Options.UseForeColor = True
        Me.txtServerName.Size = New System.Drawing.Size(302, 24)
        Me.txtServerName.TabIndex = 110
        '
        'txtRoot
        '
        Me.txtRoot.Location = New System.Drawing.Point(154, 68)
        Me.txtRoot.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.txtRoot.Name = "txtRoot"
        Me.txtRoot.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.txtRoot.Properties.Appearance.Options.UseForeColor = True
        Me.txtRoot.Size = New System.Drawing.Size(302, 24)
        Me.txtRoot.TabIndex = 111
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(154, 105)
        Me.txtPassword.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.txtPassword.Properties.Appearance.Options.UseForeColor = True
        Me.txtPassword.Properties.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(302, 24)
        Me.txtPassword.TabIndex = 112
        '
        'LabelControl4
        '
        Me.LabelControl4.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl4.Appearance.Options.UseForeColor = True
        Me.LabelControl4.Location = New System.Drawing.Point(16, 77)
        Me.LabelControl4.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl4.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl4.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl4.Name = "LabelControl4"
        Me.LabelControl4.Size = New System.Drawing.Size(62, 17)
        Me.LabelControl4.TabIndex = 116
        Me.LabelControl4.Text = "Username:"
        '
        'LabelControl5
        '
        Me.LabelControl5.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl5.Appearance.Options.UseForeColor = True
        Me.LabelControl5.Location = New System.Drawing.Point(16, 149)
        Me.LabelControl5.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl5.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl5.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl5.Name = "LabelControl5"
        Me.LabelControl5.Size = New System.Drawing.Size(97, 17)
        Me.LabelControl5.TabIndex = 117
        Me.LabelControl5.Text = "Database Name:"
        '
        'LabelControl2
        '
        Me.LabelControl2.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl2.Appearance.Options.UseForeColor = True
        Me.LabelControl2.Location = New System.Drawing.Point(16, 114)
        Me.LabelControl2.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl2.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl2.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl2.Name = "LabelControl2"
        Me.LabelControl2.Size = New System.Drawing.Size(59, 17)
        Me.LabelControl2.TabIndex = 115
        Me.LabelControl2.Text = "Password:"
        '
        'txtDBName
        '
        Me.txtDBName.Location = New System.Drawing.Point(154, 141)
        Me.txtDBName.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.txtDBName.Name = "txtDBName"
        Me.txtDBName.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.txtDBName.Properties.Appearance.Options.UseForeColor = True
        Me.txtDBName.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.txtDBName.Size = New System.Drawing.Size(302, 24)
        Me.txtDBName.TabIndex = 113
        '
        'ReceivedText
        '
        Me.ReceivedText.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ReceivedText.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ReceivedText.Location = New System.Drawing.Point(0, 0)
        Me.ReceivedText.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.ReceivedText.Name = "ReceivedText"
        Me.ReceivedText.ReadOnly = True
        Me.ReceivedText.Size = New System.Drawing.Size(965, 403)
        Me.ReceivedText.TabIndex = 0
        Me.ReceivedText.Text = ""
        '
        'tmConnect
        '
        Me.tmConnect.Enabled = True
        Me.tmConnect.Interval = 2000
        '
        'tbDbaseSettings
        '
        Me.tbDbaseSettings.AppearancePage.Header.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.tbDbaseSettings.AppearancePage.Header.Options.UseFont = True
        Me.tbDbaseSettings.Enabled = False
        Me.tbDbaseSettings.Location = New System.Drawing.Point(505, 16)
        Me.tbDbaseSettings.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.tbDbaseSettings.Name = "tbDbaseSettings"
        Me.tbDbaseSettings.SelectedTabPage = Me.XtraTabPage5
        Me.tbDbaseSettings.Size = New System.Drawing.Size(476, 241)
        Me.tbDbaseSettings.TabIndex = 121
        Me.tbDbaseSettings.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.XtraTabPage5})
        '
        'XtraTabPage5
        '
        Me.XtraTabPage5.Controls.Add(Me.txtServerName)
        Me.XtraTabPage5.Controls.Add(Me.LabelControl5)
        Me.XtraTabPage5.Controls.Add(Me.LabelControl4)
        Me.XtraTabPage5.Controls.Add(Me.LabelControl1)
        Me.XtraTabPage5.Controls.Add(Me.LabelControl2)
        Me.XtraTabPage5.Controls.Add(Me.txtPassword)
        Me.XtraTabPage5.Controls.Add(Me.txtDBName)
        Me.XtraTabPage5.Controls.Add(Me.txtRoot)
        Me.XtraTabPage5.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.XtraTabPage5.Name = "XtraTabPage5"
        Me.XtraTabPage5.Size = New System.Drawing.Size(474, 202)
        Me.XtraTabPage5.Text = "Database Settings"
        '
        'XtraTabControl3
        '
        Me.XtraTabControl3.AppearancePage.Header.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold)
        Me.XtraTabControl3.AppearancePage.Header.Options.UseFont = True
        Me.XtraTabControl3.Location = New System.Drawing.Point(14, 263)
        Me.XtraTabControl3.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.XtraTabControl3.Name = "XtraTabControl3"
        Me.XtraTabControl3.SelectedTabPage = Me.XtraTabPage7
        Me.XtraTabControl3.Size = New System.Drawing.Size(967, 442)
        Me.XtraTabControl3.TabIndex = 122
        Me.XtraTabControl3.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.XtraTabPage7})
        '
        'XtraTabPage7
        '
        Me.XtraTabPage7.Controls.Add(Me.ReceivedText)
        Me.XtraTabPage7.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.XtraTabPage7.Name = "XtraTabPage7"
        Me.XtraTabPage7.Size = New System.Drawing.Size(965, 403)
        Me.XtraTabPage7.Text = "Communication"
        '
        'btnEdit
        '
        Me.btnEdit.ImageOptions.Image = CType(resources.GetObject("btnEdit.ImageOptions.Image"), System.Drawing.Image)
        Me.btnEdit.Location = New System.Drawing.Point(750, 735)
        Me.btnEdit.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Size = New System.Drawing.Size(112, 34)
        Me.btnEdit.TabIndex = 123
        Me.btnEdit.Text = "&Edit"
        '
        'lblConnection
        '
        Me.lblConnection.Appearance.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.lblConnection.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.lblConnection.Appearance.Options.UseFont = True
        Me.lblConnection.Appearance.Options.UseForeColor = True
        Me.lblConnection.Location = New System.Drawing.Point(14, 743)
        Me.lblConnection.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.lblConnection.Name = "lblConnection"
        Me.lblConnection.Size = New System.Drawing.Size(207, 21)
        Me.lblConnection.TabIndex = 130
        Me.lblConnection.Text = "Connection Status: Connected"
        '
        'btnDisconnect
        '
        Me.btnDisconnect.ImageOptions.Image = CType(resources.GetObject("btnDisconnect.ImageOptions.Image"), System.Drawing.Image)
        Me.btnDisconnect.Location = New System.Drawing.Point(496, 735)
        Me.btnDisconnect.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.btnDisconnect.Name = "btnDisconnect"
        Me.btnDisconnect.Size = New System.Drawing.Size(112, 34)
        Me.btnDisconnect.TabIndex = 131
        Me.btnDisconnect.Text = "&Disconnect"
        '
        'btnConnect
        '
        Me.btnConnect.ImageOptions.Image = CType(resources.GetObject("btnConnect.ImageOptions.Image"), System.Drawing.Image)
        Me.btnConnect.Location = New System.Drawing.Point(377, 735)
        Me.btnConnect.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(112, 34)
        Me.btnConnect.TabIndex = 132
        Me.btnConnect.Text = "&Connect"
        '
        'btnClear
        '
        Me.btnClear.ImageOptions.Image = CType(resources.GetObject("btnClear.ImageOptions.Image"), System.Drawing.Image)
        Me.btnClear.Location = New System.Drawing.Point(631, 735)
        Me.btnClear.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(112, 34)
        Me.btnClear.TabIndex = 133
        Me.btnClear.Text = "&Clear Log"
        '
        'tmACK
        '
        Me.tmACK.Interval = 500
        '
        'btnRefresh
        '
        Me.btnRefresh.ImageOptions.Image = CType(resources.GetObject("btnRefresh.ImageOptions.Image"), System.Drawing.Image)
        Me.btnRefresh.Location = New System.Drawing.Point(258, 735)
        Me.btnRefresh.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(112, 34)
        Me.btnRefresh.TabIndex = 134
        Me.btnRefresh.Text = "&Refresh"
        '
        'lblVersion
        '
        Me.lblVersion.Appearance.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblVersion.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.lblVersion.Appearance.Options.UseFont = True
        Me.lblVersion.Appearance.Options.UseForeColor = True
        Me.lblVersion.Location = New System.Drawing.Point(43, 719)
        Me.lblVersion.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.lblVersion.LookAndFeel.UseDefaultLookAndFeel = False
        Me.lblVersion.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(44, 19)
        Me.lblVersion.TabIndex = 152
        Me.lblVersion.Text = "0.0.0.0"
        '
        'LabelControl3
        '
        Me.LabelControl3.Appearance.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold)
        Me.LabelControl3.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(27, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(62, Byte), Integer))
        Me.LabelControl3.Appearance.Options.UseFont = True
        Me.LabelControl3.Appearance.Options.UseForeColor = True
        Me.LabelControl3.Location = New System.Drawing.Point(14, 719)
        Me.LabelControl3.LookAndFeel.SkinName = "Visual Studio 2013 Blue"
        Me.LabelControl3.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LabelControl3.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LabelControl3.Name = "LabelControl3"
        Me.LabelControl3.Size = New System.Drawing.Size(23, 19)
        Me.LabelControl3.TabIndex = 151
        Me.LabelControl3.Text = "Ver"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(993, 777)
        Me.Controls.Add(Me.lblVersion)
        Me.Controls.Add(Me.LabelControl3)
        Me.Controls.Add(Me.btnRefresh)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.btnConnect)
        Me.Controls.Add(Me.btnDisconnect)
        Me.Controls.Add(Me.lblConnection)
        Me.Controls.Add(Me.btnEdit)
        Me.Controls.Add(Me.XtraTabControl3)
        Me.Controls.Add(Me.tbDbaseSettings)
        Me.Controls.Add(Me.tbRSSettings)
        Me.Controls.Add(Me.btnSave)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.mnuStrip.ResumeLayout(False)
        CType(Me.tbRSSettings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbRSSettings.ResumeLayout(False)
        Me.XtraTabPage1.ResumeLayout(False)
        Me.XtraTabPage1.PerformLayout()
        CType(Me.cboDataBits.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cboStopBits.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cboBaudrate.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ComPortsBox.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.XtraTabPage3.ResumeLayout(False)
        Me.XtraTabPage3.PerformLayout()
        CType(Me.cboLocation.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtMachine.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtSerial.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtServerName.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtRoot.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtPassword.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtDBName.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbDbaseSettings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbDbaseSettings.ResumeLayout(False)
        Me.XtraTabPage5.ResumeLayout(False)
        Me.XtraTabPage5.PerformLayout()
        CType(Me.XtraTabControl3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.XtraTabControl3.ResumeLayout(False)
        Me.XtraTabPage7.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents LogsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ChangeUserToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OPenDebuggerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuStrip As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tmReadData As System.Windows.Forms.Timer
    Friend WithEvents btnSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents tbRSSettings As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents XtraTabPage1 As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents ComPortsBox As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents LabelControl11 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents cboDataBits As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents LabelControl6 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl7 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl8 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl9 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl10 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents cboStopBits As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents cboBaudrate As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents LabelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents txtServerName As DevExpress.XtraEditors.TextEdit
    Friend WithEvents txtRoot As DevExpress.XtraEditors.TextEdit
    Friend WithEvents txtPassword As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LabelControl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl5 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents txtDBName As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents txtMachine As DevExpress.XtraEditors.TextEdit
    Friend WithEvents txtSerial As DevExpress.XtraEditors.TextEdit
    Friend WithEvents tmConnect As System.Windows.Forms.Timer
    Friend WithEvents ReceivedText As System.Windows.Forms.RichTextBox
    Friend WithEvents tbDbaseSettings As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents XtraTabPage5 As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents XtraTabControl3 As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents XtraTabPage7 As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents btnEdit As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents lblConnection As DevExpress.XtraEditors.LabelControl
    Friend WithEvents btnDisconnect As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents btnConnect As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents btnClear As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents XtraTabPage3 As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents LabelControl16 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents cboLocation As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents tmACK As System.Windows.Forms.Timer
    Friend WithEvents chkReceiving As System.Windows.Forms.CheckBox
    Friend WithEvents btnRefresh As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents lblVersion As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LabelControl3 As DevExpress.XtraEditors.LabelControl
End Class
