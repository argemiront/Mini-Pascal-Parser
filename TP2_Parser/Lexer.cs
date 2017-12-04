using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace TP2_Parser
{
    class Lexer
    {
        StringReader programa;
        public Token Lookahed { get; set; }
        public int NrLinha { get; set; }
        public int NrColuna { get; set; }
        private int colAnterior;

        public Lexer(string prog)
        {
            programa = new StringReader(prog);

            NrLinha = 1;
            NrColuna = 1;
        }

        public bool NextToken()
        {
            StringBuilder tempText = new StringBuilder();
            string caractere = "";
            bool tokenOK = false;

            //Busca no texto até encontrar um token válido ou erro
            do
            {
                #region Trata os literais
                //TODO: verificar se será necessário alterar para apenas 1 caractere
                //Verifica se temos um literal = tratamos o literal
                if (programa.Peek() == 39)  //se o próximo caractere é '
                {
                    int tempCar;

                    while (true)
                    {
                        caractere += NextChar();

                        tempCar = programa.Peek();

                        if (tempCar == 39) //se é a aspa simples de fechamento
                        {
                            caractere += NextChar(); //armazeno em caractere e encerro o loop
                            tempText.Append(caractere);
                            break;
                        }

                        //Se encontramos o final do arquivo antes do literal acabar, então, ERRO
                        if (tempCar == -1)
                        {
                            Lookahed = new Token(Type.NOK, "");
                            return false;
                        }
                    }

                    //Encerro o loop principal com caractere contedo o literal completo.
                    break;
                }
                #endregion

                #region Trata a leitura de um caractere válido
                //Faz a leitura de um caractere da entrada e o trata
                string tempCaractere = NextChar();

                if (tempCaractere == "EOF")
                {
                    if (tempText.Length == 0)
                        return false;
                    else
                        break;
                }

                caractere = tempCaractere;

                //Pula os \s enquanto não existe texto nenhum
                if (Regex.IsMatch(caractere, @"\s") && tempText.Length == 0)
                    continue;

                #endregion

                #region Trata os Comentários
                //Se for comentário... eliminamos
                if (caractere == @"/")
                {
                    int LA = programa.Peek();

                    //Comentário do tipo: // <== consome entrada até a próxima linha
                    if (LA == 47) //47 = /
                    {
                        string temp;

                        do
	                    {
	                        temp = NextChar();

                            if (temp == "EOF")
                            {
                                break;
                            }

	                    } while (temp != "\n");

                        //Começo a ler novamente sem o comentário
                        continue;
                    }
                    //Comentário do tipo: /* */
                    else if (LA == 42) //42 = *
                    {
                        string strAtual = Convert.ToChar(47).ToString();
                        string strProx = NextChar();

                        if (strProx == "EOF")
                        {
                            //Trata fim do arquivo
                            break;
                        }

                        do
                        {
                            strAtual = strProx;
                            strProx = NextChar();

                            if (strProx == "EOF")
                            {
                                //Trata fim do arquivo
                                break;
                            }
                        }
                        while ((strAtual + strProx) != @"*/");

                        if (strProx == "EOF")
                        {
                            //Trata fim do arquivo
                            break;
                        }
                        //Começo a ler novamente sem o comentário
                        continue;
                    }
                }
                else if (caractere == "{")  //TP3//
                {
                    string temp = "";

                    do
                    {
                        temp = NextChar();

                        if (temp == "EOF")
                        {
                            //TODO: trata fim do arquivo e lança erro
                            //Programa finalizado sem fechar os comentários
                            break;
                        }
 
                    } while (temp != "}");
                }
                #endregion

                #region Trata os delimitadores
                if (Regex.IsMatch(caractere, @"" + Resource1.STR_DELIMITADORES) && tempText.Length == 0)
                {
                    int tempCaractereInt = programa.Peek();

                    if (tempCaractereInt == -1)
                    {
                        //trata fim do arquivo
                        tempText.Append(caractere);
                        break;
                    }

                    var LAChar = Convert.ToChar(tempCaractereInt).ToString();

                    //Vericando se continuo a ler ou não
                    switch (caractere + LAChar)
                    {
                        case ":=":
                            caractere += NextChar();
                            tokenOK = true;
                            break;

                        case "<=":
                            caractere += NextChar();
                            tokenOK = true;
                            break;

                        case "<>":
                            caractere += NextChar();
                            tokenOK = true;
                            break;

                        case ">=":
                            caractere += NextChar();
                            tokenOK = true;
                            break;

                        case "..":
                            caractere += NextChar();
                            tokenOK = true;
                            break;

                        default:
                            tokenOK = true;
                            break;
                    }
                }
                else if (Regex.IsMatch(caractere, @"" + Resource1.STR_DELIMITADORES) && tempText.Length != 0)
                {
                    programa = new StringReader(caractere + programa.ReadToEnd());
                    caractere = "";
                    tokenOK = true;

                    if (caractere == @"\n")
                    {
                        NrLinha--;
                        NrColuna = colAnterior;
                    }
                    else
                        NrColuna--;
                }

                tempText.Append(caractere);
                #endregion

            } while (!tokenOK);

            #region Faz a Validação dos Dados
            //Valida o token lido
            Lookahed = Token.ValidaToken(tempText.ToString());

            if (Lookahed.TypeOf != Type.NOK)
                return true;
            else
                return false;

            #endregion
        }

        private string NextChar()
        {
            string retorno;

            //Lê o próximo caractere
            int tempCar = programa.Read();

            //Incrementa o contador de colunas
            NrColuna++;

            //Se é quebra de linha incrementa a linha e zera coluna
            if (tempCar == 10)
            {
                colAnterior = NrColuna;
                NrLinha++;
                NrColuna = 1;
            }

            //Se fim do arquivo encontrado, retorna 'EOF'
            if (tempCar == -1)
                return "EOF";

            //Converte o caractere em string para retorno
            retorno = Convert.ToChar(tempCar).ToString();

            return retorno;
        }
    }
}
