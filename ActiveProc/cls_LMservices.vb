Imports System.Globalization
Imports System.ComponentModel
Imports System.Management
Imports System.Runtime.InteropServices
Imports System.Data.SqlClient
Imports System.Data.Odbc
Imports Microsoft.Win32


Public Class cls_LMservices

    Public Structure SpecItem
        Public sFreq As String
        Public sPlant As String
        Public sProdCode As String
        Public sVersion As String
        Public sWkCdFormat As String
        Public sParameter As String
        Public sFormat As String
    End Structure

    Public Structure Rec
        Public Lot_No As String
        Public IMI_No As String
        Public FreqVal As String
        Public Opt As String
        Public RecDate As String
        Public Profile As String
        Public CtrlNo As String
        Public MacNo As String
        Public MData1 As String
        Public MData2 As String
        Public MData3 As String
        Public MData4 As String
        Public MData5 As String
        Public MData6 As String
    End Structure

    Public IMIDataItem As SpecItem


    Public Function AboutMe() As String

        Return "This WebServices is designed by Zulhisham @2010."

    End Function

    Public Function UpdateRecords(ByVal MarkingRec() As String) As Integer

        Dim MarkRec As Rec = Nothing

        With MarkRec
            .Lot_No = MarkingRec(0)
            .IMI_No = MarkingRec(1)
            .FreqVal = MarkingRec(2)
            .Opt = MarkingRec(3)
            .RecDate = MarkingRec(4)
            .Profile = MarkingRec(5)
            .CtrlNo = MarkingRec(6)
            .MacNo = MarkingRec(7)
            .MData1 = MarkingRec(8)
            .MData2 = MarkingRec(9)
            .MData3 = MarkingRec(10)
            .MData4 = MarkingRec(11)
            .MData5 = MarkingRec(12)
            .MData6 = MarkingRec(13)
        End With

        Dim FuncRet As Integer = InsertNewProfile_sql(MarkRec)
        Return FuncRet

    End Function

    Public Function GetMarkingCode(ByVal Lot_No As String, ByVal MI_Spec As String, ByRef RetData() As String) As Integer

        'Dim IMI_Path As String = ActiveProc.PreviousVerPath & "\Data\IMI"
        Dim IMI_Path As String = My.Application.Info.DirectoryPath & "\Data\IMI"

        Dim Records As Rec = Nothing
        Dim MarkingData As String = String.Empty
        Dim TargetSpec As String = String.Empty
        Dim FuncRet As Integer = 0

        If Lot_No.IndexOf("-") < 0 Then
            ReDim RetData(0)
            RetData(0) = "Invalid Lot No. !"
            Return -1
        Else
            TargetSpec = Lot_No.Substring(0, Lot_No.IndexOf("-")).ToUpper.Trim

            If Not CheckDatabase() < 0 Then
#If UseSQL_Server = 1 Then
                FuncRet = GetSqlRecords(Lot_No, Records)
#Else
                FuncRet = GetOdbcRecords(Lot_No, Records)
