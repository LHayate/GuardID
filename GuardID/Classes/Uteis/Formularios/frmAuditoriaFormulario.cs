﻿using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using Classes.Autenticacoes;
using Classes.Conexoes;

namespace System.Uteis
{
	public partial class frmAuditoriaFormulario : FormBasic
    {
        private string _tabela = string.Empty;
        private string _esquema = string.Empty;
        private string _chave = string.Empty;
        private string _camposChave = string.Empty;
        private string _tabelaLog = string.Empty;
        private string _detalhe { get; set; }

        public frmAuditoriaFormulario(string tabela, string esquema, string camposChave, string chave, string tabelaLog = "sga.vw_log_sga", string detalhe = "")
        {
            try
            {
                this._tabelaLog = tabelaLog;
                this._detalhe = detalhe;

                if (!string.IsNullOrEmpty(tabela))
                    _tabela = tabela;
                else
                    throw new Exception();

                if (!string.IsNullOrEmpty(esquema))
                    _esquema = esquema;
                else
                    throw new Exception();

                _chave = chave;
                if (!string.IsNullOrEmpty(camposChave))
                {
                    _camposChave = "(" + camposChave + ")";

                    InitializeComponent();

                    if (!string.IsNullOrEmpty(_tabela))
                    {
                        string[] descricao = new string[] { _tabela, _chave, _camposChave };

                        lbTabelaDescricao.Text = descricao[0];
                        txtChaveDescricao.Text = descricao[1];
                        lbExemplo.Text = descricao[2];
                    }
                }
                else
                    throw new Exception();


                if (!string.IsNullOrEmpty(_chave))
                    PreencheGridLog(_tabela, _chave, _esquema);

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(tabela) || string.IsNullOrEmpty(esquema) || string.IsNullOrEmpty(camposChave))
                    MessageBox.Show("Existem parâmetros a serem passados.\nPor gentileza entre em contato com o núcleo de TI.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Erro: " + ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
                this.Close();
            }
        }

        private void PreencheGridLog(string tabela, string chave, string esquema)
        {
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);

            string sel = string.Empty;
            var dtLog = new DataTable();

