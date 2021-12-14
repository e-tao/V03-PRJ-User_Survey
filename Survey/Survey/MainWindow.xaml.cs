﻿global using System.Collections.Generic;
global using System.Threading.Tasks;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WebUsers;
using System.Linq;


namespace Survey
{
    public partial class MainWindow : Window
    {
        private List<string> questions = new();
        private Dictionary<string, uint> questionAnswerPair = new();

        private List<uint> questionIdList = new();
        private List<uint> userIdList = new();

        private IEnumerable<DBQuestion> questionsFromDb;
        private ObservableCollection<User> users;

        Random rand = new();

        private uint userId;
        private string ageGroup;
        private string gender;
        //private uint answer;


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
        }

        private async void Flipper_IsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            questionsFromDb = await SurveyModel.GetQuestions();
            Questions.ItemsSource = questionsFromDb;
        }

        private void BtnSaveSurvey_Click(object sender, RoutedEventArgs e)
        {

            // return the questionId in the order the questions were answered.
            // so when writing into database the question and answer pair is preserved.

            foreach(var keyValuePair in questionAnswerPair)
            {
                foreach(var question in questionsFromDb)
                {
                    if (keyValuePair.Key.Equals(question.ToString()))
                    {
                        questionIdList.Add(question.QuestionId);
                    }
                }
            }

            if (questionIdList.Count == 5)
            {
                for (int i = 0; i < questionIdList.Count; i++)
                {
                    SurveyModel.AddAnswer(userId, questionIdList[i], questionAnswerPair.ElementAt(i).Value);
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
            RadioButton rb = sender as RadioButton;
            var seledted = Questions.SelectedItem.ToString();

            if (questionAnswerPair.ContainsKey(seledted))
            {
                questionAnswerPair.Remove(seledted);
            }

            questionAnswerPair.Add(Questions.SelectedItem.ToString(), uint.Parse(rb.Content.ToString()));


/*            foreach (var keyvaluepair in questionAnswerPair)
            {
                Debug.Write(keyvaluepair.Key + " ");
                Debug.WriteLine(keyvaluepair.Value);
            }*/
        }

        private string RandomAgeGroup()
        {
            List<string> ageGroupList = new()
            {
                "18-24 years old",
                "25-34 years old",
                "35-44 years old",
                "45-54 years old",
                "55-64 years old",
                "65-74 years old",
                "75 years or older"
            };

            return ageGroupList[rand.Next(7)];
        }

        private async void AddRandomResults(ObservableCollection<User> users)
        {
            for (int i = 1; i <= 5; i++)
            {
                questionIdList.Add((uint)(i));
            }

            foreach (var user in users)
            {
                userIdList.Add(await SurveyModel.AddUser(user.Name.First, user.Name.Last, RandomAgeGroup(), user.Gender, user.Email));
            }

            foreach (var userId in userIdList)
            {
                for (int i = 0; i < 5; i++)
                {
                    await SurveyModel.AddAnswer(userId, questionIdList[i], (uint)rand.Next(1,6));
                }
            }
            PrograssBar.Visibility = Visibility.Hidden;
        }

        private async void BtnAddRandomUser_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Content.ToString())
            {
                case "+ 20":
                    PrograssBar.Visibility = Visibility.Visible;
                    users = await RandomUsers.GetUsers(rand.Next(20,50));
                    AddRandomResults(users);
                    break;
                case "+ 50":
                    PrograssBar.Visibility = Visibility.Visible;
                    users = await RandomUsers.GetUsers(rand.Next(50,100));
                    AddRandomResults(users);
                    break;
                case "+100":
                    PrograssBar.Visibility = Visibility.Visible;
                    users = await RandomUsers.GetUsers(rand.Next(100, 120));
                    AddRandomResults(users);
                    break;
            }
        }
    }
}
