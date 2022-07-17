Public Class frm_Alarm

    Private Sub frm_Alarm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        With Me
            .tmr_IOScan.Enabled = False
            .tmr_Blink.Enabled = False
        End With

        With ActiveProc
            .ErrorReset = 0
            .IO.o9_Buzzer.Trigger_OFF()
        End With

    End Sub

    Private Sub frm_Alarm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        With Me
            .lbl_Msg.Text = ActiveProc.ErrorMsg

            With .tmr_Blink
                .Interval = 200
                .Enabled = True
            End With

            With .tmr_IOScan
                .Interval = 70
                .Enabled = True
            End With
        End With

        With ActiveProc.IO
            .o3_Start_LED.Trigger_OFF()
            .o9_Buzzer.Trigger_ON()
        End With

    End Sub

    Private Sub tmr_Blink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_Blink.Tick

        With Me
            .lbl_Msg.Visible = Not .lbl_Msg.Visible
        End With

        With ActiveProc
            Select Case .Init_HDW
                Case Is = Func_Ret_Success
                    If Me.lbl_Msg.Visible = True Then
                        .IO.o1_Stop_LED.Trigger_ON()
                    Else
                        .IO.o1_Stop_LED.Trigger_OFF()
                    End If
            End Select
        End With

    End Sub

    Private Sub tmr_IOScan_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_IOScan.Tick

        With ActiveProc
            If .IO.i4_sw_Stop.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                With Me
                    .tmr_IOScan.Enabled = False
                    .Close()
                End With
            End If

            If Not .ErrorReset = 0 Then
                If .IO.i2_sw_Cover.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                    With Me
                        .tmr_IOScan.Enabled = False
                        .Close()
                    End With
                End If
            End If
        End With

    End Sub

End Class