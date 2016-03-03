namespace OntologyEditor
{
    partial class DiagramBox
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.информацияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.наПереднийПланToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.наЗаднийПланToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.свернутьВУзелToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.развернутьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.информацияToolStripMenuItem,
            this.toolStripSeparator1,
            this.свернутьВУзелToolStripMenuItem,
            this.развернутьToolStripMenuItem,
            this.toolStripSeparator2,
            this.наПереднийПланToolStripMenuItem,
            this.наЗаднийПланToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(176, 148);
            // 
            // информацияToolStripMenuItem
            // 
            this.информацияToolStripMenuItem.Name = "информацияToolStripMenuItem";
            this.информацияToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.информацияToolStripMenuItem.Text = "Информация";
            this.информацияToolStripMenuItem.Click += new System.EventHandler(this.информацияToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(172, 6);
            // 
            // наПереднийПланToolStripMenuItem
            // 
            this.наПереднийПланToolStripMenuItem.Name = "наПереднийПланToolStripMenuItem";
            this.наПереднийПланToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.наПереднийПланToolStripMenuItem.Text = "На передний план";
            this.наПереднийПланToolStripMenuItem.Click += new System.EventHandler(this.наПереднийПланToolStripMenuItem_Click);
            // 
            // наЗаднийПланToolStripMenuItem
            // 
            this.наЗаднийПланToolStripMenuItem.Name = "наЗаднийПланToolStripMenuItem";
            this.наЗаднийПланToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.наЗаднийПланToolStripMenuItem.Text = "На задний план";
            this.наЗаднийПланToolStripMenuItem.Click += new System.EventHandler(this.наЗаднийПланToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox10);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(255, 104);
            this.panel1.TabIndex = 3;
            this.panel1.Visible = false;
            // 
            // textBox10
            // 
            this.textBox10.BackColor = System.Drawing.SystemColors.Info;
            this.textBox10.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox10.Location = new System.Drawing.Point(0, 0);
            this.textBox10.Multiline = true;
            this.textBox10.Name = "textBox10";
            this.textBox10.ReadOnly = true;
            this.textBox10.Size = new System.Drawing.Size(255, 104);
            this.textBox10.TabIndex = 0;
            this.textBox10.TabStop = false;
            this.textBox10.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBox10_MouseDown);
            this.textBox10.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textBox10_MouseMove);
            this.textBox10.MouseUp += new System.Windows.Forms.MouseEventHandler(this.textBox10_MouseUp);
            // 
            // свернутьВУзелToolStripMenuItem
            // 
            this.свернутьВУзелToolStripMenuItem.Name = "свернутьВУзелToolStripMenuItem";
            this.свернутьВУзелToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.свернутьВУзелToolStripMenuItem.Text = "Свернуть";
            this.свернутьВУзелToolStripMenuItem.Click += new System.EventHandler(this.свернутьВУзелToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(172, 6);
            // 
            // развернутьToolStripMenuItem
            // 
            this.развернутьToolStripMenuItem.Name = "развернутьToolStripMenuItem";
            this.развернутьToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.развернутьToolStripMenuItem.Text = "Развернуть";
            this.развернутьToolStripMenuItem.Click += new System.EventHandler(this.развернутьToolStripMenuItem_Click);
            // 
            // DiagramBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.panel1);
            this.Name = "DiagramBox";
            this.Size = new System.Drawing.Size(461, 233);
            this.Click += new System.EventHandler(this.DiagramBox_Click);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DiagramBox_KeyPress);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem информацияToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem наПереднийПланToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem наЗаднийПланToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.ToolStripMenuItem свернутьВУзелToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem развернутьToolStripMenuItem;
    }
}
