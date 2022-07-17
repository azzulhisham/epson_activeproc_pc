Public Class frm_Support

    Dim fg_Load As Integer = 1
    Dim SeqNo As Integer = 0
    Dim MsgLbl() As String = {"Please Machine Control No. (M00000)...", _
                              "Kindly Wait For A Moment..."}


    Private Sub frm_Support_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        With Me
            .fg_Load = 1
            .SeqNo = 0

            With .txt_KeyIn
                .Text = ""
                .Visible = True
            End With

            .lbl_Progress.Visible = False
            DispMsg()
        End With

    End Sub

    Private Sub DispMsg()

        With Me
            .lbl_MsgBox.Text = MsgLbl(SeqNo)

            If SeqNo = .MsgLbl.GetUpperBound(0) Then
                .txt_KeyIn.Visible = False
                .lbl_Progress.Visible = True
            Else
                .lbl_Progress.Visible = False
                .txt_KeyIn.Visible = True
            End If
        End With

    End Sub

    Private Sub frm_Support_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        With Me
            .fg_Load = 0

            With .txt_KeyIn
                .Text = ""
                .Focus()
            End With
        End With

    End Sub

    Private Sub txt_KeyIn_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt_KeyIn.KeyDown

        With ActiveProc
            If e.KeyCode = Keys.Escape Then
                With Me
                    .Close()
                End With
            ElseIf e.KeyCode = Keys.Enter Then
                Dim CtrlNoVal As String = Me.txt_KeyIn.Text.ToUpper.Trim

                If CtrlNoVal.StartsWith("M") And CtrlNoVal.Length >= 6 Then
                    .CtrlNo = CtrlNoVal
                    Me.txt_KeyIn.Text = ""
                    SeqNo += 1
                    DispMsg()

                    Me.lbl_Progress.Text = "Insert Default Data..."

                    Dim GetOldPmf As Integer = 0
                    Dim SQLcmd As String = _
                                "SELECT * FROM Setting WHERE CtrlNo='" & ActiveProc.CtrlNo & "' " & _
                                "ORDER BY Spec"

#If UseSQL_Server = 1 Then
                    If GetProfileDetailsFromServer(SQLcmd) < 0 Then
                        GetOldPmf = 1
                        InsertNewProfile_sql(DefaultProfile)
                    End If
#Else
                    If Read_odbcDB_Setting(SQLcmd) < 0 Then
                        GetOldPmf=1
                        InsertNewProfile_odbc(DefaultProfile)
                    End If
