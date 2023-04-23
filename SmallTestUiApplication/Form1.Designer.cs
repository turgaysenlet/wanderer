namespace SmallTestUiApplication
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            timer1 = new System.Windows.Forms.Timer(components);
            imageBox1 = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)imageBox1).BeginInit();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // imageBox1
            // 
            imageBox1.Dock = DockStyle.Fill;
            imageBox1.Location = new Point(0, 0);
            imageBox1.Margin = new Padding(2);
            imageBox1.Name = "imageBox1";
            imageBox1.Size = new Size(862, 581);
            imageBox1.SizeMode = PictureBoxSizeMode.Zoom;
            imageBox1.TabIndex = 2;
            imageBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(862, 581);
            Controls.Add(imageBox1);
            Margin = new Padding(2);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            ((System.ComponentModel.ISupportInitialize)imageBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Emgu.CV.UI.ImageBox imageBox1;
    }
}