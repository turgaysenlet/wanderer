namespace Wanderer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.StartCameraButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.MapPictureBox = new System.Windows.Forms.PictureBox();
            this.MapTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.RenderControl = new OpenGL.GlControl();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).BeginInit();
            this.MapTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartCameraButton
            // 
            this.StartCameraButton.Location = new System.Drawing.Point(12, 12);
            this.StartCameraButton.Name = "StartCameraButton";
            this.StartCameraButton.Size = new System.Drawing.Size(149, 34);
            this.StartCameraButton.TabIndex = 0;
            this.StartCameraButton.Text = "Start Camera";
            this.StartCameraButton.UseVisualStyleBackColor = true;
            this.StartCameraButton.Click += new System.EventHandler(this.StartCameraButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(11, 52);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 480);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(657, 52);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(640, 480);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // MapPictureBox
            // 
            this.MapPictureBox.BackColor = System.Drawing.Color.White;
            this.MapPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MapPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapPictureBox.Location = new System.Drawing.Point(925, 3);
            this.MapPictureBox.Name = "MapPictureBox";
            this.MapPictureBox.Size = new System.Drawing.Size(916, 661);
            this.MapPictureBox.TabIndex = 3;
            this.MapPictureBox.TabStop = false;
            this.MapPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.MapPictureBox_Paint);
            this.MapPictureBox.Resize += new System.EventHandler(this.MapPictureBox_Resize);
            // 
            // MapTableLayoutPanel
            // 
            this.MapTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapTableLayoutPanel.ColumnCount = 2;
            this.MapTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MapTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MapTableLayoutPanel.Controls.Add(this.RenderControl, 0, 0);
            this.MapTableLayoutPanel.Controls.Add(this.MapPictureBox, 1, 0);
            this.MapTableLayoutPanel.Location = new System.Drawing.Point(12, 538);
            this.MapTableLayoutPanel.Name = "MapTableLayoutPanel";
            this.MapTableLayoutPanel.RowCount = 1;
            this.MapTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MapTableLayoutPanel.Size = new System.Drawing.Size(1844, 667);
            this.MapTableLayoutPanel.TabIndex = 5;
            // 
            // RenderControl
            // 
            this.RenderControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.RenderControl.ColorBits = ((uint)(24u));
            this.RenderControl.DepthBits = ((uint)(0u));
            this.RenderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderControl.Location = new System.Drawing.Point(5, 6);
            this.RenderControl.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.RenderControl.MultisampleBits = ((uint)(0u));
            this.RenderControl.Name = "RenderControl";
            this.RenderControl.Size = new System.Drawing.Size(912, 655);
            this.RenderControl.StencilBits = ((uint)(0u));
            this.RenderControl.TabIndex = 5;
            this.RenderControl.ContextCreated += new System.EventHandler<OpenGL.GlControlEventArgs>(this.RenderControl_ContextCreated);
            this.RenderControl.Render += new System.EventHandler<OpenGL.GlControlEventArgs>(this.RenderControl_Render);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(1303, 52);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.SelectedObject = this;
            this.propertyGrid1.Size = new System.Drawing.Size(553, 480);
            this.propertyGrid1.TabIndex = 6;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1865, 1217);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.MapTableLayoutPanel);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.StartCameraButton);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).EndInit();
            this.MapTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Button StartCameraButton;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox MapPictureBox;
        private TableLayoutPanel MapTableLayoutPanel;
        private OpenGL.GlControl RenderControl;
        private PropertyGrid propertyGrid1;
        private System.Windows.Forms.Timer timer1;
    }
}