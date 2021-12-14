﻿global using System.Collections.Generic;
global using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WebUsers;


namespace Survey
{
    public partial class MainWindow : Window
    {
        private List<string> questions = new();
        private List<uint> answerList = new();
        private List<uint> questionIdList = new();
        private IEnumerable<DBQuestion> questionsFromDb;


        private ObservableCollection<User> users;

        private uint userId;
        private string ageGroup;
        private string gender;
        private uint answer;


        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task InitDB()
        {
            SurveyModel.DBName = "V03_Survey_Cheng";
            await SurveyModel.CreateDatabase();
            if (await SurveyModel.GetNumberQuestion() == 0)
            {
                questions.Add("It is easy to navigate through the app.");
                questions.Add("It is easy to create a new user account.");
                questions.Add("The booking process is smooth and easy.");
                questions.Add("I will use this service again.");
                questions.Add("I will recommand FireRnR to my family and friends.");
                await SurveyModel.AddQuestions(questions);
            }
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitDB();

            //users = await RandomUsers.GetUsers(100);
            //randomusers.ItemsSource = users;
        }

        private async void Flipper_IsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            questionsFromDb = await SurveyModel.GetQuestions();
            foreach(var question in questionsFromDb)
            {
                questionIdList.Add(question.QuestionId);
            }

            Questions.ItemsSource = questionsFromDb;
        }

        private void BtnSaveSurvey_Click(object sender, RoutedEventArgs e)
        {
            if (answerList.Count == 5)
            {
                for (int i = 0; i < answerList.Count; i++)
                {
                    SurveyModel.AddAnswer(userId, questionIdList[i], answerList[i]);
                }
            }

            AddUserLeft.Visibility = Visibility.Hidden;
            AddUserRight.Visibility = Visibility.Hidden;
            Thanks.Visibility = Visibility.Visible;
        }

        private async void BtnSaveUser_Click(object sender, RoutedEventArgs e)
        {
            userId = await SurveyModel.AddUser(First.Text, Last.Text, ageGroup, gender, Email.Text);
        }

        private void RbAgeGroup_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            ageGroup = rb.Content.ToString();
        }

        private void RbGender_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            gender = rb.Content.ToString();
        }

        private void RbAnswers_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton ;
            answer = uint.Parse(rb.Content.ToString());
            answerList.Add(answer);
        }
    }
}
