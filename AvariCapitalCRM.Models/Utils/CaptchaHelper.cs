using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AvariCapitalCRM.Models.Utils
{
    public class CaptchaHelper
    {
        static Random rnd = new Random();
        static Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Brown, Color.DarkBlue };
        static string[] font = { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" };


        public static void Create(Bitmap bmp, string chkCode, ref Graphics g, int num = 4)
        {
            if (string.IsNullOrEmpty(chkCode))
                return;

            Color clr = color[rnd.Next(color.Length)];
            Font ft = new Font("Times New Roman", 16);
            g.Clear(Color.White);
            //画噪线 
            for (int i = 0; i < 3; i++)
            {
                int x1 = rnd.Next(50);
                int y1 = rnd.Next(30);
                int x2 = rnd.Next(80);
                int y2 = rnd.Next(20);
                clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            //画验证码字符串 
            for (int i = 0; i < chkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                ft = new Font(fnt, 17);
                clr = color[rnd.Next(color.Length)];
                g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 15+8 , (float)2);
            }
            //画噪点                 
            for (int i = 0; i < 100; i++)
            {
                int x = rnd.Next(bmp.Width);
                int y = rnd.Next(bmp.Height);
                clr = color[rnd.Next(color.Length)];
                bmp.SetPixel(x, y, clr);
            }

        }

        public static string Create(int num = 4)
        {
            string chkCode = string.Empty;
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };

            //生成验证码字符串 
            for (int i = 0; i < num; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            return chkCode;
        }

        /// <summary>
        /// 随机生成6位短信验证码   
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string CreateCode(int num = 6)
        {
            if (num < 4) num = 4;
            string code = string.Empty;
            Random rand = new Random();
            for (int i = 0; i < num; i++)
            {
                code += rand.Next(0, 10);
            }
            return code;
        }

    }
}
