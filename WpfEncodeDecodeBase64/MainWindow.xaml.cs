using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace WpfEncodeDecodeBase64
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FileInfo sourceFile;
        int charPerLine = 77;   // кол-во символов в линиях строки, значение по умолчанию

        public MainWindow()
        {
            InitializeComponent();

            // получение пути по которому запущено приложение
            // используется для получения пути и имени файла для сохранения, если контент берется не из файла
            tbxSaveFilePath.Text = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) +
                "\\EncodedBase64.txt";
        }

        /// <summary>
        /// Обработчик события открытия файла
        /// </summary>
        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            // вызов стандартного окна открытия файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                sourceFile = new FileInfo(openFileDialog.FileName);
            }

            // проверка условия что файл был открыт
            if (sourceFile != null)
            {
                tbxSourceFilePath.Text = sourceFile.FullName;
                tbxFileContets.Text = File.ReadAllText(sourceFile.FullName);

                // вывод информации о файле
                tbLength.Text += Convert.ToString(sourceFile.Length) + " byte";
                tbCreationTime.Text += sourceFile.CreationTime.ToString();
                tbLastWriteTime.Text += sourceFile.LastWriteTime.ToString();

                // путь и имя файла для сохранения, по умолчанию
                tbxSaveFilePath.Text = sourceFile.DirectoryName + "\\" +
                    sourceFile.Name.Trim(sourceFile.Extension.ToCharArray()) +
                    "_encoded.txt";
            }

        }

        /// <summary>
        /// Обработчик события изменений в окне контента
        /// </summary>
        private void TbxFileContets_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ограничение, т.к. это событие наступает раньше инициализации поля tbxFileEncoded
            if (tbxFileEncoded == null) return;


            byte[] buffer = new byte[0];

            // Получение массива байт из строки в кодировке UTF-8
            if (rbUtf8 != null && (bool)rbUtf8.IsChecked)
                buffer = Encoding.UTF8.GetBytes(tbxFileContets.Text);

            // Получение массива байт из строки в кодировке UTF-16
            if (rbUtf16 != null && (bool)rbUtf16.IsChecked)
                buffer = Encoding.Unicode.GetBytes(tbxFileContets.Text);

            // Получение строки закодированной в Base64
            string encodedStr = Convert.ToBase64String(buffer);

            int charCount = 0;

            StringWriter res = new StringWriter();

            // разбивка линий строки на заданное кол-во символов
            for (int i = 0; i < encodedStr.Length; i++)
            {
                res.Write(encodedStr[i]);

                charCount++;

                if (charCount == charPerLine - 1)
                {
                    res.Write('\r');
                    charCount = 0;
                }
            }

            tbxFileEncoded.Text = res.ToString();
        }

        /// <summary>
        /// Обработчик события на изменение кол-ва символов в линии
        /// </summary>
        private void TbxCharPerLine_TextChanged(object sender, TextChangedEventArgs e)
        {
            int number;

            if (int.TryParse(tbxCharPerLine.Text, out number))
                charPerLine = number;
            else
                tbxCharPerLine.Text = charPerLine.ToString();

            TbxFileContets_TextChanged(sender, e);
        }

        /// <summary>
        /// Обработчик события установки кодировки UTF-16
        /// </summary>
        private void RbUtf16_Click(object sender, RoutedEventArgs e)
        {
            TextChangedEventArgs ev = null;
            TbxFileContets_TextChanged(sender, ev);
        }

        /// <summary>
        /// Обработчик события установки кодировки UTF-16
        /// </summary>
        private void RbUtf8_Click(object sender, RoutedEventArgs e)
        {
            TextChangedEventArgs ev = null;
            TbxFileContets_TextChanged(sender, ev);
        }

        /// <summary>
        /// Обработчик события сохранения закодированной строки в файл
        /// </summary>
        private void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(tbxSaveFilePath.Text, tbxFileEncoded.Text);
        }
    }
}
