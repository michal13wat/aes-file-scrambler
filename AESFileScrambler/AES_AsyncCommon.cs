using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AESFileScrambler
{
    public class AES_AsyncCommon
    {
        public BackgroundWorker backgroundWorker;

        public AES_AsyncCommon() {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
        }

        protected void AES_Completed() {
            backgroundWorker.ProgressChanged -=
                (System.Windows.Application.Current.MainWindow as MainWindow)
                .updateEncProgressBar;
        }
    }
}
