<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_HardwareInit
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frm_HardwareInit))
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btn_Cmd = New System.Windows.Forms.Button()
        Me.pic_Stop = New System.Windows.Forms.PictureBox()
        Me.pic_Start = New System.Windows.Forms.PictureBox()
        Me.pic_PwrOFF = New System.Windows.Forms.PictureBox()
        Me.lbl_Status = New System.Windows.Forms.Label()
        Me.tmr_LblBlink = New System.Windows.Forms.Timer(Me.components)
        Me.tmr_BlinkLED = New System.Windows.Forms.Timer(Me.components)
        Me.tmr_IOScan = New System.Windows.Forms.Timer(Me.components)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic_Stop, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic_Start, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic_PwrOFF, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), System.Drawing.Image)
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.Location = New System.Drawing.Point(-16, -8)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(241, 256)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Times New Roman", 18.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(28, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(446, 27)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "ActiveProc - Laser Marking Barcode System"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Times New Roman", 12.75!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Gray
        Me.Label2.Location = New System.Drawing.Point(119, 36)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(355, 19)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Reliability Enhancement with SQL and IIS Server"
        '
        'PictureBox2
        '
        Me.PictureBox2.BackgroundImage = CType(resources.GetObject("PictureBox2.BackgroundImage"), System.Drawing.Image)
        Me.PictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox2.Location = New System.Drawing.Point(183, 140)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(83, 73)
        Me.PictureBox2.TabIndex = 3
        Me.PictureBox2.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.Gray
        Me.Label3.Location = New System.Drawing.Point(152, 206)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(147, 14)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Think to make a different"
        '
        'btn_Cmd
        '
        Me.btn_Cmd.BackColor = System.Drawing.Color.Green
        Me.btn_Cmd.ForeColor = System.Drawing.Color.Yellow
        Me.btn_Cmd.Image = CType(resources.GetObject("btn_Cmd.Image"), System.Drawing.Image)
        Me.btn_Cmd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btn_Cmd.Location = New System.Drawing.Point(386, 180)
        Me.btn_Cmd.Name = "btn_Cmd"
        Me.btn_Cmd.Padding = New System.Windows.Forms.Padding(3, 0, 6, 0)
        Me.btn_Cmd.Size = New System.Drawing.Size(86, 37)
        Me.btn_Cmd.TabIndex = 5
        Me.btn_Cmd.Text = "Start"
        Me.btn_Cmd.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btn_Cmd.UseVisualStyleBackColor = False
        Me.btn_Cmd.Visible = False
        '
        'pic_Stop
        '
        Me.pic_Stop.BackgroundImage = CType(resources.GetObject("pic_Stop.BackgroundImage"), System.Drawing.Image)
        Me.pic_Stop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.pic_Stop.Location = New System.Drawing.Point(273, 153)
        Me.pic_Stop.Name = "pic_Stop"
        Me.pic_Stop.Size = New System.Drawing.Size(26, 26)
        Me.pic_Stop.TabIndex = 6
        Me.pic_Stop.TabStop = False
        Me.pic_Stop.Visible = False
        '
        'pic_Start
        '
        Me.pic_Start.BackgroundImage = CType(resources.GetObject("pic_Start.BackgroundImage"), System.Drawing.Image)
        Me.pic_Start.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.pic_Start.Location = New System.Drawing.Point(305, 153)
        Me.pic_Start.Name = "pic_Start"
        Me.pic_Start.Size = New System.Drawing.Size(26, 26)
        Me.pic_Start.TabIndex = 7
        Me.pic_Start.TabStop = False
        Me.pic_Start.Visible = False
        '
        'pic_PwrOFF
        '
        Me.pic_PwrOFF.BackgroundImage = CType(resources.GetObject("pic_PwrOFF.BackgroundImage"), System.Drawing.Image)
        Me.pic_PwrOFF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.pic_PwrOFF.Location = New System.Drawing.Point(337, 161)
        Me.pic_PwrOFF.Name = "pic_PwrOFF"
        Me.pic_PwrOFF.Size = New System.Drawing.Size(18, 18)
        Me.pic_PwrOFF.TabIndex = 8
        Me.pic_PwrOFF.TabStop = False
        Me.pic_PwrOFF.Visible = False
        '
        'lbl_Status
        '
        Me.lbl_Status.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.lbl_Status.Location = New System.Drawing.Point(213, 95)
        Me.lbl_Status.Name = "lbl_Status"
        Me.lbl_Status.Size = New System.Drawing.Size(255, 19)
        Me.lbl_Status.TabIndex = 9
        Me.lbl_Status.Text = "Initializing..."
        Me.lbl_Status.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tmr_LblBlink
        '
        '
        'tmr_BlinkLED
        '
        '
        'tmr_IOScan
        '
        '
        'frm_HardwareInit
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(479, 226)
        Me.ControlBox = False
        Me.Controls.Add(Me.lbl_Status)
        Me.Controls.Add(Me.pic_PwrOFF)
        Me.Controls.Add(Me.pic_Start)
        Me.Controls.Add(Me.pic_Stop)
        Me.Controls.Add(Me.btn_Cmd)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "frm_HardwareInit"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic_Stop, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic_Start, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic_PwrOFF, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btn_Cmd As System.Windows.Forms.Button
    Friend WithEvents pic_Stop As System.Windows.Forms.PictureBox
    Friend WithEvents pic_Start As System.Windows.Forms.PictureBox
    Friend WithEvents pic_PwrOFF As System.Windows.Forms.PictureBox
    Friend WithEvents lbl_Status As System.Windows.Forms.Label
    Friend WithEvents tmr_LblBlink As System.Windows.Forms.Timer
    Friend WithEvents tmr_BlinkLED As System.Windows.Forms.Timer
    Friend WithEvents tmr_IOScan As System.Windows.Forms.Timer
End Class
