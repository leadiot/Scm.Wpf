namespace WinForm
{
    partial class HttpForm
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
            label1 = new Label();
            TbUrl = new TextBox();
            label2 = new Label();
            TbUser = new TextBox();
            label3 = new Label();
            TbTerminal = new TextBox();
            label4 = new Label();
            TbToken = new TextBox();
            TbResult = new TextBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(31, 17);
            label1.TabIndex = 0;
            label1.Text = "URL";
            // 
            // TbUrl
            // 
            TbUrl.Location = new Point(63, 12);
            TbUrl.Name = "TbUrl";
            TbUrl.Size = new Size(180, 23);
            TbUrl.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 44);
            label2.Name = "label2";
            label2.Size = new Size(45, 17);
            label2.TabIndex = 2;
            label2.Text = "用户ID";
            // 
            // TbUser
            // 
            TbUser.Location = new Point(63, 41);
            TbUser.Name = "TbUser";
            TbUser.Size = new Size(180, 23);
            TbUser.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 73);
            label3.Name = "label3";
            label3.Size = new Size(45, 17);
            label3.TabIndex = 4;
            label3.Text = "终端ID";
            // 
            // TbTerminal
            // 
            TbTerminal.Location = new Point(63, 70);
            TbTerminal.Name = "TbTerminal";
            TbTerminal.Size = new Size(180, 23);
            TbTerminal.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 102);
            label4.Name = "label4";
            label4.Size = new Size(44, 17);
            label4.TabIndex = 6;
            label4.Text = "Token";
            // 
            // TbToken
            // 
            TbToken.Location = new Point(63, 99);
            TbToken.Name = "TbToken";
            TbToken.Size = new Size(180, 23);
            TbToken.TabIndex = 7;
            // 
            // TbResult
            // 
            TbResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TbResult.Location = new Point(12, 128);
            TbResult.Multiline = true;
            TbResult.Name = "TbResult";
            TbResult.Size = new Size(637, 281);
            TbResult.TabIndex = 8;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(574, 415);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 9;
            button1.Text = "执行";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // HttpForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(661, 450);
            Controls.Add(button1);
            Controls.Add(TbResult);
            Controls.Add(TbToken);
            Controls.Add(label4);
            Controls.Add(TbTerminal);
            Controls.Add(label3);
            Controls.Add(TbUser);
            Controls.Add(label2);
            Controls.Add(TbUrl);
            Controls.Add(label1);
            Name = "HttpForm";
            Text = "HttpForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox TbUrl;
        private Label label2;
        private TextBox TbUser;
        private Label label3;
        private TextBox TbTerminal;
        private Label label4;
        private TextBox TbToken;
        private TextBox TbResult;
        private Button button1;
    }
}