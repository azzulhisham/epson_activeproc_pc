Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Web.Services.Protocols


Public Class frm_Main

#If UseWebServices = 1 Then
    Dim azWebService As New zulhisham_pc.az_Services
#Else
    Dim azLMServices As New cls_LMservices 
#End If


    Dim MyWeekDay() As String = {"", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"}
    Dim MyMonth() As String = {"", "Jan", "Feb", "Mac", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"}

    Dim MyAnimation(4) As PictureBox
    Dim BlockMarker(5) As Label
    Dim TSX_Marker(5) As Label

    Dim fg_Load As Integer = 1
    Dim fg_Ignore As Integer = 0
    Dim fg_IgnoreSelect As Integer = 0


    Private Sub frm_Main_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If Not ActiveProc._Mode = optMode.Update Then
            If MessageBox.Show("Are you very sure you want close the system application ?", "Laser Marking...", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then
                e.Cancel = 1
                Exit Sub
            End If
        End If

        With Me
            .tmr_Blink.Enabled = False
            .tmr_WS.Enabled = False
        End With

        With ActiveProc
            Miyachi.Close()

            If ._MachineType = MachineType.PC Then
                If .Init_HDW = Func_Ret_Success Then
                    Me.tmr_IOScan.Enabled = False
                    Me.tmr_DispLED.Enabled = False
                End If
            End If
        End With

    End Sub

    Private Sub Set_TCI_Marking(ByVal e As Boolean)

        With Me
            .pic_TCIformat.Visible = e
            .lbl_Dot_.Visible = e
            .lbl_Plant_.Visible = e
            .lbl_WeekCode_.Visible = e
            .lbl_Rank_.Visible = e

            .pic_TCIformat_.Visible = e
            .lbl_Dot__.Visible = e
            .lbl_Plant__.Visible = e
            .lbl_WeekCode__.Visible = e
            .lbl_Rank__.Visible = e

            .PictureBox4.Visible = Not e
            .lbl_MarkChar1.Visible = Not e
            .lbl_MarkChar2.Visible = Not e
            .lbl_MarkChar3.Visible = Not e
            .lbl_MarkChar4.Visible = Not e

            .lbl_WeekCode.Visible = Not e
            .lbl_Freq.Visible = Not e
        End With

    End Sub

    Private Sub frm_Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        With ActiveProc
            .GetAuthentication = 0
            .SysBsyCode = 0
            .DataRecorded = 0
            .ErrorReset = 0

            ReDim .Lotdata(1)
            ReDim .MarkingSetting(5)
        End With

        With Me
            Dim AppMode As Integer = CType(regSubKey.GetValue("SimulationMode", "0"), Integer)

            If AppMode = 0 Then
                .btn_CheckWC.Visible = False
            Else
                .btn_CheckWC.Visible = True
            End If

            Set_TCI_Marking(False)

            With .tmr_PostRdy
                .Enabled = False
                .Interval = 2.5 * 1000
            End With

            .WindowState = FormWindowState.Maximized
            .fg_Load = 1
            .fg_Ignore = 0

            If My.Computer.Screen.WorkingArea.Width < 1280 Then
                .Label1.Location = New Point(300, .Label1.Location.Y)
                .lbl_WeekDayVal.Location = New Point(660, .lbl_WeekDayVal.Location.Y)
                .lbl_TimeVal.Location = New Point(888, .lbl_TimeVal.Location.Y)
                .lbl_YearVal.Location = New Point(660, lbl_YearVal.Location.Y)
                .lbl_MonthVal.Location = New Point(670, .lbl_MonthVal.Location.Y)
                .lbl_DayVal.Location = New Point(792, .lbl_DayVal.Location.Y)
                .lbl_thLabel.Location = New Point(916, .lbl_thLabel.Location.Y)
            End If

            .lbl_MarkChar1.Text = Chr(7)
            .lbl_WarningMsg.Text = ""
            .Text = "Laser Marking - Active Procedure " & String.Format("<Ver. : {0:D4}.{1:D2}.{2:D2}.{3:D3}>", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.MinorRevision)
            .pic_Animation.Tag = "0"

            With .cbo_BlockNo
                .Items.Clear()

                For iLp As Integer = 0 To ActiveProc.MarkingSetting.GetUpperBound(0) - 1
                    Application.DoEvents()
                    .Items.Add((iLp + 1).ToString)
                Next
            End With

            BlockMarker(0) = New Label
            BlockMarker(1) = New Label
            BlockMarker(2) = New Label
            BlockMarker(3) = New Label

            TSX_Marker(0) = New Label
            TSX_Marker(1) = New Label
            TSX_Marker(2) = New Label
            TSX_Marker(3) = New Label
            TSX_Marker(4) = New Label

            BlockMarker(0) = .lbl_MarkChar1
            BlockMarker(1) = .lbl_MarkChar2
            BlockMarker(2) = .lbl_MarkChar3
            BlockMarker(3) = .lbl_MarkChar4
            'BlockMarker(4) = Me.lbl_MarkChar5

            TSX_Marker(0) = .lbl_Dot__
            TSX_Marker(1) = .lbl_Plant__
            TSX_Marker(2) = .lbl_WeekCode__
            TSX_Marker(3) = .lbl_Rank__
            TSX_Marker(4) = .lbl_TSXmark__


            MyAnimation(0) = New PictureBox
            MyAnimation(1) = New PictureBox
            MyAnimation(2) = New PictureBox
            MyAnimation(3) = New PictureBox
            MyAnimation(4) = New PictureBox

            MyAnimation(0) = .pic_Marking1
            MyAnimation(1) = .pic_Marking2
            MyAnimation(2) = .pic_Marking3
            MyAnimation(3) = .pic_Marking4
            MyAnimation(4) = .pic_Marking5

            DispCalender()
            SetMode()

            If ReadRegData() = Func_Ret_Success Then
                .Text = "Laser Marking - Active Procedure " & String.Format("<Ver. : {0:D4}.{1:D2}.{2:D2}.{3:D3}>", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.MinorRevision) & " --> #" & ActiveProc.CtrlNo

                If InitSerialPort() = Func_Ret_Success Then
                    With .tmr_Blink
                        .Interval = 250
                        .Enabled = True
                    End With
                End If
            End If

            With .tmr_WS
                .Interval = 30 * 1000
                .Enabled = True
            End With

            With .tmr_Marking
                .Interval = 30
                .Enabled = False
            End With
        End With

    End Sub

    Private Sub DispMarkingSetting()

        With ActiveProc
            If Me.cbo_BlockNo.SelectedIndex < 0 Then Exit Sub

            Me.fg_Ignore = 1
            Me.cbo_DrawType.SelectedIndex = Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).A_DrawType)
            Me.cbo_TextAlign.SelectedIndex = Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).E_TextAlign)
            Me.cbo_SpaceAlign.SelectedIndex = Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).G_SpaceAlign)
            Me.fg_Ignore = 0

            Me.lbl_Xaxis.Text = String.Format("{0:F3}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).B_X_Axis) / 1000)
            Me.lbl_Yaxis.Text = String.Format("{0:F3}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).C_Y_Axis) / 1000)
            Me.lbl_TextAngle.Text = String.Format("{0:F1}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).D_TextAngle) / 10)
            Me.lbl_WidthAlign.Text = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).F_WidthAlign
            Me.lbl_SpaceWidth.Text = String.Format("{0:F3}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).H_SpaceWidth) / 1000)
            Me.lbl_XaxisOrg.Text = String.Format("{0:F3}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).I_X_AxisOrg) / 1000)
            Me.lbl_YaxisOrg.Text = String.Format("{0:F3}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).J_Y_AxisOrg) / 1000)
            Me.lbl_CharHeight.Text = String.Format("{0:F3}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).K_CharHeight) / 1000)
            Me.lbl_Compress.Text = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).L_Compress
            Me.lbl_OppDir.Text = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).M_OppDir
            Me.lbl_CharAngle.Text = String.Format("{0:F1}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).N_CharAngle) / 10)
            Me.lbl_Current.Text = String.Format("{0:F1}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).O_Current) / 10)
            Me.lbl_QSW.Text = String.Format("{0:F1}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).P_QSW) / 10)
            Me.lbl_Speed.Text = String.Format("{0:F2}", Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).Q_Speed) / 100)
            Me.lbl_Repeat.Text = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).R_Repeat
            Me.lbl_VarType.Text = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).T_VarType
            Me.lbl_VarNo.Text = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).U_VarNo

            .TempSetting = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex)
        End With

        Me.SetMarker()

    End Sub

    Private Sub SetMarker()

        With Me
            If .cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
                'Set_TCI_Marking(True)

                For iLp As Integer = 0 To TSX_Marker.GetUpperBound(0) - 2
                    Application.DoEvents()
                    TSX_Marker(iLp).ForeColor = Color.Black
                Next
            Else
                'Set_TCI_Marking(False)

                For iLp As Integer = 0 To BlockMarker.GetUpperBound(0) - 2
                    Application.DoEvents()
                    BlockMarker(iLp).ForeColor = Color.Black
                Next
            End If


            Try
                If .cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
                    If .cbo_BlockNo.SelectedIndex = 4 Then
                        TSX_Marker(.cbo_BlockNo.SelectedIndex).Visible = True
                    Else
                        TSX_Marker(4).Visible = False
                        TSX_Marker(.cbo_BlockNo.SelectedIndex).ForeColor = Color.Red
                    End If
                Else
                    If Val(Profiles(Me.cbo_Profiles.SelectedIndex).UseDot) = 1 Then
                        BlockMarker(.cbo_BlockNo.SelectedIndex).ForeColor = Color.Red
                    Else
                        BlockMarker(.cbo_BlockNo.SelectedIndex + 1).ForeColor = Color.Red
                    End If
                End If
            Catch ex As Exception
                If .cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
                    .cbo_BlockNo.SelectedIndex = .cbo_BlockNo.Items.Count - 1
                Else
                    .cbo_BlockNo.SelectedIndex = .cbo_BlockNo.Items.Count - 2
                End If
            End Try
        End With

    End Sub

    Private Sub DispMarkingConditionSetting()

        With ActiveProc
            Me.lbl_LayoutNo.Text = .MarkingCondSetting.A_Layout
            Me.lbl_CurSetting.Text = String.Format("{0:F1}", Val(.MarkingCondSetting.E_Current) / 10)
            Me.lbl_QSWSetting.Text = String.Format("{0:F1}", Val(.MarkingCondSetting.F_QSW) / 10)
            Me.lbl_SpeedSetting.Text = String.Format("{0:F2}", Val(.MarkingCondSetting.G_Speed) / 100)

            Me.lbl_XoffsetSetting.Text = String.Format("{0:F3}", Val(.MarkingCondSetting.B_Xoffset) / 1000)
            Me.lbl_YoffsetSetting.Text = String.Format("{0:F3}", Val(.MarkingCondSetting.C_Yoffset) / 1000)
            Me.lbl_RotateSetting.Text = String.Format("{0:F6}", Val(.MarkingCondSetting.D_Rotation) / 1000000)
            Me.lbl_LayoutSetting.Text = .MarkingCondSetting.A_Layout

            Me.lbl_SetCurrent_.Text = String.Format("{0:F1}", Val(.MarkingCondSetting.E_Current) / 10)
            Me.lbl_SetQSW_.Text = String.Format("{0:F1}", Val(.MarkingCondSetting.F_QSW) / 10)
            Me.lbl_SetSpeed_.Text = String.Format("{0:F2}", Val(.MarkingCondSetting.G_Speed) / 100)

            Me.lbl_SetXoffset_.Text = String.Format("{0:F3}", Val(.MarkingCondSetting.B_Xoffset) / 1000)
            Me.lbl_SetYoffset_.Text = String.Format("{0:F3}", Val(.MarkingCondSetting.C_Yoffset) / 1000)
            Me.lbl_SetRotation_.Text = String.Format("{0:F6}", Val(.MarkingCondSetting.D_Rotation) / 1000000)

            .TempCondSetting = .MarkingCondSetting
        End With

    End Sub

    Private Sub SetMode(Optional ByVal WriteMode As Boolean = True)

        Dim pt As Drawing.Point


        With Me
            .tmr_Marking.Enabled = Not WriteMode

            If WriteMode Then
                .tmr_PostRdy.Enabled = Not WriteMode
                .RecStatus.Text = "..."

                .lbl_LotNo.Text = ""
                .lbl_EmpNo.Text = ""
                .lbl_Freq.Text = "2000AP"

                With lbl_WeekCode
                    .Text = "EymdA"
                    pt.X = 50
                    pt.Y = 165
                    .Location = pt
                End With

                .lbl_Dot.Text = Chr(7)

                With .lbl_Dot
                    .Visible = True

                    pt.X = 50
                    pt.Y = 165
                    .Location = pt
                End With


                Try
                    If Profiles(Me.cbo_Profiles.SelectedIndex).Spec.Contains("TCI_F") Then
                        .lbl_Freq.Visible = False
                        .lbl_Mark1.Visible = False
                    Else
                        .lbl_Freq.Visible = True
                        .lbl_Mark1.Visible = True
                    End If
                Catch ex As Exception
                    .lbl_Freq.Visible = True
                    .lbl_Mark1.Visible = True
                End Try


                .lbl_WeekCode.TextAlign = ContentAlignment.MiddleRight
                .lbl_WarningMsg.Visible = True
                .lbl_WarningMsg.Text = "Click 'Data Entry' Button To Set Marking Data..."
            End If

            .pic_PostDone.Visible = False
            .pic_Post.Visible = Not WriteMode
            .pic_Write.Visible = WriteMode

            .btn_Post.Visible = Not WriteMode
            .btn_Post.Enabled = Not WriteMode
            .btn_DataEntry.Visible = WriteMode

            .btn_Cancel.Enabled = Not WriteMode
        End With

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_DataEntry.Click

        If Me.Opacity < 1 Then Exit Sub
        Dim pt As Drawing.Point


#If UseWebServices = 1 Then
        If My.Computer.Network.IsAvailable = False Then
            MessageBox.Show("No Network is available. Please consult with your system engineer.", "System Error...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
#End If

#If UseSQL_Server = 1 Then
        Try
            If My.Computer.Network.Ping(ActiveProc.DataBase_.Server.Substring(0, ActiveProc.DataBase_.Server.IndexOf("\")), 5000) = False Then
                MessageBox.Show("The Database server was not responded the 'Ping' command.", "System Error...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        Catch ex As Exception
            If My.Computer.Network.IsAvailable Then
                MessageBox.Show("The system could not reached the Database Server. Please check system setting!", "System Error...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            Else
                MessageBox.Show("The Database network is not available.", "System Error...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        End Try
#End If


        frm_DataEntry.ShowDialog(Me)

        With ActiveProc
            .DataRecorded = 0

            If Not .Lotdata(1).Lot_No = "" And Not .Lotdata(1).IMI_No = "" Then
                If .Lotdata(1).Profile.IndexOf(",") < 0 Then
                    .Lotdata(1).Profile = .Lotdata(1).Profile & "," & .Lotdata(1).Profile.Replace("-", "") & ".mrk"
                End If


                Dim _profile() As String = .Lotdata(1).Profile.Split(","c)


                If Me.cbo_Profiles.FindString(_profile(0)) < 0 Then
                    MessageBox.Show("No profiles was setup for this product code.", "Profiles Not Found...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Exit Sub
                End If


                Me.SetMode(False)
                Me.cbo_Profiles.SelectedIndex = Me.cbo_Profiles.FindString(_profile(0))
                .SelectedProfile = Me.cbo_Profiles.SelectedIndex

                With .Lotdata(1)
                    Dim Mdata() As String = {"", "", ""}
                    Mdata(0) = .MData1
                    Mdata(1) = .MData2
                    Mdata(2) = .MData3

                    Me.lbl_LotNo.Text = .Lot_No
                    Me.lbl_EmpNo.Text = .IMI_No


                    If Not Val(Profiles(Me.cbo_Profiles.SelectedIndex).UseDot) = 0 And Mdata(1).StartsWith("o") Then
                        If .IMI_No.Length = 12 Then
                            Mdata(1) = Mdata(1).Substring(1)
                        End If
                    End If

                    If _profile(0).ToUpper.Contains("TCI_F") Then
                        Set_TCI_Marking(True)

                        Me.lbl_Dot_.Text = Chr(7)
                        Me.lbl_Plant_.Text = Mdata(0)
                        Me.lbl_WeekCode_.Text = Mdata(1)
                        Me.lbl_Rank_.Text = Mdata(2)

                        Me.lbl_Dot__.Text = Chr(7)
                        Me.lbl_Plant__.Text = Mdata(0)
                        Me.lbl_WeekCode__.Text = Mdata(1)
                        Me.lbl_Rank__.Text = Mdata(2)
                    Else
                        Set_TCI_Marking(False)
                        Me.lbl_WeekCode.Font = New System.Drawing.Font("Tahoma", 33.0!)
                        Me.lbl_Dot.Font = New System.Drawing.Font("Tahoma", 33.0!)

                        Me.lbl_Freq.Text = IIf(Mdata(0) = "!", "", Mdata(0))
                        Me.lbl_WeekCode.Text = Mdata(1)

                        If Profiles(Me.cbo_Profiles.SelectedIndex).Spec.ToLower = "rakon2" Or Profiles(Me.cbo_Profiles.SelectedIndex).Spec.ToLower = "rakon" Then
                            Me.lbl_WeekCode.TextAlign = ContentAlignment.MiddleCenter
                            Me.lbl_Freq.Visible = False
                            Me.lbl_Mark1.Visible = True
                            Me.lbl_Mark1.Text = "_"

                            pt.X = 130
                            pt.Y = 170
                            Me.lbl_Mark1.Location = pt
                        Else
                            Me.lbl_WeekCode.TextAlign = ContentAlignment.MiddleRight
                            Me.lbl_Freq.Visible = True
                            Me.lbl_Mark1.Text = "_"

                            If Val(Profiles(Me.cbo_Profiles.SelectedIndex).UseBlock) = 0 Then
                                Me.lbl_Mark1.Visible = False
                                Me.lbl_Emark.Visible = False
                            Else
                                If _profile(0).ToUpper.StartsWith("XV") Then
                                    Me.lbl_Emark.Visible = True
                                    Me.lbl_Emark_.Visible = True
                                    Me.lbl_Mark1.Visible = False
                                    Me.lbl_MarkChar4.Visible = False

                                    Me.lbl_WeekCode.Font = New System.Drawing.Font("Tahoma", 32.0!)
                                    Me.lbl_Dot.Font = New System.Drawing.Font("Tahoma", 32.0!)
                                    BlockMarker(3) = Me.lbl_Emark_
                                Else
                                    Me.lbl_Mark1.Visible = True
                                    Me.lbl_MarkChar4.Visible = True
                                    Me.lbl_Emark.Visible = False
                                    Me.lbl_Emark_.Visible = False

                                    BlockMarker(3) = Me.lbl_MarkChar4
                                End If
                            End If
                        End If


                        If Profiles(Me.cbo_Profiles.SelectedIndex).Spec = "FA-12T" Then
                            pt.X = 66
                            pt.Y = 172
                        Else
                            If Mdata(1).Contains("."c) Then
                                'pt.X = 65
                                pt.X = 55
                                pt.Y = 165
                            Else
                                pt.X = 43
                                pt.Y = 165
                            End If
                        End If

                        If Profiles(Me.cbo_Profiles.SelectedIndex).Spec = "FA-20HDOT" Then
                            pt.X = 48
                            Me.lbl_WeekCode.Width = 220
                        Else
                            Me.lbl_WeekCode.Width = 216
                        End If

                        Me.lbl_WeekCode.Location = pt
                    End If
                End With

                With Me
                    'If Val(Profiles(Me.cbo_Profiles.SelectedIndex).UseBlock) = 0 Then
                    '    .lbl_Mark1.Visible = False
                    'Else
                    '    .lbl_Mark1.Visible = True
                    'End If

                    If Val(Profiles(Me.cbo_Profiles.SelectedIndex).UseDot) = 0 Then
                        .lbl_Dot.Visible = False
                        .lbl_Dot_.Visible = False
                        .lbl_Dot__.Visible = False
                    Else
                        If _profile(0).ToUpper.Contains("TCI_F") Then
                            .lbl_Dot_.Visible = True
                            .lbl_Dot__.Visible = True
                        Else
                            With .lbl_Dot
                                If Profiles(Me.cbo_Profiles.SelectedIndex).Spec = "TD3225" Then
                                    pt.X = 50
                                    pt.Y = 188
                                Else
                                    pt.X = 50
                                    pt.Y = 165
                                End If

                                If Profiles(Me.cbo_Profiles.SelectedIndex).Spec = "TSX-2016H" Or Profiles(Me.cbo_Profiles.SelectedIndex).Spec.StartsWith("XV") Then
                                    pt.X = 50
                                    pt.Y = 160
                                    .Text = "o"
                                Else
                                    .Text = Chr(7)
                                End If

                                .Location = pt
                                .Visible = True
                            End With
                        End If
                    End If

                    If _profile(0).ToUpper.Contains("TCI_F") Then

                    Else
                        .lbl_MarkChar2.Text = .lbl_Freq.Text
                        .lbl_MarkChar3.Text = .lbl_WeekCode.Text
                    End If
                End With


                If .Lotdata(1).IMI_No.Substring(4, 1) = "P" Then
                    If SaveRecord() = Func_Ret_Success Then
                        MessageBox.Show("This spec. (" & .Lotdata(1).IMI_No & ") not required to do Marking.", My.Application.Info.ProductName & "...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                        With Me
                            .pic_PostDone.Visible = True
                            .btn_Post.Enabled = False
                            .btn_Cancel.Text = "Reset"
                        End With
                    Else
                        MessageBox.Show("This spec.(" & .Lotdata(1).IMI_No & ") not required to do Marking. But data is not sucessfully recorded. Please try to scan again!", My.Application.Info.ProductName & "...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                        With Me
                            .pic_PostDone.Visible = True
                            .btn_Post.Enabled = False
                            .btn_Cancel.Text = "Reset"
                        End With
                    End If
                Else
                    With Me
                        With .tmr_PostRdy
                            .Interval = 2.5 * 1000      'Tick every 2.5 sec
                            .Enabled = True
                        End With
                    End With

                    frm_WarnMsg.Show(Me)
                    ._Mode = optMode.PostData
                End If

            End If
        End With

    End Sub

    Private Sub tmr_WS_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_WS.Tick

        With Me
            Try
                With .WS_Status
#If UseWebServices = 1 Then
                    If My.Computer.Network.IsAvailable Then
                        .ToolTipText = azWebService.AboutMe
                        .Text = "Connected..."
                    Else
                        .ToolTipText = ""
                        .Text = "Disconnected..."
                    End If
#Else
                    .Text = "Not Used..."
#End If
                End With

            Catch ex As Exception
                With .WS_Status
                    .ToolTipText = ""
                    .Text = "Disconnected..."
                End With
            End Try
        End With

    End Sub

    Private Sub tmr_Blink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_Blink.Tick

        Static PicNo As Integer


        With Me
            .pic_Animation.BackgroundImage = .MyAnimation(PicNo).BackgroundImage
            PicNo += 1 : If PicNo > .MyAnimation.GetUpperBound(0) Then PicNo = 0

            .DispCalender()

            If .btn_Post.Visible = True And .btn_Post.Enabled = True Then
                .pic_Post.Visible = Not .pic_Post.Visible
            Else
                If Not .btn_Post.Enabled = True Then
                    If .pic_Post.Visible = True Then .pic_Post.Visible = False
                End If
            End If

            If Not fg_IgnoreSelect = 0 Then Exit Sub
            'If .cbo_Profiles.SelectedItem.ToString.ToUpper = "TCI_FORMAT" Then Exit Sub

            Dim MarkUseBlock As Integer = 3


            Try
                If Not Me.cbo_Profiles.SelectedIndex < 0 Then
                    If Val(Profiles(Me.cbo_Profiles.SelectedIndex).UseDot) = 1 Then
                        MarkUseBlock = 3
                    Else
                        MarkUseBlock = 2
                    End If
                End If
            Catch ex As Exception
                Exit Sub
            End Try

            If ActiveProc.GetAuthentication = 1 And .cbo_BlockNo.SelectedIndex = MarkUseBlock AndAlso (.cbo_Profiles.Items.Count > 0 AndAlso Not .cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F")) Then
                With .lbl_MarkChar4
                    .Cursor = Cursors.Hand

                    If .Text = "" Then
                        If .BorderStyle = BorderStyle.None Then
                            .BorderStyle = BorderStyle.FixedSingle
                        Else
                            .BorderStyle = BorderStyle.None
                        End If
                    Else
                        .BorderStyle = BorderStyle.None
                    End If
                End With
            Else
                .lbl_MarkChar4.BorderStyle = BorderStyle.None
                .lbl_MarkChar4.Cursor = Cursors.Default
            End If
        End With

    End Sub

    Private Sub DispCalender()

        Dim MyDay As Date = Now


        With Me
            .lbl_YearVal.Text = String.Format("{0:D4}", MyDay.Year)
            .lbl_MonthVal.Text = MyMonth(MyDay.Month)
            .lbl_DayVal.Text = String.Format("{0:D2}", MyDay.Day)
            .lbl_WeekDayVal.Text = MyWeekDay(MyDay.DayOfWeek)
            .lbl_TimeVal.Text = String.Format("{0:D2}:{1:D2}:{2:D2}", MyDay.Hour, MyDay.Minute, MyDay.Second)

            If .lbl_DayVal.Text.EndsWith("1") Then
                If MyDay.Day = 11 Then
                    .lbl_thLabel.Text = "th"
                Else
                    .lbl_thLabel.Text = "st"
                End If
            ElseIf .lbl_DayVal.Text.EndsWith("2") Then
                If MyDay.Day = 12 Then
                    .lbl_thLabel.Text = "th"
                Else
                    .lbl_thLabel.Text = "nd"
                End If
            ElseIf .lbl_DayVal.Text.EndsWith("3") Then
                If MyDay.Day = 13 Then
                    .lbl_thLabel.Text = "th"
                Else
                    .lbl_thLabel.Text = "rd"
                End If
            End If
        End With

    End Sub

    Private Sub btn_Cancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click

        If Me.Opacity < 1 Then Exit Sub

        SetMode()

        With ActiveProc
            ._Mode = optMode.DataEntry
            .DataRecorded = 0
        End With

        If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
            If My.Computer.Network.IsAvailable Then
                Try
                    If My.Computer.Network.Ping(MachineSystemServer, 5000) Then
                        If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed And System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CheckForUpdate Then
                            With ActiveProc
                                If ActiveProc._MachineType = MachineType.PC Then
                                    With Me
                                        .tmr_IOScan.Enabled = False
                                        .tmr_DispLED.Enabled = False
                                    End With
                                End If
                            End With

                            Me.Opacity = 0.7
                            MessageBox.Show("The program detected newer version is available on the publisher's network ! Please click 'OK' to proceed the update now!", "Network Deployment...", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            ActiveProc._Mode = optMode.Update

                            AddHandler System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateCompleted, AddressOf AppReStart
                            System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateAsync()
                            Me.Opacity = 1
                        End If
                    End If
                Catch ex As Exception

                End Try
            End If
        End If

    End Sub

    Private Sub AppReStart()

        Application.Restart()

    End Sub

    Private Sub btn_ML_ErrRst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_ErrRst.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        Dim ML_Cmd As String = "ERR"
        Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

        Me.pic_Animation.Tag = "0"

    End Sub

    Public Function SendMLCmd(ByVal ML_Cmd As String, Optional ByRef RepData As String = "") As Integer

        Dim RetData As String = String.Empty
        Dim RetCmd As Integer = ML7111A_cmd(ML_Cmd, RetData)


        With Me
            Dim NowDate As Date = Now
            Dim LogData As String = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", NowDate.Day, NowDate.Month, NowDate.Year, NowDate.Hour, NowDate.Minute, NowDate.Second) & vbTab

            LogData &= ML_Cmd

            If RetCmd >= 0 Then
                RepData = RetData
                LogData &= vbTab & "->" & vbTab & RetData & vbCrLf
            Else
                LogData &= vbTab & "->" & vbTab & "Fail..." & vbCrLf
            End If

            LogData &= .txt_LogData.Text
            .txt_LogData.Text = LogData
        End With

        Return RetCmd

    End Function

    Private Sub btn_ML_CurMode_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_ML_CurMode.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        Dim ML_Cmd As String = "RLR"
        Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_Send_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Send.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = Me.txt_CustomCmd.Text.Trim.ToUpper
            Dim RetCmd As Integer = 0

            Try
                If ActiveProc._LaserIUnit = LaserUnit.ML7110B Then
                    If ML_Cmd.ToUpper.StartsWith("MRR") Or ML_Cmd.ToUpper.StartsWith("MRW") Then
                        Dim LayoutNo As String = ML_Cmd.Substring(0, ML_Cmd.IndexOf(","))
                        LayoutNo = LayoutNo.Substring(3)

                        RetCmd = SendMLCmd("CMW" & LayoutNo)
                        RetCmd = SendMLCmd(ML_Cmd)
                    Else
                        RetCmd = SendMLCmd(ML_Cmd)
                    End If
                Else
                    RetCmd = SendMLCmd(ML_Cmd)
                End If
            Catch ex As Exception
                MessageBox.Show("Command Error...", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Try

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Public Function GetOffsetValue(ByVal AxisLabel As String, Optional ByRef value As String = "") As Integer

        Dim OffsetValue As String = String.Empty


        With Me
            If Not .pic_Animation.Tag = "0" Then Return 0
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = AxisLabel & "OR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd, OffsetValue)

            OffsetValue = OffsetValue.Replace(Chr(ch_STX), "")
            OffsetValue = OffsetValue.Replace(Chr(ch_ETX), "")
            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"
        value = OffsetValue

        Return 0

    End Function

    Public Function Set_ML_Mode(Optional ByVal IntExt As Integer = 0) As Integer

        Dim ML_Cmd As String = "RLW" & IntExt.ToString.Trim
        Return SendMLCmd(ML_Cmd)

    End Function

    Private Sub btn_ML_CurLayout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_CurLayout.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "LNR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_CheckLD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_CheckLD.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "LMR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetQSW_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetQSW.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "PUR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetSpeed_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetSpeed.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "SPR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetCurrent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetCurrent.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "CUR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetRotation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetRotation.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "RTR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetYoffset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetYoffset.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "YOR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetXoffset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetXoffset.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "XOR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetMarkingStatus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetMarkingStatus.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "MSR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Function SaveRecord() As Integer

        Dim DateRecord As Date = Now
        ActiveProc.Lotdata(1).RecDate = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", DateRecord.Month, DateRecord.Day, DateRecord.Year, DateRecord.Hour, DateRecord.Minute, DateRecord.Second)

        Dim FuncRet As Integer = 0
        Dim FormMarking(13) As String


        With ActiveProc.Lotdata(1)
            FormMarking(0) = .Lot_No
            FormMarking(1) = .IMI_No
            FormMarking(2) = .FreqVal
            FormMarking(3) = .Opt
            FormMarking(4) = .RecDate
            FormMarking(5) = .Profile.Substring(0, .Profile.IndexOf(","))
            FormMarking(6) = .CtrlNo
            FormMarking(7) = .MacNo
            FormMarking(8) = .MData1
            FormMarking(9) = .MData2
            FormMarking(10) = .MData3
            FormMarking(11) = .MData4
            FormMarking(12) = .MData5
            FormMarking(13) = .MData6
        End With

        If ActiveProc.Lotdata(1).Lot_No.Length < 18 Then
#If UseWebServices = 1 Then
            FuncRet = azWebService.UpdateRecords(FormMarking)
#Else
                    FuncRet = InsertNewRecord_odbc(ActiveProc.Lotdata(1))
#End If
        Else
            FuncRet = 1
        End If


        'With Me
        '    .pic_PostDone.Visible = True
        '    .btn_Post.Enabled = False
        '    .btn_Cancel.Text = "Reset"
        'End With

        If FuncRet > 0 Then
            Return Func_Ret_Success
            'MessageBox.Show("Data has been completely transfered and recorded!", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Return Func_Ret_Fail
            'If FuncRet <= 0 And FuncRet >= -10 Then
            '    MessageBox.Show("Data has been completely transfered but data was not being recorded!", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            'Else
            '    MessageBox.Show("Data has been completely transfered but text data was not being recorded!", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            'End If
        End If

    End Function

    Private Sub btn_Post_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Post.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .tmr_PostRdy.Enabled = False
            .pic_Animation.Tag = "1"
            .lbl_WarningMsg.Visible = False
        End With

        With ActiveProc
            '.SysBsyCode = 1
            .SysBsyCode = 2

            If ._LaserIUnit = LaserUnit.SigmaKoki Then
                .SysBsyCode = SendToSK(ActiveProc.Lotdata(1))
            Else
                frm_SystemBusy.ShowDialog(Me)
            End If
        End With

        With Me
            .pic_Animation.Tag = "0"

            If ActiveProc.SysBsyCode = 0 Then
                .pic_PostDone.Visible = True
                .btn_Post.Enabled = False

                Dim DateRecord As Date = Now
                ActiveProc.Lotdata(1).RecDate = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", DateRecord.Month, DateRecord.Day, DateRecord.Year, DateRecord.Hour, DateRecord.Minute, DateRecord.Second)

                Dim FuncRet As Integer = 0
                Dim FormMarking(13) As String


                With ActiveProc.Lotdata(1)
                    FormMarking(0) = .Lot_No
                    FormMarking(1) = .IMI_No
                    FormMarking(2) = .FreqVal
                    FormMarking(3) = .Opt
                    FormMarking(4) = .RecDate
                    FormMarking(5) = .Profile.Substring(0, .Profile.IndexOf(","))
                    FormMarking(6) = .CtrlNo
                    FormMarking(7) = .MacNo
                    FormMarking(8) = .MData1
                    FormMarking(9) = .MData2
                    FormMarking(10) = .MData3
                    FormMarking(11) = .MData4
                    FormMarking(12) = .MData5
                    FormMarking(13) = .MData6
                End With

                If ActiveProc.Lotdata(1).Lot_No.Length < 18 Then
#If UseWebServices = 1 Then
                    If Not ActiveProc._MachineType = MachineType.PC Then
                        FuncRet = azWebService.UpdateRecords(FormMarking)
                    End If
#Else
                    FuncRet = InsertNewRecord_odbc(ActiveProc.Lotdata(1))
#End If
                Else
                    FuncRet = 1
                End If

                .btn_Cancel.Text = "Reset"

                If FuncRet > 0 Then
                    MessageBox.Show("Data has been completely transfered and recorded!", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    If FuncRet <= 0 And FuncRet >= -10 Then
                        MessageBox.Show("Data has been completely transfered but data was not being recorded!", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Else
                        MessageBox.Show("Data has been completely transfered but text data was not being recorded!", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End If
                End If

                With ActiveProc
                    .DataRecorded = 0
                    ._Mode = optMode.Auto
                End With
            Else
                .btn_Post.Enabled = True
                .btn_Cancel.Text = "Cancel"
                .lbl_WarningMsg.Visible = True

                MessageBox.Show("Data not being transfered.", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)

                With .tmr_PostRdy
                    .Interval = 2.5 * 1000      'Tick every 2.5 sec
                    .Enabled = True
                End With

                frm_WarnMsg.Show(Me)
            End If
        End With

    End Sub

    Private Sub btn_SetLD_ON_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_SetLD_ON.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "LMW1"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_Marking_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Marking.Click

        With ActiveProc
            If ._MachineType = MachineType.PC Then
                If .IO.i2_sw_Cover.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                    .ErrorMsg = "Safety cover not closed..."
                    frm_Alarm.ShowDialog(Me)
                    Exit Sub
                End If
            End If
        End With

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "MSW1"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged

        With ActiveProc
            If Not Me.TabControl1.SelectedIndex = 0 And Not Me.TabControl1.SelectedIndex = 3 Then
                .AuthenticalAccess = ""
                .TempCondSetting = .MarkingCondSetting
                .TempSetting = .MarkingSetting(0)

                With Me
                    Me.DispMarkingSetting()
                    .btn_Save.Enabled = False
                    .btn_PostSetting.Enabled = False
                End With

                If .GetAuthentication = 0 Then
                    frm_Access.ShowDialog(Me)

                    If .AuthenticalAccess = .AuthenticationCode Then
                        'If Me.TabControl1.SelectedIndex = 1 Then
                        'End If
                        .GetAuthentication = 1
                    Else
                        Me.TabControl1.SelectedIndex = 0
                    End If
                End If
            Else
                .GetAuthentication = 0
            End If
        End With

    End Sub

    Private Sub tmr_Marking_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_Marking.Tick

        Static WarnMsg As Integer


        With Me
            If .btn_Post.Visible = True And .btn_Post.Enabled = True Then
                .lbl_WarningMsg.Text = WarningMsg1.Substring(0, WarnMsg + 1)
                WarnMsg += 1
                If WarnMsg >= WarningMsg1.Length Then WarnMsg = 0
            Else
                If Not .btn_Post.Enabled = True Or Not .btn_Post.Visible = True Then
                    WarnMsg = 0
                End If
            End If
        End With

    End Sub

    Private Sub lbl_SetCurrent__DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbl_SetCurrent_.DoubleClick, lbl_SetQSW_.DoubleClick, lbl_SetSpeed_.DoubleClick, lbl_SetXoffset_.DoubleClick, lbl_SetYoffset_.DoubleClick, lbl_SetRotation_.DoubleClick

        With ActiveProc
            If sender.Equals(Me.lbl_SetCurrent_) Then
                .EditParameter.IdxNo = 0
                .EditParameter.OldData = .MarkingCondSetting.E_Current
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetCurrent_.Text = .EditParameter.NewData
                    .TempCondSetting.E_Current = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetQSW_) Then
                .EditParameter.IdxNo = 1
                .EditParameter.OldData = .MarkingCondSetting.F_QSW
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetQSW_.Text = .EditParameter.NewData
                    .TempCondSetting.F_QSW = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetSpeed_) Then
                .EditParameter.IdxNo = 2
                .EditParameter.OldData = .MarkingCondSetting.G_Speed
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetSpeed_.Text = .EditParameter.NewData
                    .TempCondSetting.G_Speed = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetXoffset_) Then
                .EditParameter.IdxNo = 3
                .EditParameter.OldData = .MarkingCondSetting.B_Xoffset
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetXoffset_.Text = .EditParameter.NewData
                    .TempCondSetting.B_Xoffset = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetYoffset_) Then
                .EditParameter.IdxNo = 4
                .EditParameter.OldData = .MarkingCondSetting.C_Yoffset
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetYoffset_.Text = .EditParameter.NewData
                    .TempCondSetting.C_Yoffset = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetRotation_) Then
                .EditParameter.IdxNo = 5
                .EditParameter.OldData = .MarkingCondSetting.D_Rotation
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetRotation_.Text = .EditParameter.NewData
                    .TempCondSetting.D_Rotation = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            End If
        End With

    End Sub

    Private Sub btn_Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Save.Click

        With ActiveProc
            .MarkingCondSetting = .TempCondSetting
            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex) = .TempSetting

            Dim NewSetting As String = FormPostStream(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex), 1)

            If Me.cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
            Else
                If Not Me.lbl_MarkChar4.Text = "" Then
                    If Me.cbo_BlockNo.SelectedIndex = 3 Then
                        If .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).B_X_Axis = "0" Or .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).C_Y_Axis = "0" Or _
                            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).K_CharHeight = "" Or .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).L_Compress = "" Then
                            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex) = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex - 1)
                            NewSetting = FormPostStream(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex), 1)
                            NewSetting = NewSetting.Substring(0, NewSetting.LastIndexOf(",") + 1) & "_"
                        End If
                    End If
                End If
            End If

            With Profiles(Me.cbo_Profiles.SelectedIndex)
                .ParamData(Me.cbo_BlockNo.SelectedIndex).SettingString = NewSetting
                .SettingCond.A_Layout = ActiveProc.MarkingCondSetting.A_Layout
                .SettingCond.B_Xoffset = ActiveProc.MarkingCondSetting.B_Xoffset
                .SettingCond.C_Yoffset = ActiveProc.MarkingCondSetting.C_Yoffset
                .SettingCond.D_Rotation = ActiveProc.MarkingCondSetting.D_Rotation
                .SettingCond.E_Current = ActiveProc.MarkingCondSetting.E_Current
                .SettingCond.F_QSW = ActiveProc.MarkingCondSetting.F_QSW
                .SettingCond.G_Speed = ActiveProc.MarkingCondSetting.G_Speed


                If Me.cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
                    .UseBlock = "0"
                Else
                    If Me.lbl_MarkChar4.Text = "" Then
                        .UseBlock = "0"
                    Else
                        .UseBlock = "1"
                    End If
                End If


                Dim SQLcmd As String = "UPDATE Setting SET " & _
                                        "LayoutNo='" & .SettingCond.A_Layout & "', " & _
                                        "Xoffset='" & .SettingCond.B_Xoffset & "', " & _
                                        "Yoffset='" & .SettingCond.C_Yoffset & "', " & _
                                        "Rotate='" & .SettingCond.D_Rotation & "', " & _
                                        "[Current]='" & .SettingCond.E_Current & "', " & _
                                        "QSW='" & .SettingCond.F_QSW & "', " & _
                                        "Speed='" & .SettingCond.G_Speed & "', "

                Select Case Me.cbo_BlockNo.SelectedIndex
                    Case Is = 0
                        SQLcmd &= "Block1='"
                    Case Is = 1
                        SQLcmd &= "Block2='"
                    Case Is = 2
                        SQLcmd &= "Block3='"
                    Case Is = 3
                        SQLcmd &= "Block4='"
                    Case Is = 4
                        SQLcmd &= "Block5='"
                    Case Is = 5
                        SQLcmd &= "Block6='"
                End Select

                If Me.cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
                    SQLcmd &= .ParamData(Me.cbo_BlockNo.SelectedIndex).SettingString & "', " & _
                        "UseDot='" & "1', " & _
                        "UseBlock='" & "0' " & _
                        "WHERE CtrlNo='" & ActiveProc.CtrlNo & "' AND Spec='" & .Spec & "'"
                Else
                    SQLcmd &= .ParamData(Me.cbo_BlockNo.SelectedIndex).SettingString & "', " & _
                        "UseDot='" & IIf(Me.lbl_MarkChar1.Text = Chr(7), "1", "0") & "', " & _
                        "UseBlock='" & IIf(Me.lbl_MarkChar4.Text = "_", "1", "0") & "' " & _
                        "WHERE CtrlNo='" & ActiveProc.CtrlNo & "' AND Spec='" & .Spec & "'"
                End If

                Dim SQLrslt As Integer = 0


#If UseSQL_Server = 0 Then
                SQLrslt = Update_odbcDB_Setting(SQLcmd)
#Else
                SQLrslt = SQL_Server_Proc(SQLcmd, "Marking")
#End If

                If SQLrslt < 0 Then
                    MessageBox.Show("The SQL command fail to execute correctly. The Data is not being saved!", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Else
                    If SQLrslt > 1 Then
                        MessageBox.Show("There are more than 1 records affected which is incorrect. Please check with your system engineer.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End If
                End If
            End With

                'SaveRegMarkingConditionSetting(.MarkingCondSetting, NewSetting, Me.cbo_BlockNo.SelectedIndex)
        End With

        With Me
            .lbl_DeltaX.Visible = False
            .lbl_DeltaY.Visible = False

            .btn_Save.Enabled = False
            .btn_PostSetting.Enabled = True
            .DispMarkingConditionSetting()
            .DispMarkingSetting()
        End With

    End Sub

    Private Sub btn_PostSetting_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_PostSetting.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        With ActiveProc
            '.SelectedBlock = Me.cbo_BlockNo.SelectedIndex
            '.UntilBlock = .SelectedBlock
            .SysBsyCode = 2
        End With

        frm_SystemBusy.ShowDialog(Me)

        With Me
            .pic_Animation.Tag = "0"

            If ActiveProc.SysBsyCode = 0 Then
                .btn_PostSetting.Enabled = False
                MessageBox.Show("Setting Data has been completely transfered !", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                .btn_PostSetting.Enabled = True
                MessageBox.Show("Setting Data not being transfered.", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End With

    End Sub

    Private Sub cbo_DrawType_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cbo_DrawType.MouseDown

        If e.Button = Windows.Forms.MouseButtons.Right Then
            With Me
                .ContextMenuStrip1.Show(.cbo_DrawType, e.X, e.Y)
            End With
        End If

    End Sub

    Private Sub cbo_DrawType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbo_DrawType.SelectedIndexChanged

        If Me.fg_Ignore = 1 Or Me.fg_Load = 1 Then Exit Sub

        With ActiveProc
            If .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).A_DrawType = .TempSetting.A_DrawType Then
                If Not Me.cbo_DrawType.SelectedIndex = Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).A_DrawType) Then
                    Me.cbo_DrawType.SelectedIndex = Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).A_DrawType)
                End If
            Else
                If Not Me.cbo_DrawType.SelectedIndex = Val(.TempSetting.A_DrawType) Then
                    Me.cbo_DrawType.SelectedIndex = Val(.TempSetting.A_DrawType)
                End If
            End If
        End With

    End Sub

    Private Sub cbo_TextAlign_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cbo_TextAlign.MouseDown

        If e.Button = Windows.Forms.MouseButtons.Right Then
            With Me
                .ContextMenuStrip2.Show(.cbo_TextAlign, e.X, e.Y)
            End With
        End If

    End Sub

    Private Sub cbo_TextAlign_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbo_TextAlign.SelectedIndexChanged

        If Me.fg_Ignore = 1 Or Me.fg_Load = 1 Then Exit Sub

        With ActiveProc
            If .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).E_TextAlign = .TempSetting.E_TextAlign Then
                If Not Me.cbo_TextAlign.SelectedIndex = Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).E_TextAlign) Then
                    Me.cbo_TextAlign.SelectedIndex = Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).E_TextAlign)
                End If
            Else
                If Not Me.cbo_TextAlign.SelectedIndex = Val(.TempSetting.E_TextAlign) Then
                    Me.cbo_TextAlign.SelectedIndex = Val(.TempSetting.E_TextAlign)
                End If
            End If
        End With

    End Sub

    Private Sub cbo_SpaceAlign_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cbo_SpaceAlign.MouseDown

        If e.Button = Windows.Forms.MouseButtons.Right Then
            With Me
                .ContextMenuStrip3.Show(.cbo_SpaceAlign, e.X, e.Y)
            End With
        End If

    End Sub

    Private Sub cbo_SpaceAlign_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbo_SpaceAlign.SelectedIndexChanged

        If Me.fg_Ignore = 1 Or Me.fg_Load = 1 Then Exit Sub

        With ActiveProc
            If .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).G_SpaceAlign = .TempSetting.G_SpaceAlign Then
                If Not Me.cbo_SpaceAlign.SelectedIndex = Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).G_SpaceAlign) Then
                    Me.cbo_SpaceAlign.SelectedIndex = Val(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex).G_SpaceAlign)
                End If
            Else
                If Not Me.cbo_SpaceAlign.SelectedIndex = Val(.TempSetting.G_SpaceAlign) Then
                    Me.cbo_SpaceAlign.SelectedIndex = Val(.TempSetting.G_SpaceAlign)
                End If
            End If
        End With

    End Sub

    Private Sub ContextMenuStrip1_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening

    End Sub

    Private Sub sub_DrawType_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles sub_DrawType.Click, sub_TextAlign.Click, sub_SpaceAlign.Click

        With ActiveProc
            If sender.Equals(Me.sub_DrawType) Then
                .EditParameter.IdxNo = 0
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).A_DrawType
                frm_SetOptionVal.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.fg_Ignore = 1

                    Me.cbo_DrawType.SelectedIndex = Val(.EditParameter.NewData)
                    .TempSetting.A_DrawType = .EditParameter.NewData
                    Me.fg_Ignore = 0
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.sub_TextAlign) Then
                .EditParameter.IdxNo = 1
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).E_TextAlign
                frm_SetOptionVal.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.fg_Ignore = 1

                    Me.cbo_TextAlign.SelectedIndex = Val(.EditParameter.NewData)
                    .TempSetting.E_TextAlign = .EditParameter.NewData
                    Me.fg_Ignore = 0
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.sub_SpaceAlign) Then
                .EditParameter.IdxNo = 2
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).G_SpaceAlign
                frm_SetOptionVal.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.fg_Ignore = 1

                    Me.cbo_SpaceAlign.SelectedIndex = Val(.EditParameter.NewData)
                    .TempSetting.G_SpaceAlign = .EditParameter.NewData
                    Me.fg_Ignore = 0
                    Me.btn_Save.Enabled = True
                End If
            End If
        End With

    End Sub

    Private Sub lbl_Xaxis_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbl_Xaxis.DoubleClick, lbl_Yaxis.DoubleClick, lbl_TextAngle.DoubleClick, lbl_WidthAlign.DoubleClick, lbl_SpaceWidth.DoubleClick, lbl_XaxisOrg.DoubleClick, lbl_YaxisOrg.DoubleClick, lbl_CharHeight.DoubleClick, lbl_Compress.DoubleClick, lbl_LayoutNo.DoubleClick

        With ActiveProc
            If sender.Equals(Me.lbl_Xaxis) Then
                .EditParameter.IdxNo = 6
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).B_X_Axis
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_Xaxis.Text = .EditParameter.NewData
                    .TempSetting.B_X_Axis = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString

                    Me.btn_Save.Enabled = True
                    Me.lbl_DeltaX.Text = (Val(.TempSetting.B_X_Axis) - Val(.EditParameter.OldData)) / EditModifier(.EditParameter.IdxNo)
                    Me.lbl_DeltaX.Tag = EditModifier(.EditParameter.IdxNo).ToString
                    Me.lbl_DeltaX.Visible = True
                End If
            ElseIf sender.Equals(Me.lbl_Yaxis) Then
                .EditParameter.IdxNo = 7
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).C_Y_Axis
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_Yaxis.Text = .EditParameter.NewData
                    .TempSetting.C_Y_Axis = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString

                    Me.btn_Save.Enabled = True
                    Me.lbl_DeltaY.Text = (Val(.TempSetting.C_Y_Axis) - Val(.EditParameter.OldData)) / EditModifier(.EditParameter.IdxNo)
                    Me.lbl_DeltaY.Tag = EditModifier(.EditParameter.IdxNo).ToString
                    Me.lbl_DeltaY.Visible = True
                End If
            ElseIf sender.Equals(Me.lbl_TextAngle) Then
                .EditParameter.IdxNo = 8
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).D_TextAngle
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_TextAngle.Text = .EditParameter.NewData
                    .TempSetting.D_TextAngle = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_WidthAlign) Then
                .EditParameter.IdxNo = 9
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).F_WidthAlign
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_WidthAlign.Text = .EditParameter.NewData
                    .TempSetting.F_WidthAlign = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SpaceWidth) Then
                .EditParameter.IdxNo = 10
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).H_SpaceWidth
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SpaceWidth.Text = .EditParameter.NewData
                    .TempSetting.H_SpaceWidth = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_XaxisOrg) Then
                .EditParameter.IdxNo = 11
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).I_X_AxisOrg
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_XaxisOrg.Text = .EditParameter.NewData
                    .TempSetting.I_X_AxisOrg = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_YaxisOrg) Then
                .EditParameter.IdxNo = 12
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).J_Y_AxisOrg
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_YaxisOrg.Text = .EditParameter.NewData
                    .TempSetting.J_Y_AxisOrg = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_CharHeight) Then
                .EditParameter.IdxNo = 13
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).K_CharHeight
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_CharHeight.Text = .EditParameter.NewData
                    .TempSetting.K_CharHeight = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_Compress) Then
                .EditParameter.IdxNo = 14
                .EditParameter.OldData = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).L_Compress
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_Compress.Text = .EditParameter.NewData
                    .TempSetting.L_Compress = .EditParameter.NewData
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_LayoutNo) Then
                .EditParameter.IdxNo = 15
                .EditParameter.OldData = .MarkingCondSetting.A_Layout
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_LayoutNo.Text = .EditParameter.NewData
                    .TempCondSetting.A_Layout = .EditParameter.NewData
                    Me.btn_Save.Enabled = True
                End If
            End If
        End With

    End Sub

    Private Sub frm_Main_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        If fg_Load = 1 Then
            fg_Load = 0

