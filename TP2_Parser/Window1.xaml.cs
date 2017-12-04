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
            //programa = programa.Replace("\r\n", " ");

            Antlr.Runtime.ANTLRStringStream ss = new Antlr.Runtime.ANTLRStringStream(programa);

            minipascalLexer lexer = new minipascalLexer(ss);
            Antlr.Runtime.CommonTokenStream tokens = new Antlr.Runtime.CommonTokenStream(lexer);

            minipascalParser parser = new minipascalParser(tokens);

            try
            {
                parser.start();
                MessageBox.Show("Resultado OK!");
            }
            catch (Antlr.Runtime.RecognitionException ex)
            {
                MessageBox.Show(("Erro no código!\n" + ex.Message),"Erro de Sintaxe", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
