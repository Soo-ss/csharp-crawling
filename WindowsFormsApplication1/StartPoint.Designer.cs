namespace WindowsFormsApplication1
{
    partial class StartPoint
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.endDate = new System.Windows.Forms.DateTimePicker();
            this.startDate = new System.Windows.Forms.DateTimePicker();
            this.submit = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.inputName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.instagramRadioButton = new System.Windows.Forms.RadioButton();
            this.clienRadioButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.stopBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // endDate
            // 
            this.endDate.Location = new System.Drawing.Point(203, 187);
            this.endDate.Name = "endDate";
            this.endDate.Size = new System.Drawing.Size(200, 21);
            this.endDate.TabIndex = 19;
            // 
            // startDate
            // 
            this.startDate.Location = new System.Drawing.Point(203, 137);
            this.startDate.Name = "startDate";
            this.startDate.Size = new System.Drawing.Size(200, 21);
            this.startDate.TabIndex = 18;
            // 
            // submit
            // 
            this.submit.Location = new System.Drawing.Point(470, 187);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(75, 23);
            this.submit.TabIndex = 17;
            this.submit.Text = "검색";
            this.submit.UseVisualStyleBackColor = true;
            this.submit.Click += new System.EventHandler(this.SubmitClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 187);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "끝나는 날짜 (년/월/일)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 12);
            this.label2.TabIndex = 15;
            this.label2.Text = "시작날짜 (년/월/일)";
            // 
            // inputName
            // 
            this.inputName.Location = new System.Drawing.Point(203, 43);
            this.inputName.Name = "inputName";
            this.inputName.Size = new System.Drawing.Size(100, 21);
            this.inputName.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "Tag";
            // 
            // instagramRadioButton
            // 
            this.instagramRadioButton.AutoSize = true;
            this.instagramRadioButton.Checked = true;
            this.instagramRadioButton.Location = new System.Drawing.Point(15, 19);
            this.instagramRadioButton.Name = "instagramRadioButton";
            this.instagramRadioButton.Size = new System.Drawing.Size(79, 16);
            this.instagramRadioButton.TabIndex = 20;
            this.instagramRadioButton.TabStop = true;
            this.instagramRadioButton.Text = "instagram";
            this.instagramRadioButton.UseVisualStyleBackColor = true;
            // 
            // clienRadioButton
            // 
            this.clienRadioButton.AutoSize = true;
            this.clienRadioButton.Location = new System.Drawing.Point(120, 19);
            this.clienRadioButton.Name = "clienRadioButton";
            this.clienRadioButton.Size = new System.Drawing.Size(50, 16);
            this.clienRadioButton.TabIndex = 21;
            this.clienRadioButton.Text = "clien";
            this.clienRadioButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.instagramRadioButton);
            this.panel1.Controls.Add(this.clienRadioButton);
            this.panel1.Location = new System.Drawing.Point(470, 46);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(231, 64);
            this.panel1.TabIndex = 22;
            // 
            // logBox
            // 
            this.logBox.Font = new System.Drawing.Font("JetBrains Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logBox.Location = new System.Drawing.Point(48, 229);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(970, 342);
            this.logBox.TabIndex = 23;
            this.logBox.Text = "";
            // 
            // stopBtn
            // 
            this.stopBtn.Location = new System.Drawing.Point(943, 577);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(75, 23);
            this.stopBtn.TabIndex = 24;
            this.stopBtn.Text = "검색 중단";
            this.stopBtn.UseVisualStyleBackColor = true;
            this.stopBtn.Click += new System.EventHandler(this.StopClick);
            // 
            // StartPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1047, 612);
            this.Controls.Add(this.stopBtn);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.endDate);
            this.Controls.Add(this.startDate);
            this.Controls.Add(this.submit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.inputName);
            this.Controls.Add(this.label1);
            this.Name = "StartPoint";
            this.Text = "Instagram";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker endDate;
        private System.Windows.Forms.DateTimePicker startDate;
        private System.Windows.Forms.Button submit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox inputName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton instagramRadioButton;
        private System.Windows.Forms.RadioButton clienRadioButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.Button stopBtn;
    }
}