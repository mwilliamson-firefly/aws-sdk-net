/*
 * Copyright 2010-2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
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

/*
 * Do not modify this file. This file is generated from the logs-2014-03-28.normal.json service model.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using ThirdParty.Json.LitJson;

namespace Amazon.CloudWatchLogs.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// PutSubscriptionFilter Request Marshaller
    /// </summary>       
    public class PutSubscriptionFilterRequestMarshaller : IMarshaller<IRequest, PutSubscriptionFilterRequest> , IMarshaller<IRequest,AmazonWebServiceRequest>
    {
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((PutSubscriptionFilterRequest)input);
        }

        public IRequest Marshall(PutSubscriptionFilterRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.CloudWatchLogs");
            string target = "Logs_20140328.PutSubscriptionFilter";
            request.Headers["X-Amz-Target"] = target;
            request.Headers["Content-Type"] = "application/x-amz-json-1.1";
            request.HttpMethod = "POST";

            string uriResourcePath = "/";
            request.ResourcePath = uriResourcePath;
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonWriter(stringWriter);
                writer.WriteObjectStart();
                var context = new JsonMarshallerContext(request, writer);
                if(publicRequest.IsSetDestinationArn())
                {
                    context.Writer.WritePropertyName("destinationArn");
                    context.Writer.Write(publicRequest.DestinationArn);
                }

                if(publicRequest.IsSetFilterName())
                {
                    context.Writer.WritePropertyName("filterName");
                    context.Writer.Write(publicRequest.FilterName);
                }

                if(publicRequest.IsSetFilterPattern())
                {
                    context.Writer.WritePropertyName("filterPattern");
                    context.Writer.Write(publicRequest.FilterPattern);
                }

                if(publicRequest.IsSetLogGroupName())
                {
                    context.Writer.WritePropertyName("logGroupName");
                    context.Writer.Write(publicRequest.LogGroupName);
                }

                if(publicRequest.IsSetRoleArn())
                {
                    context.Writer.WritePropertyName("roleArn");
                    context.Writer.Write(publicRequest.RoleArn);
                }

        
                writer.WriteObjectEnd();
                string snippet = stringWriter.ToString();
                request.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }


            return request;
        }


    }
}