#End If


                    If GetOldPmf = 1 Then
                        Me.lbl_Progress.Text = "Retrieving old parameter data..."
                        Dim FuncRet As Integer = RetrieveOldPMF()

                        If Not FuncRet < 0 Then
                            Me.lbl_Progress.Text = "Backing Up IMI Files..."

                            If Not BackUpIMI() < 0 Then
                                MessageBox.Show("Backup parameter data and IMI files completed successfully.", "Back Up Successfull...", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                regSubKey.SetValue("CtrlNo", .CtrlNo)
                            End If

                            Me.Close()
                        Else
                            With Me.txt_KeyIn
                                .Text = ""
                                .Focus()
                            End With

                            SeqNo = 0
                            DispMsg()
                        End If
                    Else
                        regSubKey.SetValue("CtrlNo", .CtrlNo)
                        Me.Close()
                    End If
                Else
                    MessageBox.Show("The control no. is invalid. It should have the following format (M00000).", "Invalid Control No.", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        End With

    End Sub

    Private Function BackUpIMI() As Integer

        If Not My.Computer.FileSystem.DirectoryExists(My.Application.Info.DirectoryPath & "\Data\IMI") Then
            My.Computer.FileSystem.CreateDirectory(My.Application.Info.DirectoryPath & "\Data\IMI")
        End If


        Dim imiFiles() As String = {}
        Dim FuncRet As Integer = 0

        If Not GetFilesList(ActiveProc.PreviousVerPath & "\Data\IMI", ".dat", imiFiles) < 0 Then
            For iLp As Integer = 0 To imiFiles.GetUpperBound(0)
                Application.DoEvents()
                My.Computer.FileSystem.CopyFile(imiFiles(iLp), My.Application.Info.DirectoryPath & "\Data\IMI" & imiFiles(iLp).Substring(imiFiles(iLp).LastIndexOf("\")), True)
            Next
        Else
            FuncRet = -1
            MessageBox.Show("Unabled to locate IMI files. Please refer to system engineer for this problem.", "IMI Files not found...", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        Return FuncRet

    End Function

    Private Function RetrieveOldPMF() As Integer

        Dim pmfFiles() As String = {"@"}
        Dim FuncRet As Integer = 0


        If Not GetFilesList(ActiveProc.PreviousVerPath & "\Parameter", ".pmf", pmfFiles) < 0 Then
            For iLp As Integer = 0 To pmfFiles.GetUpperBound(0)
                Application.DoEvents()

                If InsertPMFintoSQLdb(pmfFiles(iLp)) < 0 Then
                    MessageBox.Show("Unabled to read '" & pmfFiles(iLp) & "' . Please refer to system engineer for this problem.", "PMF Files Error...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    FuncRet = -1
                    Exit For
                End If
            Next
        Else
            FuncRet = -1
            MessageBox.Show("Unabled to locate PMF files. Please refer to system engineer for this problem.", "PMF Files not found...", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        Return FuncRet

    End Function

    Private Function InsertPMFintoSQLdb(Optional ByVal pmfFileName As String = "") As Integer

        Dim FuncRet As Integer = 0
        Dim OldParameterProfile As ParameterProfile
        Dim SpecName As String = pmfFileName.Substring(pmfFileName.LastIndexOf("\") + 1)
        SpecName = SpecName.Substring(0, SpecName.LastIndexOf(".pmf"))

        Dim ProfileKeyName As String = String.Empty
        Dim ProfileExt As String = ActiveProc.PreviousVerPath & "\Chg20050908.ini"


        With OldParameterProfile
            .Spec = SpecName
            Dim SQLcmd As String = _
                        "SELECT * FROM Setting WHERE CtrlNo='" & ActiveProc.CtrlNo & "' AND Spec='" & .Spec & "' " & _
                        "ORDER BY Spec"

#If UseSQL_Server = 1 Then
            If Not GetProfileDetailsFromServer(SQLcmd) < 0 Then
                Return FuncRet
            End If
#Else
            If Not Read_odbcDB_Setting(SQLcmd) < 0 Then
                Return FuncRet
            End If
#End If


            If .Spec.IndexOf("-") = 2 Then
                ProfileKeyName = .Spec.Replace("-", "")
            Else
                ProfileKeyName = .Spec.Trim
            End If

            .UseBlock = GetPrivateInfoString("LineMark", ProfileKeyName, ProfileExt)
            If .UseBlock = "" Then .UseBlock = "0"

            .UseDot = GetPrivateInfoString("Dot", ProfileKeyName, ProfileExt)
            If .UseDot = "" Then .UseDot = "0"

            .StartNo = 2

            With .SettingCond
                .A_Layout = GetPrivateInfoString("LayoutNo", ProfileKeyName, ProfileExt)
                If .A_Layout = "" Then .A_Layout = "1"

                .B_Xoffset = GetPrivateInfoString("MarkingCondition", "Xoffset", pmfFileName)
                If .B_Xoffset = "" Then
                    Dim Offset As String = String.Empty
                    Dim RetVal As Integer = frm_Main.GetOffsetValue("X", Offset)

                    If Offset = "" Then
                        .B_Xoffset = "0"
                    Else
                        .B_Xoffset = Offset
                    End If
                End If

                .C_Yoffset = GetPrivateInfoString("MarkingCondition", "Yoffset", pmfFileName)
                If .C_Yoffset = "" Then
                    Dim Offset As String = String.Empty
                    Dim RetVal As Integer = frm_Main.GetOffsetValue("Y", Offset)

                    If Offset = "" Then
                        .C_Yoffset = "0"
                    Else
                        .C_Yoffset = Offset
                    End If
                End If

                .D_Rotation = GetPrivateInfoString("MarkingCondition", "Rotation", pmfFileName)
                If .D_Rotation = "" Then .D_Rotation = "359900000"

                .E_Current = GetPrivateInfoString("MarkingCondition", "Current", pmfFileName)
                If .E_Current = "" Then .E_Current = "220"

                .F_QSW = GetPrivateInfoString("MarkingCondition", "QSW", pmfFileName)
                If .F_QSW = "" Then .F_QSW = "200"

                .G_Speed = GetPrivateInfoString("MarkingCondition", "Speed", pmfFileName)
                If .G_Speed = "" Then .G_Speed = "20000"
            End With


            ReDim .ParamData(5)
            Dim sSetting As String = String.Empty

            For iLP As Integer = 0 To .ParamData.GetUpperBound(0)
                Application.DoEvents()

                With .ParamData(iLP)
                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Orientation", pmfFileName)
                    .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "X_Axis", pmfFileName)
                    .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Y_Axis", pmfFileName)
                    .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Text Angle", pmfFileName)
                    If Val(sSetting) = 0 Or sSetting = "" Then .SettingString &= "0" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Text Align", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Text Width", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Space Align", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Space Width", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    .SettingString &= "" & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "X_Axis Org", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Y_Axis Org", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Char Height", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Compress", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Reverse", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Char Angle", pmfFileName)
                    If Val(sSetting) = 0 Or sSetting = "" Then .SettingString &= "0" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Current", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "QSW", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Speed", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Repeatation", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "Mirror", pmfFileName)
                    If Val(sSetting) = 0 Then .SettingString &= "" & "," Else .SettingString &= sSetting & ","

                    .SettingString &= "" & ","

                    If Not Val(OldParameterProfile.UseDot) = 0 Then
                        If iLP = 0 Then
                            .SettingString &= "2" & ","

                            sSetting = GetPrivateInfoString("MarkingDataLine" & (iLP + 1).ToString.Trim, "DefaultDotVal", pmfFileName)
                            .SettingString &= IIf(sSetting = "", "200001", sSetting)
                        Else
                            .SettingString &= "0" & ","
                            .SettingString &= "."
                        End If
                    Else
                        .SettingString &= "0" & ","
                        .SettingString &= "."
                    End If
                End With
            Next
        End With

#If UseSQL_Server = 1 Then
        FuncRet = InsertNewProfile_sql(OldParameterProfile)
#Else
        FuncRet = InsertNewProfile_odbc(OldParameterProfile)
#End If
        Return FuncRet

    End Function

End Class