using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionPlatzi.Web.Models
{
    public class EmoFace
    {
        public int Id { get; set; }
        public int EmoPictureId { get; set; }

        #region region//se utiliza para colapsar parte del código que no esten dentro de una sentencia, if, etc
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        #endregion

        public virtual EmoPicture Picture { get; set; }
        public virtual ObservableCollection<EmoEmotion> Emotions { get; set; }
    }

}
