﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Uteis
{
    [ToolboxBitmap(@"iButton.ico")]
    public partial class ButtonGuard : Button
    {
        public ButtonGuard()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(195,217,241);
            this.FlatStyle = FlatStyle.Flat;
            this.ImageAlign = ContentAlignment.MiddleLeft;
            this.UseVisualStyleBackColor = true;
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
