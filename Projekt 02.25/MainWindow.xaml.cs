using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        string filePath = string.Empty;
        string attributeRegex = "^[0-9]*$";

        public MainWindow()
        {
            InitializeComponent();
        }

        public string caesarEncode(string input, int shift)
        {
            string output = "";

            foreach (char c in input)
            {
                if (char.IsLetter(c)) // Sprawdzam, czy znak to litera
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a'; // Określam bazę dla przesunięcia(początek w tablicy ASCII dla szyfrowania)
                    char shiftedChar = (char)(baseChar + (c - baseChar + shift) % 26);
                    output += shiftedChar;
                }
                else
                {
                    output += c; // Nie zmieniam znaków niebędących literami
                }
            }


            return output;
        }

        public string caesarDecode(string input, int shift)
        {
            string output = "";

            foreach (char c in input)
            {
                if (char.IsLetter(c)) // Sprawdzam, czy znak to litera
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a'; // Określam bazę dla przesunięcia(początek w tablicy ASCII dla szyfrowania)
                    char shiftedChar = (char)(baseChar + (c - baseChar - shift % 26 + 26)%26); //Chore
                    output += shiftedChar;
                }
                else
                {
                    output += c; // Nie zmieniam znaków niebędących literami
                }
            }


            return output;
        }

        public string vigenereEncode(string input, string key)
        {
            string output = "";
            int keyIndex = 0;
            int keyLength = key.Length;

            foreach (char c in input)
            {
                if (char.IsLetter(c)) // Sprawdzam, czy znak to litera
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a'; // Ustalam bazę (A dla wielkich liter, a dla małych)
                    int shift = char.ToUpper(key[keyIndex % keyLength]) - 'A'; // Obliczam przesunięcie na podstawie klucza
                    char encodedChar = (char)(baseChar + (c - baseChar + shift) % 26); // Wykonuję przesunięcie
                    output += encodedChar;
                    keyIndex++; // Przechodzę do kolejnego znaku klucza
                }
                else
                {
                    output += c; // Nie zmieniam znaków, które nie są literami
                }
            }

            return output;
        }

        public string vigenereDecode(string input, string key)
        {
            string output = "";
            int keyIndex = 0;
            int keyLength = key.Length;

            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    int shift = char.ToUpper(key[keyIndex % keyLength]) - 'A';
                    char decodedChar = (char)(baseChar + (c - baseChar - shift + 26) % 26); // Odejmujemy zamiast dodawać
                    output += decodedChar;
                    keyIndex++;
                }
                else
                {
                    output += c;
                }
            }

            return output;
        }

        public string affineEncode(string input, int a, int b)
        {
            string output = "";

            foreach(char c in input)
            {
                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    int x = c - baseChar; // Zamieniam znak na zakres 0-25 (żeby było tak jak w tuto)
                    int encodedX = (a*x + b) % 26;
                    char encodedChar = (char)(baseChar + encodedX); // Wydaje się, że nie potrzebne i można by to w linijce z outputem zrobić, ale nie - za nic nie działa, więc zostawiam tak
                    output += encodedChar; // Dodanie zaszyfrowanego znaku przekonwertowanego z powrotem na ASCII
                }
                else
                {
                    output += c;
                }
            }

            return output;
        }

        public string affineDecode(string input, int a, int b)
        {
            //D(x)= a^−1*(x−b) mod 26 (wzór do deszyfrowania z tuto)
            string output = "";
            int aInverse = -1;

            // Znajduję odwrotność modularną a mod 26 
            for (int i = 1; i < 26; i++)  
            {
                if ((a * i) % 26 == 1)
                {
                    aInverse = i;
                    break;
                }
            }

            // Jeśli nie znaleziono odwrotności, a jest niepoprawne
            if (aInverse == -1)
                return "Nie udało się odszyfrować wiadomości";

            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    int x = c - baseChar;

                    // Upewniamy się, że wynik nie jest ujemny
                    int decodedChar = (aInverse * ((x - b + 26) % 26)) % 26;
                    output += (char)(baseChar + decodedChar);
                }
                else
                {
                    output += c;
                }
            }

            return output;
        }


        public string fenceEncode(string input, int fenceHeight) //Doesn't handle spaces
        {
            string output = "";

            List<char>[] array = new List<char>[fenceHeight];
            for (int i = 0; i < fenceHeight; i++)
            {
                array[i] = new List<char>();
            }

            bool incrOrDecr = false; //increment or decrement arrayRow [false - increment, true - decrement]
            int arrayRow = 0;

            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    //Appending to array
                    array[arrayRow].Add(c);
                    //if (arrayRow == fenceHeight - 1 || arrayRow == 0) incrOrDecr = !incrOrDecr;
                    //arrayRow = incrOrDecr ? (arrayRow - 1) : (arrayRow + 1);
                    if (arrayRow == fenceHeight - 1) //Change of direction
                        incrOrDecr = true;
                    else if (arrayRow == 0)
                        incrOrDecr = false;
                    arrayRow += incrOrDecr ? -1 : 1;

                }

            }

            //Receiving encrypted message from array

            foreach (List<char> row in array)
            {
                foreach (char receivedCharacter in row)
                {
                    output += receivedCharacter;
                }
            }

            return output;
        }

        public string fenceDecode(string input, int fenceHeight)
        {
            string output = "";

            List<char>[] array = new List<char>[fenceHeight];
            for (int i = 0; i < fenceHeight; i++)
            {
                array[i] = new List<char>();
            }

            bool incrOrDecr = false; //increment or decrement arrayRow [false - increment, true - decrement]
            int arrayRow = 0;

            int[] rowPattern = new int[input.Length]; // Generating fictional fence with arrayRows for each character
            for (int i = 0; i < input.Length; i++)
            {
                rowPattern[i] = arrayRow;

                if (arrayRow == fenceHeight - 1)
                    incrOrDecr = true;
                else if (arrayRow == 0)
                    incrOrDecr = false;

                arrayRow += incrOrDecr ? -1 : 1;
            }

            int charIndex = 0;
            for (int row = 0; row < fenceHeight; row++)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (rowPattern[i] == row)
                    {
                        array[row].Add(input[charIndex]);
                        charIndex++;
                    }
                }
            }

            int[] rowReadIndex = new int[fenceHeight]; // Indywidualne wskaźniki dla każdej listy
            foreach (int row in rowPattern)
            {
                output += array[row][rowReadIndex[row]];
                rowReadIndex[row]++;
            }

            return output;
        }

        private void attribute_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true; // Blocking space

            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (!Regex.IsMatch(Clipboard.GetText(), attributeRegex))
                    e.Handled = true; // Blocking character pasting which are not numbers
            }
        }

        private void attribute_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, attributeRegex)){
                e.Handled = true; //Unabling user to enter character, which is not number
            }
        }

        private void Encode(object sender, RoutedEventArgs e)
        {
            if (!caesarCipher.IsSelected && !vigenereCipher.IsSelected && !affineCipher.IsSelected && !fenceCipher.IsSelected)
            {
                MessageBox.Show("Ustal szyfr!!!");
                return;
            }

            int cipher = -1; //0 - cezara 1 - vigenera(czy jak mu tam) 2 - affine 3 - płotkowy **Czy to jest potrzebne?
            string input = endecodeInput.Text, output = "";
            string key = "";
            int shift = 0; //For Caesar cipher
            int aValue = 0, bValue = 0, fenceHeight = 0; //For affine cipher

            if (caesarCipher.IsSelected == true)
            {
                if (shiftInput.Text.Length > 0)
                {
                    try
                    {
                        shift = Int32.Parse(shiftInput.Text);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"Error: {exc.Message}"); // To nie z czata, visual to wygenerował
                        return;                                   // (i nie wiem czy działa, bo póki co żadnych
                                                                  // błędów nie złapało)
                    }
                }
                else
                {
                    MessageBox.Show("Podaj przesunięcie!");
                    return;
                }

            }
            else if (affineCipher.IsSelected == true)
            {
                if (aInput.Text.Length > 0 && bInput.Text.Length > 0)
                {
                    try // Przydatna rzecz
                    {
                        aValue = Int32.Parse(aInput.Text);
                        bValue = Int32.Parse(bInput.Text);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"Error: {exc.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Podaj wartości a oraz b");
                    return;
                }
            }
            else if (fenceCipher.IsSelected == true)
            {
                if (keyInput.Text.Length > 0)
                {
                    try // Przydatna rzecz
                    {
                        fenceHeight = Int32.Parse(keyInput.Text);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"Error: {exc.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Podaj klucz (wysokość płotka)");
                    return;
                }
            }
            else if (vigenereCipher.IsSelected == true)
            {
                bool isNotNumerical = true;
                for(int i = 0; i < keyInput.Text.Length; i++)
                {
                    if (!Char.IsLetter(keyInput.Text[i])) isNotNumerical = false; // Sprawdzam, czy klucz jest poprawny
                }

                if (keyInput.Text.Length > 0 && isNotNumerical)
                {
                    try // Przydatna rzecz
                    {
                        key = keyInput.Text;
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"Error: {exc.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Podaj poprawny klucz!");
                    return;
                }
            }

            



            //SZYFRY
            if (caesarCipher.IsSelected) //CEZAR
            {
                cipher = 0;
                output = caesarEncode(input, shift);
            }

            else if (vigenereCipher.IsSelected) //VIGENERE
            {
                if(key == ""|| key == null)
                {
                    MessageBox.Show("Podaj klucz!");
                    return;
                }
                cipher = 1;
                output = vigenereEncode(input, key);
            }

            else if (affineCipher.IsSelected) //AFINICZNY (tak to się po polsku pisze?)
            {
                cipher = 2;
                output = affineEncode(input, aValue, bValue);
            }

            else if (fenceCipher.IsSelected) // PŁOTKOWY
            {
                cipher = 3;
                output = fenceEncode(input, fenceHeight);
            }

            endecodeOutput.Text = output;

        }
        private void Decode(object sender, RoutedEventArgs e)
        {
            if (!caesarCipher.IsSelected && !vigenereCipher.IsSelected && !affineCipher.IsSelected && !fenceCipher.IsSelected)
            {
                MessageBox.Show("Ustal szyfr!!!");
                return;
            }

            int cipher = -1; //0 - cezara 1 - vigenera(czy jak mu tam) 2 - affine 3 - płotkowy
            string input = endecodeInput.Text, output = "";
            int shift = 0, aValue = 0, bValue = 0, fenceHeight = 0;
            string key = keyInput.Text;


            if (caesarCipher.IsSelected == true)
            {
                if (shiftInput.Text.Length > 0)
                {
                    try
                    {
                        shift = Int32.Parse(shiftInput.Text);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"Error: {exc.Message}"); // To nie z czata, visual to wygenerował
                        return;                                   // (i nie wiem czy działa, bo póki co żadnych
                                                                  // błędów nie złapało)
                    }
                }
                else
                {
                    MessageBox.Show("Podaj przesunięcie!");
                    return;
                }

            }
            else if (affineCipher.IsSelected == true)
            {
                if (aInput.Text.Length > 0 && bInput.Text.Length > 0)
                {
                    try // Przydatna rzecz
                    {
                        aValue = Int32.Parse(aInput.Text);
                        bValue = Int32.Parse(bInput.Text);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"Error: {exc.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Podaj wartości a oraz b");
                    return;
                }
            }
            else if (fenceCipher.IsSelected == true)
            {
                if (keyInput.Text.Length > 0)
                {
                    try // Przydatna rzecz
                    {
                        fenceHeight = Int32.Parse(keyInput.Text);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"Error: {exc.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Podaj klucz (wysokość płotka)");
                    return;
                }
            }
            else if (vigenereCipher.IsSelected == true)
            {
                bool isNotNumerical = true;
                for (int i = 0; i < keyInput.Text.Length; i++)
                {
                    if (!Char.IsLetter(keyInput.Text[i])) isNotNumerical = false; // Sprawdzam, czy klucz jest poprawny
                }

                if (keyInput.Text.Length > 0 && isNotNumerical)
                {
                    try // Przydatna rzecz
                    {
                        key = keyInput.Text;
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show($"Error: {exc.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Podaj poprawny klucz!");
                    return;
                }
            }


            if (caesarCipher.IsSelected)
            {
                cipher = 0;
                output = caesarDecode(input, shift);
            }
            else if (vigenereCipher.IsSelected)
            {
                if (key == "" || key == null)
                {
                    MessageBox.Show("Podaj przesunięcie!");
                    return;
                }
                cipher = 1;
                output = vigenereDecode(input, key);
            }
            else if (affineCipher.IsSelected)
            {
                cipher = 2;
                output = affineDecode(input, aValue, bValue);
            }
            else if (fenceCipher.IsSelected)
            {
                cipher = 3;
                output = fenceDecode(input, fenceHeight);
            }

            endecodeOutput.Text = output;

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cipherListBox.SelectedItem == null) return;

            string selectedCipher = (cipherListBox.SelectedItem as ListBoxItem).Content.ToString(); //Wybrany szyfr

            
            shiftPANEL.Visibility = Visibility.Collapsed;
            keyPANEL.Visibility = Visibility.Collapsed;
            aPANEL.Visibility = Visibility.Collapsed;
            bPANEL.Visibility = Visibility.Collapsed;

            
            switch (selectedCipher)
            {
                case "Szyfr Cezara":
                    shiftPANEL.Visibility = Visibility.Visible;
                    break;
                case "Szyfr Vigenère’a":
                    keyPANEL.Visibility = Visibility.Visible;
                    break;
                case "Szyfr Affine":
                    aPANEL.Visibility = Visibility.Visible;
                    bPANEL.Visibility = Visibility.Visible;
                    break;
                case "Szyfr płotkowy":
                    keyPANEL.Visibility = Visibility.Visible; 
                    break;
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            endecodeInput.Text = string.Empty;
            endecodeOutput.Text= string.Empty;
            aInput.Text = string.Empty;
            bInput.Text = string.Empty;
            keyInput.Text = string.Empty;
            shiftInput.Text = string.Empty;
            caesarCipher.IsSelected = false;
            vigenereCipher.IsSelected = false;
            affineCipher.IsSelected = false;
            fenceCipher.IsSelected = false;
            Expander.IsExpanded = false;
        }

        private void saveAs()
        {
            Microsoft.Win32.SaveFileDialog saveAsPrompt = new Microsoft.Win32.SaveFileDialog();
            saveAsPrompt.FileName = "szyfr"; // Default file name
            saveAsPrompt.DefaultExt = ".text"; // Default file extension
            saveAsPrompt.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = saveAsPrompt.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                filePath = saveAsPrompt.FileName;
            }
            File.WriteAllText(filePath, endecodeOutput.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(filePath != String.Empty)
            {
                File.WriteAllText(filePath, endecodeOutput.Text);
            }
            else
            {
                saveAs();
            }
            
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            saveAs();
        }

        private void Kill_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
