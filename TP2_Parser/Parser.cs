using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;
using System.IO;

namespace TP2_Parser
{
    struct RetCpo
    {
        public Types type;
        public int Escopo;
        public string Lexem;
        public string False;
        public string True;
        public string Next;

        //public RetCpo()
        //{
        //    cod = new List<string>();
        //    type = Types.NOK;
        //    Escopo = 0;
        //    Lexem = "";
        //}
    }

    enum Types
    {
        LIT,
        INTEGER,
        CHAR,
        BOOLEAN,
        RECORD,
        NOK
    }

    class Parser
    {
        List<Dictionary<string, string>> tabSimb = new List<Dictionary<string, string>>(); //Nome, tipo : variável
        Lexer lexer;
        static int lblCount = 0;
        static int tempCount = 0;
        List<string> codigo = new List<string>();
        string tab = "\t";
        string esp = " ";
        int escopo = 0;

        public Parser(Lexer lex)
        {
            lexer = lex;
        }

        static private string NextLabel()
        {
            return "L" + (++lblCount).ToString();
        }

        static private string NextTemp()
        {
            return "t" + (++tempCount).ToString();
        }

        public bool start() 
        {
            codigo.Clear();
            lblCount = 0;
            tempCount = 0;

            tabSimb.Add(new Dictionary<string, string>());

            try
            {       
                    // start     	: 'PROGRAM' ident ';' decl subProgDecls compoundStm '.'
                    //              ;
                
                Match("PROGRAM");
                Ident();
                Match(";");

                //<Analisador Sintático>
                codigo.Add("int main()");
                codigo.Add("{");
                //<!Analisador Sintático>
                
                //<Analisador Sintático>
                var temp1 = Decl();
                codigo.Add(temp1.Lexem);
                //<!Analisador Sintático>

                SubProgDecls();

                //<Analisador Sintático>
                var temp2 = CompoundStm();
                //codigo.Add(temp2.Lexem); TODO: verificar se é aqui ou não
                //<Analisador Sintático>

                Match(".");

                //<Analisador Sintático>
                codigo.Add("system(\"pause\");");
                codigo.Add("return 0;");
                codigo.Add("}");
                //<!Analisador Sintático>

                //Salvando no arquivo
                TextWriter arquivo = new StreamWriter("main.cpp");

                string linhaOK;

                foreach (var linha in codigo)
                {
                    linhaOK = (linha.Contains("TRUE"))? linha.Replace("TRUE", "true") : linha;
                    linhaOK = (linhaOK.Contains("FALSE")) ? linhaOK.Replace("FALSE", "false") : linhaOK;

                    arquivo.WriteLine(linhaOK);
                }
                arquivo.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro no Programa", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            return true;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo Decl(RetCpo retorno = new RetCpo()) 
        {
            /*
                <Incluso>
                Decl		: decls  decl
		                    |
		                    ;
                */

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && (lexer.Lookahed.Lexem == "VAR" || lexer.Lookahed.Lexem == "CONST" || lexer.Lookahed.Lexem == "TYPE"))
            {
                var temp1 = Decls();
                var temp2 = Decl();

                retorno.Lexem = temp1.Lexem + temp2.Lexem;
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo Decls(RetCpo retorno = new RetCpo())
        {
            //RetCpo retorno = new RetCpo();

            //TODO: falta construir o resto dos retornos (CONST E TYPE)

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "VAR")
            {
                Match("VAR");
                retorno = VarDecls();
            }
            else
            {
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "CONST")
                {
                    Match("CONST");
                    retorno = ConstDecls();
                }
                else
                {
                    if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "TYPE")
                    {
                        Match("TYPE");
                        retorno = TypeDecls();
                    }
                    else
                    {
                        throw new Exception();//Erro, não temos uma KEY (palavra reservada VAR, CONST e TYPE).   
                    }
                }
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo IdentList(RetCpo retorno = new RetCpo())
        {       
                //identList 	: ident idLists
                //              ;
            //RetCpo retorno = new RetCpo();

            var temp1 = Ident();
            var temp2 = IdLists();

            retorno.Lexem = temp1.Lexem + temp2.Lexem;
            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo IdLists(RetCpo retorno = new RetCpo())
        {       
                // idLists   	: ',' ident idLists 
                //              | 
                //              ;
            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == ",")
            {
                Match(",");
                var temp1 = Ident();
                var temp2 = IdLists();

                //TODO: verificar se a ordem está correta
                retorno.Lexem = ", " + temp1.Lexem + temp2.Lexem;
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //Não usamos 
        public RetCpo ConstDecls(RetCpo retorno = new RetCpo()) 
        {
            /*
                <Incluso>
                constDecls	: ident '=' ConstintLit ';' ConstDecls
		                    |
		                    ;
             */

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
            {
                Ident();
                Match("=");
                ConstIntLit();
                Match(";");
                ConstDecls();

            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //Não usamos
        public RetCpo ConstIntLit(RetCpo retorno = new RetCpo())
        {
             /*
                <Incluso>
                ConstIntLit	: sign intConst
		                    | Lit
		                    ;
             */
            //RetCpo retorno = new RetCpo();

            if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.NUM) ||(lexer.Lookahed.TypeOf == TP2_Parser.Type.OP && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-")))
            {
                Sign();
                IntConst();
            }
            else
            {
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.LIT )
                {
                    Lit();
                }
                else
                {
                    throw new Exception();//Erro, não temos um +, ou -, ou literal.   
                }
            }

            return retorno;
        }

        //Não usamos
        public RetCpo TypeDecls(RetCpo retorno = new RetCpo())
        {
            //      <Incluso>
            //      typeDecls	: ident '=' type ';' typeDecls
		    //                  |
            //                  ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
            {
                Ident();
                Match("=");
                Type();
                Match(";");
                TypeDecls();
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo VarDecls(RetCpo retorno = new RetCpo())
        {       
                // varDecls  	: identList ':' type ';' varDecls 
                //              | 
                //              ;

            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)    // Casar apenas o tipo, podendo ser qualquer palavra.
            {
                var temp1 = IdentList();
                Match(":");
                var temp2 = Type();
                Match(";");
                var temp3 = VarDecls();

                MatchCollection matches = Regex.Matches(temp1.Lexem, @"(?<id>\w),?");
                
                try
                {
                    foreach (Match item in matches)
                    {
                        tabSimb[0].Add(item.Value, temp2.type.ToString());
                    }
                }
                catch (Exception ex)
                {
                    
                    //throw new Exception("Erro. Variável declarada mais de uma vez");
                }

                string tipo;

                switch (temp2.type)
                {
                    case Types.LIT:
                        tipo = "char";
                        break;
                    case Types.INTEGER:
                        tipo = "int";
                        break;
                    case Types.CHAR:
                        tipo = "char";
                        break;
                    case Types.BOOLEAN:
                        tipo = "bool";
                        break;
                    case Types.RECORD:
                        tipo = "struct";
                        break;
                    case Types.NOK:
                        tipo = "void";
                        break;
                    default:
                        tipo = "int";
                        break;
                }

                retorno.Lexem = tipo + esp + temp1.Lexem + temp2.Lexem + ";" + "\n" + temp3.Lexem;
            }
            else
            {
                //Vazio            
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo Type(RetCpo retorno = new RetCpo())
        {       
                // type      	: 'ARRAY' '[' intConst '..' intConst ']' OF type
		        //              | CHAR
		        //              | INTEGER
		        //              | BOOLEAN
            	//              | ident
                //              | 'RECORD' varDecls 'END'
                //              ;
            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "ARRAY")
            {
                Match("ARRAY");
                Match("[");
                var temp1 = IntConst();
                Match("..");
                var temp2 = IntConst();
                Match("]");
                Match("OF");

                var tam = Int32.Parse(temp2.Lexem) - Int32.Parse(temp1.Lexem) + 1;
                // Coloquei para que o Array pudesse ter um tipo definido na linguagem, não apenas Identificável por ela.
                // Aqui antes tinha apenas Ident(); no lugar de Type();
                var temp3 = Type();

                retorno.Lexem = "[" + tam.ToString() + "]" + temp3.Lexem;
                retorno.type = temp3.type;
            }
            else
            {
                if(lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
                {
                    retorno = Ident();
                }
                else
                {
                    if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "CHAR")
                    {
                        retorno.type = Types.CHAR;
                        Match("CHAR");                       
                    }
                    else
                    {
                        if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "INTEGER")
                        {
                            retorno.type = Types.INTEGER;
                            Match("INTEGER");
                        }
                        else 
                        {
                            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "BOOLEAN")
                            {
                                retorno.type = Types.BOOLEAN;
                                Match("BOOLEAN");
                            }
                            else 
                            {
                                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "RECORD")
                                {
                                    //TODO: ver com calma: não vai dar certo.
                                    retorno.type = Types.RECORD;
                                    Match("RECORD");
                                    retorno = VarDecls();
                                    Match("END");
                                }
                                else
                                {
                                    throw new Exception();//Erro, não temos um ID e nem uma KEY (palavra reservadas que identifiquem tipo).   
                                }    
                            }                            
                        }                    
                    }
                }   
            }

            return retorno;
        }
        
        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo SubProgDecls(RetCpo retorno = new RetCpo())
        {
                    //subProgDecls 	: subProgDecl  ';' subProgDecls 
		            //              |
                    //              ;

            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && (lexer.Lookahed.Lexem == "FUNCTION" || lexer.Lookahed.Lexem == "PROCEDURE"))
            {
                SubProgDecl();
                Match(";");
                SubProgDecls(); 
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo SubProgDecl(RetCpo retorno = new RetCpo())
        {
            //subProgDecl  	: subProgHead decls compoundStm
            //              ;

            //RetCpo retorno = new RetCpo();

            var temp1 = SubProgHead();
            var temp2 = Decl();
            var temp3 = CompoundStm();

            temp3.Lexem = temp3.Lexem + "}";

            codigo.Insert(0, temp1.Lexem);
            codigo.Insert(1, "{");
            codigo.Insert(2, temp2.Lexem);
            codigo.Insert(3, temp3.Lexem);

            return retorno;
        }

        //V3.4: OK
        //Não estamos fazendo verificação de tipo em funções
        public RetCpo SubProgHead(RetCpo retorno = new RetCpo())
        {
                //subProgHead  	: 'FUNCTION'  ident arguments ':' Type ';'
               	//              | 'PROCEDURE' ident arguments ';'
		        //              ;

            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "FUNCTION")
            {
                Match("FUNCTION");
                var temp1 = Ident(); 
                var temp2 = Arguments();
                Match(":");
                var temp3 = Type(); 
                Match(";");

                retorno.Lexem = temp3.Lexem + esp + temp1.Lexem + temp2.Lexem;
            }
            else
            {
                if(lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "PROCEDURE")
                {
                    //Match("PROCEDURE");
                    //var temp1 = Ident(); 
                    //var temp2 = Arguments();
                    //Match(";");
                }
                else
                {
                    throw new Exception();// ERRO: Não temos um cabeçalho que comece com Procedure ou Fuction
                }
            }

            return retorno;
        }

        //V3.4: OK
        //Não estamos fazendo verificação de tipo em funções
        public RetCpo Arguments(RetCpo retorno = new RetCpo())
        {
                    //arguments    	: '(' parameterList ')'
		            //              |
                    //              ;

            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == "(")
            {
                Match("(");
                retorno = ParameterList();
                Match(")");

                retorno.Lexem = "(" + retorno.Lexem + ")";
            } 
            else
            {
                //Vazio
            }

            return retorno;
        }

        //V3.4: OK
        //Não estamos fazendo verificação de tipo em funções
        public RetCpo ParameterList(RetCpo retorno = new RetCpo())
        {
            //parameterList  	: identList ':' type parameterLists
            //                  ;

            //RetCpo retorno = new RetCpo();

            var temp1 = IdentList(); 
            Match(":");
            var temp2 = Type(); 
            var temp3 = ParameterLists();

            retorno.Lexem =temp2.Lexem + esp + temp1.Lexem + temp3.Lexem;

            return retorno;
        }

        //V3.4: OK
        //Não estamos fazendo verificação de tipo em funções
        public RetCpo ParameterLists(RetCpo retorno = new RetCpo())
        {
            //parameterLists 	: ';' identList ':' type parameterLists 
            //                  |
		    //                  ;

            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == ";")
            {
                Match(";");
                var temp1 = IdentList(); 
                Match(":");
                var temp2 = Type();
                var temp3 = ParameterLists();

                retorno.Lexem = "," + temp2.Lexem + esp + temp1.Lexem + temp3.Lexem;
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo CompoundStm(RetCpo retorno = new RetCpo())
        {
            //compoundStm 	: 'BEGIN' StmList 'END'
            //              | Statement ';'
            //              ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "BEGIN")
            {
                Match("BEGIN");
                retorno = StmList(); // Aqui mudamos a produção de OptStm() para stmList()
                Match("END");
            }
            else
            {       // É o mesmo first de Statement, exceto BEGIN.
                if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && (lexer.Lookahed.Lexem == "WHILE" || lexer.Lookahed.Lexem == "IF" || lexer.Lookahed.Lexem == "READ" || lexer.Lookahed.Lexem == "WRITE")) || lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
                {
                    retorno = Statement();
                   // Match(";"); // Não temos ponto e vírgula para um statmente apenas.
                    
                }
                else
                {
                    throw new Exception();  //Erro: Não temos um comando único e nem BEGIN e END
                }
            }    

            

            //TODO: verificar onde colocar corretamente as chaves
            //retorno.Lexem = "{" + retorno.Lexem + "}";

            return retorno;
        }
        
        /*  // Não precisa dessa produção
        //AS
        public RetCpo OptStm()
        {
            //optStm      	: stmList 
		    //              |
		    //              ;
            RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.ID || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "WHILE" || lexer.Lookahed.Lexem == "BEGIN" || lexer.Lookahed.Lexem == "IF" || lexer.Lookahed.Lexem == "READ" || lexer.Lookahed.Lexem == "WRITE")))
            {
                retorno = StmList(); 
            }
            else
            {
                //Vazio
            }

            return retorno;
        }
        */

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo StmList(RetCpo retorno = new RetCpo())
        {
            
            //stmList     	: statement ';' stmLists
            //              |
            //              ;
                              
            
            //RetCpo retorno = new RetCpo();

            if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && (lexer.Lookahed.Lexem == "BEGIN" || lexer.Lookahed.Lexem == "WHILE" || lexer.Lookahed.Lexem == "IF" || lexer.Lookahed.Lexem == "READ" || lexer.Lookahed.Lexem == "WRITE")) || lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
            {
                var temp1 = Statement();
                Match(";");     // Inclui isso na gramática. O código antigo está acima.
                var temp2 = StmList();

                retorno.Lexem = (temp1.Lexem == "") ? temp2.Lexem : temp1.Lexem + ";" + temp2.Lexem;
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        /* // Não precisava desta produção
        // Modifiquei esta função, o código antigo está acima.
        //AS
        public RetCpo StmLists()
        {
            //stmLists    	: statement ';' stmLists 
		    //              | 
		    //              ;

            RetCpo retorno = new RetCpo();

            if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && (lexer.Lookahed.Lexem == "BEGIN" || lexer.Lookahed.Lexem == "WHILE" || lexer.Lookahed.Lexem == "IF" || lexer.Lookahed.Lexem == "READ" || lexer.Lookahed.Lexem == "WRITE")) || lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
            {
                var temp1 = Statement();
                Match(";");
                var temp2 = StmLists();

                retorno.Lexem = temp1.Lexem + ";" + temp2.Lexem;
            }
            else
            {
                //Vazio
            }

            return retorno;
        }
        */

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo Statement(RetCpo retorno = new RetCpo())
        {
            //statement 	: compoundStm
            //              | Ident StatementPart
            // 	            | 'IF' exp 'THEN' CopoundStm elsePart
            //	            | 'WHILE' exp 'DO' CopoundStm
		    //              | 'READ'  '(' RW_Param')'
		    //              | 'WRITE' '(' RW_Param')'
		    //              |
		    //              ;

            RetCpo param = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "BEGIN")
            {
                retorno = CompoundStm();
            }
            else
            {
                if(lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "WHILE")
                {
                    var next = NextLabel();
                    param.Next = next;

                    var begin = NextLabel();

                    Match("WHILE");

                    codigo.Add(begin + ":");
                    var temp1 = Exp();

                    Match("DO");

                    codigo.Add("if (!" + temp1.Lexem + ") goto " + next + ";");
                    // Antes tínhamos o  Statement();, decidi mudar para CompundStm(); pois ele garante a abertura e o fechamento do While com BEGIN e END.
                    CompoundStm();

                    codigo.Add("goto " + begin + ";");
                    codigo.Add(next + ":");
                }
                else
                {
                    if (lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
                    {
                        var temp1 = Ident();

                        string tipo;

                        if (!tabSimb[escopo].TryGetValue(temp1.Lexem, out tipo))
                            throw new Exception("Erro. Uso de variável não declarada");

                        var temp2 = StatementPart();

                        //Verificação de tipo
                        //Se o tipo da variável sendo atribuída tiver um tipo diferente da atribuição então erro
                        string lixo1;

                        if (tabSimb[escopo].TryGetValue(temp1.Lexem,out lixo1))
                        {
                            //if (lixo1 != temp2.type.ToString())
                            //    throw new Exception("Erro de tipo na atribuição com tipos diferentes.");
                        }

                        codigo.Add(temp1.Lexem + temp2.Lexem + ";");
                        
                        retorno.type = temp1.type;
                        retorno.Lexem = temp1.Lexem;
                    }
                    else
                    {
                        if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "IF")
                        {
                            Match("IF");

                            var next = NextLabel();
                            param.Next = next;
                            
                            //string trueLabel = NextLabel();
                            string falseLabel = NextLabel();

                            //param1.True = trueLabel;
                            param.False = falseLabel;
                            param.Next = retorno.Next;

                            var temp1 = Exp(param);

                            codigo.Add("if (" + "!" + temp1.Lexem + ") goto " + falseLabel + ";");

                            Match("THEN");
                            // Aqui havia um Statement();, decidi colocar um CopundStm(); pois aqui podemos ter uma lista de execuções com o BEGIN no começo e o END; no final de cada uma delas.
                            //codigo.Add(trueLabel + ":");
                            var temp2 = CompoundStm(param);

                            //codigo.Add("goto " + retorno.Next + ";");
                            codigo.Add("goto " + next + ";");
                            codigo.Add(falseLabel + ":");

                            var temp3 = ElsePart(param);

                            codigo.Add("goto " + next + ";");
                            codigo.Add(next + ":");
                        }
                        else
                        {
                            // ACRESCENTEI ESTA PRODUÇÃO PARA O RECONHECIMENTO DO READ.
                            // Statement --> 'READ' '(' XXXX ')'
                            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "READ")
                            {
                                Match("READ");
                                Match("(");
                                var temp1 = Ident();
                                var temp2 = Read_Param();
                                Match(")");

                                string scanfParam = "";

                                switch (temp1.type)
                                {
                                    case Types.LIT:
                                        scanfParam = "%c";
                                        break;

                                    case Types.INTEGER:
                                        scanfParam = "%i";
                                        break;

                                    case Types.CHAR:
                                        scanfParam = "%c";
                                        break;

                                    case Types.BOOLEAN:
                                        scanfParam = "%i";
                                        break;

                                    case Types.RECORD:
                                        break;

                                    case Types.NOK:
                                        break;
                                    default:
                                        break;
                                }

                                codigo.Add("scanf(\"" + scanfParam + "\", &" + temp1.Lexem + ");");
                            }
                            else
                            {       // ACRESCENTEI ESTA PRODUÇÃO PARA O RECONHECIMENTO DO WRITE.
                                // Statement --> 'WRITE' '(' XXXXX ')'
                                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "WRITE")
                                {
                                    Match("WRITE");
                                    Match("(");
                                    var temp1 = Write_Param();
                                    Match(")");

                                    string printfParam = "";

                                    string tipo;
                                    tabSimb[escopo].TryGetValue(temp1.Lexem, out tipo);

                                    switch (tipo)
                                    {
                                        case "LIT":
                                            printfParam = "%c";
                                            break;

                                        case "INTEGER":
                                            printfParam = "%i";
                                            break;

                                        case "CHAR":
                                            printfParam = "%c";
                                            break;

                                        case "BOOLEAN":
                                            printfParam = "%i";
                                            break;

                                        case "RECORD":
                                            break;

                                        case "NOK":
                                            break;
                                        default:
                                            break;
                                    }

                                    codigo.Add("printf(\"" + printfParam + "\", " + temp1.Lexem + ");");
                                }
                                else
                                {
                                    //Vazio                                    
                                }
                            }
                        }
                    } 
                }
            }

            //codigo.Add(next + ":");
            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo Read_Param(RetCpo retorno = new RetCpo())
        { 
                    // <incluso>
                    // READ_Param	: '[' Exp ']'
		            //              | '.' Ident
		            //              |
		            //              ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == "[" )
            {
                Match("[");
                var temp1 = Exp();

                //TODO: verificar se não precisa testar de booleano

                Match("]");
                var temp2 = WR_Param_Part_();

                retorno.Lexem = "[" + temp1.Lexem + "]" + temp2.Lexem;
            }
            else
            {
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == ".")
                {
                    Match(".");
                    //retorno.Lexem = "." + Ident().Lexem;
                    retorno.Lexem = Ident().Lexem;
                }
                else
                {
                    //Vazio
                }
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo Write_Param(RetCpo retorno = new RetCpo())
        {
            // <incluso>
            // READ_Param	: Ident WRITE_Param_Part
            //              | Lit
            //              ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
            {
                var temp1 = Ident();
                var temp2 = Write_Param_Part();

                retorno.Lexem = temp1.Lexem + temp2.Lexem;
            }
            else
            {
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.LIT)
                {
                    retorno = Lit();
                }
                else
                {
                    throw new Exception(); //Vazio
                }
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo Write_Param_Part(RetCpo retorno = new RetCpo())
        {
            //<incluso>
            //   WRITE_Param_Part   : '[' Exp ']' Write_Parm_Part_
		    //                      | '.' Ident
		    //                      | '(' IdentList ')'
		    //                      |
		    //                      ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == "[")
            {
                Match("[");
                var temp1 = Exp();
                Match("]");
                var temp2 = WR_Param_Part_();

                retorno.Lexem = "[" + temp1.Lexem + "]" + temp2.Lexem;
            }
            else
            {
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == ".")
                {
                    Match(".");
                    retorno = Ident();

                    //retorno.Lexem = "." + retorno.Lexem;
                    retorno.Lexem = retorno.Lexem;
                }
                else
                {
                    if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == "(")
                    {
                        Match("(");
                        retorno = IdentList();
                        Match(")");

                        retorno.Lexem = "(" + retorno.Lexem + ")";
                    }
                    else
                    {
                        //Vazio
                    }
                }
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo WR_Param_Part_(RetCpo retorno = new RetCpo()) 
        { 
            //  <Incluso>
            //      WRITE_Param_Part_   : '.' Ident
		    //                          |
		    //                          ;
            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == ".")
            {
                Match(".");
                retorno = Ident();

                //retorno.Lexem = "." + retorno.Lexem;
                retorno.Lexem = retorno.Lexem;
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo StatementPart(RetCpo retorno = new RetCpo())
        {
            //StatementPart	    : Parametros
		    //  <modificado>    | ArrayIndex ':=' StatementPart_
            //  <modificado>    | ':=' StatementPart_
		    //                  |
		    //                  ;
            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == "(")
            {
                retorno = Parametros();
            }
            else
            {
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == "[")
                {
                    var temp1 = ArrayIndex();
                    Match(":=");
                    var temp2 = StatementPart_();

                    if (temp2.Lexem.Contains("TRUE"))
                        temp2.Lexem = temp2.Lexem.Replace("TRUE", "true");
                    else if (temp2.Lexem.Contains("FALSE"))
                        temp2.Lexem = temp2.Lexem.Replace("FALSE", "false");

                    retorno.type = temp2.type;
                    retorno.Lexem = temp1.Lexem + " = " + temp2.Lexem;
                }
                else
                {
                    if (lexer.Lookahed.TypeOf == TP2_Parser.Type.RELOP && lexer.Lookahed.Lexem == ":=")
                    {
                        Match(":=");
                        var temp1 = StatementPart_();

                        if (temp1.Lexem.Contains("TRUE"))
                            temp1.Lexem = temp1.Lexem.Replace("TRUE", "true");
                        else if (temp1.Lexem.Contains("FALSE"))
                            temp1.Lexem = temp1.Lexem.Replace("FALSE", "false");

                        retorno.type = temp1.type;
                        retorno.Lexem = " = " + temp1.Lexem;
                    }
                    else
                    {
                        // Vazio
                    }
                }
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo StatementPart_(RetCpo retorno = new RetCpo())
        {   //<Incluso>
            //StatementPart_	: Exp
            //                  | Lit
            //                  ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.NUM || lexer.Lookahed.TypeOf == TP2_Parser.Type.ID || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "NOT")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-" || lexer.Lookahed.Lexem == "(")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-")))
            {
                retorno = Exp();

                if (retorno.Lexem.Contains("TRUE"))
                    retorno.Lexem = retorno.Lexem.Replace("TRUE", "true");
                else if (retorno.Lexem.Contains("FALSE"))
                    retorno.Lexem = retorno.Lexem.Replace("FALSE", "false");
            }
            else
            {
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.LIT)
                {
                    retorno = Lit();

                    if (retorno.Lexem.Contains("TRUE"))
                        retorno.Lexem = retorno.Lexem.Replace("TRUE", "true");
                    else if (retorno.Lexem.Contains("FALSE"))
                        retorno.Lexem = retorno.Lexem.Replace("FALSE", "false");
                }
                else
                {
                    throw new Exception();//Erro: Não temos atribuição
                }
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo ElsePart(RetCpo retorno = new RetCpo())
        {
            //ElsePart 	: 'ELSE' CopundStm
		    //          |
		    //          ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY && lexer.Lookahed.Lexem == "ELSE")
            {
                Match("ELSE");
                // Havia  Statement(); aqui. Porém é mais apropriado um CompoundStm(); Para que reconheçamos o BEGIN e o END do IF THEN ELSE

                CompoundStm();
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //Não usamos
        public RetCpo LitList(RetCpo retorno = new RetCpo())
        {      
            //LitList		: Lit   LitLists
		    //              | Ident LitLisPart
		    //              | Exp   LitLists
		    //              ;

            //RetCpo retorno = new RetCpo();

            // Podemos ter, dentro do parâmetro do WRITE, um Literal.
            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.LIT)
            {
                Lit();
                LitLists();
            }
            else
            {            // Podemos ter, dentro do parâmetro do WRITE, uma Variável, uma Função ou Procedimento.
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
                {
                    Ident();
                    LitListPart();
                }
                else
                {           // Podemos ter, dentro do parâmetro do WRITE, uma expressão
                    if (lexer.Lookahed.TypeOf == TP2_Parser.Type.NUM || lexer.Lookahed.TypeOf == TP2_Parser.Type.ID || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "NOT")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-" || lexer.Lookahed.Lexem == "(")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-")))
                    {
                        Exp();
                        LitLists();
                    }
                    else
                    {
                        throw new Exception();  //Erro: Não temos literal, nem Exp e nem ID.
                    }  
                }  
            }

            return retorno;
        }

        //Não usamos
        public RetCpo LitLists(RetCpo retorno = new RetCpo())
        {          
            //LitLists	: ',' LitListsPart_
		    //          | 
		    //          ;
            
            // Caso tenhamos a ','
            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == ",")
            {
                Match(",");
                // Retirado Lit(); e LitLists(); para colocar o nome LitListsPart(); para fatorar o que tínhamos anterirormente.
                LitListsPart_();
            }
            else
            {
                // Vazio
            }

            return retorno;
        }

        //Não usamos
        public RetCpo LitListPart(RetCpo retorno = new RetCpo())
        {       
                //LitListPart	: LitLists
		        //              | '(' Exp ')' LitLists
		        //              ;
            
                //Caso seja ',', terminal.
            //RetCpo retorno = new RetCpo();

            if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM)&&(lexer.Lookahed.Lexem == ","))
            {
                LitLists();
            }
            else
            {           //Caso seja '('
                if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "("))
                {
                    Match("(");
                    Exp();
                    Match(")");
                    LitLists();
                }
                else
                {
                    throw new Exception();  //Erro: 
                }  
            }

            return retorno;
        }

