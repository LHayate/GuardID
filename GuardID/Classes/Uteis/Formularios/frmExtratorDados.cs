﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Classes.Autenticacoes;
using Classes.Conexoes;
using Classes.Uteis;

namespace System.Uteis
{
	public partial class frmExtratorDados : FormProcesso
    {
        private int Sistema;

        public frmExtratorDados()
            : this(Globals.Sistema)
        {
        }
        public frmExtratorDados(int pSistema)
        {
            InitializeComponent();

            dgvFiltros.DataSource = Classes.Uteis.Diversos.ConverteGridEmDataTable(dgvFiltros);

            this.dgvFiltros.CellMouseEnter += delegate(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex > -1 && e.ColumnIndex == dgvFiltros.Columns["colExcluir"].Index)
                {
                    dgvFiltros.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightCoral;
                    dgvFiltros.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.LightCoral;
                }
            };
            this.dgvFiltros.CellMouseLeave += delegate(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex > -1 && e.ColumnIndex == dgvFiltros.Columns["colExcluir"].Index)
                {
                    dgvFiltros.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                    dgvFiltros.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.SelectionBackColor = Color.White;
                }
            };

            if (pSistema == 0)
            {
                MessageBox.Show("Parâmetro 'Sistema' não informado. Contate o Administrador do Sistema para verificar o problema.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
            this.Sistema = pSistema;

        }

        private struct ParametrosFiltro
        {
            public string NOME_CAMPO,
                          TIPO_FILTRO,
                          OBRIGATORIO,
                          TIPO_VALOR,
                          CONTEM_PESQUISA,
                          POSICAO_FILTRO,
                          NOME_CAMPO_FILTRO;
        }

        private bool ValidaAdicionarFiltro()
        {
            if (!txtFiltro.ContemValor())
            {
                MessageBox.Show("Informe o Filtro.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else if ((txtValorFiltro.Visible && !txtValorFiltro.ContemValor()) ||
                    (mtxtPeriodoInicio.Visible && mtxtPeriodoFim.Visible && (!mtxtPeriodoInicio.MaskFull || !mtxtPeriodoFim.MaskFull)) ||
                    (mtxtPeriodoInicio.Visible && !mtxtPeriodoFim.Visible && !mtxtPeriodoInicio.MaskFull) ||
                    (txtValorFiltroSemPesquisa.Visible && !txtValorFiltroSemPesquisa.ContemValor()) ||
                    (mtxtCompetenciaInicial.Visible && !mtxtCompetenciaFinal.Visible && !mtxtCompetenciaInicial.MaskFull) ||
                    (mtxtCompetenciaInicial.Visible && mtxtCompetenciaFinal.Visible && (!mtxtCompetenciaInicial.MaskFull || !mtxtCompetenciaFinal.MaskFull)))
            {
                MessageBox.Show("Informe os Valores dos Filtros corretamente.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (((ParametrosFiltro)txtFiltro.Tag).TIPO_FILTRO.Equals("4") && dgvFiltros.DataSource != null) // Parâmetro fixo
            {
                var a = from l in ((DataTable)dgvFiltros.DataSource).AsEnumerable()
                        where l.Field<string>("FILTRO").Equals(txtFiltro.Text)
                        select l.Field<string>("FILTRO");
                if (a.Count() > 0)
                {
                    MessageBox.Show("Este filtro só pode ser inserido uma vez.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            return true;
        }
        private bool ValidaGerarPlanilha()
        {
            string filtrosFaltando = "";

            var filtrosObrigatorios = from l in BuscaExtratorDadosComandosFiltros(txtComando.Text, null, this.Sistema).AsEnumerable()
                                      where l.Field<decimal>("OBRIGATORIO") == 1
                                      select new { filtro = l.Field<decimal>("FILTRO"), filtro_desc = l.Field<string>("FILTRO_DESC") };

            foreach (var f in filtrosObrigatorios)
            {
                var r = from l in ((DataTable)dgvFiltros.DataSource).AsEnumerable()
                        where l.Field<string>("FILTRO") == f.filtro.ToString()
                        select l.Field<string>("FILTRO_DESC");

                if (r.Count() == 0)
                    filtrosFaltando += f.filtro.ToString() + " - " + f.filtro_desc.ToString() + "\n";
            }

            if ((txtValorFiltro.Visible && txtValorFiltro.ContemValor()) ||
                    (mtxtPeriodoInicio.Visible && mtxtPeriodoFim.Visible && (mtxtPeriodoInicio.MaskFull || mtxtPeriodoFim.MaskFull)) ||
                    (mtxtPeriodoInicio.Visible && !mtxtPeriodoFim.Visible && mtxtPeriodoInicio.MaskFull) ||
                    (txtValorFiltroSemPesquisa.Visible && txtValorFiltroSemPesquisa.ContemValor()) ||
                    (mtxtCompetenciaInicial.Visible && !mtxtCompetenciaFinal.Visible && mtxtCompetenciaInicial.MaskFull) ||
                    (mtxtCompetenciaInicial.Visible && mtxtCompetenciaFinal.Visible && (mtxtCompetenciaInicial.MaskFull || mtxtCompetenciaFinal.MaskFull)))
            {
                MessageBox.Show("Existe um filtro preenchido e não adicionado. Limpe o campo ou adicione o filtro para continuar.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else if (!string.IsNullOrEmpty(filtrosFaltando))
            {
                MessageBox.Show("Os seguintes filtros são obrigatórios e não foram informados: \n\n" + filtrosFaltando, "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        }
        private bool ValidarCadastroFiltro(DataTable dt)
        {
            //-- tipo_valor
            //1-Inteiro, 2-Data Única 3- Data Período, 4-String, 5-Competência, 6-Período de Competência

            //-- tipo_filtro
            //1- in, 2 - not in, 3 - between

            if (((dt.Rows[0]["TIPO_VALOR"].ToString().Equals("3") || dt.Rows[0]["TIPO_VALOR"].ToString().Equals("6"))
                  && (dt.Rows[0]["TIPO_FILTRO"].ToString().Equals("1") || dt.Rows[0]["TIPO_FILTRO"].ToString().Equals("2")))
                ||
                ((dt.Rows[0]["TIPO_VALOR"].ToString().Equals("1") || dt.Rows[0]["TIPO_VALOR"].ToString().Equals("2")
                   /*|| dt.Rows[0]["TIPO_VALOR"].ToString().Equals("4") */|| dt.Rows[0]["TIPO_VALOR"].ToString().Equals("5"))
                   && dt.Rows[0]["TIPO_FILTRO"].ToString().Equals("3")))
            {
                MessageBox.Show("Filtro cadastrado com uma combinação de Tipo de Valor e Tipo de Filtro inválida. Contate o Administrador do Sistema para verificar o problema.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (!new List<string>() { "1", "2", "3", "4", "5","7","8" }.Contains(dt.Rows[0]["TIPO_FILTRO"].ToString()))
            {
                MessageBox.Show("Relaçao de Filtros por Comando cadastrada com um Tipo de Filtro inválido. Contate o Administrador do Sistema para verificar o problema.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private List<OracleParameter> AplicarFiltrosRetornarParametros(ref string pSQL)
        {
            List<OracleParameter> lParametros = new List<OracleParameter>();

            DataTable dt = ((DataTable)dgvFiltros.DataSource).Copy();
            dt.DefaultView.Sort = "TIPO_FILTRO, FILTRO, VALOR";
            dt = dt.DefaultView.ToTable(); // filtros adicionados ordenadamente

            // Buscar o Distinct de cada filtro
            List<string> lFiltrosDistinct = dt.Rows.Cast<DataRow>()
                                            .Select(dr => dr["FILTRO"].ToString())
                                            .Distinct().ToList();

            // cada filtro é tratado separadamente na where
            // exemplo: and (filtro1 in (&p1, &p2)) and (filtro2 not in (&p3, &p4))
            for (int a = 0; a < lFiltrosDistinct.Count; a++)
            {
                // busca os valores adicionados para o Filtro do for 
                var f = from l in dt.AsEnumerable()
                        where l.Field<string>("FILTRO") == lFiltrosDistinct[a]
                        select new
                        {
                            TIPO_FILTRO = l.Field<string>("TIPO_FILTRO"),
                            TIPO_VALOR = l.Field<string>("TIPO_VALOR"),
                            VALOR = l.Field<string>("VALOR"),
                            POSICAO_FILTRO = l.Field<string>("POSICAO_FILTRO"),
                            FILTRO = l.Field<string>("FILTRO"),
                            NOME_CAMPO_FILTRO = l.Field<string>("NOME_CAMPO_FILTRO"),
                        };

                string filtro = "";

                if (f.ElementAt(0).TIPO_FILTRO == "1" || f.ElementAt(0).TIPO_FILTRO == "2") // in e not in
                {
                    filtro = " and  ( ";
                    for (int i = 0; i < f.Count(); i++)
                    {
                        if (i == 0) // se for o primeiro registro do filtro adiciona o in
                        {
                            filtro += f.ElementAt(i).NOME_CAMPO_FILTRO;
                            filtro += f.ElementAt(i).TIPO_FILTRO.ToString().Equals("2") ? " not " : "";
                            filtro += " in (";
                        }

                        // adicionar o parâmetro na SQL de acordo com o nome inserido na lista lParametros
                        filtro += "&f" + f.ElementAt(i).FILTRO + "p" + i;

                        // se não for o último registro do filtro adiciona a vírgula
                        if (i != f.Count() - 1)
                            filtro += ", ";

                        // Adicionar Parametro na lista
                        // Padrao nome: f<codigofiltro>p<cont do filtro>
                        lParametros.Add(new OracleParameter("f" + f.ElementAt(i).FILTRO + "p" + i, f.ElementAt(i).VALOR));
                    }
                    // fecha os parênteses do filtro após inserir todos
                    filtro += ")) ";

                    // Trocar o texto de identificação do filtro na SQL pelo filtro montado
                    pSQL = pSQL.Replace(f.ElementAt(0).POSICAO_FILTRO, filtro);
                }
                else if (f.ElementAt(0).TIPO_FILTRO.ToString().Equals("3")) // between
                {
                    filtro = " and  ( ";
                    for (int i = 0; i < f.Count(); i++)
                    {
                        if (f.ElementAt(i).TIPO_VALOR == "6")
                            // filtros de Competência são filtrados no between com o formato 'yyyyMM'
                            filtro += "to_char(to_date(" + f.ElementAt(i).NOME_CAMPO_FILTRO + ", 'MM/yyyy'), 'yyyyMM')";
                        else if (f.ElementAt(i).TIPO_VALOR == "7")
                            // filtros de Competência são filtrados no between com o formato 'dd/MM/yyyy HH24:MI:SS'
                            filtro += f.ElementAt(i).NOME_CAMPO_FILTRO;
                        else if (f.ElementAt(i).TIPO_VALOR == "9")
                            // filtros de Competência são filtrados no between com o formato 'HH24:MI:SS'
                            filtro += "TO_CHAR("+f.ElementAt(i).NOME_CAMPO_FILTRO+", 'HH24:MI:SS')";
                        else
                            filtro += f.ElementAt(i).NOME_CAMPO_FILTRO;

                        filtro += " between ";

                        if (f.ElementAt(i).TIPO_VALOR == "7")
                            filtro += "TO_DATE(&f" + f.ElementAt(i).FILTRO + "p" + i + "v1,'DD/MM/YYYY HH24:MI:SS') and TO_DATE(&f" + f.ElementAt(i).FILTRO + "p" + i + "v2,'DD/MM/YYYY HH24:MI:SS')";
                        else
                            filtro += "&f" + f.ElementAt(i).FILTRO + "p" + i + "v1 and &f" + f.ElementAt(i).FILTRO + "p" + i + "v2";

                        // Mais de um between para o mesmo filtro é preciso usar a cláusula or
                        // ex: data between '01/01/2010' and '10/01/2010' or data between '01/02/2010' and '10/02/2010'
                        if (i != f.Count() - 1)
                            filtro += " or ";

                        // Adicionar Parametro na lista
                        // Padrao nome: f<codigofiltro>p<cont do filtro><valor 1 ou valor 2>
                        lParametros.Add(new OracleParameter("f" + f.ElementAt(i).FILTRO + "p" + i + "v1", f.ElementAt(i).VALOR.Split(';')[0]));
                        lParametros.Add(new OracleParameter("f" + f.ElementAt(i).FILTRO + "p" + i + "v2", f.ElementAt(i).VALOR.Split(';')[1]));
                    }
                    filtro += ") ";

                    // Trocar o texto de identificação do filtro na SQL pelo filtro montado
                    string[] vPos = f.ElementAt(0).POSICAO_FILTRO.Split(';'),
                             vFiltros = filtro.Split(';');
                    for (int i = 0; i < vPos.Length; i++)
                        pSQL = pSQL.Replace(vPos[i], vFiltros[i]);
                }
                else if (f.ElementAt(0).TIPO_FILTRO.ToString().Equals("4")) // parâmetro
                {
                    // adiciona no filtro o NOME_CAMPO_FILTRO para adicionar o(s) parâmetro(s) com este(s) nome(s)
                    filtro = f.ElementAt(0).NOME_CAMPO_FILTRO;

                    for (int i = 0; i < f.Count(); i++)
                    {
                        // Adicionar Parametro na lista
                        // Padrao nome: f<codigofiltro>p<cont do filtro><valor 1 ou valor 2>
                        string[] vNomParam = f.ElementAt(i).NOME_CAMPO_FILTRO.Split(';'),
                                 vValParam = f.ElementAt(i).VALOR.Split(';');
                        for (int c = 0; c < vNomParam.Length; c++)
                            lParametros.Add(new OracleParameter(vNomParam[c], vValParam[c]));
                    }

                    // Trocar o texto de identificação do filtro na SQL pelo filtro montado
                    string[] vPos = f.ElementAt(0).POSICAO_FILTRO.Split(';'),
                             vFiltros = filtro.Split(';');

                    for (int i = 0; i < vPos.Length; i++)
                        pSQL = pSQL.Replace(vPos[i], "&" + vFiltros[i]);
                }
                else if (f.ElementAt(0).TIPO_FILTRO.ToString().Equals("5")) // parâmetro de substituição
                {
                    // adiciona no filtro o NOME_CAMPO_FILTRO para adicionar o(s) parâmetro(s) com este(s) nome(s)
                    filtro = f.ElementAt(0).NOME_CAMPO_FILTRO;

                    // Trocar o texto de identificação do filtro na SQL pelo filtro montado
                    string[] vPos = f.ElementAt(0).POSICAO_FILTRO.Split(';'),
                             vFiltros = filtro.Split(';'),
                             vValParam = f.ElementAt(0).VALOR.Split(';');

                    for (int i = 0; i < vPos.Length; i++)
                        pSQL = pSQL.Replace(vPos[i], vValParam[i]);
                }
            }

            // Remover da SQL o Identificador de Posição dos filtros não adicionados
            var lFiltrosComando = from i in BuscaExtratorDadosComandosFiltros(txtComando.Text, null, this.Sistema).AsEnumerable()
                                  where !lFiltrosDistinct.Contains(i.Field<decimal>("FILTRO").ToString())
                                  select new { filtro = i.Field<decimal>("FILTRO").ToString(), posicao_filtro = i.Field<string>("POSICAO_FILTRO") };

            foreach (var i in lFiltrosComando)
                foreach (string p in i.posicao_filtro.Split(';'))
                    pSQL = pSQL.Replace(p, " ");

            return lParametros;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.LimpaCampos(this.Controls);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{

                if (ValidaGerarPlanilha())
                {
                    WaitWindow.Begin("Aguarde!! Gerando pesquisa...");

                    string sql = BuscaExtratorDadosComandoSQL(txtComando.Text, this.Sistema);

                    List<OracleParameter> lParametros = AplicarFiltrosRetornarParametros(ref sql);

                    DataTable dt = ExtratorDadosExecutarComando(sql, lParametros, this.Sistema);

                    ExportaExcel.ExportaParaExcel(dt, null, true,false, txtComando.Text, txtComandoDesc.Text, null);

                    WaitWindow.End();

                    comandoExecutado = true;//Variavel do FormBasic para utilizar filtros anteriores
                    if (comandoExecutado && atribuirFiltros)
                    {
                        ExecutarSQL("DELETE FROM HU.EXTRATOR_DADOS_USUARIO_FILTROS WHERE USUARIO = USER AND COMANDO = " + txtComando.Text);
                        string filtro = "";
                        for (int i = 0; i < dgvFiltros.Rows.Count; i++)
                        {
                            if (!filtro.Contains(dgvFiltros.Rows[i].Cells["colFiltro"].Value.ToString()))
                            {
                                filtro += dgvFiltros.Rows[i].Cells["colFiltro"].Value.ToString() + " ";
                                CriarAlterarArquivoRelatorioFiltrosAnteriores(this.Sistema, int.Parse(txtComando.Text), dgvFiltros.Rows[i].Cells["colFiltro"].Value.ToString(), 2);
                            }
                        }
                        
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    WaitWindow.End();
            //    //throw ex; comentado e substituído em 13/08/2015
            //    MessageBox.Show(this, "Falha ao realizar pesquisa.\nMotivo:" + ex.Message + (ex.InnerException != null ? "\n\nDetalhes do Erro: " + ex.InnerException.Message : ""), "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }



        private void txtComando_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtComando.ContemValor())
                {
                    DataTable dt = BuscaExtratorDadosComando(txtComando.Text, this.Sistema);

                    if (dt.Rows.Count == 1)
                    {
                        txtComandoDesc.Text = dt.Rows[0]["COMANDO_DESC"].ToString();
                        validarFiltrosAnteriores("");
                    }
                    else
                    {
                        MessageBox.Show("Comando não encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtComando.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Erro ao pesquisar comando.\nMotivo:" + ex.Message + (ex.InnerException != null ? "\nDetalhes do Erro: " + ex.InnerException.Message : ""), "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }
        private void txtComando_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                frmExtratorDadosConsulta frm = new frmExtratorDadosConsulta(this.Sistema, false);
                frm.ShowDialog();

                if (!string.IsNullOrEmpty(frm.Comando))
                {
                    txtComando.Text = frm.Comando;
                    txtComando.Focus();
                    SendKeys.Send("{TAB}");
                }
            }
        }
        private void txtComando_TextChanged(object sender, EventArgs e)
        {
            txtComandoDesc.Text = "";
            txtFiltro.Text = "";
            if (dgvFiltros.DataSource != null)
                ((DataTable)dgvFiltros.DataSource).Clear();
        }
        private void txtComando_Validating(object sender, CancelEventArgs e)
        {
            if (txtComando.ContemValor())
            {
                if (!ValidaPermissaoUsuarioExtratorDadosComando(Globals.Usuario.ToString(), txtComando.Text))
                {
                    DataTable dt = BuscaAdministradoresExtratorDadosComando(txtComando.Text);

                    if (dt.Rows.Count > 0)
                    {
                        string adm = "";
                        foreach (DataRow l in dt.Rows)
                            adm += l["COLABORADOR"].ToString() + " - " + l["COLABORADOR_DESC"].ToString() + "\n";

                        MessageBox.Show("Você não possui Permissão para executar este Comando. Solicite a Permissão para algum dos seguintes Administradores: \n\n" + adm, "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("Você não possui Permissão para executar este Comando.\n\nAtualmente não existem Administradores capazes de liberar Permissão para este Comando, entre em contato com o Administrador do Sistema para verificar a situação.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    e.Cancel = true;
                    txtComando.Text = "";
                }
            }
        }

        private void txtFiltro_Leave(object sender, EventArgs e)
        {
            // verifica o txtFiltroDesc para não executar o Leave 2x na hora que da o Focus no campo Valor exibido
            if (txtFiltro.ContemValor() && string.IsNullOrEmpty(txtFiltroDesc.Text))
            {
                DataTable dt = BuscaExtratorDadosComandosFiltros(txtComando.Text, txtFiltro.Text, this.Sistema);
                mtxtCompetenciaInicial.Location = new Point(89,53);
                mtxtCompetenciaFinal.Location = new Point(200, 53);
                mtxtCompetenciaInicial.Size = new Size(84, 21);
                mtxtCompetenciaFinal.Size = new Size(84,21);
                lblPeriodo.Location = new Point(179, 56);

                if (dt.Rows.Count == 1)
                {
                    //validarFiltrosAnteriores(txtFiltro.Text);

                    if (ValidarCadastroFiltro(dt))
                    {
                        txtFiltroDesc.Text = dt.Rows[0]["FILTRO_DESC"].ToString();

                        // a Tag do txtFiltro irá guardar os campos relevantes para serem inseridos na grid pelo botão Inserir e posteriormente serem
                        // usados na hora de montar os Filtros no botão Gerar.
                        ParametrosFiltro pf = new ParametrosFiltro();
                        pf.NOME_CAMPO = dt.Rows[0]["NOME_CAMPO"].ToString();
                        pf.TIPO_FILTRO = dt.Rows[0]["TIPO_FILTRO"].ToString();
                        pf.OBRIGATORIO = dt.Rows[0]["OBRIGATORIO"].ToString();
                        pf.TIPO_VALOR = dt.Rows[0]["TIPO_VALOR"].ToString();
                        pf.CONTEM_PESQUISA = dt.Rows[0]["CONTEM_PESQUISA"].ToString();
                        pf.POSICAO_FILTRO = dt.Rows[0]["POSICAO_FILTRO"].ToString();
                        pf.NOME_CAMPO_FILTRO = dt.Rows[0]["NOME_CAMPO_FILTRO"].ToString();
                        //txtFiltro.Tag = dt.Rows[0]["NOME_CAMPO"].ToString() + "&" +
                        //                dt.Rows[0]["TIPO_FILTRO"].ToString() + "&" +
                        //                dt.Rows[0]["OBRIGATORIO"].ToString() + "&" +
                        //                dt.Rows[0]["TIPO_VALOR"].ToString() + "&" +
                        //                dt.Rows[0]["CONTEM_PESQUISA"].ToString() + "&" +
                        //                dt.Rows[0]["POSICAO_FILTRO"].ToString() + "&" +
                        //                dt.Rows[0]["NOME_CAMPO_FILTRO"].ToString();
                        txtFiltro.Tag = pf;


                        txtValorFiltroSemPesquisa.Visible = false;
                        txtFaixaInicial.Visible = false;
                        txtFaixaFinal.Visible = false;
                        txtValorFiltro.Visible = false;
                        txtValorFiltroDesc.Visible = false;
                        mtxtPeriodoInicio.Visible = false;
                        mtxtPeriodoFim.Visible = false;
                        lblPeriodo.Visible = false;
                        mtxtCompetenciaInicial.Visible = false;
                        mtxtCompetenciaFinal.Visible = false;

                        // tipos de valor:1-Inteiro, 2-Data Única 3- Data Período, 4-String, 5-Competência, 6-Período de Competência
                        //                7-Periodo DateTime, 8-Competencia DateTime, 9-Periodo Hora, 10-Competencia Hora
                        if (pf.TIPO_VALOR.Equals("1") || pf.TIPO_VALOR.Equals("4")) // inteiro ou string
                        {
                            txtValorFiltro.Visible = pf.CONTEM_PESQUISA.Equals("1");
                            txtValorFiltroDesc.Visible = pf.CONTEM_PESQUISA.Equals("1");
                            txtValorFiltroSemPesquisa.Visible = !pf.CONTEM_PESQUISA.Equals("1");

                            if (pf.CONTEM_PESQUISA.ToString().Equals("1"))
                                txtValorFiltro.Focus();
                            else
                                txtValorFiltroSemPesquisa.Focus();
                        }
                        else if (pf.TIPO_VALOR.Equals("2")) // data única
                        {
                            mtxtPeriodoInicio.Visible = true;

                            mtxtPeriodoInicio.Text = DateTime.Parse(DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToShortDateString();
                            mtxtPeriodoFim.Text = "";

                            mtxtPeriodoInicio.Focus();
                        }
                        else if (pf.TIPO_VALOR.Equals("3")) // período de data
                        {
                            mtxtPeriodoInicio.Visible = true;
                            mtxtPeriodoFim.Visible = true;
                            lblPeriodo.Visible = true;
                            mtxtPeriodoInicio.Text = DateTime.Parse(DateTime.Now.AddMonths(-1).ToString("MM/yyyy")).ToShortDateString();
                            mtxtPeriodoFim.Text = DateTime.Parse("01/" + DateTime.Now.ToString("MM/yyyy")).AddDays(-1).ToShortDateString();

                            mtxtPeriodoInicio.Focus();
                        }
                        else if (pf.TIPO_VALOR.Equals("5")) // Competência
                        {
                            mtxtCompetenciaInicial.Visible = true;
                            mtxtCompetenciaInicial.Text = DateTime.Now.AddMonths(-1).ToString("MM/yyyy");
                        }
                        else if (pf.TIPO_VALOR.Equals("6")) //Período de Competência Obrigatório e Opcional
                        {
                            mtxtCompetenciaInicial.Visible = true;
                            mtxtCompetenciaFinal.Visible = true;
                            lblPeriodo.Visible = true;

                            mtxtCompetenciaInicial.Text = DateTime.Now.AddMonths(-1).ToString("MM/yyyy");
                            mtxtCompetenciaFinal.Text = DateTime.Now.AddMonths(-1).ToString("MM/yyyy");
                        }
                        else if (pf.TIPO_VALOR.Equals("7")) //DATETIME PERIODO
                        {
                            mtxtCompetenciaInicial.Visible = true;
                            mtxtCompetenciaFinal.Visible = true;
                            lblPeriodo.Visible = true;

                            mtxtCompetenciaInicial.Mask = "00/00/0000 00:00:00";
                            mtxtCompetenciaFinal.Mask = "00/00/0000 00:00:00";

                            mtxtCompetenciaInicial.Size = new Size(mtxtCompetenciaInicial.Width + 50, mtxtCompetenciaInicial.Height);
                            mtxtCompetenciaFinal.Size = new Size(mtxtCompetenciaFinal.Width + 50, mtxtCompetenciaFinal.Height);
                            mtxtCompetenciaFinal.Location = new Point(mtxtCompetenciaFinal.Location.X + 50, mtxtCompetenciaFinal.Location.Y);
                            lblPeriodo.Location = new Point(lblPeriodo.Location.X + 50, lblPeriodo.Location.Y);
                        }
                        else if (pf.TIPO_VALOR.Equals("8")) //DATETIME COMPETENCIA 
                        {
                            mtxtCompetenciaInicial.Visible = true;

                            mtxtCompetenciaInicial.Mask = "00/00/0000 00:00:00";
                            mtxtCompetenciaInicial.Size = new Size(mtxtCompetenciaInicial.Width + 50, mtxtCompetenciaInicial.Height);
                        }
                        else if (pf.TIPO_VALOR.Equals("9")) //TIME PERIODO
                        {
                            mtxtCompetenciaInicial.Visible = true;
                            mtxtCompetenciaFinal.Visible = true;
                            lblPeriodo.Visible = true;

                            mtxtCompetenciaInicial.Mask = "00:00:00";
                            mtxtCompetenciaFinal.Mask = "00:00:00";
                        }
                        else if (pf.TIPO_VALOR.Equals("10")) //TIME COMPETENCIA 
                        {
                            mtxtCompetenciaInicial.Visible = true;

                            mtxtCompetenciaInicial.Mask = "00:00:00";
                        }
                        else if (pf.TIPO_VALOR.Equals("11")) //Faixas de String/Inteiro
                        {
                            txtFaixaInicial.Visible = true;
                            txtFaixaFinal.Visible = true;
                            lblPeriodo.Visible = true;
                        }
                        else
                            throw new Exception(@"Tipo de Valor do Filtro inválido.");
                    }
                    else
                    {
                        txtFiltro.Text = "";
                        txtFiltro.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Filtro não encontrado ou não relacionado ao Comando informado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtFiltro.Text = "";
                }

            }
        }
        private void txtFiltro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                FormBusca fb = new FormBusca(QueryBuscaExtratorDadosComandosFiltros(txtComando.Text, null, false).ToString(), new List<System.Data.OracleClient.OracleParameter>() { new System.Data.OracleClient.OracleParameter("pComando", txtComando.Text), new System.Data.OracleClient.OracleParameter("pSistema", this.Sistema) }, true, "FILTROS DO COMANDO", "FILTRO_DESC", null, "Nenhum registro encontrado");
                fb.ShowDialog();

                if (fb.retorno != null)
                {
                    txtFiltro.Text = fb.retorno["FILTRO"].ToString();
                    txtFiltro_Leave(null, null); // para exibir os campos necessários e o TAB focar o campo correto.
                    SendKeys.Send("{TAB}");
                }
            }

        }
        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            txtFiltroDesc.Text = "";
            txtFiltro.Tag = "";
            txtValorFiltro.Text = "";
            mtxtPeriodoInicio.Text = "";
            mtxtPeriodoFim.Text = "";
        }
        private void txtFiltro_Enter(object sender, EventArgs e)
        {
            if (!txtComando.ContemValor())
            {
                MessageBox.Show("Informe o Comando.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtComando.Focus();
            }
        }

        private void txtValorFiltro_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtValorFiltro.ContemValor() && txtFiltro.ContemValor())
                {
                    string sql = BuscaExtratorDadosFiltroSQLPesquisa(txtFiltro.Text);

                    DataTable dt = BuscaExtratorDadosFiltroSQLPesquisa(sql, txtValorFiltro.Text, ((ParametrosFiltro)txtFiltro.Tag).NOME_CAMPO);

                    if (dt.Rows.Count == 1)
                    {
                        txtValorFiltroDesc.Text = dt.Rows[0][1].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Registro não encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtValorFiltro.Text = "";
                    }
                }
                else
                    txtValorFiltro.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Falha ao realizar pesquisa.\nMotivo:" + ex.Message + (ex.InnerException != null ? "\n\nDetalhes do Erro: " + ex.InnerException.Message : ""), "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtValorFiltro_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F3)
                {
                    if (!txtFiltro.ContemValor())
                        MessageBox.Show("Informe o Filtro.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        // Para verificar se vai auto executar a pesquisa no F3 ou não
                        DataTable dt = BuscaExtratorDadosComandosFiltros(txtComando.Text, txtFiltro.Text, this.Sistema);

                        FormBuscaPaginacao fb = new FormBuscaPaginacao(BuscaExtratorDadosFiltroSQLPesquisa(txtFiltro.Text), new List<System.Data.OracleClient.OracleParameter>(), dt.Rows[0]["auto_executar_pesquisa"].ToString().Equals("1"), "FILTROS", dt.Rows[0]["NOME_CAMPO"].ToString(), null, "Nenhum registro encontrado.");
                        fb.ShowDialog();

                        if (fb.retorno != null)
                        {
                            txtValorFiltro.Text = fb.retorno[0].ToString();
                            SendKeys.Send("{TAB}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Falha ao realizar pesquisa.\nMotivo:" + ex.Message + (ex.InnerException != null ? "\n\nDetalhes do Erro: " + ex.InnerException.Message : ""), "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void txtValorFiltro_TextChanged(object sender, EventArgs e)
        {
            txtValorFiltroDesc.Text = "";
        }

        private void btnAdicionarFiltro_Click(object sender, EventArgs e)
        {
            if (ValidaAdicionarFiltro())
            {
                // recupera as informações da tag do txtFiltro para inserir na grid
                ParametrosFiltro p = ((ParametrosFiltro)txtFiltro.Tag);

                DataTable dt = ((DataTable)dgvFiltros.DataSource);
                DataRow linha = dt.NewRow();

                linha["FILTRO"] = txtFiltro.Text;
                linha["FILTRO_DESC"] = txtFiltroDesc.Text;

                linha["NOME_CAMPO"] = p.NOME_CAMPO;
                linha["TIPO_FILTRO"] = p.TIPO_FILTRO;
                linha["OBRIGATORIO"] = p.OBRIGATORIO;
                linha["TIPO_VALOR"] = p.TIPO_VALOR;
                linha["POSICAO_FILTRO"] = p.POSICAO_FILTRO;
                linha["NOME_CAMPO_FILTRO"] = p.NOME_CAMPO_FILTRO;

                // Verifica o Tipo de Valor para exibir de forma específica e melhor visível para o usuário
                // 1-Inteiro, 2-Data Única 3- Data Período, 4-String, 5-Competência, 6-Período de Competência
                switch (linha["TIPO_VALOR"].ToString())
                {
                    case "1": // inteiro
                        if (p.CONTEM_PESQUISA.Equals("1")) // Filtro informado contém pesquisa
                        {
                            linha["VALOR"] = txtValorFiltro.Text;
                            linha["VALOR_DESC"] = txtValorFiltro.Text + " - " + txtValorFiltroDesc.Text;
                        }
                        else
                        {
                            linha["VALOR"] = txtValorFiltroSemPesquisa.Text;
                            linha["VALOR_DESC"] = txtValorFiltroSemPesquisa.Text;
                        }
                        break;
                    case "2": // data única
                        linha["VALOR"] = mtxtPeriodoInicio.Text;
                        linha["VALOR_DESC"] = mtxtPeriodoInicio.Text;
                        break;
                    case "3": // data período
                        linha["VALOR"] = mtxtPeriodoInicio.Text + ";" + mtxtPeriodoFim.Text;
                        linha["VALOR_DESC"] = "Entre " + mtxtPeriodoInicio.Text + " e " + mtxtPeriodoFim.Text;
                        break;
                    case "4": // string
                        if (p.CONTEM_PESQUISA.Equals("1")) // Filtro informado contém pesquisa
                        {
                            linha["VALOR"] = txtValorFiltro.Text;
                            linha["VALOR_DESC"] = txtValorFiltro.Text + " - " + txtValorFiltroDesc.Text;
                        }
                        else
                        {
                            linha["VALOR"] = txtValorFiltroSemPesquisa.Text;
                            linha["VALOR_DESC"] = txtValorFiltroSemPesquisa.Text;
                        }
                        break;
                    case "5": // competência
                        linha["VALOR"] = mtxtCompetenciaInicial.Text;
                        linha["VALOR_DESC"] = mtxtCompetenciaInicial.Text;
                        break;
                    case "6": // período de competência
                        linha["VALOR"] = DateTime.Parse(mtxtCompetenciaInicial.Text).ToString("yyyyMM") + ";" + DateTime.Parse(mtxtCompetenciaFinal.Text).ToString("yyyyMM");
                        linha["VALOR_DESC"] = mtxtCompetenciaInicial.Text + " a " + mtxtCompetenciaFinal.Text;
                        break;
                    case "7": // DateTime período
                        linha["VALOR"] = DateTime.Parse(mtxtCompetenciaInicial.Text).ToString("dd/MM/yyyy HH:mm:ss") + ";" + DateTime.Parse(mtxtCompetenciaFinal.Text).ToString("dd/MM/yyyy HH:mm:ss");
                        linha["VALOR_DESC"] = mtxtCompetenciaInicial.Text + " a " + mtxtCompetenciaFinal.Text;
                        break;
                    case "8": // DateTime competência
                        linha["VALOR"] = DateTime.Parse(mtxtCompetenciaInicial.Text).ToString("dd/MM/yyyy HH:mm:ss");
                        linha["VALOR_DESC"] = mtxtCompetenciaInicial.Text;
                        break;
                    case "9": // Time período
                        linha["VALOR"] = DateTime.Parse(mtxtCompetenciaInicial.Text).ToString("HH:mm:ss") + ";" + DateTime.Parse(mtxtCompetenciaFinal.Text).ToString("HH:mm:ss");
                        linha["VALOR_DESC"] = mtxtCompetenciaInicial.Text + " a " + mtxtCompetenciaFinal.Text;
                        break;
                    case "10": // Time competência
                        linha["VALOR"] = DateTime.Parse(mtxtCompetenciaInicial.Text).ToString("HH:mm:ss");
                        linha["VALOR_DESC"] = mtxtCompetenciaInicial.Text;
                        break;
                    case "11": // Faixas de string/inteiro
                        linha["VALOR"] = txtFaixaInicial.Text + ";" + txtFaixaFinal.Text;
                        linha["VALOR_DESC"] = txtFaixaInicial.Text + " a " + txtFaixaFinal.Text;
                        break;
                    default:
                        throw new Exception(@"Tipo de Valor do Filtro " + txtFiltro.Text + " não encontrado.");
                }

                dt.Rows.Add(linha);

                dt.DefaultView.Sort = "FILTRO, VALOR";
                dt = dt.DefaultView.ToTable();

                dgvFiltros.DataSource = dt;

                txtValorFiltro.Text = "";
                txtValorFiltroSemPesquisa.Text = "";
                mtxtPeriodoInicio.Text = "";
                mtxtPeriodoFim.Text = "";
                mtxtCompetenciaInicial.Text = "";
                mtxtCompetenciaFinal.Text = "";

                txtFiltro.Focus();
            }
        }

        private void dgvFiltros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == dgvFiltros.Columns["colExcluir"].Index)
            {
                if (MessageBox.Show("Confirma exclusão do filtro?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    ((DataTable)dgvFiltros.DataSource).Rows.RemoveAt(e.RowIndex);
                    dgvFiltros.Refresh();
                }
            }
        }

        private static StringBuilder QueryBuscaExtratorDadosComandosFiltros(string pComando, string pFiltro, bool pExibirCamposParametros)
        {
            string where = "";

            if (!string.IsNullOrEmpty(pComando))
                where += " and c.comando = &pComando ";
            if (!string.IsNullOrEmpty(pFiltro))
                where += " and f.filtro = &pFiltro ";

            if (Globals.Sistema == 129) //MV
            {
                return new StringBuilder(@"select f.filtro, f.descricao filtro_desc, decode(nvl(cf.obrigatorio,0), 0, 'NÃO', 'SIM') filtro_obrigatorio "
                                  + (pExibirCamposParametros ? @", 
                                               nvl(cf.obrigatorio,0) obrigatorio, cf.tipo_filtro, decode(cf.tipo_filtro, 1, 'in', 2, 'not in', 3, 'between') tipo_filtro_desc,
                                               f.tipo_valor, f.nome_campo, case when f.query_pesquisa is null then 0 else 1 end contem_pesquisa, 
                                               auto_executar_pesquisa, ident_posicao_filtro posicao_filtro, cf.nome_campo_filtro " : "") + @"
                                        from hu.extrator_dados_comandos_filtro cf
                                        join hu.extrator_dados_comandos c on c.comando = cf.comando
                                        join hu.extrator_dados_filtros f on f.filtro = cf.filtro
                                        where c.sistema = &pSistema "
                            + where);
            }
            else
            {
                return new StringBuilder(@"select f.filtro, f.descricao filtro_desc, decode(nvl(cf.obrigatorio,0), 0, 'NÃO', 'SIM') filtro_obrigatorio "
                                  + (pExibirCamposParametros ? @", 
                                               nvl(cf.obrigatorio,0) obrigatorio, cf.tipo_filtro, decode(cf.tipo_filtro, 1, 'in', 2, 'not in', 3, 'between') tipo_filtro_desc,
                                               f.tipo_valor, f.nome_campo, case when f.query_pesquisada is null then 0 else 1 end contem_pesquisa, 
                                               auto_executar_pesquisa, ident_posicao_filtro posicao_filtro, cf.nome_campo_filtro " : "") + @"
                                        from hu.extrator_dados_comandos_filtro cf
                                        join hu.extrator_dados_comandos c on c.comando = cf.comando
                                        join hu.extrator_dados_filtros f on f.filtro = cf.filtro
                                        where c.sistema = &pSistema "
                            + where);
            }
        }
        private static DataTable BuscaExtratorDadosComandosFiltros(string pComando, string pFiltro, int pSistema)
        {
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);

            if (!string.IsNullOrEmpty(pComando))
                dal.AddParam("pComando", pComando);
            if (!string.IsNullOrEmpty(pFiltro))
                dal.AddParam("pFiltro", pFiltro);

            dal.AddParam("pSistema", pSistema);

            return dal.ExecuteQuery(QueryBuscaExtratorDadosComandosFiltros(pComando, pFiltro, true).ToString());
        }

        private static string BuscaExtratorDadosComandoSQL(string pComando, int pSistema)
        {
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);

            dal.AddParam("pComando", pComando);
            dal.AddParam("pSistema", pSistema);

            DataTable dt = dal.ExecuteQuery(@" select descricao sql from hu.extrator_dados_comandos where comando = &pComando and sistema = &pSistema ");

            if (dt.Rows.Count == 1)
            {
                string sql = dt.Rows[0]["SQL"].ToString();

                return sql;
            }
            else
                return null;
        }

        private static DataTable ExtratorDadosExecutarComando(string pSQLComando, List<System.Data.OracleClient.OracleParameter> lParam, int sistema)
        {
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);

            // Acrescentado em 25/06/2015 para atribuir a empresa sempre que for executar um SELECT no MV
            if (sistema == 129)
                dal.ExecuteNonQuery(@"CALL PKG_MV2000.ATRIBUI_EMPRESA(1)");

            foreach (System.Data.OracleClient.OracleParameter p in lParam)
                dal.AddParam(p.ParameterName, p.Value);

            return dal.ExecuteQuery(pSQLComando);
        }

        private static StringBuilder QueryBuscaExtratorDadosComando(string pComando)
        {
            return new StringBuilder(@"select comando, nome comando_desc, descricao sql, finalidade from hu.extrator_dados_comandos 
                                        where sistema = &pSistema " + (string.IsNullOrEmpty(pComando) ? "" : " and comando = &pComando "));
        }
        private static DataTable BuscaExtratorDadosComando(string pComando, int pSistema)
        {
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);

            if (!string.IsNullOrEmpty(pComando))
                dal.AddParam("pComando", pComando);
            dal.AddParam("pSistema", pSistema);

            return dal.ExecuteQuery(QueryBuscaExtratorDadosComando(pComando).ToString());
        }

        private static bool ValidaPermissaoUsuarioExtratorDadosComando(string pUsuario, string pComando)
        {
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);

            dal.AddParam("pUsuario", pUsuario);
            dal.AddParam("pComando", pComando);

            return dal.ExecuteQuery(@"select 1 
                                      from hu.extrator_dados_permissoes 
                                      where comando = &pComando 
                                        and usuario = &pUsuario
                                      union all
                                      select 1
                                      from hu.extrator_dados_usuarios_grupos ug
                                      join hu.extrator_dados_grupos_comandos gc on gc.grupo = ug.grupo
                                      where gc.comando = &pComando
                                        and ug.usuario = &pUsuario").Rows.Count > 0;
        }

        private static StringBuilder QueryBuscaAdministradoresExtratorDadosComando(string pComando)
        {
            return new StringBuilder(@"select ep.comando, fun.numcad colaborador, fun.NOMFUN colaborador_desc
                                        from hu.extrator_dados_permissoes ep
                                        join bhora.r034fun fun on fun.numcad = ep.usuario
                                        where fun.numemp = 1
                                          and fun.tipcol = 1
                                          and nvl(ep.administrador,0) = 1" +
                                       (string.IsNullOrEmpty(pComando) ? "" : " and ep.comando = &pComando")
                                       + " order by ep.comando, fun.nomfun ");
        }
        private static DataTable BuscaAdministradoresExtratorDadosComando(string pComando)
        {
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);

            if (!string.IsNullOrEmpty(pComando))
                dal.AddParam("pComando", pComando);

            return dal.ExecuteQuery(QueryBuscaAdministradoresExtratorDadosComando(pComando).ToString());
        }

        private static string BuscaExtratorDadosFiltroSQLPesquisa(string pFiltro)
        {
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);

            dal.AddParam("pFiltro", pFiltro);
            DataTable dt;

            if (Globals.Sistema == 129)
                dt = dal.ExecuteQuery(@"select query_pesquisa query_pesquisa from hu.extrator_dados_filtros where filtro = &pFiltro");
            else
                dt = dal.ExecuteQuery(@"select query_pesquisada query_pesquisa from hu.extrator_dados_filtros where filtro = &pFiltro");

            if (dt.Rows.Count == 1)
                return dt.Rows[0]["QUERY_PESQUISA"].ToString();
            else
                return null;
        }
        private static DataTable BuscaExtratorDadosFiltroSQLPesquisa(string pSQLPesquisa, string pValorFiltro, string pNomeCampo)
        {
            Conexao dal = new Conexao(Globals.GetStringConnection(), 2);

            dal.AddParam("pValorFiltro", pValorFiltro);

            return dal.ExecuteQuery(pSQLPesquisa.ToString() + " where " + pNomeCampo + @" = &pValorFiltro ");
        }


        private void validarFiltrosAnteriores(string pFiltro)
        {
            //Utilizado nos filtros anteriores. Júlio
            string comandoAux, comandoDescAux, filtroAux, filtroDescAux;
            comandoAux = txtComando.Text;
            filtroAux = pFiltro;
            comandoDescAux = txtComandoDesc.Text;
            filtroDescAux = txtFiltroDesc.Text;

            if (string.IsNullOrEmpty(pFiltro))
                LimpaCampos(this.Controls);
            
            txtComando.Text = comandoAux;
            txtComandoDesc.Text = comandoDescAux;
            txtFiltro.Text = filtroAux;
            txtFiltroDesc.Text = filtroDescAux;

            atribuirFiltrosAnteriores(this.Sistema, int.Parse(txtComando.Text), pFiltro);
            if (atribuirFiltros)
            {
                bool deletarLinha = true;

                //Comparar todas as linhas para não repetir filtros
                for (int i = 0; i < dgvFiltros.Rows.Count; i++)
                {
                    for (int j = i + 1; j < dgvFiltros.Rows.Count; j++)
                    {
                        deletarLinha = true;
                        for (int k = 0; k < dgvFiltros.Columns.Count; k++)
                        {
                            if (!dgvFiltros.Rows[i].Cells[k].Value.ToString().Equals(dgvFiltros.Rows[j].Cells[k].Value.ToString()))
                                deletarLinha = false;
                        }

                        if (deletarLinha)
                            dgvFiltros.Rows.RemoveAt(j);
                    }
                }
            }
        }
    }
}
