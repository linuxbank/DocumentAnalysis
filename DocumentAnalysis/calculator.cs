using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace DocumentAnalysis
{
    class calculator
    {    
        public static int NewPointX(int x3old, int x1new, int x2new)  // old 는 기준처방전 new 는 실제처방전, 1 과 2는 기준좌표 두점을 말하고, 모르는 제3의 new 를 찾는 함수
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////          

            int x1old = int.Parse(ini.GetIniValue("스캔으로 찾은 좌표1번", "X"));
            int x2old = int.Parse(ini.GetIniValue("스캔으로 찾은 좌표2번", "X"));

            double x3new = 0;

            if ((x3old - x1old) < 0)
            {
                x3new = x1new - (Math.Abs((x3old - x1old)) * Math.Abs((x2new - x1new)) / Math.Abs((x2old - x1old)));  // x1old 는 기준처방의 1번좌표, x2old 는 기준처방의 2번좌표, x3old 는 사용자가 지정한 기준처방의 좌표
            }                                                                                                      // x1new 는 실제처방의 1번좌표, x2new 는 실제처방의 2번좌표, x3new 는 찾으려고 하는 좌표  
            else if ((x3old - x1old) > 0)
            {
                x3new = Math.Abs((x3old - x1old)) * Math.Abs((x2new - x1new)) / Math.Abs((x2old - x1old)) + x1new;
            }
            return (int)x3new;  // 실제처방전의 원하는 좌표 x 값
        }

        public static int NewPointY(int y3old, int y1new, int y2new)  // old 는 기준처방전 new 는 실제처방전, 1 과 2는 기준좌표 두점을 말하고, 모르는 제3의 new 를 찾는 함수
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////          

            int y1old = int.Parse(ini.GetIniValue("스캔으로 찾은 좌표1번", "Y"));
            int y2old = int.Parse(ini.GetIniValue("스캔으로 찾은 좌표2번", "Y"));

            double y3new = 0;

            if ((y3old - y1old) < 0)
            {
                y3new = y1new - (Math.Abs((y3old - y1old)) * Math.Abs((y2new - y1new)) / Math.Abs((y2old - y1old)));  // x1old 는 기준처방의 1번좌표, x2old 는 기준처방의 2번좌표, x3old 는 사용자가 지정한 기준처방의 좌표
            }                                                                                                      // x1new 는 실제처방의 1번좌표, x2new 는 실제처방의 2번좌표, x3new 는 찾으려고 하는 좌표  
            else if ((y3old - y1old) > 0)
            {
                y3new = Math.Abs((y3old - y1old)) * Math.Abs((y2new - y1new)) / Math.Abs((y2old - y1old)) + y1new;
            }
            return (int)y3new;
        }

        public static float quadrantX(int quad) //  사분면 선택을 받아 각 사분면에 해당하는 X의 기준좌표값을 조정하기 위한 조정값을 반환한다.
        {
            int order;
            float adjustment = 0;

            order = quad - 1;

            Bitmap img = new Bitmap(@"C:\Program Files\PLOCR\prescription.png");
            var width = img.Width;            

            switch (order)
            {
                case 0:
                    adjustment = width / 2;

                    break;
                case 1:
                    adjustment = 0;

                    break;
                case 2:
                    adjustment = 0;

                    break;
                case 3:
                    adjustment = width / 2;

                    break;
                //case 4:
                //    adjustment = width;

                //    break;
            }

            return adjustment;
        }

        public static float quadrantY(int quad) //  사분면 선택을 받아 각 사분면에 해당하는 Y의 기준좌표값을 조정하기 위한 조정값을 반환한다.
        {
            int order;
            float adjustment = 0;

            order = quad - 1;

            Bitmap img = new Bitmap(@"C:\Program Files\PLOCR\prescription.png");
            var height = img.Height;   

            switch (order)
            {
                case 0:
                    adjustment = 0;

                    break;
                case 1:
                    adjustment = 0;

                    break;
                case 2:
                    adjustment = height / 2;

                    break;
                case 3:
                    adjustment = height / 2;

                    break;
                //case 4:
                //    adjustment = height;

                //    break;
            }
            
            return adjustment;
        }

        public static string CreateFileCheck(string path)  // 파일명 중복체크해서 중복이면 파일명+(번호)를 추가해서 저장한다.
        {
            string FullPath = path;
            string Extension = Path.GetExtension(path);
            string FileName = FullPath.Replace(Extension, "");
            int Count = 1;
            while (true)
            {
                if (File.Exists(FullPath))
                {
                    FullPath = FileName + "(" + Count.ToString() + ")" + Extension;
                    Count++;
                    continue;
                }
                return FullPath;
            }
        }

        public static Bitmap imgProcess(Bitmap source, string callFunction)  // 지정된 이미지 처리를 한다.
        {
            if (callFunction == "색상 필터")
            {
                Global.source = imageProcess.colorFilter(source);
            }
            else if (callFunction == "색상 반전")
            {
                Global.source = imageProcess.invert(source);
            }
            else if (callFunction == "선 굵게")
            {
                Global.source = imageProcess.thick(source);
            }
            else if (callFunction == "선 가늘게")
            {
                Global.source = imageProcess.thin(source);
            }
            else if (callFunction == "밝게")
            {
                Global.source = imageProcess.bright(source);
            }
            else if (callFunction == "어둡게")
            {
                Global.source = imageProcess.dark(source);
            }
            else if (callFunction == "기울어짐 바로잡기")
            {
                Global.source = imageProcess.skew(source);
            }
            else if (callFunction == "원본")
            {
                string path = @"C:\Program Files\PLOCR\prescription.png";
                Global.source = (Bitmap)Bitmap.FromFile(path);
            }
            else
            {
            }

            return Global.source;
        }

        public static Bitmap sequence(Bitmap source)        // 이미지 처리 몇단계까지 해야하나 확인하고 처리
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            string exeNo1 = ini.GetIniValue("실행 순서", "1단계");
            string exeNo2 = ini.GetIniValue("실행 순서", "2단계");
            string exeNo3 = ini.GetIniValue("실행 순서", "3단계");
            string exeNo4 = ini.GetIniValue("실행 순서", "4단계");
            string exeNo5 = ini.GetIniValue("실행 순서", "5단계");
            string exeNo6 = ini.GetIniValue("실행 순서", "6단계");
            string exeNo7 = ini.GetIniValue("실행 순서", "7단계");

            if (exeNo7 == "끝")
            {
                source = imgProcess(source, exeNo1);                
                source = imgProcess(source, exeNo2);
                source = imgProcess(source, exeNo3);
                source = imgProcess(source, exeNo4);
                source = imgProcess(source, exeNo5);
                source = imgProcess(source, exeNo6);
            }
            else if (exeNo6 == "끝")
            {
                source = imgProcess(source, exeNo1);
                source = imgProcess(source, exeNo2);
                source = imgProcess(source, exeNo3);
                source = imgProcess(source, exeNo4);
                source = imgProcess(source, exeNo5);
            }
            else if (exeNo5 == "끝")
            {
                source = imgProcess(source, exeNo1);
                source = imgProcess(source, exeNo2);
                source = imgProcess(source, exeNo3);
                source = imgProcess(source, exeNo4);
            }
            else if (exeNo4 == "끝")
            {
                source = imgProcess(source, exeNo1);
                source = imgProcess(source, exeNo2);
                source = imgProcess(source, exeNo3);
            }
            else if (exeNo3 == "끝")
            {
                source = imgProcess(source, exeNo1);
                source = imgProcess(source, exeNo2);
            }
            else if (exeNo2 == "끝")
            {
                source = imgProcess(source, exeNo1);
            }
            else if (exeNo1 == "끝")
            {
            }

            return source;
        }

        public static Bitmap originalcheck(Bitmap source)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 ///////////////////////////////////////////////////////// 

            string exeNo1 = ini.GetIniValue("실행 순서", "1단계");
            string exeNo2 = ini.GetIniValue("실행 순서", "2단계");
            string exeNo3 = ini.GetIniValue("실행 순서", "3단계");
            string exeNo4 = ini.GetIniValue("실행 순서", "4단계");
            string exeNo5 = ini.GetIniValue("실행 순서", "5단계");
            string exeNo6 = ini.GetIniValue("실행 순서", "6단계");
            string exeNo7 = ini.GetIniValue("실행 순서", "7단계");

            if (exeNo1 == "원본" ||
               exeNo2 == "원본" ||
               exeNo3 == "원본" ||
               exeNo4 == "원본" ||
               exeNo5 == "원본" ||
               exeNo6 == "원본" ||
               exeNo7 == "원본")
            {
                string path = @"C:\Program Files\PLOCR\prescription.png";
                source = (Bitmap)Bitmap.FromFile(path);
            }
            return source;
        }
    }
}
