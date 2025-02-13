using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt_02._25
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string attributeRegex = "^[0-9]*$";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void attribute_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true; // Blokuje spację

            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (!Regex.IsMatch(Clipboard.GetText(), attributeRegex))
                    e.Handled = true; // Blokuje wklejanie znaków, które nie są cyframi
            }
        }

        private void attribute_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, attributeRegex)){
                e.Handled = true; //Uniemożliwiamy wpisanie danego znaku, który nie jest cyfrą
            }
        }

        private void Encode(object sender, RoutedEventArgs e)
        {
            int cipher = -1; //0 - cezara 1 - vigenera(czy jak mu tam) 2 - affine 3 - płotkowy
            if (caesarCipher.IsSelected)
            {
                cipher = 0;
            }else if (vigenereCipher.IsSelected)
            {
                cipher = 1;
            }else if (affineCipher.IsSelected)
            {
                cipher = 2;
            }else if (fenceCipher.IsSelected)
            {
                cipher = 3;
            }

            string caesarEncode(string input, int shift)
            {
                return input; //TODO
            }
            string vigenereEncode(string input, string key)
            {
                return input; //TODO
            }
            string affineEncode(string input, int a, int b)
            {
                return input; //TODO
            }
            string fenceEncode(string input, int shift) // ??SHIFT/KEY ?? ****
            {
                return input; //TODOOOOO
            }



        }
        private void Decode(object sender, RoutedEventArgs e)
        {
            int cipher = -1; //0 - cezara 1 - vigenera(czy jak mu tam) 2 - affine 3 - płotkowy
            if (caesarCipher.IsSelected)
            {
                cipher = 0;
            }
            else if (vigenereCipher.IsSelected)
            {
                cipher = 1;
            }
            else if (affineCipher.IsSelected)
            {
                cipher = 2;
            }
            else if (fenceCipher.IsSelected)
            {
                cipher = 3;
            }

            string caesarDecode(string input, int shift)
            {
                return input; //TODO
            }
            string vigenereDecode(string input, string key)
            {
                return input; //TODO
            }
            string affineDecode(string input, int a, int b)
            {
                return input; //TODO
            }
            string fenceDecode(string input, int shift) // ??SHIFT/KEY ?? ****
            {
                return input; //TODOOOOO
            }
        }
    }
}
