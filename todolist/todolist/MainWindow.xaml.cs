using System;
using System.Collections.Generic;
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

namespace todolist
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTask();
        }

        private void AddTask()
        {
            string taskText = TaskTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(taskText))
            {
                // Создаем Grid с ТАКИМИ ЖЕ колонками как в основном окне
                Grid taskGrid = new Grid();
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                taskGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                // Колонка 0: Дата
                Border dateBorder = new Border();
                dateBorder.BorderBrush = Brushes.Black;
                dateBorder.BorderThickness = new Thickness(1);
                TextBlock dateText = new TextBlock();
                dateText.Text = DateTime.Now.ToString("dd.MM.yyyy");
                dateText.TextAlignment = TextAlignment.Center;
                dateText.VerticalAlignment = VerticalAlignment.Center;
                dateText.Margin = new Thickness(2);
                dateBorder.Child = dateText;
                Grid.SetColumn(dateBorder, 0);

                // Колонка 1: Текст задачи
                Border taskBorder = new Border();
                taskBorder.BorderBrush = Brushes.Black;
                taskBorder.BorderThickness = new Thickness(1);
                TextBlock taskTextBlock = new TextBlock();
                taskTextBlock.Text = taskText;
                taskTextBlock.VerticalAlignment = VerticalAlignment.Center;
                taskTextBlock.Margin = new Thickness(5);
                taskBorder.Child = taskTextBlock;
                Grid.SetColumn(taskBorder, 1);

                // Колонка 2: Выполнение (Чекбокс)
                Border completionBorder = new Border();
                completionBorder.BorderBrush = Brushes.Black;
                completionBorder.BorderThickness = new Thickness(1);
                CheckBox completionCheckBox = new CheckBox();
                completionCheckBox.VerticalAlignment = VerticalAlignment.Center;
                completionCheckBox.HorizontalAlignment = HorizontalAlignment.Center;
                completionCheckBox.Margin = new Thickness(3);
                completionBorder.Child = completionCheckBox;
                Grid.SetColumn(completionBorder, 2);

                // Колонка 3: Категория
                Border categoryBorder = new Border();
                categoryBorder.BorderBrush = Brushes.Black;
                categoryBorder.BorderThickness = new Thickness(1);
                TextBlock categoryText = new TextBlock();
                categoryText.Text = "Общее";
                categoryText.TextAlignment = TextAlignment.Center;
                categoryText.VerticalAlignment = VerticalAlignment.Center;
                categoryText.Margin = new Thickness(10, 3, 10, 3);
                categoryBorder.Child = categoryText;
                Grid.SetColumn(categoryBorder, 3);

                // Добавляем все элементы в Grid
                taskGrid.Children.Add(dateBorder);
                taskGrid.Children.Add(taskBorder);
                taskGrid.Children.Add(completionBorder);
                taskGrid.Children.Add(categoryBorder);

                // Добавляем задачу в панель
                TasksPanel.Children.Add(taskGrid);

                // Очищаем текстовое поле
                TaskTextBox.Clear();
                TaskTextBox.Focus();
            }
        }
    }

}
