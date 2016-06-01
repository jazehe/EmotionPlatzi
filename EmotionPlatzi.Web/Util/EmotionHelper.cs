using EmotionPlatzi.Web.Models;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;
using System.Web.Services.Description;
using System.Reflection;
using System.Threading.Tasks;

namespace EmotionPlatzi.Web.Util
{
    public class EmotionHelper
    {
        public EmotionServiceClient emoClient;

        public EmotionHelper(string key)
        {
            emoClient = new EmotionServiceClient(key);
        }

        public async Task <EmoPicture> DetectAndExtractFacesAsinc(Stream imageStream)
        {
            Emotion[] emotions = await emoClient.RecognizeAsync(imageStream);

            var emoPicture = new EmoPicture();

            emoPicture.Faces = DtectAndExtractFaces(emotions, emoPicture);
            return emoPicture;

        }

        private ObservableCollection<EmoFace> DtectAndExtractFaces(Emotion[] emotions, EmoPicture emoPicture)
        {
            var ListaFaces = new ObservableCollection<EmoFace>();//es de este tipo para que Entity Framework se dispare y ejecute la 
            //acción de borrado en cascada, etc.
            foreach (var emotion in emotions)
            {
                var emoFace = new EmoFace()
                {
                    X = emotion.FaceRectangle.Left,
                    Y = emotion.FaceRectangle.Top,
                    Width = emotion.FaceRectangle.Width,
                    Height = emotion.FaceRectangle.Height,
                    Picture = emoPicture
                };

                emoFace.Emotions = ProcessEmotions(emotion.Scores, emoFace);
                ListaFaces.Add(emoFace);

            }
            return ListaFaces;
        }

        private ObservableCollection<EmoEmotion> ProcessEmotions(Scores scores, EmoFace emoFace)
        {
            var emotionList = new ObservableCollection<EmoEmotion>();

            var properties = scores.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //var filterProperties = properties.Where(p => p.PropertyType == typeof(float));
            //Usando LinQ
            var filterProperties = from p in properties
                                   where p.PropertyType == typeof(float)
                                   select p;

            var emoType = EmoEmotionEnum.Undetermined;
            foreach (var prop in filterProperties)
            {
                if (!Enum.TryParse<EmoEmotionEnum>(prop.Name, out emoType))
                    emoType = EmoEmotionEnum.Undetermined;

                var emoEmotion = new EmoEmotion();
                emoEmotion.Score = (float)prop.GetValue(scores);
                emoEmotion.EmotionType = emoType;
                emoEmotion.Face = emoFace;

                emotionList.Add(emoEmotion);
            }
            return emotionList;
        }
    }
}