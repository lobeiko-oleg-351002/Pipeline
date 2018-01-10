namespace AdminClient.Forms
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.служебныеОбъектыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.статусыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.пользователиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.аттрибутыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.типыСобытийToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.группыПользователейToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.служебныеОбъектыToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(716, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // служебныеОбъектыToolStripMenuItem
            // 
            this.служебныеОбъектыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.статусыToolStripMenuItem,
            this.пользователиToolStripMenuItem,
            this.аттрибутыToolStripMenuItem,
            this.типыСобытийToolStripMenuItem,
            this.группыПользователейToolStripMenuItem});
            this.служебныеОбъектыToolStripMenuItem.Name = "служебныеОбъектыToolStripMenuItem";
            this.служебныеОбъектыToolStripMenuItem.Size = new System.Drawing.Size(134, 20);
            this.служебныеОбъектыToolStripMenuItem.Text = "Служебные объекты";
            // 
            // статусыToolStripMenuItem
            // 
            this.статусыToolStripMenuItem.Name = "статусыToolStripMenuItem";
            this.статусыToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.статусыToolStripMenuItem.Text = "Статусы";
            this.статусыToolStripMenuItem.Click += new System.EventHandler(this.статусыToolStripMenuItem_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(482, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(234, 20);
            this.textBox1.TabIndex = 3;
            // 
            // пользователиToolStripMenuItem
            // 
            this.пользователиToolStripMenuItem.Name = "пользователиToolStripMenuItem";
            this.пользователиToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.пользователиToolStripMenuItem.Text = "Пользователи";
            this.пользователиToolStripMenuItem.Click += new System.EventHandler(this.пользователиToolStripMenuItem_Click);
            // 
            // аттрибутыToolStripMenuItem
            // 
            this.аттрибутыToolStripMenuItem.Name = "аттрибутыToolStripMenuItem";
            this.аттрибутыToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.аттрибутыToolStripMenuItem.Text = "Атрибуты";
            this.аттрибутыToolStripMenuItem.Click += new System.EventHandler(this.аттрибутыToolStripMenuItem_Click);
            // 
            // типыСобытийToolStripMenuItem
            // 
            this.типыСобытийToolStripMenuItem.Name = "типыСобытийToolStripMenuItem";
            this.типыСобытийToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.типыСобытийToolStripMenuItem.Text = "Типы событий";
            this.типыСобытийToolStripMenuItem.Click += new System.EventHandler(this.типыСобытийToolStripMenuItem_Click);
            // 
            // группыПользователейToolStripMenuItem
            // 
            this.группыПользователейToolStripMenuItem.Name = "группыПользователейToolStripMenuItem";
            this.группыПользователейToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.группыПользователейToolStripMenuItem.Text = "Группы пользователей";
            this.группыПользователейToolStripMenuItem.Click += new System.EventHandler(this.группыПользователейToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 451);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem служебныеОбъектыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem статусыToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem пользователиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem аттрибутыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem типыСобытийToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem группыПользователейToolStripMenuItem;
    }
}