        //Não usamos
        public RetCpo LitListsPart_(RetCpo retorno = new RetCpo())
        {
                //LitListsPart_	: Lit LitLists
                //              | Exp LitLists
                //              | Ident LitListsPart__
                //              ;
            
                // CASO SEJA LIT
            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.LIT)
            {
                Lit();
                LitLists();
            }
            else
            {           // CASO SEJA EXP
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.NUM || lexer.Lookahed.TypeOf == TP2_Parser.Type.ID || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "NOT")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-" || lexer.Lookahed.Lexem == "(")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-")))
                {
                    Exp();
                    LitLists();
                }
                else
                {           // CASO SEJA ID
                    if (lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
                    {
                        Ident();
                        LitListsPart__();
                    }
                    else
                    {
                        throw new Exception(); // Erro:
                    }  
                }  
            }

            return retorno;
        }

        //Não usamos
        public RetCpo LitListsPart__(RetCpo retorno = new RetCpo())
        {       
                //LitListsPart__	: LitLists
		        //                  | Arguments LitLists
		        //                  ;

                // Caso tenhamos a ',' : First de LitLists

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == ",")
            {   
                LitLists();
            }
            else
            {         // Caso tenhamos a '(' : First de Arguments
                if (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == "(")
                {
                    Arguments();
                    LitLists();
                }
                else
                {
                    throw new Exception(); // Erro:
                }
            }

            return retorno;
        }

        //Não usamos
        public RetCpo Variable(RetCpo retorno = new RetCpo())
        {
                //variable 	: ident arrayIndex
                //          ;

            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)
            {
                var temp1 = Ident();
                var temp2 = ArrayIndex();

                retorno.Lexem = temp1.Lexem + temp2.Lexem;
            }
            else
            {
                throw new Exception();// Não é um ID válido
            }

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo ArrayIndex(RetCpo retorno = new RetCpo())
        {
                //arrayIndex 	: '[' exp ']'
		        //              ;
            //RetCpo retorno = new RetCpo();

            Match("[");  
            retorno = Exp();  
            Match("]");

            retorno.Lexem = "[" + retorno.Lexem + "]";

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo Parametros(RetCpo retorno = new RetCpo())
        {
                //parametros     	: '(' expList ')'
                //                  ;
            //RetCpo retorno = new RetCpo();
            Match("(");
            retorno = ExpList();
            Match(")");

            retorno.Lexem = "(" + retorno.Lexem + ")";

            return retorno;
        }
        
        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo ExpList(RetCpo retorno = new RetCpo())
        {
            //RetCpo retorno = new RetCpo();

            var temp1 = Exp();
            var temp2 = ExpLists();

            retorno.Lexem = temp1.Lexem + temp2.Lexem;

            return retorno;
        }

        //V3.4: OK
        //Verificação de tipo: OK
        public RetCpo ExpLists(RetCpo retorno = new RetCpo())
        {
                //expLists   	: ',' Exp ExpLists 
		        //              | 
		        //              ;

            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM && lexer.Lookahed.Lexem == ",")
            {
                Match(",");
                var temp1 = Exp();
                var temp2 = ExpLists();

                //TODO: Verificar a posição da vírgula
                retorno.Lexem = "," + temp1.Lexem + temp2.Lexem;
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo Exp(RetCpo retorno = new RetCpo())
        {
                //Exp        	: SimpleExp RelExp
                //              ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.NUM || lexer.Lookahed.TypeOf == TP2_Parser.Type.ID || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "NOT")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-" || lexer.Lookahed.Lexem == "(")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-")))
            {
                var temp1 = SimpleExp();
                var temp2 = RelExp();

                if (temp1.Lexem.Contains("TRUE"))
                    temp1.Lexem = temp1.Lexem.Replace("TRUE", "true");
                else if (temp1.Lexem.Contains("FALSE"))
                    temp1.Lexem = temp1.Lexem.Replace("FALSE", "false");

                if (temp2.Lexem != null)
                {
                    if (temp2.Lexem.Contains("TRUE"))
                        temp2.Lexem = temp2.Lexem.Replace("TRUE", "true");
                    else if (temp2.Lexem.Contains("FALSE"))
                        temp2.Lexem = temp2.Lexem.Replace("FALSE", "false");
                }

                var t1 = NextTemp();

                tabSimb[0].Add(t1, Types.INTEGER.ToString());

                codigo.Add("int" + esp + t1 + " = " + temp1.Lexem + temp2.Lexem + ";");
                retorno.Lexem = t1;
            }
            else
            {
                throw new Exception(); //Erro, não é + - ( NOT ID INTEGER 
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo RelExp(RetCpo retorno = new RetCpo())
        {
                //relExp     	: RelOp SimpleExp 
		        //              |
		        //              ;

            //RetCpo retorno = new RetCpo();

            if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.RELOP)&& (lexer.Lookahed.Lexem == "<" || lexer.Lookahed.Lexem == "<=" || lexer.Lookahed.Lexem == ">" || lexer.Lookahed.Lexem == ">=" || lexer.Lookahed.Lexem == "=" || lexer.Lookahed.Lexem == "<>" ))
            {
                var temp1 = RelOp();
                var temp2 = SimpleExp();

                retorno.Lexem = temp1.Lexem + temp2.Lexem;
            }
            else
            {
                // Vazio
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo SimpleExp(RetCpo retorno = new RetCpo())
        {
                //simpleExp  	: Sign Term AddExp
                //              ;

            //RetCpo retorno = new RetCpo();

            if (((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "(")) || (lexer.Lookahed.TypeOf == TP2_Parser.Type.NUM) || (lexer.Lookahed.TypeOf == TP2_Parser.Type.ID) || (lexer.Lookahed.TypeOf == TP2_Parser.Type.LIT) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "NOT")))
            {
                var temp1 = Sign();
                var temp2 = Term();
                var temp3 = AddExp();

                var t1 = NextTemp();
                tabSimb[0].Add(t1, Types.INTEGER.ToString());
                var t2 = NextTemp();
                tabSimb[0].Add(t2, Types.INTEGER.ToString());

                codigo.Add("int" + esp + t1 + " = " + temp1.Lexem + temp2.Lexem + ";");
                codigo.Add("int" + esp + t2 + " = " + t1 + temp3.Lexem + ";");

                retorno.Lexem = t2;
            }
            else
            {
                throw new Exception();// Erro, não encontrado + - ( INTEGER ID NOT
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo AddExp(RetCpo retorno = new RetCpo())
        {
                //AddExp     	: AddOp Term AddExp 
		        //              |
		        //              ;

            //RetCpo retorno = new RetCpo();

		   if (((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "+" || lexer.Lookahed.Lexem == "-")) || ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "OR")))
            {
               var temp1 = AddOp();  
               var temp2 = Term();  
               var temp3 = AddExp();

               var t = NextTemp();
               tabSimb[0].Add(t, Types.INTEGER.ToString());

               codigo.Add("int" + esp + t + " = " + temp2.Lexem + temp3.Lexem + ";");
               retorno.Lexem = temp1.Lexem + t;
            }
            else
            {
               //Vazio
            }

           return retorno;
        }

        //V3.4: OK
        public RetCpo Term(RetCpo retorno = new RetCpo())
        {
                //term       	: Factor MulExp
                //              ;

            //RetCpo retorno = new RetCpo();

            if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "NOT") || (lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "(") || lexer.Lookahed.TypeOf == TP2_Parser.Type.ID || lexer.Lookahed.TypeOf == TP2_Parser.Type.LIT || lexer.Lookahed.TypeOf == TP2_Parser.Type.NUM)
            {
                var temp1 = Factor();  
                var temp2 = MulExp();

                retorno.Lexem = temp1.Lexem + temp2.Lexem;
            }
            else
            {
                throw new Exception();//Erro, não foi encontrado ID INTEGER NOT (
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo MulExp(RetCpo retorno = new RetCpo())
        {
                //mulExp     	: mulOp factor mulExp 
		        //              |
		        //              ;

            //RetCpo retorno = new RetCpo();

            if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "DIV" || lexer.Lookahed.Lexem == "MOD" || lexer.Lookahed.Lexem == "AND") || (lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && ( lexer.Lookahed.Lexem == "/" || lexer.Lookahed.Lexem == "*"))
            {
                var temp1 = MulOp();  
                var temp2 = Factor();  
                var temp3 = MulExp();

                var t = NextTemp();
                tabSimb[0].Add(t, Types.INTEGER.ToString());

                codigo.Add("int" + esp + t + " = " + temp2.Lexem + temp3.Lexem + ";");
                retorno.Lexem = temp1.Lexem + t;
            }
            else
            {
                //Vazio
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo Factor(RetCpo retorno = new RetCpo())
        {
                //factor     	: intConst
                // <modificado> | Lit
             	//              | ident factorPart
             	//              | '(' exp ')' 
             	//              | 'NOT' factor
		        //              ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.NUM)
            {
                retorno = IntConst();
            }
            else
            {
                if((lexer.Lookahed.TypeOf == TP2_Parser.Type.ID))
                {
                    var temp = Ident(); 
                    var temp2 = FactorPart();

                    retorno.Lexem = temp.Lexem + temp2.Lexem; //AS
                }
                else
                {
                    if((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "("))
                    {
                        Match("(");
                        retorno.Lexem = Exp().Lexem;    //AS
                        Match(")");
                    }
                    else
                    {
                        if((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "NOT"))
                        {
                            Match("NOT");
                            var temp1 = Factor();

                            var t = NextTemp();
                            tabSimb[0].Add(t, Types.INTEGER.ToString());

                            codigo.Add("int" + esp + t + "= !" + temp1.Lexem + ";");    //AS
                            retorno.Lexem = t;
                        }
                        else
                        {
                            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.LIT)
                            {
                                retorno = Lit();
                            }
                            else
                            {
                                throw new Exception();// Erro, não é INTEGER ID ( NOT
                            }
                        }
                    }
                }
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo FactorPart(RetCpo retorno = new RetCpo())
        {
                //factorPart      : parametros
                //                | arrayIndex WR_Param_Part_
                //                | WR_Param_Part_	
                //                |
		        //                ;

            //RetCpo retorno = new RetCpo();

            if((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "("))
            {
                 retorno = Parametros();
            }
            else
            {
                if((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "["))
                { 
                    var temp = ArrayIndex();
                    var temp2 = WR_Param_Part_();

                    //retorno.Lexem = temp.Lexem + "." + temp2.Lexem;
                    retorno.Lexem = temp.Lexem + temp2.Lexem;
                }
                else
                {
                    if ((lexer.Lookahed.TypeOf == TP2_Parser.Type.TERM) && (lexer.Lookahed.Lexem == "."))
                    {
                        retorno = WR_Param_Part_();
                    }
                    else
                    {
                        // Vazio
                    }
                }
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo Sign(RetCpo retorno = new RetCpo())
        {
                //sign  		: '+' 
		        //              | '-' 
		        //              |
		        //              ;

            //RetCpo retorno = new RetCpo();

            if((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "+"))
            {
                retorno.Lexem = "+";
                retorno.type = Types.INTEGER;
                Match("+");
            }
            else
            {
                if((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "-"))
                {
                    retorno.Lexem = "-";
                    retorno.type = Types.INTEGER;
                    Match("-");
                }
                else
                {
                    // Vazio
                }
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo RelOp(RetCpo retorno = new RetCpo())
        {
                //relOp 		: '<' 
		        //              | '<=' 
		        //              | '>' 
		        //              | '>=' 
		        //              | '=' 
		        //              | '<>'
		        //              ;

            //RetCpo retorno = new RetCpo();

            if((lexer.Lookahed.TypeOf == TP2_Parser.Type.RELOP) && (lexer.Lookahed.Lexem == "<"))
            {
                retorno.Lexem = "<";
                retorno.type = Types.BOOLEAN;
                Match("<");
            }
            else
            {
                if((lexer.Lookahed.TypeOf == TP2_Parser.Type.RELOP) && (lexer.Lookahed.Lexem == "<="))
                {
                    retorno.Lexem = "<=";
                    retorno.type = Types.BOOLEAN;
                    Match("<=");
                }
                else
                {
                    if((lexer.Lookahed.TypeOf == TP2_Parser.Type.RELOP) && (lexer.Lookahed.Lexem == ">"))
                    {
                        retorno.Lexem = ">";
                        retorno.type = Types.BOOLEAN;
                        Match(">");
                    }
                    else
                    {
                        if((lexer.Lookahed.TypeOf == TP2_Parser.Type.RELOP) && (lexer.Lookahed.Lexem == ">="))
                        {
                            retorno.Lexem = ">=";
                            retorno.type = Types.BOOLEAN;
                            Match(">=");
                        }
                        else
                        {
                            if((lexer.Lookahed.TypeOf == TP2_Parser.Type.RELOP) && (lexer.Lookahed.Lexem == "="))
                            {
                                retorno.Lexem = "==";
                                retorno.type = Types.BOOLEAN;
                                Match("=");
                            }
                            else
                            {
                                if((lexer.Lookahed.TypeOf == TP2_Parser.Type.RELOP) && (lexer.Lookahed.Lexem == "<>"))
                                {
                                    retorno.Lexem = "!=";
                                    retorno.type = Types.BOOLEAN;
                                    Match("<>");
                                }
                                else
                                {
                                    throw new Exception();// Erro, não é um operador relacional < <= > >= = <>
                                }
                            }
                        }                 
                    }
                }
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo AddOp(RetCpo retorno = new RetCpo())
        {
                //addOp 		: '+' 
		        //              | '-' 
		        //              | OR
		        //              ;
            //RetCpo retorno = new RetCpo();

            if((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "+"))
            {
                retorno.Lexem = "+";
                retorno.type = Types.INTEGER;
                Match("+");
            }
            else
            {
                if((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "-"))
                {
                    retorno.Lexem = "-";
                    retorno.type = Types.INTEGER;
                    Match("-");
                }
                else
                {
                    if((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "OR"))
                    {
                        retorno.Lexem = "||";
                        retorno.type = Types.BOOLEAN;
                        Match("OR");
                    }
                    else
                    {
                        throw new Exception();// Erro, não é operador de adição + - OR
                    }
                                  
                }
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo MulOp(RetCpo retorno = new RetCpo())
        {
            //TODO: retirar o DIV
                //mulOp 		: '*' 
		        //              | '/' 
		        //              | 'DIV' 
		        //              | 'MOD' 
		        //              | 'AND'
		        //              ;

            //RetCpo retorno = new RetCpo();

            if((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "*"))
            {
                retorno.Lexem = "*";
                retorno.type = Types.INTEGER;
                Match("*");
            }
            else
            {
                if((lexer.Lookahed.TypeOf == TP2_Parser.Type.OP) && (lexer.Lookahed.Lexem == "/"))
                {
                    retorno.Lexem = "/";
                    retorno.type = Types.INTEGER;
                    Match("/");
                }
                else
                {
                    if((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "DIV"))
                    {
                        retorno.Lexem = "%";
                        Match("DIV");
                    }
                    else
                    {
                        if((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "MOD"))
                        {
                            retorno.Lexem = "%";
                            retorno.type = Types.INTEGER;
                            Match("MOD");
                        }
                        else
                        {
                            if((lexer.Lookahed.TypeOf == TP2_Parser.Type.KEY) && (lexer.Lookahed.Lexem == "AND"))
                            {
                                retorno.Lexem = "&&";
                                retorno.type = Types.BOOLEAN;
                                Match("AND");
                            }
                            else
                            {
                                throw new Exception();// Erro, não é um operador de multiplicação válido * / DIV MOD AND
                            }
                        }                 
                    }
                }
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo Ident(RetCpo retorno = new RetCpo())
        {
                //ident 		: ('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_')*
                //              ;

                // Pode ser ID
            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.ID)    //Aqui estava errado. Tinha KEY's como identificadores
            {
                //AS
                retorno.Lexem = lexer.Lookahed.Lexem;

                Match(lexer.Lookahed.Lexem);
                // Ok, é um tipo identificador válido.
            }
            else
            {
                throw new Exception();// Erro, não é um identificador válido.
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo Lit(RetCpo retorno = new RetCpo())
        {
                //Lit       : ''' ('a'..'z'|'A'..'Z'|'_')* '''
                //          ;

            //RetCpo retorno = new RetCpo();

            if (lexer.Lookahed.TypeOf == TP2_Parser.Type.LIT)
            {
                //AS
                retorno.Lexem = lexer.Lookahed.Lexem;
                retorno.type = Types.LIT;

                Match(lexer.Lookahed.Lexem);
                // Ok, é um tipo de literal válido.
            }
            else
            {
                throw new Exception();// Erro, não é um identificador válido.
            }

            return retorno;
        }

        //V3.4: OK
        public RetCpo IntConst(RetCpo retorno = new RetCpo())
        {
                //intConst  	: ('0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9') ('0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9')*
		        //              ;

            //RetCpo retorno = new RetCpo();

            if(lexer.Lookahed.TypeOf == TP2_Parser.Type.NUM)
            {
                retorno.Lexem = lexer.Lookahed.Lexem;
                retorno.type = Types.INTEGER;
                Match(lexer.Lookahed.Lexem);// Ok, é um tipo número.
            }
            else
            {
                throw new Exception();// Erro, não é uma constante válida.
            }

            return retorno;
        }

        //V3.4: OK
        public void Match(string terminal)
        {
            if (lexer.Lookahed.Lexem == terminal)
            {

                lexer.NextToken();
            }
            else
            {
                throw new Exception();//  Erro, terminal não casado.
            }
        }
    }
}


