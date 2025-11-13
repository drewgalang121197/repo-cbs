Public Class frmLogin

    Public ID As String

    Private Sub btnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
        Me.Dispose()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If (Me.txtUsername.Text = "technician") And (Me.txtPassword.Text = "lis@sbsi.wisdom!2018") Then
                Disconnect()
                MainForm.tbDbaseSettings.Enabled = True
                MainForm.tbRSSettings.Enabled = True
                MainForm.btnSave.Enabled = True
                Me.Hide()
                txtPassword.Text = ""
                txtUsername.Text = ""
            Else
                MessageBox.Show("Invalid Username or Password.", "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Disconnect()
                Exit Sub
            End If
            Disconnect()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try
    End Sub
End Class