#End If

                If FuncRet < 0 Then
                    ReDim RetData(0)
                    RetData(0) = "SQL error!"
                    Return FuncRet
                ElseIf FuncRet > 0 Then
                    ReDim RetData(13)

                    With Records
                        RetData(0) = .Lot_No
                        RetData(1) = .IMI_No
                        RetData(2) = .FreqVal
                        RetData(3) = .Opt
                        RetData(4) = .RecDate
                        RetData(5) = .Profile
                        RetData(6) = .CtrlNo
                        RetData(7) = .MacNo
                        RetData(8) = .MData1
                        RetData(9) = .MData2
                        RetData(10) = .MData3
                        RetData(11) = .MData4
                        RetData(12) = .MData5
                        RetData(13) = .MData6
                    End With

                    Return FuncRet
                End If

            End If
        End If

        If TargetSpec.StartsWith("P") Then
            '--- Testing Location -> Remark this line for runtime ---
            'IMI_Path = "D:\AI\MyVBProject\VB_Project\ML-7111A\PXFA\Data\IMI\ML-7111A"
            Dim IMIFilePath As String = IMI_Path & "\" & MI_Spec & ".dat"

            If My.Computer.FileSystem.FileExists(IMIFilePath) Then
                If ParseSpecData(IMIFilePath, Records) < 0 Then
                    ReDim RetData(0)
                    RetData(0) = "Parse Spec. File Error!"
                    Return -1
                Else
                    ReDim RetData(13)

                    With Records
                        RetData(0) = Lot_No
                        RetData(1) = MI_Spec
                        RetData(2) = .FreqVal
                        RetData(3) = ""
                        RetData(4) = ""
                        RetData(5) = .Profile
                        RetData(6) = ""
                        RetData(7) = ""
                        RetData(8) = FormMarkCode1()
                        RetData(9) = FormMarkCode2()
                        RetData(10) = "-"
                        RetData(11) = "-"
                        RetData(12) = "-"
                        RetData(13) = "-"
                    End With
                End If
            Else
                ReDim RetData(1)
                RetData(0) = "Spec. File Not Found!"
                RetData(1) = IMIFilePath
                Return -1
            End If
        Else
            ReDim RetData(2)
            MarkingData = "A" & FormWeekCode("yww") & "L"
            RetData(0) = Lot_No
            RetData(1) = MI_Spec
            RetData(2) = MarkingData
        End If

        Return RetData.GetUpperBound(0)

    End Function

    Private Function FormMarkCode1() As String

        Dim MarkData As String = String.Empty

        With IMIDataItem
            Try
                .sFreq = String.Format("{0:F3}", CType(.sFreq, Decimal))

                If .sVersion.Length > 2 Then
                    MarkData = .sVersion
                Else
                    If .sPlant.Length > 1 Then
                        Dim chByte() As Char = .sPlant.ToCharArray

                        For ilp As Integer = 0 To chByte.GetUpperBound(0)
                            If chByte(ilp) = "#" Then
                                chByte(ilp) = .sFreq.Replace(".", "").Substring(ilp, 1)
                            End If
                        Next

                        MarkData = chByte
                    Else
                        If Val(.sFreq) = 0 Then
                            MarkData = .sVersion
                        Else
                            MarkData = .sFreq.Replace(".", "").Substring(0, 5 - .sPlant.Length) & .sPlant
                        End If
                    End If
                End If

                If .sParameter.ToLower = "fa-12t" Then
                    MarkData = .sProdCode
                End If
            Catch ex As Exception
                MarkData = ""
            End Try
        End With

        Return MarkData

    End Function

    Private Function FormMarkCode2() As String

        Dim MarkData As String = String.Empty


        Try
            With IMIDataItem
                If .sVersion.Length > 2 Then
                    MarkData = .sProdCode & FormWeekCode(.sWkCdFormat)
                Else
                    If .sVersion = "_" Then
                        MarkData = .sPlant & FormWeekCode(.sWkCdFormat)
                    Else
                        MarkData = .sProdCode & FormWeekCode(.sWkCdFormat) & .sVersion
                    End If
                End If

                If .sParameter.ToLower = "fa-12t" Then
                    MarkData = FormWeekCode(.sWkCdFormat)
                End If
            End With
        Catch ex As Exception
            MarkData = ""
        End Try

        Return MarkData

    End Function

    Private Function ParseSpecData(ByVal SpecFile As String, ByRef ParseData As Rec) As Integer

        Dim FuncRet As Integer = 0
        Dim FileDataItem As String = My.Computer.FileSystem.ReadAllText(SpecFile)

        With IMIDataItem
            Try
                .sFreq = FileDataItem.Substring(FileDataItem.IndexOf("L001") + 5)
                .sFreq = .sFreq.Substring(0, .sFreq.IndexOf(vbCr)).Trim

                If .sFreq.Contains("-") Then
                    Return -1
                End If

                ParseData.FreqVal = .sFreq

                .sPlant = FileDataItem.Substring(FileDataItem.IndexOf("L002") + 5)
                .sPlant = .sPlant.Substring(0, .sPlant.IndexOf(vbCr)).Trim

                .sProdCode = FileDataItem.Substring(FileDataItem.IndexOf("L003") + 5)
                .sProdCode = .sProdCode.Substring(0, .sProdCode.IndexOf(vbCr)).Trim

                .sVersion = FileDataItem.Substring(FileDataItem.IndexOf("L004") + 5)
                .sVersion = .sVersion.Substring(0, .sVersion.IndexOf(vbCr)).Trim

                .sWkCdFormat = FileDataItem.Substring(FileDataItem.IndexOf("L005") + 5)
                .sWkCdFormat = .sWkCdFormat.Substring(0, .sWkCdFormat.IndexOf(vbCr)).Trim

                .sParameter = FileDataItem.Substring(FileDataItem.IndexOf("L006") + 5)
                .sParameter = .sParameter.Substring(0, .sParameter.IndexOf(vbCr)).Trim

                If .sParameter.ToLower.Contains("tci_f") Then
                    Return -1
                End If

                .sFormat = FileDataItem.Substring(FileDataItem.IndexOf("L007") + 5).Trim
                ParseData.Profile = .sParameter & "," & .sFormat
            Catch ex As Exception
                FuncRet = -1
            End Try
        End With

        Return FuncRet

    End Function

    Private Function FormWeekCode(Optional ByVal strFormat As String = "ymd") As String

        Dim m_Format As String = strFormat.ToLower.Trim
        Dim m_WkCode As String = String.Empty
        Dim m_Today As Date = Today

        Dim Month_D As String = "123456789XYZ"
        Dim Day_D As String = "123456789ABCDEFGHJKLMNOPQRSTUVW"
        Dim WkNoCd As String = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ"

        Dim myCI As New CultureInfo("en-US")
        Dim myCal As Calendar = myCI.Calendar


        Select Case m_Format
            Case Is = "ymd"
                m_WkCode = String.Format("{0:D2}", m_Today.Year)
                m_WkCode = m_WkCode.Substring(m_WkCode.Length - 1) & Month_D.Substring(m_Today.Month - 1, 1) & Day_D.Substring(m_Today.Day - 1, 1)
            Case Is = "ymdd"
                m_WkCode = String.Format("{0:D2}", m_Today.Year)
                m_WkCode = m_WkCode.Substring(m_WkCode.Length - 1) & Month_D.Substring(m_Today.Month - 1, 1) & String.Format("{0:D2}", m_Today.Day)
            Case Is = "yww"
                m_WkCode = String.Format("{0:D2}", m_Today.Year)
                m_WkCode = m_WkCode.Substring(m_WkCode.Length - 1) & String.Format("{0:D2}", myCal.GetWeekOfYear(m_Today, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
            Case Is = "ww"
                Dim YearStart As Integer = 2010
                Dim StartCode As String = "98"

                Dim DiffYrs As Integer = m_Today.Year - Val(YearStart)
                m_WkCode = String.Format("{0:D2}", myCal.GetWeekOfYear(m_Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday))

                Do Until DiffYrs = 0
                    Application.DoEvents()
                    Dim prvYrsWeekNo As Integer = myCal.GetWeekOfYear(myCal.AddDays(m_Today, myCal.GetDayOfYear(m_Today) * -1), CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday)

                    Dim NextWkNoCd As String = WkNoCd.Substring(WkNoCd.IndexOf(StartCode.Substring(StartCode.Length - (StartCode.Length - 1))))
                    Dim Next_WkCode As String = String.Empty

                    If NextWkNoCd.Length >= Val(prvYrsWeekNo) Then
                        Next_WkCode = NextWkNoCd.Substring(Val(prvYrsWeekNo), 1)
                        Next_WkCode = StartCode.Substring(0, StartCode.Length - 1) & Next_WkCode
                    Else
                        Dim WkNoCd_Diff As Integer = Val(prvYrsWeekNo) - NextWkNoCd.Length
                        Dim WkNoCdMajor As Integer = WkNoCd.IndexOf(StartCode.Substring(0, StartCode.Length - 1)) + ((WkNoCd_Diff \ 53) + 1)
                        WkNoCdMajor += WkNoCd_Diff \ WkNoCd.Length

                        Next_WkCode = WkNoCd.Substring(WkNoCdMajor, 1) & WkNoCd.Substring((WkNoCd_Diff Mod WkNoCd.Length), 1)
                    End If

                    YearStart = m_Today.Year
                    StartCode = Next_WkCode

                    If m_WkCode >= prvYrsWeekNo And myCal.GetWeekOfYear(m_Today, CalendarWeekRule.FirstDay, DayOfWeek.Monday) = 1 Then
                        m_WkCode = 0
                    End If

                    DiffYrs -= 1
                Loop


                Dim TrimWkNoCd As String = WkNoCd.Substring(WkNoCd.IndexOf(StartCode.Substring(StartCode.Length - (StartCode.Length - 1))))
                Dim tmp_WkCode As String = String.Empty

                If TrimWkNoCd.Length >= Val(m_WkCode) Then
                    tmp_WkCode = TrimWkNoCd.Substring(Val(m_WkCode) - 1, 1)
                    tmp_WkCode = StartCode.Substring(0, StartCode.Length - 1) & tmp_WkCode
                Else
                    Dim WkNoCd_Diff As Integer = Val(m_WkCode) - TrimWkNoCd.Length
                    Dim WkNoCdMajor As Integer = WkNoCd.IndexOf(StartCode.Substring(0, StartCode.Length - 1)) + ((WkNoCd_Diff \ 53) + 1)
                    WkNoCdMajor += WkNoCd_Diff \ WkNoCd.Length

                    tmp_WkCode = WkNoCd.Substring(WkNoCdMajor, 1) & WkNoCd.Substring((WkNoCd_Diff Mod WkNoCd.Length) - 1, 1)
                End If

                m_WkCode = tmp_WkCode
        End Select

        Return m_WkCode

    End Function

    Private Function CheckDatabase() As Integer

        Dim RetVal As Integer = 0


#If UseSQL_Server = 1 Then
        Dim sConnStr As String = _
            "SERVER=" & ActiveProc.DataBase_.Server & "; " & _
            "DataBase=" & "; " & _
            "uid=" & ActiveProc.DataBase_.uid & ";" & _
            "pwd=" & ActiveProc.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "IF NOT EXISTS (SELECT * FROM Sys.DATABASES WHERE Name='" & _
            ActiveProc.DataBase_.Name & "') " & _
            "CREATE DATABASE [" & ActiveProc.DataBase_.Name & "]"

            Try
                dbConnection.Open()

                Dim cmd As New SqlCommand(strSQL, dbConnection)
                cmd.ExecuteNonQuery()
            Catch sqlExc As SqlException
                RetVal = -1
            End Try

            dbConnection.Close()
#Else
        With ActiveProc
            If .DataBase_.Server.ToLower.Trim = "local" Then
                If Not My.Computer.FileSystem.FileExists(My.Application.Info.DirectoryPath & "\" & .DataBase_.Name) Then
                    RetVal = -1
                End If
            Else
                If Not My.Computer.FileSystem.FileExists(.DataBase_.Server & "\" & .DataBase_.Name) Then
                    RetVal = -1
                End If
            End If
        End With
#End If

        Return RetVal

    End Function

    Private Function GetSqlRecords(ByVal Lot_No As String, ByRef RecData As Rec) As Integer

        Dim CreateTblString As String = String.Empty


        CreateTblString = "[Lot_No] [nvarchar](20) NOT NULL CONSTRAINT [DF_Records_Lot_No]  DEFAULT (N'-')," & _
                        "[IMI_No] [nvarchar](20) NOT NULL CONSTRAINT [DF_Records_IMI_No]  DEFAULT (N'-')," & _
                        "[FreqVal] [nvarchar](16) NOT NULL CONSTRAINT [DF_Records_FreqVal]  DEFAULT (N'-')," & _
                        "[Opt] [nvarchar](8) NOT NULL CONSTRAINT [DF_Records_Opt]  DEFAULT (N'-')," & _
                        "[RecDate] [datetime] NOT NULL," & _
                        "[Profile] [nvarchar](12) NOT NULL CONSTRAINT [DF_Records_Profile]  DEFAULT (N'-')," & _
                        "[CtrlNo] [nvarchar](12) NOT NULL CONSTRAINT [DF_Records_CtrlNo]  DEFAULT (N'-')," & _
                        "[MacNo] [nvarchar](2) NOT NULL CONSTRAINT [DF_Records_MacNo]  DEFAULT (N'-')," & _
                        "[MData1] [nvarchar](8) NOT NULL CONSTRAINT [DF_Records_MData1]  DEFAULT (N'-')," & _
                        "[MData2] [nvarchar](8) NOT NULL CONSTRAINT [DF_Records_MData2]  DEFAULT (N'-')," & _
                        "[MData3] [nvarchar](8) NOT NULL CONSTRAINT [DF_Records_MData3]  DEFAULT (N'-')," & _
                        "[MData4] [nvarchar](8) NOT NULL CONSTRAINT [DF_Records_MData4]  DEFAULT (N'-')," & _
                        "[MData5] [nvarchar](8) NOT NULL CONSTRAINT [DF_Records_MData5]  DEFAULT (N'-')," & _
                        "[MData6] [nvarchar](8) NOT NULL CONSTRAINT [DF_Records_MData6]  DEFAULT (N'-')"

        If Not Check_dboTables("Records", CreateTblString) < 0 Then
            Return GetRecordsFromServer(Lot_No, RecData)
        Else
            Return -1
        End If

    End Function

    Private Function GetOdbcRecords(ByVal Lot_No As String, ByRef RecData As Rec) As Integer

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


        Dim SQLcmd As String = _
            "SELECT * FROM Records WHERE Lot_No='" & Lot_No & "' " & _
            "ORDER BY Lot_No"

        Dim OdbcCmd As New OdbcCommand(SQLcmd, oConn)
        Dim oDBReader As OdbcDataReader = OdbcCmd.ExecuteReader()

        With oDBReader
            Dim iFieldCnt As Integer = .FieldCount
            Dim iRecNo As Integer = 0

            If .HasRows Then
                Dim sRetData(iFieldCnt - 1) As String

                Do While .Read()
                    With RecData
                        .Lot_No = oDBReader.GetString(0)
                        .IMI_No = oDBReader.GetString(1)
                        .FreqVal = oDBReader.GetString(2)
                        .Opt = oDBReader.GetString(3)
                        .RecDate = oDBReader.GetDateTime(4).ToString
                        .Profile = oDBReader.GetString(5)
                        .CtrlNo = oDBReader.GetString(6)
                        .MacNo = oDBReader.GetString(7)
                        .MData1 = oDBReader.GetString(8)
                        .MData2 = oDBReader.GetString(9)
                        .MData3 = oDBReader.GetString(10)
                        .MData4 = oDBReader.GetString(11)
                        .MData5 = oDBReader.GetString(12)
                        .MData6 = oDBReader.GetString(13)
                    End With

                    iRecNo += 1
                Loop

                lRetVal = iRecNo
            Else
                lRetVal = 0
            End If
        End With

        Return lRetVal

    End Function

    Private Function GetRecordsFromServer(ByVal Lot_No As String, ByRef RecData As Rec) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String = _
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " & _
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " & _
                    "uid=VB-SQL;" & _
                    "pwd=Anyn0m0us"
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "SELECT * FROM Records WHERE Lot_No='" & Lot_No & "' " & _
            "ORDER BY Lot_No"

        Try
            dbConnection.Open()

            Dim cmd As New SqlCommand(strSQL, dbConnection)
            cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()

            With sqlReader
                Dim iFieldCnt As Integer = .FieldCount
                Dim iRecNo As Integer = 0

                If .HasRows Then
                    Dim sRetData(iFieldCnt - 1) As String

                    Do While .Read()
                        With RecData
                            .Lot_No = sqlReader.GetString(0)
                            .IMI_No = sqlReader.GetString(1)
                            .FreqVal = sqlReader.GetString(2)
                            .Opt = sqlReader.GetString(3)
                            .RecDate = sqlReader.GetDateTime(4).ToString
                            .Profile = sqlReader.GetString(5)
                            .CtrlNo = sqlReader.GetString(6)
                            .MacNo = sqlReader.GetString(7)
                            .MData1 = sqlReader.GetString(8)
                            .MData2 = sqlReader.GetString(9)
                            .MData3 = sqlReader.GetString(10)
                            .MData4 = sqlReader.GetString(11)
                            .MData5 = sqlReader.GetString(12)
                            .MData6 = sqlReader.GetString(13)
                        End With

                        iRecNo += 1
                    Loop

                    RetVal = iRecNo
                Else
                    RetVal = 0
                End If
            End With
        Catch sqlExc As SqlException
            RetVal = 0
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Private Function Check_dboTables(ByVal TableName As String, ByVal CreateTblStr As String) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String = _
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " & _
                    "DataBase=" & "; " & _
                    "uid=VB-SQL;" & _
                    "pwd=Anyn0m0us"
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "USE [" & ActiveProc.DataBase_.Name & "]" & vbCrLf & _
            "IF NOT EXISTS (SELECT * FROM sys.objects " & _
            "WHERE object_id=OBJECT_ID(N'[dbo].[" & TableName & "]') AND type in (N'U')) " & _
            "CREATE Table [" & TableName & "] (" & _
            CreateTblStr & ")"

        Try
            dbConnection.Open()

            Dim cmd As New SqlCommand(strSQL, dbConnection)
            cmd.ExecuteNonQuery()
        Catch sqlExc As SqlException
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Private Function InsertNewProfile_sql(ByVal NewRecData As Rec) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String = _
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " & _
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " & _
                    "uid=VB-SQL;" & _
                    "pwd=Anyn0m0us"
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
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

        Try
            dbConnection.Open()

            Dim cmd As New SqlCommand(strSQL, dbConnection)
            'cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()
            RetVal = sqlReader.RecordsAffected
        Catch sqlExc As SqlException
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

End Class
