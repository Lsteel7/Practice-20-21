using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace Practice_20_21
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _inputFilePath;
        private string _outputFile1Path;

        private string _inputFile2Path;
        private string _outputFile2Path;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void FindNumbersButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (!int.TryParse(NDigitTextBox.Text, out int n))
            {
                MessageBox.Show("Введите корректное значение для N (количество цифр).");
                return;
            }

            if (!int.TryParse(KSumTextBox.Text, out int k))
            {
                MessageBox.Show("Введите корректное значение для K (сумма цифр).");
                return;
            }

            
            if (n <= 0 || k <= 0)
            {
                MessageBox.Show("N и K должны быть положительными числами.");
                return;
            }

            
            List<long> numbers = FindNSumDigitNumbers(n, k);

            
            ResultTextBlock.Text = string.Join(", ", numbers);
        }

        
        private List<long> FindNSumDigitNumbers(int n, int k)
        {
            List<long> result = new List<long>();
            FindNSumDigitNumbersRecursive(n, k, 0, 0, result);
            return result;
        }

        
        private void FindNSumDigitNumbersRecursive(int n, int k, int currentDigit, long currentNumber, List<long> result)
        {
            
            if (currentDigit == n)
            {
                
                if (k == 0)
                {
                    result.Add(currentNumber);
                }
                return;
            }

           
            for (int digit = (currentDigit == 0 ? 1 : 0); digit <= 9; digit++)
            {
                
                if (k - digit < 0)
                {
                    continue;
                }

                
                FindNSumDigitNumbersRecursive(n, k - digit, currentDigit + 1, currentNumber * 10 + digit, result);
            }
        }

        private void ReplaceZeros_Click(object sender, RoutedEventArgs e)
        {
            
            string inputText = InputTextBox.Text;

            
            (int[] numberArray, int invalidCount) = ParseInput(inputText);

            
            (int[] replacedArray, int replacementCount) = ReplaceZeros(numberArray);

            
            string outputText = string.Join(" ", replacedArray);

            
            OutputTextBox.Text = outputText;
            CountTextBox.Text = replacementCount.ToString();

            
            if (invalidCount > 0)
            {
                MessageBox.Show($"Обнаружено {invalidCount} некорректных чисел.  Они были проигнорированы.");
            }
        }

        
        private (int[], int) ParseInput(string input)
        {
            string[] numbers = input.Split(' ');
            List<int> numberList = new List<int>();
            int invalidCount = 0;

            foreach (string s in numbers)
            {
                if (int.TryParse(s, out int num))
                {
                    numberList.Add(num);
                }
                else
                {
                    invalidCount++;
                }
            }

            return (numberList.ToArray(), invalidCount);
        }

        
        private (int[], int) ReplaceZeros(int[] numbers)
        {
            int replacementCount = 0;
            int[] replacedArray = numbers.ToArray();

            for (int i = 0; i < replacedArray.Length; i++)
            {
                if (replacedArray[i] == 0)
                {
                    replacedArray[i] = 1;
                    replacementCount++;
                }
            }

            return (replacedArray, replacementCount);
        }

        private void SelectInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            _inputFilePath = GetFilePath("Open");
            InputFilePathTextBox.Text = _inputFilePath;
        }

        private void SelectOutputFile1Button_Click(object sender, RoutedEventArgs e)
        {
            _outputFile1Path = GetFilePath("Save");
            OutputFile1PathTextBox.Text = _outputFile1Path;
        }

        private string GetFilePath(string dialogType)
        {
            string filePath = null;

            if (dialogType == "Open")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    filePath = openFileDialog.FileName;
                }
            }
            else if (dialogType == "Save")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    filePath = saveFileDialog.FileName;
                }
            }

            return filePath;
        }
        private void ProcessDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_inputFilePath) || string.IsNullOrEmpty(_outputFile1Path))
                {
                    SetMessage("Пожалуйста, выберите входной и выходной файлы для задания 1.");
                    return;
                }


                (List<Student> students, string readError) = ReadStudentsFromFile(_inputFilePath);
                if (!string.IsNullOrEmpty(readError))
                {
                    SetMessage(readError);
                    return;
                }


                List<Student> goodStudents = FilterGoodStudents(students);


                string writeError = WriteGoodStudentsToFile(_outputFile1Path, goodStudents);
                if (!string.IsNullOrEmpty(writeError))
                {
                    SetMessage(writeError);
                    return;
                }

                SetMessage("Обработка данных (Задание 1) завершена успешно.");
            }
            catch (Exception ex)
            {
                SetMessage($"Произошла ошибка (Задание 1): {ex.Message}");
            }
        }


        private void SetMessage(string message)
        {
            MessageTextBox.Text = message;
        }
        private class Student
        {
            public string LastName { get; set; }
            public DateTime BirthDate { get; set; }
            public List<int> Grades { get; set; } = new List<int>();
        }

        private (List<Student>, string) ReadStudentsFromFile(string filePath)
        {
            List<Student> students = new List<Student>();
            string errorMessage = null;

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(';');
                        if (parts.Length < 3) continue;

                        Student student = new Student();
                        student.LastName = parts[0];
                        if (DateTime.TryParse(parts[1], out DateTime birthDate))
                        {
                            student.BirthDate = birthDate;
                        }
                        else
                        {
                            errorMessage = "Неверный формат даты рождения";
                            return (null, errorMessage);
                        }
                        string[] grades = parts[2].Split(',');
                        foreach (string grade in grades)
                        {
                            if (int.TryParse(grade.Trim(), out int parsedGrade))
                            {
                                student.Grades.Add(parsedGrade);
                            }
                        }

                        students.Add(student);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Ошибка при чтении файла (Задание 1): {ex.Message}";
            }

            return (students, errorMessage);
        }

        private List<Student> FilterGoodStudents(List<Student> students)
        {
            return students.Where(s => !s.Grades.Contains(3)).ToList();
        }

        private string WriteGoodStudentsToFile(string filePath, List<Student> students)
        {
            string errorMessage = null;

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (Student student in students)
                    {
                        writer.WriteLine($"{student.LastName};{student.BirthDate.ToShortDateString()}");
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Ошибка при записи в файл (Задание 1): {ex.Message}";
            }

            return errorMessage;
        }
    }
}

