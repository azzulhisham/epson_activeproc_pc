Public Class frm_WarnMsg

    Private Sub frm_WarnMsg_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        With Me
            .tmr_Blink.Enabled = False
        End With

    End Sub

    Private Sub frm_WarnMsg_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        If e.KeyValue = Keys.Enter Then
            Me.Close()
            frm_Main.btn_Post.PerformClick()
        End If

    End Sub

    Private Sub frm_WarnMsg_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        With Me
            Me.Location = New Point(My.Computer.Screen.WorkingArea.Width - Me.Size.Width - 60, My.Computer.Screen.WorkingArea.Height - Me.Height - 110)

            With .tmr_Blink
                .Interval = 250
                .Enabled = True
            End With
        End With

    End Sub

    Private Sub frm_WarnMsg_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LostFocus

        Me.Close()

    End Sub

    Private Sub tmr_Blink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_Blink.Tick

        With Me
            .pic_BtnBlink.Visible = Not .pic_BtnBlink.Visible
            If .pic_BtnBlink.Visible = True Then Me.BackColor = Color.Red Else Me.BackColor = Color.White
        End With

    End Sub

    Private Sub pic_BtnBlink_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles pic_BtnBlink.Click, PictureBox1.Click

        Me.Close()
        frm_Main.btn_Post.PerformClick()

    End Sub

    Private Sub frm_WarnMsg_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        Me.Focus()

    End Sub

End Class