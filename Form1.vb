Imports System.IO.File
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Drawing.Drawing2D
Imports System.Text
Imports System.Threading
Imports System.ComponentModel
Imports Tamir.SharpSsh 'Library implementing SSH protocol
'Cross-Platform IDE for C/C++ - A mini project

Public Class Form1
    Dim mc As MatchCollection
    Dim i As Integer
    Dim dbg As Debug
    Dim state As State
    Public Shared cppenabled As Boolean = False 'detecting whether current program is C/C++
    Dim s As New SyntaxHighlight 'Object for highlighting syntax
    Dim undoStack As New Stack(Of String) 'stack for undo
    Dim redoStack As New Stack(Of String) 'stack for redo
    Dim m1 As New MenuItem("Cut") 'Context menu items
    Dim m2 As New MenuItem("Copy")
    Dim m3 As New MenuItem("Paste")
    Dim m4 As New MenuItem("Undo")
    Dim m5 As New MenuItem("Redo")
    Dim tabc As New TabControl
    Dim tabpages(10) As TabPage
    Dim rtb(10) As RichTextBox
    Dim num As Integer = 0
    Dim ACList As New ListBox
  '  Dim ac As AutoCompleteList
    Dim transfer As Boolean = False
    Dim filename As String
    Public Shared serverip As String, user As String, pass As String
    Dim undoBuffer As String = ""
    Dim statusBar As New StatusBar
    Dim statusBarPanel As New StatusBarPanel
   '' Dim autocompleteenabled As Boolean = True
    Dim pages(10) As Boolean
    Dim bw As New BackgroundWorker
    Dim saved As Boolean = False
	Dim find as Regex
    Private Sub NewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripMenuItem.Click
        Dim form4 As New Form4
        form4.ShowDialog()
        NewFile()
    End Sub

    Public Sub NewFile()
        If num < 9 Then
l1:         num = num + 1

            Dim contextMenu As New ContextMenu()
            If Clipboard.GetText Is Nothing Then
                m3.Enabled = False
            End If
            Dim contextmenu2 As New ContextMenu
            rtb(num) = New RichTextBox
            tabpages(num) = New TabPage
            contextMenu.MenuItems.Add(m4)
            contextMenu.MenuItems.Add(m5)
            contextMenu.MenuItems.Add(m1)
            contextMenu.MenuItems.Add(m2)
            contextMenu.MenuItems.Add(m3)
            Dim closeAllButThis As New MenuItem("Close ALL but THIS")
            Dim closeThis As New MenuItem("Close this")
            AddHandler closeAllButThis.Click, AddressOf close_AllButThis
            contextmenu2.MenuItems.Add(closeAllButThis)
            contextmenu2.MenuItems.Add(closeThis)
            rtb(num).ContextMenu = contextMenu
            AddHandler m1.Click, AddressOf CutToolStripMenuItem_Click
            AddHandler m2.Click, AddressOf CopyToolStripMenuItem_Click
            AddHandler m3.Click, AddressOf PasteToolStripMenuItem_Click
            rtb(num).Font = New Font("Consolas", 10, FontStyle.Regular)
            AddHandler rtb(num).TextChanged, AddressOf RichTextBox1_TextChanged_1
            AddHandler rtb(num).PreviewKeyDown, AddressOf rtb_PreviewKeyDown
            AddHandler rtb(num).MouseClick, AddressOf rtb_MouseClicked
            AddHandler rtb(num).KeyUp, AddressOf rtb_KeyUp
            AddHandler rtb(num).KeyPress, AddressOf rtb_KeyPress
            tabc.ContextMenu = contextmenu2
            rtb(num).AcceptsTab = True
            rtb(num).Controls.Add(ACList)
            rtb(num).Dock = DockStyle.Fill
            tabpages(num).Controls.Add(rtb(num))
            tabc.Controls.Add(tabpages(num))
            Me.Controls.Add(tabc)
            If cppenabled = True Then
                tabpages(num).Text = "Untitled" & num & ".cpp"
            Else
                tabpages(num).Text = "Untitled" & num & ".c"
            End If
            tabpages(num).Show()
            tabc.Enabled = True
            rtb(num).Visible = True
            rtb(num).BringToFront()
            tabc.SelectedTab = tabpages(num)
            tabpages(num).ContextMenu = contextmenu2
            AddHandler rtb(num).KeyUp, AddressOf rtb_KeyUp
            tabc.ShowToolTips = True
            tabc.SelectedTab.ToolTipText = tabc.SelectedTab.Text
            updateLine()
            SaveAsToolStripMenuItem.Enabled = True
            SaveToolStripMenuItem.Enabled = True
            RemoteSaveToolStripMenuItem.Enabled = True
            statusBar.Visible = True
        Else
            For i As Integer = 0 To 9 Step 1
                If pages(i) = False Then
                    num = i
                    GoTo l1
                End If
            Next
        End If
    End Sub

    Private Sub rtb_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)
        If e.KeyChar <> Convert.ToChar(Keys.Space) Then
            undoBuffer = undoBuffer & e.KeyChar
          ''  MsgBox(undoBuffer)
        ElseIf e.KeyChar = Convert.ToChar(Keys.Space) Then
            undoStack.Push(undoBuffer)
            UndoToolStripMenuItem.Enabled = True
            RedoToolStripMenuItem.Enabled = True
        End If

    End Sub
    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Dispose()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        If num < 9 Then
