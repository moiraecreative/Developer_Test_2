using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace Moirae.Utility
{
    public static class ImageBackgroundRemover
    {

        public static string RemoveWhiteBackground(string imageUrl, int threshold = 200)
        {
            // Download the image from the URL
            using (var webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(imageUrl);

                // Load the image into a Bitmap object
                using (var ms = new MemoryStream(imageBytes))
                using (var image = new Bitmap(ms))
                {
                    // Check if the image already has transparency
                    if (HasTransparency(image))
                    {
                        // Image already has transparency, return the original URL
                        return imageUrl;
                    }

                    // Create a new Bitmap with transparent background
                    var resultImage = new Bitmap(image.Width, image.Height);
                    resultImage.MakeTransparent();

                    // Iterate through each pixel of the image
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            Color pixelColor = image.GetPixel(x, y);

                            // Check if the pixel is white (or close to white) based on the threshold
                            if (IsWhiteOrClose(pixelColor, threshold))
                            {
                                // Set the pixel to transparent in the result image
                                resultImage.SetPixel(x, y, Color.Transparent);
                            }
                            else
                            {
                                // Copy the pixel to the result image
                                resultImage.SetPixel(x, y, pixelColor);
                            }
                        }
                    }

                    // Generate the new file path for the modified image
                    string modifiedFileName = Path.GetFileName(imageUrl);

                    // Get the current directory path
                    string directoryPath = Directory.GetCurrentDirectory();
                    string fullPath = Path.Combine(directoryPath, "wwwroot", "Images");

                    // Combine the directory path and the modified filename to get the new URL
                    string modifiedUrl = Path.Combine(fullPath, modifiedFileName);

                    // Save the modified image to the new URL
                    resultImage.Save(modifiedUrl);

                    // Return the new URL including the file name
                    return "/Images/" + modifiedFileName;
                }
            }
        }


        private static bool HasTransparency(Bitmap image)
        {
            // Check if any pixel in the image has an alpha component less than 255
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    if (pixelColor.A < 255)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsWhiteOrClose(Color color, int threshold)
        {

            // Check if the color is close to white based on RGB values
            return (color.R > threshold && color.G > threshold && color.B > threshold);
        }
    }
}
