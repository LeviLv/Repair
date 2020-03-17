using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Repair.Services
{
    public static class ImageHelper
    {
        /// <summary>
        /// 取小写文件名后缀
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>返回小写后缀，不带“.”</returns>
        public static string GetFileExt(string name)
        {
            return name.Split(".").Last().ToLower();
        }

        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="fileExt">文件扩展名，不含“.”</param>
        public static bool IsImage(string fileExt)
        {
            ArrayList al = new ArrayList { "bmp", "jpeg", "jpg", "gif", "png", "ico" };
            return al.Contains(fileExt);
        }

        /// <summary>
        /// 检查是否允许文件
        /// </summary>
        /// <param name="fileExt">文件后缀</param>
        /// <param name="allowExt">允许文件数组</param>
        public static bool CheckFileExt(string fileExt, string[] allowExt)
        {
            return allowExt.Any(t => t == fileExt);
        }


        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="original">图片对象</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void ThumbImg(Image original, string newFileName, int maxWidth, int maxHeight)
        {
            Size newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);
            using (Image displayImage = new Bitmap(original, newSize))
            {
                try
                {
                    displayImage.Save(newFileName, original.RawFormat);
                }
                finally
                {
                    original.Dispose();
                }
            }
        }

        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void ThumbImg(string fileName, string newFileName, int maxWidth, int maxHeight)
        {
            Console.WriteLine(fileName);
            byte[] imageBytes = File.ReadAllBytes(fileName);
            Console.WriteLine(imageBytes);
            using var ms = new MemoryStream(imageBytes);
            ms.Position = 0;
            Console.WriteLine(ms.Length);
            Image img = Image.FromStream(ms);
            ThumbImg(img, newFileName, maxWidth, maxHeight);
        }


        /// <summary>
        /// 计算新尺寸
        /// </summary>
        /// <param name="width">原始宽度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新宽度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            if (maxWidth <= 0)
                maxWidth = width;
            if (maxHeight <= 0)
                maxHeight = height;
            decimal MAX_WIDTH = maxWidth;
            decimal MAX_HEIGHT = maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;
            decimal originalWidth = width;
            decimal originalHeight = height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            return new Size(newWidth, newHeight);
        }


        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
        {
            string ext = GetFileExt(name);
            switch (ext)
            {
                case "ico":
                    return ImageFormat.Icon;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }
    }
}
