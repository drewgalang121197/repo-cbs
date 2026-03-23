Imports System.IO
Imports System.IO.Ports
Imports System.Threading
Imports System.Threading.Thread
Imports System.Text.RegularExpressions

Imports System.Text
Imports DevExpress

Module modClass
    Public Sub SaveRecord(ByVal SQL As String)
        Connect()
        rs.Connection = conn
        rs.CommandText = SQL
        rs.ExecuteNonQuery()
        Disconnect()
        MessageBox.Show("New record has been successfuly save.", "Record Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Public Sub SaveRecordwthoutMSG(ByVal SQL As String)
        Connect()
        rs.Connection = conn
        rs.CommandText = SQL
        rs.ExecuteNonQuery()
        Disconnect()
    End Sub

    Public Sub UpdateRecord(ByVal SQL As String)
        Connect()
        rs.Connection = conn
        rs.CommandText = SQL
        rs.ExecuteNonQuery()
        Disconnect()
        MessageBox.Show("The selected record has been successfuly update.", "Record Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Public Sub UpdateRecordwthoutMSG(ByVal SQL As String)
        Connect()
        rs.Connection = conn
        rs.CommandText = SQL
        rs.ExecuteNonQuery()
        Disconnect()
    End Sub

    Public Sub DeleteRecord(ByVal DeleteFromTable As String, ByVal DeleteIdentifier As String, ByVal DeleteID As String)
        If (MessageBox.Show("Are you sure you want to delete the selected record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) = DialogResult.Yes Then
            Connect()
            rs.Connection = conn
            rs.CommandText = "DELETE FROM " & DeleteFromTable & " WHERE " & DeleteIdentifier & " LIKE '" & DeleteID & "'"
            rs.ExecuteNonQuery()
            Disconnect()
            MessageBox.Show("The selected record has been successfully deleted.", "Record Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Public Sub DeleteRecordSQL(ByVal SQL As String)
        If (MessageBox.Show("Are you sure you want to delete the selected record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) = DialogResult.Yes Then
            Connect()
            rs.Connection = conn
            rs.CommandText = SQL
            rs.ExecuteNonQuery()
            Disconnect()
            MessageBox.Show("The selected record has been successfully deleted.", "Record Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Public Sub DeleteRecordWithoutMsg(ByVal DeleteFromTable As String, ByVal DeleteIdentifier As String, ByVal DeleteID As String)
        Connect()
        rs.Connection = conn
        rs.CommandText = "DELETE FROM " & DeleteFromTable & " WHERE " & DeleteIdentifier & " LIKE '" & DeleteID & "'"
        rs.ExecuteNonQuery()
        Disconnect()
    End Sub

    Public Sub MaterialStyleDataGrid(ByRef DTview As DataGridView)
        DTview.Rows.Clear()
        DTview.Font = New Font("Segoe UI", 12, FontStyle.Regular)

        DTview.BorderStyle = BorderStyle.None
        'DTview.RowsDefaultCellStyle.SelectionBackColor = Color.CornflowerBlue
        'DTview.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249)
        'DTview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal

        DTview.DefaultCellStyle.SelectionBackColor = Color.LightGray
        DTview.DefaultCellStyle.SelectionForeColor = Color.Black
        DTview.BackgroundColor = Color.White
        DTview.EnableHeadersVisualStyles = False
        DTview.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
        DTview.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        'DTview.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 88, 173)
        DTview.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray
    End Sub

    Public Sub AutoGenID(ByVal ITem As String)
        Randomize()
        Dim a As Integer
        a = Rnd() * 9999999
        ITem = a.ToString
    End Sub

    Public Sub AutoLoadCombo(ByVal cboContent As ComboBox, ByVal TableName As String, ByVal ColumnValue As Integer, ByVal ColumnOrder As String)
        cboContent.Items.Clear()
        Connect()
        rs.Connection = conn
        rs.CommandText = "SELECT * FROM " & TableName & " ORDER BY " & ColumnOrder
        reader = rs.ExecuteReader
        While reader.Read
            cboContent.Items.Add(reader(ColumnValue))
        End While
        Disconnect()
    End Sub

    Public Sub AutoLoadComboDev(ByVal cboContent As XtraEditors.ComboBoxEdit, ByVal TableName As String, ByVal ColumnValue As Integer, ByVal ColumnOrder As String)
        cboContent.Properties.Items.Clear()
        Connect()
        rs.Connection = conn
        rs.CommandText = "SELECT * FROM " & TableName & " ORDER BY " & ColumnOrder
        reader = rs.ExecuteReader
        While reader.Read
            cboContent.Properties.Items.Add(reader(ColumnValue))
        End While
        Disconnect()
    End Sub

    Public Sub AutoLoadComboDistinct(ByVal cboContent As ComboBox, ByVal TableName As String, ByVal DistinctName As String, ByVal ColumnOrder As String)
        cboContent.Items.Clear()
        Connect()
        rs.Connection = conn
        rs.CommandText = "SELECT DISTINCT " & DistinctName & " FROM " & TableName & " ORDER BY " & ColumnOrder
        reader = rs.ExecuteReader
        While reader.Read
            cboContent.Items.Add(reader(DistinctName))
        End While
        Disconnect()
    End Sub

    Public Sub AutoLoadComboWithCondition(ByVal cboContent As ComboBox, ByVal TableName As String, ByVal strWHERE As String, ByVal strLIKE As String, ByVal ColumnValue As Integer, ByVal ColumnOrder As String)
        cboContent.Items.Clear()
        Connect()
        rs.Connection = conn
        rs.CommandText = "SELECT * FROM " & TableName & " WHERE " & strWHERE & " LIKE '" & strLIKE & "' ORDER BY " & ColumnOrder
        reader = rs.ExecuteReader
        While reader.Read
            cboContent.Items.Add(reader(ColumnValue))
        End While
        Disconnect()
    End Sub

    Public Sub LoadRecordsOnLV(ByRef lvList As ListView, ByVal TableName As String, ByVal NumFields As Integer, ByVal Sort As String)
        lvList.Items.Clear()

        Connect()
        rs.Connection = conn
        rs.CommandText = "SELECT * FROM " & TableName & " ORDER BY '" & Sort & "'"
        reader = rs.ExecuteReader
        While reader.Read
            iItem = New ListViewItem(reader(0).ToString, 0)
            iItem.Checked = False
            For x As Integer = 1 To NumFields Step 1
                iItem.SubItems.Add(reader(x).ToString())
            Next
            lvList.Items.Add(iItem)
        End While
        Disconnect()
    End Sub

    Public Sub LoadRecordsOnLVWithCondition(ByRef lvList As ListView, ByVal TableName As String, ByVal NumFields As Integer, ByVal strWhere As String, ByVal strLike As String, ByVal Sort As String)
        lvList.Items.Clear()

        Connect()
        rs.Connection = conn
        rs.CommandText = "SELECT * FROM " & TableName & " WHERE " & strWhere & " LIKE '" & strLike & "' ORDER BY '" & Sort & "'"
        reader = rs.ExecuteReader
        While reader.Read
            iItem = New ListViewItem(reader(0).ToString, 0)
            iItem.Checked = False
            For x As Integer = 1 To NumFields Step 1
                iItem.SubItems.Add(reader(x).ToString())
            Next
            lvList.Items.Add(iItem)
        End While
        Disconnect()
    End Sub

    Public Sub LoadRecordsOnLVWithTwoCondition(ByRef lvList As ListView, ByVal TableName As String, ByVal NumFields As Integer, ByVal strWhere1 As String, ByVal strLike1 As String, ByVal strWhere2 As String, ByVal strLike2 As String, ByVal Sort As String)
        lvList.Items.Clear()

        Connect()
        rs.Connection = conn
        rs.CommandText = "SELECT * FROM " & TableName & " WHERE " & strWhere1 & " LIKE '" & strLike1 & "' OR " & strWhere2 & " LIKE '" & strLike2 & "' ORDER BY '" & Sort & "'"
        reader = rs.ExecuteReader
        While reader.Read
            iItem = New ListViewItem(reader(0).ToString, 0)
            iItem.Checked = False
            For x As Integer = 1 To NumFields Step 1
                iItem.SubItems.Add(reader(x).ToString())
            Next
            lvList.Items.Add(iItem)
        End While
        Disconnect()
    End Sub

    Public Sub LoadRecordsOnLVSQL(ByRef lvList As ListView, ByVal SQL As String, ByVal NumFields As Integer)
        lvList.Items.Clear()

        Connect()
        rs.Connection = conn
        rs.CommandText = SQL
        reader = rs.ExecuteReader
        While reader.Read
            iItem = New ListViewItem(reader(0).ToString, 0)
            iItem.Checked = False
            For x As Integer = 1 To NumFields Step 1
                iItem.SubItems.Add(reader(x).ToString())
            Next
            lvList.Items.Add(iItem)
        End While
        Disconnect()
    End Sub

    Public Sub LoadRecordsOnLVSQLNoClear(ByRef lvList As ListView, ByVal SQL As String, ByVal NumFields As Integer)
        'lvList.Items.Clear()

        Connect()
        rs.Connection = conn
        rs.CommandText = SQL
        reader = rs.ExecuteReader
        While reader.Read
            iItem = New ListViewItem(reader(0).ToString, 0)
            iItem.Checked = False
            For x As Integer = 1 To NumFields Step 1
                iItem.SubItems.Add(reader(x).ToString())
            Next
            lvList.Items.Add(iItem)
        End While
        Disconnect()
    End Sub

    Public Sub SearchRecordsOnLV(ByRef lvList As ListView, ByVal TableName As String, ByVal NumFields As Integer, ByVal strWhere As String, ByVal strLike As String, ByVal Sort As String)
        lvList.Items.Clear()

        Connect()
        rs.Connection = conn
        rs.CommandText = "SELECT * FROM " & TableName & " WHERE " & strWhere & " LIKE '" & strLike & "%' ORDER BY '" & Sort & "'"
        reader = rs.ExecuteReader
        While reader.Read
            iItem = New ListViewItem(reader(0).ToString, 0)
            iItem.Checked = False
            For x As Integer = 1 To NumFields Step 1
                iItem.SubItems.Add(reader(x).ToString())
            Next
            lvList.Items.Add(iItem)
        End While
        Disconnect()
    End Sub

    '----------------------for License Key generation-------------------------------
    Public Function CpuId() As String
        Dim computer As String = "."
        Dim wmi As Object = GetObject("winmgmts:" & _
                                      "{impersonationLevel=impersonate}!\\" & _
                                      computer & "\root\cimv2")
        Dim processor As Object = wmi.ExecQuery("Select * from " & _
                                                "Win32_Processor")
        Dim cpu_ids As String = ""
        For Each cpu As Object In processor
            cpu_ids = cpu_ids & ", " & cpu.ProcessorId
        Next cpu
        If cpu_ids.Length > 0 Then cpu_ids = _
            cpu_ids.Substring(2)
        Return cpu_ids
    End Function

    '---------------------------------RegEx----------------------------------------

    Public Function FormatDateRegex() As String
        Dim str As String
        str = Regex.Replace(Now.ToString("yyyy/MM/dd hh:mm:ss"), "[\/\ \:/AM]", "")
        Return str
    End Function

    Public Function FormatBDateRegex(ByVal dtPicker As DateTimePicker) As String
        Dim str As String
        str = Regex.Replace(dtPicker.Value.ToString("yyyy/MM/dd hh:mm:ss"), "[\/\ \:/AM]", "")
        Return str
    End Function

    Public Function FormatBDateRegex_String(ByVal bDate As String) As String
        Dim str As String
        str = Regex.Replace(bDate, "[\/\ \:/AM]", "")
        Return str
    End Function

    Public Function CheckSum(ByVal strValue As String) As String
        Dim aList As New ListBox

        aList.Items.Clear()

        Dim HexValue As String
        Dim str As String = strValue
        Dim abData As Byte()

        Dim i As Long
        Dim sum As Double

        abData = Encoding.Default.GetBytes(str)

        Dim strOutput As String

        For i = 0 To abData.Length - 1
            aList.Items.Add(abData(i) & vbCrLf)
            sum += CDbl(aList.Items(i))
        Next i

        HexValue = Hex(sum Mod 256).ToString
        If HexValue.Length = 1 Then
            strOutput = "0" & HexValue
        Else
            strOutput = HexValue
        End If

        Return strOutput

    End Function

    Public Sub SelectResult(ByVal SQL As String, ByVal Value As Integer, ByVal MEASUREMENT As XtraEditors.TextEdit)
        Connect()
        rs.Connection = conn
        rs.CommandText = SQL
        reader = rs.ExecuteReader
        reader.Read()
        If reader.HasRows Then
            MEASUREMENT.Text = reader(Value).ToString
        End If
        Disconnect()
    End Sub

    Function RemoveWhitespace(ByVal fullString As String) As String
        If fullString = "" Then
            Return fullString
        End If

        Return New String(fullString.Where(Function(x) Not Char.IsWhiteSpace(x)).ToArray())
    End Function

#Region "Original Code"
    'Private Sub QueryProcess(ByVal SampleID As String, ByVal Section As String, ByVal Instrument As String, ByVal SerialNo As String)
    '    rs.Parameters.Clear()
    '    rs.Parameters.AddWithValue("@SampleID", SampleID)
    '    rs.Parameters.AddWithValue("@instrument", Instrument & "_" & SerialNo)

    '    Connect()
    '    rs.Connection = conn
    '    rs.CommandText = "SELECT `tmpWorklist`.`sample_id`, `tmpWorklist`.`patient_id`, `tmpWorklist`.`patient_name`, `tmpWorklist`.`sex`, `tmpWorklist`.`bdate`, `patient_order`.`mode` FROM `tmpWorklist`, `patient_order` WHERE `tmpWorklist`.`sample_id` = @SampleID AND `patient_order`.`sample_id` = @SampleID LIMIT 1"
    '    reader = rs.ExecuteReader
    '    reader.Read()
    '    If reader.HasRows Then
    '        PatientID_Out = reader(1).ToString
    '        PatientName_Out = Replace(reader(2).ToString, ", ", "^")
    '        PatientBDate_Out = "" 'DateTime.ParseExact(reader(4), "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyyMMdd")
    '        PatientSex_Out = reader(3).ToString.Substring(0, 1)
    '        OrderSampleID_Out = reader(0).ToString
    '        Disconnect()

    '        MainID = OrderSampleID_Out

    '        Connect()
    '        rs.Connection = conn
    '        rs.CommandText = "SELECT `test_code` FROM `patient_order` WHERE `sample_id` = '" & MainID & "'"
    '        reader = rs.ExecuteReader
    '        While reader.Read
    '            TestList.Add(reader(0).ToString)
    '        End While
    '        Disconnect()

    '        For Each a In TestList
    '            Connect()
    '            rs.Connection = conn
    '            rs.CommandText = "SELECT `channel` FROM `specimen` WHERE `test_code` = '" & a.ToString & "' AND `instrument` = @instrument AND `status` = 'Enable'"
    '            reader = rs.ExecuteReader
    '            reader.Read()
    '            If reader.HasRows Then
    '                OrderTest_Out &= "^^^" & reader(0).ToString & "\"
    '            End If
    '            Disconnect()
    '        Next

    '        Dim HeaderOutput As String
    '        Dim PatientOutput As String
    '        Dim OrderOutput As String
    '        Dim Terminator As String


    '        ''This part is for PentraC400
    '        HeaderOutput = "1H|\^&|||SBSI LIS|||||||P|E1394-97|" & FormatDateRegex()
    '        PatientOutput = "2P|1||" & PatientID_Out & "||" & PatientName_Out & "||" & PatientBDate_Out & "|" & PatientSex_Out & "|||||"
    '        OrderOutput = "3O|1|" & OrderSampleID_Out & "||" & OrderTest_Out.TrimEnd("\") & "|R|" & FormatDateRegex() & "|||||N||||1|"
    '        Terminator = "4L|1|N"
    '        'End

    '        ''This part is for PentraXL80
    '        'HeaderOutput = "1H|\^&|||ABX|||||||P|E1394-97|" & FormatDateRegex()
    '        'PatientOutput = "2P|1||" & PatientID_Out & "||" & PatientName_Out & "||" & PatientBDate_Out & "|" & PatientSex_Out
    '        'OrderOutput = "3O|1|" & OrderSampleID_Out & "||" & OrderTest_Out & "|R|" & FormatDateRegex() & "|||||A"
    '        'Terminator = "4L|1|N"
    '        ''END

    '        ''This part is for Yumizen H500/550
    '        'HeaderOutput = "1H|\^&|||SBSI LIS|||||||P|LIS2-A2|" & FormatDateRegex()
    '        'PatientOutput = "2P|1||" & PatientID_Out & "||" & PatientName_Out & "||" & PatientBDate_Out & "|" & PatientSex_Out & "|||||"
    '        'OrderOutput = "3O|1|" & OrderSampleID_Out & "||" & OrderTest_Out.TrimEnd("\") & "|R|" & FormatDateRegex() & "|||||N||||||||||||||Q|||||"
    '        'Terminator = "4L|1|"
    '        ''End

    '        Dim SumHeader As String = CheckSum(HeaderOutput & CR & ETX)
    '        Dim SumPatient As String = CheckSum(PatientOutput & CR & ETX)
    '        Dim SumOrder As String = CheckSum(OrderOutput & CR & ETX)
    '        Dim SumTerminator As String = CheckSum(Terminator & CR & ETX)

    '        Enquery(Port)
    '        Thread.Sleep(500)
    '        Me.Invoke(Sub() AppendText("******************************Start-Query-Request******************************" & vbNewLine))
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine))
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

    '        'Write file to TraceComm.Log
    '        CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
    '        CommWriter = New StreamWriter(CommLog)
    '        CommWriter.WriteLine("******************************Start-Query-Request******************************" & vbNewLine)
    '        CommWriter.WriteLine(System.DateTime.Now & " [Host] : <ENQ>" & vbNewLine)
    '        CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
    '        CommWriter.Close()
    '        CommLog.Close()

    '        Port.Write(STX & HeaderOutput & CR & ETX & SumHeader & CR & LF)
    '        Thread.Sleep(500)
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>" & vbNewLine))
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

    '        'Write file to TraceComm.Log
    '        CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
    '        CommWriter = New StreamWriter(CommLog)
    '        CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & HeaderOutput & "<CR><ETX>" & SumHeader & "<CR><LF>")
    '        CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
    '        CommWriter.Close()
    '        CommLog.Close()

    '        Port.Write(STX & PatientOutput & CR & ETX & SumPatient & CR & LF)
    '        Thread.Sleep(500)
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & PatientOutput & "<CR><ETX>" & SumPatient & "<CR><LF>" & vbNewLine))
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

    '        'Write file to TraceComm.Log
    '        CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
    '        CommWriter = New StreamWriter(CommLog)
    '        CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & PatientOutput & "<CR><ETX>" & SumPatient & "<CR><LF>")
    '        CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
    '        CommWriter.Close()
    '        CommLog.Close()

    '        Port.Write(STX & OrderOutput & CR & ETX & SumOrder & CR & LF)
    '        Thread.Sleep(500)
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & OrderOutput & "<CR><ETX>" & SumOrder & "<CR><LF>" & vbNewLine))
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

    '        'Write file to TraceComm.Log
    '        CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
    '        CommWriter = New StreamWriter(CommLog)
    '        CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & OrderOutput & "<CR><ETX>" & SumOrder & "<CR><LF>")
    '        CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
    '        CommWriter.Close()
    '        CommLog.Close()

    '        Port.Write(STX & Terminator & CR & ETX & SumTerminator & CR & LF)
    '        Thread.Sleep(500)
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>" & vbNewLine))
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine))

    '        'Write file to TraceComm.Log
    '        CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
    '        CommWriter = New StreamWriter(CommLog)
    '        CommWriter.WriteLine(System.DateTime.Now & " [Host] : <STX>" & Terminator & "<CR><ETX>" & SumTerminator & "<CR><LF>")
    '        CommWriter.WriteLine(System.DateTime.Now & " [Machine] : <ACK>" & vbNewLine)
    '        CommWriter.Close()
    '        CommLog.Close()

    '        EndTrasmission(Port)
    '        Thread.Sleep(500)
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine))
    '        Me.Invoke(Sub() AppendText("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine))

    '        'Write file to TraceComm.Log
    '        CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
    '        CommWriter = New StreamWriter(CommLog)
    '        CommWriter.WriteLine(System.DateTime.Now & " [Host] : <EOT>" & vbNewLine)
    '        CommWriter.WriteLine("******************************End-Query-Request*******************************" & vbNewLine & vbNewLine)
    '        CommWriter.Close()
    '        CommLog.Close()

    '        QueryStatus = False

    '        PatientID_Out = ""
    '        PatientName_Out = ""
    '        PatientBDate_Out = ""
    '        PatientSex_Out = ""

    '        OrderSampleID_Out = ""
    '        OrderTest_Out = ""

    '        TestList.Clear()

    '    Else
    '        QueryStatus = False

    '        ConnectionTimedOut(Port)
    '        Me.Invoke(Sub() AppendText("******************************Error-Communication*******************************" & vbNewLine & vbNewLine))
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : <NAK>" & vbNewLine))
    '        Me.Invoke(Sub() AppendText(System.DateTime.Now & " [Host] : No-Sample-Found" & vbNewLine))
    '        Me.Invoke(Sub() AppendText("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine))

    '        'Write file to TraceComm.Log
    '        CommLog = New FileStream(TraceComm, FileMode.Append, FileAccess.Write)
    '        CommWriter = New StreamWriter(CommLog)
    '        CommWriter.WriteLine("******************************Error-Communication*******************************" & vbNewLine)
    '        CommWriter.WriteLine(System.DateTime.Now & " [Host] : <NAK>" & vbNewLine)
    '        CommWriter.WriteLine(System.DateTime.Now & " [Host] : No-Sample-Found" & vbNewLine)
    '        CommWriter.WriteLine("******************************End-of-Communication*******************************" & vbNewLine & vbNewLine)
    '        CommWriter.Close()
    '        CommLog.Close()
    '    End If
    '    Disconnect()
    'End Sub
#End Region

End Module