l1:         num = num + 1
            rtb(num) = New RichTextBox
            tabpages(num) = New TabPage
            Dim contextmenu2 As New ContextMenu
            Dim contextMenu As New ContextMenu()
            contextMenu.MenuItems.Add(m4)
            contextMenu.MenuItems.Add(m5)
            contextMenu.MenuItems.Add(m1)
            contextMenu.MenuItems.Add(m2)
            contextMenu.MenuItems.Add(m3)
            rtb(num).ContextMenu = contextMenu
            AddHandler m1.Click, AddressOf CutToolStripMenuItem_Click
            AddHandler m2.Click, AddressOf CopyToolStripMenuItem_Click
            AddHandler m3.Click, AddressOf PasteToolStripMenuItem_Click
            rtb(num).Font = New Font("Consolas", 10, FontStyle.Regular)
            AddHandler rtb(num).TextChanged, AddressOf RichTextBox1_TextChanged_1
            AddHandler rtb(num).MouseClick, AddressOf rtb_MouseClicked
            Dim closeAllButThis As New MenuItem("Close ALL but THIS")
            Dim closeThis As New MenuItem("Close this")
            AddHandler closeAllButThis.Click, AddressOf close_AllButThis
            contextmenu2.MenuItems.Add(closeAllButThis)
            contextmenu2.MenuItems.Add(closeThis)
            rtb(num).AcceptsTab = True
            rtb(num).Dock = DockStyle.Fill
            tabpages(num).Controls.Add(rtb(num))
            tabc.ContextMenu = contextmenu2
            tabc.Controls.Add(tabpages(num))
            Me.Controls.Add(tabc)
            Dim myStreamReader As System.IO.StreamReader
            OpenFileDialog1.Filter = "C(*.c)|*.c|C++ (*.cpp)|*.cpp"
            OpenFileDialog1.CheckFileExists = True
            OpenFileDialog1.ShowDialog(Me)
            SaveAsToolStripMenuItem.Enabled = True
            SaveToolStripMenuItem.Enabled = True
            RemoteSaveToolStripMenuItem.Enabled = True
            Try
                OpenFileDialog1.OpenFile()
                myStreamReader = System.IO.File.OpenText(OpenFileDialog1.FileName)
                If OpenFileDialog1.FileName.EndsWith("cpp") Then
                    cppenabled = True
                End If
                rtb(num).Text = myStreamReader.ReadToEnd()
                tabpages(num).Text = OpenFileDialog1.FileName
            Catch ex As Exception
                Me.Dispose()
                ' Do nothing on Exception
            End Try
            statusBar.Visible = True
            If tabc.SelectedTab.Text.EndsWith("cpp") Then
                cppenabled = True
            Else
                cppenabled = False
            End If
            tabc.SelectedTab.ContextMenu = contextmenu2
            pages(num) = True
        Else
            For i As Integer = 0 To 9 Step 1
                If pages(i) = False Then
                    num = i
                    GoTo l1
                End If
            Next
        End If
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        cppenabled = False
    End Sub

    Private Sub rtb_MouseClicked(ByVal sender As Object, ByVal e As MouseEventArgs)
        updateLine()
    End Sub
    Private Sub rtb_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
       '' If autocompleteenabled Then
         ''   ac.rtb_KeyUp(CType(tabc.SelectedTab.Controls.Item(0), RichTextBox), ACList, e)
       '' End If
    End Sub
    Private Sub LB_KeyUp(ByVal sender As Object, ByVal E As KeyEventArgs)
    ''    If autocompleteenabled Then
         
      ''  End If
    End Sub
    Private Sub CPP_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        cppenabled = True
    End Sub
    Private Sub bw_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)
        For i As Integer = 1 To 10 Step 1
            If bw.CancellationPending = True Then
                e.Cancel = True
                Exit For
            Else
                ' Perform a time consuming operation and report progress.
                System.Threading.Thread.Sleep(500)
                bw.ReportProgress(i * 10)
            End If
        Next
    End Sub
    Private Sub bw_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)
        Me.ProgressBar2.Value = e.ProgressPercentage
        If ProgressBar2.Value = 100 Then
            bw.CancelAsync()
            ProgressBar2.Visible = False
            Me.Enabled = True
        End If
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    	Me.Enabled = False
    	state =New State
        ProgressBar2.Dock = DockStyle.Bottom
        bw.WorkerSupportsCancellation = True
        bw.WorkerReportsProgress = True
        AddHandler bw.DoWork, AddressOf bw_DoWork
        AddHandler bw.ProgressChanged, AddressOf bw_ProgressChanged
        bw.RunWorkerAsync()
        Me.Text = "Cross Platform IDE for C/C++"
        ACList.Visible = False
     ''   ac = New AutoCompleteList(cppenabled)
        AddHandler ACList.KeyUp, AddressOf LB_KeyUp
        statusBar.ShowPanels = True
        statusBar.Panels.Add(statusBarPanel)
        Me.Controls.Add(statusBar)
        statusBar.Visible = False
        statusBar.Dock = DockStyle.Bottom
      ''  undoStack.Push("")
        For i As Integer = 0 To 9 Step 1
            pages(num) = New Boolean
            pages(num) = False
            num = num + 1
        Next
        num = 0
        SaveAsToolStripMenuItem.Enabled = False
        SaveToolStripMenuItem.Enabled = False
        RemoteSaveToolStripMenuItem.Enabled = False
        Dim p As Point = tabc.Location
        p.Y = p.Y + 25
        tabc.Location = p
        tabc.Size = New Size(1268, 690)
        RunToolStripMenuItem1.Enabled = False
        StacktraceToolStripMenuItem.Enabled = False
        AddHandler tabc.SelectedIndexChanged, AddressOf updateRTB
    End Sub
    Private Sub updateRTB(ByVal sender As Object, ByVal e As EventArgs)
        s.ColorText(CType(tabc.SelectedTab.Controls.Item(0), RichTextBox), cppenabled)
    End Sub
    Private Sub close_AllButThis(ByVal sender As Object, ByVal e As EventArgs)
        Dim index As Integer = tabc.SelectedIndex
        pages(index) = True
        For i As Integer = 0 To 9
            If pages(i) = True And i <> index Then
                tabpages(i).Dispose()
            End If
        Next
        SaveAsToolStripMenuItem.Enabled = False
        SaveToolStripMenuItem.Enabled = False
        RemoteSaveToolStripMenuItem.Enabled = False
    End Sub
    Private Sub updateLine()
        Dim Position As Integer = CType(tabc.SelectedTab.Controls.Item(0), RichTextBox).SelectionStart
        Dim Line As Integer = CType(tabc.SelectedTab.Controls.Item(0), RichTextBox).GetLineFromCharIndex(Position)
        Dim Col As Integer = Position - CType(tabc.SelectedTab.Controls.Item(0), RichTextBox).GetFirstCharIndexOfCurrentLine
        statusBarPanel.Text = "Line - " & Line & " , Col - " & Col
    End Sub
    Private Sub rtb_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.EventArgs)
        
    End Sub
    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        If saved = False Then
            Dim rtb As RichTextBox = tabc.SelectedTab.Controls.Item(0)
            Dim myStreamWriter As StreamWriter
            SaveFileDialog1.Filter = "C (*.c)|*.c|C++ (*.cpp)|*.cpp"
            SaveFileDialog1.CheckPathExists = True
            SaveFileDialog1.ShowDialog(Me)
            Try
                myStreamWriter = File.CreateText(SaveFileDialog1.FileName)
                myStreamWriter.Write(rtb.Text)
                myStreamWriter.Flush()
                tabc.SelectedTab.Text = SaveFileDialog1.FileName
            Catch ex As Exception

            End Try
        ElseIf tabc.SelectedTab.Text.Contains("*") Then
            Dim mystreamWriter As StreamWriter = File.CreateText(tabc.SelectedTab.Text)
            mystreamWriter.Write(tabc.SelectedTab.Controls.Item(0).Text)
            mystreamWriter.Flush()
            tabc.SelectedTab.Text.Remove(0, 1)
        End If
    End Sub
   
    Private Sub CompileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CompileToolStripMenuItem.Click
        compile()
    End Sub
    Private Sub compile()
        Dim ssh As New Scp(serverip, user, pass)
        ssh.Connect()
        If ssh.Connected = True Then
            If cppenabled = False Then
                filename = InputBox("Enter the remote filename")
                ssh.Put(tabc.SelectedTab.Text, filename & ".c")
            Else
                filename = InputBox("Enter the remote filename")
                ssh.Put(tabc.SelectedTab.Text, filename & ".cpp")
            End If
        End If
        Dim sshexec As New SshExec(serverip, user, pass)
        Dim stdout As String = ""
        Dim stderr As String = ""
        sshexec.Connect()
        If sshexec.Connected Then
            If cppenabled Then
                sshexec.RunCommand("g++ -g " & filename & ".cpp", stdout, stderr)
                If stdout <> "" Then
                    MsgBox("Compiled successfully")
                Else
                    Dim debug As New Form2(stderr)
                    debug.Show()
                End If
            Else
                sshexec.RunCommand("gcc -g " & filename & ".c", stdout, stderr)
                If stderr = "" Then
                    MsgBox("Compiled successfully")
                Else
                    MsgBox("Compilation error")
                    Dim debug As New Form2(stderr)
                    debug.Show()
                End If
            End If
        End If
    End Sub
    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        tabc.SelectedTab.Controls.Item(0).Visible = True
        tabpages(num).Show()
        tabc.Enabled = True
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Dispose()
    End Sub
    Public Sub CutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CutToolStripMenuItem.Click
    	state.undoPush(tabc.SelectedTab.Controls.Item(0))
    	Dim rtb1 As RichTextBox = tabc.SelectedTab.Controls.Item(0)
        If rtb1.SelectedText = Nothing Then
            MsgBox("No text selected")
            Exit Sub
        End If
        Clipboard.SetText(rtb1.SelectedText)
        Ctype(tabc.SelectedTab.Controls.Item(0),RichTextBox).Text = ""
        updateLine()
        CType(tabc.SelectedTab.Controls.Item(0), RichTextBox).DeselectAll()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToolStripMenuItem.Click
        If Ctype(tabc.SelectedTab.Controls.Item(0),RichTextBox).SelectedText = Nothing Then
            MsgBox("No text selected")
            Exit Sub
        End If
        Clipboard.SetText(CType(tabc.SelectedTab.Controls.Item(0),RichTextBox).SelectedText)
         End Sub
    Private Sub PasteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteToolStripMenuItem.Click
    	state.undoPush(CType(tabc.SelectedTab.Controls.Item(0),RichTextBox))
    	If Clipboard.GetText = Nothing Then
            MsgBox("No text in clipboard")
            Exit Sub
        End If
        Dim start As Integer = Ctype(tabc.SelectedTab.Controls.Item(0),RichTextBox).selectionStart
        Ctype(tabc.SelectedTab.Controls.Item(0),RichTextBox).SelectedText = Clipboard.GetText
        Ctype(tabc.SelectedTab.Controls.Item(0),RichTextBox).DeselectAll()
        updateLine()
    End Sub

    Private Sub RemoteSaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoteSaveToolStripMenuItem.Click
        Dim inp As String = InputBox("Enter remote file name")
        If cppenabled Then
            inp = inp & ".cpp"
        Else
            inp = inp & ".c"
        End If
        Dim scp As New Scp(serverip, user, pass)
        scp.Connect()
        If scp.Connected Then
            scp.Put(tabc.SelectedTab.Text, inp)
            MsgBox("File successfully saved as " & inp & " on server")
        End If
    End Sub

    Private Sub UndoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UndoToolStripMenuItem.Click
    	Dim rtb As RichTextBox=CType(tabc.SelectedTab.Controls.Item(0),RichTextBox)
    	state.undoPage()
    	rtb.Text=state.undoPage().text
    End Sub

    Private Sub RedoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RedoToolStripMenuItem.Click
    	Dim rtb As RichTextBox=CType(tabc.SelectedTab.Controls.Item(0),RichTextBox)
    	rtb=state.redoPage()
    End Sub



    Private Sub GuideToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        MsgBox(" Welcome to this IDE ")
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub RichTextBox1_TextChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
    	If tabc.SelectedTab.Controls.Item(0) IsNot Nothing
    		state.undoPush(CType(tabc.SelectedTab.Controls.Item(0),RichTextBox))
    		End If
    	If tabc.SelectedTab.Text.Contains("*") = False Then
            tabc.SelectedTab.Text = "*" & tabc.SelectedTab.Text
        End If
        updateLine()
        s.ColorText(CType(tabc.SelectedTab.Controls.Item(0), RichTextBox), cppenabled)
        saved = False
    End Sub

    Private Sub ViewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ViewToolStripMenuItem.Click

    End Sub

    Private Sub FindToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FindToolStripMenuItem.Click
        Panel1.Visible = True
    End Sub

    Private Sub ExecuteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExecuteToolStripMenuItem.Click
        execute()
    End Sub
    Private Sub execute()
        Try
            Dim sshs As New SshShell(serverip, user, pass)
            sshs.Connect()
            If sshs.Connected Then
                sshs.Expect("$")
                sshs.WriteLine("./a.out")
                Dim output As String = sshs.Expect()
                If output <> Nothing Or output.Contains("$") = False Then
                    Dim input As String = InputBox(output)
                    sshs.WriteLine(input)
                    output = sshs.Expect("\n")
                End If
                Dim debug As New Form2(output)
                debug.Show()
            End If
        Catch e As NullReferenceException
            MsgBox("Enter settings before proceeding")
        End Try
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsToolStripMenuItem.Click
        Dim settings As New Form3(serverip, user, pass)
        settings.Show()
    End Sub

    Private Sub BreakpointsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BreakpointsToolStripMenuItem.Click
        dbg = New Debug(serverip, user, pass)
        compile()
        Dim input As String = InputBox("Enter line number for breakpoint")
        dbg.breakpoint(Convert.ToInt32(input))
        RunToolStripMenuItem1.Enabled = True
        StacktraceToolStripMenuItem.Enabled = True
        PrintVariableValueToolStripMenuItem.Enabled = True
    End Sub

    Private Sub StacktraceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StacktraceToolStripMenuItem.Click
        dbg.stacktrace()
    End Sub

    Private Sub RunToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunToolStripMenuItem1.Click
        dbg.run()
    End Sub

    Private Sub EnabledToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EnabledToolStripMenuItem.Click
        If EnabledToolStripMenuItem.Checked = False Then
            ''autocompleteenabled = False
        End If
    End Sub

    Private Sub AutoCompleteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoCompleteToolStripMenuItem.Click

    End Sub

    Private Sub HelpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HelpToolStripMenuItem.Click

    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        AboutBox1.Visible = True
    End Sub

    Private Sub GuideToolStripMenuItem_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GuideToolStripMenuItem.Click
        Me.IsMdiContainer = True
        Guide.Visible = True
    End Sub
    Private Sub PrintVariableValueToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintVariableValueToolStripMenuItem.Click
        Dim inp As String = InputBox("Enter variable name")
        dbg.print(inp)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim a As String = TextBox1.Text
        If a <> Nothing Then
            find = New Regex(a)
            mc = find.Matches(CType(tabc.SelectedTab.Controls.Item(0), RichTextBox).Text)
            If i >= mc.Count Then
                MsgBox("Word not found")
                Panel1.Visible = False
                Me.Enabled = True
                i = 0
            End If
            Dim m As Match = mc.Item(i)
            i = i + 1
            CType(tabc.SelectedTab.Controls.Item(0), RichTextBox).Select(m.Index, m.Length)
            CType(tabc.SelectedTab.Controls.Item(0), RichTextBox).Focus()
        Else
            MsgBox("Enter pattern")
        End If
    End Sub
