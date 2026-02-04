Public Class About

    Public ReadOnly Property VERSION As String = "Public Beta (v0.1)"
    Private Sub okBtn_Click(sender As Object, e As EventArgs) Handles okBtn.Click
        Hide()
    End Sub

    Private Sub About_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        verLabel.Text = VERSION
        Try
            PictureBox1.Image = Image.FromFile(Application.StartupPath & "\banner.png")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LinkLabel21_Click(sender As Object, e As EventArgs) Handles LinkLabel21.Click
        Process.Start("https://github.com/cbladeOfficial/spectrum")
    End Sub

    Private Sub LinkLabel22_Click(sender As Object, e As EventArgs) Handles LinkLabel22.Click
        Process.Start("https://discord.gg/Ew4tvf4vuK")
    End Sub

    Private Sub LinkLabel23_Click(sender As Object, e As EventArgs) Handles LinkLabel23.Click
        Process.Start("https://cbladeofficial.is-a.dev/projects/spectrum")
    End Sub

    Private Sub LinkLabel24_Click(sender As Object, e As EventArgs) Handles LinkLabel24.Click
        Process.Start("https://cbladeofficial.is-a.dev/projects/spectrum/guide")
    End Sub
End Class