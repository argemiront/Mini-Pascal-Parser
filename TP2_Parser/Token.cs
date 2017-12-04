using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TP2_Parser
{
    public enum Type
    {
        KEY,        // Palavras chaves reservadas da linguagem: PROGRAM, OF, BEGIN, 
                    //END, VAR, ARRAY, CHAR, BOOLEN, INTEGER, FUNCTION, PROCEDURE, 
                    //IF, ELSE, WHILE, DO, THEN, NOT, OR, AND, MOD, DIV.
        ID,         // Identificador válido da linguagem: (('a'|'z')|('A'|'Z')|('_')) (('a'|'z')|('A'|'Z')|('_'))*.
        RELOP,      // Operadores relacionais >, <, <=, >=, =, <>
        TERM,       // Outros terminais não ligados a uma produção específica: '.', '[', ']', '(', ')', ',',
                    // ':=',  ';',  '..', ':' 
        OP,         // '+','-','/','*'
        NUM,        // ('0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9') ('0'|'1'|'2'|'3'|'4'|'5'|'6'|'7'|'8'|'9')*
        LIT,        //Literal = qualquer string de texto sem '
        NOK         //Token não válido
    }

    class Token
    {
        public Type TypeOf { get; set; }
        public string Lexem { get; set; }

        public Token() { }

        public Token(Type tipo, string lexema)
        {
            TypeOf = tipo;
            Lexem = lexema;
        }

        public static Token ValidaToken(string entrada)
        {
            if (Regex.IsMatch(entrada, @"" + Resource1.STR_KEY))
                return new Token(Type.KEY, entrada);

            else if (Regex.IsMatch(entrada, @"" + Resource1.STR_RELOP))
                return new Token(Type.RELOP, entrada);

            else if (Regex.IsMatch(entrada, @"" + Resource1.STR_TERM))
                return new Token(Type.TERM, entrada);

            else if (Regex.IsMatch(entrada, @"" + Resource1.STR_OP))
                return new Token(Type.OP, entrada);

            else if (Regex.IsMatch(entrada, @"" + Resource1.STR_ID))
                return new Token(Type.ID, entrada);

            else if (Regex.IsMatch(entrada, @"" + Resource1.STR_NUM))
                return new Token(Type.NUM, entrada);

            else if (Regex.IsMatch(entrada, @"" + Resource1.STR_LIT))
                return new Token(Type.LIT, entrada);

            else
                return new Token(Type.NOK, entrada);
        }
    }
}