End Class

Public Class SyntaxHighlight
    Dim regexBlue As New Regex("\b(int|char|float|double|long|short|unsigned|void)\b")
    Dim regexMaroon As New Regex("#include<([^>]*)>|#ifndef|#define|#ifdef|\\[a-zA-Z]+")
    Dim regexYellow As New Regex("\b(for|\bwhile\b|do|if|else|break|case|switch|auto|static|extern|register|volatile|return|goto|continue)\b")
    Dim regexRed As New Regex("(NULL|[0-9][0-9]*|"".*"")")
    Dim regexPurple As New Regex("\b(struct|union|sizeof|new|delete|typedef)\b")
    Dim regexCPP As New Regex("\b(public|private|protected|virtual|bool|abstract|try|catch|template|friend|inline|namespace|operator|this|enum|new|delete|class)\b")
    Dim regexComments As New Regex("//[^\n]*|/\*[^(\*/)]\*/|/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/")
    Dim mCollectionBlue As MatchCollection
    Private Declare Function LockWindowUpdate Lib "user32" (ByVal hWnd As Integer) As Integer
    Public Sub ColorText(ByVal rtb As RichTextBox, ByVal cpp As Boolean)
        LockWindowUpdate(rtb.Handle.ToInt32)
        Dim selectStart As Integer = rtb.SelectionStart
        mCollectionBlue = regexBlue.Matches(rtb.Text)
        rtb.SelectAll()
        rtb.SelectionColor = Color.Black
        rtb.SelectionStart = selectStart
        For Each m As Match In mCollectionBlue
            rtb.Select(m.Index, m.Length)
            rtb.SelectionColor = Color.Blue
            rtb.SelectionStart = rtb.TextLength
        Next
        rtb.SelectionStart = selectStart
        Dim mCollectionRed As MatchCollection
        mCollectionRed = regexRed.Matches(rtb.Text)
        If mCollectionRed.Count = 0 Then
            rtb.SelectionColor = Color.Black
        End If
        rtb.SelectionStart = selectStart
        For Each m As Match In mCollectionRed
            rtb.Select(m.Index, m.Length)
            rtb.SelectionColor = Color.Red
            rtb.SelectionStart = rtb.TextLength
        Next
        rtb.SelectionStart = selectStart
        Dim mCollectionMaroon As MatchCollection = regexMaroon.Matches(rtb.Text)
        For Each m As Match In mCollectionMaroon
            rtb.Select(m.Index, m.Length)
            rtb.SelectionColor = Color.Maroon
            rtb.SelectionStart = rtb.TextLength
        Next
        rtb.SelectionStart = selectStart
        Dim mCollectionComments As MatchCollection = regexComments.Matches(rtb.Text)
        For Each m As Match In mCollectionComments
            rtb.Select(m.Index, m.Length)
            rtb.SelectionColor = Color.Green
            rtb.SelectionStart = rtb.TextLength
        Next
        rtb.SelectionStart = selectStart
        Dim mCollectionPurple As MatchCollection = regexPurple.Matches(rtb.Text)
        For Each m As Match In mCollectionPurple
            rtb.Select(m.Index, m.Length)
            rtb.SelectionColor = Color.Purple
            rtb.SelectionStart = rtb.TextLength
        Next
        rtb.SelectionStart = selectStart
        Dim mCollectionYellow As MatchCollection = regexYellow.Matches(rtb.Text)
        For Each m As Match In mCollectionYellow
            rtb.Select(m.Index, m.Length)
            rtb.SelectionColor = Color.Green
            rtb.SelectionStart = rtb.TextLength
        Next
        rtb.SelectionStart = selectStart
        If cpp = True Then
            Dim mCollectionCPP As MatchCollection = regexCPP.Matches(rtb.Text)
            For Each m As Match In mCollectionCPP
                rtb.Select(m.Index, m.Length)
                rtb.SelectionColor = Color.YellowGreen
            Next
        End If
        rtb.SelectionStart = selectStart
        LockWindowUpdate(0)
    End Sub