            int codigo = BuscaCodigoTabela(tabela, esquema);
            if (codigo != -1)
            {
                dal.AddParam("tabela", codigo);
                dal.AddParam("chave", chave);

                sel = @"";

                dtLog = dal.ExecuteQuery(sel);

                if (dtLog.Rows.Count > 0)
                {
                    dgvLog.DataSource = dtLog;
                    foreach (DataGridViewColumn col in dgvLog.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable; // desabilita a ordenação
                    }
                    dgvLog.Refresh();
                }
                else
                {
                    txtChaveDescricao.Enabled = true;
                    txtChaveDescricao.Focus();
                }
            }
            else
            {
                MessageBox.Show(this, "Log indisponível! Contate a equipe de desenvolvimento da DTI!", Globals.TituloSistema, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CriaColunasDgvDetalhe(int acao)
        {
            if (acao > 0)
            {
                switch (acao)
                {
                    case 1:
                        {
                            dgvLogDetalhe.AutoGenerateColumns = false;
                            dgvLogDetalhe.DataSource = null;
                            dgvLogDetalhe.Refresh();

                            DataGridViewColumn column1 = new DataGridViewTextBoxColumn();
                            column1.DataPropertyName = "VALOR_INSERIDO";
                            column1.Name = "colValorInserido";
                            column1.HeaderText = "Valor Inserido";
                            column1.DisplayIndex = 0;
                            column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            column1.Width = 425;
                            column1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            column1.DefaultCellStyle.Font = new Font("verdana", 8);
                            column1.ReadOnly = true;
                            dgvLogDetalhe.Columns.Add(column1);


                            DataGridViewColumn column = new DataGridViewTextBoxColumn();
                            column.DataPropertyName = "CAMPO";
                            column.Name = "colCampo";
                            column.HeaderText = "Campo";
                            column.DisplayIndex = 0;
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            column.Width = 150;
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            column.DefaultCellStyle.Font = new Font("verdana", 8);
                            column.ReadOnly = true;
                            dgvLogDetalhe.Columns.Add(column);
                            break;
                        }
                    case 2:
                        {
                            dgvLogDetalhe.AutoGenerateColumns = false;
                            dgvLogDetalhe.DataSource = null;
                            dgvLogDetalhe.Refresh();

                            DataGridViewColumn column2 = new DataGridViewTextBoxColumn();
                            column2.DataPropertyName = "VALOR_ATUAL";
                            column2.Name = "colNovoValor";
                            column2.HeaderText = "Novo Valor";
                            column2.DisplayIndex = 0;
                            column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            column2.Width = 215;
                            column2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            column2.ReadOnly = true;
                            column2.DefaultCellStyle.Font = new Font("verdana", 8);

                            dgvLogDetalhe.Columns.Add(column2);

                            DataGridViewColumn column1 = new DataGridViewTextBoxColumn();
                            column1.DataPropertyName = "VALOR_ANTERIOR";
                            column1.Name = "colValorAnterior";
                            column1.HeaderText = "Valor Anterior";
                            column1.DisplayIndex = 0;
                            column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            column1.Width = 210;
                            column1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            column1.DefaultCellStyle.Font = new Font("verdana", 8);
                            column1.ReadOnly = true;
                            dgvLogDetalhe.Columns.Add(column1);

                            DataGridViewColumn column = new DataGridViewTextBoxColumn();
                            column.DataPropertyName = "CAMPO";
                            column.Name = "colCampo";
                            column.HeaderText = "Campo";
                            column.DisplayIndex = 0;
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            column.Width = 150;
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            column.DefaultCellStyle.Font = new Font("verdana", 8);
                            column.ReadOnly = true;
                            dgvLogDetalhe.Columns.Add(column);
                            break;
                        }
                    case 3:
                        {
                            dgvLogDetalhe.AutoGenerateColumns = false;
                            dgvLogDetalhe.DataSource = null;
                            dgvLogDetalhe.Refresh();

                            DataGridViewColumn column1 = new DataGridViewTextBoxColumn();
                            column1.DataPropertyName = "VALOR_EXCLUIDO";
                            column1.Name = "colValorExcluido";
                            column1.HeaderText = "Valor Excluido";
                            column1.DisplayIndex = 0;
                            column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            column1.Width = 425;
                            column1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            column1.DefaultCellStyle.Font = new Font("verdana", 8);
                            column1.ReadOnly = true;
                            dgvLogDetalhe.Columns.Add(column1);


                            DataGridViewColumn column = new DataGridViewTextBoxColumn();
                            column.DataPropertyName = "CAMPO";
                            column.Name = "colCampo";
                            column.HeaderText = "Campo";
                            column.DisplayIndex = 0;
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            column.Width = 150;
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            column.DefaultCellStyle.Font = new Font("verdana", 8);
                            column.ReadOnly = true;
                            dgvLogDetalhe.Columns.Add(column);
                            break;
                        }
                }
            }
        }

        private void DesmembraDetalhe(string detalhe, int acao)
        {
            if (acao > 0)
            {
                DataTable dtLogDetalhe = new DataTable();

                switch (acao)
                {
                    case 1:
                        {
                            dtLogDetalhe.Columns.Add("CAMPO", typeof(string));
                            dtLogDetalhe.Columns.Add("VALOR_INSERIDO", typeof(string));
                            DataRow drLogDetalhe = dtLogDetalhe.NewRow();

                            string colTabela = string.Empty;
                            string RowTabela = string.Empty;
                            string caracter = string.Empty;
                            string det = string.Empty;
                            string palavra = string.Empty;

                            ArrayList campos = new ArrayList();
                            string palavra1 = detalhe;
                            string p = string.Empty;
                            string c = string.Empty;

                            foreach (char x in palavra1)
                            {
                                c = string.Empty;
                                c = c + x;
                                if (c != "\r")
                                {
                                    if (c != "\n")
                                    {
                                        p = p + x;
                                    }
                                }
                                else
                                {
                                    campos.Add(p);
                                    p = string.Empty;
                                }
                            }

                            for (int i = 0; i < campos.Count; i++)
                            {
                                palavra = campos[i].ToString();
                                foreach (char s in palavra)
                                {
                                    det = string.Empty;
                                    det = det + s;
                                    if (caracter != ";" && det != ";")
                                    {
                                        colTabela = colTabela + s;
                                    }
                                    else
                                    {
                                        if (det == ";")
                                        {
                                            caracter = ";";
                                        }
                                        if (caracter == ";")
                                        {
                                            if (det != ";")
                                            {
                                                RowTabela = RowTabela + s;
                                            }
                                        }
                                    }
                                }
                                caracter = string.Empty;
                                drLogDetalhe["CAMPO"] = colTabela;
                                drLogDetalhe["VALOR_INSERIDO"] = RowTabela;
                                colTabela = string.Empty;
                                RowTabela = string.Empty;
                                dtLogDetalhe.Rows.Add(drLogDetalhe);
                                drLogDetalhe = dtLogDetalhe.NewRow();
                            }
                            break;
                        }
                    case 2:
                        {
                            dtLogDetalhe.Columns.Add("CAMPO", typeof(string));
                            dtLogDetalhe.Columns.Add("VALOR_ANTERIOR", typeof(string));
                            dtLogDetalhe.Columns.Add("VALOR_ATUAL", typeof(string));
                            DataRow drLogDetalhe = dtLogDetalhe.NewRow();

                            string colTabela = string.Empty;
                            string RowTabela = string.Empty;
                            string RowTabela1 = string.Empty;
                            string caracter = string.Empty;
                            string det = string.Empty;
                            string palavra = string.Empty;
                            int cont = 0;

                            ArrayList campos = new ArrayList();
                            string palavra1 = detalhe;
                            string p = string.Empty;
                            string c = string.Empty;

                            foreach (char x in palavra1)
                            {
                                c = string.Empty;
                                c = c + x;
                                if (c != "\r")
                                {
                                    if (c != "\n")
                                    {
                                        p = p + x;
                                    }
                                }
                                else
                                {
                                    campos.Add(p);
                                    p = string.Empty;
                                }
                            }

                            for (int i = 0; i < campos.Count; i++)
                            {
                                palavra = campos[i].ToString();
                                foreach (char s in palavra)
                                {
                                    det = string.Empty;
                                    det = det + s;
                                    if (caracter != ";" && det != ";")
                                    {
                                        colTabela = colTabela + s;
                                    }
                                    else
                                    {
                                        if (det == ";")
                                        {
                                            cont++;
                                            caracter = ";";
                                        }
                                        if (caracter == ";")
                                        {
                                            if (det != ";" && cont == 1)
                                            {
                                                RowTabela = RowTabela + s;
                                            }
                                            else
                                            {
                                                if (det != ";" && cont > 1)
                                                {
                                                    RowTabela1 = RowTabela1 + s;
                                                }
                                            }
                                        }
                                    }
                                }

                                caracter = string.Empty;
                                drLogDetalhe["CAMPO"] = colTabela;
                                drLogDetalhe["VALOR_ANTERIOR"] = RowTabela;
                                drLogDetalhe["VALOR_ATUAL"] = RowTabela1;
                                colTabela = string.Empty;
                                RowTabela = string.Empty;
                                RowTabela1 = string.Empty;
                                dtLogDetalhe.Rows.Add(drLogDetalhe);
                                drLogDetalhe = dtLogDetalhe.NewRow();
                                cont = 0;
                            }
                            break;
                        }
                    case 3:
                        {
                            dtLogDetalhe.Columns.Add("CAMPO", typeof(string));
                            dtLogDetalhe.Columns.Add("VALOR_EXCLUIDO", typeof(string));
                            DataRow drLogDetalhe = dtLogDetalhe.NewRow();

                            ArrayList campos = new ArrayList();
                            string palavra1 = detalhe;
                            string palavra = string.Empty;
                            string p = string.Empty;
                            string c = string.Empty;

                            foreach (char x in palavra1)
                            {
                                c = string.Empty;
                                c = c + x;
                                if (c != "\r")
                                {
                                    if (c != "\n")
                                    {
                                        p = p + x;
                                    }
                                }
                                else
                                {
                                    campos.Add(p);
                                    p = string.Empty;
                                }
                            }

                            string colTabela = string.Empty;
                            string RowTabela = string.Empty;
                            string caracter = string.Empty;
                            string det = string.Empty;

                            for (int i = 0; i < campos.Count; i++)
                            {
                                palavra = campos[i].ToString();
                                foreach (char s in palavra)
                                {
                                    det = string.Empty;
                                    det = det + s;
                                    if (caracter != ";" && det != ";")
                                    {
                                        colTabela = colTabela + s;
                                    }
                                    else
                                    {
                                        if (det == ";")
                                        {
                                            caracter = ";";
                                        }
                                        if (caracter == ";")
                                        {
                                            if (det != ";")
                                            {
                                                RowTabela = RowTabela + s;
                                            }
                                        }
                                    }

                                }
                                caracter = string.Empty;
                                drLogDetalhe["CAMPO"] = colTabela;
                                drLogDetalhe["VALOR_EXCLUIDO"] = RowTabela;
                                colTabela = string.Empty;
                                RowTabela = string.Empty;
                                dtLogDetalhe.Rows.Add(drLogDetalhe);
                                drLogDetalhe = dtLogDetalhe.NewRow();
                            }
                            break;
                        }
                }

                CriaColunasDgvDetalhe(acao);
                dgvLogDetalhe.DataSource = dtLogDetalhe;

                foreach (DataGridViewColumn col in dgvLogDetalhe.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable; // desabilita a ordenação
                }
                dgvLogDetalhe.Refresh();
            }
        }

        private int BuscaCodigoTabela(string tabela, string esquema)
        {
            int codTabela = -1;
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);
            string sel = string.Empty;
            var dtBuscaCodigo = new DataTable();

            dal.AddParam("tabela", tabela);
            dal.AddParam("esquema", esquema);

            sel = "";

            dtBuscaCodigo = dal.ExecuteQuery(sel);
            if (dtBuscaCodigo != null && dtBuscaCodigo.Rows.Count > 0)
                codTabela = int.Parse(dtBuscaCodigo.Rows[0]["TABELA"].ToString());
            return codTabela;
        }

        private void frm_auditoria_formulario_Load(object sender, EventArgs e)
        {
            if (dgvLog.Rows.Count > 0)
            {
                string detalhe = string.Empty;
                int acao = 0;

                detalhe = dgvLog.CurrentRow.Cells["colDetalhe"].Value.ToString();
                acao = int.Parse(dgvLog.CurrentRow.Cells["colNomeAcao"].Value.ToString());

                DesmembraDetalhe(detalhe, acao);
            }
        }

        private void dgvLog_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvLog.Rows.Count > 0)
            {
                string detalhe = string.Empty;
                int acao = 0;

                detalhe = dgvLog.CurrentRow.Cells["colDetalhe"].Value.ToString();
                acao = int.Parse(dgvLog.CurrentRow.Cells["colNomeAcao"].Value.ToString());

                DesmembraDetalhe(detalhe, acao);
            }
        }
        private void dgvLog_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (e.RowIndex % 2 == 0)

