using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Playground.ConsoleApp.DelegateEx
{
    public class VideoEventArgs : EventArgs
    {
        public Video Video { get; set; }
        public bool Status { get; set; }
        public DateTime CompletionTime { get; set; }
    }

    public class Video
    {
        public string Title { get; set; }
    }

    public class MailService
    {
        public void OnVideoEncoded(object source, VideoEventArgs e)
        {
            Console.WriteLine($"MailService: Sending an email...{e.Video.Title}");   
        }
    }

    public class MessageService
    {
        public void OnVideoEncoded(object source, VideoEventArgs e)
        {
            Console.WriteLine($"MessageService: Sending an textMessage... {e.Video.Title}");
        }
    }

    public class VideoEncoder
    {
        public event EventHandler<VideoEventArgs> VideoEncoded;

        public void Encode(Video video)
        {
            Console.WriteLine("Encoding Video...");
            Thread.Sleep(3000);
        }

        protected virtual void OnVideoEncoded(Video video)
        {
            if(VideoEncoded != null)
            {
                VideoEncoded(this, new VideoEventArgs() { Video = video });
            }
        }
    }

    public class VideoEncoderHost
    {
        public void Start()
        {
            var video = new Video() { Title = "Video 1" };
            var videoEncoder = new VideoEncoder(); //publisher
            var mailService = new MailService(); //Subscriber
            var messageService = new MessageService(); //Subscriber

            videoEncoder.VideoEncoded += mailService.OnVideoEncoded;
            videoEncoder.VideoEncoded += messageService.OnVideoEncoded;

            videoEncoder.Encode(video);

        }
    }
}
