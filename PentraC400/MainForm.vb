Imports System.Drawing.Imaging
Imports System.Drawing.Printing
Imports System.Environment
Imports System.IO
Imports System.IO.Ports
Imports System.Text
Imports System.Threading
Imports System.Timers
Imports System.Xml

Imports Bluegrams.Application
Imports MySql.Data.MySqlClient

Public Class MainForm

#Region "Variables"
    'ASCII Values
    Public SOH As String = Char.ConvertFromUtf32(1)
    Public STX As String = Char.ConvertFromUtf32(2)
    Public ETX As String = Char.ConvertFromUtf32(3)
    Public EOT As String = Char.ConvertFromUtf32(4)
    Public ENQ As String = Char.ConvertFromUtf32(5)
    Public ACK As String = Char.ConvertFromUtf32(6)
    Public NAK As String = Char.ConvertFromUtf32(21)
    Public ETB As String = Char.ConvertFromUtf32(23)
    Public LF As String = Char.ConvertFromUtf32(10)
    Public CR As String = Char.ConvertFromUtf32(13)
    Public DLE As String = Char.ConvertFromUtf32(16)
    Public CapsEnye As String = Char.ConvertFromUtf32(Convert.ToInt32("D1", 16))
    Public LowEnye As String = Char.ConvertFromUtf32(Convert.ToInt32("F1", 16))
    'ENd ASCII

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
    Dim Section As String
    Dim SubSection As String
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
                cboBaudrate.Text = .Baudrate
                cboStopBits.Text = .Stopbits
                cboDataBits.Text = .Databits
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
                Port.DataBits = 8
                Port.StopBits = 1
                Port.Open()

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
                    'conn.ConnectionString = TXTConnectionString
                    'conn.Open()
                    'rs.Connection = conn
                    'rs.CommandText = "SELECT * FROM `machines` WHERE `name` = '" & txtMachine.Text & "' AND `serial_no` = '" & txtSerial.Text & "'"
                    'reader = rs.ExecuteReader
                    'reader.Read()
                    'If reader.HasRows Then
                    '    conn.Close()
                    '    conn.ConnectionString = TXTConnectionString
                    '    conn.Open()
                    '    rs.Connection = conn
                    '    rs.CommandText = "UPDATE `machines` SET `status` = 'Connected' WHERE `name` = '" & txtMachine.Text & "' AND `serial_no` = '" & txtSerial.Text & "'"
                    '    rs.ExecuteNonQuery()
                    '    conn.Close()
                    'End If
                    'conn.Close()
                    '############################################################################################################################################################
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
            Exit Sub
        End Try
    End Sub

    Private Sub Acknowledge(ByVal SP As SerialPort)
        SP.Write(ACK)
    End Sub

    Private Sub Enquery(ByVal SP As SerialPort)
        SP.Write(ENQ)
    End Sub

    Private Sub EndTrasmission(ByVal SP As SerialPort)
        SP.Write(EOT)
    End Sub

    Private Sub ConnectionTimedOut(ByVal SP As SerialPort)
        SP.Write(NAK)
    End Sub

    Private Sub StartLineFeed(ByVal SP As SerialPort, ByVal Data As String)
        SP.Write(Data)
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

        'rs.Parameters.Clear()
        'rs.Parameters.AddWithValue("@sample_id", OrderID)
        'rs.Parameters.AddWithValue("@mainID", OrderID)

        'Connect()
        'rs.Connection = conn
        'rs.CommandText = "mple"
        'reader = rs.ExecuteReader
        'reader.Read()
        'If reader.HasRows Then
        '    Disconnect()
        'Else
        '    Disconnect()
        '    DisplayOrder(OrderID, PatientID, PatientName, Instrument, SerialNo, MainID, PatientGender, PatientBirthDate)
        'End If
        'Disconnect()
    End Sub

    Private Sub DisplayOrder(ByVal OrderID As String, ByVal PatientID As String, ByVal PatientName As String, ByVal Instrument As String, ByVal SerialNo As String, ByVal MainID As String, ByVal PatientGender As String, ByVal PatientBirthDate As String)

        Using conn As New MySqlConnection(My.Settings.ConnectionString)
            conn.Open()
            Using trans As MySqlTransaction = conn.BeginTransaction()
                Try
                    If My.Settings.ReceivingStatus = 1 Then
                        Dim result As Object
                        Using cmd1 As New MySqlCommand("SELECT `main_id` FROM `tmpWorklist` WHERE `main_id` = @mainid", conn, trans)
                            cmd1.Parameters.AddWithValue("@mainid", MainID)
                            result = cmd1.ExecuteScalar()
                        End Using

                        If result Is Nothing OrElse IsDBNull(result) Then
                            'Insert in tmpworklist
                            Using cmd1 As New MySqlCommand("INSERT INTO `tmpWorklist` (`sample_id`, `patient_id`, `patient_name`, `sex`, `bdate`, `status`, `date`, `time`, `main_id`, `instrument`, `testtype`, `location`,`sub_section`) VALUES " _
                                                   & "(@sampleid, @patient_id, @name, @sex, @bdate, @status, @date, @time, @mainid, @instrument, @section, @location, @sub_section)", conn, trans)
                                cmd1.Parameters.AddWithValue("@sampleid", OrderID)
                                cmd1.Parameters.AddWithValue("@patient_id", PatientID)
                                cmd1.Parameters.AddWithValue("@name", PatientName)
                                cmd1.Parameters.AddWithValue("@sex", PatientGender)
                                cmd1.Parameters.AddWithValue("@bdate", PatientBirthDate)
                                cmd1.Parameters.AddWithValue("@status", "Result Received")
                                cmd1.Parameters.AddWithValue("@date", Now)
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
                            Using cmd1 As New MySqlCommand("UPDATE `tmpWorklist` SET `status` = @status, `instrument` = @instrument WHERE main_id = @mainid and testtype = @section AND sub_section = @sub_section", conn, trans)
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
                        Using cmd1 As New MySqlCommand("UPDATE `tmpWorklist` SET `status` = @status, `instrument` = @instrument WHERE main_id = @mainid AND testtype = @section AND `sub_section` = @sub_section AND `status` IN('Processing', 'Result Received')", conn, trans)
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

        ''LoadSampleID()

        'rs.Parameters.Clear()
        'rs.Parameters.AddWithValue("@sampleid", OrderID)
        'rs.Parameters.AddWithValue("@mainid", MainID)
        'rs.Parameters.AddWithValue("@patient_id", PatientID)
        'rs.Parameters.AddWithValue("@name", PatientName)
        'rs.Parameters.AddWithValue("@sex", PatientGender)
        'rs.Parameters.AddWithValue("@bdate", PatientBirthDate)
        'rs.Parameters.AddWithValue("@date", Now)
        'rs.Parameters.AddWithValue("@time", Now.ToLongTimeString())
        'rs.Parameters.AddWithValue("@status", "Result Received")
        'rs.Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
        'rs.Parameters.AddWithValue("@section", Section)
        'rs.Parameters.AddWithValue("@sub_section", SubSection)
        'rs.Parameters.AddWithValue("@location", cboLocation.Text)
        'rs.Parameters.AddWithValue("@time_checked_in", Now)

        'If My.Settings.ReceivingStatus = 1 Then
        '    'Save Order to Worklist
        '    Connect()
        '    rs.Connection = conn
        '    rs.CommandText = "SELECT `main_id` FROM `tmpWorklist` WHERE `main_id` = @mainid"
        '    reader = rs.ExecuteReader
        '    reader.Read()
        '    If reader.HasRows Then
        '        Disconnect()
        '        'Update Worklist
        '        UpdateRecordwthoutMSG("UPDATE `tmpWorklist` SET `status` = @status, `instrument` = @instrument WHERE main_id = @mainid AND `testtype` = @section AND `status` IN ('Processing','Checked-In') AND `sub_section` = @sub_section;")
        '    Else
        '        Disconnect()
        '        'Save Order to Worklist
        '        SaveRecordwthoutMSG("INSERT INTO `tmpWorklist` (`sample_id`, `patient_id`, `patient_name`, `sex`, `bdate`, `status`, `date`, `time`, `main_id`, `instrument`, `testtype`, `sub_section`, `location`) VALUES " _
        '                & "(" _
        '                & "@sampleid," _
        '                & "@patient_id," _
        '                & "@name," _
        '                & "@sex," _
        '                & "@bdate," _
        '                & "@status," _
        '                & "@date," _
        '                & "@time," _
        '                & "@mainid," _
        '                & "@instrument," _
        '                & "@section," _
        '                & "@sub_section," _
        '                & "@location" _
        '                & ")"
        '                )

        '        SaveRecordwthoutMSG("INSERT INTO `specimen_tracking` (`sample_id`, `received`, `extracted`, `section`, `sub_section`) VALUES " _
        '                & "(" _
        '                & "@mainid," _
        '                & "@time_checked_in," _
        '                & "@time_checked_in," _
        '                & "@section," _
        '                & "@sub_section" _
        '                & ")"
        '                )
        '    End If
        '    Disconnect()
        'Else
        '    Disconnect()
        '    UpdateRecordwthoutMSG("UPDATE `tmpWorklist` SET `status` = @status, `instrument` = @instrument WHERE main_id = @mainid AND `testtype` = @section AND `sub_section` = @sub_section;")
        'End If

    End Sub

    'Private Sub DisplayResult(ByVal OrderID As String, ByVal ResultName As String, ByVal ResultValue As String, ByVal Instrument As String, ByVal SerialNo As String, ByVal MainID As String, DateReceived As Date)
    '    rs.Parameters.Clear()
    '    rs.Parameters.AddWithValue("@sample_id", OrderID)

    '    Connect()
    '    rs.Connection = conn
    '    rs.CommandText = "SELECT `ctrl_id` FROM `control_setting` WHERE `ctrl_id` = @sample_id"
    '    reader = rs.ExecuteReader
    '    reader.Read()
    '    If reader.HasRows Then
    '        Disconnect()
    '        InsertControl(OrderID, ResultName, ResultValue, Instrument, SerialNo, DateReceived)
    '    Else
    '        Disconnect()
    '        InsertResultData(OrderID, ResultName, ResultValue, Instrument, SerialNo, MainID)
    '    End If
    '    Disconnect()
    'End Sub
    Private param_Ctr As Integer
    Private testHasRows As Boolean = False
    Private Sub InsertResultData(ByVal OrderID As String, ByVal ResultName As String, ByVal ResultValue As String, ByVal Instrument As String, ByVal SerialNo As String, ByVal MainID As String)

        Using conn As New MySqlConnection(MyConnectionString)
            conn.Open()
            Using trans As MySqlTransaction = conn.BeginTransaction()
                Try
                    Using cmd As New MySqlCommand("SELECT `main_id` FROM `tmpWorklist` WHERE `sample_id` = @mainid AND `status` IN ('Processing','Result Received')", conn)
                        With cmd
                            .Parameters.AddWithValue("@mainid", MainID)

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
        '###########################################
        'rs.Parameters.Clear()
        'rs.Parameters.AddWithValue("@UniversalID", ResultName)
        'rs.Parameters.AddWithValue("@Measurement", ResultValue.Trim)
        'rs.Parameters.AddWithValue("@sample_id", MainID)
        'rs.Parameters.AddWithValue("@patient_id", PatientID)
        'rs.Parameters.AddWithValue("@channel", ResultName)
        'rs.Parameters.AddWithValue("@date", Now)
        'rs.Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
        ''rs.Parameters.AddWithValue("@section", "Chemistry")
        ''rs.Parameters.AddWithValue("@sub_section", "Clinical Chemistry")

        ''MessageBox.Show($"Channel: {ResultName} Result: {ResultValue} Sample id: {MainID} Patient ID: {PatientID} Date : {Now} Instrument: {Instrument & "_" & SerialNo}")
        ''Test names and units for SI Unit
        'Connect()
        'rs.Connection = conn
        'rs.CommandText = "SELECT `specimen`, `test_code`, `order_no`, `test_group`, `si_unit`, `conventional_unit`,`section`,`test_name` FROM `specimen` where `channel` = @channel AND `instrument` = @instrument AND `status` = 'Enable'"
        'reader = rs.ExecuteReader
        'reader.Read()
        'If reader.HasRows Then
        '    test_name = reader(0).ToString
        '    test_code = reader(1).ToString
        '    order_no = reader(2).ToString
        '    test_group = reader(3).ToString
        '    unit = reader(4).ToString
        '    unit_conv = reader(5).ToString
        '    Section = reader(6).ToString
        '    SubSection = reader(7).ToString
        '    Disconnect()

        '    'Check for duplicate result if exist save result to `rerun_result` table
        '    Connect()
        '    rs.Connection = conn
        '    rs.CommandText = "SELECT `test_code`, `sample_id`, `status` FROM `tmpresult` WHERE `test_code` = '" & test_code & "' AND `sample_id` = @sample_id"
        '    reader = rs.ExecuteReader
        '    reader.Read()
        '    If reader.HasRows Then
        '        If reader(2) = 0 Then
        '            Disconnect()
        '            UpdateRecordwthoutMSG("UPDATE `tmpResult` SET " _
        '                    & "`measurement` = @Measurement," _
        '                    & "`date` = @date," _
        '                    & "`instrument` = @instrument," _
        '                    & "`status` = 1" _
        '                    & " WHERE `sample_id` = @sample_id AND `test_code` = '" & test_code & "' AND `status` = 0"
        '                    )

        '        ElseIf reader(2) = 1 Then
        '            Disconnect()
        '            'Save Result
        '            SaveRecordwthoutMSG("INSERT INTO `rerun_result` (`universal_id`, `measurement`, `test_code`, `sample_id`, `date`, `patient_id`, `order_no`, `units`, `unit_conv`, `instrument`, `status`, `test_group`, `section`, `sub_section`) VALUES " _
        '                & "(" _
        '                & "'" & test_name & "', @Measurement, '" & test_code & "', @sample_id, @date, @patient_id, '" & order_no & "', '" & unit & "', '" & unit_conv & "', @instrument, 1, '" & test_group & "', '" & Section & "', '" & SubSection & "'" _
        '                & ")"
        '                )
        '        End If
        '        'MessageBox.Show($"Ordered {test_code} Sample ID: {MainID}")
        '        'MessageBox.Show($"Ordered Channel: {ResultName} Result: {ResultValue} Sample id: {MainID} Patient ID: {PatientID} Date : {Now} Instrument: {Instrument & "_" & SerialNo}")
        '    Else
        '        Disconnect()
        '        If My.Settings.ReceivingStatus = 1 Then
        '            'Save Result
        '            SaveRecordwthoutMSG("INSERT INTO `tmpresult` (`universal_id`, `measurement`, `test_code`, `sample_id`, `date`, `patient_id`, `order_no`, `units`, `unit_conv`, `instrument`, `status`, `test_group`, `section`, `sub_section`) VALUES " _
        '                & "(" _
        '                & "'" & test_name & "', @Measurement, '" & test_code & "', @sample_id, @date, @patient_id, '" & order_no & "', '" & unit & "', '" & unit_conv & "', @instrument, 1, '" & test_group & "',  '" & Section & "', '" & SubSection & "'" _
        '                & ")"
        '                )
        '            'MessageBox.Show($"Not Ordered Channel: {ResultName} Result: {ResultValue} Sample id: {MainID} Patient ID: {PatientID} Date : {Now} Instrument: {Instrument & "_" & SerialNo}")
        '        End If
        '        'MessageBox.Show($"Not Ordered {test_code} Sample ID: {MainID}")
        '    End If
        '    Disconnect()

        'End If
        'Disconnect()

    End Sub

    Private Sub InsertControl(ByVal ControlID As String, ByVal ResultName As String, ByVal ResultValue As String, ByVal Instrument As String, ByVal SerialNo As String, RunDate As Date)
        'rs.Parameters.Clear()
        'rs.Parameters.AddWithValue("@UniversalID", ResultName)
        'rs.Parameters.AddWithValue("@Measurement", ResultValue)
        'rs.Parameters.AddWithValue("@sample_id", ControlID)
        'rs.Parameters.AddWithValue("@patient_id", PatientID)
        'rs.Parameters.AddWithValue("@channel", ResultName)
        ''rs.Parameters.AddWithValue("@date", Now)
        ''rs.Parameters.AddWithValue("@month", Now.Month)
        ''rs.Parameters.AddWithValue("@year", Now.Year)
        'rs.Parameters.AddWithValue("@date", RunDate)
        'rs.Parameters.AddWithValue("@month", RunDate.Month)
        'rs.Parameters.AddWithValue("@year", RunDate.Year)
        'rs.Parameters.AddWithValue("@instrument", Instrument.Trim & "_" & SerialNo.Trim)
        ''rs.Parameters.AddWithValue("@section", Section)
        'rs.Parameters.AddWithValue("@lotNo", LotNo)

        ''Test Names
        'Connect()
        'rs.Connection = conn
        'rs.CommandText = "SELECT `specimen`, `test_code`, `si_unit`,`section`,`test_name` FROM `specimen` where `channel` = @channel AND `instrument` = @instrument"
        'reader = rs.ExecuteReader
        'reader.Read()
        'If reader.HasRows Then
        '    test_name = reader(0).ToString
        '    test_code = reader(1).ToString
        '    unit = reader(2).ToString
        '    Section = reader(3).ToString
        '    SubSection = reader(4).ToString
        'Else
        '    test_name = ""
        '    test_code = ""
        '    unit = ""
        'End If
        'Disconnect()

        'SaveRecordwthoutMSG("INSERT INTO `control_result` (`universal_id`, `measurement`, `test_code`, `sample_id`, `date`, `month`, `year`, `instrument`, `test_type`, `unit`,`lot_no`) VALUES " _
        '        & "(" _
        '        & "'" & test_name & "'," _
        '        & "@Measurement," _
        '        & "'" & test_code & "'," _
        '        & "@sample_id," _
        '        & "@date," _
        '        & "@month," _
        '        & "@year," _
        '        & "@instrument," _
        '        & "'" & Section & "'," _
        '        & "'" & unit & "'," _
        '        & "@lotNo" _
        '        & ")"
        '        )

        Using conn As New MySqlConnection(My.Settings.ConnectionString)
            conn.Open()
            Using trans As MySqlTransaction = conn.BeginTransaction()
                Try
                    Using cmd As New MySqlCommand()
                        cmd.Connection = conn
                        cmd.CommandText = "SELECT `specimen`, `test_code`, `si_unit`,`section`,`test_name` FROM `specimen` where `channel` = @channel AND `instrument` = @instrument"
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@channel", ResultName)
                        cmd.Parameters.AddWithValue("@instrument", Instrument.Trim & "_" & SerialNo)

                        Using reader As MySqlDataReader = cmd.ExecuteReader()
                            If reader.Read() Then
                                test_name = reader(0).ToString
                                test_code = reader(1).ToString
                                unit = reader(2).ToString
                                Section = reader(3).ToString
                                SubSection = reader(4).ToString
                            Else
                                test_name = ""
                                test_code = ""
                                unit = ""
                            End If
                        End Using
                    End Using

                    Using cmd As New MySqlCommand()
                        cmd.Connection = conn
                        cmd.Transaction = trans
                        cmd.CommandText = "INSERT INTO `control_result` (`universal_id`, `measurement`, `test_code`, `sample_id`, `date`, `month`, `year`, `instrument`, `test_type`, `unit`,`lot_no`) VALUES " _
                & "( @test_name, @Measurement, @test_code, @sample_id, @date, @month, @year, @instrument, @section, @unit, @lotNo)"
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@test_name", test_name)
                        cmd.Parameters.AddWithValue("@Measurement", ResultValue)
                        cmd.Parameters.AddWithValue("@test_code", test_code)
                        cmd.Parameters.AddWithValue("@sample_id", ControlID)
                        cmd.Parameters.AddWithValue("@date", RunDate)
                        cmd.Parameters.AddWithValue("@month", RunDate.Month)
                        cmd.Parameters.AddWithValue("@year", RunDate.Year)
                        cmd.Parameters.AddWithValue("@instrument", Instrument.Trim & "_" & SerialNo)
                        cmd.Parameters.AddWithValue("@section", Section)
                        cmd.Parameters.AddWithValue("@unit", unit)
                        cmd.Parameters.AddWithValue("@LOTNo", LotNo)

                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()
                Catch ex As Exception
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
            'Connect()
            'rs.Connection = conn
            'rs.CommandText = "SELECT `unitformat` FROM `default_unit` WHERE `instrument` = '" & Me.txtMachine.Text & "'"
            'reader = rs.ExecuteReader
            'reader.Read()
            'If reader.HasRows Then
            '    DefaultUnit = reader(0)
            'End If
            'Disconnect()

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
                ''Parameteres for table machines
                'rs.Parameters.Clear()
                'rs.Parameters.AddWithValue("@path", path)

                'conn.ConnectionString = TXTConnectionString
                'conn.Open()
                'rs.Connection = conn
                'rs.CommandText = "SELECT * FROM `machines` WHERE `name` = '" & txtMachine.Text & "' AND `serial_no` = '" & txtSerial.Text & "'"
                'reader = rs.ExecuteReader
                'reader.Read()
                'If reader.HasRows Then
                '    conn.Close()
                '    conn.ConnectionString = TXTConnectionString
                '    conn.Open()
                '    rs.Connection = conn
                '    rs.CommandText = "UPDATE `machines` SET " _
                '        & "`name` = '" & txtMachine.Text & "'," _
                '        & "`test_name` = NULL," _
                '        & "`serial_no` = '" & txtSerial.Text & "'," _
                '        & "`com_port` = '" & ComPortsBox.Text & "'," _
                '        & "`baud_rate`= '" & cboBaudrate.Text & "'," _
                '        & "`status` = 'Disconnected'," _
                '        & "`location` = @path" _
                '        & " WHERE `name` = '" & txtMachine.Text & "' AND `serial_no` = '" & txtSerial.Text & "'"
                '    rs.ExecuteNonQuery()
                '    conn.Close()
                'Else
                '    conn.Close()
                '    conn.ConnectionString = TXTConnectionString
                '    conn.Open()
                '    rs.Connection = conn
                '    rs.CommandText = "INSERT INTO `machines` (`name`, `test_name`, `serial_no`, `com_port`, `baud_rate`, `status`, `location`) VALUES ('" & txtMachine.Text & "', NULL, '" & txtSerial.Text & "', '" & ComPortsBox.Text & "', '" & cboBaudrate.Text & "', 'Disconnected', @path)"
                '    rs.ExecuteNonQuery()
                '    conn.Close()
                'End If
                'conn.Close()

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
            '    If Port.IsOpen Then
            '        ReadENQData()
            '        TimedOut = 0
            '    Else
            '        Port.Open()
            '        ReadENQData()
            '        TimedOut = 0
            '    End If
            If Port.IsOpen Then
                ReadENQData()
            End If

        Catch ex As Exception
            'MessageBox.Show(ex.Message, "System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            'Exit Sub
            'Finally
            '    If Port.IsOpen Then
            '        ReadENQData()
            '        TimedOut = 0
            '    Else
            '        Port.Open()
            '        ReadENQData()
            '        TimedOut = 0
            '    End If
            ErrorLog(ex.ToString & vbNewLine)
            connectionCheckerTimer.Stop()
            connectionCheckerTimer.Start()
            TimedOut = 0
        End Try
    End Sub

    Private Sub ReadENQData()
        ReadDataASTM1 = Port.ReadExisting()
        If Not ReadDataASTM1 = "" Then
            EnqReceived = ReadDataASTM1(0)
            If (EnqReceived = ENQ) Then

                ReceivedText.Text = ""
                'If ComPort received Inquery <ENQ> then Host send Acknowledge <ACK>
                Acknowledge(Port)

                'Stop sending <ACK> and proceeds in reading the packets
                tmConnect.Stop()
                tmReadData.Start()

                'Converts ASCII to readable text
                ReadDataASTM1 = Data_Converter(ReadDataASTM1)

                'Display received and send packets
                Me.Invoke(Sub() AppendText("******************************Start-of-Communication*******************************" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine("******************************Start-of-Communication*******************************" & vbNewLine)
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM1)
                CommWriter.Close()
                CommLog.Close()

            ElseIf (EnqReceived = ACK) Then
                'If ComPort received Inquery <ENQ> then Host send Acknowledge <ACK>
                Acknowledge(Port)

                'Stop sending <ACK> and proceeds in reading the packets
                tmConnect.Stop()
                tmReadData.Start()

                'Converts ASCII to readable text
                ReadDataASTM1 = Data_Converter(ReadDataASTM1)

                'Display received and send packets
                Me.Invoke(Sub() AppendText("******************************Start-of-Communication*******************************" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine("******************************Start-of-Communication*******************************" & vbNewLine)
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM1)
                CommWriter.Close()
                CommLog.Close()

            ElseIf (EnqReceived = EOT) Then
                'Acknowledge(Port)

                ReadDataASTM1 = Data_Converter(ReadDataASTM1)

                Me.Invoke(Sub() AppendText("******************************Communication Restarting Please Wait!*******************************" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine("******************************Communication Restarting Please Wait!*******************************" & vbNewLine)
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM1)
                CommWriter.Close()
                CommLog.Close()
            Else
                'If received unknown packet display <NAK>
                If Not (EnqReceived = "") Then
                    ReadDataASTM1 = Data_Converter(ReadDataASTM1)

                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : Unrecognized Data Received! {" + ReadDataASTM1 + "}"))
                    Me.Invoke(Sub() AppendText("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine))

                    'Write file to TraceComm.Log
                    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                    CommWriter = New StreamWriter(CommLog)

                    CommWriter.WriteLine(System.DateTime.Now & " [Host] : Unrecognized Data Received! {" + ReadDataASTM1 + "}")
                    CommWriter.WriteLine("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine)

                    CommWriter.Close()
                    CommLog.Close()
                End If
            End If
        End If
    End Sub
    'End of Reading <ENQ>

    'Start reading packets after receiving <ENQ>
    Private Sub tmReadData_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmReadData.Tick
        Try
            'If Port.IsOpen Then
            '    'Port.Open()
            Main_ReadDataRS232()
            'Else
            '    Port.Open()
            '    Main_ReadDataRS232()
            'End If

        Catch ex As Exception

            'Finally
            'If Port.IsOpen Then
            '    'Port.Open()
            '    Main_ReadDataRS232()
            'Else
            '    Port.Open()
            '    Main_ReadDataRS232()
            'End If
        End Try
    End Sub
    'End of reading packets

    Private Sub Main_ReadDataRS232()

        ReadDataASTM2 = Port.ReadExisting
        If Not ReadDataASTM2 = "" Then
            EOTReceived = Trim(ReadDataASTM2(0))
            NAKReceived = Trim(ReadDataASTM2(0))
            ACKReceived = Trim(ReadDataASTM2(0))
            If (EOTReceived = EOT) Then
                ''Comment this if Machine is Not Yumizen
                'Acknowledge(Port)
                ''End
                'Start and stop timer
                tmReadData.Stop()
                tmConnect.Start()

                'Disconnect Database connection
                Disconnect()

                'To update Temporary Result Status table for refreshing list in software
                UpdateResultStatus()

                'Start Query if Set to Yes
                If QueryStatus Then
                    Me.Invoke(Sub() QueryProcess(QRMainID, Instrument, Serial))
                End If

                PatientID = ""
                PatientName = ""
                MainID = ""
                SampleID = ""
                OrderID = ""
                ControlID = ""
                ResultName = ""
                ResultValue = ""
                PatientBdateFinal = ""

                QRMainID = ""
                QRSampleID = ""

                ReadDataASTM2 = Data_Converter(ReadDataASTM2)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <ACK>" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM2 & vbNewLine))
                Me.Invoke(Sub() AppendText("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine))
                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <ACK>")
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM2)
                CommWriter.WriteLine("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine)
                CommWriter.Close()
                CommLog.Close()
            ElseIf (TimedOut = 5000) Then
                Port.Write(EOT)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <NAK>" & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <NAK>")
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : Communication Timed Out!")
                CommWriter.WriteLine("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine)
                CommWriter.Close()
                CommLog.Close()

                tmReadData.Stop()
                tmConnect.Start()
                TimedOut = 0
            ElseIf (ReadDataASTM2 = "") Then
                TimedOut = TimedOut + 1
            ElseIf (NAKReceived = NAK) Then
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <NAK>" & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <NAK>  ")
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : Communication Timed Out!")
                CommWriter.WriteLine("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine)
                CommWriter.Close()
                CommLog.Close()

                'tmReadData.Stop()
                'tmConnect.Start()
                'TimedOut = 0
            Else
                Acknowledge(Port)

                ReadDataASTM2 = Data_Converter(ReadDataASTM2)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <ACK>" & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <ACK>")
                CommWriter.Close()
                CommLog.Close()

                Dim data As String = ReadDataASTM2 & vbNewLine
                ReadDataRS232(data)

                'Me.Invoke(Sub() AppendText(data & vbCrLf))

                ReceivedText.SelectionStart = ReceivedText.Text.Length
                ReceivedText.SelectionLength = 0
            End If
        End If

    End Sub
    'Identify all packets received, save file to database
    Private Sub ReadDataRS232(ByVal data As String)
        Dim DataReceived As Char = ""

        DataReceived = data(6).ToString()
        With My.Settings
            Select Case (DataReceived)
                Case "H"
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & data))

                    'Write file to TraceComm.Log
                    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                    CommWriter = New StreamWriter(CommLog)
                    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & Replace(data, Chr(10), ""))
                    CommWriter.Close()
                    CommLog.Close()
                    ctrl_val = data.Split("|").GetValue(11)
                Case "P"
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & Data_Converter(data)))

                    'Write file to TraceComm.Log
                    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                    CommWriter = New StreamWriter(CommLog)
                    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & Replace(Data_Converter(data), Chr(10), ""))
                    CommWriter.Close()
                    CommLog.Close()

                    PatientID = data.Split("|").GetValue(3).ToString() & vbCrLf

                    PatientName = Data_Converter(data.Split("|").GetValue(5).ToString()) & vbCrLf




                    '#################---For Pentra ES60---#############################################
                    'PatientName = PatientName.Split("<").GetValue(0).ToString & vbCrLf
                    '#################---For Pentra ES60---#############################################

                    PatientName = PatientName.Replace("^", ", ").ToString().TrimEnd

                    PatientBdate = data.Split("|").GetValue(7).ToString().TrimEnd & vbCrLf

                    If Not PatientBdate.ToString = "" Then
                        PatientBdateFinal = DateTime.ParseExact(PatientBdate.Trim, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")
                    Else
                        PatientBdateFinal = ""
                    End If

                    PatientSex = data.Split("|").GetValue(8).ToString() & vbCrLf
                    PatientSex = PatientSex(0).ToString
                    If RemoveWhitespace(PatientSex) = "M" Then
                        PatientSexFinal = "Male"
                    ElseIf RemoveWhitespace(PatientSex) = "F" Then
                        PatientSexFinal = "Female"
                    Else
                        PatientSexFinal = ""
                    End If

                Case "O"
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & data))

                    'Write file to TraceComm.Log
                    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                    CommWriter = New StreamWriter(CommLog)
                    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & Replace(Data_Converter(data), Chr(10), ""))
                    CommWriter.Close()
                    CommLog.Close()

                    SampleID = data.Split("|").GetValue(2).ToString()
                    If SampleID.Split("^").Length >= 6 Then
                        LotNo = SampleID.Split("^").GetValue(5).ToString
                    Else
                        LotNo = ""
                    End If



                    'OrderID = StrConv(SampleID.Split("^").GetValue(0).ToString, VbStrConv.Uppercase)

                    'If OrderID.Trim = "CONTROL" Then
                    '    SampleID_Final = StrConv(SampleID.Split("^").GetValue(4).ToString.TrimEnd, VbStrConv.Uppercase)
                    'Else
                    '    SampleID_Final = StrConv(SampleID.Split("^").GetValue(0).ToString.TrimEnd, VbStrConv.Uppercase)
                    'End If

                    If ctrl_val = "P" Then
                        'MainID = RemoveWhitespace(SampleID.Split("^").GetValue(0).ToString)
                        MainID = SampleID.Split("^").GetValue(0).ToString
                    ElseIf ctrl_val = "D" Then
                        'MainID = RemoveWhitespace(SampleID.Split("^").GetValue(4).ToString)
                        MainID = SampleID.Split("^").GetValue(4).ToString
                    ElseIf ctrl_val = "Q" Then
                        'MainID = RemoveWhitespace(SampleID.Split("^").GetValue(4).ToString)
                        MainID = SampleID.Split("^").GetValue(4).ToString
                        'MainID = SampleID.Split("^").GetValue(1).ToString
                    Else
                        'MainID = RemoveWhitespace(SampleID)
                        MainID = SampleID
                    End If

                'MainID = Replace(MainID, ", ", "^")
                'MainID = RemoveWhitespace(MainID)
                'MainID = SampleID_Final

                'Me.Invoke(Sub() IdentifyOrder(MainID, RemoveWhitespace(PatientID), PatientName, Instrument, Serial, MainID, PatientSexFinal, PatientBdateFinal))
                ''Me.Invoke(Sub() IdentifyOrder(MainID, RemoveWhitespace(PatientID), PatientName, Instrument, Serial, Section, MainID, PatientSexFinal, PatientBdateFinal))
                Case "R"

                    If data.Split("|").GetValue(11).ToString().Trim.Split("<").GetValue(0).ToString.Trim = "" Then

                        DateReceived = Now.ToString("yyyy-MM-dd")
                        TimeReceived = Now.ToString("HH:mm:ss tt")

                    Else

                        DateReceived = DateTime.ParseExact(data.Split("|").GetValue(11).ToString().Trim.Split("<").GetValue(0).ToString.Trim, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")
                        TimeReceived = DateTime.ParseExact(data.Split("|").GetValue(11).ToString().Trim.Split("<").GetValue(0).ToString.Trim, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("HH:mm:ss tt")

                    End If

                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [machine] : " & data))

                    'write file to tracecomm.log
                    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                    CommWriter = New StreamWriter(CommLog)
                    CommWriter.WriteLine(System.DateTime.Now & " [machine] : " & Replace(data, Chr(10), ""))
                    CommWriter.Close()
                    CommLog.Close()

                    ResultName = Replace(data.Split("|").GetValue(2).ToString(), "^", " ").TrimStart() & vbCrLf
                    'resultname = resultname.split(" "c)(4)
                    ResultName = ResultName.Split(" "c)(0)
                    ResultValue = data.Split("|").GetValue(3).ToString() & vbCrLf

                    'messagebox.show($"type : {ctrl_val}")
                    If ctrl_val = "P" Then
                        'me.invoke(sub() insertresultdata(mainid, resultname, resultvalue.trimend, instrument, serial, mainid, datereceived, timereceived))
                        Me.Invoke(Sub() InsertResultData(MainID, ResultName, ResultValue, Instrument, Serial, MainID))
                    ElseIf ctrl_val = "D" Then
                        'me.invoke(sub() insertcontrol(mainid, resultname, resultvalue.trimend, instrument, serial, section, datereceived, mainid, datereceived, timereceived))
                        Me.Invoke(Sub() InsertControl(MainID, ResultName, ResultValue, Instrument, Serial, DateReceived))
                    ElseIf ctrl_val = "Q" Then
                        'me.invoke(sub() insertcontrol(mainid, resultname, resultvalue.trimend, instrument, serial, section, datereceived, mainid, datereceived, timereceived))
                        Me.Invoke(Sub() InsertControl(MainID, ResultName, ResultValue, Instrument, Serial, DateReceived))
                    Else
                        'me.invoke(sub() insertresultdata(mainid, resultname, resultvalue.trimend, instrument, serial, mainid, datereceived, timereceived))
                        'Me.Invoke(Sub() InsertResultData(MainID, ResultName, ResultValue, Instrument, Serial, MainID))
                        Me.Invoke(Sub() InsertControl(MainID, ResultName, ResultValue, Instrument, Serial, DateReceived))
                    End If

                'Me.Invoke(Sub() DisplayResult(SampleID_Final, RemoveWhitespace(ResultName), RemoveWhitespace(ResultValue), Instrument, Serial, Section, MainID))
                Case "Q"
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & data))

                    'Write file to TraceComm.Log
                    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                    CommWriter = New StreamWriter(CommLog)
                    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & Replace(data, Chr(10), ""))
                    CommWriter.Close()
                    CommLog.Close()

                    QRSampleID = data.Split("|").GetValue(2).ToString().Trim & vbCrLf
                    QRMainID = StrConv(QRSampleID.Split("^").GetValue(1).ToString.Trim, VbStrConv.Uppercase)

                    'QueryStatus = True
                    QueryStatus = QRMainID <> ""

                'Remove this part for Micros ES60----------------------------------------------------------------------
                Case "C"
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & data))

                    'Write file to TraceComm.Log
                    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                    CommWriter = New StreamWriter(CommLog)
                    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & Replace(data, Chr(10), ""))
                    CommWriter.Close()
                    CommLog.Close()
                '-------------------------------------------------------------------------------------------------------
                Case "L"
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & data))

                    If QueryStatus = False Then

                        Disconnect()
                        Me.Invoke(Sub() IdentifyOrder(MainID, RemoveWhitespace(PatientID), PatientName, Instrument, Serial, MainID, PatientSexFinal, PatientBdateFinal))

                    End If
                    'Write file to TraceComm.Log
                    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                    CommWriter = New StreamWriter(CommLog)
                    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & Replace(data, Chr(10), ""))
                    CommWriter.Close()
                    CommLog.Close()
            End Select

        End With
    End Sub
    'End of reading packets

    'Display received packets to richtextbox
    Private Sub AppendText(ByVal str As String)
        ReceivedText.AppendText(str)
    End Sub
    'End of reading packets

    'Convert ASCII to readable text
    Function Data_Converter(ByVal Raw_Data As String) As String
        Raw_Data = Raw_Data.Replace(STX, "<STX>")
        Raw_Data = Raw_Data.Replace(ETX, "<ETX>")
        Raw_Data = Raw_Data.Replace(SOH, "<SOH>")
        Raw_Data = Raw_Data.Replace(LF, "<LF>")
        Raw_Data = Raw_Data.Replace(CR, "<CR>")
        Raw_Data = Raw_Data.Replace(NAK, "<NAK>")
        Raw_Data = Raw_Data.Replace(ENQ, "<ENQ>")
        Raw_Data = Raw_Data.Replace(EOT, "<EOT>")
        Raw_Data = Raw_Data.Replace(DLE, "<DLE>")
        Raw_Data = Raw_Data.Replace("\xd1", "Ñ")
        Raw_Data = Raw_Data.Replace("\xf1", "ñ")
        Return Raw_Data
    End Function
    'End of Convertion

    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        frmLogin.Show()
    End Sub

    Dim PatientID_Out As String
    Dim PatientName_Out As String
    Dim PatientBDate_Out As String
    Dim PatientSex_Out As String

    Dim OrderSampleID_Out As String
    Dim OrderTest_Out As String

    Dim Specimen_Type As String = 1

    Public TestList As New List(Of String)

    Private Sub QueryProcess(ByVal SampleID As String, ByVal Instrument As String, ByVal SerialNo As String)
        Dim exist As Boolean = False
        Using conn As New MySqlConnection(My.Settings.ConnectionString)
            conn.Open()

            Using cmd As New MySqlCommand()
                cmd.Connection = conn
                cmd.CommandText = "SELECT `tmpWorklist`.`sample_id`, `tmpWorklist`.`patient_id`, `tmpWorklist`.`patient_name`, `tmpWorklist`.`sex`, `tmpWorklist`.`bdate`, `patient_order`.`mode`,`tmpWorklist`.`testtype`,`tmpWorklist`.`sub_section` FROM `tmpWorklist`, `patient_order` WHERE `tmpWorklist`.`sample_id` = @SampleID AND `patient_order`.`sample_id` = @SampleID AND `tmpWorklist`.`status` IN ('Processing', 'Result Received') LIMIT 1"
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@SampleID", SampleID)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    PatientID_Out = reader(1).ToString
                    PatientName_Out = Replace(reader(2).ToString, ", ", "^")
                    PatientBDate_Out = "" 'DateTime.ParseExact(reader(4), "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyyMMdd")
                    PatientSex_Out = reader(3).ToString.Substring(0, 1)
                    OrderSampleID_Out = reader(0).ToString
                    exist = True
                End Using
            End Using

            If exist Then
                MainID = OrderSampleID_Out

                Using cmd As New MySqlCommand()
                    cmd.Connection = conn
                    cmd.CommandText = "Select `test_code` FROM `tmpresult` WHERE `sample_id` = @main_id"
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@main_id", MainID)

                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read
                            TestList.Add(reader(0).ToString)
                        End While
                    End Using
                End Using

                For Each a In TestList
                    Using cmd As New MySqlCommand()
                        cmd.Connection = conn
                        cmd.CommandText = "SELECT `channel` FROM `specimen` WHERE `test_code` = @test_code AND `instrument` = @instrument AND `status` = 'Enable'"
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
                        cmd.Parameters.AddWithValue("@test_code", a.ToString)

                        Using reader As MySqlDataReader = cmd.ExecuteReader()
                            If reader.Read() Then
                                Dim channel As String = $"^^^{reader(0).ToString()}\"
                                If Not OrderTest_Out.Contains(channel) Then
                                    OrderTest_Out &= channel
                                End If
                            End If
                        End Using
                    End Using
                Next

                TestList.Clear()

                Dim HeaderOutput As String
                Dim PatientOutput As String
                Dim OrderOutput As String
                Dim Terminator As String

                'This part is for Yumizen H500/550 /Sending query to machine
                HeaderOutput = "1H|\^&|||SBSI LIS|||||||P|E1394-97|" & FormatDateRegex()
                PatientOutput = "2P|1||" & PatientID_Out & "||" & PatientName_Out & "||" & PatientBDate_Out & "|" & PatientSex_Out & "|||||"
                'OrderOutput = "3O|1|" & OrderSampleID_Out & "||" & OrderTest_Out.TrimEnd("\") & "|R|" & FormatDateRegex() & "|||||N||||||||||||||Q|||||"
                OrderOutput = "3O|1|" & OrderSampleID_Out & "||" & OrderTest_Out.TrimEnd("\") & "|R|" & FormatDateRegex() & "|||||A||||1|"
                Terminator = "4L|1|"
                'End

                Dim SumHeader As String = CheckSum(HeaderOutput & CR & ETX)
                Dim SumPatient As String = CheckSum(PatientOutput & CR & ETX)
                Dim SumOrder As String = CheckSum(OrderOutput & CR & ETX)
                Dim SumTerminator As String = CheckSum(Terminator & CR & ETX)

                Enquery(Port)
                Thread.Sleep(500)
                Me.Invoke(Sub() AppendText("******************************Start-Query-Request******************************" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine("******************************Start-Query-Request******************************" & vbNewLine)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine)
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
                CommWriter.Close()
                CommLog.Close()

                Port.Write(STX & HeaderOutput & CR & ETX & SumHeader & CR & LF)
                Thread.Sleep(500)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>")
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
                CommWriter.Close()
                CommLog.Close()

                Port.Write(STX & PatientOutput & CR & ETX & SumPatient & CR & LF)
                Thread.Sleep(500)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & PatientOutput & "<CR><ETX>" & SumPatient & "<CR><LF>" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & PatientOutput & "<CR><ETX>" & SumPatient & "<CR><LF>")
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
                CommWriter.Close()
                CommLog.Close()

                Port.Write(STX & OrderOutput & CR & ETX & SumOrder & CR & LF)
                Thread.Sleep(500)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & OrderOutput & "<CR><ETX>" & SumOrder & "<CR><LF>" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & OrderOutput & "<CR><ETX>" & SumOrder & "<CR><LF>")
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
                CommWriter.Close()
                CommLog.Close()

                Port.Write(STX & Terminator & CR & ETX & SumTerminator & CR & LF)
                Thread.Sleep(500)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>")
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
                CommWriter.Close()
                CommLog.Close()

                EndTrasmission(Port)
                Thread.Sleep(500)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine))
                Me.Invoke(Sub() AppendText("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine))

                'Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine)
                CommWriter.WriteLine("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine)
                CommWriter.Close()
                CommLog.Close()

                QueryStatus = False

                PatientID_Out = ""
                PatientName_Out = ""
                PatientBDate_Out = ""
                PatientSex_Out = ""

                OrderSampleID_Out = ""
                OrderTest_Out = ""

                TestList.Clear()
            Else
                QueryStatus = False

                Dim HeaderOutput As String
                Dim Terminator As String

                'Start sending <ENQ>---#############################################################################################################
                'Send <ENQ> to Analyzer
                Enquery(Port)
                'Write Serial Data to Readable text
                Me.Invoke(Sub() AppendText("******************************Start-Query-Request******************************" & vbNewLine))
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine))
                Thread.Sleep(500)
                'Read Serial Data to complete transaction
                ReadDataASTM1 = ""
                ReadDataASTM1 = Port.ReadExisting()
                ReadDataASTM1 = Data_Converter(ReadDataASTM1)
                'Write Serial Data to readable text
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
                Thread.Sleep(1000)

                '--Resend Packet if received <Nak>
                If ReadDataASTM1 = "<NAK>" Then
                    'Write Header Packet to Serial Port (Port)
                    Enquery(Port)
                    Thread.Sleep(500)
                    'Write Serial Data to readable text sent by Host
                    Me.Invoke(Sub() AppendText("******************************Start-Query-Request******************************" & vbNewLine))
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine))
                    'Read Serial Data to complete transaction
                    ReadDataASTM1 = ""
                    ReadDataASTM1 = Port.ReadExisting()
                    ReadDataASTM1 = Data_Converter(ReadDataASTM1)
                    'Write Serial Data to readable text received from Analyzer
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
                    Thread.Sleep(1000)
                End If
                '--End If Statement

                '--Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine("******************************Start-Query-Request******************************" & vbNewLine)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine)
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine)
                CommWriter.Close()
                CommLog.Close()
                '---End Log File
                '###################################################################################################################################

                'Send Header Packet---##############################################################################################################
                'Construct Header(H) Packet for sending
                HeaderOutput = "1H|\^&|||SBSI LIS|||||||P|LIS2-A2|" & FormatDateRegex()
                'Get the sum of packects as HEX value for checking
                Dim SumHeader As String = CheckSum(HeaderOutput & CR & ETX)
                'Write Header Packet to Serial Port (Port)
                Port.Write(STX & HeaderOutput & CR & ETX & SumHeader & CR & LF)
                Thread.Sleep(500)
                'Write Serial Data to readable text sent by Host
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>" & vbNewLine))
                'Read Serial Data to complete transaction
                ReadDataASTM1 = ""
                ReadDataASTM1 = Port.ReadExisting()
                ReadDataASTM1 = Data_Converter(ReadDataASTM1)
                'Write Serial Data to readable text received from Analyzer
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
                Thread.Sleep(1000)

                '--Resend Packet if received <Nak>
                If ReadDataASTM1 = "<NAK>" Then
                    'Write Header Packet to Serial Port (Port)
                    Port.Write(STX & HeaderOutput & CR & ETX & SumHeader & CR & LF)
                    Thread.Sleep(500)
                    'Write Serial Data to readable text sent by Host
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>" & vbNewLine))
                    'Read Serial Data to complete transaction
                    ReadDataASTM1 = ""
                    ReadDataASTM1 = Port.ReadExisting()
                    ReadDataASTM1 = Data_Converter(ReadDataASTM1)
                    'Write Serial Data to readable text received from Analyzer
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
                    Thread.Sleep(1000)
                End If
                '--End If Statement

                '--Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>")
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine)
                CommWriter.Close()
                CommLog.Close()
                '--End Log File
                '###################################################################################################################################

                'Send Terminator Packet
                Terminator = "2L|1|I"
                Dim SumTerminator As String = CheckSum(Terminator & CR & ETX)

                Port.Write(STX & Terminator & CR & ETX & SumTerminator & CR & LF)
                Thread.Sleep(500)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>" & vbNewLine))

                ReadDataASTM1 = ""
                ReadDataASTM1 = Port.ReadExisting()
                ReadDataASTM1 = Data_Converter(ReadDataASTM1)
                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
                Thread.Sleep(1000)

                If ReadDataASTM1 = "<NAK>" Then
                    Port.Write(STX & Terminator & CR & ETX & SumTerminator & CR & LF)
                    Thread.Sleep(500)
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>" & vbNewLine))

                    ReadDataASTM1 = ""
                    ReadDataASTM1 = Port.ReadExisting()
                    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
                    Thread.Sleep(1000)
                End If

                '--Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>")
                CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine)
                CommWriter.Close()
                CommLog.Close()
                'End

                'Send <EOT> to end transaction
                EndTrasmission(Port)

                Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine))
                Me.Invoke(Sub() AppendText("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine))

                '--Write file to TraceComm.Log
                CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                CommWriter = New StreamWriter(CommLog)
                CommWriter.WriteLine(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine)
                CommWriter.WriteLine("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine)
                CommWriter.Close()
                CommLog.Close()

                'QueryStatus = False

                'ConnectionTimedOut(Port)
                'Me.Invoke(Sub() AppendText("******************************Error-Communication*******************************" & vbNewLine & vbNewLine))
                'Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <NAK>" & vbNewLine))
                'Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : No-Sample-Found" & vbNewLine))
                'Me.Invoke(Sub() AppendText("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine))

                ''Write file to TraceComm.Log
                'CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
                'CommWriter = New StreamWriter(CommLog)
                'CommWriter.WriteLine("******************************Error-Communication*******************************" & vbNewLine)
                'CommWriter.WriteLine(System.DateTime.Now & " [Host] : <NAK>" & vbNewLine)
                'CommWriter.WriteLine(System.DateTime.Now & " [Host] : No-Sample-Found" & vbNewLine)
                'CommWriter.WriteLine("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine)
                'CommWriter.Close()
                'CommLog.Close()
                Thread.Sleep(2000)
            End If

        End Using

        '######################
        'rs.Parameters.Clear()
        'rs.Parameters.AddWithValue("@SampleID", SampleID)
        ''rs.Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)

        'Connect()
        'rs.Connection = conn
        'rs.CommandText = "SELECT `tmpWorklist`.`sample_id`, `tmpWorklist`.`patient_id`, `tmpWorklist`.`patient_name`, `tmpWorklist`.`sex`, `tmpWorklist`.`bdate`, `patient_order`.`mode`,`tmpWorklist`.`testtype`,`tmpWorklist`.`sub_section` FROM `tmpWorklist`, `patient_order` WHERE `tmpWorklist`.`sample_id` = @SampleID AND `patient_order`.`sample_id` = @SampleID AND `tmpWorklist`.`status` IN ('Processing', 'Result Received') LIMIT 1"
        'reader = rs.ExecuteReader
        'reader.Read()
        'If reader.HasRows Then
        '    PatientID_Out = reader(1).ToString
        '    PatientName_Out = Replace(reader(2).ToString, ", ", "^")
        '    PatientBDate_Out = "" 'DateTime.ParseExact(reader(4), "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyyMMdd")
        '    PatientSex_Out = reader(3).ToString.Substring(0, 1)
        '    OrderSampleID_Out = reader(0).ToString

        '    'Section = reader(6).ToString
        '    'SubSection = reader(7).ToString
        '    Disconnect()

        '    MainID = OrderSampleID_Out

        '    'Connect()
        '    'rs.Connection = conn
        '    'rs.CommandText = "SELECT `order_code` FROM `lis_order` WHERE `sample_id` = '" & MainID & "'"
        '    'reader = rs.ExecuteReader
        '    'While reader.Read
        '    '    TestList.Add(reader(0).ToString)
        '    'End While
        '    'Disconnect()

        '    Connect()
        '    rs.Connection = conn
        '    'rs.CommandText = "Select `test_code` FROM `tmpresult` WHERE `sample_id` = '" & MainID & "' AND section = '" & Section & "' AND sub_section = '" & SubSection & "'"
        '    rs.CommandText = "Select `test_code` FROM `tmpresult` WHERE `sample_id` = '" & MainID & "'"
        '    reader = rs.ExecuteReader
        '    While reader.Read
        '        TestList.Add(reader(0).ToString)
        '    End While
        '    Disconnect()

        '    OrderTest_Out = ""
        '    For Each a In TestList
        '        Connect()
        '        rs.Connection = conn
        '        rs.Parameters.Clear()
        '        rs.Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)
        '        rs.CommandText = "SELECT `channel` FROM `specimen` WHERE `test_code` = '" & a.ToString & "' AND `instrument` = @instrument AND `status` = 'Enable'"
        '        reader = rs.ExecuteReader
        '        reader.Read()
        '        If reader.HasRows Then
        '            'OrderTest_Out &= "^^^" & reader(0).ToString & "\"
        '            ''OrderTest_Out &= "^^^" & a.ToString & "\"

        '            Dim channel As String = $"^^^{reader(0).ToString()}\"
        '            If Not OrderTest_Out.Contains(channel) Then
        '                OrderTest_Out &= channel
        '                'countOrderedTest += 1
        '            End If
        '        End If
        '        Disconnect()
        '    Next
        '    TestList.Clear()

        '    Dim HeaderOutput As String
        '    Dim PatientOutput As String
        '    Dim OrderOutput As String
        '    Dim Terminator As String

        '    'This part is for Yumizen H500/550 /Sending query to machine
        '    HeaderOutput = "1H|\^&|||SBSI LIS|||||||P|E1394-97|" & FormatDateRegex()
        '    PatientOutput = "2P|1||" & PatientID_Out & "||" & PatientName_Out & "||" & PatientBDate_Out & "|" & PatientSex_Out & "|||||"
        '    'OrderOutput = "3O|1|" & OrderSampleID_Out & "||" & OrderTest_Out.TrimEnd("\") & "|R|" & FormatDateRegex() & "|||||N||||||||||||||Q|||||"
        '    OrderOutput = "3O|1|" & OrderSampleID_Out & "||" & OrderTest_Out.TrimEnd("\") & "|R|" & FormatDateRegex() & "|||||A||||1|"
        '    Terminator = "4L|1|"
        '    'End

        '    Dim SumHeader As String = CheckSum(HeaderOutput & CR & ETX)
        '    Dim SumPatient As String = CheckSum(PatientOutput & CR & ETX)
        '    Dim SumOrder As String = CheckSum(OrderOutput & CR & ETX)
        '    Dim SumTerminator As String = CheckSum(Terminator & CR & ETX)

        '    Enquery(Port)
        '    Thread.Sleep(500)
        '    Me.Invoke(Sub() AppendText("******************************Start-Query-Request******************************" & vbNewLine))
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine))
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

        '    'Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine("******************************Start-Query-Request******************************" & vbNewLine)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()

        '    Port.Write(STX & HeaderOutput & CR & ETX & SumHeader & CR & LF)
        '    Thread.Sleep(500)
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>" & vbNewLine))
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

        '    'Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>")
        '    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()

        '    Port.Write(STX & PatientOutput & CR & ETX & SumPatient & CR & LF)
        '    Thread.Sleep(500)
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & PatientOutput & "<CR><ETX>" & SumPatient & "<CR><LF>" & vbNewLine))
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

        '    'Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & PatientOutput & "<CR><ETX>" & SumPatient & "<CR><LF>")
        '    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()

        '    Port.Write(STX & OrderOutput & CR & ETX & SumOrder & CR & LF)
        '    Thread.Sleep(500)
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & OrderOutput & "<CR><ETX>" & SumOrder & "<CR><LF>" & vbNewLine))
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

        '    'Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & OrderOutput & "<CR><ETX>" & SumOrder & "<CR><LF>")
        '    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()

        '    Port.Write(STX & Terminator & CR & ETX & SumTerminator & CR & LF)
        '    Thread.Sleep(500)
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>" & vbNewLine))
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

        '    'Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>")
        '    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()

        '    EndTrasmission(Port)
        '    Thread.Sleep(500)
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine))
        '    Me.Invoke(Sub() AppendText("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine))

        '    'Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine)
        '    CommWriter.WriteLine("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()

        '    QueryStatus = False

        '    PatientID_Out = ""
        '    PatientName_Out = ""
        '    PatientBDate_Out = ""
        '    PatientSex_Out = ""

        '    OrderSampleID_Out = ""
        '    OrderTest_Out = ""

        '    TestList.Clear()

        'Else
        '    QueryStatus = False

        '    Dim HeaderOutput As String
        '    Dim Terminator As String

        '    'Start sending <ENQ>---#############################################################################################################
        '    'Send <ENQ> to Analyzer
        '    Enquery(Port)
        '    'Write Serial Data to Readable text
        '    Me.Invoke(Sub() AppendText("******************************Start-Query-Request******************************" & vbNewLine))
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine))
        '    Thread.Sleep(500)
        '    'Read Serial Data to complete transaction
        '    ReadDataASTM1 = ""
        '    ReadDataASTM1 = Port.ReadExisting()
        '    ReadDataASTM1 = Data_Converter(ReadDataASTM1)
        '    'Write Serial Data to readable text
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
        '    Thread.Sleep(1000)

        '    '--Resend Packet if received <Nak>
        '    If ReadDataASTM1 = "<NAK>" Then
        '        'Write Header Packet to Serial Port (Port)
        '        Enquery(Port)
        '        Thread.Sleep(500)
        '        'Write Serial Data to readable text sent by Host
        '        Me.Invoke(Sub() AppendText("******************************Start-Query-Request******************************" & vbNewLine))
        '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine))
        '        'Read Serial Data to complete transaction
        '        ReadDataASTM1 = ""
        '        ReadDataASTM1 = Port.ReadExisting()
        '        ReadDataASTM1 = Data_Converter(ReadDataASTM1)
        '        'Write Serial Data to readable text received from Analyzer
        '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
        '        Thread.Sleep(1000)
        '    End If
        '    '--End If Statement

        '    '--Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine("******************************Start-Query-Request******************************" & vbNewLine)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()
        '    '---End Log File
        '    '###################################################################################################################################

        '    'Send Header Packet---##############################################################################################################
        '    'Construct Header(H) Packet for sending
        '    HeaderOutput = "1H|\^&|||SBSI LIS|||||||P|LIS2-A2|" & FormatDateRegex()
        '    'Get the sum of packects as HEX value for checking
        '    Dim SumHeader As String = CheckSum(HeaderOutput & CR & ETX)
        '    'Write Header Packet to Serial Port (Port)
        '    Port.Write(STX & HeaderOutput & CR & ETX & SumHeader & CR & LF)
        '    Thread.Sleep(500)
        '    'Write Serial Data to readable text sent by Host
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>" & vbNewLine))
        '    'Read Serial Data to complete transaction
        '    ReadDataASTM1 = ""
        '    ReadDataASTM1 = Port.ReadExisting()
        '    ReadDataASTM1 = Data_Converter(ReadDataASTM1)
        '    'Write Serial Data to readable text received from Analyzer
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
        '    Thread.Sleep(1000)

        '    '--Resend Packet if received <Nak>
        '    If ReadDataASTM1 = "<NAK>" Then
        '        'Write Header Packet to Serial Port (Port)
        '        Port.Write(STX & HeaderOutput & CR & ETX & SumHeader & CR & LF)
        '        Thread.Sleep(500)
        '        'Write Serial Data to readable text sent by Host
        '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>" & vbNewLine))
        '        'Read Serial Data to complete transaction
        '        ReadDataASTM1 = ""
        '        ReadDataASTM1 = Port.ReadExisting()
        '        ReadDataASTM1 = Data_Converter(ReadDataASTM1)
        '        'Write Serial Data to readable text received from Analyzer
        '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
        '        Thread.Sleep(1000)
        '    End If
        '    '--End If Statement

        '    '--Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>")
        '    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()
        '    '--End Log File
        '    '###################################################################################################################################

        '    'Send Terminator Packet
        '    Terminator = "2L|1|I"
        '    Dim SumTerminator As String = CheckSum(Terminator & CR & ETX)

        '    Port.Write(STX & Terminator & CR & ETX & SumTerminator & CR & LF)
        '    Thread.Sleep(500)
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>" & vbNewLine))

        '    ReadDataASTM1 = ""
        '    ReadDataASTM1 = Port.ReadExisting()
        '    ReadDataASTM1 = Data_Converter(ReadDataASTM1)
        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
        '    Thread.Sleep(1000)

        '    If ReadDataASTM1 = "<NAK>" Then
        '        Port.Write(STX & Terminator & CR & ETX & SumTerminator & CR & LF)
        '        Thread.Sleep(500)
        '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>" & vbNewLine))

        '        ReadDataASTM1 = ""
        '        ReadDataASTM1 = Port.ReadExisting()
        '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine))
        '        Thread.Sleep(1000)
        '    End If

        '    '--Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>")
        '    CommWriter.WriteLine(System.DateTime.Now & " [Machine] : " & ReadDataASTM1 & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()
        '    'End

        '    'Send <EOT> to end transaction
        '    EndTrasmission(Port)

        '    Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine))
        '    Me.Invoke(Sub() AppendText("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine))

        '    '--Write file to TraceComm.Log
        '    CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    CommWriter = New StreamWriter(CommLog)
        '    CommWriter.WriteLine(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine)
        '    CommWriter.WriteLine("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine)
        '    CommWriter.Close()
        '    CommLog.Close()

        '    'QueryStatus = False

        '    'ConnectionTimedOut(Port)
        '    'Me.Invoke(Sub() AppendText("******************************Error-Communication*******************************" & vbNewLine & vbNewLine))
        '    'Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <NAK>" & vbNewLine))
        '    'Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : No-Sample-Found" & vbNewLine))
        '    'Me.Invoke(Sub() AppendText("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine))

        '    ''Write file to TraceComm.Log
        '    'CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
        '    'CommWriter = New StreamWriter(CommLog)
        '    'CommWriter.WriteLine("******************************Error-Communication*******************************" & vbNewLine)
        '    'CommWriter.WriteLine(System.DateTime.Now & " [Host] : <NAK>" & vbNewLine)
        '    'CommWriter.WriteLine(System.DateTime.Now & " [Host] : No-Sample-Found" & vbNewLine)
        '    'CommWriter.WriteLine("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine)
        '    'CommWriter.Close()
        '    'CommLog.Close()
        '    Thread.Sleep(2000)
        'End If
        'Disconnect()
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

            'conn.ConnectionString = TXTConnectionString
            'conn.Open()
            'rs.Connection = conn
            'rs.CommandText = "SELECT * FROM `machines` WHERE `name` = '" & txtMachine.Text & "' AND `serial_no` = '" & txtSerial.Text & "'"
            'reader = rs.ExecuteReader
            'reader.Read()
            'If reader.HasRows Then
            '    conn.Close()
            '    conn.ConnectionString = TXTConnectionString
            '    conn.Open()
            '    rs.Connection = conn
            '    rs.CommandText = "UPDATE `machines` SET `status` = 'Disconnected' WHERE `name` = '" & txtMachine.Text & "' AND `serial_no` = '" & txtSerial.Text & "'"
            '    rs.ExecuteNonQuery()
            '    conn.Close()
            'End If
            'conn.Close()
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
            'conn.ConnectionString = TXTConnectionString
            'conn.Open()
            'rs.Connection = conn
            'rs.CommandText = "SELECT * FROM `machines` WHERE `name` = '" & txtMachine.Text & "' AND `serial_no` = '" & txtSerial.Text & "'"
            'reader = rs.ExecuteReader
            'reader.Read()
            'If reader.HasRows Then
            '    conn.Close()
            '    conn.ConnectionString = TXTConnectionString
            '    conn.Open()
            '    rs.Connection = conn
            '    rs.CommandText = "UPDATE `machines` SET `status` = 'Connected' WHERE `name` = '" & txtMachine.Text & "' AND `serial_no` = '" & txtSerial.Text & "'"
            '    rs.ExecuteNonQuery()
            '    conn.Close()
            'End If
            'conn.Close()
            '############################################################################################################################################################
        End If
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Me.ReceivedText.Text = ""
    End Sub

    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
        Port.Write(EOT)
        Thread.Sleep(1000)
    End Sub

End Class