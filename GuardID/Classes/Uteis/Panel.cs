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
    [ToolboxBitmap(@"iPanel.ico")]
    public partial class PanelGuard : Panel
    {
        public PanelGuard()
        {
            InitializeComponent();
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        public PanelGuard(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
