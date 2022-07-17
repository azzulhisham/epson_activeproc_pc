Module mdl_SigmaKoki_Mach

    Public Structure _Machine
        Public PLCmemory As String
        Public MarkingLineNo As String
    End Structure


    Public Function ReadCommInfo(ByRef machine As _Machine) As Integer

        Dim _ret As Integer = Func_Ret_Fail


        With machine
            Try
                .PLCmemory = regSubKey.GetValue("Machine_PLCmemory", "@00RD02000001")
                .MarkingLineNo = regSubKey.GetValue("Machine_MarkingLineNo", "2")
            Catch ex As Exception
                Return _ret
            End Try
        End With

        Return Func_Ret_Success

    End Function

    Public Function ChkMachineStatus(ByVal _commport As System.IO.Ports.SerialPort) As Integer

        Dim _MacInfo As New _Machine
        Dim _ret As Integer = ReadCommInfo(_MacInfo)


        _ret = InitSerialPort(_commport)

        With _commport
            If .IsOpen = False Then
                Try
                    .Open()
                Catch ex As Exception
                    Return Func_Ret_Fail
                End Try
            End If

            Dim strCmd As String = _MacInfo.PLCmemory & Trim(GetFCS(_MacInfo.PLCmemory)) & "*" & vbCrLf
            .Write(strCmd)


            Dim RecvBytesFlg As Integer = Func_Ret_Success
            Dim WaitReplyTimer As Integer = My.Computer.Clock.TickCount


            Do While .BytesToRead = 0
                Application.DoEvents()
                If My.Computer.Clock.TickCount > WaitReplyTimer + 3000 Then RecvBytesFlg = Func_Ret_Fail : Exit Do
            Loop

            If RecvBytesFlg < 0 Then Return Func_Ret_Fail

            Dim ReadByteSize As Integer = .BytesToRead
            Dim str_Buffer As String = String.Empty
            Dim Buffer() As Char

            WaitReplyTimer = My.Computer.Clock.TickCount

            Do Until ReadByteSize = 0
                Application.DoEvents()
                If My.Computer.Clock.TickCount > WaitReplyTimer + 3000 Then Return Func_Ret_Fail

                ReDim Buffer(ReadByteSize)
                .Read(Buffer, 0, ReadByteSize)

                For int_Dmy As Integer = 0 To Buffer.GetUpperBound(0)
                    Application.DoEvents()

                    If Not Buffer(int_Dmy) = Nothing Then
                        If Buffer(int_Dmy) = vbCr Then
                            str_Buffer &= vbCr
                        ElseIf Buffer(int_Dmy) = vbLf Then
                            str_Buffer &= vbLf
                        Else
                            str_Buffer &= Buffer(int_Dmy)
                        End If
                    End If
                Next

                ReadByteSize = .BytesToRead
            Loop

            If str_Buffer = "" Or str_Buffer.IndexOf(_MacInfo.PLCmemory) < 0 Then
                Return Func_Ret_Fail
            End If

            str_Buffer = str_Buffer.Replace(vbCr, "")
            str_Buffer = str_Buffer.Replace(vbLf, "")

            If str_Buffer.EndsWith(GetFCS(str_Buffer)) Then
                str_Buffer = str_Buffer.Substring(0, str_Buffer - 2)
                str_Buffer = str_Buffer.Substring(7)

                Return CType(str_Buffer, Integer)
            Else
                Return Func_Ret_Fail
            End If

        End With

    End Function

    Public Function GetFCS(ByVal _Code As String) As String

        Dim IntCode As Integer


        IntCode = Asc(_Code.Substring(0, 1))

        For iLp As Integer = 1 To _Code.Length - 1
            IntCode = IntCode Xor Asc(_Code.Substring(iLp, 1))
        Next

        Dim HexCode As String = String.Format("{0:X2}", IntCode)
        Return HexCode.Substring(HexCode.Length - 2)

    End Function

    Public Function SendToSK(ByVal _SKdata As Rec) As Integer

        Dim _MachineStatus As Integer = 0

        If CType(regSubKey.GetValue("ChkMachineStatus", "0"), Integer) = 1 Then
            _MachineStatus = ChkMachineStatus(MachinePort)
        End If

        Select Case _MachineStatus
            Case Is < 0
                'Error
            Case Is = 0
                'Manu Mode
                Dim _fn As String = "SMPFL " & _SKdata.Profile.Substring(0, _SKdata.Profile.IndexOf(","c))

                If SK_cmd(_fn) = Func_Ret_Success Then
                    Dim _MacInfo As New _Machine
                    Dim _ret As Integer = ReadCommInfo(_MacInfo)

                    For iLp As Integer = 1 To _MacInfo.MarkingLineNo
                        Dim _dt As String = String.Format("TRNSC {0:D1} ", iLp)

                        Select Case iLp
                            Case Is = 1
                                If _SKdata.MData1 = "" Then
                                    Continue For
                                Else
                                    _dt &= Chr(34) & _SKdata.MData1 & Chr(34)
                                End If
                            Case Is = 2
                                If _SKdata.MData2 = "" Then
                                    Continue For
                                Else
                                    _dt &= Chr(34) & IIf(_SKdata.MData2.IndexOf("o") >= 0, _SKdata.MData2.Substring(_SKdata.MData2.IndexOf("o") + 1), _SKdata.MData2) & Chr(34)
                                End If
                        End Select

                        If Not SK_cmd(_dt) = Func_Ret_Success Then
                            MessageBox.Show(_dt, "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            Return Func_Ret_Fail
                        End If
                    Next

                    Return Func_Ret_Success
                Else
                    MessageBox.Show(_fn, "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return Func_Ret_Fail
                End If
            Case Is > 0
                'Auto Mode

        End Select

    End Function

    Private Function CalSum(ByVal _str As String) As String

        Dim _sum As Integer = 0


        For iLp As Integer = 0 To _str.Length - 1
            _sum += Asc(_str.Substring(iLp, 1))
        Next

        _sum = (Not _sum) + 1

        Dim HexCode As String = String.Format("{0:X2}", _sum)
        Return HexCode.Substring(HexCode.Length - 2)

    End Function

    Public Function SK_cmd(ByRef _cmd As String) As Integer

        Dim _CmdStr As String = Chr(ch_STX) & _cmd & Chr(ch_ETX)
        _CmdStr = _CmdStr & CalSum(_CmdStr)


        With Miyachi
            If .IsOpen = False Then
                Try
                    .Open()
                Catch ex As Exception
                    Return Func_Ret_Fail
                End Try
            End If

            .Write(_CmdStr & vbCrLf)

            Dim RecvBytesFlg As Integer = Func_Ret_Success
            Dim WaitReplyTimer As Integer = My.Computer.Clock.TickCount


            Do While .BytesToRead = 0
                Application.DoEvents()
                If My.Computer.Clock.TickCount > WaitReplyTimer + 3000 Then RecvBytesFlg = Func_Ret_Fail : Exit Do
            Loop

            If RecvBytesFlg < 0 Then
                _cmd = "Communication Time Out !"
                Return Func_Ret_Fail
            End If

            Dim ReadByteSize As Integer = .BytesToRead
            Dim str_Buffer As String = String.Empty
            Dim Buffer() As Char

            WaitReplyTimer = My.Computer.Clock.TickCount

            Do Until ReadByteSize = 0 And str_Buffer.Contains(vbCr)
                Application.DoEvents()
                If My.Computer.Clock.TickCount > WaitReplyTimer + 3000 Then Return Func_Ret_Fail

                ReDim Buffer(ReadByteSize)
                .Read(Buffer, 0, ReadByteSize)

                For int_Dmy = 0 To Buffer.GetUpperBound(0)
                    Application.DoEvents()

                    If Not Buffer(int_Dmy) = Nothing Then
                        If Buffer(int_Dmy) = vbCr Then
                            str_Buffer &= vbCr
                        ElseIf Buffer(int_Dmy) = vbLf Then
                            str_Buffer &= vbLf
                        Else
                            str_Buffer &= Buffer(int_Dmy)
                        End If
                    End If
                Next

                ReadByteSize = .BytesToRead
            Loop

            str_Buffer = str_Buffer.Replace(vbLf, "")
            str_Buffer = str_Buffer.Replace(vbCr, "")


            Dim _tmp As Integer = str_Buffer.LastIndexOf(Chr(ch_ETX))

            If _tmp < 0 Then
                _cmd = "Invalid command respond !"
                Return Func_Ret_Fail
            Else
                If CalSum(str_Buffer.Substring(0, _tmp + 1)) = str_Buffer.Substring(_tmp + 1) Then
                    If Not str_Buffer.IndexOf("N") < 0 Then
                        str_Buffer = str_Buffer.Replace(Chr(ch_STX), "")
                        str_Buffer = str_Buffer.Replace("N", "")

                        _tmp = CType(str_Buffer.Substring(0, str_Buffer.IndexOf(Chr(ch_ETX))), Integer)
                        Dim sErrMsg As String = ""

                        Select Case _tmp
                            Case 1, 2, 3, 4
                                sErrMsg = "Command String Format Error..."
                            Case 5
                                sErrMsg = "Command String Exist 64 Char !"
                            Case 7
                                sErrMsg = "File Not Found !"
                            Case 10
                                sErrMsg = "Marking Line No. Selection Error !"
                            Case 20
                                sErrMsg = "Undefined Command !"
                            Case 30
                                sErrMsg = "Check Sum Error !"
                            Case 99
                                sErrMsg = "Undefined Error !"
                            Case 120
                                sErrMsg = "Directory Not Found !"
                            Case 130
                                sErrMsg = "Duplicate Directory !"
                            Case 131
                                sErrMsg = "Can't Make Directory !"
                            Case 140
                                sErrMsg = "Can't Delete Directory !"
                            Case 141
                                sErrMsg = "Error 141 ! (Not being defined...)"
                            Case 142
                                sErrMsg = "Duplicate File !"
                            Case 160
                                sErrMsg = "Marking File Not Found !"
                            Case 221
                                sErrMsg = "Wrong Line Number !"
                            Case 222
                                sErrMsg = "Big Character Size !"
                            Case 224
                                sErrMsg = "Invalid Character !"
                            Case 230
                                sErrMsg = "Can't Start Marking !"
                            Case 231
                                sErrMsg = "Marking Error !"
                            Case 300
                                sErrMsg = "Communication Error !"
                            Case 340
                                sErrMsg = "Initialization fail !"
                            Case 400
                                sErrMsg = "Error Code : 400 - Parameter Error !"
                            Case 401
                                sErrMsg = "Error Code : 401 - Marking Area Over !"
                            Case 410
                                sErrMsg = "Error Code : 410 - Parameter Error !"
                            Case 411
                                sErrMsg = "Error Code : Marking Area Over !"
                            Case 999
                                sErrMsg = "Communication with Sigma-Koki's MarkBoy Fail ! MarkBoy may not in Ready Mode." & vbCrLf
                                sErrMsg = sErrMsg & "Or you may have no connection between MarkBoy." & vbCrLf
                            Case Else
                                sErrMsg = "Unidentified Error !"
                        End Select

                        _cmd = sErrMsg
                        Return Func_Ret_Fail
                    Else
                        Return Func_Ret_Success
                    End If
                Else
                    _cmd = "Check Sum not match !"
                    Return Func_Ret_Fail
                End If
            End If

        End With

    End Function

End Module
