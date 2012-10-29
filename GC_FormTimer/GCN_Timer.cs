using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


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
using GC_FormTimer;
using cerver.timer;

//using Bentley.Wrapper;

namespace cerver.timer
{
    [GCNamespace("cerver.timer",true)]
    public class Timer2: Node
    {
        // The purpose of this particular node type is to represent the sun in the sky, for
        // lighting purposes. More precisely, any instance of this node type serves as a
        // "front end" to the sun object that's built into MicroStation.
        //
        // As such, you will see a lot of code herein that does things with a class named
        // MicrostationConfiguration. Please be aware that such code is relevant only because
        // of the functionality of this particular node type. Your own node types, on the other
        // hand, will most likely not have any association with MicroStation, at all.
        public event UpdateHandler Changed;
        public EventArgs e = null;
        public delegate void UpdateHandler(Timer2 m, EventArgs e);

        public event NodeClosingHandler NodeClosing;
        public delegate void NodeClosingHandler(EventArgs e);
        

        public const string NameOfUpdateObject             = "ObjToUpdate";
        public const string NameOfInterval =                  "UpdateInterval";

        public const string NameOfDefaultTechnique              = "Default";
     
        static private readonly NodeGCType s_gcTypeOfAllInstances = (NodeGCType) GCTypeTools.GetGCType(typeof(Timer2));
       


        static public NodeGCType GCTypeOfAllInstances
        {
            get { return s_gcTypeOfAllInstances; }
        }

        static private void GCType_AddAdditionalMembersTo(GCType gcType, NativeNamespaceTranslator namespaceTranslator)
        {
            // This method is called through reflection when this type's corresponding GCType is populated.
            {
                NodeTechnique method = gcType.AddDefaultNodeTechnique(NameOfDefaultTechnique, Default);
                
                method.AddArgumentDefinition(NameOfUpdateObject, typeof(Feature[]), "null", "The nodes to upadte with the timer" );
                method.AddArgumentDefinition(NameOfInterval,  typeof(double), "25", "The update time in milliseconds");

            }

        }
        
        static private NodeTechniqueResult Default(Node node, IGCEnvironment gcEnvironment, NameCatalog nameCatalog, NodeScopeUpdateReason updateReason)
        {
            Timer2 tmr = (Timer2)node;
            
            if(tmr.Changed != null)
                tmr.Changed(tmr, tmr.e); 
            return NodeTechniqueResult.Success;
        }

        public static void testFunc(string message)
        {
            System.Windows.MessageBox.Show(message);
       
        }
        public void triggerChange(Timer2 tmr, EventArgs e)
        {
            Changed(tmr, e);
        }
  
        // ======================================== end of static members ========================================
        

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
     

        public Timer2
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
            // The result can be any type that derives from a WPF FrameworkElement.

            return typeof(TimerControl);
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



        public new class NodeState: Node.NodeState
        {
            internal readonly NodeProperty UpdateObjectProperty;
            internal readonly NodeProperty UpdateIntervalProperty;

            internal protected NodeState(Timer2 parentNode, NodeScopeState parentNodeScopeState, NodeTechniqueDetermination initialActiveTechniqueDetermination)
                : base(parentNode, parentNodeScopeState, initialActiveTechniqueDetermination)
            {
                // This constructor is called when the parent node is created.
                // To create each property, we call AddProperty (rather to GetProperty).

                UpdateObjectProperty = AddProperty(NameOfUpdateObject);
                UpdateIntervalProperty = AddProperty(NameOfInterval);

            }

            protected NodeState(NodeState source, NodeScopeState parentNodeScopeState): base(source, parentNodeScopeState)  // For cloning.
            {
                // This constructor is called whenever the node state is copied.
                // To copy each property, we call GetProperty (rather than AddProperty).

                UpdateObjectProperty = GetProperty(NameOfUpdateObject);
                UpdateIntervalProperty = GetProperty(NameOfInterval);
            }

            protected new Timer2 ParentNode
            {
                get { return (Timer2)base.ParentNode; }
            }

            public override Node.NodeState Clone(NodeScopeState newParentNodeScopeState)
            {
                return new NodeState(this, newParentNodeScopeState);
            }

            
        }
    }
}
