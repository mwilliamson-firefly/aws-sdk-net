/*
 * Copyright 2010-2012 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */ 
    using System;
    using System.Collections.Generic; 
    using Amazon.SimpleWorkflow.Model; 
    using Amazon.Runtime.Internal.Transform; 

    namespace Amazon.SimpleWorkflow.Model.Internal.MarshallTransformations 
    { 
      /// <summary> 
      /// WorkflowExecutionTerminatedEventAttributesUnmarshaller 
      /// </summary> 
      internal class WorkflowExecutionTerminatedEventAttributesUnmarshaller : IUnmarshaller<WorkflowExecutionTerminatedEventAttributes, UnmarshallerContext>, IUnmarshaller<WorkflowExecutionTerminatedEventAttributes, JsonUnmarshallerContext> 
      { 
        WorkflowExecutionTerminatedEventAttributes IUnmarshaller<WorkflowExecutionTerminatedEventAttributes, UnmarshallerContext>.Unmarshall(UnmarshallerContext context)
        {
          throw new NotImplementedException();
        }
        
        public WorkflowExecutionTerminatedEventAttributes Unmarshall(JsonUnmarshallerContext context) 
        { 
          WorkflowExecutionTerminatedEventAttributes workflowExecutionTerminatedEventAttributes = new WorkflowExecutionTerminatedEventAttributes();
          
          int originalDepth = context.CurrentDepth;
          int targetDepth = originalDepth + 1;
          while (context.Read())
          {
            if ((context.IsKey) && (context.CurrentDepth == targetDepth))
            {
              context.Read();
              context.Read();
               
              if (context.TestExpression("Reason", targetDepth)) 
              {
                workflowExecutionTerminatedEventAttributes.Reason = StringUnmarshaller.GetInstance().Unmarshall(context);
                continue; 
              }
   
              if (context.TestExpression("Details", targetDepth)) 
              {
                workflowExecutionTerminatedEventAttributes.Details = StringUnmarshaller.GetInstance().Unmarshall(context);
                continue; 
              }
   
              if (context.TestExpression("ChildPolicy", targetDepth)) 
              {
                workflowExecutionTerminatedEventAttributes.ChildPolicy = StringUnmarshaller.GetInstance().Unmarshall(context);
                continue; 
              }
   
              if (context.TestExpression("Cause", targetDepth)) 
              {
                workflowExecutionTerminatedEventAttributes.Cause = StringUnmarshaller.GetInstance().Unmarshall(context);
                continue; 
              }
   
            } 
            else if (context.IsEndElement && context.CurrentDepth <= originalDepth) 
            { 
              return workflowExecutionTerminatedEventAttributes; 
            } 
          } 
          return workflowExecutionTerminatedEventAttributes; 
        } 
        
        private static WorkflowExecutionTerminatedEventAttributesUnmarshaller instance; 
        public static WorkflowExecutionTerminatedEventAttributesUnmarshaller GetInstance() 
        { 
          if (instance == null) 
            instance = new WorkflowExecutionTerminatedEventAttributesUnmarshaller(); 
          return instance;
        } 
      } 
    } 
  