using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Bentley.MicroStation.WinForms;
using Bentley.GenerativeComponents.UISupport;
using Bentley.GenerativeComponents.Features;
using Bentley.GenerativeComponents.Features.Specific;

namespace GC_FormTimer
{
    public partial class TimercontrolFloat : AdapterWinForm
    {
        private TimerControl nodeTimer;
        public int FrameCt;

 
        protected override void OnAdapterWinFormClosing(bool trulyClosing)
        {

            FrameCt = this.timerControl1.frameCt;
            this.timerControl1.StopTimer();
            this.Dispose();
  
        }
        protected override void DisposeManagedResources()
        {
            if (components != null)
            {

                components.Dispose();
            }
        }

        public TimercontrolFloat()
            : base(true, "GC Timer", new System.Drawing.Size(190, 111), AdapterWinFormDockability.All)
        {
            InitializeComponent();
        }
        public TimercontrolFloat(TimerControl tc)
            : base(true, "GC Timer", new System.Drawing.Size(190, 111), AdapterWinFormDockability.All)
        {
            
            nodeTimer = tc;
            
            InitializeComponent();
            
            this.FormClosing += TimercontrolFloat_FormClosing;

            //this.Name = nodeTimer.tmrName;
            this.timerControl1.btFloat.Visibility = System.Windows.Visibility.Hidden;
            FrameCt = nodeTimer.frameCt;
            this.timerControl1.frameCt = nodeTimer.frameCt;
            this.timerControl1.txtFrameNum.Content = nodeTimer.frameCt.ToString();
            this.timerControl1.timeInt = nodeTimer.timeInt;
            this.timerControl1.timer.Interval = TimeSpan.FromMilliseconds( nodeTimer.timeInt);
            this.timerControl1.objs = nodeTimer.objs;
    
        }

        void TimercontrolFloat_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrameCt = this.timerControl1.frameCt;
        }

        public void updateTimer(double interval, Feature[] objects)
        {
            this.timerControl1.timer.Interval = TimeSpan.FromMilliseconds(interval);
            this.timerControl1.objs = objects;

        }

    }
}
