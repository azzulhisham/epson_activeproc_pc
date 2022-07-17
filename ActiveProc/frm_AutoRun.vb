Public Class frm_AutoRun

    Dim MyAnimation(4) As PictureBox
    Dim ProcNo As Integer = 0
    Dim ProcTime As Integer = 0


    Private Sub frm_AutoRun_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        With ActiveProc
            .IO.o5_Cover_Sol.Trigger_OFF()
        End With

        With Me
            .tmr_Animate.Enabled = False
            .tmr_AutoRun.Enabled = False
            .Dispose()
        End With

    End Sub

    Private Sub frm_AutoRun_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        MyAnimation(0) = New PictureBox
        MyAnimation(1) = New PictureBox
        MyAnimation(2) = New PictureBox
        MyAnimation(3) = New PictureBox
        MyAnimation(4) = New PictureBox

        With Me
            .MyAnimation(0) = .pic_1
            .MyAnimation(1) = .pic_2
            .MyAnimation(2) = .pic_3
            .MyAnimation(3) = .pic_4
            .MyAnimation(4) = .pic_5

            With .tmr_Animate
                .Interval = 300
                .Enabled = True
            End With

            .ProcNo = 0
            .ProcTime = 0
        End With

    End Sub

    Private Sub tmr_Animate_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_Animate.Tick

        Static PicNo As Integer


        With Me
            .pic_Progress.BackgroundImage = .MyAnimation(PicNo).BackgroundImage
            PicNo += 1

            If PicNo >= 5 Then
                PicNo = 0
            End If
        End With

    End Sub

    Private Sub frm_AutoRun_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        Dim wt As Integer = 0

        With ActiveProc
            Me.lbl_ProcNo.Text = "Checking Safety Cover..."

            If .IO.i2_sw_Cover.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                .ErrorMsg = "Safety cover not closed..."
                frm_Alarm.ShowDialog(Me)

                Me.Close()
                Exit Sub
            End If


            Me.lbl_ProcNo.Text = "Checking target..."

            If .IO.i11_sw_TraySensor.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                .ErrorMsg = "No Tray..."
                frm_Alarm.ShowDialog(Me)

                Me.Close()
                Exit Sub
            End If

            Me.lbl_ProcNo.Text = "Lock Safety Cover..."
            .IO.o5_Cover_Sol.Trigger_ON()

            Me.lbl_ProcNo.Text = "Checking Tray position..."

            If .IO.i7_sw_slsRight.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                wt = My.Computer.Clock.TickCount
                .IO.o4_Tray_Sol.Trigger_OFF()

                Do Until .IO.i6_sw_slsLeft.BitState = cls_PCIBoard.BitsState.eBit_OFF
                    Application.DoEvents()

                    If My.Computer.Clock.TickCount > wt + 3000 Then
                        .ErrorMsg = "Cylinder Sensor (Left) abnormal..."
                        frm_Alarm.ShowDialog(Me)

                        Me.Close()
                        Exit Sub
                    End If
                Loop

                wt = My.Computer.Clock.TickCount

                Do Until .IO.i7_sw_slsRight.BitState = cls_PCIBoard.BitsState.eBit_ON
                    Application.DoEvents()

                    If My.Computer.Clock.TickCount > wt + 3000 Then
                        .ErrorMsg = "Cylinder Sensor (Right) abnormal..."
                        frm_Alarm.ShowDialog(Me)

                        Me.Close()
                        Exit Sub
                    End If
                Loop
            End If


            If ._LaserIUnit = LaserUnit.ML7110B Then
                Dim RetData As String = String.Empty
                Dim RetCmd As Integer = ML7111A_cmd("ERR", RetData)
            End If

            wt = My.Computer.Clock.TickCount
            Me.lbl_ProcNo.Text = "Checking LD Status..."

            If .IO.i9_in_LaserRdy.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                Me.lbl_ProcNo.Text = "Turning <ON> LD..."

                If frm_Main.Set_ML_Mode() >= 0 Then
                    Dim ML_Cmd As String = "LMW1"
                    Dim RetCmd As Integer = frm_Main.SendMLCmd(ML_Cmd)
                End If

                Do Until .IO.i9_in_LaserRdy.BitState = cls_PCIBoard.BitsState.eBit_ON
                    Application.DoEvents()

                    If My.Computer.Clock.TickCount > wt + 18000 Then
                        .ErrorMsg = "Laser not ready..."
                        frm_Alarm.ShowDialog(Me)

                        Me.Close()
                        Exit Sub
                    End If
                Loop
            End If

            Me.lbl_Progress.Text = "Marking in progress..."
            Me.lbl_ProcNo.Text = "Ready to start..."
            .IO.o2_LaserRdy_LED.Trigger_ON()
        End With

        With Me
            With .tmr_AutoRun
                .Interval = 30
                .Enabled = True
            End With

            .ProcTime = My.Computer.Clock.TickCount
        End With

    End Sub

    Private Sub tmr_AutoRun_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_AutoRun.Tick

        Static wt As Integer

        With Me
            Dim CycTm As Integer = CType((My.Computer.Clock.TickCount - .ProcTime), Integer) / 1000

            .lbl_Elapsed.Text = String.Format("{0:D2}sec Elapsed.", CycTm)
            '.lbl_ProcNo.Text = "Process No. : " & .ProcNo.ToString
            .tmr_AutoRun.Enabled = False
        End With

        With ActiveProc
            Select Case ProcNo
                Case Is = 0
                    Me.lbl_ProcNo.Text = "Trigger the laser..."

                    If frm_Main.Set_ML_Mode() >= 0 Then
                        Dim ML_Cmd As String = "MSW1"
                        Dim RetCmd As Integer = frm_Main.SendMLCmd(ML_Cmd)
                    End If

                    wt = My.Computer.Clock.TickCount
                    Me.ProcNo += 1
                Case Is = 1
                    If .IO.i10_in_Marking.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        If My.Computer.Clock.TickCount > wt + 1000 Then
                            .ErrorMsg = "Laser not start..."
                            frm_Alarm.ShowDialog(Me)
                            Me.Close()
                            Exit Sub
                        End If
                    Else
                        Me.ProcNo += 1
                    End If
                Case Is = 2
                    Me.lbl_ProcNo.Text = "Waiting marking complete..."

                    If .IO.i10_in_Marking.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        Me.ProcNo += 1
                    End If

                    If .IO.i2_sw_Cover.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        .ErrorMsg = "Safety Cover not closed..."
                        frm_Alarm.ShowDialog(Me)
                        Me.ProcNo = 13
                    End If

                    If .IO.i16_in_LaserErr.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                        .ErrorMsg = "Laser Error..."
                        frm_Alarm.ShowDialog(Me)
                        Me.Close()
                        Exit Sub
                    End If

                    If .IO.i5_sw_Power.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        Me.Close()
                        Exit Sub
                    End If
                Case Is = 3
                    Me.lbl_ProcNo.Text = "Prepare for 2nd marking partition..."

                    .IO.o4_Tray_Sol.Trigger_ON()
                    wt = My.Computer.Clock.TickCount
                    Me.ProcNo += 1
                Case Is = 4
                    If .IO.i7_sw_slsRight.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                        If My.Computer.Clock.TickCount > wt + 3000 Then
                            .ErrorMsg = "Tray not move..."
                            frm_Alarm.ShowDialog(Me)
                            Me.Close()
                            Exit Sub
                        End If
                    Else
                        wt = My.Computer.Clock.TickCount
                        Me.ProcNo += 1
                    End If
                Case Is = 5
                    If .IO.i6_sw_slsLeft.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        If My.Computer.Clock.TickCount > wt + 3000 Then
                            .ErrorMsg = "Tray not move..."
                            frm_Alarm.ShowDialog(Me)
                            Me.Close()
                            Exit Sub
                        End If
                    Else
                        wt = My.Computer.Clock.TickCount
                        Me.ProcNo += 1
                    End If
                Case Is = 6
                    Me.lbl_ProcNo.Text = "Trigger the laser..."

                    If frm_Main.Set_ML_Mode() >= 0 Then
                        Dim ML_Cmd As String = "MSW1"
                        Dim RetCmd As Integer = frm_Main.SendMLCmd(ML_Cmd)
                    End If

                    wt = My.Computer.Clock.TickCount
                    Me.ProcNo += 1
                Case Is = 7
                    If .IO.i10_in_Marking.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        If My.Computer.Clock.TickCount > wt + 3000 Then
                            .ErrorMsg = "Laser not start..."
                            frm_Alarm.ShowDialog(Me)
                            Me.Close()
                            Exit Sub
                        End If
                    Else
                        Me.ProcNo += 1
                    End If
                Case Is = 8
                    Me.lbl_ProcNo.Text = "Waiting marking complete..."

                    If .IO.i10_in_Marking.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        Me.ProcNo += 1
                    End If

                    If .IO.i2_sw_Cover.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        .ErrorMsg = "Safety Cover not closed..."
                        frm_Alarm.ShowDialog(Me)
                        Me.ProcNo = 13
                    End If

                    If .IO.i16_in_LaserErr.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                        .ErrorMsg = "Laser Error..."
                        frm_Alarm.ShowDialog(Me)
                        Me.Close()
                        Exit Sub
                    End If

                    If .IO.i5_sw_Power.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        Me.Close()
                        Exit Sub
                    End If
                Case Is = 9
                    Me.lbl_ProcNo.Text = "Return Tray to origin..."

                    .IO.o4_Tray_Sol.Trigger_OFF()
                    wt = My.Computer.Clock.TickCount
                    Me.ProcNo += 1
                Case Is = 10
                    If .IO.i6_sw_slsLeft.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                        If My.Computer.Clock.TickCount > wt + 3000 Then
                            .ErrorMsg = "Tray not move..."
                            frm_Alarm.ShowDialog(Me)
                            Me.Close()
                            Exit Sub
                        End If
                    Else
                        wt = My.Computer.Clock.TickCount
                        Me.ProcNo += 1
                    End If
                Case Is = 11
                    If .IO.i7_sw_slsRight.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        If My.Computer.Clock.TickCount > wt + 3000 Then
                            .ErrorMsg = "Tray not move..."
                            frm_Alarm.ShowDialog(Me)
                            Me.Close()
                            Exit Sub
                        End If
                    Else
                        wt = My.Computer.Clock.TickCount
                        Me.ProcNo += 1
                    End If
                Case Is = 12
                    Me.lbl_Progress.Text = "Marking process stopped..."

                    Me.lbl_ProcNo.Text = "Release safety cover..."
                    .IO.o5_Cover_Sol.Trigger_OFF()

                    Me.lbl_ProcNo.Text = "Marking complete..."

                    .ErrorReset = 1
                    .ErrorMsg = "Change Tray..."
                    frm_Alarm.ShowDialog(Me)

                    tmr_AutoRun.Enabled = True
                    Me.ProcNo += 1
                Case Is = 13
                    Me.lbl_Progress.Text = "Marking process stopped..."
                    Me.lbl_ProcNo.Text = "Waiting Tray to be removed..."

                    If .IO.i11_sw_TraySensor.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        With Me
                            .tmr_AutoRun.Enabled = False
                            .ProcNo = 0
                            .Close()
                            Exit Sub
                        End With
                    End If
            End Select
        End With

        Me.tmr_AutoRun.Enabled = True

    End Sub

End Class