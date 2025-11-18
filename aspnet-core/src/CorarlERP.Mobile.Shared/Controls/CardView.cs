using Xamarin.Forms;

namespace CorarlERP.Controls
{
    public class CardView : Frame
    {
        public CardView()
        {
            Padding = 0;

            if (Device.RuntimePlatform == Device.iOS)
            {
                HasShadow = false;
                OutlineColor = Color.Gray;
            }
        }
    }
}

