using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;

namespace WpfApplication1
{
    public class Picture : INotifyPropertyChanged
    {

        private readonly BitmapImage frontImage;

        private readonly string name;


        public Picture(string name, BitmapImage frontImage)
        {
            this.name = name;
            this.frontImage = frontImage;


            this.Status = pictureState.Covered;
        }

        public string Name
        {
            get { return name; }
        }

        public pictureState Status { get; set; }

        public Brush ActiveImage
        {
            get
            {

                if (this.Status == pictureState.Uncovered)
                {
                    var brush = new ImageBrush(frontImage);
                    brush.Stretch = Stretch.Uniform;
                    return brush;
                }

                if (this.Status == pictureState.Matched)
                {
                    return new SolidColorBrush(Colors.LightBlue);
                }

                throw new InvalidOperationException("Invalid picture State.");
            }
        }

        public void Uncover()
        {
            this.Status = pictureState.Uncovered;
            RaiseNotifyChanged("ActiveImage");
        }

        public void Cover()
        {
            this.Status = pictureState.Covered;
            RaiseNotifyChanged("ActiveImage");
        }

        public void Match()
        {
            this.Status = pictureState.Matched;
            RaiseNotifyChanged("ActiveImage");
        }

        private void RaiseNotifyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum pictureState
    {
        Covered,
        Uncovered,
        Matched
    }
}
