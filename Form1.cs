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
            CarregaComboBoxBuscaServiços();

        }
        private void CarregaComboBoxBuscaClientes()
        {
            string combobox = "Nome,CPF,CNPJ,Telefone,Whatsapp";
            foreach(string nome in combobox.Split(','))
            {
                TipoBuscaClientes.Items.Add(nome);
            }
        }
        private void CarregaComboBoxBuscaServiços()
        {
            string combobox = "Nome,CPF,CNPJ,Telefone,Whatsapp";
            foreach (string nome in combobox.Split(','))
            {
                TipoBuscaServiços.Items.Add(nome);
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
        private void HabilitaBotoesDeServiço()
        {
            BtNovoServiços.Enabled = true;
            BtNovoServiços.BackColor = Color.Pink;
        }
        private void DesabilitarBotoesDeServiço()
        {
            BtNovoServiços.Enabled = false;
            BtNovoServiços.BackColor = Color.Transparent;
        }
        private void HabilitaDadosDeCadastrodeDeServiço()
        {
            EquipamentoServiços.Enabled = true;
            ValorCompraServiços.Enabled = true;
            ValorServiçoServiços.Enabled = true;
            EquipNovoServiços.Enabled = true;
            EquipUsadoServiços.Enabled = true;
            EquipDefeitoServiços.Enabled = true;
        }
        private void DesabilitarDadosDeCadastrodeDeServiço()
        {
            EquipamentoServiços.Enabled = true;
            ValorCompraServiços.Enabled = true;
            ValorServiçoServiços.Enabled = true;
            EquipNovoServiços.Enabled = true;
            EquipUsadoServiços.Enabled = true;
            EquipDefeitoServiços.Enabled = true;
        }
        private void ApagaDadosDeCadastrodeDeServiço()
        {
            EquipamentoServiços.Text = "";
            ValorCompraServiços.Text = "";
            ValorServiçoServiços.Text = "";
            EquipNovoServiços.Checked = false;
            EquipUsadoServiços.Checked = false;
            EquipDefeitoServiços.Text = "";
        }

        private void BtSalvarCadastro_Click(object sender, EventArgs e)
        {
            string[] CamposAInserir = new string[] { "cpf,cnpj,razaosocial,nome,telefone,whatsapp,email,endereço,sexo",
                                                     "idcadastro,equipamento,valorcompra,valorserviço,estado,defeito" };
            string[] ValorCamposAInserir = new string[2];
            int UltimoId;
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
               !String.IsNullOrWhiteSpace(EquipDefeitoCadastro.Text) &&
               (EquipNovoCadastro.Checked ^ EquipUsadoCadastro.Checked))
            {
                if (PessoaFisicaChecked.Checked || PessoaJuridicaChecked.Checked)
                {
                    if(!String.IsNullOrWhiteSpace(CPFCadastro.Text) || !String.IsNullOrWhiteSpace(CNPJCadastro.Text))
                    {
                        banco.conectar();
                        if (PessoaFisicaChecked.Checked)
                        {
                            ValorCamposAInserir[0] = "'" + CPFCadastro.Text + "','" +
                                                      "NULL" + "','" +
                                                      "NULL" + "','" +
                                                      NomeCadastro.Text + "','" +
                                                      TelefoneCadastro.Text + "','" +
                                                      WhatsAppCadastro.Text + "','" +
                                                      EmailCadastro.Text + "','" +
                                                      EndereçoCadastro.Text + "','" +
                                                      SexoCadastro.Text + "'";

                            resultado = banco.PesquisaSeExiste("cadastro", "cpf", CPFCadastro.Text);
                            Existe = "cpf";
                            //   cpf,cnpj,razaosocial,nome,telefone,whatsap,endereço,sexo

                        }
                        else
                        {
                            ValorCamposAInserir[0] = "'" + "NULL" + "','" +
                                                       CNPJCadastro.Text + "','" +
                                                       RazaoSocialCadastro.Text + "','" +
                                                       NomeCadastro.Text + "','" +
                                                       TelefoneCadastro.Text + "','" +
                                                       WhatsAppCadastro.Text + "','" +
                                                       EmailCadastro.Text + "','" +
                                                       EndereçoCadastro.Text + "','" +
                                                       SexoCadastro.Text + "'";

                            resultado = banco.PesquisaSeExiste("cadastro", "cnpj", CNPJCadastro.Text);
                            Existe = "cnpj";
                        }

                        if (!resultado)
                        {
                            if (EquipNovoCadastro.Checked)
                            {
                                estado = "NOVO";
                            }
                            else
                            {
                                estado = "USADO";
                            }

                            UltimoId = banco.PesquisarUltimoIdAutoIncremento("cadastro");

                            ValorCamposAInserir[1] = "'" + (UltimoId + 1).ToString() + "','" +
                                                           EquipamentoCadastro.Text + "','" +
                                                           ValorCompraCadastro.Text.Replace(",", ".") + "','" +
                                                           ValorServiçoCadastro.Text.Replace(",", ".") + "','" +
                                                           estado + "','" +
                                                           EquipDefeitoCadastro.Text + "'";

                            resultado = banco.InserirEm("cadastro", CamposAInserir[0], ValorCamposAInserir[0]);
                            if (resultado)
                            {
                                resultado = banco.InserirEm("equipamento", CamposAInserir[1], ValorCamposAInserir[1]);
                                if (resultado)
                                {
                                    MessageBox.Show("Cliente cadastrado com sucesso!", @"SiehaR&C", MessageBoxButtons.OK);
                                }
                                else
                                {
                                    MessageBox.Show("Erro ao cadastrar dados do equipamento do cliente.", "Erro!", MessageBoxButtons.OK);
                                    banco.DeleteOnde("cadastro", "id", (UltimoId + 1).ToString());
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
                        MessageBox.Show("O Campo CPF ou CNPJ esta vazio!", "Erro!", MessageBoxButtons.OK);
                    }
                    
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
       
        private void BtBuscarCadastro_Click(object sender, EventArgs e)
        {
            object[][] val;
            string CamposBanco = "id,cpf,nome,endereço";
            string[] nomesDGV;
            string onde = TipoBuscaClientes.Text;
            string valor = ValorBuscaClientes.Text;

            GradeDeBuscaClientes.DataSource = null;
            GradeDeBuscaClientes.Rows.Clear();
            GradeDeBuscaClientes.Columns.Clear();

            banco.conectar();
            val = banco.PesquisarOnde("cadastro", CamposBanco, onde, valor);
            banco.desconectar();
            

            if (!EstaNullOuZero(val))
            {
                if(VerificaBuscaCPFouCNPJ(val) == "CPFeCNPJ")
                {
                    CamposBanco = "id,cpf,cnpj,nome,endereço";
                }
                else
                {
                    if(VerificaBuscaCPFouCNPJ(val) == "CNPJ")
                    {
                        CamposBanco = "id,cnpj,nome,endereço";
                    }
                }
                

                banco.conectar();
                val = banco.PesquisarOnde("cadastro", CamposBanco, onde, valor);
                banco.desconectar();

                nomesDGV = CamposBanco.Split(',');
                GradeDeBuscaClientes.ColumnCount = nomesDGV.Length;
                for (int ciclo = 0; ciclo < GradeDeBuscaClientes.ColumnCount; ciclo++)
                {
                    GradeDeBuscaClientes.Columns[ciclo].Width = (int)(690 / GradeDeBuscaClientes.ColumnCount);
                    GradeDeBuscaClientes.Columns[ciclo].Name = nomesDGV[ciclo];
                }
                foreach (object[] tab in val)
                {
                    GradeDeBuscaClientes.Rows.Add(tab);
                }
            }
            
        }

        private void PessoaFisicaChecked_Click(object sender, EventArgs e)
        {
            if(PessoaFisicaChecked.Checked)
            {
                CNPJCadastro.Text        = "";
                RazaoSocialCadastro.Text = "";
                PessoaFisicaClickConfigura();
            }
            
        }

        private void PessoaJuridicaChecked_Click(object sender, EventArgs e)
        {
            if(PessoaJuridicaChecked.Checked)
            {
                CPFCadastro.Text = "";
                PessoaJuridicaClickConfigura();
            }
        }
        private void PessoaJuridicaClickConfigura()
        {
            CPFCadastro.Enabled = false;
            PessoaFisicaChecked.Enabled = true;
            PessoaFisicaChecked.CheckState = CheckState.Unchecked;
            CNPJCadastro.Enabled = true;
            PessoaJuridicaChecked.Enabled = false;
            RazaoSocialCadastro.Enabled = true;
        }
        private void PessoaFisicaClickConfigura()
        {
            CPFCadastro.Enabled = true;
            PessoaFisicaChecked.Enabled = false;
            CNPJCadastro.Enabled = false;
            PessoaJuridicaChecked.Enabled = true;
            PessoaJuridicaChecked.CheckState = CheckState.Unchecked;
            RazaoSocialCadastro.Enabled = false;
        }
        public string VerificaBuscaCPFouCNPJ(object[][] pesquisa)
        {
            bool cpf = false;
            bool cnpj = false;
            string resultado = null;
            foreach(object[] val in pesquisa)
            {
                if(val[1].ToString() == "NULL")
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
            
            banco.conectar();
            banco.DeleteLinha(textBox1.Text, Convert.ToInt16(textBox2.Text));
            banco.desconectar();
            

        }
        

        private void BtAtualizarCadastro_Click(object sender, EventArgs e)
        {
            string[] CamposAInserir = new string[] { "cpf,cnpj,razaosocial,nome,telefone,whatsapp,email,endereço,sexo",
                                                     "idcadastro,equipamento,valorcompra,valorserviço,estado,defeito" };
            string[] ValorCamposAInserir = new string[1];
            bool resultado = false;
            string Existe = null;
            string ValorExiste = null;

            if (!String.IsNullOrWhiteSpace(NomeCadastro.Text)    &&
               !String.IsNullOrWhiteSpace(TelefoneCadastro.Text) &&
               !String.IsNullOrWhiteSpace(WhatsAppCadastro.Text) &&
               !String.IsNullOrWhiteSpace(EmailCadastro.Text)    &&
               !String.IsNullOrWhiteSpace(EndereçoCadastro.Text) &&
               !String.IsNullOrWhiteSpace(SexoCadastro.Text) )
            {
                if (PessoaFisicaChecked.Checked || PessoaJuridicaChecked.Checked)
                {
                    if (!String.IsNullOrWhiteSpace(CPFCadastro.Text) || !String.IsNullOrWhiteSpace(CNPJCadastro.Text))
                    {
                        banco.conectar();
                        if (PessoaFisicaChecked.Checked)
                        {
                            ValorCamposAInserir[0] = "'" + CPFCadastro.Text + "','" +
                                                      "NULL" + "','" +
                                                      "NULL" + "','" +
                                                      NomeCadastro.Text + "','" +
                                                      TelefoneCadastro.Text + "','" +
                                                      WhatsAppCadastro.Text + "','" +
                                                      EmailCadastro.Text + "','" +
                                                      EndereçoCadastro.Text + "','" +
                                                      SexoCadastro.Text + "'";
                            ValorExiste = CPFCadastro.Text;
                            resultado = banco.PesquisaSeExiste("cadastro", "cpf", ValorExiste);
                            Existe = "cpf";
                            //   cpf,cnpj,razaosocial,nome,telefone,whatsap,endereço,sexo

                        }
                        else
                        {
                            ValorCamposAInserir[0] = "'" + "NULL" + "','" +
                                                       CNPJCadastro.Text + "','" +
                                                       RazaoSocialCadastro.Text + "','" +
                                                       NomeCadastro.Text + "','" +
                                                       TelefoneCadastro.Text + "','" +
                                                       WhatsAppCadastro.Text + "','" +
                                                       EmailCadastro.Text + "','" +
                                                       EndereçoCadastro.Text + "','" +
                                                       SexoCadastro.Text + "'";
                            ValorExiste = CNPJCadastro.Text;
                            resultado = banco.PesquisaSeExiste("cadastro", "cnpj", ValorExiste);
                            Existe = "cnpj";
                        }

                        if (resultado)
                        {
                            resultado = banco.AtualizarOnde("cadastro", CamposAInserir[0], ValorCamposAInserir[0], Existe, ValorExiste);
                            if (resultado)
                            {
                                MessageBox.Show("Cliente atualizado com sucesso!", @"SiehaR&C", MessageBoxButtons.OK);
                            }
                            else
                            {
                                MessageBox.Show("Erro ao atualizar o cliente.", "Erro!", MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            MessageBox.Show(@"Não existe um cadastro com este " + Existe + "!", "Erro!", MessageBoxButtons.OK);
                        }

                        banco.desconectar();
                    }
                    else
                    {
                        MessageBox.Show("O Campo CPF ou CNPJ esta vazio!", "Erro!", MessageBoxButtons.OK);
                    }
                    
                }
                else
                {
                    MessageBox.Show(@"Selecione pessoa física ou jurídica.", "Erro!", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Algum dado está em branco.", "Erro!", MessageBoxButtons.OK);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            object[][] Cadastro,serviço,Mix = null;
            string[] CamposBanco = new string[] { "id,nome,endereço" ,"id,equipamento", "IdCliente,IdServiço,equipamento,nome,endereço" };
            string[] nomesDGV;
            string onde = TipoBuscaServiços.Text;
            string valor = ValorBuscaServiços.Text;
            List<object[]> lista = new List<object[]>();

            GradeDeBuscaServiços.DataSource = null;
            GradeDeBuscaServiços.Rows.Clear();
            GradeDeBuscaServiços.Columns.Clear();

            banco.conectar();
            Cadastro = banco.PesquisarOnde("cadastro", CamposBanco[0], onde, valor);
            banco.desconectar();

            foreach(object[] cad in Cadastro)
            {
                if (!EstaNullOuZero(Cadastro))
                {


                    //id,equipamento,nome,endereço
                    banco.conectar();
                    serviço = banco.PesquisarOnde("equipamento", CamposBanco[1], "idcadastro", Convert.ToString(cad[0]));
                    banco.desconectar();
                    lista.Clear();
                    foreach (object[] serv in serviço)
                    {
                        object[] Condicionamento = new object[] { cad[0], serv[0],serv[1], cad[1], cad[2] };
                        lista.Add(Condicionamento);
                    }
                    Mix = lista.ToArray();

                    nomesDGV = CamposBanco[2].Split(',');
                    GradeDeBuscaServiços.ColumnCount = nomesDGV.Length;
                    for (int ciclo = 0; ciclo < GradeDeBuscaServiços.ColumnCount; ciclo++)
                    {
                        GradeDeBuscaServiços.Columns[ciclo].Width = (int)(690 / GradeDeBuscaServiços.ColumnCount);
                        GradeDeBuscaServiços.Columns[ciclo].Name = nomesDGV[ciclo];
                    }
                    
                    foreach (object[] tab in Mix)
                    {
                        GradeDeBuscaServiços.Rows.Add(tab);
                    }
                }
            }
           
        }
        private void GradeDeBuscaClientesDuploClick(object sender, DataGridViewCellEventArgs e)
        {
            string valor = null;
            string tipo = null;
            string testelinha = null;
            string testecoluna = null;
            string pesquisa = null;
            object[][] resultado = null;

            if (!GradeDeBuscaClientes.CurrentRow.IsNewRow) // inibe a ultima linha em branco de ser selecionada
            {
                testelinha = GradeDeBuscaClientes.CurrentRow.Cells[0].Value.ToString();
                testecoluna = GradeDeBuscaClientes.Columns[0].Name;
                if (testecoluna == "cnpj")
                {
                    valor = GradeDeBuscaClientes.CurrentRow.Cells[0].Value.ToString();
                    tipo = "cnpj";
                    pesquisa = "cnpj,razaosocial,nome,telefone,whatsapp,email,endereço,sexo";
                }
                else
                {
                    if (testelinha == "NULL")
                    {
                        valor = GradeDeBuscaClientes.CurrentRow.Cells[1].Value.ToString();
                        tipo = "cnpj";
                        pesquisa = "cnpj,razaosocial,nome,telefone,whatsapp,email,endereço,sexo";
                    }
                    else
                    {
                        valor = GradeDeBuscaClientes.CurrentRow.Cells[0].Value.ToString();
                        tipo = "cpf";
                        pesquisa = "cpf,nome,telefone,whatsapp,email,endereço,sexo";
                    }

                }

                banco.conectar();
                resultado = banco.PesquisarOnde("cadastro", pesquisa, tipo, valor);
                banco.desconectar();

                if (!EstaNullOuZero(resultado))
                {
                    if (tipo == "cpf")
                    {
                        CPFCadastro.Text = resultado[0][0].ToString();
                        CNPJCadastro.Text = "";
                        RazaoSocialCadastro.Text = "";
                        NomeCadastro.Text = resultado[0][1].ToString();
                        TelefoneCadastro.Text = resultado[0][2].ToString();
                        WhatsAppCadastro.Text = resultado[0][3].ToString();
                        EmailCadastro.Text = resultado[0][4].ToString();
                        EndereçoCadastro.Text = resultado[0][5].ToString();
                        SexoCadastro.Text = resultado[0][6].ToString();

                        PessoaFisicaChecked.CheckState = CheckState.Checked;
                        PessoaFisicaClickConfigura();
                    }
                    else
                    {
                        CPFCadastro.Text = "";
                        CNPJCadastro.Text = resultado[0][0].ToString();
                        RazaoSocialCadastro.Text = resultado[0][1].ToString();
                        NomeCadastro.Text = resultado[0][2].ToString();
                        TelefoneCadastro.Text = resultado[0][3].ToString();
                        WhatsAppCadastro.Text = resultado[0][4].ToString();
                        EmailCadastro.Text = resultado[0][5].ToString();
                        EndereçoCadastro.Text = resultado[0][6].ToString();
                        SexoCadastro.Text = resultado[0][7].ToString();

                        PessoaJuridicaChecked.CheckState = CheckState.Checked;
                        PessoaJuridicaClickConfigura();
                    }
                }
                else
                {
                    MessageBox.Show("Nao foi retornado nenhum valor", "Erro!", MessageBoxButtons.OK);
                }
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            object[][] val;
            string CamposBanco = "nome,endereço";
            string onde = "id";
            string valor = IdClienteServiços.Text;
            
            if(!String.IsNullOrEmpty(valor))
            {
                banco.conectar();
                val = banco.PesquisarOnde("cadastro", CamposBanco, onde, valor);
                banco.desconectar();

                if (!EstaNullOuZero(val))
                {
                    NomeClienteServiços.Text     = val[0][0].ToString();
                    EndereçoClienteServiços.Text = val[0][1].ToString();
                    HabilitaBotoesDeServiço();
                    HabilitaDadosDeCadastrodeDeServiço();
                }
                else
                {
                    MessageBox.Show(banco.retornoDaQuery, "Erro!", MessageBoxButtons.OK);
                }
                
            }
            else
            {
                MessageBox.Show("O id nao pode estar vazio!", "Erro!", MessageBoxButtons.OK);
            }
            
        }

        private void BtNovoServiços_Click(object sender, EventArgs e)
        {
            string[] CamposAInserir = new string[] {"idcadastro,equipamento,valorcompra,valorserviço,estado,defeito" };
            string[] ValorCamposAInserir = new string[1];
            string estado;
            bool resultado = false;
            string Existe = null;

            if (!String.IsNullOrWhiteSpace(EquipamentoServiços.Text) &&
               !String.IsNullOrWhiteSpace(ValorCompraServiços.Text) &&
               !String.IsNullOrWhiteSpace(ValorServiçoServiços.Text) &&
               !String.IsNullOrWhiteSpace(EquipDefeitoServiços.Text) &&
               !String.IsNullOrWhiteSpace(IdClienteServiços.Text) &&
               (EquipNovoServiços.Checked ^ EquipUsadoServiços.Checked))
            {
                
                banco.conectar();
                
                resultado = banco.PesquisaSeExiste("cadastro", "id", IdClienteServiços.Text);
                Existe = "Id";
                if (resultado)
                {
                    if (EquipNovoServiços.Checked)
                    {
                        estado = "NOVO";
                    }
                    else
                    {
                        estado = "USADO";
                    }
                    
                    ValorCamposAInserir[0] = "'" +  IdClienteServiços.Text + "','" +
                                                    EquipamentoServiços.Text + "','" +
                                                    ValorCompraServiços.Text.Replace(",", ".") + "','" +
                                                    ValorServiçoServiços.Text.Replace(",", ".") + "','" +
                                                    estado + "','" +
                                                    EquipDefeitoServiços.Text + "'";

                    
                    resultado = banco.InserirEm("equipamento", CamposAInserir[0], ValorCamposAInserir[0]);
                    if (resultado)
                    {
                        MessageBox.Show("Serviço cadastrado com sucesso!", @"SiehaR&C", MessageBoxButtons.OK);
                        ApagaDadosDeCadastrodeDeServiço();
                    }
                    else
                    {
                        MessageBox.Show("Erro ao cadastrar Serviço do cliente.", "Erro!", MessageBoxButtons.OK);
                    }
                   
                }
                else
                {
                    MessageBox.Show("Nao existe um cadastro com este " + Existe + "!", "Erro!", MessageBoxButtons.OK);
                }

                banco.desconectar();
                
            }
            else
            {
                MessageBox.Show("Algum dado está em branco.", "Erro!", MessageBoxButtons.OK);
            }
        }
    }
}
