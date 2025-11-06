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
using System.Xml;

namespace todolist
{
    public partial class MainWindow : Window
    {
        private List<Grid> allTasks = new List<Grid>();
        private string dataFilePath = "tasks.xml";
        private TextBlock statusTextBlock;

        public MainWindow()
        {
            InitializeComponent();
            CreateStatusBar();
            LoadTasks();
            UpdateStatus();
        }

        private void CreateStatusBar()
        {
            statusTextBlock = new TextBlock();
            statusTextBlock.FontSize = 12;
            statusTextBlock.FontWeight = FontWeights.Normal;
            statusTextBlock.TextAlignment = TextAlignment.Center;
            taskGrid.Children.Add(statusTextBlock);
            Grid.SetRow(statusTextBlock, 4);
            Grid.SetColumnSpan(statusTextBlock, 5);
            
        }

        private void UpdateStatus()
        {
            int totalTasks = allTasks.Count;
            int completedTasks = 0;
            int nocompletedTasks = 0;

            foreach (var taskGrid in allTasks)
            {
                if (taskGrid.Children[3] is Border completionBorder &&
                    completionBorder.Child is CheckBox completionCheckBox)
                {
                    if (completionCheckBox.IsChecked == true)
                    {
                        completedTasks++;
                    }
                    else
                    {
                        nocompletedTasks++;
                    }
                }
            }

            statusTextBlock.Text = $"Всего заданий: {totalTasks} | Выполненные: {completedTasks} | Невыполненные: {nocompletedTasks}";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveTasks();
        }