                    e.CellStyle.BackColor = Color.White;
                else
                    e.CellStyle.BackColor = Color.Lavender;
            }
        }
        private void dgvLogDetalhe_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (e.RowIndex % 2 == 0)

                    e.CellStyle.BackColor = Color.White;
                else
                    e.CellStyle.BackColor = Color.Lavender;
            }
        }
        private void dgvLog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 38 || e.KeyValue == 40)
            {
                if (dgvLog.Rows.Count > 0)
                {

                    frm_auditoria_formulario_Load(dgvLog, new EventArgs());
                }
            }
        }
        private void dgvLog_SelectionChanged(object sender, EventArgs e)
        {
            frm_auditoria_formulario_Load(dgvLog, new EventArgs());
        }

        private void txtChaveDescricao_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtChaveDescricao.Text))
                {
                    PreencheGridLog(_tabela, txtChaveDescricao.Text, _esquema);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
        }
        private void txtChaveDescricao_KeyDown(object sender, KeyEventArgs e)
        {
            ExecutaTabPressionaEnter(e);
        }
        private void txtChaveDescricao_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtChaveDescricao.Text))
            {
                LimpaCampos(this.Controls);
            }
        }

        private static void ExecutaTabPressionaEnter(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send((e.Shift ? "+" : "") + "{TAB}");
            }
        }

    }
}

