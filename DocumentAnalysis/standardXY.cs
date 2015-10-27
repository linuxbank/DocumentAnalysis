using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace DocumentAnalysis
{
    class standardXY
    {
        public static void startXY(string HtmlText, string standardText, out int standardX, out int standardY, out int standardWidth, out int standardHeight)   // 좌표1,2번의 문자를 주고, 해당 문자의 x,y 좌표를 반환한다.
        {
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                                    
            int ZeroIndex = HtmlText.IndexOf(standardText);  // 주어진 문자열로 기준이 되는 문자열 인덱스를 찾고,      
            if (ZeroIndex == -1)
            {
                MessageBox.Show("해당 좌표를 찾지 못했습니다.\n 다른 좌표값으로 찾아보세요.");
            }

            if (ZeroIndex != -1)
            {
                string ZeroRange = HtmlText.Substring(ZeroIndex - 30, 30); // 기준 문자열부터 앞쪽으로 30 인덱스 부분부터 기준까지의 문자열을 선택하고,   


                char[] delimiterChars = { ' ' };
                string[] ZeroDiv = ZeroRange.Split(delimiterChars);  // 공백을 기준으로 문자열을 분리하고                                   



                for (int ct = 0; ct < ZeroDiv.Length; ct++)
                {
                    if (Regex.IsMatch(ZeroDiv[ct], @"^-?\d+$"))  // 숫자인지 아닌지 판단
                    {
                        x1 = int.Parse(Regex.Replace(ZeroDiv[ct], "[^0-9.-]", ""));      // 숫자만 추출하여, x좌표 문자형 숫자를 정수형으로 형변환
                        y1 = int.Parse(Regex.Replace(ZeroDiv[ct + 1], "[^0-9.-]", ""));    // 숫자만 추출하여, y좌표 문자형 숫자를 정수형으로 형변환
                        x2 = int.Parse(Regex.Replace(ZeroDiv[ct + 2], "[^0-9.-]", ""));
                        y2 = int.Parse(Regex.Replace(ZeroDiv[ct + 3], "[^0-9.-]", ""));

                        //      Console.WriteLine(x1);
                        //      Console.WriteLine(y1);
                        //      Console.WriteLine(x2);
                        //      Console.WriteLine(y2);

                        ct = ZeroDiv.Length;  // 숫자요소를 가진 배열을 찾으면 루프 탈출                        
                    }
                }
                MessageBox.Show("좌표를 찾았습니다!");
            }

            standardWidth = x2 - x1;
            standardHeight = y2 - y1;
            standardX = x1;
            standardY = y1;                       

            //    Console.WriteLine(XYzeroWidth);
        }

        public static void calibration(int quad)
        {
            ///////////// ini 객체 생성 시작 /////////////////////////////////////////////////////
            //현재 프로그램이 실행되고 있는정보 가져오기: 디버깅 모드라면 bin/debug/프로그램명.exe
            FileInfo exefileinfo = new FileInfo(@"C:\Program Files\PLOCR\PLOCR.exe");
            string pathini = exefileinfo.Directory.FullName.ToString();  //프로그램 실행되고 있는데 path 가져오기
            string fileName = @"\PLOCRconfig.ini";  // 환경설정 파일명
            string filePath = pathini + fileName;   //ini 파일 경로
            DocumentAnalysis.IniUtil ini = new DocumentAnalysis.IniUtil(filePath);   // 만들어 놓았던 iniUtil 객체 생성(생성자 인자로 파일경로 정보 넘겨줌)
            //////////// ini 객체 생성 끝 /////////////////////////////////////////////////////////

            int order;
            int sX1 = 0, sX2 = 0, sY1 = 0, sY2 = 0;                         

            int specialX = int.Parse(ini.GetIniValue("특정좌표영역", "X"));
            int specialY = int.Parse(ini.GetIniValue("특정좌표영역", "Y"));

            order = quad - 1;

            string path = @"C:\Program Files\PLOCR\prescription.png";
            Bitmap source = (Bitmap)Bitmap.FromFile(path);
                                                
            switch (order)
            {
                case 0:                             //1사분면일 경우
                    sX1 = (source.Width) / 2 + int.Parse(ini.GetIniValue("좌표1번", "X"));
                    sY1 = int.Parse(ini.GetIniValue("좌표1번", "Y"));
                    sX2 = (source.Width) / 2 + int.Parse(ini.GetIniValue("좌표2번", "X"));
                    sY2 = int.Parse(ini.GetIniValue("좌표1번", "Y"));

                    break;
                case 1:                             //2사분면일 경우
                    sX1 = int.Parse(ini.GetIniValue("좌표1번", "X"));
                    sY1 = int.Parse(ini.GetIniValue("좌표1번", "Y"));
                    sX2 = int.Parse(ini.GetIniValue("좌표2번", "X"));
                    sY2 = int.Parse(ini.GetIniValue("좌표2번", "Y"));

                    break;
                case 2:                             //3사분면일 경우
                    sX1 = int.Parse(ini.GetIniValue("좌표1번", "X"));
                    sY1 = (source.Height) / 2 + int.Parse(ini.GetIniValue("좌표1번", "Y"));
                    sX2 = int.Parse(ini.GetIniValue("좌표1번", "X"));
                    sY2 = (source.Height) / 2 + int.Parse(ini.GetIniValue("좌표2번", "Y"));

                    break;
                case 3:                             //4사분면일 경우
                    sX1 = (source.Width) / 2 + int.Parse(ini.GetIniValue("좌표1번", "X"));
                    sY1 = (source.Height) / 2 + int.Parse(ini.GetIniValue("좌표1번", "Y"));
                    sX2 = (source.Width) / 2 + int.Parse(ini.GetIniValue("좌표2번", "X"));
                    sY2= (source.Height) / 2 + int.Parse(ini.GetIniValue("좌표2번", "Y"));

                    break;
                case 4:                             //특정영역일 경우
                    sX1 = specialX + int.Parse(ini.GetIniValue("좌표1번", "X"));
                    sY1 = specialY + int.Parse(ini.GetIniValue("좌표1번", "Y"));
                    sX2 = specialX + int.Parse(ini.GetIniValue("좌표2번", "X"));
                    sY2 = specialY + int.Parse(ini.GetIniValue("좌표2번", "Y"));

                    break;            
            }    
                
           
        }

       
    }
}
