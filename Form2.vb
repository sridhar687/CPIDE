Public Class Form2
    Public Sub New(ByVal s As String)
        InitializeComponent()
        RichTextBox1.ReadOnly = True
        RichTextBox1.Text = s
    End Sub

End Class