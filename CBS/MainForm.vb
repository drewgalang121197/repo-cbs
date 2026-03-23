Imports System.Drawing.Imaging
Imports System.Drawing.Printing
Imports System.Environment
Imports System.IO
Imports System.IO.Ports
Imports System.Text
Imports System.Threading
Imports System.Timers
Imports System.Xml
Imports System.Text.RegularExpressions

Imports System.Threading.Tasks

Imports Bluegrams.Application
Imports MySql.Data.MySqlClient

Public Class MainForm

#Region "Variables"
    'Dim WithEvents Port As New SerialPort
    Dim Port As New SerialPort

    'String manipulation in "Received Data"
    Dim PatientID As String
    Dim PatientName As String
    Dim PatientSex As String
    Dim PatientSexFinal As String
    Dim PatientBdate As String
    Dim PatientBdateFinal As String
    Dim MainID As String
    Dim SampleID As String
    Dim OrderID As String
    Dim ControlID As String
    Dim ResultName As String
    Dim ResultValue As String
    Dim Instrument As String
    Dim Serial As String
    Dim Section As String = "Chemistry"
    Dim SubSection As String = "ABG"
    Dim QueryStatus As Boolean = False
    Dim ctrl_val As String
    Dim LotNo As String
    Dim DateReceived As String
    Dim TimeReceived As String

    Dim SampleID_Final As String
    'END

    'String manipulation "Query Data"
    Dim QRSampleID As String
    Dim QRMainID As String
    'END

    'Variables for retrieving data in the database
    Dim unit, range, unit_conv, range_conv, age_group, patient_id, test_name, test_code, test_group, order_no As String
    Dim DefaultUnit As Integer = 0
    'END

    'Communication Port Data Handler
    Dim ReadDataASTM1 As String = ""
    Dim ReadDataASTM2 As String = ""
    Dim EnqReceived As Char = ""
    Dim EOTReceived As Char = ""
    Dim NAKReceived As Char = ""
    Dim ACKReceived As Char = ""

    Dim TimedOut As Integer
    Dim connectionCheckerTimer As New Timers.Timer()
    'END

    'Write to TraceComm.log and Error.Log FIle
    Dim CommLog As FileStream
    Dim CommWriter As StreamWriter
    Dim TraceComm As String = "TraceComm.log"
    Dim ErrorComm As String = "Error.log"
    'END

    'Write to ConnectionString.log File
    Dim ConnectionLog As FileStream
    Dim ConnectionWriter As StreamWriter
    Dim ConnectionString As String = "ConnectionString.log"

    Dim Settings As String = "Settings.config"
    Dim SettingsContent As String = ""
    'END
    Public Const WM_RESTORE_FROM_TRAY As Integer = &H8001
