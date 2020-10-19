using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Playground.WpfApp.Controls
{
    public class ScrollingContentControl : ContentControl
    {
        bool _processing;

        DispatcherTimer _centerTimer;

        Storyboard _scrollIn;
        Storyboard _scrollOut;

        Queue<string> _requests = new Queue<string>(); //replace with ConcurrentQueue<string> if multiple threads will be adding to InfoContent property

        public double CenterTime
        {
            get { return (double)GetValue(CenterTimeProperty); }
            set { SetValue(CenterTimeProperty, value); }
        }

        public static readonly DependencyProperty CenterTimeProperty =
            DependencyProperty.Register("CenterTime", typeof(double), typeof(ScrollingContentControl), new PropertyMetadata(2.0d));


        public string InfoContent
        {
            get { return (string)GetValue(InfoContentProperty); }
            set { SetValue(InfoContentProperty, value); }
        }

        public static readonly DependencyProperty InfoContentProperty =
            DependencyProperty.Register("InfoContent", typeof(string), typeof(ScrollingContentControl), new PropertyMetadata(string.Empty, InfoContentCallback));

        private static void InfoContentCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollingContentControl control = (ScrollingContentControl)d;
            if (!string.IsNullOrEmpty(e.NewValue as string))
                control.EnqueueNewInfo(e.NewValue as string);
        }

        public ScrollingContentControl()
        {
            InitStoryboards();
            RenderTransform = new TranslateTransform();
            Visibility = Visibility.Collapsed;
        }

        private void InitStoryboards()
        {
            Contract.Ensures(_scrollIn != null, "Failed to init scroll in animation");
            Contract.Ensures(_scrollOut != null, "Failed to init scroll out animation");
            _scrollIn = FindResource("scrollIn") as Storyboard;
            _scrollOut = FindResource("scrollOut") as Storyboard;
        }

        private void EnqueueNewInfo(string info)
        {
            _requests.Enqueue(info);
            if (!_processing)
                HandleQueue();
        }

        private void HandleQueue()
        {
            _processing = true;

            if (_centerTimer == null)
                InitTimer();

            string info = _requests.Peek();
            Content = info;

            var scrollInClone = _scrollIn.Clone();
            scrollInClone.Completed += (s, e) =>
            {
                _centerTimer.Tick += TimerTick;
                _centerTimer.Start();
            };

            scrollInClone.Begin(this);
            Visibility = Visibility.Visible;
        }

        private void TimerTick(object sender, EventArgs args)
        {
            _centerTimer.Tick -= TimerTick;
            _centerTimer.Stop();
            var scrollOutClone = _scrollOut.Clone();
            scrollOutClone.Completed += (snd, ear) =>
            {
                Visibility = Visibility.Collapsed;
                if (_requests.Count > 0)
                    _requests.Dequeue();
                if (_requests.Count == 0)
                    _processing = false;
                else
                {
                    CheckTimeInterval();
                    HandleQueue();
                }
            };
            scrollOutClone.Begin(this);
        }

        private void InitTimer()
        {
            _centerTimer = new DispatcherTimer();
            _centerTimer.Interval = TimeSpan.FromSeconds(CenterTime);
        }

        private void CheckTimeInterval()
        {
            if (_centerTimer == null)
                return;
            if (_centerTimer.Interval != TimeSpan.FromSeconds(CenterTime))
                _centerTimer.Interval = TimeSpan.FromSeconds(CenterTime);
        }
    }
}
