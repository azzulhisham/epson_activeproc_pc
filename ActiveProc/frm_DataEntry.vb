Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Web.Services.Protocols


Public Class frm_DataEntry

#If UseWebServices = 1 Then
    Dim azWebService As New zulhisham_pc.az_Services
#Else
    Dim azLMServices As New cls_LMservices
#End If


    Dim fg_Load As Integer = 1
    Dim SeqNo As Integer = 0
    Dim MsgLbl() As String = {"Please Enter Lot No. ...", _
                              "Please Enter IMI No. ...", _
                              "Please Enter Week Code ...", _
                              "Please Enter Emp. No. ...", _
                              "Kindly Wait For A Moment..."}
    Dim rplcLbl() As String = {"Please Enter Lot No. ...", _
                               "Please Enter IMI No. ...", _
                               "Please Enter Freq. Value ...", _
                               "Please Enter Emp. No. ...", _
                               "Kindly Wait For A Moment..."}

    Dim BlinkLbl() As String = {"Lot No.", "IMI No.", "Week Code", "Emp. No.", ""}
    Dim BlinkRplc() As String = {"Lot No.", "IMI No.", "Freq.", "Emp. No.", ""}

    Dim bc_LotNo As String = String.Empty
    Dim bc_IMINo As String = String.Empty
    Dim bc_WeekCode As String = String.Empty
    Dim AppMode As Integer = CType(regSubKey.GetValue("SimulationMode", "0"), Integer)


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        With Me
            ActiveProc.Lotdata(1).Lot_No = ""
            .Close()
        End With

    End Sub

    Private Sub frm_DataEntry_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated


    End Sub

    Private Sub frm_DataEntry_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        With Me
            .tmr_Blink.Enabled = False
            .SeqNo = 0
        End With

    End Sub

    Private Sub frm_DataEntry_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        With ActiveProc
            .Lotdata(1).Lot_No = ""
        End With

        With Me
            .fg_Load = 1
            .bc_IMINo = ""
            .bc_LotNo = ""

            If Not Me.AppMode = 0 Then
                ActiveProc.Lotdata(1).Lot_No = "PAZ-DMY" & String.Format("{0:D2}{1:D2}", Now.Day, Now.Hour)
                .SeqNo = 1
            Else
                .SeqNo = 0
            End If

            With .txt_Scan
                .Text = ""
                .Visible = True
            End With

            DispMsg()

            With .tmr_Blink
                .Interval = 250
                .Enabled = True
            End With
        End With

    End Sub

    Private Sub DispMsg(Optional ByVal _replMsg As Integer = 0)

        With Me
            .lbl_Msg.Text = IIf(_replMsg = 0, .MsgLbl(SeqNo), .rplcLbl(SeqNo))
            .lbl_Msg.Refresh()

            .lbl_Label.Text = IIf(_replMsg = 0, .BlinkLbl(SeqNo), .BlinkRplc(SeqNo))
            .lbl_Label.Refresh()

            If SeqNo = .MsgLbl.GetUpperBound(0) Then
                .txt_Scan.Visible = False
                .Button1.Visible = False
                .lbl_Label.Visible = False
            Else
                .txt_Scan.Visible = True
                .Button1.Visible = True
                .lbl_Label.Visible = True
            End If

            Application.DoEvents()
        End With

    End Sub

    Private Sub tmr_Blink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_Blink.Tick

        With Me
            .lbl_Label.Visible = Not .lbl_Label.Visible
        End With

    End Sub

    Private Function ChkLotNo(ByVal LotNo As String) As Integer

        Dim RetVal As Integer = -1


        'If Not LotNo.Contains("-") Then
        '    Return RetVal
        'End If

        If Not LotNo.Contains("-") And LotNo.Length <> 10 Then
            Return RetVal
        End If

        'If Not IsNumeric(LotNo.Substring(LotNo.IndexOf("-") + 1)) And Not LotNo.ToUpper.Trim.Contains("DMY") Then
        '    Return RetVal
        'End If

        If Not IsNumeric(LotNo.Substring(LotNo.IndexOf("-") + 1)) And LotNo.Length <> 10 And Not LotNo.ToUpper.Trim.Contains("DMY") Then
            Return RetVal
        End If

        RetVal = 0
        Dim chCnt = IIf(LotNo.Contains("-"), LotNo.IndexOf("-") - 1, LotNo.Length - 1)

        'For iLp As Integer = 0 To LotNo.IndexOf("-") - 1
        For iLp As Integer = 0 To chCnt
            Dim tmp As String = LotNo.Substring(iLp, 1).ToUpper.Trim
            Dim byteVal As Byte = Asc(tmp)

            If (byteVal < Asc("A") Or byteVal > Asc("Z")) And (byteVal < Asc("0") Or byteVal > Asc("9")) Then
                RetVal = -1
                Exit For
            End If
        Next

        Return RetVal

    End Function

    Private Sub txt_Scan_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt_Scan.KeyDown

        Static CustMarking As Integer


        If e.KeyCode = Keys.Escape Then
            With Me
                .Close()
            End With
        ElseIf e.KeyCode = Keys.Enter Then
            If Me.txt_Scan.Text.Trim.StartsWith("/") Then
                Me.txt_Scan.Text = Me.txt_Scan.Text.Substring(2)
            End If

            With ActiveProc
                Select Case SeqNo
                    Case Is = 0
                        .Lotdata(1).Lot_No = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""

                        If .Lotdata(1).Lot_No.Length = 12 AndAlso .Lotdata(1).Lot_No.ToUpper.StartsWith("V") Then
                            'SeqNo += 2
                            'CustMarking = 1
                        Else
                            If Not Me.AppMode = 0 Then
                                If ChkLotNo(.Lotdata(1).Lot_No) = 0 Then
                                    SeqNo += 1
                                    CustMarking = 0
                                Else
                                    MessageBox.Show("Barcode Error... Invalid Lot No.!", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    Me.bc_LotNo = ""
                                End If
                            Else
                                If Me.bc_LotNo = "" Then
                                    Me.bc_LotNo = .Lotdata(1).Lot_No
                                Else
                                    If Me.bc_LotNo = .Lotdata(1).Lot_No Then
                                        If ChkLotNo(.Lotdata(1).Lot_No) = 0 Then
                                            SeqNo += 1
                                            CustMarking = 0
                                        Else
                                            MessageBox.Show("Barcode Error... Invalid Lot No.!", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                            Me.bc_LotNo = ""
                                        End If
                                    Else
                                        MessageBox.Show("Barcode Error... First scan & second scan not matched!", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                        Me.bc_LotNo = ""
                                    End If
                                End If
                            End If
                        End If

                        DispMsg()

                    Case Is = 1
                        .Lotdata(1).IMI_No = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""

                        If Not Me.AppMode = 0 Then
                            If ._GetWeekCode = WeekCode.Auto And .Lotdata(1).IMI_No.Length <> 12 Then
                                SeqNo += 2
                            Else
                                SeqNo += 1
                            End If
                        Else
                            If Me.bc_IMINo = "" Then
                                Me.bc_IMINo = .Lotdata(1).IMI_No
                            Else
                                If Me.bc_IMINo = .Lotdata(1).IMI_No Then
                                    If ._GetWeekCode = WeekCode.Auto And .Lotdata(1).IMI_No.Length <> 12 Then
                                        SeqNo += 2
                                    Else
                                        SeqNo += 1
                                        Me.bc_WeekCode = ""
                                    End If
                                Else
                                    MessageBox.Show("Barcode Error... First scan & second scan not matched!", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                    Me.bc_IMINo = ""
                                End If
                            End If
                        End If

                        If .Lotdata(1).IMI_No.Length = 12 Then
                            DispMsg(1)
                        Else
                            DispMsg()
                        End If

                    Case Is = 2
                        .Lotdata(1)._WeekCode = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""

                        If .Lotdata(1)._WeekCode.Length < 5 Or .Lotdata(1)._WeekCode.Length > 8 And .Lotdata(1).IMI_No.Length < 12 Then
                            MessageBox.Show("Week code Error... Week code must be between 5 to 8 characters!", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            Me.bc_WeekCode = ""
                            Exit Sub
                        Else
                            If Not Me.AppMode = 0 Then
                                SeqNo += 1
                            Else
                                If Me.bc_WeekCode = "" Then
                                    Me.bc_WeekCode = .Lotdata(1)._WeekCode
                                Else
                                    If Me.bc_WeekCode = .Lotdata(1)._WeekCode Then
                                        If .Lotdata(1).IMI_No.Length = 12 Then
                                            .Lotdata(1).FreqVal = .Lotdata(1)._WeekCode
                                            .Lotdata(1)._WeekCode = ""
                                        End If

                                        SeqNo += 1
                                    Else
                                        MessageBox.Show("Barcode Error... First scan & second scan not matched!", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                        Me.bc_WeekCode = ""
                                        Exit Sub
                                    End If
                                End If
                            End If
                        End If

                        If .Lotdata(1).IMI_No.Length = 12 Then
                            DispMsg(1)
                        Else
                            DispMsg()
                        End If

                    Case Is = 3
                        .Lotdata(1).Opt = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""
                        SeqNo += 1
                        DispMsg()

                        Dim FormMarking() As String = {}
                        Dim FuncRet As Integer = 0
                        ReDim .MarkingChar(5)


                        If (.Lotdata(1).IMI_No.StartsWith("80") Or .Lotdata(1).IMI_No.StartsWith("31") Or .Lotdata(1).IMI_No.StartsWith("32")) AndAlso .Lotdata(1).IMI_No.Length = 12 Then
                            CustMarking = 1
                        Else
                            CustMarking = 0
                        End If


                        Try
                            If CustMarking = 0 Then

#If UseWebServices = 1 Then
                                If ._GetWeekCode = WeekCode.Auto Then
                                    FuncRet = azWebService.GetMarkingCode(.Lotdata(1).Lot_No, .Lotdata(1).IMI_No, FormMarking)
                                Else
                                    FuncRet = azWebService.az_SDMarking_ad(.Lotdata(1).Lot_No, .Lotdata(1).IMI_No & ";" & .Lotdata(1)._WeekCode, FormMarking)
                                End If
#Else
                                FuncRet = azLMServices.GetMarkingCode(.Lotdata(1).Lot_No, .Lotdata(1).IMI_No, FormMarking)
#End If


                                If Not FuncRet < 0 Then
                                    If FuncRet = 1 Then
                                        frm_Main.RecStatus.Text = "Previous Records..."
                                    Else
                                        frm_Main.RecStatus.Text = "New Marking Code..."
                                    End If

                                    'Check Weekcode (for SD Only)
                                    If Not ._GetWeekCode = WeekCode.Auto Then
                                        Dim SQLrslt As Integer = 0
                                        Dim SQLcmd As String = "SELECT * FROM Records WHERE IMI_No='" & .Lotdata(1).IMI_No & "' AND MData2='" & .Lotdata(1)._WeekCode & "'"

#If UseSQL_Server = 0 Then
                                        SQLrslt = Update_odbcDB_Setting(SQLcmd)
#Else
                                        Dim ChkdbRec As Rec = Nothing
                                        SQLrslt = SQL_Server_Proc(SQLcmd, "Marking", ChkdbRec)

                                        If SQLrslt > 0 Then
                                            If ChkdbRec.Lot_No.ToUpper <> .Lotdata(1).Lot_No Then
                                                MessageBox.Show("Invalid week code : " & .Lotdata(1)._WeekCode, "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                .Lotdata(1).Lot_No = ""
                                                .Lotdata(1).IMI_No = ""

                                                Me.Close()
                                                Exit Sub
                                            End If
                                        End If
#End If
                                    End If


                                    With .Lotdata(1)
                                        .Lot_No = FormMarking(0).Trim
                                        .IMI_No = FormMarking(1).Trim
                                        .FreqVal = FormMarking(2).Trim
                                        .Opt = IIf(FormMarking(3) = "", ActiveProc.Lotdata(1).Opt, FormMarking(3))
                                        .RecDate = FormMarking(4)
                                        .Profile = FormMarking(5).Trim
                                        .CtrlNo = IIf(FormMarking(6) = "", ActiveProc.CtrlNo, FormMarking(6))
                                        .MacNo = IIf(FormMarking(7) = "", "-", FormMarking(7))
                                        .MData1 = FormMarking(8).Trim
                                        .MData2 = FormMarking(9).Trim
                                        .MData3 = FormMarking(10).Trim
                                        .MData4 = FormMarking(11)
                                        .MData5 = FormMarking(12)
                                        .MData6 = FormMarking(13)
                                    End With

                                    For iLp As Integer = 0 To .MarkingChar.GetUpperBound(0)
                                        Application.DoEvents()
                                        .MarkingChar(iLp) = FormMarking(8 + iLp)
                                    Next
                                Else
                                    Dim errMsg As String = String.Empty

                                    Try
                                        errMsg = String.Format("Fail to form marking data for Lot No. : {0} due to...{1}!", .Lotdata(1).Lot_No, FormMarking(0))
                                    Catch ex As Exception
                                        errMsg = String.Format("Fail to form marking data for Lot No. : {0}...!", .Lotdata(1).Lot_No)
                                    End Try

                                    MessageBox.Show(errMsg, "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Error)

                                    With .Lotdata(1)
                                        .Lot_No = ""
                                        .IMI_No = ""
                                        .MData1 = ""
                                        .MData2 = ""
                                    End With

                                    frm_Main.RecStatus.Text = "Error..."
                                End If
                            Else
                                frm_Main.RecStatus.Text = "Custom Marking..."
                                Dim _ret As Integer = CPSG_MarkingData(.Lotdata(1))

                                With .Lotdata(1)
                                    '.Lot_No = .Lot_No.ToUpper
                                    '.IMI_No = "FA2000az"
                                    '.FreqVal = "00.000000"
                                    .Opt = .Opt.ToUpper
                                    .RecDate = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", Now.Month, Now.Day, Now.Year, Now.Hour, Now.Minute, Now.Second)
                                    '.Profile = "CUSTMARK"
                                    .CtrlNo = ActiveProc.CtrlNo
                                    .MacNo = "-"
                                    '.MData1 = "."
                                    '.MData2 = "."
                                    .MData3 = "-"
                                    .MData4 = "-"
                                    .MData5 = "-"
                                    .MData6 = "-"
                                End With

                                'For iLp As Integer = 0 To .MarkingChar.GetUpperBound(0)
                                '    Application.DoEvents()
                                '    .MarkingChar(iLp) = FormMarking(8 + iLp)
                                'Next

                                If _ret = 0 Then
                                    .MarkingChar(0) = .Lotdata(1).MData1
                                    .MarkingChar(1) = .Lotdata(1).MData2
                                Else
                                    .Lotdata(1).Lot_No = ""
                                    .Lotdata(1).IMI_No = ""

                                    If _ret > 0 Then
                                        MessageBox.Show("Unable to Connect to CPSG database.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Else
                                        MessageBox.Show("Unable to validate marking code in CPSG database.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End If
                                End If
                            End If

                        Catch ex As Exception
                            .Lotdata(1).Lot_No = ""
                            .Lotdata(1).IMI_No = ""
                        End Try

                        Me.Close()
                End Select
            End With
        End If

    End Sub

    Private Sub frm_DataEntry_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        fg_Load = 0

        With Me
            .txt_Scan.Focus()
        End With

    End Sub

End Class