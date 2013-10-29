Public Class Form4
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If RadioButton1.Checked Then
            Form1.cppenabled = False
            Me.Visible = False
        Else
            Form1.cppenabled = True
            Me.Visible = False
        End If
    End Sub
End Class