Imports System
Imports System.Globalization
Imports System.Math
Imports System.Threading
Imports System.IO
Imports System.IO.Ports
Imports System.Management
Imports System.Runtime.InteropServices
Imports System.Data.SqlClient
Imports Microsoft.Win32


Module mdl_ActiveProc

    'Private Declare Function WritePrivateProfileSection Lib "kernel32" Alias "WritePrivateProfileSectionA" (ByVal lpAppName As String, ByVal lpString As String, ByVal lpFileName As String) As Long
    'Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Long
    Private Declare Function GetPrivateProfileSection Lib "kernel32" Alias "GetPrivateProfileSectionA" (ByVal lpAppName As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer


    Public Const MachineDataServer As String = "172.16.59.2"        'epm2mn1
    Public Const MachineSystemServer As String = "20.10.30.2"    'etmymnet

    Public Const Func_Ret_Success = 0
    Public Const Func_Ret_Fail = -1

    Public Const fg_OFF = 0
    Public Const fg_ON = 1

    Public Const ch_STX = &H2
    Public Const ch_ETX = &H3
    Public Const ch_ACK = &H6
    Public Const ch_NAK = &H15

    Public Const WarningMsg1 = "Click The 'Post Data' Button To Re-Fresh Laser Marking Data !!!"
    Public Const ImportantMsg1 = "Click 'Data Entry' Button To Set Marking Data... "

    Public EditTitle() As String = {"Edit Current Setting (A)", _
                                    "Edit QSW Setting (kHz)", _
                                    "Edit Speed Setting (mm/s)", _
                                    "Edit X-Axis Offset Setting (mm)", _
                                    "Edit Y-Axis Offset Setting (mm)", _
                                    "Edit Rotation Setting (deg)", _
                                    "Edit X-Axis Positioning (mm)", _
                                    "Edit Y-Axis Positioning (mm)", _
                                    "Edit Text Angle (deg)", _
                                    "Edit Width Alignment (mm)", _
                                    "Edit Space Width (mm)", _
                                    "Edit X-Axis Org. (mm)", _
                                    "Edit Y-Axis Org. (mm)", _
                                    "Edit Character Height (mm)", _
                                    "Edit Compress Rate (%)", _
                                    "Change Layout No..."}

    Public EditStrFmt() As String = {"{0:F1}", "{0:F1}", "{0:F2}", "{0:F3}", "{0:F3}", "{0:F6}", _
                                     "{0:F3}", "{0:F3}", "{0:F1}", "{0:D2}", "{0:F3}", "{0:F3}", "{0:F3}", _
                                     "{0:F3}", "{0:D1}", "{0:D1}"}
    Public EditDefault() As String = {"183", "200", "20000", "3430", "550", "359900000", _
                                      "0", "0", "2700", "", "", "", "", _
                                      "300", "100", "11"}
    Public EditModifier() As Integer = {10, 10, 100, 1000, 1000, 1000000, _
                                        1000, 1000, 10, 1, 1000, 1000, 1000, _
                                        1000, 1, 1}
    Public EditRng() As String = {"(5.0 ~ 30.0)", _
                                  "(0.0 ~ 199.9)", _
                                  "(1.0 ~ 300.00)", _
                                  "(-70 ~ 70)", _
                                  "(-70 ~ 70)", _
                                  "(0.0 ~ 360.0)", _
                                  "(-70 ~ 70)", _
                                  "(-70 ~ 70)", _
                                  "(0.0 ~ 360.0)", _
                                  "-", _
                                  "-", _
                                  "(-70 ~ 70)", _
                                  "(-70 ~ 70)", _
                                  "(0.001 ~ 5.000)", _
                                  "(1 ~ 199)", _
                                  "(1 ~ 99)"}

    Public EditOption() As String = {"Select Draw Type", _
                                     "Select Text Align Type", _
                                     "Select Space Align Type"}

    Public DrawOption() As String = {"Arc (IL)", "Arc (OL)", "Line"}
    Public DrawOptionDefault As String = "2"

    Public TextAlignOption() As String = {"No Set", "Left", "Center", "Right", "Zoom"}
    Public TextAlignOptionDefault As String = "0"

    Public SpaceAlignOption() As String = {"No Set", "Pitch", "Space"}
    Public SpaceAlignOptionDefault As String = "0"


    Public Enum MachineType
        PLC = 0
        PC = 1
    End Enum

    Public Enum LaserUnit
        ML7111A = 0
        ML7110B = 1
        ML7061 = 2
        SigmaKoki = 3
    End Enum

    Public Enum WeekCode
        Auto = 0
        Hardcoded
    End Enum

    Public Enum optMode
        DataEntry = 0
        PostData = 1
        Auto = 2
        Update = 3
    End Enum

    Public Structure CommPortData
        Public PortName As String
        Public DataBits As Integer
        Public BaudRate As Integer
        Public StopBits As System.IO.Ports.StopBits
        Public Parity As System.IO.Ports.Parity
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
        Public _WeekCode As String
    End Structure

    Public Structure SystemData
        Public LotNo As String
        Public IMINo As String
        Public EmpNo As String
        Public WeekCode As String
    End Structure

    Public Structure DB_Data
        Public Server As String
        Public Name As String
        Public uid As String
        Public pwd As String
    End Structure

    Public Enum SysAppMode
        app_Auto = 0
        app_Manu
        app_Setting
        app_NotInit
        app_AutoRun
        app_sysError
    End Enum

    Public Structure hw_Confg
        '---  DIO Declaration ---
        Public DIO_0 As cls_DIO2727

        Public OutPort1 As cls_DIO_Port
        Public OutPort2 As cls_DIO_Port
        Public OutPort3 As cls_DIO_Port
        Public OutPort4 As cls_DIO_Port

        Public i1_sw_Auto As cls_ioBits
        Public i2_sw_Cover As cls_ioBits
        Public i3_sw_Start As cls_ioBits
        Public i4_sw_Stop As cls_ioBits
        Public i5_sw_Power As cls_ioBits
        Public i6_sw_slsLeft As cls_ioBits
        Public i7_sw_slsRight As cls_ioBits
        Public i8_sw_Pressure As cls_ioBits

        Public i9_in_LaserRdy As cls_ioBits
        Public i10_in_Marking As cls_ioBits
        Public i11_sw_TraySensor As cls_ioBits
        Public i12_nc_1 As cls_ioBits
        Public i13_nc_2 As cls_ioBits
        Public i14_nc_3 As cls_ioBits
        Public i15_nc_4 As cls_ioBits
        Public i16_in_LaserErr As cls_ioBits

        Public o1_Stop_LED As cls_ioBits
        Public o2_LaserRdy_LED As cls_ioBits
        Public o3_Start_LED As cls_ioBits
        Public o4_Tray_Sol As cls_ioBits
        Public o5_Cover_Sol As cls_ioBits
        Public o6_Ext_Ctrl As cls_ioBits
        Public o7_MarkingStart As cls_ioBits
        Public o8_MarkingStop As cls_ioBits

        Public o9_Buzzer As cls_ioBits
        Public o10_nc As cls_ioBits
        Public o11_nc As cls_ioBits
        Public o12_nc As cls_ioBits
        Public o13_nc As cls_ioBits
        Public o14_nc As cls_ioBits
        Public o15_LaserLD As cls_ioBits
        Public o16_nc As cls_ioBits
    End Structure

    Public Structure SystemStruc
        Public MarkingSetting() As MarkingParameter
        Public TempSetting As MarkingParameter

        Public MarkingCondSetting As MarkingCondition
        Public TempCondSetting As MarkingCondition

        Public Lotdata() As Rec
        Public ML7111A As CommPortData
        Public _MachineCPU As CommPortData
        Public EditParameter As UpdateData
        Public DataBase_ As DB_Data
        Public IO As hw_Confg

        Public _MachineType As MachineType
        Public _LaserIUnit As LaserUnit
        Public _Mode As optMode
        Public _GetWeekCode As WeekCode

        Public AuthenticationCode As String
        Public AuthenticalAccess As String
        Public EquipType As String
        Public CtrlNo As String
        Public DataTrans As String
        Public NewLayoutNo As String
        Public CurSelection As String
        Public PreviousVerPath As String
        Public MarkingChar() As String
        Public ErrorMsg As String

        Public SysBsyCode As Integer
        Public GetAuthentication As Integer
        Public SelectedProfile As Integer
        Public SelectedBlock As Integer
        Public UntilBlock As Integer

        Public Init_HDW As Integer
        Public DataRecorded As Integer
        Public ErrorReset As UInteger
    End Structure

    Public Structure UpdateData
        Public OldData As String
        Public NewData As String
        Public IdxNo As Integer
    End Structure

    Public Structure MarkingParameter
        Public A_DrawType As String
        Public B_X_Axis As String
        Public C_Y_Axis As String
        Public D_TextAngle As String
        Public E_TextAlign As String
        Public F_WidthAlign As String
        Public G_SpaceAlign As String
        Public H_SpaceWidth As String
        Public I_X_AxisOrg As String
        Public J_Y_AxisOrg As String
        Public K_CharHeight As String
        Public L_Compress As String
        Public M_OppDir As String
        Public N_CharAngle As String
        Public O_Current As String
        Public P_QSW As String
        Public Q_Speed As String
        Public R_Repeat As String
        Public S_Mirror As String
        Public T_VarType As String
        Public U_VarNo As String
        Public LineNo As String
        Public SettingString As String
    End Structure

    Public Structure MarkingCondition
        Public A_Layout As String
        Public B_Xoffset As String
        Public C_Yoffset As String
        Public D_Rotation As String
        Public E_Current As String
        Public F_QSW As String
        Public G_Speed As String
    End Structure

    Public Structure ParameterProfile
        Public Spec As String
        Public StartNo As String
        Public UseDot As String
        Public UseBlock As String
        Public SettingCond As MarkingCondition
        Public ParamData() As MarkingParameter
    End Structure

    Public WithEvents Miyachi As SerialPort = New SerialPort
    Public WithEvents MachinePort As SerialPort = New SerialPort

    Public regKey As RegistryKey = Registry.CurrentUser
    Public regSubKey As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc")

    Public ActiveProc As SystemStruc
    Public DefaultProfile As ParameterProfile
    Public Profiles() As ParameterProfile = Nothing


    Public GrnLED_OnOff(1) As Color
    Public RedLED_OnOff(1) As Color
    Public OrgLED_OnOff(1) As Color
    Public BlueLED_OnOff(1) As Color
    Public PeruLED_OnOff(1) As Color
    Public YellowLED_OnOff(1) As Color



    Public Function ReadRegData() As Integer

        Try
            Dim regSubKeyComm_1 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\Miyachi_ML7111A")


            With ActiveProc
                .EquipType = regSubKey.GetValue("EquipType", "ActiveProc")
                .CtrlNo = regSubKey.GetValue("CtrlNo", "Mxxxxx")
                .AuthenticationCode = regSubKey.GetValue("AuthenticationAccess", "azActive")
                .PreviousVerPath = regSubKey.GetValue("PreviousVerPath", "C:\Program Files\ML7061_BarSys")

                ._MachineType = CType(regSubKey.GetValue("MachineType", "0"), MachineType)
                ._LaserIUnit = CType(regSubKey.GetValue("LaserUnit", "0"), LaserUnit)
                ._GetWeekCode = CType(regSubKey.GetValue("GetWeekCode", "0"), WeekCode)

                ' --- Add This Statement For Debug At Developer Machine ---
                '.PreviousVerPath = regSubKey.GetValue("PreviousVerPath", "h:\m02615\ML7061_BarSys")


#If UseSQL_Server = 1 Then
                With .DataBase_
                    .Server = regSubKey.GetValue("Database_server", "20.10.30.2\SQLEXPRESS")
                    .Name = regSubKey.GetValue("Database_name", "Marking")
                    .uid = regSubKey.GetValue("Database_uid", "VB-SQL")
                    .pwd = regSubKey.GetValue("Database_pwd", "Anyn0m0us")
                End With

                'With .DataBase_
                '    .Server = regSubKey.GetValue("Database_server", "DESKTOP-TLVFD7V\SQLEXPRESS")
                '    .Name = regSubKey.GetValue("Database_name", "Marking")
                '    .uid = regSubKey.GetValue("Database_uid", "sa")
                '    .pwd = regSubKey.GetValue("Database_pwd", "Az@HoePinc0615")
                'End With
#Else
                With .DataBase_
                    .Server = regSubKey.GetValue("Database_server", "local")
                    .Name = regSubKey.GetValue("Database_name", "Marking.mdb")
                    .uid = regSubKey.GetValue("Database_uid", "")
                    .pwd = regSubKey.GetValue("Database_pwd", "")
                End With
#End If

                With .ML7111A
                    .PortName = regSubKeyComm_1.GetValue("CommPortName", "COM1")

                    .BaudRate = CType(regSubKeyComm_1.GetValue("CommBaudRate"), Integer)
                    .BaudRate = IIf((.DataBits = 0), 9600, .BaudRate)

                    .DataBits = CType(regSubKeyComm_1.GetValue("CommDataBits"), Integer)
                    .DataBits = IIf((.DataBits = 0), 8, .DataBits)

                    .StopBits = CType(regSubKeyComm_1.GetValue("CommStopBits"), System.IO.Ports.StopBits)
                    .StopBits = IIf((.StopBits = 0), Ports.StopBits.One, .StopBits)

                    Dim sPrty As String = regSubKeyComm_1.GetValue("CommParity", "")
                    .Parity = IIf((sPrty = ""), IIf(ActiveProc._LaserIUnit = LaserUnit.ML7110B Or ActiveProc._LaserIUnit = LaserUnit.SigmaKoki, Ports.Parity.None, Ports.Parity.Even), Val(sPrty))
                    '.Parity = IIf((sPrty = ""), IIf(ActiveProc._LaserIUnit = LaserUnit.ML7110B, Ports.Parity.None, Ports.Parity.Even), Val(sPrty))
                End With

                With ._MachineCPU
                    .PortName = regSubKeyComm_1.GetValue("CommPortName", "COM2")

                    .BaudRate = CType(regSubKeyComm_1.GetValue("CommBaudRate"), Integer)
                    .BaudRate = IIf((.DataBits = 0), 9600, .BaudRate)

                    .DataBits = CType(regSubKeyComm_1.GetValue("CommDataBits"), Integer)
                    .DataBits = IIf((.DataBits = 0), 7, .DataBits)

                    .StopBits = CType(regSubKeyComm_1.GetValue("CommStopBits"), System.IO.Ports.StopBits)
                    .StopBits = IIf((.StopBits = 0), Ports.StopBits.Two, .StopBits)

                    Dim sPrty As String = regSubKeyComm_1.GetValue("CommParity", "")
                    .Parity = IIf((sPrty = ""), Ports.Parity.Even, Val(sPrty))
                End With
            End With
        Catch ex As Exception
            Return Func_Ret_Fail
        End Try

        Return Func_Ret_Success

    End Function

    Public Function GetMarkingSetting() As Integer

        RestoreDefault()

#If UseSQL_Server = 0 Then
        Return FromFile()
#Else
        Return FromServer()
#End If

    End Function

    Private Sub RestoreDefault()

        Dim regSubKeyComm_ As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\DefaultMarkingParameter")

        With DefaultProfile
            .Spec = regSubKeyComm_.GetValue("Spec", "PXFA")
            .UseBlock = regSubKeyComm_.GetValue("UseBlock", "0")
            .UseDot = regSubKeyComm_.GetValue("UseDot", "1")
            .StartNo = regSubKeyComm_.GetValue("StartLine", "2")

            With .SettingCond
                .A_Layout = regSubKeyComm_.GetValue("LayoutNo", "11")
                .B_Xoffset = regSubKeyComm_.GetValue("X_Offset", "3430")
                .C_Yoffset = regSubKeyComm_.GetValue("Y_Offset", "550")
                .D_Rotation = regSubKeyComm_.GetValue("Rotation", "359900000")
                .E_Current = regSubKeyComm_.GetValue("Current", "183")
                .F_QSW = regSubKeyComm_.GetValue("QSW", "200")
                .G_Speed = regSubKeyComm_.GetValue("Speed", "20000")
            End With

            ReDim .ParamData(5)
            .ParamData(0).SettingString = regSubKeyComm_.GetValue("Block1", "2,974,558,0,,,,,,,,276,100,,0,,,,,,,2,1100001")
            .ParamData(1).SettingString = regSubKeyComm_.GetValue("Block2", "2,701,967,900,,,1,340,,,,483,62,,0,,,,,,,0,200AP")
            .ParamData(2).SettingString = regSubKeyComm_.GetValue("Block3", "2,1351,967 ,900,,,1,340,,,,483,62,,0,,,,,,,0,EymdA")
            .ParamData(3).SettingString = regSubKeyComm_.GetValue("Block4", "2,1381,1630,900,,,,,,,,300,100,,0,,,,,,,0,_")
            .ParamData(4).SettingString = regSubKeyComm_.GetValue("Block5", "2,0,92,900,,,1,340,,,,210,10,1,0,,,,,,,0,.")
            .ParamData(5).SettingString = regSubKeyComm_.GetValue("Block6", "2,0,0,900,,,1,340,,,,210,10,,0,,,,,,,0,.")
        End With

    End Sub

    Private Function FromServer() As Integer

        Dim CreateTblString As String = String.Empty


        With DefaultProfile
            CreateTblString = "[CTRLNo] [nvarchar](12) NOT NULL CONSTRAINT [DF_Setting_CtrlNo]  DEFAULT (N'" & ActiveProc.CtrlNo & "')," & _
                "[Spec] [nvarchar](30) NOT NULL CONSTRAINT [DF_Setting_Spec]  DEFAULT (N'" & .Spec & "')," & _
                "[LayoutNo] [nvarchar](2) NOT NULL CONSTRAINT [DF_Setting_LayoutNo]  DEFAULT (N'" & .SettingCond.A_Layout & "')," & _
                "[Xoffset] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_Xoffset]  DEFAULT (N'" & .SettingCond.B_Xoffset & "')," & _
                "[Yoffset] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_Yoffset]  DEFAULT (N'" & .SettingCond.C_Yoffset & "')," & _
                "[Rotate] [nvarchar](10) NOT NULL CONSTRAINT [DF_Setting_Rotate]  DEFAULT (N'" & .SettingCond.D_Rotation & "')," & _
                "[Current] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_Current]  DEFAULT (N'" & .SettingCond.E_Current & "')," & _
                "[QSW] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_QSW]  DEFAULT (N'" & .SettingCond.F_QSW & "')," & _
                "[Speed] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_Speed]  DEFAULT (N'" & .SettingCond.G_Speed & "')," & _
                "[StartLine] [nvarchar](2) NOT NULL CONSTRAINT [DF_Setting_StartLine]  DEFAULT (N'" & .StartNo & "')," & _
                "[Block1] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block1]  DEFAULT (N'" & .ParamData(0).SettingString & "')," & _
                "[Block2] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block2]  DEFAULT (N'" & .ParamData(1).SettingString & "')," & _
                "[Block3] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block3]  DEFAULT (N'" & .ParamData(2).SettingString & "')," & _
                "[Block4] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block4]  DEFAULT (N'" & .ParamData(3).SettingString & "')," & _
                "[Block5] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block5]  DEFAULT (N'" & .ParamData(4).SettingString & "')," & _
                "[Block6] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block6]  DEFAULT (N'" & .ParamData(5).SettingString & "')," & _
                "[UseDot] [nvarchar](1) NOT NULL CONSTRAINT [DF_Setting_UseDot]  DEFAULT (N'" & .UseDot & "')," & _
                "[UseBlock] [nvarchar](2) NOT NULL CONSTRAINT [DF_Setting_UseBlock]  DEFAULT (N'" & .UseBlock & "')"
        End With


        Dim RetVal As Integer = Check_dboTables("Setting", CreateTblString)

        If Not RetVal < 0 Then
            RetVal = GetProfilesFromServer(Profiles)

            If RetVal < 0 Then
                RetVal = -9999
            End If
        End If

        Return RetVal

    End Function

    Public Function SQL_Server_Proc(ByVal SQLcmd As String, ByVal DataBaseName As String) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " &
                    "DataBase=" & DataBaseName & "; " &
                    "uid=" & ActiveProc.DataBase_.uid & "; " &
                    "pwd=" & ActiveProc.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = SQLcmd

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            'cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()
            RetVal = sqlReader.RecordsAffected
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Public Function SQL_Server_Proc(ByVal SQLcmd As String, ByVal DataBaseName As String, ByRef oldrec As Rec) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " &
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " &
                    "uid=" & ActiveProc.DataBase_.uid & "; " &
                    "pwd=" & ActiveProc.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = SQLcmd

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            'cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()

            With sqlReader
                RetVal = 0

                If .HasRows Then
                    Do While .Read()
                        Application.DoEvents()
                        RetVal += 1

                        oldrec.Lot_No = .GetString(0)
                        oldrec.IMI_No = .GetString(1)
                        oldrec.FreqVal = .GetString(2)
                        oldrec.Opt = .GetString(3)

                        Dim recdate As Date = .GetDateTime(4)
                        oldrec.RecDate = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", recdate.Day, recdate.Month, recdate.Year, recdate.Hour, recdate.Minute, recdate.Second)
                        oldrec.Profile = .GetString(5)
                        oldrec.CtrlNo = .GetString(6)
                        oldrec.MacNo = .GetString(7)
                        oldrec.MData1 = .GetString(8)
                        oldrec.MData2 = .GetString(9)
                        oldrec.MData3 = .GetString(10)
                        oldrec.MData4 = .GetString(11)
                        oldrec.MData5 = .GetString(12)
                        oldrec.MData6 = .GetString(13)
                    Loop
                End If
            End With
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Public Function InsertNewProfile_sql(ByVal NewProfileData As ParameterProfile) As Integer

        Dim regSubKeyComm_ As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\DefaultMarkingParameter")

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " &
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " &
                    "uid=" & ActiveProc.DataBase_.uid & "; " &
                    "pwd=" & ActiveProc.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
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

        'Debug.Print(strSQL)

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            'cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()
            RetVal = sqlReader.RecordsAffected
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Public Function GetProfileDetailsFromServer(ByVal SQLcmd As String, Optional ByRef SettingCondition() As ParameterProfile = Nothing) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " &
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " &
                    "uid=" & ActiveProc.DataBase_.uid & "; " &
                    "pwd=" & ActiveProc.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = SQLcmd


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
                Dim iRecNo As Integer = 0

                If .HasRows Then
                    Dim sRetData(iFieldCnt - 1) As String
                    ReDim SettingCondition(iRecNo)


                    Do While .Read()
                        Application.DoEvents()

                        ReDim Preserve SettingCondition(iRecNo)
                        ReDim SettingCondition(iRecNo).ParamData(5)

                        With SettingCondition(iRecNo)
                            .Spec = sqlReader.GetString(1)

                            With .SettingCond
                                .A_Layout = sqlReader.GetString(2)
                                .B_Xoffset = sqlReader.GetString(3)
                                .C_Yoffset = sqlReader.GetString(4)
                                .D_Rotation = sqlReader.GetString(5)
                                .E_Current = sqlReader.GetString(6)
                                .F_QSW = sqlReader.GetString(7)
                                .G_Speed = sqlReader.GetString(8)
                            End With

                            Dim StartLine As String = sqlReader.GetString(9)
                            .StartNo = StartLine

                            For iLp As Integer = 0 To .ParamData.GetUpperBound(0)
                                Application.DoEvents()
                                .ParamData(iLp).LineNo = (Val(StartLine) + iLp).ToString.Trim
                                .ParamData(iLp).SettingString = sqlReader.GetString(10 + iLp)
                            Next

                            .UseDot = sqlReader.GetString(16)
                            .UseBlock = sqlReader.GetString(17)
                        End With

                        iRecNo += 1
                    Loop
                Else
                    RetVal = -1
                End If
            End With
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Private Function GetProfilesFromServer(ByRef SettingCondition() As ParameterProfile) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " &
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " &
                    "uid=" & ActiveProc.DataBase_.uid & "; " &
                    "pwd=" & ActiveProc.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "SELECT * FROM Setting WHERE CtrlNo='" & ActiveProc.CtrlNo & "' " & _
            "ORDER BY Spec"

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
                Dim iRecNo As Integer = 0

                If .HasRows Then
                    Dim sRetData(iFieldCnt - 1) As String
                    ReDim SettingCondition(iRecNo)


                    Do While .Read()
                        Application.DoEvents()

                        ReDim Preserve SettingCondition(iRecNo)
                        ReDim SettingCondition(iRecNo).ParamData(5)

                        With SettingCondition(iRecNo)
                            .Spec = sqlReader.GetString(1)

                            With .SettingCond
                                .A_Layout = sqlReader.GetString(2)
                                .B_Xoffset = sqlReader.GetString(3)
                                .C_Yoffset = sqlReader.GetString(4)
                                .D_Rotation = sqlReader.GetString(5)
                                .E_Current = sqlReader.GetString(6)
                                .F_QSW = sqlReader.GetString(7)
                                .G_Speed = sqlReader.GetString(8)
                            End With

                            Dim StartLine As String = sqlReader.GetString(9)
                            .StartNo = StartLine

                            For iLp As Integer = 0 To .ParamData.GetUpperBound(0)
                                Application.DoEvents()
                                .ParamData(iLp).LineNo = (Val(StartLine) + iLp).ToString.Trim
                                .ParamData(iLp).SettingString = sqlReader.GetString(10 + iLp)
                            Next

                            .UseDot = sqlReader.GetString(16)
                            .UseBlock = sqlReader.GetString(17)
                        End With

                        iRecNo += 1
                    Loop
                Else
                    RetVal = -1
                End If
            End With
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Private Function Check_dboTables(ByVal TableName As String, ByVal CreateTblStr As String) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " &
                    "DataBase=" & "; " &
                    "uid=" & ActiveProc.DataBase_.uid & "; " &
                    "pwd=" & ActiveProc.DataBase_.pwd
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
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            cmd.ExecuteNonQuery()
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function


    Private Function FromFile() As Integer

        Try
            Dim regSubKeyComm_1 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\Miyachi_ML7111A")
            Dim regSubKeyComm_2 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter1")
            Dim regSubKeyComm_3 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter2")
            Dim regSubKeyComm_4 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter3")
            Dim regSubKeyComm_5 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter4")
            Dim regSubKeyComm_6 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter5")
            Dim regSubKeyComm_7 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter6")


            With ActiveProc
                'Line 1 : 2,974,558,0,,,,,,,,276,100,,0,,,,,,,2,1100001
                'Line 2 : 2,701,967,900,,,1,340,,,,483,62,,0,,,,,,,0,4000P
                'Line 3 : 2,1351,967,900,,,1,340,,,,483,62,,0,,,,,,,0,E888A
                'Line 4 : 2,1381,1630,900,,,,,,,,300,100,,0,,,,,,,0,_
                'Line 5 : 2,0,92,900,,,1,340,,,,210,10,1,0,,,,,,,0,.
                'Line 6 : 2,0,0,900,,,1,340,,,,210,10,,0,,,,,,,0,.

                Dim MarkingParameter() As String = {"", "", "", "", "", ""}

                If .DataBase_.Server.ToLower.Trim = "local" Then
                    If Not My.Computer.FileSystem.FileExists(My.Application.Info.DirectoryPath & "\" & .DataBase_.Name) Then
                        MarkingParameter(0) = regSubKeyComm_2.GetValue("Setting", "2,974,558,0,,,,,,,,276,100,,0,,,,,,,2,1100001")
                        MarkingParameter(1) = regSubKeyComm_3.GetValue("Setting", "2,701,967,900,,,1,340,,,,483,62,,0,,,,,,,0,200AP")
                        MarkingParameter(2) = regSubKeyComm_4.GetValue("Setting", "2,1351,967,900,,,1,340,,,,483,62,,0,,,,,,,0,EymdA")
                        MarkingParameter(3) = regSubKeyComm_5.GetValue("Setting", "2,1381,1630,900,,,,,,,,300,100,,0,,,,,,,0,_")
                        MarkingParameter(4) = regSubKeyComm_6.GetValue("Setting", "2,0,92,900,,,1,340,,,,210,10,1,0,,,,,,,0,.")
                        MarkingParameter(5) = regSubKeyComm_7.GetValue("Setting", "2,0,0,900,,,1,340,,,,210,10,,0,,,,,,,0,.")

                        .MarkingSetting(0).LineNo = regSubKeyComm_2.GetValue("LineNo", "2")
                        .MarkingSetting(1).LineNo = regSubKeyComm_3.GetValue("LineNo", "3")
                        .MarkingSetting(2).LineNo = regSubKeyComm_4.GetValue("LineNo", "4")
                        .MarkingSetting(3).LineNo = regSubKeyComm_5.GetValue("LineNo", "5")
                        .MarkingSetting(4).LineNo = regSubKeyComm_6.GetValue("LineNo", "6")
                        .MarkingSetting(5).LineNo = regSubKeyComm_7.GetValue("LineNo", "7")

                        For iLp As Integer = 0 To .MarkingSetting.GetUpperBound(0)
                            Application.DoEvents()
                            ReadParameter(.MarkingSetting(iLp), MarkingParameter(iLp))
                        Next

                        With .MarkingCondSetting
                            .A_Layout = regSubKeyComm_2.GetValue("LayoutNo", "11")
                            .B_Xoffset = regSubKeyComm_2.GetValue("X_Offset", "3430")
                            .C_Yoffset = regSubKeyComm_2.GetValue("Y_Offset", "550")
                            .D_Rotation = regSubKeyComm_2.GetValue("Rotation", "359900000")
                            .E_Current = regSubKeyComm_2.GetValue("Current", "183")
                            .F_QSW = regSubKeyComm_2.GetValue("QSW", "200")
                            .G_Speed = regSubKeyComm_2.GetValue("Speed", "20000")
                        End With
                    Else
                        Dim SQLcmd As String = "SELECT * FROM Setting " & _
                                               "WHERE CtrlNo='" & .CtrlNo & "' " & _
                                               "ORDER BY Spec"

                        If Read_odbcDB_Setting(SQLcmd, Profiles) < 0 Then
                            Return Func_Ret_Fail
                        End If
                    End If
                End If
            End With
        Catch ex As Exception
            Return Func_Ret_Fail
        End Try

        Return Func_Ret_Success

    End Function


    Public Sub ParseParamData(ByVal IdxNo As Integer)

        With ActiveProc
            Dim MarkingParameter() As String = {"", "", "", "", "", ""}

            For ilp As Integer = 0 To Profiles(IdxNo).ParamData.GetUpperBound(0)
                Application.DoEvents()

                .MarkingSetting(ilp).LineNo = Profiles(IdxNo).ParamData(ilp).LineNo
                MarkingParameter(ilp) = Profiles(IdxNo).ParamData(ilp).SettingString
                ReadParameter(.MarkingSetting(ilp), MarkingParameter(ilp))
            Next

            With .MarkingCondSetting
                .A_Layout = Profiles(IdxNo).SettingCond.A_Layout
                .B_Xoffset = Profiles(IdxNo).SettingCond.B_Xoffset
                .C_Yoffset = Profiles(IdxNo).SettingCond.C_Yoffset
                .D_Rotation = Profiles(IdxNo).SettingCond.D_Rotation
                .E_Current = Profiles(IdxNo).SettingCond.E_Current
                .F_QSW = Profiles(IdxNo).SettingCond.F_QSW
                .G_Speed = Profiles(IdxNo).SettingCond.G_Speed
            End With
        End With

    End Sub

    Public Function CheckDatabase() As Integer

        Dim RetVal As Integer = 0


#If UseSQL_Server = 1 Then
        Dim sConnStr As String = _
            "SERVER=" & ActiveProc.DataBase_.Server & "; " & _
            "DataBase=" & "; " & _
            "uid=" & ActiveProc.DataBase_.uid & ";" & _
            "pwd=" & ActiveProc.DataBase_.pwd & ";" & _
            "Timeout=10000"
        '"Integrated Security=SSPI"

        Try
            If My.Computer.Network.Ping(ActiveProc.DataBase_.Server.Substring(0, ActiveProc.DataBase_.Server.IndexOf("\")), 5000) = False Then
                Return -2
            End If
        Catch ex As Exception
            If My.Computer.Network.IsAvailable Then
                Return -4
            Else
                Return -3
            End If
        End Try


        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "IF NOT EXISTS (SELECT * FROM Sys.DATABASES WHERE Name='" & _
            ActiveProc.DataBase_.Name & "') " & _
            "CREATE DATABASE [" & ActiveProc.DataBase_.Name & "]"

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            cmd.ExecuteNonQuery()

        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    Public Function SaveRegMarkingConditionSetting(ByVal ConditionSetting As MarkingCondition, Optional ByVal NewMarkingSetting As String = "", Optional ByVal IdxNo As Integer = 0) As Integer

        Try
            Dim regSubKeyComm_1 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\Miyachi_ML7111A")
            Dim regSubKeyComm_2 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter1")
            Dim regSubKeyComm_3 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter2")
            Dim regSubKeyComm_4 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter3")
            Dim regSubKeyComm_5 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter4")
            Dim regSubKeyComm_6 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\MarkingParameter5")


            If Not NewMarkingSetting = "" Then
                Select Case IdxNo
                    Case Is = 0
                        regSubKeyComm_2.SetValue("Setting", NewMarkingSetting)
                    Case Is = 1
                        regSubKeyComm_3.SetValue("Setting", NewMarkingSetting)
                    Case Is = 2
                        regSubKeyComm_4.SetValue("Setting", NewMarkingSetting)
                    Case Is = 3
                        regSubKeyComm_5.SetValue("Setting", NewMarkingSetting)
                End Select
            End If

            With ConditionSetting
                regSubKeyComm_2.SetValue("X_Offset", .B_Xoffset)
                regSubKeyComm_2.SetValue("Y_Offset", .C_Yoffset)
                regSubKeyComm_2.SetValue("Rotation", .D_Rotation)
                regSubKeyComm_2.SetValue("Current", .E_Current)
                regSubKeyComm_2.SetValue("QSW", .F_QSW)
                regSubKeyComm_2.SetValue("Speed", .G_Speed)
            End With
        Catch ex As Exception
            Return Func_Ret_Fail
        End Try

    End Function

    Public Sub ReadParameter(ByRef MarkingParam As MarkingParameter, ByVal Setting As String)

        Dim MarkingParameter As String = Setting


        With MarkingParam
            .A_DrawType = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .A_DrawType = "" Then
                .A_DrawType = "2"
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .B_X_Axis = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .B_X_Axis = "" Then
                .B_X_Axis = "0"
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .C_Y_Axis = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .C_Y_Axis = "" Then
                .C_Y_Axis = "0"
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .D_TextAngle = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .D_TextAngle = "" Then
                .D_TextAngle = "0"
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .E_TextAlign = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .E_TextAlign = "" Then
                .E_TextAlign = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .F_WidthAlign = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .F_WidthAlign = "" Then
                .F_WidthAlign = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .G_SpaceAlign = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .G_SpaceAlign = "" Then
                .G_SpaceAlign = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .H_SpaceWidth = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .H_SpaceWidth = "" Then
                .H_SpaceWidth = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            Dim sDmy As String = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If sDmy = "" Then
                sDmy = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .I_X_AxisOrg = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .I_X_AxisOrg = "" Then
                .I_X_AxisOrg = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .J_Y_AxisOrg = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .J_Y_AxisOrg = "" Then
                .J_Y_AxisOrg = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .K_CharHeight = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .K_CharHeight = "" Then
                .K_CharHeight = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .L_Compress = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .L_Compress = "" Then
                .L_Compress = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .M_OppDir = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .M_OppDir = "" Then
                .M_OppDir = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .N_CharAngle = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .N_CharAngle = "" Then
                .N_CharAngle = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .O_Current = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .O_Current = "" Then
                .O_Current = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .P_QSW = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .P_QSW = "" Then
                .P_QSW = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .Q_Speed = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .Q_Speed = "" Then
                .Q_Speed = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .R_Repeat = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .R_Repeat = "" Then
                .R_Repeat = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .S_Mirror = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .S_Mirror = "" Then
                .S_Mirror = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            sDmy = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If sDmy = "" Then
                sDmy = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .T_VarType = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .T_VarType = "" Then
                .T_VarType = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .U_VarNo = MarkingParameter.Trim

            If .U_VarNo = "" Then
                .U_VarNo = ""

                If MarkingParameter.IndexOf(",") < 0 Then
                    MarkingParameter = ""
                Else
                    MarkingParameter = MarkingParameter.Substring(1)
                End If
            Else
                If MarkingParameter.IndexOf(",") < 0 Then
                    MarkingParameter = ""
                Else
                    MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
                End If
            End If
        End With

    End Sub

    Public Function InitSerialPort() As Integer

        Dim lng_RetVal As Long = Func_Ret_Fail


        'Initialize Comm. Port For Pick & Place Unit (FZ3)
        Try
            With Miyachi
                If .IsOpen = True Then
                    .Close()
                End If

                .PortName = ActiveProc.ML7111A.PortName
                .Parity = ActiveProc.ML7111A.Parity
                .BaudRate = ActiveProc.ML7111A.BaudRate
                .StopBits = ActiveProc.ML7111A.StopBits
                .DataBits = ActiveProc.ML7111A.DataBits
                .ReceivedBytesThreshold = 1
                .ReadBufferSize = 1024

                '.Open()
            End With
        Catch
            Return lng_RetVal
        End Try

        Return Func_Ret_Success

    End Function

    Public Function InitSerialPort(ByRef _sysport As System.IO.Ports.SerialPort) As Integer

        Dim lng_RetVal As Long = Func_Ret_Fail


        'Initialize Comm. Port For Pick & Place Unit (FZ3)
        Try
            With _sysport
                If .IsOpen = True Then
                    .Close()
                End If

                .PortName = ActiveProc.ML7111A.PortName
                .Parity = ActiveProc.ML7111A.Parity
                .BaudRate = ActiveProc.ML7111A.BaudRate
                .StopBits = ActiveProc.ML7111A.StopBits
                .DataBits = ActiveProc.ML7111A.DataBits
                .ReceivedBytesThreshold = 1
                .ReadBufferSize = 1024

                '.Open()
            End With
        Catch
            Return lng_RetVal
        End Try

        Return Func_Ret_Success

    End Function

    Public Function CalCheckSum(ByVal strCmd As String, Optional ByVal StringLength As Integer = 2) As String

        Dim CheckSum As Integer = 0


        For iLp As Integer = 0 To strCmd.Length - 1
            Application.DoEvents()
            CheckSum += Asc(strCmd.Substring(iLp, 1))
        Next

        Dim RetString As String = String.Format("{0:X2}", CheckSum)

        If RetString.Length > StringLength Then
            RetString = RetString.Substring(RetString.Length - StringLength)
        End If

        Return RetString

    End Function

    Public Function ML7111A_cmd(ByRef strCmd As String, Optional ByRef strRepMsg As String = "") As Integer

        Dim int_WaitDlyTimer As Long = 0
        Dim SuccessError As Integer = 0
        Dim int_Dmy As Integer = 0


        With Miyachi
            If .IsOpen = False Then
                Try
                    .Open()
                Catch ex As Exception
                    Return Func_Ret_Fail
                End Try
            End If

            strCmd = Chr(ch_STX) & strCmd & Chr(ch_ETX)
            strCmd = strCmd & CalCheckSum(strCmd)
            .Write(strCmd & IIf(ActiveProc._LaserIUnit = LaserUnit.ML7110B, "", vbCrLf))

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

            Do Until ReadByteSize = 0 And (Not str_Buffer.IndexOf(Chr(ch_ACK)) < 0 Or Not str_Buffer.IndexOf(Chr(ch_NAK)) < 0 Or Not str_Buffer.IndexOf(Chr(ch_ETX)) < 0)
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

            If Not str_Buffer.IndexOf(Chr(ch_NAK)) < 0 Then
                Return Func_Ret_Fail
            End If

            str_Buffer = str_Buffer.Trim
            strRepMsg = str_Buffer
            SuccessError = str_Buffer.Length

            Return SuccessError
        End With

    End Function

    Public Function GetWeekNoOfYear() As String

        Dim myCI As New CultureInfo("en-US")
        Dim myCal As Calendar = myCI.Calendar

        Return myCal.GetWeekOfYear(Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString

    End Function

    Public Function FormPostStream(ByVal MarkingSetting As MarkingParameter, Optional ByVal SaveMode As Integer = 0) As String

        Dim SetMarkingParameter As String = String.Empty


        With MarkingSetting
            If SaveMode = 0 Then SetMarkingParameter &= .LineNo & ","
            SetMarkingParameter &= .A_DrawType & ","
            SetMarkingParameter &= .B_X_Axis & ","
            SetMarkingParameter &= .C_Y_Axis & ","
            SetMarkingParameter &= .D_TextAngle & ","
            SetMarkingParameter &= .E_TextAlign & ","
            SetMarkingParameter &= .F_WidthAlign & ","
            SetMarkingParameter &= .G_SpaceAlign & ","
            SetMarkingParameter &= .H_SpaceWidth & ","
            SetMarkingParameter &= "" & ","
            SetMarkingParameter &= .I_X_AxisOrg & ","
            SetMarkingParameter &= .J_Y_AxisOrg & ","
            SetMarkingParameter &= .K_CharHeight & ","
            SetMarkingParameter &= .L_Compress & ","
            SetMarkingParameter &= .M_OppDir & ","
            SetMarkingParameter &= .N_CharAngle & ","
            SetMarkingParameter &= .O_Current & ","
            SetMarkingParameter &= .P_QSW & ","
            SetMarkingParameter &= .Q_Speed & ","
            SetMarkingParameter &= .R_Repeat & ","
            SetMarkingParameter &= .S_Mirror & ","
            SetMarkingParameter &= "" & ","
            SetMarkingParameter &= .T_VarType & ","

            'If Not ActiveProc.Lotdata(1).MData2 Is Nothing Then
            '    SetMarkingParameter &= IIf(ActiveProc.Lotdata(1).MData1 = "!", ".", ActiveProc.Lotdata(1).MData1)
            'Else
            '    SetMarkingParameter &= .U_VarNo
            'End If


            Dim _profile() As String = ActiveProc.Lotdata(1).Profile.Split(","c)

            Select Case .LineNo
                Case Is = 2
                    If _profile(0).ToUpper.Contains("TCI_F") Then
                        If ActiveProc._LaserIUnit = LaserUnit.ML7110B Then
                            SetMarkingParameter &= .U_VarNo
                        Else
                            SetMarkingParameter &= "1"
                        End If
                    Else
                        If Not Val(Profiles(ActiveProc.SelectedProfile).UseDot) = 0 Then
                            SetMarkingParameter &= .U_VarNo
                        Else
                            SetMarkingParameter &= IIf(ActiveProc.Lotdata(1).MData1 = "!", ".", ActiveProc.Lotdata(1).MData1)
                            'SetMarkingParameter &= ActiveProc.Lotdata(1).MData1
                        End If
                    End If
                Case Is = 3
                    If _profile(0).ToUpper.Contains("TCI_F") Then
                        SetMarkingParameter &= ActiveProc.Lotdata(1).MData1
                    Else
                        If Not Val(Profiles(ActiveProc.SelectedProfile).UseDot) = 0 Then
                            SetMarkingParameter &= IIf(ActiveProc.Lotdata(1).MData1 = "!", ".", ActiveProc.Lotdata(1).MData1)
                            'SetMarkingParameter &= ActiveProc.Lotdata(1).MData1
                        Else
                            SetMarkingParameter &= ActiveProc.Lotdata(1).MData2
                        End If
                    End If
                Case Is = 4
                    If _profile(0).ToUpper.Contains("TCI_F") Then
                        SetMarkingParameter &= ActiveProc.Lotdata(1).MData2
                    Else
                        If Not Val(Profiles(ActiveProc.SelectedProfile).UseDot) = 0 Then
                            SetMarkingParameter &= ActiveProc.Lotdata(1).MData2
                        Else
                            SetMarkingParameter &= .U_VarNo
                        End If
                    End If
                Case Is = 5
                    If _profile(0).ToUpper.Contains("TCI_F") Then
                        SetMarkingParameter &= ActiveProc.Lotdata(1).MData3
                    Else
                        SetMarkingParameter &= .U_VarNo
                    End If
                Case Is = 6
                    If _profile(0).ToUpper.Contains("TCI_F") Then
                        If ActiveProc._LaserIUnit = LaserUnit.ML7110B Then
                            SetMarkingParameter &= .U_VarNo
                        Else
                            SetMarkingParameter &= "5"
                        End If
                    Else
                        SetMarkingParameter &= .U_VarNo
                    End If
                Case Else
                    SetMarkingParameter &= .U_VarNo
            End Select
        End With

        Return SetMarkingParameter

    End Function

    Public Function GetFilesList(ByVal PathName As String, ByVal ExtName As String, ByRef allFiles() As String) As String

        Dim pngCount As Integer = -1


        With ActiveProc
            Try
                Dim files = From file In My.Computer.FileSystem.GetFiles(PathName) _
                Order By file _
                Select file

                Dim filesinfo = From file In files _
                        Select My.Computer.FileSystem.GetFileInfo(file)

                Dim SelectInfo = From file In filesinfo _
                        Where file.Extension = ExtName _
                        Select file.FullName

                allFiles = SelectInfo.ToArray
                pngCount = allFiles.GetUpperBound(0)
            Catch ex As Exception
                pngCount = -1
            End Try
        End With

        Return pngCount

    End Function

    Public Function GetPrivateInfoString(ByVal Sect_Name As String, ByVal Key_Name As String, ByVal File_Name As String) As String

        Dim dll_func As Integer = 0
        Dim RetStr As String = New String(vbNullChar, 512)


        dll_func = GetPrivateProfileString(Sect_Name, Key_Name, "", RetStr, RetStr.Length, File_Name)

        If Not RetStr = "" Then
            RetStr = RetStr.Substring(0, RetStr.IndexOf(vbNullChar))
            Return RetStr
        Else
            Return ""
        End If

    End Function

    Public Function GetPrivateInfoString(ByVal Sect_Name As String, ByVal File_Name As String) As String

        Dim dll_func As Integer = 0
        Dim RetStr As String = New String(vbNullString, 256)


        dll_func = GetPrivateProfileSection(Sect_Name, RetStr, RetStr.Length, File_Name)

        If Not RetStr = "" Then
            RetStr = RetStr.Substring(0, RetStr.IndexOf(vbNullChar))
            Return RetStr
        Else
            Return ""
        End If

    End Function

    Public Function Init_Hardware()

        Dim iRetVal As Integer = 0
        Dim ioBoardCnt As Integer = 0


        With ActiveProc.IO
            .DIO_0 = New cls_DIO2727("FBIDIO1", 0, "Interface")
            ioBoardCnt = cls_DIO2727.Count

            If .DIO_0.BoardHnd > 0 Then
                .OutPort1 = New cls_DIO_Port("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, cls_DIO_Port.PortDirection.Out_Port)
                .OutPort2 = New cls_DIO_Port("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, cls_DIO_Port.PortDirection.Out_Port)

                .OutPort1.NewPortValue(0)
                .OutPort2.NewPortValue(0)

                .i1_sw_Auto = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 1, cls_ioBits.BitDirection.In_Bit)
                .i2_sw_Cover = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 2, cls_ioBits.BitDirection.In_Bit)
                .i3_sw_Start = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 3, cls_ioBits.BitDirection.In_Bit)
                .i4_sw_Stop = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 4, cls_ioBits.BitDirection.In_Bit)
                .i5_sw_Power = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 5, cls_ioBits.BitDirection.In_Bit)
                .i6_sw_slsLeft = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 6, cls_ioBits.BitDirection.In_Bit)
                .i7_sw_slsRight = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 7, cls_ioBits.BitDirection.In_Bit)
                .i8_sw_Pressure = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 8, cls_ioBits.BitDirection.In_Bit)

                .i9_in_LaserRdy = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 1, cls_ioBits.BitDirection.In_Bit)
                .i10_in_Marking = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 2, cls_ioBits.BitDirection.In_Bit)
                .i11_sw_TraySensor = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 3, cls_ioBits.BitDirection.In_Bit)
                .i12_nc_1 = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 4, cls_ioBits.BitDirection.In_Bit)
                .i13_nc_2 = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 5, cls_ioBits.BitDirection.In_Bit)
                .i14_nc_3 = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 6, cls_ioBits.BitDirection.In_Bit)
                .i15_nc_4 = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 7, cls_ioBits.BitDirection.In_Bit)
                .i16_in_LaserErr = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 8, cls_ioBits.BitDirection.In_Bit)

                .o1_Stop_LED = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 1, cls_ioBits.BitDirection.Out_Bit)
                .o2_LaserRdy_LED = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 2, cls_ioBits.BitDirection.Out_Bit)
                .o3_Start_LED = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 3, cls_ioBits.BitDirection.Out_Bit)
                .o4_Tray_Sol = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 4, cls_ioBits.BitDirection.Out_Bit)
                .o5_Cover_Sol = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 5, cls_ioBits.BitDirection.Out_Bit)
                .o6_Ext_Ctrl = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 6, cls_ioBits.BitDirection.Out_Bit)
                .o7_MarkingStart = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 7, cls_ioBits.BitDirection.Out_Bit)
                .o8_MarkingStop = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT1_8, 8, cls_ioBits.BitDirection.Out_Bit)

                .o9_Buzzer = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 1, cls_ioBits.BitDirection.Out_Bit)
                .o10_nc = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 2, cls_ioBits.BitDirection.Out_Bit)
                .o11_nc = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 3, cls_ioBits.BitDirection.Out_Bit)
                .o12_nc = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 4, cls_ioBits.BitDirection.Out_Bit)
                .o13_nc = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 5, cls_ioBits.BitDirection.Out_Bit)
                .o14_nc = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 6, cls_ioBits.BitDirection.Out_Bit)
                .o15_LaserLD = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 7, cls_ioBits.BitDirection.Out_Bit)
                .o16_nc = New cls_ioBits("FBIDIO1", 0, "Interface", .DIO_0.BoardHnd, FBIDIO_OUT9_16, 8, cls_ioBits.BitDirection.Out_Bit)

                iRetVal = Func_Ret_Success
            Else
                iRetVal = Func_Ret_Fail
            End If

            Return iRetVal
        End With

    End Function

    Public Function SQL_CustFunction(ByVal DestCtrlNo As String, ByVal SrcCtrlNo As String, Optional ByVal Criteria As String = "") As Integer

        Dim SQLcmd As String = "IF NOT EXISTS (SELECT * FROM Setting WHERE CtrlNo='" & DestCtrlNo & IIf(Criteria = "", "')", "' AND " & Criteria & ") ") & _
                               "INSERT INTO Setting (CtrlNo, Spec,LayoutNo,Xoffset,Yoffset,Rotate,[Current],QSW,Speed,StartLine,Block1,Block2,Block3,Block4,Block5,Block6,UseDot,UseBlock)  " & _
                               "Select CtrlNo='" & DestCtrlNo & "',Spec,LayoutNo,Xoffset,Yoffset,Rotate,[Current],QSW,Speed,StartLine,Block1,Block2,Block3,Block4,Block5,Block6,UseDot,UseBlock " & _
                               "FROM Setting WHERE CtrlNo='" & SrcCtrlNo & IIf(Criteria = "", "'", "' AND " & Criteria)

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & ActiveProc.DataBase_.Server & "; " &
                    "DataBase=" & ActiveProc.DataBase_.Name & "; " &
                    "uid=" & ActiveProc.DataBase_.uid & "; " &
                    "pwd=" & ActiveProc.DataBase_.pwd


        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = SQLcmd


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


End Module
