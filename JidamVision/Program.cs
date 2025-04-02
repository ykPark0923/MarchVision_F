using JidamVision.Core;
using JidamVision.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JidamVision
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //#LOGFORM#2 log4net 설정 파일을 읽어들임
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

            SLogger.Write("Logger initialized!", SLogger.LogType.Info);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            //// SplashForm 먼저 보여줌
            //SplashForm splash = new SplashForm();
            //splash.Show();
            //splash.Refresh(); // UI 즉시 반영

            //// 초기화 작업 수행 (비동기로 진행)
            //Task.Run(() =>
            //{
            //    // 이곳에 실제 초기화 작업을 넣으면 됨
            //    Thread.Sleep(3000); // 예: 3초간 초기화 작업 가정
            //    Global.Inst.Initialize(); // 실제 초기화 함수
            //}).Wait(); // 완료될 때까지 기다림

            //// Splash 화면 닫고 메인폼 실행
            //splash.Close();

            Application.Run(new MainForm());
        }
    }
}
