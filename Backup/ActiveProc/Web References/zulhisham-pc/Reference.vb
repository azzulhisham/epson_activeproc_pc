﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.4927
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.4927.
'
Namespace zulhisham_pc
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="az_ServicesSoap", [Namespace]:="http://zulhisham-tan/az_services/")>  _
    Partial Public Class az_Services
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private HelloWorldOperationCompleted As System.Threading.SendOrPostCallback
        
        Private CheckIMIOperationCompleted As System.Threading.SendOrPostCallback
        
        Private AboutMeOperationCompleted As System.Threading.SendOrPostCallback
        
        Private azWeekCode_FCOperationCompleted As System.Threading.SendOrPostCallback
        
        Private azWeekCodeEx_FCOperationCompleted As System.Threading.SendOrPostCallback
        
        Private UpdateRecordsOperationCompleted As System.Threading.SendOrPostCallback
        
        Private GetMarkingCodeOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = Global.ActiveProc.My.MySettings.Default.ActiveProc_zulhisham_pc_az_Services
            If (Me.IsLocalFileSystemWebService(Me.Url) = true) Then
                Me.UseDefaultCredentials = true
                Me.useDefaultCredentialsSetExplicitly = false
            Else
                Me.useDefaultCredentialsSetExplicitly = true
            End If
        End Sub
        
        Public Shadows Property Url() As String
            Get
                Return MyBase.Url
            End Get
            Set
                If (((Me.IsLocalFileSystemWebService(MyBase.Url) = true)  _
                            AndAlso (Me.useDefaultCredentialsSetExplicitly = false))  _
                            AndAlso (Me.IsLocalFileSystemWebService(value) = false)) Then
                    MyBase.UseDefaultCredentials = false
                End If
                MyBase.Url = value
            End Set
        End Property
        
        Public Shadows Property UseDefaultCredentials() As Boolean
            Get
                Return MyBase.UseDefaultCredentials
            End Get
            Set
                MyBase.UseDefaultCredentials = value
                Me.useDefaultCredentialsSetExplicitly = true
            End Set
        End Property
        
        '''<remarks/>
        Public Event HelloWorldCompleted As HelloWorldCompletedEventHandler
        
        '''<remarks/>
        Public Event CheckIMICompleted As CheckIMICompletedEventHandler
        
        '''<remarks/>
        Public Event AboutMeCompleted As AboutMeCompletedEventHandler
        
        '''<remarks/>
        Public Event azWeekCode_FCCompleted As azWeekCode_FCCompletedEventHandler
        
        '''<remarks/>
        Public Event azWeekCodeEx_FCCompleted As azWeekCodeEx_FCCompletedEventHandler
        
        '''<remarks/>
        Public Event UpdateRecordsCompleted As UpdateRecordsCompletedEventHandler
        
        '''<remarks/>
        Public Event GetMarkingCodeCompleted As GetMarkingCodeCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://zulhisham-tan/az_services/HelloWorld", RequestNamespace:="http://zulhisham-tan/az_services/", ResponseNamespace:="http://zulhisham-tan/az_services/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function HelloWorld() As String
            Dim results() As Object = Me.Invoke("HelloWorld", New Object(-1) {})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub HelloWorldAsync()
            Me.HelloWorldAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub HelloWorldAsync(ByVal userState As Object)
            If (Me.HelloWorldOperationCompleted Is Nothing) Then
                Me.HelloWorldOperationCompleted = AddressOf Me.OnHelloWorldOperationCompleted
            End If
            Me.InvokeAsync("HelloWorld", New Object(-1) {}, Me.HelloWorldOperationCompleted, userState)
        End Sub
        
        Private Sub OnHelloWorldOperationCompleted(ByVal arg As Object)
            If (Not (Me.HelloWorldCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent HelloWorldCompleted(Me, New HelloWorldCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://zulhisham-tan/az_services/CheckIMI", RequestNamespace:="http://zulhisham-tan/az_services/", ResponseNamespace:="http://zulhisham-tan/az_services/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function CheckIMI(ByVal IMINo As String) As String
            Dim results() As Object = Me.Invoke("CheckIMI", New Object() {IMINo})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub CheckIMIAsync(ByVal IMINo As String)
            Me.CheckIMIAsync(IMINo, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub CheckIMIAsync(ByVal IMINo As String, ByVal userState As Object)
            If (Me.CheckIMIOperationCompleted Is Nothing) Then
                Me.CheckIMIOperationCompleted = AddressOf Me.OnCheckIMIOperationCompleted
            End If
            Me.InvokeAsync("CheckIMI", New Object() {IMINo}, Me.CheckIMIOperationCompleted, userState)
        End Sub
        
        Private Sub OnCheckIMIOperationCompleted(ByVal arg As Object)
            If (Not (Me.CheckIMICompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent CheckIMICompleted(Me, New CheckIMICompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://zulhisham-tan/az_services/AboutMe", RequestNamespace:="http://zulhisham-tan/az_services/", ResponseNamespace:="http://zulhisham-tan/az_services/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function AboutMe() As String
            Dim results() As Object = Me.Invoke("AboutMe", New Object(-1) {})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub AboutMeAsync()
            Me.AboutMeAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub AboutMeAsync(ByVal userState As Object)
            If (Me.AboutMeOperationCompleted Is Nothing) Then
                Me.AboutMeOperationCompleted = AddressOf Me.OnAboutMeOperationCompleted
            End If
            Me.InvokeAsync("AboutMe", New Object(-1) {}, Me.AboutMeOperationCompleted, userState)
        End Sub
        
        Private Sub OnAboutMeOperationCompleted(ByVal arg As Object)
            If (Not (Me.AboutMeCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent AboutMeCompleted(Me, New AboutMeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://zulhisham-tan/az_services/azWeekCode_FC", RequestNamespace:="http://zulhisham-tan/az_services/", ResponseNamespace:="http://zulhisham-tan/az_services/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function azWeekCode_FC(ByVal sFormat As String) As String
            Dim results() As Object = Me.Invoke("azWeekCode_FC", New Object() {sFormat})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub azWeekCode_FCAsync(ByVal sFormat As String)
            Me.azWeekCode_FCAsync(sFormat, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub azWeekCode_FCAsync(ByVal sFormat As String, ByVal userState As Object)
            If (Me.azWeekCode_FCOperationCompleted Is Nothing) Then
                Me.azWeekCode_FCOperationCompleted = AddressOf Me.OnazWeekCode_FCOperationCompleted
            End If
            Me.InvokeAsync("azWeekCode_FC", New Object() {sFormat}, Me.azWeekCode_FCOperationCompleted, userState)
        End Sub
        
        Private Sub OnazWeekCode_FCOperationCompleted(ByVal arg As Object)
            If (Not (Me.azWeekCode_FCCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent azWeekCode_FCCompleted(Me, New azWeekCode_FCCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://zulhisham-tan/az_services/azWeekCodeEx_FC", RequestNamespace:="http://zulhisham-tan/az_services/", ResponseNamespace:="http://zulhisham-tan/az_services/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function azWeekCodeEx_FC(ByVal SpecNo As String, ByVal sFormat As String) As String
            Dim results() As Object = Me.Invoke("azWeekCodeEx_FC", New Object() {SpecNo, sFormat})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub azWeekCodeEx_FCAsync(ByVal SpecNo As String, ByVal sFormat As String)
            Me.azWeekCodeEx_FCAsync(SpecNo, sFormat, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub azWeekCodeEx_FCAsync(ByVal SpecNo As String, ByVal sFormat As String, ByVal userState As Object)
            If (Me.azWeekCodeEx_FCOperationCompleted Is Nothing) Then
                Me.azWeekCodeEx_FCOperationCompleted = AddressOf Me.OnazWeekCodeEx_FCOperationCompleted
            End If
            Me.InvokeAsync("azWeekCodeEx_FC", New Object() {SpecNo, sFormat}, Me.azWeekCodeEx_FCOperationCompleted, userState)
        End Sub
        
        Private Sub OnazWeekCodeEx_FCOperationCompleted(ByVal arg As Object)
            If (Not (Me.azWeekCodeEx_FCCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent azWeekCodeEx_FCCompleted(Me, New azWeekCodeEx_FCCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://zulhisham-tan/az_services/UpdateRecords", RequestNamespace:="http://zulhisham-tan/az_services/", ResponseNamespace:="http://zulhisham-tan/az_services/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function UpdateRecords(ByVal MarkingRec() As String) As Integer
            Dim results() As Object = Me.Invoke("UpdateRecords", New Object() {MarkingRec})
            Return CType(results(0),Integer)
        End Function
        
        '''<remarks/>
        Public Overloads Sub UpdateRecordsAsync(ByVal MarkingRec() As String)
            Me.UpdateRecordsAsync(MarkingRec, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub UpdateRecordsAsync(ByVal MarkingRec() As String, ByVal userState As Object)
            If (Me.UpdateRecordsOperationCompleted Is Nothing) Then
                Me.UpdateRecordsOperationCompleted = AddressOf Me.OnUpdateRecordsOperationCompleted
            End If
            Me.InvokeAsync("UpdateRecords", New Object() {MarkingRec}, Me.UpdateRecordsOperationCompleted, userState)
        End Sub
        
        Private Sub OnUpdateRecordsOperationCompleted(ByVal arg As Object)
            If (Not (Me.UpdateRecordsCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent UpdateRecordsCompleted(Me, New UpdateRecordsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://zulhisham-tan/az_services/GetMarkingCode", RequestNamespace:="http://zulhisham-tan/az_services/", ResponseNamespace:="http://zulhisham-tan/az_services/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetMarkingCode(ByVal Lot_No As String, ByVal MI_Spec As String, ByRef RetData() As String) As Integer
            Dim results() As Object = Me.Invoke("GetMarkingCode", New Object() {Lot_No, MI_Spec, RetData})
            RetData = CType(results(1),String())
            Return CType(results(0),Integer)
        End Function
        
        '''<remarks/>
        Public Overloads Sub GetMarkingCodeAsync(ByVal Lot_No As String, ByVal MI_Spec As String, ByVal RetData() As String)
            Me.GetMarkingCodeAsync(Lot_No, MI_Spec, RetData, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub GetMarkingCodeAsync(ByVal Lot_No As String, ByVal MI_Spec As String, ByVal RetData() As String, ByVal userState As Object)
            If (Me.GetMarkingCodeOperationCompleted Is Nothing) Then
                Me.GetMarkingCodeOperationCompleted = AddressOf Me.OnGetMarkingCodeOperationCompleted
            End If
            Me.InvokeAsync("GetMarkingCode", New Object() {Lot_No, MI_Spec, RetData}, Me.GetMarkingCodeOperationCompleted, userState)
        End Sub
        
        Private Sub OnGetMarkingCodeOperationCompleted(ByVal arg As Object)
            If (Not (Me.GetMarkingCodeCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent GetMarkingCodeCompleted(Me, New GetMarkingCodeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        Public Shadows Sub CancelAsync(ByVal userState As Object)
            MyBase.CancelAsync(userState)
        End Sub
        
        Private Function IsLocalFileSystemWebService(ByVal url As String) As Boolean
            If ((url Is Nothing)  _
                        OrElse (url Is String.Empty)) Then
                Return false
            End If
            Dim wsUri As System.Uri = New System.Uri(url)
            If ((wsUri.Port >= 1024)  _
                        AndAlso (String.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) = 0)) Then
                Return true
            End If
            Return false
        End Function
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")>  _
    Public Delegate Sub HelloWorldCompletedEventHandler(ByVal sender As Object, ByVal e As HelloWorldCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class HelloWorldCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")>  _
    Public Delegate Sub CheckIMICompletedEventHandler(ByVal sender As Object, ByVal e As CheckIMICompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class CheckIMICompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")>  _
    Public Delegate Sub AboutMeCompletedEventHandler(ByVal sender As Object, ByVal e As AboutMeCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class AboutMeCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")>  _
    Public Delegate Sub azWeekCode_FCCompletedEventHandler(ByVal sender As Object, ByVal e As azWeekCode_FCCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class azWeekCode_FCCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")>  _
    Public Delegate Sub azWeekCodeEx_FCCompletedEventHandler(ByVal sender As Object, ByVal e As azWeekCodeEx_FCCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class azWeekCodeEx_FCCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")>  _
    Public Delegate Sub UpdateRecordsCompletedEventHandler(ByVal sender As Object, ByVal e As UpdateRecordsCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class UpdateRecordsCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Integer
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Integer)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")>  _
    Public Delegate Sub GetMarkingCodeCompletedEventHandler(ByVal sender As Object, ByVal e As GetMarkingCodeCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class GetMarkingCodeCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Integer
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Integer)
            End Get
        End Property
        
        '''<remarks/>
        Public ReadOnly Property RetData() As String()
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(1),String())
            End Get
        End Property
    End Class
End Namespace
