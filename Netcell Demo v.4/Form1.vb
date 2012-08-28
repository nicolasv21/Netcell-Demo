Imports System.Drawing
Imports OtiIcaoSDK
Imports OtiIcaoSDK.OtiIcao
Imports System.IO
Imports GrFingerXLib
Imports System.Data
Imports System.Data.SQLite
Imports System.Drawing.Printing



Public Class Form1
    Dim MyDataGridViewPrinter As DataGridViewPrinter
    Dim CONNECTION_STR As String = "Data Source=c:\Netcell\DB\CedulasDB.db;Version=3;"
    Private PersonalData As PersonalData = Nothing
    Dim ventana_regresar As Panel
    Dim ventana_actual As Panel
    Dim personalID As String = String.Empty
    Private start_time As DateTime
    Private stop_time As DateTime
    Dim elapsed_time As TimeSpan
    Private WSQ_library_native_methods As WSQImageLibraryNativeMethods
    Dim myUtil As Util
    Dim hbitmap As IntPtr
    Dim tableiniatilize As Boolean = False
    Dim huellagemalto() As Byte = New Byte() {0}
    Dim messageText As String = String.Empty
    Dim icaoAuthentityResult As IcaoAuthentityResult = icaoAuthentityResult.PassedAll
    Dim firstName As String = String.Empty
    Dim lastName As String = String.Empty
    Dim dateOfBirth As String = String.Empty
    Dim validUntil As String = String.Empty
    Dim nationality As String = String.Empty
    Dim sex As String = String.Empty
    Dim documentNumber As String = String.Empty
    Dim firstFingerIcaoIndex As Integer = -1
    Dim secondFingerIcaoIndex As Integer = -1
    Dim firstFingerTemplate As Byte() = Nothing
    Dim secondFingerTemplate As Byte() = Nothing
    Dim photo As Byte() = Nothing
    Dim fingerIcao As Byte() = Nothing
    Dim signature As Byte() = Nothing
    Dim chipId As Byte() = Nothing
    Dim dateOfBirth_11 As String = String.Empty
    Dim personalID_11 As String = String.Empty
    Dim checkDigit1 As String = String.Empty
    Dim checkDigit2 As String = String.Empty
    Dim checkDigit3 As String = String.Empty
    Dim checkDigit4 As String = String.Empty
    Dim documentCode As String = String.Empty
    Dim issuingState As String = String.Empty
    Dim holderName As String = String.Empty
    Dim holderName_11 As String = String.Empty
    Dim placeOfBirth As String = String.Empty
    Dim telephone As String = String.Empty
    Dim profession As String = String.Empty
    Dim address As String = String.Empty

    Private Function MostrarError(ByRef messageText As String)
        If messageText = "Archivo OTI ID no se encuentra. Por favor, suministrar ID Personal e intentar de nuevo." Then
            abrir(id_file)
            Return False
        ElseIf messageText = "OTI Saturn reader communication service: OTI_SATURN_CANNOT_FIND_CARD (C0000002)" & vbCrLf & "Cannot find card." Then
            MessageBox.Show("Chip No Valido!")
        ElseIf messageText = "OTI ID file not found. Please, supply Personal ID and try again." Then
            MessageBox.Show("Ingrese el numero de cédula!")
        ElseIf messageText = "OTI Saturn reader communication service: OTI_SATURN_SELECT_CHANNEL_ERROR (C0000004)" & vbCrLf & "Cannot select channel: Contactless." Then
            Application.Restart()
        ElseIf messageText = "OTI Saturn reader communication service: OTI_SATURN_CANNOT_FIND_READER (C0000000)" & vbCrLf & "Cannot find OTI Saturn reader." Then
            Application.Restart()
        ElseIf messageText = "APDU TLV files processor service: OTI_TLVAPDU_ICAO_ERROR_CANNOT_OPEN_SEC_CHNL (C0000003)" & vbCrLf & "Cannot open secure channel: APDU [BAC Mutual Authentication] Wrong response: 6300." Then
            abrir(id_file)
            Return False
        ElseIf messageText = "APDU TLV files processor service: OTI_TLVAPDU_ICAO_ERROR_CANNOT_OPEN_SEC_CHNL (C0000003)" & vbCrLf & "Cannot open secure channel: APDU [BAC Mutual Authentication] Wrong response: 6982." Then
            abrir(id_file)
            Return False
        ElseIf messageText = "OTI ECUID PICC processor service: OTI_TLVAPDU_APPLICATION_ERROR (FFFFFFFF)" & vbCrLf & "La matriz de origen no es suficientemente larga. Compruebe srcIndex, la longitud y los límites inferiores de la matriz." Then
            abrir(id_file)
            Return False
        ElseIf messageText = "OTI ECUID PICC processor service: OTI_TLVAPDU_APPLICATION_ERROR (FFFFFFFF)" & vbCrLf & "Object reference not set to an instance of an object." Then
            Application.Restart()
        End If
        Return True
    End Function
    Private Function botones(Optional ByVal btn1 As Boolean = False, Optional ByVal btn1_text As String = "", Optional ByVal btn2 As Boolean = False, Optional ByVal btn2_text As String = "", Optional ByVal btn3 As Boolean = False, Optional ByVal btn3_text As String = "", Optional ByVal btn4 As Boolean = False, Optional ByVal btn4_text As String = "")
        btn_1.Visible = False

        btn_3.Visible = False
        btn_4.Visible = False
        If btn1 = True Then
            btn_1.Text = btn1_text
            btn_1.Visible = True
        End If

        If btn3 = True Then
            btn_3.Text = btn3_text
            btn_3.Visible = True
        End If
        If btn4 = True Then
            btn_4.Text = btn4_text
            btn_4.Visible = True
        End If
        Return True
    End Function

    Private Function abrir(Optional ByRef Ventana As Panel = Nothing)
        Lectura.Visible = False  ' Ventana de leyendo chip.
        Header.Visible = False   ' Ventana de solicitud de cedula en el lector.
        id_file.Visible = False  ' Ventana de solicitud de ID FILE.
        Error_1.Visible = False  ' Ventana de error de conexion del lector de cédulas.
        Datos.Visible = False    ' Ventana que muestra todos los datos del chip.
        fail.Visible = False     ' Ventana de error de chip id.
        Report.Visible = False     ' Ventana de Reporte.
        imprimir.Visible = False
        Reporte_interno.Visible = False
        ventana_regresar = ventana_actual

        Ventana.Visible = True

        If ventana_actual.Name <> Ventana.Name Or ventana_actual.Name <> "Lectura" Then

            Ventana.Dock = DockStyle.Fill
            If Ventana.Name = "Error_1" Then
                botones(True, "ACTIVAR", True, "CERRAR")
                lbl_footer_1.Visible = False
                lbl_footer_2.Visible = False
            End If
            If Ventana.Name = "Header" Then
                botones(True, "LEER CEDULA", True, "CERRAR")
                Me.lbl_footer_1.Text = "10 Segundos"
                Me.lbl_footer_2.Text = "TIEMPO APROXIMADO DE LECTURA"
                lbl_footer_1.Visible = True
                lbl_footer_2.Visible = True
                SplitContainer1.Panel1Collapsed = True
                SplitContainer1.Panel2Collapsed = False
            End If
            If Ventana.Name = "Lectura" Then
                botones()
                lbl_footer_1.Visible = False
                lbl_footer_2.Visible = False
                start_time = Now
                leer_chip()
                stop_time = Now
                elapsed_time = stop_time.Subtract(start_time)
                Me.lbl_footer_1.Text = elapsed_time.TotalSeconds.ToString("0.000") & " Segundos"
                Me.lbl_footer_2.Text = "TIEMPO DE LECTURA"
            End If
            If Ventana.Name = "Datos" Then
                botones(True, "LEER CEDULA", True, "LIMPIAR", True, "REGRESAR", True, "GUARDAR")
                lbl_footer_1.Visible = True
                lbl_footer_2.Visible = True
                personalID = ""
                personal_id.Text = ""
            End If
            If Ventana.Name = "fail" Then
                botones(True, "LEER CEDULA", True, "REGRESAR", True, "CERRAR")
                lbl_footer_1.Visible = False
                lbl_footer_2.Visible = False
            End If
            If Ventana.Name = "id_file" Then
                botones(True, "LEER CEDULA", True, "REGRESAR", True, "CERRAR")
                lbl_footer_1.Visible = False
                lbl_footer_2.Visible = False
            End If
            If Ventana.Name = "Report" Then
                botones(True, "REGRESAR", True, "CERRAR")
                lbl_footer_1.Visible = False
                lbl_footer_2.Visible = False
            End If
            If Ventana.Name = "imprimir" Then
                botones(True, "REGRESAR", True, "CERRAR")
                lbl_footer_1.Visible = False
                lbl_footer_2.Visible = False
            End If
            If Ventana.Name = "Reporte_interno" Then
                botones(True, "REGRESAR", True, "CERRAR")
                lbl_footer_1.Visible = False
                lbl_footer_2.Visible = False
            End If
            Return True
            ventana_actual = Ventana
        End If
        Return False


    End Function
    Private Function activar_lector()
        Dim message As String = String.Empty
        If Not Me.OtiIcao1.ActivateReaders(message) Then
            abrir(Error_1)
            Return False
        Else
            abrir(Header)
            AddHandler Me.OtiIcao1.OnFileStartProcess, New FileStartProcessEventHandler(AddressOf Me.otiIcao1_OnFileStartProcess)
            AddHandler Me.OtiIcao1.OnFileBlockProcessed, New FileBlockProcessedEventHandler(AddressOf Me.otiIcao1_OnFileBlockProcessed)
            Return True
        End If
    End Function
    Private Function leer_chip()
        If verificar_validez() Then

            personalID = personal_id.Text
            Dim cantidad As Integer
            Dim op1, op2, op3 As Short
            cantidad = 0

            If r1.Checked = True Then
                cantidad = cantidad + 1
                If op1 = 0 Then
                    op1 = 2
                ElseIf op2 = 0 Then
                    op2 = 2
                ElseIf op3 = 0 Then
                    op3 = 2
                End If
            End If

            If r2.Checked = True Then
                cantidad = cantidad + 1
                If op1 = 0 Then
                    op1 = 8
                ElseIf op2 = 0 Then
                    op2 = 8
                ElseIf op3 = 0 Then
                    op3 = 8
                End If
            End If
            If r3.Checked = True Then
                cantidad = cantidad + 1
                If op1 = 0 Then
                    op1 = 4
                ElseIf op2 = 0 Then
                    op2 = 4
                ElseIf op3 = 0 Then
                    op3 = 4
                End If
            End If

            If r4.Checked = True Or r5.Checked = True Or r6.Checked = True Or r7.Checked = True Or r8.Checked = True Or r9.Checked = True Or r10.Checked = True Or r11.Checked = True Or r12.Checked = True Or r13.Checked = True Or r14.Checked = True Or r15.Checked = True Or r16.Checked = True Then
                cantidad = cantidad + 1
                If op1 = 0 Then
                    op1 = 1
                ElseIf op2 = 0 Then
                    op2 = 1
                ElseIf op3 = 0 Then
                    op3 = 1
                End If
            End If

            Try

                If cantidad = 1 Then
                    If Not Me.OtiIcao1.ReadIcaoData(op1, (personalID), firstName, lastName, nationality, sex, documentNumber, photo, dateOfBirth, validUntil, fingerIcao, firstFingerIcaoIndex, secondFingerIcaoIndex, firstFingerTemplate, secondFingerTemplate, signature, dateOfBirth_11, personalID_11, checkDigit1, checkDigit2, checkDigit3, checkDigit4, documentCode, issuingState, holderName, holderName_11, placeOfBirth, telephone, profession, address, chipId, icaoAuthentityResult, messageText) Then
                        MostrarError(messageText)
                        Return False
                        Exit Function
                    End If
                End If
                If cantidad = 2 Then
                    If Not Me.OtiIcao1.ReadIcaoData(op1 Or op2, (personalID), firstName, lastName, nationality, sex, documentNumber, photo, dateOfBirth, validUntil, fingerIcao, firstFingerIcaoIndex, secondFingerIcaoIndex, firstFingerTemplate, secondFingerTemplate, signature, dateOfBirth_11, personalID_11, checkDigit1, checkDigit2, checkDigit3, checkDigit4, documentCode, issuingState, holderName, holderName_11, placeOfBirth, telephone, profession, address, chipId, icaoAuthentityResult, messageText) Then
                        MostrarError(messageText)
                        Return False
                        Exit Function
                    End If
                End If
                If cantidad = 3 Then
                    If Not Me.OtiIcao1.ReadIcaoData(op1 Or op2 Or op3, (personalID), firstName, lastName, nationality, sex, documentNumber, photo, dateOfBirth, validUntil, fingerIcao, firstFingerIcaoIndex, secondFingerIcaoIndex, firstFingerTemplate, secondFingerTemplate, signature, dateOfBirth_11, personalID_11, checkDigit1, checkDigit2, checkDigit3, checkDigit4, documentCode, issuingState, holderName, holderName_11, placeOfBirth, telephone, profession, address, chipId, icaoAuthentityResult, messageText) Then
                        MostrarError(messageText)
                        Return False
                        Exit Function
                    End If
                End If
                If cantidad = 4 Then
                    If Not Me.OtiIcao1.ReadIcaoData(0, (personalID), firstName, lastName, nationality, sex, documentNumber, photo, dateOfBirth, validUntil, fingerIcao, firstFingerIcaoIndex, secondFingerIcaoIndex, firstFingerTemplate, secondFingerTemplate, signature, dateOfBirth_11, personalID_11, checkDigit1, checkDigit2, checkDigit3, checkDigit4, documentCode, issuingState, holderName, holderName_11, placeOfBirth, telephone, profession, address, chipId, icaoAuthentityResult, messageText) Then
                        MostrarError(messageText)
                        Return False
                        Exit Function
                    End If
                End If

                Me.btn_4.Enabled = True
                abrir(Datos)


                If (icaoAuthentityResult <> icaoAuthentityResult.PassedAll) Then
                    ' MessageBox.Show(messageText)
                    certificado.Visible = True
                    certificado_txt.Visible = True
                Else
                    certificado.Visible = True
                    certificado_txt.Visible = True
                End If
                If firstFingerIcaoIndex = 1 Or firstFingerIcaoIndex = 3 Or firstFingerIcaoIndex = 5 Or firstFingerIcaoIndex = 7 Or firstFingerIcaoIndex = 9 Then
                    d_mano.Text = "Mano Derecha"
                Else
                    d_mano.Text = "Mano Izquierda"
                End If
                If firstFingerIcaoIndex = 1 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d1
                End If
                If firstFingerIcaoIndex = 2 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d2
                End If
                If firstFingerIcaoIndex = 3 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d3
                End If
                If firstFingerIcaoIndex = 4 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d4
                End If
                If firstFingerIcaoIndex = 5 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d5
                End If
                If firstFingerIcaoIndex = 6 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d6
                End If
                If firstFingerIcaoIndex = 7 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d7
                End If
                If firstFingerIcaoIndex = 8 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d8
                End If
                If firstFingerIcaoIndex = 9 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d9
                End If
                If firstFingerIcaoIndex = 10 Then
                    d_dedo.Image = Netcell_Demo_v._4.My.Resources.d10
                End If
                Me.d_nombres.Text = firstName
                Me.d_apellidos.Text = lastName
                Me.d_nacionalidad.Text = nationality
                Me.d_fecha_nac.Text = dateOfBirth
                Me.d_sexo.Text = sex
                Me.d_numero_plastico.Text = documentNumber
                Me.d_cedula.Text = personalID_11
                Me.d_fecha_expiracion.Text = validUntil
                Me.d_lugar_nac.Text = placeOfBirth
                Me.d_telefono.Text = telephone
                Me.d_profesion.Text = profession
                Me.d_direccion.Text = address
                Me.d_numero_chip.Text = BitConverter.ToString(chipId)

                If Not photo Is Nothing Then
                    Using ms = New MemoryStream(photo, 0, photo.Length)
                        ms.Write(photo, 0, photo.Length)
                        Me.d_foto.Image = Image.FromStream(ms, True)
                    End Using
                End If

                If Not signature Is Nothing Then
                    Using ms As New MemoryStream(signature, 0, signature.Length)
                        ms.Write(signature, 0, signature.Length)
                        Me.d_firma.Image = Image.FromStream(ms, True)
                    End Using
                End If
                If (Not fingerIcao Is Nothing) Then
                    ' hbitmap = WSQ_library_native_methods.CreateBMPFromWSQByteArray(fingerIcao, fingerIcao.Length)
                    '  Me.d_huella.Image = Bitmap.FromHbitmap(hbitmap)
                    ' Dim image2 As Image = Me.d_huella.Image
                    ' image2.Save("c:\NETCELL\huella.bmp", System.Drawing.Imaging.ImageFormat.Bmp)
                    extraer = True
                    iden = False
                    Dim resolution As Integer
                    resolution = 500
                    If (resolution <> 0) Then
                        If AxGrFingerXCtrl1.CapLoadImageFromFile("c:\NETCELL\huella.bmp", resolution) <> GRConstants.GR_OK Then
                            ' myUtil.WriteLog("Fail to load the file.")
                        End If
                    End If
                    myUtil.DB.clearDB()
                    Me.Timer2.Enabled = True
                Else
                    Try
                        Dim input As New FileStream("C:\Netcell\Working\blob_2_PersData.bin", FileMode.Open)
                        Dim reader As New BinaryReader(input)

                        huellagemalto = reader.ReadBytes(CInt(input.Length))
                        hbitmap = WSQ_library_native_methods.CreateBMPFromWSQByteArray(huellagemalto, huellagemalto.Length)
                        Me.d_huella.Image = Bitmap.FromHbitmap(hbitmap)
                        input.Close()
                        Dim image2 As Image = Me.d_huella.Image
                        image2.Save("c:\NETCELL\huella.bmp", System.Drawing.Imaging.ImageFormat.Bmp)
                        extraer = True
                        iden = False
                        Dim resolution As Integer
                        resolution = 500
                        If (resolution <> 0) Then
                            If AxGrFingerXCtrl1.CapLoadImageFromFile("c:\NETCELL\huella.bmp", resolution) <> GRConstants.GR_OK Then
                                ' myUtil.WriteLog("Fail to load the file.")
                            End If
                        End If
                        myUtil.DB.clearDB()
                        Me.Timer2.Enabled = True
                    Catch ex As Exception

                    End Try

                End If



                'Me.btn_4.Enabled = True
                ' abrir(Datos)

            Catch ex As Exception


            End Try

        End If
        Return True
    End Function
    Private Function verificar_validez()
        Dim chipLocked As Boolean = False
        Dim messageText As String = String.Empty
        Dim chipId As Byte() = Nothing
        If personal_id.Text <> "" Then
            personalID = personal_id.Text
        End If
        If Not Me.OtiIcao1.ReadChipId(personalID, chipLocked, chipId, messageText) Then
            If messageText = "OTI Saturn reader communication service: OTI_SATURN_CANNOT_FIND_CARD (C0000002)" & vbCrLf & "Cannot find card." Then
                abrir(fail)
                Return False
            End If
            If messageText = "OTI Saturn reader communication service: OTI_SATURN_CANNOT_FIND_READER (C0000000)" & vbCrLf & "Cannot find OTI Saturn reader." Then
                abrir(Error_1)
                Return False
            End If
            If messageText = "OTI ECUID PICC processor service: OTI_TLVAPDU_APPLICATION_ERROR (FFFFFFFF)" & vbCrLf & "La matriz de origen no es suficientemente larga. Compruebe srcIndex, la longitud y los límites inferiores de la matriz." Then
                Return True
            End If
            If messageText = "APDU TLV files processor service: OTI_TLVAPDU_ICAO_ERROR_CANNOT_OPEN_SEC_CHNL (C0000003)" & vbCrLf & "Cannot open secure channel: APDU [BAC Mutual Authentication] Wrong response: 6300." Then
                abrir(id_file)
                id_file_error.Text = "El número de cédula no coincide."
                id_file_error.Visible = True
                Return False
            End If
            MsgBox(messageText)
            Return False
        Else
            Return True
        End If
    End Function
    Private Function activar_biometrico()
        WSQ_library_native_methods = New WSQImageLibraryNativeMethods
        Dim err As Integer
        myUtil = New Util(loglist, Me.d_sensor, AxGrFingerXCtrl1)
        err = myUtil.InitializeGrFinger()
        If err < 0 Then
            'myUtil.WriteError(err)
            Return False
            Exit Function
        Else
            Return True
            ' myUtil.WriteLog("**GrFingerX Initialized Successfull**")
        End If
    End Function
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'CedulasDBDataSet.cedulas' table. You can move, or remove it, as needed.
        Me.cedulasTableAdapter.Fill(Me.CedulasDBDataSet.cedulas)
        'TODO: This line of code loads data into the 'CedulasDBDataSet.cedulas' table. You can move, or remove it, as needed.

        ventana_actual = Header
        activar_lector()
        activar_biometrico()
        Me.ReportViewer1.RefreshReport()
        tabla_settings()
        Me.ReportViewer2.RefreshReport()
    End Sub
    Private Sub otiIcao1_OnFileBlockProcessed()
        Application.DoEvents()
    End Sub
    Private Sub otiIcao1_OnFileStartProcess(ByVal dg_tag As Byte, ByVal block_count As Integer)
        Application.DoEvents()
    End Sub
    Private Sub leer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If btn_1.Text = "ACTIVAR" Then
            activar_lector()
            Exit Sub
        End If
        If btn_1.Text = "LEER CEDULA" Then

        End If
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)




    End Sub
    Private Sub btn_3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If btn_3.Text = "CERRAR" Then
            MyBase.Close()
            Application.Exit()
        End If
        If btn_3.Text = "REGRESAR" Then
            If ventana_actual.Name = "Datos" Then
                abrir(Header)
            Else
                ' abrir(Me.ventana_regresar)
            End If
        End If

    End Sub
    Private Function Identify()
        Dim ret As Integer, score As Integer

        score = 0
        ret = myUtil.Identify(score)

        Me.v_6.Text = Now.ToString("G")
        Me.v_3.Text = "Puntaje: " + CStr(score)
        If ret > 0 Then
            Me.v_1.Visible = True
            Me.v_2.Visible = True
            Me.v_3.Visible = True
            Me.v_4.Visible = False
            Me.v_5.Visible = True
            Me.v_6.Visible = True
            Me.v_sello.Visible = True
            myUtil.PrintBiometricDisplay(True, GRConstants.GR_DEFAULT_CONTEXT)
            Me.v_1.ForeColor = Color.Green
            Me.v_2.ForeColor = Color.Green
            Me.v_3.ForeColor = Color.Green
            Me.v_5.ForeColor = Color.Green
            Me.v_6.ForeColor = Color.Green
            Me.v_1.Text = "Huella Coincide"
            Me.v_sello.Image = Netcell_Demo_v._4.My.Resources.Resources.ok
        ElseIf ret = 0 Then
            Me.v_1.Visible = True
            Me.v_2.Visible = True
            Me.v_3.Visible = True
            Me.v_5.Visible = True
            Me.v_6.Visible = True
            Me.v_4.Visible = False
            Me.v_sello.Visible = True
            Me.v_1.ForeColor = Color.DarkRed
            Me.v_2.ForeColor = Color.DarkRed
            Me.v_3.ForeColor = Color.DarkRed
            Me.v_5.ForeColor = Color.DarkRed
            Me.v_6.ForeColor = Color.DarkRed
            Me.v_1.Text = "Huella No Coincide"
            Me.v_sello.Image = Netcell_Demo_v._4.My.Resources.Resources.cancel
        Else
            'myUtil.WriteError(ret)
        End If

        Return True
    End Function
    Private Function Extract()


        Dim ret As Integer
        ' extract template

        ret = myUtil.ExtractTemplate()
        ' write template quality to log
        If ret = GRConstants.GR_BAD_QUALITY Then
            Me.v_2.Text = "Calidad Baja"
        ElseIf ret = GRConstants.GR_MEDIUM_QUALITY Then
            Me.v_2.Text = "Calidad Mediana"
        ElseIf ret = GRConstants.GR_HIGH_QUALITY Then
            Me.v_2.Text = "Calidad Alta"
        End If

        If ret >= 0 Then

            ' if no error, display minutiae/segments/directions into the image
            myUtil.PrintBiometricDisplay(True, GRConstants.GR_NO_CONTEXT)
        Else
            ' write error to log
            'myUtil.WriteError(ret)
        End If
        Return True
    End Function






    Private Sub AxGrFingerXCtrl1_ImageAcquired1(ByVal sender As Object, ByVal e As AxGrFingerXLib._IGrFingerXCtrlEvents_ImageAcquiredEvent) Handles AxGrFingerXCtrl1.ImageAcquired
        ' Copying aquired image
        myUtil.raw.height = e.height
        myUtil.raw.width = e.width
        myUtil.raw.res = e.res
        myUtil.raw.img = e.rawImage
        ' Signaling that an Image Event occurred.
        'myUtil.WriteLog("Sensor: " & e.idSensor & ". Event: Image captured.")
        ' display fingerprint image
        myUtil.PrintBiometricDisplay(False, GRConstants.GR_DEFAULT_CONTEXT)
        ' extracting template from image
        If extraer Then Extract()
        ' identify fingerprint
        If iden Then Identify()
        GC.Collect()
    End Sub

    Private Sub AxGrFingerXCtrl1_SensorPlug_1(ByVal sender As System.Object, ByVal e As AxGrFingerXLib._IGrFingerXCtrlEvents_SensorPlugEvent) Handles AxGrFingerXCtrl1.SensorPlug
        AxGrFingerXCtrl1.CapStartCapture(e.idSensor)
    End Sub

    Private Sub AxGrFingerXCtrl1_SensorUnplug1(ByVal sender As Object, ByVal e As AxGrFingerXLib._IGrFingerXCtrlEvents_SensorUnplugEvent) Handles AxGrFingerXCtrl1.SensorUnplug
        AxGrFingerXCtrl1.CapStopCapture(e.idSensor)
    End Sub

    Private Sub Datos_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Datos.Paint

    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)


    End Sub

    Private Sub CreateDatabase()
        Dim objConn As SQLiteConnection
        Dim objCommand As SQLiteCommand
        Try
            objConn = New SQLiteConnection(CONNECTION_STR & "New=True;")
            objConn.Open()
            objCommand = objConn.CreateCommand()
            objCommand.CommandText = "CREATE TABLE cedulas (id integer primary key, cedula varchar(10), nombres TEXT, apellidos TEXT, nacionalidad TEXT, sexo varchar(2), lugar_nacimiento TEXT, fecha_nacimiento TEXT, profesion TEXT, telefono TEXT,direccion TEXT,fecha_expiracion TEXT,numero_plastico TEXT,numero_chip TEXT);"
            objCommand.ExecuteNonQuery()
        Finally
            If Not IsNothing(objConn) Then
                objConn.Close()
            End If
        End Try
    End Sub
    Private Sub SaveRecord()
        Dim objConn As SQLiteConnection
        Dim objCommand As SQLiteCommand
        Try
            objConn = New SQLiteConnection(CONNECTION_STR)
            objConn.Open()
            objCommand = objConn.CreateCommand()
            objCommand.CommandText = "INSERT INTO cedulas (cedula, nombres , apellidos,nacionalidad,sexo,lugar_nacimiento,fecha_nacimiento,profesion,telefono,direccion,fecha_expiracion,numero_plastico,numero_chip,foto,firma,huella) VALUES ('" & d_cedula.Text & "','" & d_nombres.Text & "','" & d_apellidos.Text & "','" & d_nacionalidad.Text & "','" & d_sexo.Text & "','" & d_lugar_nac.Text & "','" & d_fecha_nac.Text & "','" & d_profesion.Text & "','" & d_telefono.Text & "','" & d_direccion.Text & "','" & d_fecha_expiracion.Text & "','" & d_numero_plastico.Text & "','" & d_numero_chip.Text & "',@photo,@firma,@huella);"
            objCommand.Parameters.Add("@photo", DbType.Binary, 20).Value = photo
            If (huellagemalto.Length > 1) Then
                objCommand.Parameters.Add("@huella", DbType.Binary, 20).Value = huellagemalto
            Else
                objCommand.Parameters.Add("@huella", DbType.Binary, 20).Value = fingerIcao
            End If
            objCommand.Parameters.Add("@firma", DbType.Binary, 20).Value = signature
            objCommand.ExecuteNonQuery()
        Finally
            If Not IsNothing(objConn) Then
                objConn.Close()
            End If
        End Try
    End Sub

    Private Sub ReadData()
        Dim objConn As SQLiteConnection
        Dim objCommand As SQLiteCommand
        Dim objReader As SQLiteDataReader
        Try
            objConn = New SQLiteConnection(CONNECTION_STR)
            objConn.Open()
            objCommand = objConn.CreateCommand()
            objCommand.CommandText = "SELECT * FROM customer"
            objReader = objCommand.ExecuteReader()
            ' lstCustomers.Items.Clear()
            While (objReader.Read())
                'lstCustomers.Items.Add(objReader("name"))
            End While
        Catch ex As Exception
            MessageBox.Show("An error has occurred: " & ex.Message)
        Finally
            If Not IsNothing(objConn) Then
                objConn.Close()
            End If
        End Try

    End Sub

    Private Sub btn_4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            SaveRecord()
            MessageBox.Show("Data Saved successfully")
        Catch ex As Exception
            MessageBox.Show("An error occurred saving data: " & ex.Message)
        End Try

    End Sub

    Private Sub Button1_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ReportViewer1.Reset()


        Dim objConn As SQLiteConnection
        Dim objCommand As SQLiteCommand

        objConn = New SQLiteConnection(CONNECTION_STR)
        objConn.Open()
        objCommand = objConn.CreateCommand()
        objCommand.CommandText = "SELECT * FROM cedulas"



        Dim SQLiteDataAdapter = New SQLiteDataAdapter(objCommand)
        Dim DataSet = New DataSet()
        Dim DataTable = New DataTable()
        SQLiteDataAdapter.Fill(DataSet)
        DataTable = DataSet.Tables(0)

        objConn.Close()
        Dim ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource
        ReportDataSource.Name = "Report1"
        ReportDataSource.Value = DataTable


        ReportViewer1.LocalReport.DataSources.Clear()
        ReportViewer1.LocalReport.DataSources.Add(ReportDataSource)
        ReportViewer1.RefreshReport()

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        CreateDatabase()
        SaveRecord()
    End Sub

    Private Sub Header_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Header.Paint

    End Sub

    Private Sub Button1_Click_3(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button1_Click_4(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub cedulasBindingSource_CurrentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cedulasBindingSource.CurrentChanged

    End Sub

    Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Label67_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Panel8_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)

    End Sub

    Private Sub Panel4_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel4.Paint

    End Sub

    Private Sub Button1_Click_5(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        abrir(imprimir)
    End Sub

    Private Sub Button2_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        abrir(Reporte_interno)
    End Sub

    Private Sub btn_4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_4.Click
        SaveRecord()
    End Sub

    Private Sub btn_3_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_3.Click

    End Sub

    Private Sub btn_1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_1.Click
        If ventana_actual.Name = "id_file" Then
            If personal_id.Text.Length < 10 Then
                id_file_error.Text = "El número de cédula no tiene 10 caracteres"
                id_file_error.Visible = True
            Else
                id_file_error.Visible = False
                abrir(Lectura)
            End If
            Exit Sub
        End If
        abrir(Lectura)
        Exit Sub
    End Sub

    Private Sub Label67_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)


    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button5_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub txt_cedula_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_cedula.GotFocus
        getfocus(txt_cedula)
    End Sub
    Private Sub txt_cedula_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_cedula.LostFocus
        lestfocus(txt_cedula)
    End Sub
    Private Sub txt_cedula_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_cedula.TextChanged
        buscar()
    End Sub

    Private Sub txt_nombres_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_nombres.GotFocus
        getfocus(txt_nombres)
    End Sub
    Private Sub txt_nombres_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_nombres.LostFocus
        lestfocus(txt_nombres)
    End Sub
    Private Sub txt_nombres_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_nombres.TextChanged
        buscar()
    End Sub

    Private Sub txt_apellidos_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_apellidos.GotFocus
        getfocus(txt_apellidos)
    End Sub
    Private Sub txt_apellidos_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_apellidos.LostFocus
        lestfocus(txt_apellidos)
    End Sub
    Private Sub txt_apellidos_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_apellidos.TextChanged
        buscar()
    End Sub

    Private Sub txt_nacionalidad_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_nacionalidad.GotFocus
        getfocus(txt_nacionalidad)
    End Sub
    Private Sub txt_nacionalidad_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_nacionalidad.LostFocus
        lestfocus(txt_nacionalidad)
    End Sub
    Private Sub txt_nacionalidad_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_nacionalidad.TextChanged
        buscar()
    End Sub

    Private Sub txt_sexo_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_sexo.GotFocus
        getfocus(txt_sexo)
    End Sub
    Private Sub txt_sexo_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_sexo.LostFocus
        lestfocus(txt_sexo)
    End Sub
    Private Sub txt_sexo_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_sexo.TextChanged
        buscar()
    End Sub

    Private Sub txt_lugar_nacimiento_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_lugar_nacimiento.GotFocus
        getfocus(txt_lugar_nacimiento)
    End Sub
    Private Sub txt_lugar_nacimiento_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_lugar_nacimiento.LostFocus
        lestfocus(txt_lugar_nacimiento)
    End Sub
    Private Sub txt_lugar_nacimiento_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_lugar_nacimiento.TextChanged
        buscar()
    End Sub

    Private Sub txt_fecha_nacimiento_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_fecha_nacimiento.GotFocus
        getfocus(txt_fecha_nacimiento)
    End Sub
    Private Sub txt_fecha_nacimiento_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_fecha_nacimiento.LostFocus
        lestfocus(txt_fecha_nacimiento)
    End Sub
    Private Sub txt_fecha_nacimiento_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_fecha_nacimiento.TextChanged
        buscar()
    End Sub

    Private Sub txt_profesion_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_profesion.GotFocus
        getfocus(txt_profesion)
    End Sub
    Private Sub txt_profesion_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_profesion.LostFocus
        lestfocus(txt_profesion)
    End Sub
    Private Sub txt_profesion_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_profesion.TextChanged
        buscar()
    End Sub

    Private Sub txt_telefono_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_telefono.GotFocus
        getfocus(txt_telefono)
    End Sub
    Private Sub txt_telefono_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_telefono.LostFocus
        lestfocus(txt_telefono)
    End Sub
    Private Sub txt_telefono_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_telefono.TextChanged
        buscar()
    End Sub

    Private Sub txt_direccion_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_direccion.GotFocus
        getfocus(txt_direccion)
    End Sub
    Private Sub txt_direccion_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_direccion.LostFocus
        lestfocus(txt_direccion)
    End Sub
    Private Sub txt_direccion_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_direccion.TextChanged
        buscar()
    End Sub

    Private Sub txt_fecha_expiracion_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_fecha_expiracion.GotFocus
        getfocus(txt_fecha_expiracion)
    End Sub
    Private Sub txt_fecha_expiracion_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_fecha_expiracion.LostFocus
        lestfocus(txt_fecha_expiracion)
    End Sub
    Private Sub txt_fecha_expiracion_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_fecha_expiracion.TextChanged
        buscar()
    End Sub

    Private Sub txt_numero_plastico_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_numero_plastico.GotFocus
        getfocus(txt_numero_plastico)
    End Sub
    Private Sub txt_numero_plastico_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_numero_plastico.LostFocus
        lestfocus(txt_numero_plastico)
    End Sub
    Private Sub txt_numero_plastico_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_numero_plastico.TextChanged
        buscar()
    End Sub

    Private Sub txt_numero_chip_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_numero_chip.GotFocus
        getfocus(txt_numero_chip)
    End Sub
    Private Sub txt_numero_chip_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_numero_chip.LostFocus
        lestfocus(txt_numero_chip)
    End Sub
    Private Sub txt_numero_chip_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt_numero_chip.TextChanged
        buscar()
    End Sub

    Function getfocus(ByVal textbox_name As TextBox)
        If textbox_name.Text = textbox_name.Tag Then
            textbox_name.ForeColor = Color.Black
            textbox_name.Text = ""
        End If
        Return True
    End Function
    Function lestfocus(ByVal textbox_name As TextBox)
        If textbox_name.Text = Nothing Then
            textbox_name.ForeColor = Color.Gray
            textbox_name.Text = textbox_name.Tag
        End If
        Return True
    End Function
    Function buscar()
        Dim cedula_param As String = "%" & txt_cedula.Text & "%"
        Dim nombres_param As String = "%" & txt_nombres.Text & "%"
        Dim apellidos_param As String = "%" & txt_apellidos.Text & "%"
        Dim nacionalidad_param As String = "%" & txt_nacionalidad.Text & "%"
        Dim sexo_param As String = "%" & txt_sexo.Text & "%"
        Dim lugar_nacimiento_param As String = "%" & txt_lugar_nacimiento.Text & "%"
        Dim fecha_nacimiento_param As String = "%" & txt_fecha_nacimiento.Text & "%"
        Dim profesion_param As String = "%" & txt_profesion.Text & "%"
        Dim telefono_param As String = "%" & txt_telefono.Text & "%"
        Dim direccion_param As String = "%" & txt_direccion.Text & "%"
        Dim fecha_expiracion_param As String = "%" & txt_fecha_expiracion.Text & "%"
        Dim numero_plastico_param As String = "%" & txt_numero_plastico.Text & "%"
        Dim numero_chip_param As String = "%" & txt_numero_chip.Text & "%"
        If cedula_param = "%" & txt_cedula.Tag & "%" Then cedula_param = "%%"
        If nombres_param = "%" & txt_nombres.Tag & "%" Then nombres_param = "%%"
        If apellidos_param = "%" & txt_apellidos.Tag & "%" Then apellidos_param = "%%"
        If nacionalidad_param = "%" & txt_nacionalidad.Tag & "%" Then nacionalidad_param = "%%"
        If sexo_param = "%" & txt_sexo.Tag & "%" Then sexo_param = "%%"
        If lugar_nacimiento_param = "%" & txt_lugar_nacimiento.Tag & "%" Then lugar_nacimiento_param = "%%"
        If fecha_nacimiento_param = "%" & txt_fecha_nacimiento.Tag & "%" Then fecha_nacimiento_param = "%%"
        If profesion_param = "%" & txt_profesion.Tag & "%" Then profesion_param = "%%"
        If telefono_param = "%" & txt_telefono.Tag & "%" Then telefono_param = "%%"
        If direccion_param = "%" & txt_direccion.Tag & "%" Then direccion_param = "%%"
        If fecha_expiracion_param = "%" & txt_fecha_expiracion.Tag & "%" Then fecha_expiracion_param = "%%"
        If numero_plastico_param = "%" & txt_numero_plastico.Tag & "%" Then numero_plastico_param = "%%"
        If numero_chip_param = "%" & txt_numero_chip.Tag & "%" Then numero_chip_param = "%%"
        Me.cedulasTableAdapter.Buscar(Me.CedulasDBDataSet.cedulas, cedula_param, nombres_param, apellidos_param, nacionalidad_param, sexo_param, lugar_nacimiento_param, fecha_nacimiento_param, profesion_param, telefono_param, direccion_param, fecha_expiracion_param, numero_plastico_param, numero_chip_param)
        Return True
    End Function

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        abrir(ventana_regresar)
    End Sub

    Private Sub Button4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Derecha.Visible = False
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Derecha.Visible = False
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Derecha.Visible = True
        If SplitContainer1.Panel2Collapsed = False Then
            SplitContainer1.Panel2Collapsed = True
            SplitContainer1.Panel1Collapsed = False
            Imprimir_ventanas.Panel1Collapsed = True
            Imprimir_ventanas.Panel2Collapsed = False
        Else
            Imprimir_ventanas.Panel1Collapsed = True
            Imprimir_ventanas.Panel2Collapsed = False
        End If
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Application.Exit()
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        myUtil.ExtractTemplate()
        Dim id As Integer
        id = myUtil.Enroll()
        Me.d_sensor.Image = Nothing
        Me.d_sensor.Update()


        ' write result to log
        If id >= 0 Then
            'myUtil.WriteLog("Fingerprint enrolled with id = " & id)
        Else
            'myUtil.WriteLog("Error: Fingerprint not enrolled")
        End If
        extraer = True
        iden = True
        Me.Timer2.Enabled = False
    End Sub


    Private Sub Label82_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label82.Click

    End Sub

    Private Sub Button5_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Derecha.Visible = True
        If SplitContainer1.Panel2Collapsed = False Then
            SplitContainer1.Panel2Collapsed = True
            SplitContainer1.Panel1Collapsed = False
            Imprimir_ventanas.Panel2Collapsed = True
            Imprimir_ventanas.Panel1Collapsed = False
        Else
            Imprimir_ventanas.Panel2Collapsed = True
            Imprimir_ventanas.Panel1Collapsed = False
        End If
    End Sub



    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)


        ' Me.cedulasTableAdapter.Adapter.SelectCommand.CommandText = "Select * from cedulas"
        'Me.cedulasTableAdapter.Fill(Me.CedulasDBDataSet.cedulas)


    End Sub

    Private Sub a1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a1.CheckedChanged
        If a1.Checked = False Then
            DataGridView1.Columns.Item(1).Visible = False
            My.Settings.Tabla_cedula = 0
        Else
            DataGridView1.Columns.Item(1).Visible = True
            My.Settings.Tabla_cedula = 1
        End If
    End Sub

    Private Sub a2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a2.CheckedChanged
        If a2.Checked = False Then
            DataGridView1.Columns.Item(2).Visible = False
            My.Settings.Tabla_nombres = 0
        Else
            DataGridView1.Columns.Item(2).Visible = True
            My.Settings.Tabla_nombres = 1
        End If
    End Sub
    Private Sub a3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a3.CheckedChanged
        If a3.Checked = False Then
            DataGridView1.Columns.Item(3).Visible = False
            My.Settings.Tabla_apellidos = 0
        Else
            DataGridView1.Columns.Item(3).Visible = True
            My.Settings.Tabla_apellidos = 1
        End If
    End Sub
    Private Sub a4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a4.CheckedChanged
        If a4.Checked = False Then
            DataGridView1.Columns.Item(4).Visible = False
            My.Settings.Tabla_nacionalidad = 0
        Else
            DataGridView1.Columns.Item(4).Visible = True
            My.Settings.Tabla_nacionalidad = 1
        End If
    End Sub
    Private Sub a5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a5.CheckedChanged
        If a5.Checked = False Then
            DataGridView1.Columns.Item(5).Visible = False
            My.Settings.Tabla_sexo = 0
        Else
            DataGridView1.Columns.Item(5).Visible = True
            My.Settings.Tabla_sexo = 1
        End If
    End Sub
    Private Sub a6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a6.CheckedChanged
        If a6.Checked = False Then
            DataGridView1.Columns.Item(6).Visible = False
            My.Settings.Tabla_lugar_nacimiento = 0
        Else
            DataGridView1.Columns.Item(6).Visible = True
            My.Settings.Tabla_lugar_nacimiento = 1
        End If
    End Sub
    Private Sub a7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a7.CheckedChanged
        If a7.Checked = False Then
            DataGridView1.Columns.Item(7).Visible = False
            My.Settings.Tabla_fecha_nacimiento = 0
        Else
            DataGridView1.Columns.Item(7).Visible = True
            My.Settings.Tabla_fecha_nacimiento = 1
        End If
    End Sub
    Private Sub a8_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a8.CheckedChanged
        If a8.Checked = False Then
            DataGridView1.Columns.Item(8).Visible = False
            My.Settings.Tabla_profesion = 0
        Else
            DataGridView1.Columns.Item(8).Visible = True
            My.Settings.Tabla_profesion = 1
        End If
    End Sub
    Private Sub a9_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a9.CheckedChanged
        If a9.Checked = False Then
            DataGridView1.Columns.Item(9).Visible = False
            My.Settings.Tabla_telefono = 0
        Else
            DataGridView1.Columns.Item(9).Visible = True
            My.Settings.Tabla_telefono = 1
        End If
    End Sub
    Private Sub a10_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a10.CheckedChanged
        If a10.Checked = False Then
            DataGridView1.Columns.Item(10).Visible = False
            My.Settings.Tabla_direccion = 0
        Else
            DataGridView1.Columns.Item(10).Visible = True
            My.Settings.Tabla_direccion = 1
        End If
    End Sub
    Private Sub a11_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a11.CheckedChanged
        If a11.Checked = False Then
            DataGridView1.Columns.Item(11).Visible = False
            My.Settings.Tabla_fecha_expiracion = 0
        Else
            DataGridView1.Columns.Item(11).Visible = True
            My.Settings.Tabla_fecha_expiracion = 1
        End If
    End Sub
    Private Sub a12_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a12.CheckedChanged
        If a12.Checked = False Then
            DataGridView1.Columns.Item(12).Visible = False
            My.Settings.Tabla_numero_plastico = 0
        Else
            DataGridView1.Columns.Item(12).Visible = True
            My.Settings.Tabla_numero_plastico = 1
        End If
    End Sub
    Private Sub a13_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles a13.CheckedChanged
        If a13.Checked = False Then
            DataGridView1.Columns.Item(13).Visible = False
            My.Settings.Tabla_numero_chip = 0
        Else
            DataGridView1.Columns.Item(13).Visible = True
            My.Settings.Tabla_numero_chip = 1
        End If
    End Sub
    Function tabla_settings()
        If My.Settings.Tabla_cedula = 1 Then a1.Checked = True Else a1.Checked = False
        If My.Settings.Tabla_nombres = 1 Then a2.Checked = True Else a2.Checked = False
        If My.Settings.Tabla_apellidos = 1 Then a3.Checked = True Else a3.Checked = False
        If My.Settings.Tabla_nacionalidad = 1 Then a4.Checked = True Else a4.Checked = False
        If My.Settings.Tabla_sexo = 1 Then a5.Checked = True Else a5.Checked = False
        If My.Settings.Tabla_lugar_nacimiento = 1 Then a6.Checked = True Else a6.Checked = False
        If My.Settings.Tabla_fecha_nacimiento = 1 Then a7.Checked = True Else a7.Checked = False
        If My.Settings.Tabla_profesion = 1 Then a8.Checked = True Else a8.Checked = False
        If My.Settings.Tabla_telefono = 1 Then a9.Checked = True Else a9.Checked = False
        If My.Settings.Tabla_direccion = 1 Then a10.Checked = True Else a10.Checked = False
        If My.Settings.Tabla_fecha_expiracion = 1 Then a11.Checked = True Else a11.Checked = False
        If My.Settings.Tabla_numero_plastico = 1 Then a12.Checked = True Else a12.Checked = False
        If My.Settings.Tabla_numero_chip = 1 Then a13.Checked = True Else a13.Checked = False
        DataGridView1.Columns.Item(1).Width = My.Settings.Tabla_cedula_ancho
        DataGridView1.Columns.Item(2).Width = My.Settings.Tabla_nombres_ancho
        DataGridView1.Columns.Item(3).Width = My.Settings.Tabla_apellidos_ancho
        DataGridView1.Columns.Item(4).Width = My.Settings.Tabla_nacionalidad_ancho
        DataGridView1.Columns.Item(5).Width = My.Settings.Tabla_sexo_ancho
        DataGridView1.Columns.Item(6).Width = My.Settings.Tabla_lugar_nacimiento_ancho
        DataGridView1.Columns.Item(7).Width = My.Settings.Tabla_fecha_nacimiento_ancho
        DataGridView1.Columns.Item(8).Width = My.Settings.Tabla_profesion_ancho
        DataGridView1.Columns.Item(9).Width = My.Settings.Tabla_telefono_ancho
        DataGridView1.Columns.Item(10).Width = My.Settings.Tabla_direccion_ancho
        DataGridView1.Columns.Item(11).Width = My.Settings.Tabla_fecha_expiracion_ancho
        DataGridView1.Columns.Item(12).Width = My.Settings.Tabla_numero_plastico_ancho
        DataGridView1.Columns.Item(13).Width = My.Settings.Tabla_numero_chip_ancho

        tableiniatilize = True
    End Function



    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub DataGridView1_ColumnWidthChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewColumnEventArgs) Handles DataGridView1.ColumnWidthChanged
        If tableiniatilize = True Then
            If e.Column.Index = 1 Then My.Settings.Tabla_cedula_ancho = e.Column.Width
            If e.Column.Index = 2 Then My.Settings.Tabla_nombres_ancho = e.Column.Width
            If e.Column.Index = 3 Then My.Settings.Tabla_apellidos_ancho = e.Column.Width
            If e.Column.Index = 4 Then My.Settings.Tabla_nacionalidad_ancho = e.Column.Width
            If e.Column.Index = 5 Then My.Settings.Tabla_sexo_ancho = e.Column.Width
            If e.Column.Index = 6 Then My.Settings.Tabla_lugar_nacimiento_ancho = e.Column.Width
            If e.Column.Index = 7 Then My.Settings.Tabla_fecha_nacimiento_ancho = e.Column.Width
            If e.Column.Index = 8 Then My.Settings.Tabla_profesion_ancho = e.Column.Width
            If e.Column.Index = 9 Then My.Settings.Tabla_telefono_ancho = e.Column.Width
            If e.Column.Index = 10 Then My.Settings.Tabla_direccion_ancho = e.Column.Width
            If e.Column.Index = 11 Then My.Settings.Tabla_fecha_expiracion_ancho = e.Column.Width
            If e.Column.Index = 12 Then My.Settings.Tabla_numero_plastico_ancho = e.Column.Width
            If e.Column.Index = 13 Then My.Settings.Tabla_numero_chip_ancho = e.Column.Width
        End If
    End Sub


    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If SetupThePrinting() Then PrintDocument1.Print()
    End Sub

    Private Sub PrintDocument1_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Dim more As Boolean

        Try
            more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics)
            If more Then e.HasMorePages = True
        Catch Ex As Exception
            MessageBox.Show(Ex.Message & vbCrLf & Ex.StackTrace, "prueba", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function SetupThePrinting() As Boolean
        Dim MyPrintDialog As PrintDialog = New PrintDialog()

        MyPrintDialog.AllowCurrentPage = False
        MyPrintDialog.AllowPrintToFile = False
        MyPrintDialog.AllowSelection = False
        MyPrintDialog.AllowSomePages = True
        MyPrintDialog.PrintToFile = False
        MyPrintDialog.ShowHelp = False
        MyPrintDialog.ShowNetwork = False
        MyPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = True
        If MyPrintDialog.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then Return False

        PrintDocument1.DocumentName = "REPORTE / Listado"
        PrintDocument1.PrinterSettings = MyPrintDialog.PrinterSettings
        PrintDocument1.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings
        PrintDocument1.DefaultPageSettings.Margins = New Margins(40, 40, 40, 40)

        If MessageBox.Show("Do you want the report to be centered on the page", "InvoiceManager - Center on Page", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            MyDataGridViewPrinter = New DataGridViewPrinter(DataGridView1, PrintDocument1, True, True, "REPORTE / Listado", New Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, True)
        Else
            MyDataGridViewPrinter = New DataGridViewPrinter(DataGridView1, PrintDocument1, False, True, "REPORTE / Listado", New Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, True)
        End If

        Return True
    End Function
  
    Private Sub Button11_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        If SetupThePrinting() Then
            Dim MyPrintPreviewDialog As PrintPreviewDialog = New PrintPreviewDialog()
            MyPrintPreviewDialog.Document = PrintDocument1
            MyPrintPreviewDialog.ShowDialog()
        End If
    End Sub

    Private Sub Label80_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label80.Click

    End Sub

    Private Sub Button8_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click

    End Sub

    Private Sub Label74_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label74.Click

    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        abrir(Report)
    End Sub
End Class
