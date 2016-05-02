using UnityEngine;
using System.Collections;
using UnityEngine.WSA;
using NotificationsExtensions.TileContent;

namespace MarkerMetro.Unity.WinShared.Example
{

    public class WSATile : MonoBehaviour
    {
        public void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                UpdateLiveTile();
            }
        }

        public void UpdateLiveTile()
        {
            var tile = UnityEngine.WSA.Tile.main;
            

            try
            {
                //***********************************************
                //Adjust these parameters with appropriate values
                string mediumImagePath = "/Assets/Logo.png";
                string wideImagePath = "/Assets/WideLogo.png";

                string mediumText1 = "This"; // **Don't forget to localise**
                string mediumText2 = "Is";
                string mediumText3 = "From";
                string mediumText4 = "Unity";

                string mediumData1 = "99"; // **This data should come from a game class**
                string mediumData2 = "1234";
                string mediumData3 = System.DateTime.Now.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                string mediumData4 = "7";

                string wideText1 = mediumText1; // **These can be made different if more detail is wanted for wide tile **
                string wideText2 = mediumText2;
                string wideText3 = mediumText3;
                string wideText4 = mediumText4;

                string wideData1 = mediumData1;
                string wideData2 = mediumData2;
                string wideData3 = mediumData3;
                string wideData4 = mediumData4;
                //
                //***********************************************


                // Using NotificationsExtensions library to build up tile content
                // Choose the tile templates to use for 150x150, 310x150 and 310x310 sizes
                // Check this link for the tile catalog for Windows Store and Windows Phone:
                // https://msdn.microsoft.com/en-us/library/windows/apps/hh761491.aspx

                var mediumTile = TileContentFactory.CreateTileSquare150x150PeekImageAndText01();
                var wideTile = TileContentFactory.CreateTileWide310x150PeekImage02();
                var largeTile = TileContentFactory.CreateTileSquare310x310SmallImageAndText01();

                // Set the branding value for the tiles (weather or not to display the app name)
                mediumTile.Branding = TileBranding.None;
                wideTile.Branding = TileBranding.None;
                largeTile.Branding = TileBranding.Name;

                //Set the tile template images
                mediumTile.Image.Src = mediumImagePath;
                wideTile.Image.Src = wideImagePath;
                largeTile.Image.Src = mediumImagePath;

                //Set the tile template text values
                mediumTile.TextHeading.Text = string.Format("{0}: {1}", mediumText1, mediumData1);
                mediumTile.TextBody1.Text = string.Format("{0}: {1}", mediumText2, mediumData2);
                mediumTile.TextBody2.Text = string.Format("{0}: {1}", mediumText3, mediumData3);
                mediumTile.TextBody3.Text = string.Format("{0}: {1}", mediumText4, mediumData4);

                wideTile.TextHeading.Text = string.Format("{0}: {1}", wideText1, wideData1);
                wideTile.TextBody1.Text = string.Format("{0}: {1}", wideText2, wideData2);
                wideTile.TextBody2.Text = string.Format("{0}: {1}", wideText3, wideData3);
                wideTile.TextBody3.Text = string.Format("{0}: {1}", wideText4, wideData4);
                wideTile.TextBody4.Text = string.Empty;

                largeTile.TextHeading.Text = string.Format("{0}: {1}", wideText1, wideData1);
                largeTile.TextBodyWrap.Text = string.Format("{0}: {1} - {2}: {3}", wideText2, wideData2, wideText4, wideData4);
                largeTile.TextBody.Text = string.Format("{0}: {1}", wideText3, wideData3);

                //This merges the tile templates together to create the single xml for all of them
                wideTile.Square150x150Content = mediumTile;
                largeTile.Wide310x150Content = wideTile;


                //This is dumb... Will fix with an updated library soon
                var xml = largeTile.GetXml();
                tile.Update(xml.ToString());

                // Reading the actual Xml to understand what is happening here.
                Debug.Log(xml.ToString());
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

}