End Class

Class Debug
    Dim sshs As SshShell
    Public Sub New(ByVal ipaddr As String, ByVal user As String, ByVal pass As String)
        sshs = New SshShell(ipaddr, user, pass)
        sshs.Connect()
        If sshs.ShellConnected Then
            sshs.ExpectPattern = "$"
            sshs.WriteLine("gdb a.out")
        End If
    End Sub
    Public Sub breakpoint(ByVal line As Integer)
        Dim output As String = sshs.Expect("(gdb)")
        sshs.WriteLine("break " & line)
        output = sshs.Expect("(gdb)")
        Dim debug1 As New Form2(output)
        debug1.Show()
    End Sub
    Public Sub stacktrace()
        sshs.WriteLine("backtrace")
        Dim output As String = sshs.Expect("(gdb)")
        Dim debug As New Form2(output)
        debug.Show()
    End Sub
    Public Sub run()
        Try
            sshs.Expect("(gdb)")
            sshs.WriteLine("run")
            Dim output As String = sshs.Expect("(gdb)")
            Dim debug As New Form2(output)
            debug.Show()
        Catch e As Exception
            MsgBox(e.ToString)
        End Try
    End Sub
    Public Sub print(ByVal var As String)
        sshs.WriteLine("print " & var)
        Dim output As String = sshs.Expect("(gdb)")
        Dim debug As New Form2(output)
        debug.Show()
    End Sub
End Class
Class State
	Public tabpage As RichTextBox
	''Public sts As StatusStrip
	Public undo As New Stack(Of RichTextBox)
	Public redo As New Stack(Of RichTextBox)
	
	Public Sub undoPush(ByRef tp As RichTextBox)
		
				undo.Push(tp)
			'MsgBox(tp.Text)
	
	End Sub
	Public function undoPage() As RichTextBox
		If undo.Count>0 Then
			Dim tp As RichTextBox=undo.Pop
			redo.Push(tp)
			Return tp
		End If
		Return nothing
	End Function
	Public Function redoPage() As RichTextBox
		If redo.Count>0 Then
			Dim tp As RichTextBox=redo.Pop
			Return tp
		End If
		Return nothing
	End Function
End Class

