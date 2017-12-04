using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TP2_Parser
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void btnParser_Click(object sender, RoutedEventArgs e)
        {
            string programa = txbCod.Text;
            programa = programa.ToUpper();

            Lexer lexer = new Lexer(programa);
            Parser parser = new Parser(lexer);

            if (lexer.NextToken())
                if (parser.start())
                    MessageBox.Show("Programa OK");
                else
                {
                    string mensagem = "Problema no programa.Linha " + lexer.NrLinha + "\nValor encontrado: " + lexer.Lookahed.ToString();
                    MessageBox.Show(mensagem, "Resultado", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            else
                MessageBox.Show("Problema no lexer");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string programa = txbCod.Text;
            programa = programa.ToUpper();

            Lexer lexer = new Lexer(programa);
            bool result;
            do
            {
                result = lexer.NextToken();
            } while (result);
        }
    }
}
