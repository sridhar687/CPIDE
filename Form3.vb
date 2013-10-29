Public Class Form3
    
    Public Sub New(ByRef ip As String, ByRef user As String, ByRef pass As String)
        InitializeComponent()
        

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Form1.serverip = TextBox1.Text
        Form1.user = TextBox2.Text
        Form1.pass = MaskedTextBox2.Text
        Me.Dispose()
    End Sub
End Class