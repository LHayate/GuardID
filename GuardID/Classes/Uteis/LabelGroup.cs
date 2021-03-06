﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace System.Uteis
{
    [ToolboxBitmap(@"iLabelgroup.ico")]
    public partial class LabelGroupGuard : Label
    {                
        public LabelGroupGuard()
        {
            InitializeComponent();
            base.Font = new Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            base.BackColor = Color.FromArgb(195, 217, 241);
            base.BorderStyle = BorderStyle.FixedSingle;
            base.Padding = new Padding(1);
            base.AutoSize = false;
            base.Size = new Size(525, 22);
        }

        public LabelGroupGuard(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }
    }
}
