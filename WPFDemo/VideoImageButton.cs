using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDemo
{
    public class VideoImageButton:ImageButton
    {
        private readonly Image[] _images;

        private bool _played = false;

        public VideoImageButton(Image[] images)
        {
            _images = images;
        }

        public void ChangePlayed(bool played)
        {
            _played = played;
            Refresh();
        }

        private void Refresh()
        {
            var index = 0;
            if (!IsMouseOver && !_played)
            {
                index = 0;
            }
            else if (!IsMouseOver && _played)
            {
                index = 2;
            }
            else if (IsMouseOver && !_played)
            {
                index = 1;
            }
            else
            {
                index = 3;
            }

            var image = _images[index];

            Content = image;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Refresh();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Refresh();
        }
    }
}
