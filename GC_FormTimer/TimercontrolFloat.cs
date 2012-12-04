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

namespace Cerver.Timer
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

            FrameCt = nodeTimer.frameCt;
            this.timerControl1.frameCt = nodeTimer.frameCt;
            this.timerControl1.txtFrameNum.Content = nodeTimer.frameCt.ToString();
            this.timerControl1.timeInt = nodeTimer.timeInt;
            this.timerControl1.timer.Interval = TimeSpan.FromMilliseconds( nodeTimer.timeInt);
            this.timerControl1.objs = nodeTimer.objs;

            //resize the buttons for the bigger display
            this.timerControl1.Height = 60;
            this.timerControl1.btPlay.Height = 45;
            this.timerControl1.btReset.Height = 45;
    
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