        private void SaveTasks()
        {
            using (XmlWriter writer = XmlWriter.Create(dataFilePath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Tasks");

                foreach (var taskGrid in allTasks)
                {
                    writer.WriteStartElement("Task");

                    // Получаем элементы из Grid
                    Border dateBorder = taskGrid.Children[0] as Border;
                    Border taskBorder = taskGrid.Children[1] as Border;
                    Border completionBorder = taskGrid.Children[3] as Border;
                    Border categoryBorder = taskGrid.Children[4] as Border;

                    if (dateBorder?.Child is StackPanel datePanel &&
                        datePanel.Children[0] is TextBlock creationDateText &&
                        datePanel.Children[1] is TextBlock completionDateText &&
                        taskBorder?.Child is TextBlock taskTextBlock &&
                        completionBorder?.Child is CheckBox completionCheckBox &&
                        categoryBorder?.Child is ComboBox categoryComboBox)
                    {
                        writer.WriteElementString("Text", taskTextBlock.Text);
                        writer.WriteElementString("CreationDate", creationDateText.Text.Split('\n')[0].Trim());
                        writer.WriteElementString("IsCompleted", completionCheckBox.IsChecked.ToString());
                        if (completionDateText.Visibility == Visibility.Visible)
                        {
                            writer.WriteElementString("CompletionDate", completionDateText.Text);
                        }
                        else
                        {
                            writer.WriteElementString("CompletionDate", "");
                        }
                        writer.WriteElementString("Category", categoryComboBox.SelectedItem as string);
                        writer.WriteElementString("HasStrikethrough", taskTextBlock.TextDecorations != null ? "True" : "False");
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void LoadTasks()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(dataFilePath);

            XmlNodeList taskNodes = doc.SelectNodes("/Tasks/Task");
            if (taskNodes == null) return;

            foreach (XmlNode taskNode in taskNodes)
            {
                string text = taskNode.SelectSingleNode("Text")?.InnerText ?? "";
                string creationDateStr = taskNode.SelectSingleNode("CreationDate")?.InnerText ?? "";
                string isCompletedStr = taskNode.SelectSingleNode("IsCompleted")?.InnerText ?? "False";
                string completionDateStr = taskNode.SelectSingleNode("CompletionDate")?.InnerText ?? "";
                string category = taskNode.SelectSingleNode("Category")?.InnerText ?? "Прочее";
                string hasStrikethroughStr = taskNode.SelectSingleNode("HasStrikethrough")?.InnerText ?? "False";

                bool isCompleted = bool.Parse(isCompletedStr);
                bool hasStrikethrough = bool.Parse(hasStrikethroughStr);

                Grid taskGrid = new Grid();
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                taskGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                DateTime creationDate;
                if (!DateTime.TryParse(creationDateStr, out creationDate))
                {
                    creationDate = DateTime.Now;
                }

                Border dateBorder = new Border();
                dateBorder.BorderBrush = Brushes.Black;
                dateBorder.BorderThickness = new Thickness(1);

                StackPanel datePanel = new StackPanel();
                datePanel.Margin = new Thickness(3);

                TextBlock creationDateText = new TextBlock();
                creationDateText.Text = creationDate.ToString("dd.MM.yyyy HH:mm");
                creationDateText.TextAlignment = TextAlignment.Center;
                creationDateText.FontSize = 10;

                TextBlock completionDateText = new TextBlock();
                completionDateText.Text = completionDateStr;
                completionDateText.TextAlignment = TextAlignment.Center;
                completionDateText.FontSize = 10;
                completionDateText.FontWeight = FontWeights.Bold;
                completionDateText.Foreground = Brushes.Green;
                completionDateText.Visibility = string.IsNullOrEmpty(completionDateStr) ? Visibility.Collapsed : Visibility.Visible;

                datePanel.Children.Add(creationDateText);
                datePanel.Children.Add(completionDateText);
                dateBorder.Child = datePanel;
                Grid.SetColumn(dateBorder, 0);

                Border taskBorder = new Border();
                taskBorder.BorderBrush = Brushes.Black;
                taskBorder.BorderThickness = new Thickness(1);
                TextBlock taskTextBlock = new TextBlock();
                taskTextBlock.Text = text;
                taskTextBlock.VerticalAlignment = VerticalAlignment.Center;
                taskTextBlock.Margin = new Thickness(5);

                if (hasStrikethrough)
                {
                    taskTextBlock.TextDecorations = TextDecorations.Strikethrough;
                }

                taskBorder.Child = taskTextBlock;
                Grid.SetColumn(taskBorder, 1);

                Border deleteBorder = new Border();
                deleteBorder.BorderBrush = Brushes.Black;
                deleteBorder.BorderThickness = new Thickness(1);
                Button deleteButton = new Button();
                deleteButton.Content = "X";
                deleteButton.VerticalAlignment = VerticalAlignment.Center;
                deleteButton.HorizontalAlignment = HorizontalAlignment.Center;
                deleteButton.Margin = new Thickness(3);
                deleteButton.Padding = new Thickness(8, 3, 8, 3);
                deleteButton.Background = Brushes.LightCoral;
                deleteButton.Foreground = Brushes.White;
                deleteButton.Click += (s, e) =>
                {
                    if (TasksPanel != null)
                        TasksPanel.Children.Remove(taskGrid);
                    allTasks.Remove(taskGrid);
                    UpdateStatus();
                };
                deleteBorder.Child = deleteButton;
                Grid.SetColumn(deleteBorder, 2);

                Border completionBorder = new Border();
                completionBorder.BorderBrush = Brushes.Black;
                completionBorder.BorderThickness = new Thickness(1);
                CheckBox completionCheckBox = new CheckBox();
                completionCheckBox.VerticalAlignment = VerticalAlignment.Center;
                completionCheckBox.HorizontalAlignment = HorizontalAlignment.Center;
                completionCheckBox.Margin = new Thickness(8);
                completionCheckBox.Tag = creationDate;
                completionCheckBox.IsChecked = isCompleted;
                completionBorder.Child = completionCheckBox;
                Grid.SetColumn(completionBorder, 3);

                Border categoryBorder = new Border();
                categoryBorder.BorderBrush = Brushes.Black;
                categoryBorder.BorderThickness = new Thickness(1);

                ComboBox categoryComboBox = new ComboBox();
                categoryComboBox.VerticalAlignment = VerticalAlignment.Center;
                categoryComboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                categoryComboBox.Margin = new Thickness(2);
                categoryComboBox.Padding = new Thickness(2);
                categoryComboBox.FontSize = 11;
                categoryComboBox.MinWidth = 70;

                categoryComboBox.Items.Add("Дом");
                categoryComboBox.Items.Add("Работа");
                categoryComboBox.Items.Add("Учеба");
                categoryComboBox.Items.Add("Прочее");

                int categoryIndex = categoryComboBox.Items.IndexOf(category);
                if (categoryIndex >= 0)
                {
                    categoryComboBox.SelectedIndex = categoryIndex;
                }
                else
                {
                    categoryComboBox.SelectedIndex = 3;
                }

                categoryComboBox.SelectionChanged += (s, e) =>
                {
                    ApplyFilter();
                };

                categoryBorder.Child = categoryComboBox;
                Grid.SetColumn(categoryBorder, 4);

                completionCheckBox.Checked += (s, e) =>
                {
                    DateTime completionDate = DateTime.Now;
                    creationDateText.Text = $"{creationDate:dd.MM.yyyy HH:mm}";
                    completionDateText.Text = $"{completionDate:dd.MM.yyyy HH:mm}";
                    completionDateText.Visibility = Visibility.Visible;
                    taskGrid.Background = Brushes.LightGreen;
                    taskTextBlock.TextDecorations = TextDecorations.Strikethrough;
                    UpdateStatus();
                };

                completionCheckBox.Unchecked += (s, e) =>
                {
                    creationDateText.Text = creationDate.ToString("dd.MM.yyyy HH:mm");
                    completionDateText.Visibility = Visibility.Collapsed;
                    taskGrid.Background = Brushes.Transparent;
                    taskTextBlock.TextDecorations = null;
                    UpdateStatus();
                };

                if (isCompleted)
                {
                    taskGrid.Background = Brushes.LightGreen;
                    if (!hasStrikethrough)
                    {
                        taskTextBlock.TextDecorations = TextDecorations.Strikethrough;
                    }
                }

                taskGrid.Children.Add(dateBorder);
                taskGrid.Children.Add(taskBorder);
                taskGrid.Children.Add(deleteBorder);
                taskGrid.Children.Add(completionBorder);
                taskGrid.Children.Add(categoryBorder);

                if (TasksPanel != null)
                {
                    TasksPanel.Children.Add(taskGrid);
                    allTasks.Add(taskGrid);
                }
            }

            ApplyFilter();
            UpdateStatus();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTask();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (TasksPanel == null) return;

            string selectedFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content as string;

            TasksPanel.Children.Clear();

            if (selectedFilter == "Фильтр" || selectedFilter == "Все категории")
            {
                foreach (var task in allTasks)
                {
                    TasksPanel.Children.Add(task);
                }
            }
            else
            {
                foreach (var task in allTasks)
                {
                    if (task.Children[4] is Border categoryBorder &&
                        categoryBorder.Child is ComboBox categoryComboBox)
                    {
                        string taskCategory = categoryComboBox.SelectedItem as string;
                        if (taskCategory == selectedFilter)
                        {
                            TasksPanel.Children.Add(task);
                        }
                    }
                }
            }

            UpdateStatus();
        }

        private void AddTask()
        {
            string taskText = TaskTextBox.Text.Trim();

            {
                Grid taskGrid = new Grid();
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                taskGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                taskGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                DateTime creationDate = DateTime.Now;

                Border dateBorder = new Border();
                dateBorder.BorderBrush = Brushes.Black;
                dateBorder.BorderThickness = new Thickness(1);

                StackPanel datePanel = new StackPanel();
                datePanel.Margin = new Thickness(3);

                TextBlock creationDateText = new TextBlock();
                creationDateText.Text = creationDate.ToString("dd.MM.yyyy HH:mm");
                creationDateText.TextAlignment = TextAlignment.Center;
                creationDateText.FontSize = 10;

                TextBlock completionDateText = new TextBlock();
                completionDateText.Text = "";
                completionDateText.TextAlignment = TextAlignment.Center;
                completionDateText.FontSize = 10;
                completionDateText.FontWeight = FontWeights.Bold;
                completionDateText.Foreground = Brushes.Green;
                completionDateText.Visibility = Visibility.Collapsed;

                datePanel.Children.Add(creationDateText);
                datePanel.Children.Add(completionDateText);
                dateBorder.Child = datePanel;
                Grid.SetColumn(dateBorder, 0);

                Border taskBorder = new Border();
                taskBorder.BorderBrush = Brushes.Black;
                taskBorder.BorderThickness = new Thickness(1);
                TextBlock taskTextBlock = new TextBlock();
                taskTextBlock.Text = taskText;
                taskTextBlock.VerticalAlignment = VerticalAlignment.Center;
                taskTextBlock.Margin = new Thickness(5);
                taskBorder.Child = taskTextBlock;
                Grid.SetColumn(taskBorder, 1);

                Border deleteBorder = new Border();
                deleteBorder.BorderBrush = Brushes.Black;
                deleteBorder.BorderThickness = new Thickness(1);
                Button deleteButton = new Button();
                deleteButton.Content = "X";
                deleteButton.VerticalAlignment = VerticalAlignment.Center;
                deleteButton.HorizontalAlignment = HorizontalAlignment.Center;
                deleteButton.Margin = new Thickness(3);
                deleteButton.Padding = new Thickness(8, 3, 8, 3);
                deleteButton.Background = Brushes.LightCoral;
                deleteButton.Foreground = Brushes.White;
                deleteButton.Click += (s, e) =>
                {
                    if (TasksPanel != null)
                        TasksPanel.Children.Remove(taskGrid);
                    allTasks.Remove(taskGrid);
                    UpdateStatus();
                };
                deleteBorder.Child = deleteButton;
                Grid.SetColumn(deleteBorder, 2);

                Border completionBorder = new Border();
                completionBorder.BorderBrush = Brushes.Black;
                completionBorder.BorderThickness = new Thickness(1);
                CheckBox completionCheckBox = new CheckBox();
                completionCheckBox.VerticalAlignment = VerticalAlignment.Center;
                completionCheckBox.HorizontalAlignment = HorizontalAlignment.Center;
                completionCheckBox.Margin = new Thickness(8);
                completionCheckBox.Tag = creationDate;
                completionBorder.Child = completionCheckBox;
                Grid.SetColumn(completionBorder, 3);

                Border categoryBorder = new Border();
                categoryBorder.BorderBrush = Brushes.Black;
                categoryBorder.BorderThickness = new Thickness(1);

                ComboBox categoryComboBox = new ComboBox();
                categoryComboBox.VerticalAlignment = VerticalAlignment.Center;
                categoryComboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                categoryComboBox.Margin = new Thickness(2);
                categoryComboBox.Padding = new Thickness(2);
                categoryComboBox.FontSize = 11;
                categoryComboBox.MinWidth = 70;

                categoryComboBox.Items.Add("Дом");
                categoryComboBox.Items.Add("Работа");
                categoryComboBox.Items.Add("Учеба");
                categoryComboBox.Items.Add("Прочее");

                categoryComboBox.SelectedIndex = 3;
                categoryComboBox.SelectionChanged += (s, e) =>
                {
                    ApplyFilter();
                };

                categoryBorder.Child = categoryComboBox;
                Grid.SetColumn(categoryBorder, 4);

                completionCheckBox.Checked += (s, e) =>
                {
                    DateTime completionDate = DateTime.Now;
                    creationDateText.Text = $"{creationDate:dd.MM.yyyy HH:mm}";
                    completionDateText.Text = $"{completionDate:dd.MM.yyyy HH:mm}";
                    completionDateText.Visibility = Visibility.Visible;
                    taskGrid.Background = Brushes.LightGreen;
                    taskTextBlock.TextDecorations = TextDecorations.Strikethrough;
                    UpdateStatus();
                };

                completionCheckBox.Unchecked += (s, e) =>
                {
                    creationDateText.Text = creationDate.ToString("dd.MM.yyyy HH:mm");
                    completionDateText.Visibility = Visibility.Collapsed;
                    taskGrid.Background = Brushes.Transparent;
                    taskTextBlock.TextDecorations = null;
                    UpdateStatus();
                };

                taskGrid.Children.Add(dateBorder);
                taskGrid.Children.Add(taskBorder);
                taskGrid.Children.Add(deleteBorder);
                taskGrid.Children.Add(completionBorder);
                taskGrid.Children.Add(categoryBorder);

                if (TasksPanel != null)
                {
                    TasksPanel.Children.Add(taskGrid);
                    allTasks.Add(taskGrid);
                }

                TaskTextBox.Clear();
                TaskTextBox.Focus();

                UpdateStatus();
            }
        }
    }
}