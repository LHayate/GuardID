﻿using System.Windows.Forms;

namespace System.Uteis
{
    partial class FormLoginDesenvolvedor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLoginDesenvolvedor));
            this.btOk = new System.Windows.Forms.PictureBox();
            this.rbtProducao = new System.Windows.Forms.RadioButton();
            this.rbtTeste = new System.Windows.Forms.RadioButton();
            this.btConfirma = new System.Windows.Forms.Button();
            this.rdtAcadDR = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.btOk)).BeginInit();
            this.SuspendLayout();
            // 
            // btOk
            // 
            this.btOk.Image = ((System.Drawing.Image)(resources.GetObject("btOk.Image")));
            this.btOk.Location = new System.Drawing.Point(358, 328);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(84, 28);
            this.btOk.TabIndex = 7;
            this.btOk.TabStop = false;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // rbtProducao
            // 
            this.rbtProducao.AutoSize = true;
            this.rbtProducao.BackColor = System.Drawing.Color.Transparent;
            this.rbtProducao.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtProducao.Location = new System.Drawing.Point(357, 262);
            this.rbtProducao.Name = "rbtProducao";
            this.rbtProducao.Size = new System.Drawing.Size(76, 19);
            this.rbtProducao.TabIndex = 1;
            this.rbtProducao.TabStop = true;
            this.rbtProducao.Text = "Produção";
            this.rbtProducao.UseVisualStyleBackColor = false;
            // 
            // rbtTeste
            // 
            this.rbtTeste.AutoSize = true;
            this.rbtTeste.BackColor = System.Drawing.Color.Transparent;
            this.rbtTeste.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtTeste.Location = new System.Drawing.Point(292, 262);
            this.rbtTeste.Name = "rbtTeste";
            this.rbtTeste.Size = new System.Drawing.Size(54, 19);
            this.rbtTeste.TabIndex = 0;
            this.rbtTeste.TabStop = true;
            this.rbtTeste.Text = "Teste";
            this.rbtTeste.UseVisualStyleBackColor = false;
            // 
            // btConfirma
            // 
            this.btConfirma.Location = new System.Drawing.Point(360, 330);
            this.btConfirma.Name = "btConfirma";
            this.btConfirma.Size = new System.Drawing.Size(75, 23);
            this.btConfirma.TabIndex = 3;
            this.btConfirma.Text = "OK";
            this.btConfirma.UseVisualStyleBackColor = true;
            this.btConfirma.Click += new System.EventHandler(this.btConfirma_Click);
            // 
            // rdtAcadDR
            // 
            this.rdtAcadDR.AutoSize = true;
            this.rdtAcadDR.BackColor = System.Drawing.Color.Transparent;
            this.rdtAcadDR.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdtAcadDR.Location = new System.Drawing.Point(445, 262);
            this.rdtAcadDR.Name = "rdtAcadDR";
            this.rdtAcadDR.Size = new System.Drawing.Size(74, 19);
            this.rdtAcadDR.TabIndex = 2;
            this.rdtAcadDR.TabStop = true;
            this.rdtAcadDR.Text = "ACAD DR";
            this.rdtAcadDR.UseVisualStyleBackColor = false;
            // 
            // FormLoginDesenvolvedor
            // 
            this.AcceptButton = this.btConfirma;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.rdtAcadDR);
            this.Controls.Add(this.rbtTeste);
            this.Controls.Add(this.rbtProducao);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.btConfirma);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLoginDesenvolvedor";
            this.Load += new System.EventHandler(this.FormLoginDesenvolvedor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btOk)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox btOk;
        private System.Windows.Forms.RadioButton rbtProducao;
        private System.Windows.Forms.RadioButton rbtTeste;
        private System.Windows.Forms.Button btConfirma;
        private RadioButton rdtAcadDR;
    }
}