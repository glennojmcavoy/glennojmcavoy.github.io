using HelloWorld.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HelloWorld
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagePage : ContentPage
    {

        private string LastMsgSequence = "fff";
        static bool MsgSearch = true;
        int RowNumberCounter = 0;

        public MessagePage()
        {

            InitializeComponent();
            // CheckLastLogin();

        }

        protected override void OnAppearing()
        {

            base.OnAppearing();
            MsgSearch = true;
            LoadMessages();

        }

        protected override void OnDisappearing()
        {

            base.OnAppearing();
            MsgSearch = false;

        }

        public async void LoadMessages()
        {

            while (MsgSearch == true)
            {

                var response = await App.ass.GetMessagesAsync();
                var Messages = await response.Content.ReadAsStringAsync();

                if (Messages != LastMsgSequence)
                {

                    LastMsgSequence = Messages;

                    var msgs = Messages.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    RowNumberCounter = 0;

                    grid.Children.Clear();
                    grid.RowDefinitions.Clear();

                    foreach (String msg in msgs)
                    {

                        RenderMessages(msg);

                    }

                }

            }

        }

        private void ShowMenu_Clicked(object sender, EventArgs e)
        {

            var x = App.Current.MainPage;
            (x as MasterDetailPage).IsPresented = true;

        }

        private void NewChat_Clicked(object sender, EventArgs e)
        {

            GetNewRecipient();

        }

        public async void GetNewRecipient()
        {

            HttpResponseMessage response = await App.ass.GetNewRecipient();
            var responseBodyAsText = await response.Content.ReadAsStringAsync();

            if (responseBodyAsText == "")
            {
                await DisplayAlert("Sorry!", "There are no new users for you to chat with :(", "OK");
                return;
            }

            var records = responseBodyAsText.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            Random r = new Random();
            var RId = Int32.Parse(records[r.Next(records.Length)]);
            await Navigation.PushAsync(new ChatPage(RId));
        }

        public async void CheckLastLogin()
        {

            var lastLoginResponse = await App.ass.CheckLastLoginAsync();
            var responseBodyAsText = await lastLoginResponse.Content.ReadAsStringAsync();

            if (responseBodyAsText == "")
            {

                Answer a = new Answer();

                a.Value = await DisplayActionSheet("How are you feeling today?", "Not Now", null,
                    "Happy", "Neutral", "Sad");

                if (a.Value != "Not Now")
                {

                    var postAnswerResponse = await App.ass.PostAnswerAsync(a);

                }

            }
        }

        public void RenderMessages(String msg)
        {

            Label msgLabel = new Label();

            msgLabel.TextColor = Color.WhiteSmoke;
            msgLabel.BackgroundColor = Color.LightBlue;
            msgLabel.HorizontalOptions = LayoutOptions.FillAndExpand;

            Thickness ml = msgLabel.Margin;
            ml.Left = 10;
            msgLabel.Margin = ml;

            Thickness mt = msgLabel.Margin;
            mt.Top = 12;
            msgLabel.Margin = mt;

            var data = msg.Split(',');

            grid.RowDefinitions.Add(new RowDefinition() { Height = (GridLength)60 });

            msgLabel.Text = data[1] + "  -  " + data[2];

            Grid g = new Grid();
            Thickness gt = msgLabel.Margin;
            gt.Top = 10;
            g.RowSpacing = 0;
            g.Margin = gt;
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            Label l = new Label();
            l.BackgroundColor = Color.LightGray;
            l.HeightRequest = 1;

            Label l2 = new Label();
            l2.BackgroundColor = Color.LightGray;
            l2.HeightRequest = 1;

            Grid.SetRow(l, 0);
            Grid.SetColumn(l, 0);

            Grid g2 = new Grid();
            g2.RowDefinitions.Add(new RowDefinition());
            g2.ColumnDefinitions.Add(new ColumnDefinition() { Width = (GridLength)40 });
            g2.ColumnDefinitions.Add(new ColumnDefinition());
            g2.ColumnDefinitions.Add(new ColumnDefinition() { Width = (GridLength)20 });

            Grid.SetRow(msgLabel, 0);
            Grid.SetColumn(msgLabel, 1);

            Image chatIcon = new Image();
            chatIcon.Source = "chat.png";

            Thickness ciml = chatIcon.Margin;
            ciml.Left = 10;
            chatIcon.Margin = ciml;

            Thickness cimb = chatIcon.Margin;
            cimb.Bottom = 10;
            chatIcon.Margin = cimb;

            Image sendIcon = new Image();
            sendIcon.Source = "rarrow.png";

            Thickness simb = sendIcon.Margin;
            simb.Bottom = 10;
            sendIcon.Margin = simb;

            Thickness simr = sendIcon.Margin;
            simr.Bottom = 10;
            sendIcon.Margin = simr;

            Grid.SetRow(chatIcon, 0);
            Grid.SetColumn(chatIcon, 0);

            Grid.SetRow(sendIcon, 0);
            Grid.SetColumn(sendIcon, 2);

            g2.Children.Add(msgLabel);
            g2.Children.Add(chatIcon);
            g2.Children.Add(sendIcon);

            g2.BackgroundColor = Color.LightBlue;
            Grid.SetRow(g2, 1);
            Grid.SetColumn(g2, 0);

            Grid.SetRow(l2, 2);
            Grid.SetColumn(l2, 0);

            g.Children.Add(l);
            g.Children.Add(g2);
            g.Children.Add(l2);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => {
                string x = (s as Grid).StyleId;
                Navigation.PushAsync(new ChatPage(Int32.Parse(x)));
            };

            g.GestureRecognizers.Add(tapGestureRecognizer);
            g.StyleId = data[0];
            Grid.SetRow(g, RowNumberCounter);
            RowNumberCounter++;
            Grid.SetColumn(g, 0);
            this.grid.Children.Add(g);
        }
    }
}