Imports System
Imports System.Globalization
Imports System.Math
Imports System.Threading
Imports System.IO
Imports System.IO.Ports
Imports System.Management
Imports System.Runtime.InteropServices
Imports System.Data.Odbc
Imports Microsoft.Win32


Module mdl_odbc

    Public Function CreateDBConnection(ByVal dbFilePath As String) As OdbcConnection

        ' Connect string.
        Dim sConnStr As String = _
            "driver=Microsoft Access Driver (*.mdb); uid=admin; UserCommitSync=Yes; " & _
                    "Threads=3; SafeTransactions=0; PageTimeout=5; MaxScanRows=8; MaxBufferSize=2048; FIL=MS Access; DriverId=25; " & _
                    "DefaultDir=" & dbFilePath.Substring(0, dbFilePath.LastIndexOf("\")) & "; " & _
                    "DBQ=" & dbFilePath

        Try
            ' Open Connection.
            Dim oConn As New OdbcConnection(sConnStr)
            oConn.Open()
            ' Return Object.
            Return oConn
        Catch ex As Exception

        End Try

        Return Nothing

    End Function

    Public Function Update_odbcDB_Setting(ByVal SQLcmd As String) As Integer

        Dim dbPath As String = String.Empty
        Dim ch As Char = ChrW(39)
        Dim lRetVal As Long = 0


        With ActiveProc
            If .DataBase_.Server.ToLower.Trim = "local" Then
                dbPath = My.Application.Info.DirectoryPath & "\" & .DataBase_.Name
            Else
                dbPath = .DataBase_.Server & "\" & .DataBase_.Name
            End If
        End With

        Dim oConn As OdbcConnection = CreateDBConnection(dbPath)


        If IsNothing(oConn) Then
            Return -1
        End If

        Dim OdbcCmd As New OdbcCommand(SQLcmd, oConn)
        Dim oDBReader As OdbcDataReader = OdbcCmd.ExecuteReader()

        With oDBReader
            lRetVal = .RecordsAffected
        End With

        oConn.Close()
        oConn.Dispose()

        Return lRetVal

    End Function

    Public Function Read_odbcDB_Setting(ByVal SQLcmd As String, Optional ByRef SettingCondition() As ParameterProfile = Nothing) As Long

        Dim dbPath As String = String.Empty
        Dim ch As Char = ChrW(39)
        Dim lRetVal As Long = 0


        With ActiveProc
            If .DataBase_.Server.ToLower.Trim = "local" Then
                dbPath = My.Application.Info.DirectoryPath & "\" & .DataBase_.Name
            Else
                dbPath = .DataBase_.Server & "\" & .DataBase_.Name
            End If
        End With

        Dim oConn As OdbcConnection = CreateDBConnection(dbPath)


        If IsNothing(oConn) Then
            Return -1
        End If

        Dim OdbcCmd As New OdbcCommand(SQLcmd, oConn)
        Dim oDBReader As OdbcDataReader = OdbcCmd.ExecuteReader()

        With oDBReader
            Dim iFieldCnt As Integer = .FieldCount
            Dim iRecNo As Integer = 0

            If .HasRows Then
                Dim sRetData(iFieldCnt - 1) As String
                ReDim SettingCondition(iRecNo)

                Do While .Read()
                    Application.DoEvents()

                    ReDim Preserve SettingCondition(iRecNo)
                    ReDim SettingCondition(iRecNo).ParamData(5)

                    With SettingCondition(iRecNo)
                        .Spec = oDBReader.GetString(1)

                        With .SettingCond
                            .A_Layout = oDBReader.GetString(2)
                            .B_Xoffset = oDBReader.GetString(3)
                            .C_Yoffset = oDBReader.GetString(4)
                            .D_Rotation = oDBReader.GetString(5)
                            .E_Current = oDBReader.GetString(6)
                            .F_QSW = oDBReader.GetString(7)
                            .G_Speed = oDBReader.GetString(8)
                        End With

                        Dim StartLine As String = oDBReader.GetString(9)
                        .StartNo = StartLine

                        For iLp As Integer = 0 To .ParamData.GetUpperBound(0)
                            Application.DoEvents()
                            .ParamData(iLp).LineNo = (Val(StartLine) + iLp).ToString.Trim
                            .ParamData(iLp).SettingString = oDBReader.GetString(10 + iLp)
                        Next

                        .UseDot = oDBReader.GetString(16)
                        .UseBlock = oDBReader.GetString(17)
                    End With

                    iRecNo += 1
                Loop

                lRetVal = iRecNo - 1
            Else
                lRetVal = -1
            End If
        End With

        oConn.Close()
        oConn.Dispose()

        Return lRetVal

    End Function

    Public Function InsertNewProfile_odbc(ByVal NewProfileData As ParameterProfile) As Integer

        Dim dbPath As String = String.Empty
        Dim ch As Char = ChrW(39)
        Dim lRetVal As Long = 0


        With ActiveProc
            If .DataBase_.Server.ToLower.Trim = "local" Then
                dbPath = My.Application.Info.DirectoryPath & "\" & .DataBase_.Name
            Else
                dbPath = .DataBase_.Server & "\" & .DataBase_.Name
            End If
        End With

        Dim oConn As OdbcConnection = CreateDBConnection(dbPath)


        If IsNothing(oConn) Then
            Return -1
        End If


        Dim strSQL As String = String.Empty

        With NewProfileData
            strSQL = "INSERT INTO Setting " & _
                "(CTRLNo, Spec, LayoutNo, Xoffset, Yoffset, Rotate, [Current], QSW, Speed, StartLine, Block1, Block2, Block3, Block4, Block5, Block6, UseDot, UseBlock) VALUES(" & _
                ch & ActiveProc.CtrlNo & ch & ", " & _
                ch & .Spec & ch & ", " & _
                ch & .SettingCond.A_Layout & ch & ", " & _
                ch & .SettingCond.B_Xoffset & ch & ", " & _
                ch & .SettingCond.C_Yoffset & ch & ", " & _
                ch & .SettingCond.D_Rotation & ch & ", " & _
                ch & .SettingCond.E_Current & ch & ", " & _
                ch & .SettingCond.F_QSW & ch & ", " & _
                ch & .SettingCond.G_Speed & ch & ", " & _
                ch & .StartNo & ch & ", " & _
                ch & .ParamData(0).SettingString & ch & ", " & _
                ch & .ParamData(1).SettingString & ch & ", " & _
                ch & .ParamData(2).SettingString & ch & ", " & _
                ch & .ParamData(3).SettingString & ch & ", " & _
                ch & .ParamData(4).SettingString & ch & ", " & _
                ch & .ParamData(5).SettingString & ch & ", " & _
                ch & .UseDot & ch & ", " & _
                ch & .UseBlock & ch & ")"
        End With

        Dim OdbcCmd As New OdbcCommand(strSQL, oConn)
        Dim oDBReader As OdbcDataReader = OdbcCmd.ExecuteReader()

        With oDBReader
            lRetVal = .RecordsAffected
        End With

        oConn.Close()
        oConn.Dispose()

        Return lRetVal

    End Function

    Public Function InsertNewRecord_odbc(ByVal NewRecData As Rec) As Integer

        Dim dbPath As String = String.Empty
        Dim ch As Char = ChrW(39)
        Dim lRetVal As Long = 0


        With ActiveProc
            If .DataBase_.Server.ToLower.Trim = "local" Then
                dbPath = My.Application.Info.DirectoryPath & "\" & .DataBase_.Name
            Else
                dbPath = .DataBase_.Server & "\" & .DataBase_.Name
            End If
        End With

        Dim oConn As OdbcConnection = CreateDBConnection(dbPath)


        If IsNothing(oConn) Then
            Return -1
        End If


        Dim strSQL As String = String.Empty

        '--- Add to insert dummy data ---
        'With Records
        '    .Lot_No = "PA6-TEST1"
        '    .IMI_No = "D0110001"
        '    .FreqVal = "20.00"
        '    .Opt = "S1609"
        '    .RecDate = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", Now.Month, Now.Day, Now.Year, Now.Hour, Now.Minute, Now.Second)
        '    .Profile = "TSX"
        '    .CtrlNo = "M00000"
        '    .MacNo = "0"
        '    .MData1 = "5888"
        '    .MData2 = "Tymdd"
        '    .MData3 = "-"
        '    .MData4 = "-"
        '    .MData5 = "-"
        '    .MData6 = "-"
        'End With

        'FuncRet = InsertNewProfile_sql(Records)

        With NewRecData
            strSQL = "INSERT INTO Records " & _
                "(Lot_No, IMI_No, FreqVal, Opt, RecDate, [Profile], CtrlNo, MacNo, MData1, MData2, MData3, MData4, MData5, MData6) VALUES(" & _
                ch & .Lot_No & ch & ", " & _
                ch & .IMI_No & ch & ", " & _
                ch & .FreqVal & ch & ", " & _
                ch & .Opt & ch & ", " & _
                ch & .RecDate & ch & ", " & _
                ch & .Profile & ch & ", " & _
                ch & .CtrlNo & ch & ", " & _
                ch & .MacNo & ch & ", " & _
                ch & .MData1 & ch & ", " & _
                ch & .MData2 & ch & ", " & _
                ch & .MData3 & ch & ", " & _
                ch & .MData4 & ch & ", " & _
                ch & .MData5 & ch & ", " & _
                ch & .MData6 & ch & ")"
        End With

        Dim OdbcCmd As New OdbcCommand(strSQL, oConn)
        Dim oDBReader As OdbcDataReader = OdbcCmd.ExecuteReader()

        With oDBReader
            lRetVal = .RecordsAffected
        End With

        oConn.Close()
        oConn.Dispose()

        Return lRetVal

    End Function

End Module
