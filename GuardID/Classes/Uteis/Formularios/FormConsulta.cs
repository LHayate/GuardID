﻿using Classes.Autenticacoes;
using System.Windows.Forms;

namespace System.Uteis
{
	public partial class FormConsulta : FormBasic
    {
        public FormConsulta()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Construtor com bloqueio para múltiplas instâncias do formulário
        /// </summary>
        /// <param name="frmP">Formulário principal do projeto.</param>
        public FormConsulta(Form frmP):base(frmP)
        {
            InitializeComponent();
        }

        protected void LiberaTelaPermissao()
        {
            if (string.IsNullOrEmpty(this.CodigoSeguranca))
                toolStripBtnPermissao.Visible = false;
            else
                toolStripBtnPermissao.Visible = true;
        }

        private void FormConsulta_Load(object sender, EventArgs e)
        {
            LiberaTelaPermissao();
        }

        private void toolStripBtnBusca_Click(object sender, EventArgs e)
        {
            toolStripOpcoes.Focus();
        }

        private void toolStripBtnLimpar_Click(object sender, EventArgs e)
        {
            toolStripOpcoes.Focus();
        }

        private void toolStripBtnDetalhar_Click(object sender, EventArgs e)
        {
            toolStripOpcoes.Focus();
        }

        private void toolStripBtnExcel_Click(object sender, EventArgs e)
        {
            toolStripOpcoes.Focus();
        }

        private void toolStripBtnPermissao_Click(object sender, EventArgs e)
        {
            toolStripOpcoes.Focus();

            frmManutencaoPermissoes frmMP = new frmManutencaoPermissoes();
            frmMP.UsuarioLogado = Globals.Usuario;
            frmMP.CodPrograma = this.CodigoSeguranca;
            frmMP.ShowDialog();
        }
    }
}
