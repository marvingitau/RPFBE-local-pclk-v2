﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DOCMgt
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", ConfigurationName="DOCMgt.DocumentMgmt_Port")]
    public interface DocumentMgmt_Port
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt:InsertNewDocumentEntry", ReplyAction="*")]
        System.Threading.Tasks.Task<DOCMgt.InsertNewDocumentEntry_Result> InsertNewDocumentEntryAsync(DOCMgt.InsertNewDocumentEntry request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt:InsertNewDocumentEntryByLine" +
            "No", ReplyAction="*")]
        System.Threading.Tasks.Task<DOCMgt.InsertNewDocumentEntryByLineNo_Result> InsertNewDocumentEntryByLineNoAsync(DOCMgt.InsertNewDocumentEntryByLineNo request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt:InsertLeaveApplicationDocume" +
            "ntEntry", ReplyAction="*")]
        System.Threading.Tasks.Task<DOCMgt.InsertLeaveApplicationDocumentEntry_Result> InsertLeaveApplicationDocumentEntryAsync(DOCMgt.InsertLeaveApplicationDocumentEntry request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt:ModifySystemFileURL", ReplyAction="*")]
        System.Threading.Tasks.Task<DOCMgt.ModifySystemFileURL_Result> ModifySystemFileURLAsync(DOCMgt.ModifySystemFileURL request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt:ModifyCustomerSystemFileURL", ReplyAction="*")]
        System.Threading.Tasks.Task<DOCMgt.ModifyCustomerSystemFileURL_Result> ModifyCustomerSystemFileURLAsync(DOCMgt.ModifyCustomerSystemFileURL request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt:ModifyDocumentEntryFileURL", ReplyAction="*")]
        System.Threading.Tasks.Task<DOCMgt.ModifyDocumentEntryFileURL_Result> ModifyDocumentEntryFileURLAsync(DOCMgt.ModifyDocumentEntryFileURL request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt:CheckDocumentAttached", ReplyAction="*")]
        System.Threading.Tasks.Task<DOCMgt.CheckDocumentAttached_Result> CheckDocumentAttachedAsync(DOCMgt.CheckDocumentAttached request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt:UploadDocumentPathPerLine", ReplyAction="*")]
        System.Threading.Tasks.Task<DOCMgt.UploadDocumentPathPerLine_Result> UploadDocumentPathPerLineAsync(DOCMgt.UploadDocumentPathPerLine request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="InsertNewDocumentEntry", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class InsertNewDocumentEntry
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public string documentNoa46;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=1)]
        public string documentCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=2)]
        public string documentDescription;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=3)]
        public string localFilePath;
        
        public InsertNewDocumentEntry()
        {
        }
        
        public InsertNewDocumentEntry(string documentNoa46, string documentCode, string documentDescription, string localFilePath)
        {
            this.documentNoa46 = documentNoa46;
            this.documentCode = documentCode;
            this.documentDescription = documentDescription;
            this.localFilePath = localFilePath;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="InsertNewDocumentEntry_Result", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class InsertNewDocumentEntry_Result
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public bool return_value;
        
        public InsertNewDocumentEntry_Result()
        {
        }
        
        public InsertNewDocumentEntry_Result(bool return_value)
        {
            this.return_value = return_value;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="InsertNewDocumentEntryByLineNo", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class InsertNewDocumentEntryByLineNo
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public int lineNo;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=1)]
        public string documentNoa46;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=2)]
        public string documentCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=3)]
        public string documentDescription;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=4)]
        public string localFilePath;
        
        public InsertNewDocumentEntryByLineNo()
        {
        }
        
        public InsertNewDocumentEntryByLineNo(int lineNo, string documentNoa46, string documentCode, string documentDescription, string localFilePath)
        {
            this.lineNo = lineNo;
            this.documentNoa46 = documentNoa46;
            this.documentCode = documentCode;
            this.documentDescription = documentDescription;
            this.localFilePath = localFilePath;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="InsertNewDocumentEntryByLineNo_Result", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class InsertNewDocumentEntryByLineNo_Result
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public bool return_value;
        
        public InsertNewDocumentEntryByLineNo_Result()
        {
        }
        
        public InsertNewDocumentEntryByLineNo_Result(bool return_value)
        {
            this.return_value = return_value;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="InsertLeaveApplicationDocumentEntry", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class InsertLeaveApplicationDocumentEntry
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public string documentNoa46;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=1)]
        public string leaveType;
        
        public InsertLeaveApplicationDocumentEntry()
        {
        }
        
        public InsertLeaveApplicationDocumentEntry(string documentNoa46, string leaveType)
        {
            this.documentNoa46 = documentNoa46;
            this.leaveType = leaveType;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="InsertLeaveApplicationDocumentEntry_Result", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class InsertLeaveApplicationDocumentEntry_Result
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public bool return_value;
        
        public InsertLeaveApplicationDocumentEntry_Result()
        {
        }
        
        public InsertLeaveApplicationDocumentEntry_Result(bool return_value)
        {
            this.return_value = return_value;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ModifySystemFileURL", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class ModifySystemFileURL
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public string documentNoa46;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=1)]
        public string documentCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=2)]
        public string localURL;
        
        public ModifySystemFileURL()
        {
        }
        
        public ModifySystemFileURL(string documentNoa46, string documentCode, string localURL)
        {
            this.documentNoa46 = documentNoa46;
            this.documentCode = documentCode;
            this.localURL = localURL;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ModifySystemFileURL_Result", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class ModifySystemFileURL_Result
    {
        
        public ModifySystemFileURL_Result()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ModifyCustomerSystemFileURL", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class ModifyCustomerSystemFileURL
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public string documentNoa46;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=1)]
        public string documentCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=2)]
        public string localURL;
        
        public ModifyCustomerSystemFileURL()
        {
        }
        
        public ModifyCustomerSystemFileURL(string documentNoa46, string documentCode, string localURL)
        {
            this.documentNoa46 = documentNoa46;
            this.documentCode = documentCode;
            this.localURL = localURL;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ModifyCustomerSystemFileURL_Result", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class ModifyCustomerSystemFileURL_Result
    {
        
        public ModifyCustomerSystemFileURL_Result()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ModifyDocumentEntryFileURL", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class ModifyDocumentEntryFileURL
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public string documentNoa46;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=1)]
        public string documentCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=2)]
        public string localURL;
        
        public ModifyDocumentEntryFileURL()
        {
        }
        
        public ModifyDocumentEntryFileURL(string documentNoa46, string documentCode, string localURL)
        {
            this.documentNoa46 = documentNoa46;
            this.documentCode = documentCode;
            this.localURL = localURL;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ModifyDocumentEntryFileURL_Result", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class ModifyDocumentEntryFileURL_Result
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public bool return_value;
        
        public ModifyDocumentEntryFileURL_Result()
        {
        }
        
        public ModifyDocumentEntryFileURL_Result(bool return_value)
        {
            this.return_value = return_value;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CheckDocumentAttached", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class CheckDocumentAttached
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public string documentNoa46;
        
        public CheckDocumentAttached()
        {
        }
        
        public CheckDocumentAttached(string documentNoa46)
        {
            this.documentNoa46 = documentNoa46;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CheckDocumentAttached_Result", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class CheckDocumentAttached_Result
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public bool return_value;
        
        public CheckDocumentAttached_Result()
        {
        }
        
        public CheckDocumentAttached_Result(bool return_value)
        {
            this.return_value = return_value;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UploadDocumentPathPerLine", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class UploadDocumentPathPerLine
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public int lineNo;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=1)]
        public string documentNo;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=2)]
        public string workstreamCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=3)]
        public string filePath;
        
        public UploadDocumentPathPerLine()
        {
        }
        
        public UploadDocumentPathPerLine(int lineNo, string documentNo, string workstreamCode, string filePath)
        {
            this.lineNo = lineNo;
            this.documentNo = documentNo;
            this.workstreamCode = workstreamCode;
            this.filePath = filePath;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UploadDocumentPathPerLine_Result", WrapperNamespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", IsWrapped=true)]
    public partial class UploadDocumentPathPerLine_Result
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:microsoft-dynamics-schemas/codeunit/DocumentMgmt", Order=0)]
        public bool return_value;
        
        public UploadDocumentPathPerLine_Result()
        {
        }
        
        public UploadDocumentPathPerLine_Result(bool return_value)
        {
            this.return_value = return_value;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public interface DocumentMgmt_PortChannel : DOCMgt.DocumentMgmt_Port, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public partial class DocumentMgmt_PortClient : System.ServiceModel.ClientBase<DOCMgt.DocumentMgmt_Port>, DOCMgt.DocumentMgmt_Port
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public DocumentMgmt_PortClient() : 
                base(DocumentMgmt_PortClient.GetDefaultBinding(), DocumentMgmt_PortClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.DocumentMgmt_Port.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DocumentMgmt_PortClient(EndpointConfiguration endpointConfiguration) : 
                base(DocumentMgmt_PortClient.GetBindingForEndpoint(endpointConfiguration), DocumentMgmt_PortClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DocumentMgmt_PortClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(DocumentMgmt_PortClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DocumentMgmt_PortClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(DocumentMgmt_PortClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DocumentMgmt_PortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DOCMgt.InsertNewDocumentEntry_Result> DOCMgt.DocumentMgmt_Port.InsertNewDocumentEntryAsync(DOCMgt.InsertNewDocumentEntry request)
        {
            return base.Channel.InsertNewDocumentEntryAsync(request);
        }
        
        public System.Threading.Tasks.Task<DOCMgt.InsertNewDocumentEntry_Result> InsertNewDocumentEntryAsync(string documentNoa46, string documentCode, string documentDescription, string localFilePath)
        {
            DOCMgt.InsertNewDocumentEntry inValue = new DOCMgt.InsertNewDocumentEntry();
            inValue.documentNoa46 = documentNoa46;
            inValue.documentCode = documentCode;
            inValue.documentDescription = documentDescription;
            inValue.localFilePath = localFilePath;
            return ((DOCMgt.DocumentMgmt_Port)(this)).InsertNewDocumentEntryAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DOCMgt.InsertNewDocumentEntryByLineNo_Result> DOCMgt.DocumentMgmt_Port.InsertNewDocumentEntryByLineNoAsync(DOCMgt.InsertNewDocumentEntryByLineNo request)
        {
            return base.Channel.InsertNewDocumentEntryByLineNoAsync(request);
        }
        
        public System.Threading.Tasks.Task<DOCMgt.InsertNewDocumentEntryByLineNo_Result> InsertNewDocumentEntryByLineNoAsync(int lineNo, string documentNoa46, string documentCode, string documentDescription, string localFilePath)
        {
            DOCMgt.InsertNewDocumentEntryByLineNo inValue = new DOCMgt.InsertNewDocumentEntryByLineNo();
            inValue.lineNo = lineNo;
            inValue.documentNoa46 = documentNoa46;
            inValue.documentCode = documentCode;
            inValue.documentDescription = documentDescription;
            inValue.localFilePath = localFilePath;
            return ((DOCMgt.DocumentMgmt_Port)(this)).InsertNewDocumentEntryByLineNoAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DOCMgt.InsertLeaveApplicationDocumentEntry_Result> DOCMgt.DocumentMgmt_Port.InsertLeaveApplicationDocumentEntryAsync(DOCMgt.InsertLeaveApplicationDocumentEntry request)
        {
            return base.Channel.InsertLeaveApplicationDocumentEntryAsync(request);
        }
        
        public System.Threading.Tasks.Task<DOCMgt.InsertLeaveApplicationDocumentEntry_Result> InsertLeaveApplicationDocumentEntryAsync(string documentNoa46, string leaveType)
        {
            DOCMgt.InsertLeaveApplicationDocumentEntry inValue = new DOCMgt.InsertLeaveApplicationDocumentEntry();
            inValue.documentNoa46 = documentNoa46;
            inValue.leaveType = leaveType;
            return ((DOCMgt.DocumentMgmt_Port)(this)).InsertLeaveApplicationDocumentEntryAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DOCMgt.ModifySystemFileURL_Result> DOCMgt.DocumentMgmt_Port.ModifySystemFileURLAsync(DOCMgt.ModifySystemFileURL request)
        {
            return base.Channel.ModifySystemFileURLAsync(request);
        }
        
        public System.Threading.Tasks.Task<DOCMgt.ModifySystemFileURL_Result> ModifySystemFileURLAsync(string documentNoa46, string documentCode, string localURL)
        {
            DOCMgt.ModifySystemFileURL inValue = new DOCMgt.ModifySystemFileURL();
            inValue.documentNoa46 = documentNoa46;
            inValue.documentCode = documentCode;
            inValue.localURL = localURL;
            return ((DOCMgt.DocumentMgmt_Port)(this)).ModifySystemFileURLAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DOCMgt.ModifyCustomerSystemFileURL_Result> DOCMgt.DocumentMgmt_Port.ModifyCustomerSystemFileURLAsync(DOCMgt.ModifyCustomerSystemFileURL request)
        {
            return base.Channel.ModifyCustomerSystemFileURLAsync(request);
        }
        
        public System.Threading.Tasks.Task<DOCMgt.ModifyCustomerSystemFileURL_Result> ModifyCustomerSystemFileURLAsync(string documentNoa46, string documentCode, string localURL)
        {
            DOCMgt.ModifyCustomerSystemFileURL inValue = new DOCMgt.ModifyCustomerSystemFileURL();
            inValue.documentNoa46 = documentNoa46;
            inValue.documentCode = documentCode;
            inValue.localURL = localURL;
            return ((DOCMgt.DocumentMgmt_Port)(this)).ModifyCustomerSystemFileURLAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DOCMgt.ModifyDocumentEntryFileURL_Result> DOCMgt.DocumentMgmt_Port.ModifyDocumentEntryFileURLAsync(DOCMgt.ModifyDocumentEntryFileURL request)
        {
            return base.Channel.ModifyDocumentEntryFileURLAsync(request);
        }
        
        public System.Threading.Tasks.Task<DOCMgt.ModifyDocumentEntryFileURL_Result> ModifyDocumentEntryFileURLAsync(string documentNoa46, string documentCode, string localURL)
        {
            DOCMgt.ModifyDocumentEntryFileURL inValue = new DOCMgt.ModifyDocumentEntryFileURL();
            inValue.documentNoa46 = documentNoa46;
            inValue.documentCode = documentCode;
            inValue.localURL = localURL;
            return ((DOCMgt.DocumentMgmt_Port)(this)).ModifyDocumentEntryFileURLAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DOCMgt.CheckDocumentAttached_Result> DOCMgt.DocumentMgmt_Port.CheckDocumentAttachedAsync(DOCMgt.CheckDocumentAttached request)
        {
            return base.Channel.CheckDocumentAttachedAsync(request);
        }
        
        public System.Threading.Tasks.Task<DOCMgt.CheckDocumentAttached_Result> CheckDocumentAttachedAsync(string documentNoa46)
        {
            DOCMgt.CheckDocumentAttached inValue = new DOCMgt.CheckDocumentAttached();
            inValue.documentNoa46 = documentNoa46;
            return ((DOCMgt.DocumentMgmt_Port)(this)).CheckDocumentAttachedAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DOCMgt.UploadDocumentPathPerLine_Result> DOCMgt.DocumentMgmt_Port.UploadDocumentPathPerLineAsync(DOCMgt.UploadDocumentPathPerLine request)
        {
            return base.Channel.UploadDocumentPathPerLineAsync(request);
        }
        
        public System.Threading.Tasks.Task<DOCMgt.UploadDocumentPathPerLine_Result> UploadDocumentPathPerLineAsync(int lineNo, string documentNo, string workstreamCode, string filePath)
        {
            DOCMgt.UploadDocumentPathPerLine inValue = new DOCMgt.UploadDocumentPathPerLine();
            inValue.lineNo = lineNo;
            inValue.documentNo = documentNo;
            inValue.workstreamCode = workstreamCode;
            inValue.filePath = filePath;
            return ((DOCMgt.DocumentMgmt_Port)(this)).UploadDocumentPathPerLineAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.DocumentMgmt_Port))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;
                result.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.DocumentMgmt_Port))
            {
                return new System.ServiceModel.EndpointAddress("http://desktop-csglh1r:7347/PLATCORP/WS/Platcorp Training/Codeunit/DocumentMgmt");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return DocumentMgmt_PortClient.GetBindingForEndpoint(EndpointConfiguration.DocumentMgmt_Port);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return DocumentMgmt_PortClient.GetEndpointAddress(EndpointConfiguration.DocumentMgmt_Port);
        }
        
        public enum EndpointConfiguration
        {
            
            DocumentMgmt_Port,
        }
    }
}
