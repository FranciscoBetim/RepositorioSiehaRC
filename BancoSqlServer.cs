using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiehaRC
{
    class BancoSqlServer
    {
        private string NomeDoBanco;
        private string InstanciaDoBanco;
        private string LoginSqlServer;
        private string PasswordSqlServer;
        private bool EstaConectado;
        private string QueryDeEnvio;
        private string RetornoDaQuery;
        private string ComandoDaQuery;
        private SqlException UltimoErroSQL;
        private SqlConnection UltimaConexao;
        private SqlCommand UltimoComando;

        public BancoSqlServer(string instancia, string nomebanco, string login, string password)
        {
            this.NomeDoBanco = nomebanco;
            this.InstanciaDoBanco = instancia;
            this.LoginSqlServer = login;
            this.PasswordSqlServer = password;
        }

        public bool conectar()
        {
            try
            {
                this.UltimaConexao = new SqlConnection(@"Data Source=" + this.InstanciaDoBanco + ";Initial Catalog=" + this.NomeDoBanco + ";User ID=" + this.LoginSqlServer + ";Password=" + this.PasswordSqlServer);
                this.UltimaConexao.Open();
                this.EstaConectado = true;
            }
            catch (SqlException erro)
            {
                this.UltimoErroSQL = erro;
                this.EstaConectado = false;
            }

            return (this.EstaConectado);
        }

        public bool desconectar()
        {
            if (this.EstaConectado)
            {
                try
                {
                    this.UltimaConexao.Close();
                    this.EstaConectado = false;
                }
                catch (SqlException erro)
                {
                    this.UltimoErroSQL = erro;
                    this.EstaConectado = true;
                }
            }
            return (!this.EstaConectado);
        }

        public bool InserirEm(string Tabela, string campos, string valores)
        {
            bool retorno = false;
            int LinhasAfetadas = 0;
            if (this.EstaConectado)
            {
                this.ComandoDaQuery = "InserirEm";
                this.UltimoComando = new SqlCommand();

                this.UltimoComando.CommandText = "INSERT INTO " + Tabela + "(" + campos + ") VALUES(";

                foreach (string val in valores.Split(','))
                {
                    this.UltimoComando.CommandText += val + ",";
                }

                this.UltimoComando.CommandText = this.UltimoComando.CommandText.Remove(this.UltimoComando.CommandText.LastIndexOf(','), 1);
                this.UltimoComando.CommandText += ")";
                this.QueryDeEnvio = this.UltimoComando.CommandText;

                try
                {
                    this.UltimoComando.Connection = this.UltimaConexao;
                    LinhasAfetadas = this.UltimoComando.ExecuteNonQuery();
                    if (LinhasAfetadas > 0)
                    {
                        this.RetornoDaQuery = "(mensagemInternaSQL)-> " + LinhasAfetadas.ToString() + " Linhas foram inseridas com " + valores.Split(',').Count() + " colunas da tabela " + Tabela;
                        retorno = true;
                    }
                    else
                    {
                        this.RetornoDaQuery = "(mensagemInternaSQL)-> Nenhuma linha foi inserida na tabela " + Tabela;
                        retorno = false;
                    }

                }
                catch (SqlException erro)
                {
                    this.UltimoErroSQL = erro;
                    this.RetornoDaQuery = "SqlException erro";
                    retorno = false;
                }

            }

            return (retorno);
        }

        public object[][] PesquisarOnde(string Tabela, string ColunasDaPesquisa, string onde, string valor)
        {
            object[][] RetornoDaPesquisa = null;

            if (this.EstaConectado)
            {
                this.ComandoDaQuery = "PesquisarOnde";
                this.UltimoComando = new SqlCommand();

                if (ColunasDaPesquisa.ToUpper() == "TUDO")
                {
                    this.UltimoComando.CommandText = "SELECT * FROM " + Tabela;
                    this.RetornoDaQuery = "Todas colunas";
                }
                else
                {
                    this.UltimoComando.CommandText = "SELECT " + ColunasDaPesquisa + " FROM " + Tabela;
                    this.RetornoDaQuery = ColunasDaPesquisa;
                }
                if (onde != "")
                {
                    if (VerificaNumerico(valor) && !VerificaCPForCNPJorTelefone(valor))
                    {
                        if ((Convert.ToDouble(valor.Replace(".", ",")) % 1) > 0)
                        {
                            this.UltimoComando.CommandText += " WHERE ROUND(" + onde + ",13) = " + valor + " ";
                        }
                        else
                        {
                            this.UltimoComando.CommandText += " WHERE " + onde + " = '" + valor + "' ";
                        }
                        this.RetornoDaQuery += " || onde " + onde + " = '" + valor + "'";
                    }
                    else
                    {
                        this.UltimoComando.CommandText += " WHERE " + onde + " = '" + valor + "' ";
                        this.RetornoDaQuery += " || onde " + onde + " = '" + valor + "' ";
                    }

                }
                this.QueryDeEnvio = this.UltimoComando.CommandText;
                RetornoDaPesquisa = PesquisarEm(Tabela, ColunasDaPesquisa);
            }
            return (RetornoDaPesquisa);
        }
        public object[][] PesquisarOnde(string Tabela, string ColunasDaPesquisa)
        {
            object[][] RetornoDaPesquisa = null;

            RetornoDaPesquisa = PesquisarOnde(Tabela, ColunasDaPesquisa, "", "");

            return (RetornoDaPesquisa);
        }

        private object[][] PesquisarEm(string Tabela, string ColunasDaTabela)
        {
            int NumeroDeLinhas;
            int NumeroDeColunas = 0;
            int Verifica = 0;
            object[][] RetornoDaPesquisa = null;
            SqlDataReader resposta;

            try
            {
                this.UltimoComando.Connection = this.UltimaConexao;

                NumeroDeLinhas = 0;

                for (int ciclo = 0; ciclo < 2; ciclo++)
                {
                    resposta = this.UltimoComando.ExecuteReader();

                    while (resposta.Read())
                    {
                        if (ciclo > 0)
                        {
                            RetornoDaPesquisa[NumeroDeLinhas] = new object[NumeroDeColunas];
                            resposta.GetValues(RetornoDaPesquisa[NumeroDeLinhas]);
                        }
                        NumeroDeLinhas++;
                    }
                    if (ciclo == 0)
                    {
                        RetornoDaPesquisa = new object[NumeroDeLinhas][];
                        Verifica = NumeroDeLinhas;
                        NumeroDeLinhas = 0;
                        NumeroDeColunas = resposta.FieldCount;
                    }

                    resposta.Close();
                }

                if (Verifica == NumeroDeLinhas)
                {
                    if (Verifica == 0)
                    {
                        this.RetornoDaQuery = "(mensagemInternaSQL)-> " + this.RetornoDaQuery + @": Nao foi encontrada nenhuma referencia com a condiçao desejada na tabela " + Tabela + "!";
                    }
                    else
                    {
                        this.RetornoDaQuery = "(mensagemInternaSQL)-> " + this.RetornoDaQuery + ": as " + NumeroDeColunas + " colunas e as " + NumeroDeLinhas + " linhas da tabela '" + Tabela + "' do banco '" + this.NomeDoBanco + "' foram retornadas com sucesso!";
                    }

                }
                else
                {
                    this.RetornoDaQuery = "(mensagemInternaSQL)-> " + "Pesquisa invalida, o numero de Linhas mudou de " + Verifica + " para " + NumeroDeLinhas + " na tabela " + Tabela + " durante a pesquisa!";
                }

            }
            catch (SqlException erro)
            {
                this.UltimoErroSQL = erro;
                this.RetornoDaQuery = "SqlException erro";
            }


            return (RetornoDaPesquisa);
        }
        
        public object[] PesquisarUltimaLinha(string Tabela, string ColunasDaPesquisa)
        {
            object[][] RetornoDaPesquisa = null;
            object[] UltimaLinha = null;
            RetornoDaPesquisa = PesquisarOnde(Tabela, ColunasDaPesquisa, "", "");
            UltimaLinha = RetornoDaPesquisa[RetornoDaPesquisa.Length-1];

            return (UltimaLinha);
        }
        public object[] PesquisarPrimeiraLinha(string Tabela, string ColunasDaPesquisa)
        {
            object[][] RetornoDaPesquisa = null;
            object[] UltimaLinha = null;
            RetornoDaPesquisa = PesquisarOnde(Tabela, ColunasDaPesquisa, "", "");
            UltimaLinha = RetornoDaPesquisa[0];

            return (UltimaLinha);
        }
        public bool PesquisaSeExiste(string Tabela, string Coluna, string ValorColuna)
        {
            object[][] RetornoDaPesquisa = null;
            bool resultado = false;
            RetornoDaPesquisa = PesquisarOnde(Tabela, Coluna, Coluna, ValorColuna);
            if(RetornoDaPesquisa.Length>0)
            {
                resultado = true;
            }

            return (resultado);
        }

        public int PesquisarUltimoIdAutoIncremento(string Tabela)
        {
            int AutoIncremento = 0;
            object[] RetornoDaPesquisa = new object[1];
            SqlDataReader resposta;

            this.ComandoDaQuery = "PegaUltimoAutoIncremento";
            this.UltimoComando = new SqlCommand();
            this.UltimoComando.CommandText = "SELECT IDENT_CURRENT('"+Tabela+"')" ;
            this.QueryDeEnvio = this.UltimoComando.CommandText;
            try
            {
                this.UltimoComando.Connection = this.UltimaConexao;
                resposta = this.UltimoComando.ExecuteReader();
                if (resposta.Read())
                {
                    resposta.GetValues(RetornoDaPesquisa);
                    AutoIncremento = Convert.ToInt16(RetornoDaPesquisa[0]);
                }
                else
                {

                    AutoIncremento = -1;
                    this.RetornoDaQuery = "(mensagemInternaSQL)-> Nao houve retorno de auto incremento!";
                }
                resposta.Close();
            }
            catch (SqlException erro)
            {
                this.UltimoErroSQL = erro;
                this.RetornoDaQuery = "SqlException erro";
                AutoIncremento = -1;
            }

            return (AutoIncremento);
        }

        public bool DeleteTodosValoresDaTabela(string Tabela)
        {
            bool retorno = false;

            this.ComandoDaQuery = "DeleteTodosValoresDaTabela";
            this.UltimoComando = new SqlCommand();
            this.UltimoComando.CommandText = "DELETE FROM " + Tabela + ";" + "DBCC CHECKIDENT('" + Tabela + "', RESEED, 0);";
            this.QueryDeEnvio = this.UltimoComando.CommandText;
            retorno = DeleteEm(Tabela, "Cada linha existente");

            return (retorno);
        }
        public bool DeleteLinha(string Tabela, int linha)
        {
            bool retorno = false;
            object[][] procura = null;

            this.ComandoDaQuery = "DeleteLinha";
            this.UltimoComando = new SqlCommand();
            procura = PesquisarOnde(Tabela, "id");
            this.UltimoComando.CommandText = "DELETE FROM " + Tabela + " WHERE id = " + procura[linha - 1][0].ToString();
            this.QueryDeEnvio = this.UltimoComando.CommandText;
            retorno = DeleteEm(Tabela, "Linha: " + linha);

            return (retorno);
        }
        public bool DeleteOnde(string Tabela, string Coluna, string valColuna)
        {
            bool retorno = false;

            this.ComandoDaQuery = "DeleteOnde";
            this.UltimoComando = new SqlCommand();
            this.UltimoComando.CommandText = "DELETE FROM " + Tabela + " WHERE " + Coluna + " = " + valColuna;
            this.QueryDeEnvio = this.UltimoComando.CommandText;
            retorno = DeleteEm(Tabela, Coluna + ": " + valColuna);

            return (retorno);
        }
        private bool DeleteEm(string Tabela, string condicao)
        {
            bool retorno = false;
            int LinhasAfetadas = 0;

            try
            {
                this.UltimoComando.Connection = this.UltimaConexao;

                LinhasAfetadas = this.UltimoComando.ExecuteNonQuery();
                if (LinhasAfetadas > 0)
                {
                    if (this.ComandoDaQuery == "DeleteLinha")
                    {
                        this.RetornoDaQuery = "(mensagemInternaSQL)-> 1 Linha com " + condicao + " foi deletada da tabela " + Tabela + " com sucesso!";
                    }
                    else
                    {
                        if (this.ComandoDaQuery == "DeleteTodosValoresDaTabela")
                        {
                            this.RetornoDaQuery = "(mensagemInternaSQL)-> Haviam " + LinhasAfetadas + " Linhas, " + condicao + " foi deletada da tabela " + Tabela + " com sucesso!";
                        }
                        else
                        {
                            this.RetornoDaQuery = "(mensagemInternaSQL)-> Linha com " + condicao + " foi deletada da tabela " + Tabela + " com sucesso!";
                        }
                    }
                    retorno = true;
                }
                else
                {
                    if (this.ComandoDaQuery == "DeleteTodosValoresDaTabela")
                    {
                        this.RetornoDaQuery = "(mensagemInternaSQL)-> Nenhuma linha foi deletada da tabela " + Tabela + " pois a mesma estava vazia!";
                    }
                    else
                    {
                        this.RetornoDaQuery = "(mensagemInternaSQL)-> Nenhuma linha foi deletada da tabela " + Tabela + ", " + condicao + " nao existe!";
                    }
                    retorno = false;
                }


            }
            catch (SqlException erro)
            {
                this.UltimoErroSQL = erro;
                this.RetornoDaQuery = "SqlException erro";
                retorno = false;
            }

            return (retorno);
        }

        public bool AtualizarOnde(string Tabela, string Colunas_a_Atualizar, string Valor_a_Atualizar, string onde, string valor)
        {
            bool retorno = false;
            int LinhasAfetadas = 0;
            string Atualizar = "";
            string[] ColAtualizar;
            string[] ValAtualizar;

            if ((this.EstaConectado) && (!String.IsNullOrEmpty(onde)) && (!String.IsNullOrEmpty(valor)))
            {
                if (Colunas_a_Atualizar.Split(',').Length == Valor_a_Atualizar.Split(',').Length)
                {
                    ColAtualizar = Colunas_a_Atualizar.Split(',');
                    ValAtualizar = Valor_a_Atualizar.Split(',');
                    for (int ciclo = 0; ciclo < Valor_a_Atualizar.Split(',').Length; ciclo++)
                    {
                        Atualizar += ColAtualizar[ciclo] + " = " + ValAtualizar[ciclo] + ", ";
                    }
                    Atualizar = Atualizar.Remove(Atualizar.LastIndexOf(','), 1);

                }

                this.ComandoDaQuery = "AtualizarOnde";
                this.UltimoComando = new SqlCommand();

                this.UltimoComando.CommandText = "UPDATE " + Tabela + " SET " + Atualizar;
                this.RetornoDaQuery = Colunas_a_Atualizar;
                if (VerificaNumerico(valor) && !VerificaCPForCNPJorTelefone(valor))
                {
                    if( (Convert.ToDouble(valor.Replace(".",",")) % 1) > 0 )
                    {
                        this.UltimoComando.CommandText += " WHERE ROUND(" + onde + ",13) = " + valor + " ";
                    }
                    else
                    {
                        this.UltimoComando.CommandText += " WHERE " + onde + " = '" + valor + "' ";
                    }
                    this.RetornoDaQuery += " || onde " + onde + " = '" + valor + "'";
                }
                else
                {
                    this.UltimoComando.CommandText += " WHERE " + onde + " = '" + valor + "' ";
                    this.RetornoDaQuery += " || onde " + onde + " = '" + valor + "' ";
                }
                this.QueryDeEnvio = this.UltimoComando.CommandText;

                try
                {
                    this.UltimoComando.Connection = this.UltimaConexao;
                    LinhasAfetadas = this.UltimoComando.ExecuteNonQuery();
                    if (LinhasAfetadas > 0)
                    {
                        this.RetornoDaQuery = "(mensagemInternaSQL)-> " + this.RetornoDaQuery + ": " + LinhasAfetadas.ToString() + " Linhas foram Atualizadas com " + Colunas_a_Atualizar.Split(',').Count() + " colunas na tabela " + Tabela;
                        retorno = true;
                    }
                    else
                    {
                        this.RetornoDaQuery = "(mensagemInternaSQL)-> " + this.RetornoDaQuery + ": " + "Nenhuma linha foi Atualizada na tabela " + Tabela;
                        retorno = false;
                    }



                }
                catch (SqlException erro)
                {
                    this.UltimoErroSQL = erro;
                    this.RetornoDaQuery = "SqlException erro";
                    retorno = false;
                }

            }

            return (retorno);
        }

        private static bool VerificaNumerico(string s)
        {
            float output;
            return float.TryParse(s, out output);
        }
        private static bool VerificaCPForCNPJorTelefone(string s)
        {
            long numero;
            if ( (Convert.ToDouble(s.Replace(".",",")) % 1)>0)
            {
                numero = 1;
            }
            else
            {
                numero = Convert.ToInt64(s);
            }
            
            return numero > 11111111;
        }
        
        public string nomeDoBanco { get => NomeDoBanco; }
        public string instanciaDoBanco { get => InstanciaDoBanco; }
        public string loginSqlServer { get => LoginSqlServer; }
        public string passwordSqlServer { get => PasswordSqlServer; }
        public bool estaConectado { get => EstaConectado; }
        public string queryDeEnvio { get => QueryDeEnvio; }
        public string retornoDaQuery { get => RetornoDaQuery; }
        public string comandoDaQuery { get => ComandoDaQuery; }
        public SqlConnection ultimaConexao { get => UltimaConexao; }
        public SqlException ultimoErroSQL { get => UltimoErroSQL; }
        public SqlCommand ultimoComando { get => UltimoComando; }
    }
}
