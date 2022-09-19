namespace WinFormsBlink
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
            this.components = new System.ComponentModel.Container();
            this.FaceMatchStatus = new System.Windows.Forms.GroupBox();
            this.img = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.FaceMatchStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.img)).BeginInit();
            this.SuspendLayout();
            // 
            // FaceMatchStatus
            // 
            this.FaceMatchStatus.Controls.Add(this.img);
            this.FaceMatchStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.FaceMatchStatus.Location = new System.Drawing.Point(0, 89);
            this.FaceMatchStatus.Name = "FaceMatchStatus";
            this.FaceMatchStatus.Size = new System.Drawing.Size(705, 448);
            this.FaceMatchStatus.TabIndex = 0;
            this.FaceMatchStatus.TabStop = false;
            this.FaceMatchStatus.Text = "Save Path \"./Resource\"";
            this.FaceMatchStatus.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // img
            // 
            this.img.Dock = System.Windows.Forms.DockStyle.Fill;
            this.img.Location = new System.Drawing.Point(3, 23);
            this.img.Name = "img";
            this.img.Size = new System.Drawing.Size(699, 422);
            this.img.TabIndex = 1;
            this.img.TabStop = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 29);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(246, 35);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 29);
            this.button2.TabIndex = 2;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 537);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.FaceMatchStatus);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FaceMatchStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.img)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox FaceMatchStatus;
        private System.Windows.Forms.PictureBox img;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
