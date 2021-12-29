using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiehaRC
{
    public partial class Form1 : Form
    {
        BancoSqlServer banco;
        public Form1()
        {
            InitializeComponent();
            banco = new BancoSqlServer(@"DESKTOP-NJSVS9Q\SQLEXPRESS", "SiehaEconomyCadastro", "Sieha", "123");
            CarregaComboBoxSexo();
            CarregaComboBoxBuscaClientes();
        }
        private void CarregaComboBoxBuscaClientes()
        {
            string combobox = "Nome,CPF,CNPJ,Telefone,Whatsapp";
            foreach(string nome in combobox.Split(','))
            {
                TipoBuscaClientes.Items.Add(nome);
            }
        }
        private void CarregaComboBoxSexo()
        {
            string combobox = "feminino,masculino";
            foreach (string nome in combobox.Split(','))
            {
                SexoCadastro.Items.Add(nome);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string[] CamposAInserir = new string[] { "cpf,cnpj,razaosocial,nome,telefone,whatsapp,email,endereço,sexo",
                                                     "idcadastro,equipamento,valorcompra,valorserviço,estado,defeito" };
            string[] ValorCamposAInserir = new string[2];
            object[] UltimoId;
            string estado;
            bool resultado = false;
            string Existe = null;

            if (!String.IsNullOrWhiteSpace(NomeCadastro.Text)        && 
               !String.IsNullOrWhiteSpace(TelefoneCadastro.Text)     && 
               !String.IsNullOrWhiteSpace(WhatsAppCadastro.Text)     &&
               !String.IsNullOrWhiteSpace(EmailCadastro.Text)        &&
               !String.IsNullOrWhiteSpace(EndereçoCadastro.Text)     &&
               !String.IsNullOrWhiteSpace(SexoCadastro.Text)         &&
               !String.IsNullOrWhiteSpace(EquipamentoCadastro.Text)  &&
               !String.IsNullOrWhiteSpace(ValorCompraCadastro.Text)  &&
               !String.IsNullOrWhiteSpace(ValorServiçoCadastro.Text) &&
               (EquipNovoCadastro.Checked ^ EquipUsadoCadastro.Checked))
            {
                if (PessoaFisicaChecked.Checked || PessoaJuridicaChecked.Checked)
                {
                    banco.conectar();
                    if (PessoaFisicaChecked.Checked)
                    {
                        ValorCamposAInserir[0] = "'"+CPFCadastro.Text      + "','" +
                                                  "NULL"                + "','" +
                                                  "NULL"                + "','" +
                                                  NomeCadastro.Text     + "','" +
                                                  TelefoneCadastro.Text + "','" +
                                                  WhatsAppCadastro.Text + "','" +
                                                  EmailCadastro.Text    + "','" +
                                                  EndereçoCadastro.Text + "','" +
                                                  SexoCadastro.Text     + "'";

                        resultado = banco.PesquisaSeExiste("cadastro", "cpf", CPFCadastro.Text);
                        Existe = "cpf";
                        //   cpf,cnpj,razaosocial,nome,telefone,whatsap,endereço,sexo

                    }
                    else
                    {
                        ValorCamposAInserir[0] = "'"+"NULL"                    + "','" +
                                                   CNPJCadastro.Text        + "','" +
                                                   RazaoSocialCadastro.Text + "','" +
                                                   NomeCadastro.Text        + "','" +
                                                   TelefoneCadastro.Text    + "','" +
                                                   WhatsAppCadastro.Text    + "','" +
                                                   EmailCadastro.Text       + "','" +
                                                   EndereçoCadastro.Text    + "','" +
                                                   SexoCadastro.Text        +"'";

                        resultado = banco.PesquisaSeExiste("cadastro", "cnpj", CNPJCadastro.Text);
                        Existe = "cnpj";
                    }

                    if(!resultado)
                    {
                        if (EquipNovoCadastro.Checked)
                        {
                            estado = "NOVO";
                        }
                        else
                        {
                            estado = "USADO";
                        }

                        UltimoId = banco.PesquisarUltimaLinha("cadastro", "id");

                        ValorCamposAInserir[1] = "'" + (Convert.ToInt16(UltimoId[0])+1).ToString() + "','" +
                                                       EquipamentoCadastro.Text                    + "','" +
                                                       ValorCompraCadastro.Text.Replace(",",".")   + "','" +
                                                       ValorServiçoCadastro.Text.Replace(",", ".") + "','" +
                                                       estado                                      + "','" +
                                                       EquipDefeitoCadastro.Text                   + "'";

                        resultado = banco.InserirEm("cadastro", CamposAInserir[0], ValorCamposAInserir[0]);
                        if (resultado)
                        {
                            resultado = banco.InserirEm("equipamento", CamposAInserir[1], ValorCamposAInserir[1]);
                            if(resultado)
                            {
                                MessageBox.Show("Cliente cadastrado com sucesso!", @"SiehaR&C", MessageBoxButtons.OK);
                            }
                            else
                            {
                                MessageBox.Show("Erro ao cadastrar dados do equipamento do cliente.", "Erro!", MessageBoxButtons.OK);
                                banco.DeleteOnde("cadastro", "id", (Convert.ToInt16(UltimoId[0]) + 1).ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show("Erro ao cadastrar o cliente.", "Erro!", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ja existe um cadastro com este " + Existe + "!", "Erro!", MessageBoxButtons.OK);
                    }

                    banco.desconectar();
                }
                else
                {
                    MessageBox.Show("Selecione pessoa física ou jurídica.", "Erro!", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Algum dado está em branco.", "Erro!", MessageBoxButtons.OK);
            }
            
        }

        public bool EstaNullOuZero(object[][] val)
        {
            bool retorno = false;
            if(val == null)
            {
                retorno = true;
            }
            else
            {
                if(val.Length == 0)
                {
                    retorno = true;
                }
            }
            return(retorno);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            object[][] val;
            string CamposBanco = "cpf,nome,endereço";
            string[] nomesDGV;
            string onde = TipoBuscaClientes.Text;
            string valor = ValorBuscaClientes.Text;

            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            banco.conectar();
            val = banco.PesquisarOnde("cadastro", CamposBanco, onde, valor);
            banco.desconectar();
            

            if (!EstaNullOuZero(val))
            {
                if(VerificaBuscaCPFouCNPJ(val) == "CPFeCNPJ")
                {
                    CamposBanco = "cpf,cnpj,nome,endereço";
                }
                else
                {
                    if(VerificaBuscaCPFouCNPJ(val) == "CNPJ")
                    {
                        CamposBanco = "cnpj,nome,endereço";
                    }
                }
                

                banco.conectar();
                val = banco.PesquisarOnde("cadastro", CamposBanco, onde, valor);
                banco.desconectar();

                nomesDGV = CamposBanco.Split(',');
                dataGridView1.ColumnCount = nomesDGV.Length;
                for (int ciclo = 0; ciclo < dataGridView1.ColumnCount; ciclo++)
                {
                    dataGridView1.Columns[ciclo].Width = (int)(690 / dataGridView1.ColumnCount);
                    dataGridView1.Columns[ciclo].Name = nomesDGV[ciclo];
                }
                foreach (object[] tab in val)
                {
                    dataGridView1.Rows.Add(tab);
                }
            }
            
        }

        private void PessoaFisicaChecked_Click(object sender, EventArgs e)
        {
            if(PessoaFisicaChecked.Checked)
            {
                CPFCadastro.Enabled = true;
                PessoaFisicaChecked.Enabled = false;

                CNPJCadastro.Enabled = false;
                PessoaJuridicaChecked.Enabled = true;
                PessoaJuridicaChecked.CheckState = CheckState.Unchecked;
                RazaoSocialCadastro.Enabled = false;
            }
            
        }

        private void PessoaJuridicaChecked_Click(object sender, EventArgs e)
        {
            if(PessoaJuridicaChecked.Checked)
            {
                CPFCadastro.Enabled = false;
                PessoaFisicaChecked.Enabled = true;
                PessoaFisicaChecked.CheckState = CheckState.Unchecked;
                CNPJCadastro.Enabled = true;
                PessoaJuridicaChecked.Enabled = false;
                RazaoSocialCadastro.Enabled = true;
            }
        }

        public string VerificaBuscaCPFouCNPJ(object[][] pesquisa)
        {
            bool cpf = false;
            bool cnpj = false;
            string resultado = null;
            foreach(object[] val in pesquisa)
            {
                if(val[0].ToString() == "NULL")
                {
                    cnpj = true;
                }
                else
                {
                    cpf = true;
                }
            }

            if(cpf && cnpj)
            {
                resultado = "CPFeCNPJ";
            }
            else
            {
                if(cpf)
                {
                    resultado = "CPF";
                }
                else
                {
                    resultado = "CNPJ";
                }
            }
            return resultado;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }
    }
}