#End Region
    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_RESTORE_FROM_TRAY Then
            RestoreFromTray()
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub RestoreFromTray()
        Me.WindowState = FormWindowState.Normal
        Me.Show()
        ShowInTaskbar = True
        NotifyIcon1.Visible = False
    End Sub
    Private Sub GetMachineSettings()
        Dim path As String = Application.StartupPath & "\" & Settings
        Dim reader = File.OpenText(path)

        Dim line As String = Nothing

        Dim lines As Integer = 0

        SettingsContent = reader.ReadLine()

    End Sub
    Private saveSettings As Boolean = False
    Private Sub SaveMachineSettings()
        Using CommLog As New FileStream(ErrorComm, FileMode.Append, FileAccess.Write)
            Using CommWriter As New StreamWriter(CommLog)
                CommWriter.WriteLine(txtMachine.Text & "_" & txtSerial.Text)
            End Using
        End Using
        saveSettings = True
        SettingsContent = txtMachine.Text & "_" & txtSerial.Text
        LoadSettings()
        'CommLog = New FileStream(Settings, FileMode.Create, FileAccess.Write)
        'CommWriter = New StreamWriter(CommLog)
        'CommWriter.WriteLine(txtMachine.Text & "_" & txtSerial.Text)
        'CommWriter.Close()
        'CommLog.Close()
        'saveSettings = True

        'SettingsContent = txtMachine.Text & "_" & txtSerial.Text
        'LoadSettings()
    End Sub

    'Create PortableSettings
    Private Sub LoadSettings()
        GetMachineSettings()
        ' Get the path to the Application Data folder
        'Dim FolderPath As String = GetFolderPath(SpecialFolder.LocalApplicationData) & "\AppSupport\"

        Dim FolderPath As String = Application.StartupPath & "\AppSupport\"
        Dim FileInfo As FileInfo

        If Not Directory.Exists(FolderPath) Then
            'Create Folder
            Directory.CreateDirectory(FolderPath)
        End If

        If saveSettings = True Then
            For Each _file As String In Directory.GetFiles(FolderPath)
                FileInfo = New FileInfo(_file)

                File.Delete(FileInfo.FullName)
                Debug.WriteLine(FileInfo.FullName)
            Next
        End If

        PortableSettingsProvider.SettingsFileName = SettingsContent & ".cs"
        PortableSettingsProvider.SettingsDirectory = FolderPath
    End Sub

    'Method for Creating Communication File

    Public Sub CreateFile(ByVal file As String, ByVal port As String)
        Using CommLog As New FileStream(file, FileMode.Append, FileAccess.Write)
            Using CommWriter As New StreamWriter(CommLog)
                CommWriter.WriteLine("Date Opened: " + System.DateTime.Now.ToString())
                CommWriter.WriteLine("Port: " + port)
                CommWriter.WriteLine("Machine: " + Instrument + vbNewLine)
            End Using
        End Using
        'CommLog = New FileStream(file, FileMode.Append, FileAccess.Write)
        'CommWriter = New StreamWriter(CommLog)

        'CommWriter.WriteLine("Date Opened: " + System.DateTime.Now.ToString())
        'CommWriter.WriteLine("Port: " + port)
        'CommWriter.WriteLine("Machine: " + Instrument + vbNewLine)

        'CommWriter.Close()
        'CommLog.Close()
    End Sub

    'Method for Creating ConnectionString Log File
    Public Sub CreateFile_ConnectionString()

        Dim Server As String = txtServerName.Text
        Dim UID As String = txtRoot.Text
        Dim PWD As String = txtPassword.Text
        Dim DBName As String = txtDBName.Text
        Dim ConnectionString As String = "SERVER = " & Server & "; UID = " & UID & "; PWD = " & PWD & "; DATABASE = " & DBName & ";"

        'Save Connection String to Registry
        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\KeyString\CS 1.0.0", "ConnectionString", ConnectionString)
    End Sub

    Private Sub frmCommunication_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If (MessageBox.Show("Are you sure you want to close the application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes) Then
            End
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub frmSystemTray_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lblVersion.Text = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion

        'List down all available port
        GetComPorts()

        Dim path As String = Application.StartupPath & "\" & Settings
        ' This text is added only once to the file.
        If File.Exists(path) = True Then
            LoadSettings()
            LoadAllSettings()
        End If
    End Sub

    Private Sub LoadAllSettings()
        Try

            'Retrieve saved settings for default use
            With My.Settings
                'Display ComPort and Machine Settings in textbox
                ComPortsBox.Text = .COMPort
                cboBaudrate.Text = .Baudrate '9600
                cboStopBits.Text = .Stopbits '1
                cboDataBits.Text = .Databits '8
                'txtMachine.Text = "PentraC400"
                If .Machine <> "" Then
                    txtMachine.Text = .Machine
                Else
                    txtMachine.Text = ""
                End If
                txtSerial.Text = .SerialNo
                'txtSection.Text = .Section
                cboLocation.Text = .Location

                'Server Setting
                txtServerName.Text = .HostName
                txtRoot.Text = .UID
                txtPassword.Text = .PWD
                txtDBName.Text = .DBName

                'Machine Settings
                Instrument = .Machine
                Serial = .SerialNo
                'Section = .Section

                'For Two Way Process
                If .ReceivingStatus = 1 Then
                    chkReceiving.CheckState = CheckState.Checked
                Else
                    chkReceiving.CheckState = CheckState.Unchecked
                End If
                Me.Text = "Analyzer Driver " & .Machine.ToUpper & "(" & .COMPort & ")"

                'Set ComPort Settings
                Port.PortName = ComPortsBox.Text
                Port.BaudRate = cboBaudrate.Text
                Port.DataBits = cboDataBits.Text
                Port.StopBits = cboStopBits.Text
                'Port.Open()

                'Create text file for TraceComm.log
                CreateFile(TraceComm, ComPortsBox.Text)

                'Create text file for ErrorComm.log
                CreateFile(ErrorComm, ComPortsBox.Text)

                'Timer to check the Port Connection
                connectionCheckerTimer.Interval = 1000
                AddHandler connectionCheckerTimer.Elapsed, AddressOf connectionCheckerTimer_Elapsed

                connectionCheckerTimer.Start()

                'Check if Registry Setting Exist
                If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\KeyString\CS 1.0.0", "ConnectionString", Nothing) Is Nothing Then
                    Exit Sub
                Else
                    'Get Connection String value from Registry
                    Dim TXTConnectionString As String = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\KeyString\CS 1.0.0", "ConnectionString", Nothing)
                    '#########################################################################################################################################################

                    'Update Machine Status
                    Using conn As New MySqlConnection(My.Settings.ConnectionString)
                        conn.Open()
                        Dim hasrow As Object
                        Using cmd As New MySqlCommand()
                            cmd.Connection = conn
                            cmd.CommandText = "SELECT * FROM `machines` WHERE `name` = @name AND `serial_no` = @serial"
                            cmd.CommandType = CommandType.Text
                            cmd.Parameters.AddWithValue("@name", txtMachine.Text)
                            cmd.Parameters.AddWithValue("@serial", txtSerial.Text)
                            hasrow = cmd.ExecuteScalar()
                        End Using

                        If hasrow Is DBNull.Value Then
                            Using cmd As New MySqlCommand()
                                cmd.Connection = conn
                                cmd.CommandText = "UPDATE `machines` SET `status` = 'Connected' WHERE `name` = @name AND `serial_no` = @serial"
                                cmd.CommandType = CommandType.Text
                                cmd.Parameters.AddWithValue("@name", txtMachine.Text)
                                cmd.Parameters.AddWithValue("@serial", txtSerial.Text)
                                cmd.ExecuteNonQuery()
                            End Using
                        End If
                    End Using
                End If

                '###############################################################################################################################################################

            End With

            GetDefaultUnit()

            If Port.IsOpen Then
                lblConnection.Text = "Connection Status: Connected"
            Else
                lblConnection.Text = "Connection Status: Disconnected"
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ErrorLog(ex.ToString)
            Exit Sub
        End Try
    End Sub

    Private Sub IdentifyOrder(ByVal OrderID As String, ByVal PatientID As String, ByVal PatientName As String, ByVal Instrument As String, ByVal SerialNo As String, ByVal MainID As String, ByVal PatientGender As String, ByVal PatientBirthDate As String)
        Dim result As Object
        Using conn As New MySqlConnection(My.Settings.ConnectionString)
            conn.Open()
            Using cmd As New MySqlCommand("SELECT `ctrl_id` FROM `control_setting` WHERE `ctrl_id` LIKE @sample_id", conn)
                cmd.Parameters.AddWithValue("@sample_id", OrderID)
                result = cmd.ExecuteScalar()
            End Using
        End Using

        If result Is Nothing OrElse IsDBNull(result) Then
            ' NOT in control_settings
            DisplayOrder(OrderID, PatientID, PatientName, Instrument, SerialNo, MainID, PatientGender, PatientBirthDate)
        Else
            ' IN control_settings
        End If

    End Sub

    Private Sub DisplayOrder(ByVal OrderID As String, ByVal PatientID As String, ByVal PatientName As String, ByVal Instrument As String, ByVal SerialNo As String, ByVal MainID As String, ByVal PatientGender As String, ByVal PatientBirthDate As String)

        Using conn As New MySqlConnection(My.Settings.ConnectionString)
            conn.Open()
            Using trans As MySqlTransaction = conn.BeginTransaction()
                Try
                    If My.Settings.ReceivingStatus = 1 Then
                        Dim result As Object
                        Using cmd1 As New MySqlCommand("SELECT `main_id` FROM `tmpWorklist` WHERE `main_id` = @mainid and `status` = 'Result Received'", conn, trans)
                            cmd1.Parameters.AddWithValue("@mainid", MainID)
                            result = cmd1.ExecuteScalar()
                        End Using



                        If result Is Nothing OrElse IsDBNull(result) Then
                            'insert tmpworklist
                            Using cmd1 As New MySqlCommand($"INSERT INTO `tmpWorklist` (`sample_id`, `status`, `date`, `time`, `main_id`, `instrument`, `testtype`, `location`, `sub_section`) VALUES
                                                   (@sampleid, @status, @date, @time, @mainid, @instrument, @section, @location, @sub_section)", conn, trans)
                                cmd1.Parameters.AddWithValue("@sampleid", OrderID)
                                cmd1.Parameters.AddWithValue("@status", "Result Received")
                                cmd1.Parameters.AddWithValue("@Date", Now)
                                cmd1.Parameters.AddWithValue("@time", Now.ToLongTimeString())
                                cmd1.Parameters.AddWithValue("@mainid", MainID)
                                cmd1.Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
                                cmd1.Parameters.AddWithValue("@location", cboLocation.Text)
                                cmd1.Parameters.AddWithValue("@section", Section)
                                cmd1.Parameters.AddWithValue("@sub_section", SubSection)
                                cmd1.ExecuteNonQuery()
                            End Using

                            'Insert in specimen_tracking
                            Using cmd1 As New MySqlCommand("INSERT INTO `specimen_tracking` (`sample_id`, `received`, `extracted`, `section`, `sub_section`) VALUES " _
                                                    & "(@mainid, @time_checked_in, @time_checked_in, @section, @sub_section)", conn, trans)
                                cmd1.Parameters.AddWithValue("@mainid", MainID)
                                cmd1.Parameters.AddWithValue("@time_checked_in", Now)
                                cmd1.Parameters.AddWithValue("@section", Section)
                                cmd1.Parameters.AddWithValue("@sub_section", SubSection)
                                cmd1.ExecuteNonQuery()
                            End Using
                        Else

                            'Update tmpworklist
                            Using cmd1 As New MySqlCommand("UPDATE `tmpWorklist` Set `status` = @status, `instrument` = @instrument WHERE main_id = @mainid And testtype = @section And sub_section = @sub_section", conn, trans)
                                cmd1.Parameters.AddWithValue("@status", "Result Received")
                                cmd1.Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
                                cmd1.Parameters.AddWithValue("@mainid", MainID)
                                cmd1.Parameters.AddWithValue("@section", Section)
                                cmd1.Parameters.AddWithValue("@sub_section", SubSection)
                                cmd1.ExecuteNonQuery()
                            End Using
                        End If
                    Else
                        'ReceivingStatus = 0
                        Using cmd1 As New MySqlCommand("UPDATE `tmpWorklist` Set `status` = @status, `instrument` = @instrument WHERE main_id = @mainid And testtype = @section And `sub_section` = @sub_section And `status` In('Processing', 'Result Received')", conn, trans)
                            cmd1.Parameters.AddWithValue("@status", "Result Received")
                            cmd1.Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
                            cmd1.Parameters.AddWithValue("@mainid", MainID)
                            cmd1.Parameters.AddWithValue("@section", Section)
                            cmd1.Parameters.AddWithValue("@sub_section", SubSection)
                            cmd1.ExecuteNonQuery()
                        End Using
                    End If

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    ErrorLog(ex.ToString)
                    MessageBox.Show(ex.Message, "Error Updating Worklist", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub

    Private param_Ctr As Integer
    Private testHasRows As Boolean = False
    Private Sub InsertResultData(ByVal OrderID As String, ByVal ResultName As String, ByVal ResultValue As String, ByVal Instrument As String, ByVal SerialNo As String, ByVal MainID As String)

        Using conn As New MySqlConnection(MyConnectionString)
            conn.Open()
            Using trans As MySqlTransaction = conn.BeginTransaction()
                Try
                    Using cmd As New MySqlCommand("SELECT `main_id` FROM `tmpWorklist` WHERE `sample_id` = @mainid AND `testtype` = @testtype AND `sub_section` = @sub_section AND `status` IN ('Processing','Result Received')", conn, trans)
                        With cmd
                            .Parameters.AddWithValue("@mainid", MainID)
                            .Parameters.AddWithValue("@testtype", Section)
                            .Parameters.AddWithValue("@sub_section", SubSection)

                            Dim hasrow As Object = cmd.ExecuteScalar()

                            If hasrow Is Nothing OrElse hasrow Is DBNull.Value Then
                                'The command returns nothing
                                trans.Rollback()
                                Exit Sub
                            End If

                            'If the command return row, continue to do the sub
                        End With
                    End Using

                    Using cmd1 As New MySqlCommand("SELECT `specimen`, `test_code`, `order_no`, `test_group`, `si_unit`, `conventional_unit`,`section`,`test_name` FROM `specimen` where `channel` = @channel AND `instrument` = @instrument AND `status` = 'Enable'", conn)
                        With cmd1
                            .CommandType = CommandType.Text
                            .Parameters.Clear()
                            .Parameters.AddWithValue("@channel", ResultName)
                            .Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
                        End With

                        Using rdr1 As MySqlDataReader = cmd1.ExecuteReader
                            rdr1.Read()
                            If rdr1.HasRows Then
                                test_name = rdr1("specimen").ToString
                                test_code = rdr1("test_code").ToString
                                order_no = rdr1("order_no").ToString
                                test_group = rdr1("test_group").ToString
                                unit = rdr1("si_unit").ToString
                                unit_conv = rdr1("conventional_unit").ToString
                                Section = rdr1("section").ToString
                                SubSection = rdr1("test_name").ToString

                                testHasRows = True
                            End If
                        End Using
                    End Using

                    If testHasRows = True Then

                        'Dim TestParamMeasurement As String = $"@Measurement_{ResultName.Replace("%", "_P").Replace("-", "_").Replace("#", "_N")}"
                        'Dim TestParamDate As String = $"@date_{ResultName.Replace("%", "_P").Replace("-", "_").Replace("#", "_N")}"

                        'Check for duplicate result if exist save result to `rerun_result` table
                        Using cmd1 As New MySqlCommand("SELECT `test_code`, `sample_id`, `status` FROM `tmpResult` WHERE `test_code` = @test_code AND `sample_id` = @sample_id", conn, trans)
                            With cmd1
                                .Parameters.Clear()
                                .Parameters.AddWithValue("@sample_id", MainID.Trim)
                                .Parameters.AddWithValue("@test_code", test_code)
                            End With

                            Using rdr1 As MySqlDataReader = cmd1.ExecuteReader
                                rdr1.Read()
                                If rdr1.HasRows Then
                                    If rdr1("status") = 0 Then
                                        rdr1.Close()

                                        'QueryStatement &= $"UPDATE `tmpResult` SET `measurement` = {TestParamMeasurement},`date` = {TestParamDate},`instrument` = @instrument,`status` = 1 WHERE `sample_id` = @sample_id AND `test_code` = '{test_code}' AND `status` = 0;"
                                        Using cmd2 As New MySqlCommand("UPDATE `tmpResult` SET `measurement` = @measurement,`date` = @date,`instrument` = @instrument,`status` = 1 WHERE `sample_id` = @sample_id AND `test_code` = @test_code AND `status` = 0", conn, trans)
                                            With cmd2
                                                .Parameters.AddWithValue("@measurement", ResultValue.Trim)
                                                .Parameters.AddWithValue("@date", Now)
                                                .Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
                                                .Parameters.AddWithValue("@sample_id", MainID)
                                                .Parameters.AddWithValue("@test_code", test_code)
                                                .ExecuteNonQuery()
                                            End With
                                        End Using

                                    ElseIf rdr1("status") = 1 Then
                                        rdr1.Close()

                                        'QueryStatement &= $"INSERT INTO `rerun_result` (`universal_id`, `measurement`, `test_code`, `sample_id`, `date`, `patient_id`, `order_no`, `units`, `unit_conv`, `instrument`, `status`, `test_group`, `section`,`sub_section`)
                                        '    VALUES ('{ test_name }', {TestParamMeasurement}, '{ test_code }', @sample_id, {TestParamDate}, @patient_id, '{ order_no }', '{ unit }', '{ unit_conv }', @instrument, '{ Rerun }', '{ test_group }','{Section}', '{ SubSection }');"
                                        Using insertCmd As New MySqlCommand("INSERT INTO rerun_result (universal_id, measurement, test_code, sample_id, date, patient_id, order_no, units, unit_conv, instrument, status, test_group, section, sub_section) VALUES (@test_name, @measurement, @test_code, @sample_id, @date, @patient_id, @order_no, @unit, @unit_conv, @instrument, @status, @test_group, @section, @sub_section)", conn, trans)
                                            With insertCmd
                                                .Parameters.AddWithValue("@test_name", test_name)
                                                .Parameters.AddWithValue("@measurement", ResultValue.Trim)
                                                .Parameters.AddWithValue("@test_code", test_code)
                                                .Parameters.AddWithValue("@sample_id", MainID)
                                                .Parameters.AddWithValue("@date", Now)
                                                .Parameters.AddWithValue("@patient_id", patient_id)
                                                .Parameters.AddWithValue("@order_no", order_no)
                                                .Parameters.AddWithValue("@unit", unit)
                                                .Parameters.AddWithValue("@unit_conv", unit_conv)
                                                .Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
                                                .Parameters.AddWithValue("@status", 1)
                                                .Parameters.AddWithValue("@test_group", test_group)
                                                .Parameters.AddWithValue("@section", Section)
                                                .Parameters.AddWithValue("@sub_section", SubSection)
                                                .ExecuteNonQuery()
                                            End With
                                        End Using
                                    End If
                                Else
                                    rdr1.Close()
                                    If My.Settings.ReceivingStatus = 1 Then
                                        'Save Result
                                        'QueryStatement &= $"INSERT INTO `tmpresult` (`universal_id`, `measurement`, `test_code`, `sample_id`, `date`, `patient_id`, `order_no`, `units`, `unit_conv`, `instrument`, `status`, `test_group`, `section`,`sub_section`)
                                        '    VALUES ('{test_name }', {TestParamMeasurement}, '{ test_code }', @sample_id, {TestParamDate}, @patient_id,' { order_no }', '{ unit }', '{ unit_conv }', @instrument, 1, '{ test_group }', '{Section}','{ SubSection }');"
                                        Using insertCmd As New MySqlCommand("INSERT INTO tmpresult (universal_id, measurement, test_code, sample_id, date, patient_id, order_no, units, unit_conv, instrument, status, test_group, section, sub_section) VALUES (@test_name, @measurement, @test_code, @sample_id, @date, @patient_id, @order_no, @unit, @unit_conv, @instrument, 1, @test_group, @section, @sub_section)", conn, trans)
                                            With insertCmd
                                                .Parameters.AddWithValue("@test_name", test_name)
                                                .Parameters.AddWithValue("@measurement", ResultValue.Trim)
                                                .Parameters.AddWithValue("@test_code", test_code)
                                                .Parameters.AddWithValue("@sample_id", MainID)
                                                .Parameters.AddWithValue("@date", Now)
                                                .Parameters.AddWithValue("@patient_id", patient_id)
                                                .Parameters.AddWithValue("@order_no", order_no)
                                                .Parameters.AddWithValue("@unit", unit)
                                                .Parameters.AddWithValue("@unit_conv", unit_conv)
                                                .Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
                                                .Parameters.AddWithValue("@test_group", test_group)
                                                .Parameters.AddWithValue("@section", Section)
                                                .Parameters.AddWithValue("@sub_section", SubSection)
                                                .ExecuteNonQuery()
                                            End With
                                        End Using
                                    End If
                                End If
                            End Using
                        End Using
                        testHasRows = False
                    End If

                    trans.Commit()
                Catch ex As Exception
                    MessageBox.Show(ex.ToString, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    trans.Rollback()
                    ErrorLog(ex.ToString)
                End Try
            End Using
        End Using

    End Sub


    Private Sub UpdateResultStatus()
        Using conn As New MySqlConnection(My.Settings.ConnectionString)
            conn.Open()
            Using trans As MySqlTransaction = conn.BeginTransaction()
                Try
                    Using cmd As New MySqlCommand()
                        cmd.Connection = conn
                        cmd.Transaction = trans
                        cmd.CommandText = "UPDATE `tmpresultstatus` SET `status` = 'New Result'"
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    ErrorLog(ex.ToString)
                End Try
            End Using
        End Using
        'Connect()
        'rs.Connection = conn
        'rs.CommandText = "UPDATE `tmpresultstatus` SET `status` = 'New Result'"
        'rs.ExecuteNonQuery()
        'Disconnect()
    End Sub

    Private Sub GetDefaultUnit()
        If IO.File.Exists(Application.StartupPath & "\ConnectionString.log") Then
            Using conn As New MySqlConnection(My.Settings.ConnectionString)
                conn.Open()
                Using cmd As New MySqlCommand()
                    cmd.Connection = conn
                    cmd.CommandText = "SELECT `unitformat` FROM `default_unit` WHERE `instrument` = @machine"
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@machine", Me.txtMachine.Text)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            DefaultUnit = reader(0)
                        End If
                    End Using
                End Using
            End Using
        End If
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            NotifyIcon1.Visible = True
            NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info
            NotifyIcon1.BalloonTipTitle = txtMachine.Text & " (" & ComPortsBox.Text & ") " & "Communication"
            NotifyIcon1.BalloonTipText = "Do not close this application!"
            'NotifyIcon1.ShowBalloonTip(5000)
            'Me.Hide()
            ShowInTaskbar = False
        End If
    End Sub

    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.DoubleClick
        Me.WindowState = FormWindowState.Normal
        Me.Show()
        ShowInTaskbar = True
        NotifyIcon1.Visible = False
    End Sub

    Private Sub GetComPorts()
        For Each COMString As String In My.Computer.Ports.SerialPortNames ' Load all available COM ports.
            ComPortsBox.Properties.Items.Add(COMString)
        Next
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If cboBaudrate.Text = "" Or cboDataBits.Text = "" Or cboStopBits.Text = "" Or
            txtSerial.Text = "" Or txtMachine.Text = "" Or txtDBName.Text = "" Or txtRoot.Text = "" Or txtSerial.Text = "" Or
            txtServerName.Text = "" Then
            MessageBox.Show("Please fill all required parameteres.", "Null Value", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        Try
            SaveMachineSettings()

            With My.Settings
                'Save To Configuration Settings
                .COMPort = ComPortsBox.Text
                .Baudrate = cboBaudrate.Text
                .Stopbits = cboStopBits.Text
                .Databits = cboDataBits.Text
                .Machine = txtMachine.Text
                .SerialNo = txtSerial.Text
                '.Section = txtSection.Text

                .HostName = txtServerName.Text
                .UID = txtRoot.Text
                .PWD = txtPassword.Text
                .DBName = txtDBName.Text
                .Location = cboLocation.Text

                If Me.chkReceiving.CheckState = CheckState.Checked Then
                    .ReceivingStatus = 1
                Else
                    .ReceivingStatus = 0
                End If

                .ConnectionString = "SERVER = " & .HostName & "; DATABASE = " & .DBName & "; UID = " & .UID & "; PWD = " & .PWD & ";"
                .Save()
                '###########################################################################################################################################################

                'Create Registry
                CreateFile_ConnectionString()
                '##########################################################################################################################################################

                'Get Connection String value from Registry
                Dim TXTConnectionString As String = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\KeyString\CS 1.0.0", "ConnectionString", Nothing)
                '#########################################################################################################################################################

                'Get Executable Path for saving
                Dim path As String = Application.ExecutablePath

                Using conn As New MySqlConnection(My.Settings.ConnectionString)
                    Dim hasrow As Boolean = False
                    conn.Open()
                    Using chkmachine As New MySqlCommand("SELECT * FROM `machines` WHERE `name` = @machine AND `serial_no` = @serial LIMIT 1", conn)
                        chkmachine.Parameters.AddWithValue("@machine", txtMachine.Text)
                        chkmachine.Parameters.AddWithValue("@serial", txtSerial.Text)

                        hasrow = chkmachine.ExecuteScalar() IsNot Nothing
                    End Using

                    If hasrow Then
                        'Update existing machine record
                        Using updtmachine As New MySqlCommand("UPDATE machines SET
                                                                    name = @name,
                                                                    test_name = @testname,
                                                                    serial_no = @serialno,
                                                                    com_port = @comport,
                                                                    baud_rate = @baudrate,
                                                                    status = @status,
                                                                    location = @location
                                                                    WHERE name = @name AND serial_no = @serialno", conn)
                            With updtmachine
                                .Parameters.AddWithValue("@name", txtMachine.Text)
                                .Parameters.AddWithValue("@testname", "")
                                .Parameters.AddWithValue("@serialno", txtSerial.Text)
                                .Parameters.AddWithValue("@comport", ComPortsBox.Text)
                                .Parameters.AddWithValue("@baudrate", cboBaudrate.Text)
                                .Parameters.AddWithValue("@status", "Disconnected")
                                .Parameters.AddWithValue("@location", path)
                                .ExecuteNonQuery()
                            End With
                        End Using
                    Else
                        'Insert new machione to machine table
                        Using insrtamachine As New MySqlCommand("INSERT INTO machines
                                                                         (name, test_name, serial_no, com_port, baud_rate, status, location)
                                                                         VALUES
                                                                         (@name, @testname, @serialno, @comport, @baudrate, @status, @location)", conn)
                            With insrtamachine
                                .Parameters.AddWithValue("@name", txtMachine.Text)
                                .Parameters.AddWithValue("@testname", "")
                                .Parameters.AddWithValue("@serialno", txtSerial.Text)
                                .Parameters.AddWithValue("@comport", ComPortsBox.Text)
                                .Parameters.AddWithValue("@baudrate", cboBaudrate.Text)
                                .Parameters.AddWithValue("@status", "Disconnected")
                                .Parameters.AddWithValue("@location", path)
                                .ExecuteNonQuery()
                            End With
                        End Using
                    End If
                End Using
                MessageBox.Show("Settings saved! " & vbNewLine & "Please restart the application to take effect.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)
                '#############################################################################################################################################################
                End
            End With
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    Private Sub connectionCheckerTimer_Elapsed(sender As Object, e As ElapsedEventArgs)
        Try
            If Port.IsOpen = False Then
                Port.Open()
                Me.Invoke(Sub() lblConnection.Text = "Connection Status: Connected")
                ErrorLog("Port Connection Restored")
            End If
        Catch ex As Exception
            Port.Close()
            Me.Invoke(Sub() lblConnection.Text = "Connection Status: Disconnected")
            ErrorLog("Port Disconnected")
        End Try
    End Sub

    Private Sub ErrorLog(ByVal logTxt As String)

        'Write file to Error.Log
        Using CommLog As New FileStream(ErrorComm, FileMode.Append, FileAccess.Write)
            Using CommWriter As New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Error] : " & logTxt)
            End Using
        End Using
        'CommLog = New FileStream(ErrorComm, FileMode.Append, FileAccess.Write)
        'CommWriter = New StreamWriter(CommLog)
        'CommWriter.WriteLine(System.DateTime.Now & " [Error] : " & logTxt)
        'CommWriter.Close()
        'CommLog.Close()
    End Sub

    'Start reading from CommPort
    Private Sub tmConnect_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmConnect.Tick
        Try
            If Port.IsOpen Then


                ReadENQData()


                '#############
                'to connect
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString(), "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            ErrorLog(ex.ToString & vbNewLine)
            connectionCheckerTimer.Stop()
            connectionCheckerTimer.Start()
            TimedOut = 0
        End Try
    End Sub

    Private Function returnIncomingMessage()
        Dim incomingMessage As String = ""
        If Port.IsOpen Then


            incomingMessage = Port.ReadExisting

        End If

        Return incomingMessage
    End Function


    Private MessageIncoming As String
    Private Async Function ReadIncomingData() As Task
        Await Task.Run(Sub()
                           MessageIncoming = returnIncomingMessage()
                       End Sub)
    End Function

    Private Async Function WriteToComLog(data As String) As Task
        Await Task.Run(Sub()
                           Try
                               Using log As New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                                   Using writer As New StreamWriter(log)
                                       writer.WriteLine("******************************Start-of-Communication*******************************" & vbNewLine)
                                       writer.WriteLine(System.DateTime.Now & " [Machine] : " & data & vbNewLine)
                                       writer.WriteLine("******************************End-of-Communication*******************************" & vbNewLine)
                                   End Using
                               End Using
                           Catch ex As Exception
                               ErrorLog("WriteToComLog Error: " & ex.ToString())
                           End Try
                       End Sub)
    End Function

    Private Async Sub ReadENQData()

        Dim buffer As String = ""

        If Port.IsOpen Then

            Try
                Dim incoming As String = ""
                Await ReadIncomingData()

                incoming = MessageIncoming

                If incoming = "" Then
                    Exit Sub
                End If
                'MessageBox.Show($"after:: {incoming}")
                buffer &= incoming
                ReceivedText.Text = ""
                If Not (buffer = "") Then
                    tmConnect.Stop()
                End If

                Await WriteToComLog(buffer)

                If Not (buffer = "") Then
                    tmReadData.Stop()
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & "******************************Start-of-Communication*******************************" & vbNewLine & vbNewLine))
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & buffer & vbNewLine & vbNewLine))
                    Me.Invoke(Sub() AppendText("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine))
                End If

                ' Look for the start marker

                Dim rIndex As Integer = buffer.IndexOf("r")

                If rIndex >= 0 Then
                    ' Check if we have enough data for a full packet (70 chars)
                    'If buffer.Length - rIndex < 70 Then
                    '    Exit if ' Wait for more data
                    'End If

                    Dim PatientGender As String = ""
                    Dim PatientBirthDate As String = ""

                    Dim packet As String = buffer.Substring(rIndex, buffer.Length - rIndex)

                    ' Validate start marker
                    If packet(0) = "r"c Then
                        ParsePacket(packet)
                    End If

                    ' Remove processed packet from buffer
                    'buffer = buffer.Substring(rIndex + 70)
                    'rIndex = buffer.IndexOf("r")
                End If

            Catch ex As Exception
                ErrorLog("CBS500 Parse Error: " & ex.ToString())
            End Try

            Threading.Thread.Sleep(50)
        End If

        tmConnect.Start()
    End Sub
    ''End of Reading <ENQ>

    'Start reading packets after receiving <ENQ>
    Private Sub tmReadData_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmReadData.Tick
        Try
            If Port.IsOpen Then
                '    'Port.Open()
                'Else   
                '    Port.Open()
                '    Main_ReadDataRS232()
            End If

        Catch ex As Exception
            ErrorLog(ex.ToString())
        End Try
    End Sub
    'End of reading packets

    Private Sub ParsePacket(packet As String)
        Try
            Dim m As Match = Regex.Match(packet, "r(\d+)")
            If Not m.Success Then Exit Sub
            MainID = m.Groups(1).Value
            OrderID = MainID
            IdentifyOrder(OrderID, "", "", Instrument, My.Settings.SerialNo, MainID, "", "")
            Dim matches As MatchCollection = Regex.Matches(packet.Substring(16), "-?\d+\.?\d*|\.\.\.\.\.\.")

            Dim keys() As String = {"K", "Na", "Cl", "iCa", "nCa", "TCa", "pH"}
            Dim fields As New Dictionary(Of String, String)
            For i As Integer = 0 To keys.Length - 1
                fields(keys(i)) = If(i < matches.Count, matches(i).Value, "0")
            Next

            For Each kvp As KeyValuePair(Of String, String) In fields
                ResultName = kvp.Key
                ResultValue = kvp.Value
                InsertResultData(OrderID, ResultName, ResultValue, Instrument, My.Settings.SerialNo, MainID)
            Next

        Catch ex As Exception
            ErrorLog("CBS500 Parse Error: " & ex.ToString())
        End Try

    End Sub

    ''Display received packets to richtextbox
    Private Sub AppendText(ByVal str As String)
        ReceivedText.AppendText(str)
    End Sub
    'End of reading packets

    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        frmLogin.Show()
    End Sub

    Public Sub ExitMessage()
        Using CommLog As New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
            Using CommWriter As New StreamWriter(CommLog)
                CommWriter.WriteLine("Application Closed: " + System.DateTime.Now.ToString())
                CommWriter.WriteLine("Port: " + ComPortsBox.Text)
                CommWriter.WriteLine("Machine: " + Instrument + vbNewLine)
            End Using
        End Using
    End Sub

    Private Sub btnDisconnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDisconnect.Click
        If Port.IsOpen Then
            Port.Close()
            ExitMessage()
            lblConnection.Text = "Connection Status: Disconnected"


            'Get Connection String value from Registry
            Dim TXTConnectionString As String = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\KeyString\CS 1.0.0", "ConnectionString", Nothing)
            '#########################################################################################################################################################

            'Update Machine Status
            Using conn As New MySqlConnection(My.Settings.ConnectionString)
                conn.Open()
                Dim hasrow As Object
                Using cmd As New MySqlCommand()
                    cmd.Connection = conn
                    cmd.CommandText = "SELECT * FROM `machines` WHERE `name` = @name AND `serial_no` = @serial"
                    cmd.CommandType = CommandType.Text
                    cmd.Parameters.AddWithValue("@name", txtMachine.Text)
                    cmd.Parameters.AddWithValue("@serial", txtSerial.Text)
                    hasrow = cmd.ExecuteScalar()
                End Using

                If hasrow IsNot DBNull.Value Then
                    Using cmd As New MySqlCommand()
                        cmd.Connection = conn
                        cmd.CommandText = "UPDATE `machines` SET `status` = 'Disconnected' WHERE `name` = @name AND `serial_no` = @serial"
                        cmd.CommandType = CommandType.Text
                        cmd.Parameters.AddWithValue("@name", txtMachine.Text)
                        cmd.Parameters.AddWithValue("@serial", txtSerial.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            End Using

            '############################################################################################################################################################

        End If
    End Sub

    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        If Port.IsOpen Then
            MessageBox.Show("Port already connected.", "Port Connection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        Else
            Port.Open()
            CreateFile(TraceComm, ComPortsBox.Text)
            lblConnection.Text = "Connection Status: Connected"

            'Get Connection String value from Registry
            Dim TXTConnectionString As String = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\KeyString\CS 1.0.0", "ConnectionString", Nothing)
            '#########################################################################################################################################################

            'Update Machine Status
            Using conn As New MySqlConnection(My.Settings.ConnectionString)
                conn.Open()
                Dim hasrow As Object
                Using cmd As New MySqlCommand()
                    cmd.Connection = conn
                    cmd.CommandText = "SELECT * FROM `machines` WHERE `name` = @name AND `serial_no` = @serial"
                    cmd.CommandType = CommandType.Text
                    cmd.Parameters.AddWithValue("@name", txtMachine.Text)
                    cmd.Parameters.AddWithValue("@serial", txtSerial.Text)
                    hasrow = cmd.ExecuteScalar()
                End Using

                If hasrow IsNot DBNull.Value Then
                    Using cmd As New MySqlCommand()
                        cmd.Connection = conn
                        cmd.CommandText = "UPDATE `machines` SET `status` = 'Connected' WHERE `name` = @name AND `serial_no` = @serial"
                        cmd.CommandType = CommandType.Text
                        cmd.Parameters.AddWithValue("@name", txtMachine.Text)
                        cmd.Parameters.AddWithValue("@serial", txtSerial.Text)
                        cmd.ExecuteNonQuery()
                    End Using
                End If
            End Using
            '############################################################################################################################################################
        End If
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Me.ReceivedText.Text = ""
    End Sub

    'Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
    '    Port.Write(EOT)
    '    Thread.Sleep(1000)
    'End Sub

End Class