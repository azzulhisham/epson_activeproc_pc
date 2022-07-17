Imports System.Threading


Public Class frm_HardwareInit

    Private Sub frm_HardwareInit_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        With Me
            .tmr_LblBlink.Enabled = False
            .tmr_BlinkLED.Enabled = False
            .tmr_IOScan.Enabled = False
        End With

    End Sub

    Private Sub frm_HardwareInit_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Initializing LED color
        GrnLED_OnOff(0) = Color.Green
        GrnLED_OnOff(1) = Color.Lime
        PeruLED_OnOff(0) = Color.Peru
        PeruLED_OnOff(1) = Color.Bisque
        RedLED_OnOff(0) = Color.Maroon
        RedLED_OnOff(1) = Color.Red
        OrgLED_OnOff(0) = Color.DarkOrange
        OrgLED_OnOff(1) = Color.Orange
        BlueLED_OnOff(0) = Color.DarkBlue
        BlueLED_OnOff(1) = Color.Blue
        YellowLED_OnOff(0) = Color.Goldenrod
        YellowLED_OnOff(1) = Color.Gold


        With Me
            .btn_Cmd.Visible = False

            With .tmr_LblBlink
                .Interval = 250
                .Enabled = True
            End With
        End With

    End Sub

    Private Sub frm_HardwareInit_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        With ActiveProc
            .Init_HDW = Init_Hardware()

            If .Init_HDW = Func_Ret_Success Then
                .IO.o4_Tray_Sol.Trigger_OFF()
                .IO.o5_Cover_Sol.Trigger_OFF()

                With Me
                    .btn_Cmd.BackColor = GrnLED_OnOff(fg_OFF)
                    .btn_Cmd.Image = Me.pic_Start.BackgroundImage
                    .lbl_Status.Text = "The system is ready..."
                End With
            Else
                With Me
                    .btn_Cmd.BackColor = RedLED_OnOff(fg_OFF)
                    .btn_Cmd.Image = Me.pic_Stop.BackgroundImage
                    .lbl_Status.Text = "The hardware is fail to initialize..."
                End With
            End If
        End With

        With Me
            .btn_Cmd.Visible = True

            With .tmr_BlinkLED
                .Interval = 200
                .Enabled = True
            End With

            With .tmr_IOScan
                .Interval = 70
                .Enabled = True
            End With
        End With

    End Sub

    Private Sub tmr_BlinkLED_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_BlinkLED.Tick

        Static MachineState As Integer
        MachineState = MachineState Xor 1


        With ActiveProc
            Select Case .Init_HDW
                Case Is = Func_Ret_Success
                    Me.btn_Cmd.BackColor = GrnLED_OnOff(MachineState)
                    .IO.o1_Stop_LED.Trigger_OFF()

                    If MachineState = fg_ON Then
                        .IO.o3_Start_LED.Trigger_ON()
                    Else
                        .IO.o3_Start_LED.Trigger_OFF()
                    End If
                Case Is = Func_Ret_Fail
                    Me.btn_Cmd.BackColor = RedLED_OnOff(MachineState)
                    .IO.o3_Start_LED.Trigger_OFF()

                    If MachineState = fg_ON Then
                        .IO.o1_Stop_LED.Trigger_ON()
                    Else
                        .IO.o3_Start_LED.Trigger_OFF()
                    End If
            End Select
        End With

    End Sub

    Private Sub tmr_LblBlink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_LblBlink.Tick

        With Me
            .lbl_Status.Visible = Not .lbl_Status.Visible
        End With

    End Sub

    Private Sub btn_Cmd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_Cmd.Click

        With ActiveProc
            If .Init_HDW = Func_Ret_Success Then
                .IO.o9_Buzzer.Trigger_ON()
                Thread.Sleep(800)
                .IO.o9_Buzzer.Trigger_OFF()
            End If
        End With

        With Me
            .Close()
        End With

    End Sub

    Private Sub tmr_IOScan_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_IOScan.Tick

        With ActiveProc
            Select Case .Init_HDW
                Case Is = Func_Ret_Success
                    If .IO.i3_sw_Start.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                        Me.tmr_IOScan.Enabled = False

                        Do Until .IO.i3_sw_Start.BitState = cls_PCIBoard.BitsState.eBit_OFF
                            Application.DoEvents()
                        Loop

                        Me.btn_Cmd.PerformClick()
                        Exit Sub
                    End If
                Case Is = Func_Ret_Fail
                    If .IO.i4_sw_Stop.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                        With Me
                            .tmr_IOScan.Enabled = False
                            .btn_Cmd.PerformClick()
                        End With

                        Exit Sub
                    End If
            End Select
        End With

    End Sub

End Class