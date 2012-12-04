using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ServiceModel ;

using Bentley.GenerativeComponents;
using Bentley.GenerativeComponents.Features;
using Bentley.GenerativeComponents.Features.Specific;
using Bentley.GenerativeComponents.GCScript;
using Bentley.GenerativeComponents.GCScript.GCTypes;
using Bentley.GenerativeComponents.GCScript.NameScopes;
using Bentley.GenerativeComponents.GCScript.ReflectedNativeTypeSupport;
using Bentley.GenerativeComponents.GeneralPurpose;
using Bentley.GenerativeComponents.MicroStation;
using Bentley.GenerativeComponents.Nodes;
using Bentley.Geometry;
using Bentley.Interop.MicroStationDGN;
using Cerver.Timer;

//using Bentley.Wrapper;

namespace Cerver.Timer
{


    [GCNamespace("cerver.timer",true)]
    public class Timer : Node, IControlPanelShowableNode
    {
        // The purpose of this particular node type is to represent the sun in the sky, for
        // lighting purposes. More precisely, any instance of this node type serves as a
        // "front end" to the sun object that's built into MicroStation.
        //
        // As such, you will see a lot of code herein that does things with a class named
        // MicrostationConfiguration. Please be aware that such code is relevant only because
        // of the functionality of this particular node type. Your own node types, on the other
        // hand, will most likely not have any association with MicroStation, at all.
        
        
        public EventArgs e = null;
        public delegate void UpdateHandler(Timer m, EventArgs e);
        public delegate void NodeClosingHandler(EventArgs e);
        
        public event NodeClosingHandler NodeClosing;
        public event UpdateHandler Changed;
        
        public const string NameOfUpdateObject              = "ObjToUpdate";
        public const string NameOfInterval                  = "UpdateInterval";
        public const string NameOfReset                     = "Reset";
        public const string NameOfDefaultTechnique          = "Default";
     
        static private readonly NodeGCType s_gcTypeOfAllInstances = (NodeGCType) GCTypeTools.GetGCType(typeof(Timer));
       
        static public NodeGCType GCTypeOfAllInstances
        {
            get { return s_gcTypeOfAllInstances; }
        }

        static private void GCType_AddAdditionalMembersTo(GCType gcType, NativeNamespaceTranslator namespaceTranslator)
        {
            
            // This method is called through reflection when this type's corresponding GCType is populated.
            NodeTechnique method = gcType.AddDefaultNodeTechnique(NameOfDefaultTechnique, Default);
            
            //inputs
            method.AddArgumentDefinition(NameOfUpdateObject, typeof(Feature[]), "null", "The nodes to upadte with the timer" );
            method.AddArgumentDefinition(NameOfInterval,  typeof(double), "25", "The update time in milliseconds");
            //outputs
            method.AddArgumentDefinition(NameOfReset, typeof(bool), "true", "The reset state (true/false)", NodePortRole.TechniqueOutputOnly);

        }
        
        static private NodeTechniqueResult Default(Node node, IGCEnvironment gcEnvironment, NameCatalog nameCatalog, NodeScopeUpdateReason updateReason)
        {
            Timer tmr = (Timer)node;
                   
            if(tmr.Changed != null)
                tmr.Changed(tmr, tmr.e);

            return NodeTechniqueResult.Success;
        }

        public void triggerChange(Timer tmr, EventArgs e)
        {
            Changed(tmr, e);
        }
  
        // ======================================== end of static members ========================================

        public bool Reset
        {
            get { return ActiveNodeState.ResetProperty.GetNativeValue<bool>(); }
            set { ActiveNodeState.ResetProperty.SetNativeValueAndInputExpression(value); }
        }
        public double interval
        {
            get { return ActiveNodeState.UpdateIntervalProperty.GetNativeValue<double>(); }
            set {ActiveNodeState.UpdateIntervalProperty.SetNativeValueAndInputExpression(value); }
        }

        public Feature[] GCobjects
        {
            get { return ActiveNodeState.UpdateObjectProperty.GetNativeValue<Feature[]>(); }
            set { ActiveNodeState.UpdateObjectProperty.SetNativeValueAndInputExpression(value); }
        }
     

        public Timer
        (
            NodeGCType  gcType,
            INodeScope  parentNodeScope,
            NamePath    namePath
        )
            : base(gcType, parentNodeScope, namePath)
        {
            Debug.Assert(gcType == s_gcTypeOfAllInstances);

        }

        public override Type TypeOfCustomViewContent(NodeCustomViewContext context)  // INode.TypeOfCustomViewBody
        {
            switch (context)
            {
                case NodeCustomViewContext.GraphNode: return typeof(TimerControl);  
                case NodeCustomViewContext.ControlPanel: return typeof(TimerControl);  
            }
            Debug.Assert(false);
            return null;

   
        }
       
        public override void Dispose()
        {
            NodeClosing(e);
            base.Dispose();
        }


        internal new NodeState ActiveNodeState
        {
            get { return (NodeState) base.ActiveNodeState; }
        }

        protected override Node.NodeState GetInitialNodeState(NodeScopeState parentNodeScopeState, NodeTechniqueDetermination nameOfInitiallyActiveTechnique)
        {
            return new NodeState(this, parentNodeScopeState, nameOfInitiallyActiveTechnique);
        }



        // wpf bindings
        private string m_frameNum;
        private bool m_PlayPressed;
        private bool m_resetPressed = true;
        
        
        public string frameNum
        {
            get { return m_frameNum; }
            set 
            { 
                m_frameNum = value;
                OnPropertyChanged("frameNum");
            }
        }
        
        public bool PlayPressed
        {
            get { return m_PlayPressed; }
            set
            {
                m_PlayPressed = value;
                OnPropertyChanged("PlayPressed");
            }
        }

        public bool ResetPressed
        {
            get { return m_resetPressed; }
            set
            {
                m_resetPressed = value;
                OnPropertyChanged("ResetPressed");
            }
        }

        //end wpf bingings

        public new class NodeState: Node.NodeState
        {
            internal readonly NodeProperty UpdateObjectProperty;
            internal readonly NodeProperty UpdateIntervalProperty;
            internal readonly NodeProperty ResetProperty;

            internal protected NodeState(Timer parentNode, NodeScopeState parentNodeScopeState, NodeTechniqueDetermination initialActiveTechniqueDetermination)
                : base(parentNode, parentNodeScopeState, initialActiveTechniqueDetermination)
            {
                // This constructor is called when the parent node is created.
                // To create each property, we call AddProperty (rather to GetProperty).

                UpdateObjectProperty = AddProperty(NameOfUpdateObject);
                UpdateIntervalProperty = AddProperty(NameOfInterval);
                ResetProperty = AddProperty(NameOfReset);

            }

            protected NodeState(NodeState source, NodeScopeState parentNodeScopeState): base(source, parentNodeScopeState)  // For cloning.
            {
                // This constructor is called whenever the node state is copied.
                // To copy each property, we call GetProperty (rather than AddProperty).

                UpdateObjectProperty = GetProperty(NameOfUpdateObject);
                UpdateIntervalProperty = GetProperty(NameOfInterval);
                ResetProperty = GetProperty(NameOfReset);
            }

            protected new Timer ParentNode
            {
                get { return (Timer)base.ParentNode; }
            }

            public override Node.NodeState Clone(NodeScopeState newParentNodeScopeState)
            {
                return new NodeState(this, newParentNodeScopeState);
            }

            
        }
    }
}
