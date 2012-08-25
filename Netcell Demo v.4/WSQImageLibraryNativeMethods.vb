Imports System.Text
Imports System.IO
Imports System.Runtime.InteropServices


Public Class WSQImageLibraryNativeMethods

    Dim outODataStream As IntPtr = IntPtr.Zero, outCommentText As IntPtr = IntPtr.Zero
    Dim outCompressBuffer As IntPtr = IntPtr.Zero
    Dim bitmap_ptr As IntPtr = IntPtr.Zero


    Private utf8 As New UTF8Encoding



    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Sub _RegisterWSQ()
    End Sub
    Public Sub RegisterWSQ()
        Try
            _RegisterWSQ()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "RegisterWSQ Exception")
        End Try
    End Sub


    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Function _CreateBMPFromFile(ByVal lpszFileName As Byte()) As IntPtr
    End Function
    Public Function CreateBMPFromFile(ByVal FileName As String) As IntPtr

        If (FileName = Nothing) Then
            Throw (New ArgumentNullException("FileName should be not null"))
        ElseIf (FileName.Length = 0) Then
            Throw New ArgumentException("Incorrect file name")
        End If
        Try
            bitmap_ptr = _CreateBMPFromFile(utf8.GetBytes(FileName))
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "CreateBMPFromFile Exception")
        End Try
        CreateBMPFromFile = bitmap_ptr
    End Function


    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Function _SaveBMPToFile(ByVal hBitmap As IntPtr, _
            ByVal filename As Byte(), <MarshalAs(UnmanagedType.I4)> ByVal filetype As Integer) As Integer
    End Function
    Public Function SaveBMPToFile(ByVal Bitmap As IntPtr, ByVal FileName As String, ByVal FileType As Integer) As Integer
        Dim retVal As Integer = -1
        If (IsNothing(Bitmap) Or (FileName = Nothing)) Then
            Throw New ArgumentNullException("Bitmap and FileName should be not null")
        End If
        If (FileName.Length = 0) Then
            Throw New ArgumentException("Incorrect file name")
        End If
        Try
            retVal = _SaveBMPToFile(Bitmap, utf8.GetBytes(FileName), FileType)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "SaveBMPToFile Exception")
        End Try
        SaveBMPToFile = retVal
    End Function



    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Function _CreateBMPFromWSQByteArray( _
        <[In]()> ByVal input_data_stream As Byte(), _
        <MarshalAs(UnmanagedType.I4)> ByVal input_stream_length As Integer) As IntPtr
    End Function

    Public Function CreateBMPFromWSQByteArray(<[In]()> ByVal input_data_stream As Byte(), _
              <[In]()> ByVal input_stream_length As Integer) As IntPtr
        Try
            bitmap_ptr = _CreateBMPFromWSQByteArray(input_data_stream, input_stream_length)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "CreateBMPFromWSQByteArray Exception")
        End Try

        CreateBMPFromWSQByteArray = bitmap_ptr

    End Function


    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Function _SaveWSQByteArrayToImageFile( _
        <[In]()> ByVal input_data_stream As Byte(), _
        <MarshalAs(UnmanagedType.I4)> ByVal input_stream_length As Integer, _
        ByVal lpszFileName As Byte(), _
        <MarshalAs(UnmanagedType.I4)> ByVal filetype As Integer) As Integer
    End Function
    Public Function SaveWSQByteArrayToImageFile(<[In]()> ByVal input_data_stream As Byte(), _
              <[In]()> ByVal input_stream_length As Integer, ByVal FileName As String, ByVal FileType As Integer) As Integer

        Dim retVal As Integer = -1

        Try
            retVal = _SaveWSQByteArrayToImageFile(input_data_stream, input_stream_length, _
                    utf8.GetBytes(FileName), FileType)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "SaveWSQByteArrayToImageFile Exception")
        End Try

        SaveWSQByteArrayToImageFile = retVal

    End Function


    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Sub _SetShowFilePropertiesDialog( _
            <MarshalAs(UnmanagedType.I4)> ByVal file_properties_dialog As Integer)
    End Sub
    Public Sub SetShowFilePropertiesDialog(ByVal file_properties_dialog As Integer)
        Try
            _SetShowFilePropertiesDialog(file_properties_dialog)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "SetShowFilePropertiesDialog Exception")
        End Try
    End Sub





    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Sub _ShowFileConverter()
    End Sub
    Public Sub ShowFileConverter()
        Try
            _ShowFileConverter()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "ShowFileConverter Exception")
        End Try
    End Sub





    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Function _WSQ_decode_stream( _
        <[In]()> ByVal input_data_stream As Byte(), _
        <MarshalAs(UnmanagedType.I4)> ByVal input_stream_length As Integer, _
        ByRef output_data_stream As IntPtr, _
        <MarshalAs(UnmanagedType.I4)> ByRef width As Integer, _
        <MarshalAs(UnmanagedType.I4)> ByRef height As Integer, _
        <MarshalAs(UnmanagedType.I4)> ByRef ppi As Integer, _
        ByRef comment_text As IntPtr) As Integer
    End Function
    Public Function WSQ_decode_stream(<[In]()> ByVal input_data_stream As Byte(), _
                <[In]()> ByVal input_stream_length As Int32, _
                ByRef output_data_stream As Byte(), _
                ByRef width As Int32, _
                ByRef height As Int32, _
                ByRef ppi As Int32, _
                ByRef comment_text As String) As Integer

        Dim retVal As Integer = -1
        'output_data_stream = new byte[1];
        width = height = ppi = 0
        comment_text = String.Empty


        Try
            _WSQ_decode_stream(input_data_stream, input_stream_length, _
                    outODataStream, width, height, _
                    ppi, outCommentText)
            comment_text = Marshal.PtrToStringAnsi(outCommentText)
            Dim length As Integer = width * height
            ReDim output_data_stream(length)
            Marshal.Copy(outODataStream, output_data_stream, 0, length)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "WSQ_decode_stream Exception")
        End Try

        WSQ_decode_stream = retVal

    End Function



    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Function _WSQ_encode_stream( _
        <[In]()> ByVal inputBuffer As Byte(), _
        <MarshalAs(UnmanagedType.I4)> ByVal width As Integer, _
        <MarshalAs(UnmanagedType.I4)> ByVal height As Integer, _
        <MarshalAs(UnmanagedType.R8)> ByVal bitrate As Double, _
        <MarshalAs(UnmanagedType.I4)> ByVal ppi As Integer, _
        <[In]()> ByVal comment_text As Byte(), _
        ByRef compressBuffer As IntPtr, _
        <MarshalAs(UnmanagedType.I4)> ByRef outputStreamLength As Integer) As Integer
    End Function
    Public Function WSQ_encode_stream( _
        ByVal inputBuffer As Byte(), _
        ByVal width As Integer, _
        ByVal height As Integer, _
        ByVal bitrate As Double, _
        ByVal ppi As Integer, _
        ByVal commentText As String, _
        ByRef compressBuffer As Byte(), _
        ByRef outputStreamLength As Integer) As Integer

        Dim retVal As Integer = -1


        compressBuffer = Nothing
        outputStreamLength = 0

        Try
            _WSQ_encode_stream(inputBuffer, width, height, _
                    bitrate, ppi, utf8.GetBytes(commentText), _
                    outCompressBuffer, outputStreamLength)
            If (outputStreamLength = 0) Then
                Throw New ArgumentException("outputStreamLength == 0")
            End If
            ReDim compressBuffer(outputStreamLength)
            Marshal.Copy(outCompressBuffer, compressBuffer, 0, outputStreamLength)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "WSQ_encode_stream Exception")
        End Try

        WSQ_encode_stream = retVal

    End Function


    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Function _ConvertHBITMAPtoGrayScale256(ByVal hBitmap As IntPtr) As IntPtr
    End Function
    Public Function ConvertHBITMAPtoGrayScale256(ByVal Bitmap As IntPtr) As IntPtr
        Try
            bitmap_ptr = _ConvertHBITMAPtoGrayScale256(Bitmap)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "ConvertHBITMAPtoGrayScale256 Exception")
        End Try
        ConvertHBITMAPtoGrayScale256 = bitmap_ptr
    End Function


    <DllImport("WSQ_library.dll", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)> Private Shared Function _SaveHBITMAPtoFileAsGrayScale256BMP(ByVal hBitmap As IntPtr, _
            ByVal filename As Byte()) As Integer
    End Function
    Public Function SaveHBITMAPtoFileAsGrayScale256BMP(ByVal Bitmap As IntPtr, ByVal FileName As String) As Integer
        Dim retVal As Integer = -1
        If (IsNothing(Bitmap) Or (FileName = Nothing)) Then
            Throw New ArgumentNullException("Bitmap and FileName should be not null")
        End If
        If (FileName.Length = 0) Then
            Throw New ArgumentException("Incorrect file name")
        End If
        Try
            retVal = _SaveHBITMAPtoFileAsGrayScale256BMP(Bitmap, utf8.GetBytes(FileName))
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly, "SaveHBITMAPtoFileAsGrayScale256BMP Exception")
        End Try
        SaveHBITMAPtoFileAsGrayScale256BMP = retVal
    End Function



End Class