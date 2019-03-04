<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Splat
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
        Me.PictureBox_Img = New System.Windows.Forms.PictureBox()
        Me.Timer_CloseForm = New System.Windows.Forms.Timer(Me.components)
        CType(Me.PictureBox_Img, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox_Img
        '
        Me.PictureBox_Img.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox_Img.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PictureBox_Img.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox_Img.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox_Img.Name = "PictureBox_Img"
        Me.PictureBox_Img.Size = New System.Drawing.Size(20, 20)
        Me.PictureBox_Img.TabIndex = 0
        Me.PictureBox_Img.TabStop = False
        '
        'Timer_CloseForm
        '
        Me.Timer_CloseForm.Interval = 5000
        '
        'Splat
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.AutoValidate = System.Windows.Forms.AutoValidate.Disable
        Me.BackColor = System.Drawing.Color.YellowGreen
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(20, 20)
        Me.ControlBox = False
        Me.Controls.Add(Me.PictureBox_Img)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Splat"
        Me.Opacity = 0.0R
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Splat"
        CType(Me.PictureBox_Img, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox_Img As System.Windows.Forms.PictureBox
    Friend WithEvents Timer_CloseForm As System.Windows.Forms.Timer

End Class
