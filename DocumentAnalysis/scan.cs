using Saraff.Twain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace DocumentAnalysis
{
    class scan
    {
        Twain32 PLOCRtwain = new Twain32();

        //public static bool _isDataSourceOpen = false;

        //public static void _twain_TwainStateChanged(object sender, Twain32.TwainStateEventArgs e)
        //{
        //    try
        //    {
        //        // ...
        //        _isDataSourceOpen = (e.TwainState & Twain32.TwainStateFlag.DSOpen) != 0;
        //        // ...
        //        MessageBox.Show("twain 상태가 변경되었습니다.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // ...
        //    }
        //}

        //public static void timer1_Tick(object sender, EventArgs e)
        //{
        //    using (Twain32 PLOCRtwain = new Twain32())
        //    {
        //        try
        //        {
        //            // MessageBox.Show("타이머 작동 확인용 창");
        //            //     if (_isDataSourceOpen)
        //            //     {
        //            // ...
        //            var _isFeederLoaded = (bool)PLOCRtwain.GetCurrentCap(TwCap.FeederLoaded);
        //            // ...
        //            MessageBox.Show("처방전이 올려졌습니다.");
        //            //      }
        //        }
        //        catch (Exception ex)
        //        {
        //            // ...
        //        }
        //    }
        //}


        public static Bitmap scanImage()
        {
            Bitmap scanedImage = null;
            try
            {
                using (Twain32 PLOCRtwain = new Twain32())
                {
                    PLOCRtwain.IsTwain2Enable = true;   // DSM(data source manager 를 열기 전에 twain 2.0 버전이상을 쓰겠다고 지정해야함
                    //  PLOCRtwain.Dispose();   // OpenDSM() 에서 보호된 메모리 쓰려고 하는 오류가 생겨서.... 원인을 찾아보고 이 코드는 안 쓰는 방향으로 해야함.
                    PLOCRtwain.OpenDSM();

                    //    PLOCRtwain.SourceIndex = 0;

                    PLOCRtwain.OpenDataSource();

                    //       PLOCRtwain.GetResolutions();

                    PLOCRtwain.Capabilities.XResolution.Set(300f);      // 스캔 해상도 300dpi 로 지정하고, float 로 넣어야 하므로 300f 로 입력
                    PLOCRtwain.Capabilities.YResolution.Set(300f);

                    PLOCRtwain.ShowUI = false;  // twain 사용자 인터페이서를 보이지 않도록 하고

                    try
                    {
                        if ((PLOCRtwain.IsCapSupported(TwCap.AutoScan) & TwQC.Set) != 0)
                        {
                            PLOCRtwain.SetCap(TwCap.AutoScan, false);       // 한장씩 처리하기 위해 aucoscan 을 false 로 세팅
                        }

                        if ((PLOCRtwain.IsCapSupported(TwCap.AutoFeed) & TwQC.Set) != 0)
                        {
                            PLOCRtwain.SetCap(TwCap.AutoFeed, false);       // 한장 처리하고 대기하기 위해 autofed 를 false 로 세팅
                        }

                        if ((PLOCRtwain.IsCapSupported(TwCap.DuplexEnabled) & TwQC.Set) != 0)
                        {
                            PLOCRtwain.SetCap(TwCap.DuplexEnabled, true);
                        }

                        PLOCRtwain.AcquireCompleted += (sender, e) =>   // 이 부분이 Acquire 보다 먼저 나와야 함, 한참 헤멨음 
                        {
                            // scanImage();
                            // string _file_name = string.Format("{0}\\ScanImage.jpg", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                            scanedImage = (Bitmap)PLOCRtwain.GetImage(0);
                            File.Delete(@"C:\Program Files\PLOCR\prescription.png");
                            scanedImage.Save(@"C:\Program Files\PLOCR\prescription.png");
                        };

                        if ((PLOCRtwain.IsCapSupported(TwCap.FeederEnabled) & TwQC.Set) != 0)
                        {
                            PLOCRtwain.SetCap(TwCap.FeederEnabled, true);

                            //        MessageBox.Show("feederenabled");                                                                                    
                            var _isFeederLoaded = false;

                            while (_isFeederLoaded == false)        // 처방전이 ADF 트레이에 올라올 때까지 무한루프를 돌리며 대기, 좋은 방법은 아닌 것 같음
                            {
                                //           MessageBox.Show("feederloaded 루프 안");

                                _isFeederLoaded = (bool)PLOCRtwain.GetCurrentCap(TwCap.FeederLoaded);   // 반복해서 현재 feederloaded 상태를 감시

                                if (_isFeederLoaded == true)    // 처방전이 올라왔으면 바로 읽어들이기 시작
                                {
                                    /////////////////// 처방전 스캔 시작 ///////////////////////////
                                    //splashPres fmPres = new splashPres();
                                    //fmPres.Owner = DataEdit.ActiveForm;  // child form 을 알리고
                                    //fmPres.Show();       // 스캔중 화면을 띄우고

                                    PLOCRtwain.Acquire();

                                    //fmPres.Close(); // 스캔중 화면 닫고     
                                    ///////////////// 처방전 스캔 끝 ///////////////////////////

                                    //  PLOCRtwain.Dispose();      // twain 닫고
                                    //    PLOCRtwain.CloseDataSource();
                                    //    PLOCRtwain.CloseDSM();
                                }
                                else if (_isFeederLoaded == false)
                                {
                                }
                            }
                        }

                        //    PLOCRtwain.Acquire();



                    }
                    catch (Exception)
                    {
                        Console.WriteLine("스캐너 옵션을 가져오지 못했습니다.");
                        System.Diagnostics.Process.GetCurrentProcess().Kill();      // 프로그램 강제 종료
                    }

                    //PLOCRtwain.CloseDataSource();
                    //PLOCRtwain.CloseDSM();
                }
            }
            catch (TwainException)
            {
                Console.WriteLine("스캔 오류가 발생했습니다.");
                System.Diagnostics.Process.GetCurrentProcess().Kill();      // 프로그램 강제 종료
            }

            return scanedImage;
        }   
    }
}
