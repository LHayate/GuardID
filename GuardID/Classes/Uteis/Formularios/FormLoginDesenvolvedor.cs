﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Uteis;

namespace System.Uteis
{
    public partial class FormLoginDesenvolvedor : FormBasic
    {
        public string Banco = string.Empty;
        public FormLoginDesenvolvedor()
        {
            InitializeComponent();
            rbtTeste.Checked = true;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            if (rbtProducao.Checked)
                Banco = "ACAD";
            else if (rdtAcadDR.Checked)
                Banco = "ACADDR";
            else
                Banco = "ACAD_TESTE";
            this.Dispose();
        }

        private void btConfirma_Click(object sender, EventArgs e)
        {
            btOk_Click(rbtTeste, e);
        }

        private void FormLoginDesenvolvedor_Load(object sender, EventArgs e)
        {

        }
    }
}
