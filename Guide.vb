Public Class Guide

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        RichTextBox1.Visible = False
        Topics.SelectionMode = SelectionMode.One
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub Topics_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Topics.SelectedIndexChanged
        If CType(Topics.SelectedItem, String) = "Starting Off" Then
            RichTextBox1.Visible = False
            RichTextBox1.Text = "Welcome to this IDE. "
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.Dispose()
    End Sub
End Class