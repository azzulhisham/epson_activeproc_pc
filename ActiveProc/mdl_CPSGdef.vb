Imports System.Data.SqlClient
Imports Microsoft.Win32


Module mdl_CPSGdef

    Public Structure tblCPSG
        Public Product As String
        Public Code As String
        Public Spec As String
        Public Block1 As String
        Public Block2 As String
        Public AppCode As String
        Public FuncCode As String
        Public ProdCode As String
        Public MarkFile As String
        Public Parameter As String
        Public Ref As String
        Public ExRef As String
    End Structure


    Public Function PopulateDB() As Integer

        Dim _fn As String = "D:\CPSG.dat"

        If Not My.Computer.FileSystem.FileExists(_fn) Then
            Return Func_Ret_Success
        End If

        Dim _fc As String = My.Computer.FileSystem.ReadAllText(_fn, System.Text.Encoding.ASCII)

        _fc = _fc.Replace(vbLf, "")
        Dim _allItems() As String = _fc.Split(vbCr)


        If _allItems.Count > 0 Then
            For Each _item As String In _allItems
                If Not _item.ToLower.Trim.StartsWith("product") And _item.Trim <> "" Then
                    Dim _dbData() As String = _item.Split(","c)
                    Dim _db As tblCPSG = Nothing

                    With _db
                        .Product = _dbData(0)
                        .Code = _dbData(1)
                        .Spec = _dbData(2)
                        .Block1 = _dbData(3)
                        .Block2 = _dbData(4)
                        .AppCode = _dbData(5)
                        .FuncCode = _dbData(6)
                        .ProdCode = _dbData(7)
                        .MarkFile = _dbData(8)
                        .Parameter = _dbData(9)
                        .Ref = _dbData(10)
                        .ExRef = "-"
                    End With

                    Dim _ret As Integer = InsertCPSG_pmf(_db)
                End If
            Next
        End If

        MessageBox.Show("CPSG Parameters Upload Completed!", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Return Func_Ret_Success

    End Function

    Public Function InsertCPSG_pmf(ByVal _dbVal As tblCPSG) As Integer

        Dim _ret As Integer = 0
        Dim sConnStr As String = _
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " & _
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " & _
                    "uid=VB-SQL;" & _
                    "pwd=Anyn0m0us"
        '"Integrated Security=SSPI"


        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = "INSERT INTO CPSG VALUES ('"

        With _dbVal
            strSQL &= .Product & "', '"
            strSQL &= .Code & "', '"
            strSQL &= .Spec & "', '"
            strSQL &= .Block1 & "', '"
            strSQL &= .Block2 & "', '"
            strSQL &= .AppCode & "', '"
            strSQL &= .FuncCode & "', '"
            strSQL &= .ProdCode & "', '"
            strSQL &= .MarkFile & "', '"
            strSQL &= .Parameter & "', '"
            strSQL &= .Ref & "', '"
            strSQL &= .ExRef & "')"
        End With


        Try
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()

            Dim Rec As Integer = sqlReader.RecordsAffected
            _ret = Rec
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            _ret = Func_Ret_Fail
        End Try


        dbConnection.Close()
        Return _ret

    End Function

    Public Function CPSG_MarkingData(ByRef _Detail As Rec) As Integer

        Dim _ProdType As String = _Detail.IMI_No.Substring(0, 4)
        Dim _Type As Integer = 0

        Select Case _ProdType.ToUpper
            Case Is = "310S"
                _Type = 4
            Case Is = "310P", "320P", "803C", "802C"
                _Type = 6
        End Select

        Dim _prod As String = _Detail.IMI_No.Substring(0, _Type)
        Dim _AppCode As String = _Detail.IMI_No.Substring(_Detail.IMI_No.IndexOf(" ")).Trim

        _AppCode = _AppCode.Substring(0, 2)

        Dim _CPSG() As tblCPSG = Nothing
        Dim _cmd As String = String.Empty


        Select Case _ProdType.ToUpper
            Case Is = "310S"
                _cmd = "SELECT * FROM CPSG WHERE Product LIKE '" & _prod & "%'"
            Case Is = "310P", "320P", "803C", "802C"
                _cmd = "SELECT * FROM CPSG WHERE Product = '" & _prod & "'"
        End Select



        Dim _ret As Integer = Get_CPSG_(_cmd, _CPSG)


        If _ret > 0 Then
            Dim CPSG_spec As System.Collections.Generic.IEnumerable(Of tblCPSG) = _CPSG.Where(Function(q) q.AppCode = _AppCode)

            If CPSG_spec.Count > 0 Then
                _ret = _MarkingData(CPSG_spec(0), _Detail)
            Else
                Dim CPSG_spec_ As System.Collections.Generic.IEnumerable(Of tblCPSG) = _CPSG.Where(Function(q) q.AppCode = "00")

                If CPSG_spec_.Count > 0 Then
                    _ret = _MarkingData(CPSG_spec_(0), _Detail)
                Else
                    _ret = Func_Ret_Fail
                End If
            End If
        Else
            _ret = Func_Ret_Fail
        End If

        Return _ret

    End Function

    Public Function _MarkingData(ByVal _tbl As tblCPSG, ByRef _Detail As Rec) As Integer

#If UseWebServices = 1 Then
        Dim azWebService As New zulhisham_pc.az_Services
#Else
    Dim azLMServices As New cls_LMservices
#End If

        If _tbl.Code.ToLower = "null" Then _tbl.Code = ""
        If _tbl.Spec.ToLower = "null" Then _tbl.Spec = ""
        If _tbl.Block1.ToLower = "null" Then _tbl.Block1 = ""
        If _tbl.Block2.ToLower = "null" Then _tbl.Block2 = ""
        If _tbl.FuncCode.ToLower = "null" Then _tbl.FuncCode = ""


        Dim _spec As String = _Detail.IMI_No.Substring(4, 2)
        Dim _AppCode As String = _Detail.IMI_No.Substring(_Detail.IMI_No.IndexOf(" ")).Trim


        _AppCode = _AppCode.Substring(0, 2)
        _Detail.MData1 = _tbl.Block1
        _Detail.MData2 = _tbl.Block2


        Dim _ret As Integer = 0
        Dim _fmt As String = _Detail.MData1
        Dim TotalChr As Integer = GetFreqFmt(_fmt)

        If _tbl.Block1 <> "" Then
            If TotalChr > 0 Then
                'Freq. Value
                _Detail.MData1 = _Detail.MData1.Replace(_fmt, _Detail.FreqVal.Substring(0, TotalChr))
            End If

            'Grade
            _Detail.MData1 = _Detail.MData1.Replace("?", _Detail.IMI_No.Substring(_Detail.IMI_No.Length - 1, 1))

            'Spec
            _Detail.MData1 = _Detail.MData1.Replace("!", _tbl.Spec)
        Else
            _Detail.MData1 = ""
        End If

        If _tbl.Block2 <> "" Then
            'Code
            If CType(_AppCode, Integer) <= 10 Or CType(_AppCode, Integer) = 23 Then
                _Detail.MData2 = _Detail.MData2.Replace("*", _tbl.Code)
            Else
                _Detail.MData2 = _Detail.MData2.Replace("*", _spec)
            End If

            If _tbl.Code <> "" Or _tbl.Spec <> "" Then
                _Detail.MData2 = "o" & _Detail.MData2
            End If

            'Spec
            _Detail.MData2 = _Detail.MData2.Replace("!", _tbl.Spec)

            'Prod. Code
            _Detail.MData2 = _Detail.MData2.Replace("#", _tbl.ProdCode)

            'Week Code
            _Detail._WeekCode = azWebService.WeekCode("ymd")
            _Detail.MData2 = _Detail.MData2.Replace("ymd", _Detail._WeekCode)

            If _Detail.MData2.Length < 6 Then
                _Detail.MData2 = "  " & _Detail.MData2
            End If

            _Detail.Profile = _tbl.MarkFile

            If Not _Detail.MData2.StartsWith("o") Then
                'Or _Detail.MData2.Substring(0, 1) = " "
                _Detail.Profile &= "_"
            Else
                '_Detail.Profile &= "_"
            End If

            _ret = ValidateDbWeekCode(_Detail)
        Else
            _Detail.MData2 = ""
        End If

        Return _ret

    End Function

    Public Function GetFreqFmt(ByRef _fmt As String) As Integer

        Dim _cnt As Integer = 0
        Dim _tmp As String = _fmt


        _fmt = ""

        For iLp As Integer = 0 To _tmp.Length - 1
            If _tmp.Substring(iLp, 1).ToLower = "x" Then
                _cnt += 1
                _fmt &= "x"
            End If
        Next

        Return _cnt

    End Function

    Public Function Get_CPSG_(ByVal _cmd As String, ByRef _tblRec() As tblCPSG) As Integer

        Dim _ret As Integer = 0
        Dim sConnStr As String = _
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " & _
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " & _
                    "uid=VB-SQL;" & _
                    "pwd=Anyn0m0us"
        '"Integrated Security=SSPI"


        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _cmd


        Try
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()

            With sqlReader
                Dim iRecNo As Integer = 0

                If .HasRows Then
                    ReDim _tblRec(iRecNo)

                    Do While .Read()
                        Application.DoEvents()
                        ReDim Preserve _tblRec(iRecNo)

                        With _tblRec(iRecNo)
                            .Product = sqlReader.GetString(0)
                            .Code = sqlReader.GetString(1)
                            .Spec = sqlReader.GetString(2)
                            .Block1 = sqlReader.GetString(3)
                            .Block2 = sqlReader.GetString(4)
                            .AppCode = sqlReader.GetString(5)
                            .FuncCode = sqlReader.GetString(6)
                            .ProdCode = sqlReader.GetString(7)
                            .MarkFile = sqlReader.GetString(8)
                            .Parameter = sqlReader.GetString(9)
                            .Ref = sqlReader.GetString(10)
                        End With

                        iRecNo += 1
                    Loop

                    _ret = iRecNo
                End If
            End With
        Catch ex As SqlException
            MessageBox.Show(ex.ToString, "SQL Exception Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
            _ret = Func_Ret_Fail
        End Try

        dbConnection.Close()
        Return _ret

    End Function

    Private Function ValidateDbWeekCode(ByRef _Detail As Rec) As Integer
        Dim RetVal As Integer = 0
        Dim sConnStr As String = _
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " & _
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " & _
                    "uid=VB-SQL;" & _
                    "pwd=Anyn0m0us"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "SELECT * FROM Records WHERE Lot_No='" & _Detail.Lot_No & "' "

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            'cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()

            With sqlReader
                Dim iFieldCnt As Integer = .FieldCount

                If .HasRows Then
                    Dim sRetData(iFieldCnt - 1) As String

                    Do While .Read()
                        Application.DoEvents()
                        sRetData(0) = sqlReader.GetString(0)    'LotNo
                        sRetData(1) = sqlReader.GetString(1)    'IMI
                        sRetData(3) = sqlReader.GetString(8)    'Data1
                        sRetData(4) = sqlReader.GetString(9)    'Data2

                        If sRetData(0).Trim.ToUpper = _Detail.Lot_No Then
                            If sRetData(3).Trim = _Detail.MData1.Trim Then
                                _Detail.MData2 = sRetData(4)

                                If _Detail.MData2.Length < 6 Then
                                    _Detail.MData2 = "  " & _Detail.MData2
                                End If
                            Else
                                RetVal = -1
                            End If

                            Exit Do
                        End If
                    Loop
                End If
            End With
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
            MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = 1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

End Module