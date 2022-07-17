Public Class frm_SystemBusy

    Dim fg_Load As Integer = 1

    Private Sub frm_SystemBusy_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        With Me
            .tmr_Blink.Enabled = False
        End With

    End Sub

    Private Sub frm_SystemBusy_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        With Me
            fg_Load = 1

            With .tmr_Blink
                .Interval = 250
                .Enabled = True
            End With
        End With

    End Sub

    Private Sub frm_SystemBusy_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        If fg_Load = 0 Then Exit Sub
        fg_Load = 0

        With frm_Main.Progress1
            .Value = 0
            .Visible = True
        End With


        With ActiveProc
            Dim _profile() As String = .Lotdata(1).Profile.Split(","c)
            .SelectedBlock = 0

            If _profile(0).ToUpper.Contains("TCI_F") Then
                .UntilBlock = 4
            Else
                .UntilBlock = 1 + Val(Profiles(.SelectedProfile).UseDot) + Val(Profiles(.SelectedProfile).UseBlock)
            End If

            Select Case .SysBsyCode
                Case Is = 1     'Set Marking Data
                    If frm_Main.Set_ML_Mode() >= 0 Then
                        Dim RetCmd As Integer = 0
                        frm_Main.Progress1.Value = 10
                        Me.lbl_Msg.Text = "Setting Marking Data...."


                        For iLp As Integer = .SelectedBlock To .UntilBlock
                            Application.DoEvents()

                            Dim ML_Cmd As String = "MRW" & .MarkingCondSetting.A_Layout & "," & FormPostStream(.MarkingSetting(iLp))
                            RetCmd = frm_Main.SendMLCmd(ML_Cmd)

                            frm_Main.Progress1.Value = 10 + (10 * iLp)
                            RetCmd = RetCmd Or frm_Main.Set_ML_Mode(1)

                            If RetCmd < 0 Then
                                Exit For
                            End If
                        Next

                        If Not RetCmd < 0 Then
                            frm_Main.Progress1.Value = 100
                            .SysBsyCode = 0
                        End If
                    Else
                        MessageBox.Show("Unabled to set System Mode...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                    frm_Main.Progress1.Visible = False
                    Me.Close()
                Case Is = 2     'Set Condition Setting 
                    If frm_Main.Set_ML_Mode() >= 0 Then
                        frm_Main.Progress1.Value = 10

                        'Select Layout No.
                        Me.lbl_Msg.Text = "Select Layout No. " & .MarkingCondSetting.A_Layout
                        Dim ML_Cmd As String = "LNW" & .MarkingCondSetting.A_Layout
                        Dim RetCmd As Integer = frm_Main.SendMLCmd(ML_Cmd)
                        frm_Main.Progress1.Value = 15

                        If RetCmd < 0 Then
                            frm_Main.Progress1.Visible = False
                            MessageBox.Show("Unabled to select appropriate Marking Layout..." & vbCrLf & "Please refer to your system engineer.", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)

                            Me.Close()
                            Exit Sub
                        End If

                        'Set Laser Current
                        Me.lbl_Msg.Text = "Setting Laser Current...."
                        ML_Cmd = "CUW" & .MarkingCondSetting.E_Current
                        RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                        frm_Main.Progress1.Value = 20

                        If RetCmd < 0 Then
                            MessageBox.Show("Unabled to set Laser Current...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Else
                            'set Laser QSW
                            Me.lbl_Msg.Text = "Setting Laser QSW...."
                            ML_Cmd = "PUW" & .MarkingCondSetting.F_QSW
                            RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                            frm_Main.Progress1.Value = 30

                            If RetCmd < 0 Then
                                MessageBox.Show("Unabled to set QSW Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Else
                                'Set Laser Speed
                                Me.lbl_Msg.Text = "Setting Laser Speed...."
                                ML_Cmd = "SPW" & .MarkingCondSetting.G_Speed
                                RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                frm_Main.Progress1.Value = 40

                                If RetCmd < 0 Then
                                    MessageBox.Show("Unabled to set Laser Speed...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Else
                                    'Set Rotation Angle
                                    Me.lbl_Msg.Text = "Setting Rotation Angle...."
                                    ML_Cmd = "RTW" & IIf(.MarkingCondSetting.D_Rotation = "", "0", .MarkingCondSetting.D_Rotation)
                                    RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                    frm_Main.Progress1.Value = 50

                                    If RetCmd < 0 Then
                                        MessageBox.Show("Unabled to set Laser Rotation Angle...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Else
                                        'Set X-Axis Offset
                                        Me.lbl_Msg.Text = "Setting X-Axis Offset...."
                                        ML_Cmd = "XOW" & .MarkingCondSetting.B_Xoffset
                                        RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                        frm_Main.Progress1.Value = 60

                                        If RetCmd < 0 Then
                                            MessageBox.Show("Unabled to set X-Axis Offset...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        Else
                                            'Set Y-Axis Offset
                                            Me.lbl_Msg.Text = "Setting Y-Axis Offset...."
                                            ML_Cmd = "YOW" & .MarkingCondSetting.C_Yoffset
                                            RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                            frm_Main.Progress1.Value = 70

                                            If RetCmd < 0 Then
                                                MessageBox.Show("Unabled to set Y-Axis Offset...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            Else
                                                Dim MarkingLine As Integer = 0

                                                If ActiveProc._LaserIUnit = LaserUnit.ML7110B Then
                                                    frm_Main.SendMLCmd("CMW" & .MarkingCondSetting.A_Layout)
                                                End If


                                                For iLp As Integer = .SelectedBlock To .UntilBlock
                                                    Application.DoEvents()

                                                    Me.lbl_Msg.Text = "Setting Block...(" & iLp.ToString & ")"
                                                    ML_Cmd = "MRW" & .MarkingCondSetting.A_Layout & "," & FormPostStream(.MarkingSetting(iLp))

                                                    If _profile(0).ToUpper.Contains("TCI_F") Then
                                                    Else
                                                        If iLp >= (0 + Val(Profiles(.SelectedProfile).UseDot)) And iLp < (2 + Val(Profiles(.SelectedProfile).UseDot)) Then
                                                            Try
                                                                If Not .MarkingChar(MarkingLine) Is Nothing Then
                                                                    Dim MarkingBlockData As String = .MarkingChar(MarkingLine)

                                                                    If Not Val(Profiles(.SelectedProfile).UseDot) = 0 And MarkingBlockData.StartsWith("o") Then
                                                                        If .Lotdata(1).IMI_No.Length = 12 Then
                                                                            MarkingBlockData = MarkingBlockData.Substring(1)
                                                                        End If
                                                                    End If

                                                                    If MarkingBlockData = "" And .Lotdata(1).IMI_No.Length = 12 Then
                                                                        MarkingBlockData = "."
                                                                    End If

                                                                    ML_Cmd = ML_Cmd.Substring(0, ML_Cmd.LastIndexOf(",") + 1)
                                                                    ML_Cmd &= IIf(.MarkingChar(MarkingLine) = "!", ".", MarkingBlockData)
                                                                End If
                                                            Catch ex As Exception

                                                            End Try

                                                            MarkingLine += 1
                                                        End If
                                                    End If

                                                    RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                                    frm_Main.Progress1.Value = 80

                                                    RetCmd = RetCmd Or frm_Main.Set_ML_Mode(1)

                                                    If RetCmd < 0 Then
                                                        MessageBox.Show("Unabled to set Marking Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                        Exit For
                                                    End If
                                                Next

                                                If Not RetCmd < 0 Then
                                                    frm_Main.Progress1.Value = 100
                                                    .SysBsyCode = 0
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Else
                        MessageBox.Show("Unabled to set System Mode...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                    frm_Main.Progress1.Visible = False
                    Me.Close()
                Case Is = 3     'Get Condition Setting 
                    If frm_Main.Set_ML_Mode() >= 0 Then
                        Dim Data As String = String.Empty
                        frm_Main.Progress1.Value = 10

                        'Get Laser Current
                        Dim ML_Cmd As String = "CUR"
                        Dim RetCmd As Integer = frm_Main.SendMLCmd(ML_Cmd, Data)
                        frm_Main.Progress1.Value = 20

                        If RetCmd < 0 Then
                            MessageBox.Show("Unabled to read Laser Current Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Else
                            Dim STXpos As Integer = Data.IndexOf(Chr(ch_STX))
                            Dim ETXpos As Integer = Data.IndexOf(Chr(ch_ETX))

                            If Not STXpos < 0 And Not ETXpos < 0 Then
                                Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                .MarkingCondSetting.E_Current = Data
                            End If

                            'Get Laser QSW
                            ML_Cmd = "PUR"
                            RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                            frm_Main.Progress1.Value = 30

                            If RetCmd < 0 Then
                                MessageBox.Show("Unabled to read Laser QSW Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Else
                                STXpos = Data.IndexOf(Chr(ch_STX))
                                ETXpos = Data.IndexOf(Chr(ch_ETX))

                                If Not STXpos < 0 And Not ETXpos < 0 Then
                                    Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                    .MarkingCondSetting.F_QSW = Data
                                End If

                                'Get Laser Speed
                                ML_Cmd = "SPR"
                                RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                frm_Main.Progress1.Value = 40

                                If RetCmd < 0 Then
                                    MessageBox.Show("Unabled to read Laser Speed Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Else
                                    STXpos = Data.IndexOf(Chr(ch_STX))
                                    ETXpos = Data.IndexOf(Chr(ch_ETX))

                                    If Not STXpos < 0 And Not ETXpos < 0 Then
                                        Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                        .MarkingCondSetting.G_Speed = Data
                                    End If

                                    'Get Laser Rotation
                                    ML_Cmd = "RTR"
                                    RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                    frm_Main.Progress1.Value = 50

                                    If RetCmd < 0 Then
                                        MessageBox.Show("Unabled to read Laser Rotation Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Else
                                        STXpos = Data.IndexOf(Chr(ch_STX))
                                        ETXpos = Data.IndexOf(Chr(ch_ETX))

                                        If Not STXpos < 0 And Not ETXpos < 0 Then
                                            Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                            .MarkingCondSetting.D_Rotation = Data
                                        End If

                                        'Get X-Axis Offset
                                        ML_Cmd = "XOR"
                                        RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                        frm_Main.Progress1.Value = 60

                                        If RetCmd < 0 Then
                                            MessageBox.Show("Unabled to read X-Axis Offset Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        Else
                                            STXpos = Data.IndexOf(Chr(ch_STX))
                                            ETXpos = Data.IndexOf(Chr(ch_ETX))

                                            If Not STXpos < 0 And Not ETXpos < 0 Then
                                                Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                                .MarkingCondSetting.B_Xoffset = Data
                                            End If

                                            'Get Y-Axis Offset
                                            ML_Cmd = "YOR"
                                            RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                            frm_Main.Progress1.Value = 70

                                            If RetCmd < 0 Then
                                                MessageBox.Show("Unabled to read X-Axis Offset Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            Else

                                                If ActiveProc._LaserIUnit = LaserUnit.ML7110B Then
                                                    frm_Main.SendMLCmd("CMW" & .MarkingCondSetting.A_Layout)
                                                End If

                                                STXpos = Data.IndexOf(Chr(ch_STX))
                                                ETXpos = Data.IndexOf(Chr(ch_ETX))

                                                If Not STXpos < 0 And Not ETXpos < 0 Then
                                                    Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                                    .MarkingCondSetting.C_Yoffset = Data
                                                End If

                                                'Get Marking Setting
                                                For iLp As Integer = 0 To .MarkingSetting.GetUpperBound(0) - 2
                                                    Application.DoEvents()

                                                    ML_Cmd = "MRR" & .MarkingCondSetting.A_Layout & "," & .MarkingSetting(iLp).LineNo
                                                    RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                                    frm_Main.Progress1.Value = 80

                                                    If RetCmd < 0 Then
                                                        MessageBox.Show("Unabled to read Marking Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                        Exit For
                                                    Else
                                                        STXpos = Data.IndexOf(Chr(ch_STX))
                                                        ETXpos = Data.IndexOf(Chr(ch_ETX))

                                                        If Not STXpos < 0 And Not ETXpos < 0 Then
                                                            Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                                            ReadParameter(.MarkingSetting(iLp), Data)
                                                        End If
                                                    End If
                                                Next

                                                frm_Main.Progress1.Value = 100
                                                .SysBsyCode = 0
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Else
                        MessageBox.Show("Unabled to set System Mode...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                    frm_Main.Progress1.Visible = False
                    Me.Close()
            End Select
        End With

    End Sub

End Class