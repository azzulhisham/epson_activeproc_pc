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
                              "Please Enter Emp. No. ...", _
                              "Kindly Wait For A Moment..."}
    Dim BlinkLbl() As String = {"Lot No.", "IMI No.", "Emp. No.", ""}


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
            .SeqNo = 0

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

    Private Sub DispMsg()

        With Me
            .lbl_Msg.Text = MsgLbl(SeqNo)
            .lbl_Msg.Refresh()

            .lbl_Label.Text = .BlinkLbl(SeqNo)
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

    Private Sub txt_Scan_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt_Scan.KeyDown

        Static CustMarking As Integer


        If e.KeyCode = Keys.Escape Then
            With Me
                .Close()
            End With
        ElseIf e.KeyCode = Keys.Enter Then
            With ActiveProc
                Select Case SeqNo
                    Case Is = 0
                        .Lotdata(1).Lot_No = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""

                        If .Lotdata(1).Lot_No.Length = 10 AndAlso .Lotdata(1).Lot_No.ToUpper.StartsWith("V") Then
                            SeqNo += 2
                            CustMarking = 1
                        Else
                            SeqNo += 1
                            CustMarking = 0
                        End If

                        DispMsg()
                    Case Is = 1
                        .Lotdata(1).IMI_No = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""
                        SeqNo += 1
                        DispMsg()
                    Case Is = 2
                        .Lotdata(1).Opt = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""
                        SeqNo += 1
                        DispMsg()

                        Dim FormMarking() As String = {}
                        Dim FuncRet As Integer = 0

                        Try
                            If CustMarking = 0 Then

#If UseWebServices = 1 Then
                                FuncRet = azWebService.GetMarkingCode(.Lotdata(1).Lot_No, .Lotdata(1).IMI_No, FormMarking)
#Else
                                FuncRet = azLMServices.GetMarkingCode(.Lotdata(1).Lot_No, .Lotdata(1).IMI_No, FormMarking)
#End If

                                ReDim .MarkingChar(5)

                                If Not FuncRet < 0 Then
                                    If FuncRet = 1 Then
                                        frm_Main.RecStatus.Text = "Previous Records..."
                                    Else
                                        frm_Main.RecStatus.Text = "New Marking Code..."
                                    End If

                                    With .Lotdata(1)
                                        .Lot_No = FormMarking(0)
                                        .IMI_No = FormMarking(1)
                                        .FreqVal = FormMarking(2)
                                        .Opt = IIf(FormMarking(3) = "", ActiveProc.Lotdata(1).Opt, FormMarking(3))
                                        .RecDate = FormMarking(4)
                                        .Profile = FormMarking(5)
                                        .CtrlNo = IIf(FormMarking(6) = "", ActiveProc.CtrlNo, FormMarking(6))
                                        .MacNo = IIf(FormMarking(7) = "", "-", FormMarking(7))
                                        .MData1 = FormMarking(8)
                                        .MData2 = FormMarking(9)
                                        .MData3 = FormMarking(10)
                                        .MData4 = FormMarking(11)
                                        .MData5 = FormMarking(12)
                                        .MData6 = FormMarking(13)
                                    End With

                                    For iLp As Integer = 0 To .MarkingChar.GetUpperBound(0)
                                        Application.DoEvents()
                                        .MarkingChar(iLp) = FormMarking(8 + iLp)
                                    Next
                                Else
                                    MessageBox.Show("Fail to form marking code for the spec. : " & .Lotdata(1).IMI_No, "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Error)

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

                                With .Lotdata(1)
                                    .Lot_No = .Lot_No.ToUpper
                                    .IMI_No = "FA2000az"
                                    .FreqVal = "00.000000"
                                    .Opt = .Opt.ToUpper
                                    .RecDate = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", Now.Month, Now.Day, Now.Year, Now.Hour, Now.Minute, Now.Second)
                                    .Profile = "CUSTMARK"
                                    .CtrlNo = ActiveProc.CtrlNo
                                    .MacNo = "-"
                                    .MData1 = "."
                                    .MData2 = "."
                                    .MData3 = ""
                                    .MData4 = ""
                                    .MData5 = ""
                                    .MData6 = ""
                                End With

                                For iLp As Integer = 0 To .MarkingChar.GetUpperBound(0)
                                    Application.DoEvents()
                                    .MarkingChar(iLp) = FormMarking(8 + iLp)
                                Next
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