using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

using Bentley.MicroStation.WinForms;
using Bentley.GenerativeComponents.UISupport;
using Bentley.GenerativeComponents;
using Bentley.GenerativeComponents.Features;
using Bentley.GenerativeComponents.Features.Specific;
using Bentley.GenerativeComponents.GCScript;
using Bentley.GenerativeComponents.MicroStation;
using Bentley.GenerativeComponents.API;
using Bentley.GenerativeComponents.GeneralPurpose.Wpf;

using cerver.timer;

namespace GC_FormTimer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TimerControl : UserControl
    {
        
        public bool rset = true;
        public List<INode> GCobjs ;
        public int tickCt;
        public int frameCt = 0;
        public double timeInt = 25;
        public string tmrName;
        private int frameRate;
        private int prevTime = 0;
        private int currentTime;
        public DispatcherTimer timer;
        public Feature[] objs;
        public Timer2 thisNode;

        private TimercontrolFloat tcf;
        
        public TimerControl()
        {
            
            timer = new DispatcherTimer();
            timer.Tick += gcTimer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(timeInt);
            this.DataContextChanged += NodeControl_DataContextChanged;
            

            InitializeComponent();

            
        }

        void NodeControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(thisNode == null) thisNode = (Timer2)this.DataContext;
            thisNode.Changed += thisNode_Changed;
            thisNode.NodeClosing += thisNode_NodeClosing;
            //tmrName = thisNode.Name;
        }

        void thisNode_NodeClosing(EventArgs e)
        {
            timer.Stop();
            if (tcf != null)
            {
                tcf.Close();

            }
        }

        void thisNode_Changed(Timer2 m, EventArgs e)
        {
            timeInt = thisNode.interval;
            timer.Interval = TimeSpan.FromMilliseconds(timeInt);
            objs = thisNode.GCobjects;
            if (tcf != null)
            {
                tcf.updateTimer(timeInt, objs);

            }
           
        }

        private void btPlay_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)btPlay.IsChecked)
            {
                thisNode_Changed(thisNode, EventArgs.Empty);
                if ((bool)this.btReset.IsChecked) btReset.IsChecked = false;
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }


        private void btReset_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if ((bool)btReset.IsChecked)
            {
                resetTrue();
                //timerObj.UpdateNodeTree(false);
            }
            else
            {
                rset = false;
                //resetStatus.Text = "Reset: FALSE";
               // timerObj.UpdateNodeTree(false);
            }
            updateGC();
        }


        private void gcTimer_Tick(object sender, EventArgs e)
        {
            if (System.Environment.TickCount > tickCt)
            {
                currentTime = System.Environment.TickCount;
                tickCt = currentTime + timer.Interval.Milliseconds; 
                
                //////common
                frameCt++;
                txtFrameNum.Content = frameCt.ToString();
                //get actual time
                frameRate = currentTime - prevTime;
                prevTime = currentTime;
                //FrameRateTxt.Text = string.Format("Actual Time: {0} ms", frameRate);
                //update
                updateGC();
                
            }
        }

        private void resetTrue()
        {
            timer.Stop();
            btPlay.IsChecked = false;
            if(txtFrameNum != null) txtFrameNum.Content = "0";
            rset = true;
            if(frameCt>0)
                updateGC();
            frameCt = 0;
        }

        private void updateGC()
        {
           // timerObj.UpdateNodeTree();
            if (objs != null)
            {
                GCTools.UserModel.Lock(GCTools.UserModel);
                try
                {
                    APIHelper.UpdateNodeTree(objs);
                    GCTools.SyncUpMicroStation();
                }
                finally
                {
                    GCTools.UserModel.Unlock(GCTools.UserModel);
                }
                
            }

        }

       

        private void FrameRateTxt_Click(object sender, EventArgs e)
        {

        }
        /////public///////////////////////////////
        public void StopTimer()
        {
            if ((bool)btPlay.IsChecked)
            {
                if ((bool)this.btReset.IsChecked) btReset.IsChecked = false ;
                timer.Stop();
            }
        }

        public void resetTimer()
        {
            resetTrue();
        }
        public bool getResetState()
        {
            
            return rset;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (tcf == null)
            {
                timer.Stop();
                tcf = new TimercontrolFloat(this);
                tcf.Show();
                //tcf.FormClosing += tcf_FormClosed;
                tcf.Disposed += tcf_Disposed;
                
                this.btFloat.IsEnabled = false;
                this.btPlay.IsEnabled = false;
                this.btReset.IsEnabled = false;
                this.txtFrameNum.Content = "";
                this.txtRemoteControl.Visibility = System.Windows.Visibility.Visible;
            }
            
        }

 
        void tcf_Disposed(object sender, EventArgs e)
        {
            this.btFloat.IsEnabled = true;
            this.btPlay.IsEnabled = true;
            this.btReset.IsEnabled = true;
            this.txtRemoteControl.Visibility = System.Windows.Visibility.Hidden;

            this.frameCt = tcf.FrameCt;
            this.txtFrameNum.Content = tcf.FrameCt.ToString();
            tcf.Dispose();
            tcf = null;
        }

        
    }
}
