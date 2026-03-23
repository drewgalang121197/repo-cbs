Public Class frmLogout

    Public ID As String

    Private Sub btnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
        Me.Dispose()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If (Me.txtUsername.Text = "technician") And (Me.txtPassword.Text = "lis@sbsi.wisdom!2018") Then
                MainForm.ExitMessage()

                'Get Connection String value from Registry
                Dim TXTConnectionString As String = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\KeyString\CS 1.0.0", "ConnectionString", Nothing)
                '#########################################################################################################################################################

                'Update Machine Status
                conn.ConnectionString = TXTConnectionString
                conn.Open()
                rs.Connection = conn
                rs.CommandText = "SELECT * FROM `machines` WHERE `name` = '" & MainForm.txtMachine.Text & "' AND `serial_no` = '" & MainForm.txtSerial.Text & "'"
                reader = rs.ExecuteReader
                reader.Read()
                If reader.HasRows Then
                    conn.Close()
                    conn.ConnectionString = TXTConnectionString
                    conn.Open()
                    rs.Connection = conn
                    rs.CommandText = "UPDATE `machines` SET `status` = 'Disconnected' WHERE `name` = '" & MainForm.txtMachine.Text & "' AND `serial_no` = '" & MainForm.txtSerial.Text & "'"
                    rs.ExecuteNonQuery()
                    conn.Close()
                End If
                conn.Close()
                '############################################################################################################################################################
                End
            Else
                MessageBox.Show("Invalid Username or Password.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Sub
            End If
            Disconnect()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try
    End Sub
End Class