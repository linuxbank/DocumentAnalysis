using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using System.IO;
using AForge;
using AForge.Imaging;

namespace DocumentAnalysis
{
    class imageProcess
    {
        public static Bitmap Clone(Bitmap source, PixelFormat format)   // 이미지 픽셀 포맷 조정하기 위한 클론 함수
        {
            //// copy image if pixel format is the same  //
            //if (source.PixelFormat == format)          // 이 부분을 주석해제하면
            //{                                          // 포맷이 같으면 처리하지 않으므로
            //    return source;                         // 반복 이미지 처리가 안된다.
            //}                                          // 버그 잡느라 한참 헤멨으니 주석 풀지 말것 :-)

            int width = source.Width;
            int height = source.Height;

            // create new image with desired pixel format
            Bitmap bitmap = new Bitmap(width, height, format);

            // draw source image on the new one using Graphics
            Graphics g = Graphics.FromImage(bitmap);
            g.DrawImage(source, 0, 0, width, height);
            g.Dispose();

            return bitmap;
        }

        public static Bitmap RGBfilter(Bitmap source)     // rgb 필터
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
          DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////  

            int redMin = int.Parse(ini.GetIniValue("색상필터값", "RedMin"));
            int redMax = int.Parse(ini.GetIniValue("색상필터값", "RedMax"));
            int greenMin = int.Parse(ini.GetIniValue("색상필터값", "GreenMin"));
            int greenMax = int.Parse(ini.GetIniValue("색상필터값", "GreenMax"));
            int blueMin = int.Parse(ini.GetIniValue("색상필터값", "BlueMin"));
            int blueMax = int.Parse(ini.GetIniValue("색상필터값", "BlueMax"));

            // create filter
            ColorFiltering filter = new ColorFiltering();
            // set color ranges to keep
            filter.Red = new IntRange(redMin, redMax);
            filter.Green = new IntRange(greenMin, greenMax);
            filter.Blue = new IntRange(blueMin, blueMax);
            Bitmap processedImage = filter.Apply(source);

            return processedImage;
        }

        public static Bitmap colorFilter(Bitmap source)     // rgb 필터 적용
        {
            source = RGBfilter(source);

            return source;
        }

        public static Bitmap invert(Bitmap source)      // 반전
        {

            Bitmap tmp = (Bitmap)source;        // 중요! 한번 이미지 처리가 끝난 비트맵 source 는 clone 함수로 보내기 전에 다시 한번 (Bitmap) 처리 해줘야함, 이유는 잘 모르겠음            
            // convert to 24 bits per pixel
            source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);
            // delete old image
            tmp.Dispose();

            Invert invertfilter = new Invert();
            invertfilter.ApplyInPlace(source);

            return source;
        }

        public static Bitmap dark(Bitmap source)        // 어둡게
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
           DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            int order = int.Parse(ini.GetIniValue("밝기값", "어둡게횟수"));

            for (int i = 0; i < order; i++)
            {
                // create filter
                BrightnessCorrection filter = new BrightnessCorrection(-10);
                // apply the filter
                filter.ApplyInPlace(source);
            }

            return source;
        }

        public static Bitmap bright(Bitmap source)        // 밝게
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
           DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            int order = int.Parse(ini.GetIniValue("밝기값", "밝게횟수"));

            for (int i = 0; i < order; i++)
            {
                // create filter
                BrightnessCorrection filter = new BrightnessCorrection(+10);
                // apply the filter
                filter.ApplyInPlace(source);
            }

            return source;
        }

        public static Bitmap thin(Bitmap source)        // 선 가늘게
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
           DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            int order = int.Parse(ini.GetIniValue("선굵기값", "가늘게횟수"));

            for (int i = 0; i < order; i++)
            {
                Bitmap tmp = (Bitmap)source;        // 중요! 한번 이미지 처리가 끝난 비트맵 source 는 clone 함수로 보내기 전에 다시 한번 (Bitmap) 처리 해줘야함, 이유는 잘 모르겠음
                // convert to 24 bits per pixel
                source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);
                // delete old image
                tmp.Dispose();

                Dilatation filter = new Dilatation();
                filter.ApplyInPlace(source);
            }

            return source;
        }

        public static Bitmap thick(Bitmap source)        // 선 굵게, 24비트로 넣어줘야함
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
           DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            int order = int.Parse(ini.GetIniValue("선굵기값", "굵게횟수"));

            for (int i = 0; i < order; i++)
            {
                Bitmap tmp = (Bitmap)source;        // 중요! 한번 이미지 처리가 끝난 비트맵 source 는 clone 함수로 보내기 전에 다시 한번 (Bitmap) 처리 해줘야함, 이유는 잘 모르겠음
                // convert to 24 bits per pixel
                source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);
                // delete old image
                tmp.Dispose();

                Erosion filter = new Erosion();
                filter.ApplyInPlace(source); ;
            }

            return source;
        }

        public static Bitmap skew(Bitmap source)        // 기울어짐 바로잡기
        {
            // create grayscale filter (BT709)
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);       // 8비트 grayscale 로 바꾸고
            // apply the filter
            Bitmap grayImage = filter.Apply(source);

            // create instance of skew checker
            DocumentSkewChecker skewChecker = new DocumentSkewChecker();        // 8비트 grayscale 로 넣어줘야 함
            //    // get documents skew angle
            double angle = skewChecker.GetSkewAngle(grayImage);  // 기울어진 각도를 얻고

            Bitmap tmp = source;
            // convert to 24 bits per pixel
            source = imageProcess.Clone(tmp, PixelFormat.Format24bppRgb);       // 로테이션 전에 24비트로 바꿔주고
            // delete old image
            tmp.Dispose();

            // create rotation filter
            RotateBilinear rotationFilter = new RotateBilinear(-angle);
            rotationFilter.FillColor = Color.White;
            // rotate image applying the filter
            Bitmap rotatedImage = rotationFilter.Apply(source);  // 원래 이미지를 가져다가 각도만큼 돌리고(원래 이미지는 24비트로 넣어줘야함)           

            return rotatedImage;
        }
    }
}
