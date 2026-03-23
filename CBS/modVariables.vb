Imports System.IO
Imports System.IO.Ports
Imports System.Threading
Imports System.Threading.Thread
Imports MySql.Data.MySqlClient

Module modVariables

#Region "Database Connection Variables"
    '----------------------------Database Connection String Variables-------------------------
    Public ServerName = My.Settings.HostName
    Public UID = My.Settings.UID
    Public PWD = My.Settings.PWD = ""
    Public DBName = My.Settings.DBName

    '----------------------------Database Connection String-------------------------
    Public MyConnectionString = My.Settings.ConnectionString
#End Region

#Region "MySQL.Data.MySQLClient Variables"
    '---------------------------------MySQL Variables-------------------------------
    Public conn As New MySqlConnection
    Public rs As New MySqlCommand
    Public reader As MySqlDataReader
    Public adapter As MySqlDataAdapter
#End Region

#Region "Global Declaration"
    '------------------------------ListView Subitem Handler-------------------------
    Public iTemSubItems As New ListViewItem
    Public iItem As New ListViewItem
    '------------------------------------User Details-------------------------------
#Region "Connection to Database Function"
    Public Sub Connect()
        Try
            conn.ConnectionString = MyConnectionString
            conn.Open()
        Catch ex As MySqlException
            MessageBox.Show(ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            conn.Close()
            Exit Sub
        End Try
    End Sub

    Public Sub Disconnect()
        If conn.State = ConnectionState.Open Then
            conn.Close()
        End If
    End Sub
#End Region

#End Region

End Module
