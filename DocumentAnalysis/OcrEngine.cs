using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tesseract;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DocumentAnalysis
{
    class OcrEngine
    {
        public static string hocr(Bitmap source, int x, int y, int width, int height)  // 특정 좌표 지역을 받아서 그 부분만 판독하는 함수    
        {
            string htext;

            // var PrescriptionImage = CropedPrescription;                                 
            using (var engine = new TesseractEngine(@"C:\Program Files\Tesseract-OCR\tessdata\", "kor", EngineMode.Default))
            {
                //    using (var img = Pix.LoadFromFile(PrescriptionImage)
                //    {
                var roi = new Rect(x, y, width, height); // region of interest 좌표를 생성하고

                using (var page = engine.Process(source, roi, PageSegMode.Auto))
                {
                    htext = page.GetHOCRText(3);
                    System.IO.File.WriteAllText(@"C:\Program Files\PLOCR\textrecognition.html", htext);  // 인식한 글자를 html 형식으로 저장한다.
                    //  Console.WriteLine(htext);
                    //   Console.Read();
                }
                //    }
                return htext;
            }           
        }


        public static string ocr(Bitmap CropedPrescription, int x, int y, int width, int height)
        {
            string text;

            // var PrescriptionImage = CropedPrescription;
            using (var engine = new TesseractEngine(@"C:\Program Files\Tesseract-OCR\tessdata\", "kor", EngineMode.Default))
            {
                engine.SetVariable("tessedit_char_whitelist", "0123456789-."); // 숫자와 . - 만 인식하도록 설정             

                var roi = new Rect(x, y, width, height); // region of interest 좌표를 생성하고
                //     using (var img = Pix.LoadFromFile(PrescriptionImage))
                //      {
                using (var page = engine.Process(CropedPrescription, roi, PageSegMode.SingleLine))
                {
                    text = page.GetText();
                    System.IO.File.WriteAllText(@"C:\Program Files\PLOCR\textrecognition.html", text);  // 인식한 글자를 html 형식으로 저장한다.

                //    text = TextProcess.RemoveWhiteSpace(text);


                    //      Console.WriteLine("인식한 문자: \n{0}\n", text);
                    //   Console.Read();
                }
                //         }
            }

            return text;
        }
    }
}
