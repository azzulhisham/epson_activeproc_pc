<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_AutoRun
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frm_AutoRun))
        Me.pic_Progress = New System.Windows.Forms.PictureBox()
        Me.lbl_Progress = New System.Windows.Forms.Label()
        Me.lbl_Elapsed = New System.Windows.Forms.Label()
        Me.pic_1 = New System.Windows.Forms.PictureBox()
        Me.pic_2 = New System.Windows.Forms.PictureBox()
        Me.pic_3 = New System.Windows.Forms.PictureBox()
        Me.pic_4 = New System.Windows.Forms.PictureBox()
        Me.pic_5 = New System.Windows.Forms.PictureBox()
        Me.tmr_Animate = New System.Windows.Forms.Timer(Me.components)
        Me.tmr_AutoRun = New System.Windows.Forms.Timer(Me.components)
        Me.lbl_ProcNo = New System.Windows.Forms.Label()
        CType(Me.pic_Progress, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic_1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic_2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic_3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic_4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pic_5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pic_Progress
        '
        Me.pic_Progress.BackgroundImage = CType(resources.GetObject("pic_Progress.BackgroundImage"), System.Drawing.Image)
        Me.pic_Progress.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pic_Progress.Location = New System.Drawing.Point(-7, 1)
        Me.pic_Progress.Name = "pic_Progress"
        Me.pic_Progress.Size = New System.Drawing.Size(125, 129)
        Me.pic_Progress.TabIndex = 0
        Me.pic_Progress.TabStop = False
        '
        'lbl_Progress
        '
        Me.lbl_Progress.AutoSize = True
        Me.lbl_Progress.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_Progress.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lbl_Progress.Location = New System.Drawing.Point(140, 44)
        Me.lbl_Progress.Name = "lbl_Progress"
        Me.lbl_Progress.Size = New System.Drawing.Size(133, 16)
        Me.lbl_Progress.TabIndex = 1
        Me.lbl_Progress.Text = "Marking in progress..."
        '
        'lbl_Elapsed
        '
        Me.lbl_Elapsed.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_Elapsed.ForeColor = System.Drawing.Color.Gray
        Me.lbl_Elapsed.Location = New System.Drawing.Point(219, 100)
        Me.lbl_Elapsed.Name = "lbl_Elapsed"
        Me.lbl_Elapsed.Size = New System.Drawing.Size(138, 22)
        Me.lbl_Elapsed.TabIndex = 2
        Me.lbl_Elapsed.Text = "0 sec elapsed."
        Me.lbl_Elapsed.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'pic_1
        '
        Me.pic_1.BackgroundImage = CType(resources.GetObject("pic_1.BackgroundImage"), System.Drawing.Image)
        Me.pic_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pic_1.Location = New System.Drawing.Point(138, 5)
        Me.pic_1.Name = "pic_1"
        Me.pic_1.Size = New System.Drawing.Size(29, 30)
        Me.pic_1.TabIndex = 3
        Me.pic_1.TabStop = False
        Me.pic_1.Visible = False
        '
        'pic_2
        '
        Me.pic_2.BackgroundImage = CType(resources.GetObject("pic_2.BackgroundImage"), System.Drawing.Image)
        Me.pic_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pic_2.Location = New System.Drawing.Point(173, 5)
        Me.pic_2.Name = "pic_2"
        Me.pic_2.Size = New System.Drawing.Size(29, 30)
        Me.pic_2.TabIndex = 4
        Me.pic_2.TabStop = False
        Me.pic_2.Visible = False
        '
        'pic_3
        '
        Me.pic_3.BackgroundImage = CType(resources.GetObject("pic_3.BackgroundImage"), System.Drawing.Image)
        Me.pic_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pic_3.Location = New System.Drawing.Point(208, 5)
        Me.pic_3.Name = "pic_3"
        Me.pic_3.Size = New System.Drawing.Size(29, 30)
        Me.pic_3.TabIndex = 5
        Me.pic_3.TabStop = False
        Me.pic_3.Visible = False
        '
        'pic_4
        '
        Me.pic_4.BackgroundImage = CType(resources.GetObject("pic_4.BackgroundImage"), System.Drawing.Image)
        Me.pic_4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pic_4.Location = New System.Drawing.Point(243, 5)
        Me.pic_4.Name = "pic_4"
        Me.pic_4.Size = New System.Drawing.Size(29, 30)
        Me.pic_4.TabIndex = 6
        Me.pic_4.TabStop = False
        Me.pic_4.Visible = False
        '
        'pic_5
        '
        Me.pic_5.BackgroundImage = CType(resources.GetObject("pic_5.BackgroundImage"), System.Drawing.Image)
        Me.pic_5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pic_5.Location = New System.Drawing.Point(278, 5)
        Me.pic_5.Name = "pic_5"
        Me.pic_5.Size = New System.Drawing.Size(29, 30)
        Me.pic_5.TabIndex = 7
        Me.pic_5.TabStop = False
        Me.pic_5.Visible = False
        '
        'tmr_Animate
        '
        '
        'tmr_AutoRun
        '
        '
        'lbl_ProcNo
        '
        Me.lbl_ProcNo.AutoSize = True
        Me.lbl_ProcNo.ForeColor = System.Drawing.Color.Silver
        Me.lbl_ProcNo.Location = New System.Drawing.Point(140, 65)
        Me.lbl_ProcNo.Name = "lbl_ProcNo"
        Me.lbl_ProcNo.Size = New System.Drawing.Size(0, 14)
        Me.lbl_ProcNo.TabIndex = 8
        '
        'frm_AutoRun
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(360, 120)
        Me.ControlBox = False
        Me.Controls.Add(Me.lbl_ProcNo)
        Me.Controls.Add(Me.pic_5)
        Me.Controls.Add(Me.pic_4)
        Me.Controls.Add(Me.pic_3)
        Me.Controls.Add(Me.pic_2)
        Me.Controls.Add(Me.pic_1)
        Me.Controls.Add(Me.lbl_Elapsed)
        Me.Controls.Add(Me.lbl_Progress)
        Me.Controls.Add(Me.pic_Progress)
        Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "frm_AutoRun"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        CType(Me.pic_Progress, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic_1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic_2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic_3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic_4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pic_5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pic_Progress As System.Windows.Forms.PictureBox
    Friend WithEvents lbl_Progress As System.Windows.Forms.Label
    Friend WithEvents lbl_Elapsed As System.Windows.Forms.Label
    Friend WithEvents pic_1 As System.Windows.Forms.PictureBox
    Friend WithEvents pic_2 As System.Windows.Forms.PictureBox
    Friend WithEvents pic_3 As System.Windows.Forms.PictureBox
    Friend WithEvents pic_4 As System.Windows.Forms.PictureBox
    Friend WithEvents pic_5 As System.Windows.Forms.PictureBox
    Friend WithEvents tmr_Animate As System.Windows.Forms.Timer
    Friend WithEvents tmr_AutoRun As System.Windows.Forms.Timer
    Friend WithEvents lbl_ProcNo As System.Windows.Forms.Label
End Class