#If dbg = 1 Then
            Exit Sub
#End If

            With Me
                .SQL_Status.Text = ActiveProc.DataBase_.Server
                Application.DoEvents()

                Dim FuncRet As Integer = CheckDatabase()

                If FuncRet < 0 Then
                    Select Case FuncRet
                        Case Is = -1
                            MessageBox.Show("The system database is currently not attached to the system.", "az_ActiveProc", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Case Is = -2
                            MessageBox.Show("Ping command issued to the Database Server (" & ActiveProc.DataBase_.Server & ") was fail.", "az_ActiveProc", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Case Is = -3
                            MessageBox.Show("The network is currently not available. Kindly check with your system engineer.", "az_ActiveProc", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Case Is = -4
                            MessageBox.Show("The system could not reach the Database server (" & ActiveProc.DataBase_.Server & ").", "az_ActiveProc", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Select
                Else
                    FuncRet = GetMarkingSetting()

                    If FuncRet = Func_Ret_Success Then
                        .cbo_Profiles.Items.Clear()

                        For ilp As Integer = 0 To Profiles.GetUpperBound(0)
                            Application.DoEvents()
                            .cbo_Profiles.Items.Add(Profiles(ilp).Spec)
                        Next

                        .cbo_Profiles.SelectedIndex = 0

                        If Profiles.GetUpperBound(0) < 1 Then
                            .btn_Delete.Enabled = False
                        Else
                            .btn_Delete.Enabled = True
                        End If
                    Else
                        If ActiveProc.CtrlNo = "M00000" Or FuncRet = -9999 Then
                            MessageBox.Show("No profile was being attached to the system. The system will loading up the default setting for use.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            frm_Support.ShowDialog(Me)

                            'Reload
                            FuncRet = GetMarkingSetting()

                            If FuncRet = Func_Ret_Success Then
                                .Text = "Laser Marking - Active Procedure " & String.Format("<Ver. : {0:D4}.{1:D2}.{2:D2}.{3:D3}>", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.MinorRevision) & " --> #" & ActiveProc.CtrlNo
                                .cbo_Profiles.Items.Clear()

                                For ilp As Integer = 0 To Profiles.GetUpperBound(0)
                                    Application.DoEvents()
                                    .cbo_Profiles.Items.Add(Profiles(ilp).Spec)
                                Next

                                .cbo_Profiles.SelectedIndex = 0

                                If Profiles.GetUpperBound(0) < 1 Then
                                    .btn_Delete.Enabled = False
                                Else
                                    .btn_Delete.Enabled = True
                                End If
                            End If
                        End If
                    End If
                End If


                Try
                    With .WS_Status
#If UseWebServices = 1 Then
                        'Me.ToolTip1.Show(azWebService.AboutMe, Me.StatusStrip1)
                        .Text = "Connected..."
#Else
                        .Text = "Not Used..."
#End If
                    End With
                Catch ex As Exception
                    With .WS_Status
                        .ToolTipText = ""
                        .Text = "Disconnected..."
                    End With
                End Try
            End With

            Me.pic_Animation.Tag = "0"

            '--- PC Based Controlled ---
            If ActiveProc._MachineType = MachineType.PC Then
                frm_HardwareInit.ShowDialog(Me)

                If ActiveProc.Init_HDW = Func_Ret_Success Then
                    With Me
                        .btn_MoveTray.Visible = True

                        With .tmr_IOScan
                            .Interval = 70
                            .Enabled = True
                        End With

                        With .tmr_DispLED
                            .Interval = 230
                            .Enabled = True
                        End With
                    End With
                End If
            End If
        End If

        ActiveProc._Mode = optMode.DataEntry

    End Sub

    Private Sub WS_Status_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles WS_Status.MouseEnter

#If UseWebServices = 1 Then
        Try
            Me.ToolTip1.Show(azWebService.AboutMe, Me.StatusStrip1)
        Catch ex As Exception

        End Try
#Else
        Me.ToolTip1.Show("Web Services Not Used...", Me.StatusStrip1)
#End If

    End Sub

    Private Sub WS_Status_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles WS_Status.MouseLeave

        Me.ToolTip1.Show("", Me.StatusStrip1)

    End Sub

    Private Sub cbo_BlockNo_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbo_BlockNo.SelectedIndexChanged

        With Me
            .btn_Save.Enabled = False
            .lbl_DeltaX.Visible = False
            .lbl_DeltaY.Visible = False
        End With

        DispMarkingSetting()

    End Sub

    Private Sub cbo_Profiles_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbo_Profiles.SelectedIndexChanged

        With Me
            .btn_Save.Enabled = False
            .lbl_DeltaX.Visible = False
            .lbl_DeltaY.Visible = False

            ParseParamData(.cbo_Profiles.SelectedIndex)

            If .cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
                Set_TCI_Marking(True)

                Dim _profile() As String = {}

                Try
                    _profile = ActiveProc.Lotdata(1).Profile.Split(","c)
                Catch ex As Exception
                    _profile = {"", ""}
                End Try

                If Not _profile(0).ToUpper.Contains("TCI_F") Then
                    .TSX_Marker(0).Text = Chr(7)
                    .TSX_Marker(1).Text = "3e"
                    .TSX_Marker(2).Text = "238"
                    .TSX_Marker(3).Text = "XX"
                    .TSX_Marker(4).Visible = False
                End If
            Else
                Set_TCI_Marking(False)

                If Val(Profiles(.cbo_Profiles.SelectedIndex).UseDot) = 0 Then
                    .lbl_MarkChar1.Text = ""
                Else
                    .lbl_MarkChar1.Text = Chr(7)
                End If

                If Val(Profiles(.cbo_Profiles.SelectedIndex).UseBlock) = 0 Then
                    .lbl_MarkChar4.Text = ""
                Else
                    .lbl_MarkChar4.Text = "_"
                End If
            End If


            DispMarkingConditionSetting()
            DispMarkingSetting()

            .cbo_BlockNo.SelectedIndex = 0
            .GroupBox5.Text = "Laser Setting ~ " & .cbo_Profiles.Text
        End With

    End Sub

    Private Sub btn_Copy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Copy.Click

        frm_NewProfiles.ShowDialog(Me)

        With ActiveProc
            If Not .DataTrans = "" And Not .NewLayoutNo = "" Then
                If Not Me.cbo_Profiles.FindString(.DataTrans) < 0 Then
                    MessageBox.Show("The profile name already exists in the system Database. Duplicated name can not be used.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                Me.fg_IgnoreSelect = 1
                Dim NewProfile As ParameterProfile = Profiles(Me.cbo_Profiles.SelectedIndex)
                .CurSelection = .DataTrans

                Dim AffectedRec As Integer = 0

                NewProfile.Spec = .DataTrans
                NewProfile.SettingCond.A_Layout = .NewLayoutNo


#If UseSQL_Server = 1 Then
                AffectedRec = InsertNewProfile_sql(NewProfile)
#Else
                AffectedRec = InsertNewProfile_odbc(NewProfile)
#End If

                Dim FuncRet As Integer = GetMarkingSetting()

                With Me
                    If FuncRet = Func_Ret_Success Then
                        .cbo_Profiles.Items.Clear()

                        For ilp As Integer = 0 To Profiles.GetUpperBound(0)
                            Application.DoEvents()
                            .cbo_Profiles.Items.Add(Profiles(ilp).Spec)
                        Next

                        .fg_IgnoreSelect = 0
                        .cbo_Profiles.SelectedIndex = .cbo_Profiles.FindString(ActiveProc.DataTrans)
                    End If
                End With
            End If

            Me.fg_IgnoreSelect = 0
        End With

    End Sub

    Private Function FindArrayItem(ByVal ProfileLookAt As ParameterProfile) As Boolean

        If ProfileLookAt.Spec = ActiveProc.CurSelection Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Sub btn_Delete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Delete.Click

        If MessageBox.Show("Are you sure you want to permenantly delete this profile (" & Profiles(Me.cbo_Profiles.SelectedIndex).Spec & ") ?", "az_Active...", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            Dim SQLrslt As Integer = 0
            Dim SQLcmd As String = "DELETE FROM Setting " & _
                                "WHERE Spec='" & Profiles(Me.cbo_Profiles.SelectedIndex).Spec & "' " & _
                                "AND CtrlNo='" & ActiveProc.CtrlNo & "'"


#If UseSQL_Server = 0 Then
            SQLrslt  = Update_odbcDB_Setting(SQLcmd)
#Else
            SQLrslt = SQL_Server_Proc(SQLcmd, "Marking")
#End If

            Dim FuncRet As Integer = GetMarkingSetting()

            With Me
                If FuncRet = Func_Ret_Success Then
                    .cbo_Profiles.Items.Clear()

                    For ilp As Integer = 0 To Profiles.GetUpperBound(0)
                        Application.DoEvents()
                        .cbo_Profiles.Items.Add(Profiles(ilp).Spec)
                    Next

                    .cbo_Profiles.SelectedIndex = 0

                    If Profiles.GetUpperBound(0) < 1 Then
                        .btn_Delete.Enabled = False
                    Else
                        .btn_Delete.Enabled = True
                    End If
                End If
            End With
        End If

    End Sub

    Private Sub lbl_MarkChar1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbl_MarkChar1.DoubleClick

        With Me
            'If .lbl_MarkChar1.Text = Chr(7) Then
            '    .lbl_MarkChar1.Text = ""
            'Else
            '    .lbl_MarkChar1.Text = Chr(7)
            'End If

            '.btn_Save.Enabled = True
        End With

    End Sub

    Private Sub lbl_MarkChar4_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbl_MarkChar4.DoubleClick

        With Me
            If Me.cbo_BlockNo.SelectedIndex = 3 Then
                If .lbl_MarkChar4.Text = "_" Then
                    .lbl_MarkChar4.Text = ""
                Else
                    .lbl_MarkChar4.Text = "_"
                End If

                .btn_Save.Enabled = True
            End If
        End With

    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim imiFiles() As String = {}
        Dim FuncRet As Integer = 0
        Dim FormMarking() As String = {}


        With ActiveProc
            If Not GetFilesList(.PreviousVerPath & "\Data\IMI", ".dat", imiFiles) < 0 Then
                For iLp As Integer = 0 To imiFiles.GetUpperBound(0)
                    Application.DoEvents()

#If UseWebServices = 1 Then
                    FuncRet = azWebService.GetMarkingCode("P00-Eva01", imiFiles(iLp).Substring(imiFiles(iLp).LastIndexOf("\") + 1).ToLower.Replace(".dat", ""), FormMarking)
#Else
                    FuncRet = azLMServices.GetMarkingCode("P00-Eva01", imiFiles(iLp).Substring(imiFiles(iLp).LastIndexOf("\") + 1).ToLower.Replace(".dat", ""), FormMarking)
#End If

                    If Not FuncRet < 0 Then
                        With .Lotdata(1)
                            .Lot_No = FormMarking(0)
                            .IMI_No = FormMarking(1)
                            .FreqVal = FormMarking(2)
                            .Opt = FormMarking(3)
                            .RecDate = FormMarking(4)
                            .Profile = FormMarking(5)
                            .CtrlNo = FormMarking(6)
                            .MacNo = FormMarking(7)
                            .MData1 = FormMarking(8)
                            .MData2 = FormMarking(9)
                            .MData3 = FormMarking(10)
                            .MData4 = FormMarking(11)
                            .MData5 = FormMarking(12)
                            .MData6 = FormMarking(13)
                        End With

                        Debug.Print(.Lotdata(1).IMI_No.ToUpper & " -->   " & .Lotdata(1).MData1 & " / " & .Lotdata(1).MData2 & " (" & .Lotdata(1).Profile & ")")
                    Else
                        MessageBox.Show("Fail to form marking code for the spec. : " & imiFiles(iLp), "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        With .Lotdata(1)
                            .Lot_No = ""
                            .IMI_No = ""
                            .MData1 = ""
                            .MData2 = ""
                        End With
                    End If
                Next
            Else
                FuncRet = -1
                MessageBox.Show("Unabled to locate IMI files. Please refer to system engineer for this problem.", "IMI Files not found...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End With

    End Sub

    Private Sub lbl_DeltaX_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lbl_DeltaX.MouseDown, lbl_DeltaY.MouseDown

        With Me
            If e.Button = Windows.Forms.MouseButtons.Right Then
                If sender.Equals(.lbl_DeltaX) Then
                    .ApplyChangesToAllBlocks.Text = "Apply changes to all blocks (X)..."
                    .ContextMenuStrip4.Show(.lbl_DeltaX, e.Location)
                ElseIf sender.Equals(.lbl_DeltaY) Then
                    .ApplyChangesToAllBlocks.Text = "Apply changes to all blocks (Y)..."
                    .ContextMenuStrip4.Show(.lbl_DeltaY, e.Location)
                End If
            End If
        End With

    End Sub

    Private Sub ApplyChangesToAllBlocks_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ApplyChangesToAllBlocks.Click

        Dim Adjust As Integer = 0


        With Me
            If Not .ApplyChangesToAllBlocks.Text.IndexOf("(X)") < 0 Then
                Adjust = Val(.lbl_DeltaX.Text) * Val(.lbl_DeltaX.Tag)
                .lbl_DeltaX.Visible = False
            Else
                Adjust = Val(.lbl_DeltaY.Text) * Val(.lbl_DeltaY.Tag)
                .lbl_DeltaY.Visible = False
            End If
        End With


        With ActiveProc
            .MarkingCondSetting = .TempCondSetting
            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex) = .TempSetting

            For iLp As Integer = 0 To .MarkingSetting.GetUpperBound(0)
                Application.DoEvents()
                Dim TargetBlock As Integer = 0

                If Me.cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
                    If Not iLp = Me.cbo_BlockNo.SelectedIndex Then
                        If Not Me.ApplyChangesToAllBlocks.Text.IndexOf("(X)") < 0 Then
                            .MarkingSetting(iLp).B_X_Axis += Adjust
                        Else
                            .MarkingSetting(iLp).C_Y_Axis += Adjust
                        End If
                    End If
                Else
                    If iLp <= (1 + Val(Profiles(.SelectedProfile).UseDot) + Val(Profiles(.SelectedProfile).UseBlock)) Then
                        If Not iLp = Me.cbo_BlockNo.SelectedIndex Then
                            If Not Me.ApplyChangesToAllBlocks.Text.IndexOf("(X)") < 0 Then
                                .MarkingSetting(iLp).B_X_Axis += Adjust
                            Else
                                .MarkingSetting(iLp).C_Y_Axis += Adjust
                            End If
                        End If
                    End If
                End If

                .MarkingSetting(iLp).SettingString = FormPostStream(.MarkingSetting(iLp), 1)
            Next

            If Me.cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then

            Else
                Dim MarkUseBlock As Integer = 3

                If Not Me.lbl_MarkChar4.Text = "" Then
                    If Not Me.cbo_Profiles.SelectedIndex < 0 Then
                        If Val(Profiles(Me.cbo_Profiles.SelectedIndex).UseDot) = 1 Then
                            MarkUseBlock = 3
                        Else
                            MarkUseBlock = 2
                        End If
                    End If

                    If Me.cbo_BlockNo.SelectedIndex = MarkUseBlock Then
                        If .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).B_X_Axis = "0" Or .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).C_Y_Axis = "0" Or _
                            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).K_CharHeight = "" Or .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).L_Compress = "" Then
                            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex) = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex - 1)
                            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).SettingString = FormPostStream(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex), 1)
                            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).SettingString = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).SettingString.Substring(0, .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).SettingString.LastIndexOf(",") + 1) & "_"
                        End If
                    End If
                End If
            End If

            With Profiles(Me.cbo_Profiles.SelectedIndex)
                .SettingCond.A_Layout = ActiveProc.MarkingCondSetting.A_Layout
                .SettingCond.B_Xoffset = ActiveProc.MarkingCondSetting.B_Xoffset
                .SettingCond.C_Yoffset = ActiveProc.MarkingCondSetting.C_Yoffset
                .SettingCond.D_Rotation = ActiveProc.MarkingCondSetting.D_Rotation
                .SettingCond.E_Current = ActiveProc.MarkingCondSetting.E_Current
                .SettingCond.F_QSW = ActiveProc.MarkingCondSetting.F_QSW
                .SettingCond.G_Speed = ActiveProc.MarkingCondSetting.G_Speed

                If Me.cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
                    .UseBlock = "0"
                Else
                    If Me.lbl_MarkChar4.Text = "" Then
                        .UseBlock = "0"
                    Else
                        .UseBlock = "1"
                    End If
                End If


                Dim SQLcmd As String = "UPDATE Setting SET " & _
                                        "LayoutNo='" & .SettingCond.A_Layout & "', " & _
                                        "Xoffset='" & .SettingCond.B_Xoffset & "', " & _
                                        "Yoffset='" & .SettingCond.C_Yoffset & "', " & _
                                        "Rotate='" & .SettingCond.D_Rotation & "', " & _
                                        "[Current]='" & .SettingCond.E_Current & "', " & _
                                        "QSW='" & .SettingCond.F_QSW & "', " & _
                                        "Speed='" & .SettingCond.G_Speed & "', " & _
                                        "Block1='" & ActiveProc.MarkingSetting(0).SettingString & "', " & _
                                        "Block2='" & ActiveProc.MarkingSetting(1).SettingString & "', " & _
                                        "Block3='" & ActiveProc.MarkingSetting(2).SettingString & "', " & _
                                        "Block4='" & ActiveProc.MarkingSetting(3).SettingString & "', " & _
                                        "Block5='" & ActiveProc.MarkingSetting(4).SettingString & "', " & _
                                        "Block6='" & ActiveProc.MarkingSetting(5).SettingString & "', "


                If Me.cbo_Profiles.SelectedItem.ToString.ToUpper.Contains("TCI_F") Then
                    SQLcmd &= "UseDot='" & "1', " & _
                            "UseBlock='" & "0' " & _
                            "WHERE CtrlNo='" & ActiveProc.CtrlNo & "' AND Spec='" & .Spec & "'"
                Else
                    SQLcmd &= "UseDot='" & IIf(Me.lbl_MarkChar1.Text = Chr(7), "1", "0") & "', " & _
                            "UseBlock='" & IIf(Me.lbl_MarkChar4.Text = "_", "1", "0") & "' " & _
                            "WHERE CtrlNo='" & ActiveProc.CtrlNo & "' AND Spec='" & .Spec & "'"
                End If


                Dim SQLrslt As Integer = 0


#If UseSQL_Server = 0 Then
                SQLrslt = Update_odbcDB_Setting(SQLcmd)
#Else
                SQLrslt = SQL_Server_Proc(SQLcmd, "Marking")
#End If

                If SQLrslt < 0 Then
                    MessageBox.Show("The SQL command fail to execute correctly. The Data is not being saved!", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Else
                    If SQLrslt > 1 Then
                        MessageBox.Show("There are more than 1 records affected which is incorrect. Please check with your system engineer.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End If
                End If
            End With

            'SaveRegMarkingConditionSetting(.MarkingCondSetting, NewSetting, Me.cbo_BlockNo.SelectedIndex)
        End With


        With Me
            If Me.lbl_DeltaX.Visible = False And Me.lbl_DeltaY.Visible = False Then
                Me.btn_Save.Enabled = False
            End If

            .btn_PostSetting.Enabled = True
            .DispMarkingSetting()
        End With

    End Sub

    Private Sub txt_CustomCmd_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt_CustomCmd.KeyDown

        If e.KeyValue = Keys.Enter Then
            Me.btn_Send.PerformClick()
        End If

    End Sub

    Private Sub tmr_PostRdy_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_PostRdy.Tick

        Static TmrCnt As Integer


        If frm_WarnMsg.IsHandleCreated = False Then
            TmrCnt += 1

            If TmrCnt >= 4 Then    '10 sec
                TmrCnt = 0
                Me.tmr_PostRdy.Enabled = False

                If Me.btn_Post.Visible = True And Me.btn_Post.Enabled = True Then
                    If Set_ML_Mode() >= 0 Then
                        Me.btn_Post.PerformClick()
                    Else
                        With Me.tmr_PostRdy
                            .Interval = 1.25 * 1000      'Tick every 1.25 sec
                            .Enabled = True
                        End With

                        frm_WarnMsg.Show(Me)
                    End If
                End If
            End If
        End If

    End Sub

    Private Sub btn_MoveTray_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_MoveTray.Click

        With ActiveProc
            If .IO.i2_sw_Cover.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                .ErrorMsg = "Safety cover not closed..."
                frm_Alarm.ShowDialog(Me)
                Exit Sub
            End If

            Me.btn_MoveTray.Enabled = False
            Dim wt As Integer = My.Computer.Clock.TickCount
            Dim ret As Integer = Func_Ret_Success

            If .IO.i7_sw_slsRight.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                .IO.o4_Tray_Sol.Trigger_ON()

                Do Until .IO.i7_sw_slsRight.BitState = cls_PCIBoard.BitsState.eBit_OFF
                    Application.DoEvents()

                    If My.Computer.Clock.TickCount > wt + 3000 Then
                        ret = Func_Ret_Fail
                        .ErrorMsg = "Cylinder Sensor (Right) abnormal..."
                        frm_Alarm.ShowDialog(Me)
                        Me.btn_MoveTray.Enabled = True

                        Exit Sub
                    End If
                Loop

                wt = My.Computer.Clock.TickCount

                Do Until .IO.i6_sw_slsLeft.BitState = cls_PCIBoard.BitsState.eBit_ON
                    Application.DoEvents()

                    If My.Computer.Clock.TickCount > wt + 3000 Then
                        ret = Func_Ret_Fail
                        .ErrorMsg = "Cylinder Sensor (Left) abnormal..."
                        frm_Alarm.ShowDialog(Me)

                        Exit Do
                    End If
                Loop
            Else
                .IO.o4_Tray_Sol.Trigger_OFF()

                Do Until .IO.i6_sw_slsLeft.BitState = cls_PCIBoard.BitsState.eBit_OFF
                    Application.DoEvents()

                    If My.Computer.Clock.TickCount > wt + 3000 Then
                        ret = Func_Ret_Fail
                        .ErrorMsg = "Cylinder Sensor (Left) abnormal..."
                        frm_Alarm.ShowDialog(Me)
                        Me.btn_MoveTray.Enabled = True

                        Exit Sub
                    End If
                Loop

                wt = My.Computer.Clock.TickCount

                Do Until .IO.i7_sw_slsRight.BitState = cls_PCIBoard.BitsState.eBit_ON
                    Application.DoEvents()

                    If My.Computer.Clock.TickCount > wt + 3000 Then
                        ret = Func_Ret_Fail
                        .ErrorMsg = "Cylinder Sensor (Right) abnormal..."
                        frm_Alarm.ShowDialog(Me)

                        Exit Do
                    End If
                Loop
            End If

            Me.btn_MoveTray.Enabled = True
        End With

    End Sub

    Private Sub tmr_IOScan_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_IOScan.Tick

        Static wt As Integer = 0


        With ActiveProc
            Select Case .Init_HDW
                Case Is = Func_Ret_Success
                    If .IO.i3_sw_Start.BitState = cls_PCIBoard.BitsState.eBit_ON AndAlso .IO.i4_sw_Stop.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        If Me.TabControl1.SelectedIndex = 0 Then
                            Me.tmr_IOScan.Enabled = False

                            Do Until .IO.i3_sw_Start.BitState = cls_PCIBoard.BitsState.eBit_OFF
                                Application.DoEvents()
                            Loop

                            If ._Mode = optMode.DataEntry Then
                                If frm_DataEntry.IsHandleCreated = False Then Me.btn_DataEntry.PerformClick()
                            ElseIf ._Mode = optMode.PostData Then
                                If frm_SystemBusy.IsHandleCreated = False Then Me.btn_Post.PerformClick()
                            ElseIf ._Mode = optMode.Auto Then
                                .IO.o3_Start_LED.Trigger_ON()
                                If frm_AutoRun.IsHandleCreated = False Then frm_AutoRun.ShowDialog(Me)

                                If .DataRecorded = 0 Then
                                    If SaveRecord() = Func_Ret_Success Then
                                        .DataRecorded = 1
                                    Else
                                        .DataRecorded = 0
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If .IO.i4_sw_Stop.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                        If Me.TabControl1.SelectedIndex = 0 Then
                            If wt = 0 Then wt = My.Computer.Clock.TickCount

                            If ._Mode = optMode.Auto Then
                                If frm_AutoRun.IsHandleCreated = False Then
                                    If My.Computer.Clock.TickCount > wt + 3000 Then
                                        Me.btn_Cancel.PerformClick()
                                    End If
                                End If
                            End If
                        End If
                    Else
                        wt = 0
                    End If

                    If .IO.i9_in_LaserRdy.BitState = cls_PCIBoard.BitsState.eBit_ON Then
                        .IO.o2_LaserRdy_LED.Trigger_ON()
                    End If

                    If .IO.i5_sw_Power.BitState = cls_PCIBoard.BitsState.eBit_OFF Then
                        .ErrorMsg = "System power failure..."
                        frm_Alarm.ShowDialog(Me)
                    End If

                    Me.tmr_IOScan.Enabled = True
            End Select
        End With

    End Sub

    Private Sub tmr_DispLED_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_DispLED.Tick

        Static _machineState As Integer
        _machineState = _machineState Xor 1


        With ActiveProc
            If _machineState = fg_ON Then
                If frm_Alarm.IsHandleCreated = False Then
                    .IO.o3_Start_LED.Trigger_ON()
                End If
            Else
                If frm_AutoRun.IsHandleCreated = False Then
                    .IO.o3_Start_LED.Trigger_OFF()
                End If
            End If
        End With

    End Sub

    Private Sub btn_CheckWC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_CheckWC.Click

        Dim fn As String = String.Empty
        Dim sFn As String = String.Empty


        With Me.OpenFileDialog1
            .ShowDialog()
            fn = .FileName
        End With

        If fn <> "" Then
            Dim allfiles As String = My.Computer.FileSystem.ReadAllText(fn, System.Text.Encoding.ASCII)
            allfiles = allfiles.Replace(vbLf, "")

            Dim file() As String = allfiles.Split(vbCr)

            With Me.SaveFileDialog1
                .ShowDialog()
                sFn = .FileName
            End With

            If sFn = "" Then
                MessageBox.Show("You need to provide a filename to save the result into your hard drive.", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End If

            For ilP As Integer = 0 To file.GetUpperBound(0)
                Application.DoEvents()

                If file(ilP).Trim <> "" Then
                    Dim imi() As String = file(ilP).Split("."c)
                    Dim FormMarking() As String = {}
                    Dim FuncRet As Integer = 0

#If UseWebServices = 1 Then
                    FuncRet = azWebService.GetMarkingCode("PAZ-DMY" & String.Format("{0:D2}{1:D2}", Now.Day, Now.Hour), imi(0).Trim, FormMarking)
#Else
                    FuncRet = azLMServices.GetMarkingCode(.Lotdata(1).Lot_No, .Lotdata(1).IMI_No, FormMarking)
#End If
                    If Not FuncRet < 0 Then
                        My.Computer.FileSystem.WriteAllText(sFn, FormMarking(1) & ", " & FormMarking(8) & ", " & FormMarking(9) & ", " & FormMarking(5) & vbCrLf, True, System.Text.Encoding.ASCII)
                    End If
                End If
            Next

            MessageBox.Show("Weekcode generated successfully!!!", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    Private Sub btn_DbgCmd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_DbgCmd.Click

        'If Set_ML_Mode(0) >= 0 Then
        '    Dim ML_Cmd As String = Me.txt_CustomCmd.Text.Trim.ToUpper
        '    Dim RetCmd As Integer = SendMLCmd(ML_Cmd)
        'End If

        SQL_CustFunction("M00001", "M00002", "Spec='TCI Format'")

    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        Me.txt_LogData.Text = ""

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        Dim DateRecord As Date = Now
        ActiveProc.Lotdata(1).RecDate = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", DateRecord.Month, DateRecord.Day, DateRecord.Year, DateRecord.Hour, DateRecord.Minute, DateRecord.Second)

        Dim FuncRet As Integer = 0
        Dim FormMarking(13) As String


        With ActiveProc.Lotdata(1)
            FormMarking(0) = .Lot_No
            FormMarking(1) = .IMI_No
            FormMarking(2) = .FreqVal
            FormMarking(3) = .Opt
            FormMarking(4) = .RecDate
            FormMarking(5) = .Profile.Substring(0, .Profile.IndexOf(","))
            FormMarking(6) = .CtrlNo
            FormMarking(7) = .MacNo
            FormMarking(8) = .MData1
            FormMarking(9) = .MData2
            FormMarking(10) = .MData3
            FormMarking(11) = .MData4
            FormMarking(12) = .MData5
            FormMarking(13) = .MData6
        End With

        If ActiveProc.Lotdata(1).Lot_No.Length < 18 Then
#If UseWebServices = 1 Then
            If Not ActiveProc._MachineType = MachineType.PC Then
                FuncRet = azWebService.UpdateRecords(FormMarking)
            End If
#Else
                    FuncRet = InsertNewRecord_odbc(ActiveProc.Lotdata(1))
#End If
        Else
            FuncRet = 1
        End If

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        SendToSK(ActiveProc.Lotdata(1))

    End Sub

End Class
