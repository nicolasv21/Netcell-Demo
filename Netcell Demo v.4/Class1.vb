
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports System.Windows.Forms.Design
Imports System.Diagnostics


Namespace CustomControls

    <Designer(GetType(ScrollbarControlDesigner))> _
    Public Class CustomScrollbar
        Inherits UserControl

        Protected moChannelColor As Color = Color.Empty
        Protected moUpArrowImage As Image = Nothing
        'protected Image moUpArrowImage_Over = null;
        'protected Image moUpArrowImage_Down = null;
        Protected moDownArrowImage As Image = Nothing
        'protected Image moDownArrowImage_Over = null;
        'protected Image moDownArrowImage_Down = null;
        Protected moThumbArrowImage As Image = Nothing

        Protected moThumbTopImage As Image = Nothing
        Protected moThumbTopSpanImage As Image = Nothing
        Protected moThumbBottomImage As Image = Nothing
        Protected moThumbBottomSpanImage As Image = Nothing
        Protected moThumbMiddleImage As Image = Nothing

        Protected moLargeChange As Integer = 56
        Protected moSmallChange As Integer = 1
        Protected moMinimum As Integer = 0
        Protected moMaximum As Integer = 560
        Protected moValue As Integer = 0
        Private nClickPoint As Integer

        Protected moThumbTop As Integer = 0

        Protected moAutoSize As Boolean = False

        Private moThumbDown As Boolean = False
        Private moThumbDragging As Boolean = False

        Public Shadows Event Scroll As EventHandler
        Public Event ValueChanged As EventHandler

        Private Function GetThumbHeight() As Integer
            Dim nTrackHeight As Integer = (Me.Height - (UpArrowImage.Height + DownArrowImage.Height))
            Dim fThumbHeight As Single = (CSng(LargeChange) / CSng(Maximum)) * nTrackHeight
            Dim nThumbHeight As Integer = CInt(Math.Truncate(fThumbHeight))

            If nThumbHeight > nTrackHeight Then
                nThumbHeight = nTrackHeight
                fThumbHeight = nTrackHeight
            End If
            If nThumbHeight < 56 Then
                nThumbHeight = 56
                fThumbHeight = 56
            End If

            Return nThumbHeight
        End Function

        Public Sub New()

            InitializeComponent()
            SetStyle(ControlStyles.ResizeRedraw, True)
            SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            SetStyle(ControlStyles.DoubleBuffer, True)

            moChannelColor = Color.FromArgb(51, 166, 3)
            UpArrowImage = My.Resources.uparrow
            DownArrowImage = My.Resources.downarrow


            ThumbBottomImage = My.Resources.ThumbBottom
            ThumbBottomSpanImage = My.Resources.ThumbSpanBottom
            ThumbTopImage = My.Resources.ThumbTop
            ThumbTopSpanImage = My.Resources.ThumbSpanTop
            ThumbMiddleImage = My.Resources.ThumbMiddle

            Me.Width = UpArrowImage.Width
            MyBase.MinimumSize = New Size(UpArrowImage.Width, UpArrowImage.Height + DownArrowImage.Height + GetThumbHeight())
        End Sub

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Behavior"), Description("LargeChange")> _
        Public Property LargeChange() As Integer
            Get
                Return moLargeChange
            End Get
            Set(ByVal value As Integer)
                moLargeChange = value
                Invalidate()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Behavior"), Description("SmallChange")> _
        Public Property SmallChange() As Integer
            Get
                Return moSmallChange
            End Get
            Set(ByVal value As Integer)
                moSmallChange = value
                Invalidate()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Behavior"), Description("Minimum")> _
        Public Property Minimum() As Integer
            Get
                Return moMinimum
            End Get
            Set(ByVal value As Integer)
                moMinimum = value
                Invalidate()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Behavior"), Description("Maximum")> _
        Public Property Maximum() As Integer
            Get
                Return moMaximum
            End Get
            Set(ByVal value As Integer)
                moMaximum = value
                Invalidate()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Behavior"), Description("Value")> _
        Public Property Value() As Integer
            Get
                Return moValue
            End Get
            Set(ByVal value As Integer)
                moValue = value

                Dim nTrackHeight As Integer = (Me.Height - (UpArrowImage.Height + DownArrowImage.Height))
                Dim fThumbHeight As Single = (CSng(LargeChange) / CSng(Maximum)) * nTrackHeight
                Dim nThumbHeight As Integer = CInt(Math.Truncate(fThumbHeight))

                If nThumbHeight > nTrackHeight Then
                    nThumbHeight = nTrackHeight
                    fThumbHeight = nTrackHeight
                End If
                If nThumbHeight < 56 Then
                    nThumbHeight = 56
                    fThumbHeight = 56
                End If

                'figure out value
                Dim nPixelRange As Integer = nTrackHeight - nThumbHeight
                Dim nRealRange As Integer = (Maximum - Minimum) - LargeChange
                Dim fPerc As Single = 0.0F
                If nRealRange <> 0 Then

                    fPerc = CSng(moValue) / CSng(nRealRange)
                End If

                Dim fTop As Single = fPerc * nPixelRange
                moThumbTop = CInt(Math.Truncate(fTop))


                Invalidate()
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Skin"), Description("Channel Color")> _
        Public Property ChannelColor() As Color
            Get
                Return moChannelColor
            End Get
            Set(ByVal value As Color)
                moChannelColor = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Skin"), Description("Up Arrow Graphic")> _
        Public Property UpArrowImage() As Image
            Get
                Return moUpArrowImage
            End Get
            Set(ByVal value As Image)
                moUpArrowImage = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Skin"), Description("Up Arrow Graphic")> _
        Public Property DownArrowImage() As Image
            Get
                Return moDownArrowImage
            End Get
            Set(ByVal value As Image)
                moDownArrowImage = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Skin"), Description("Up Arrow Graphic")> _
        Public Property ThumbTopImage() As Image
            Get
                Return moThumbTopImage
            End Get
            Set(ByVal value As Image)
                moThumbTopImage = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Skin"), Description("Up Arrow Graphic")> _
        Public Property ThumbTopSpanImage() As Image
            Get
                Return moThumbTopSpanImage
            End Get
            Set(ByVal value As Image)
                moThumbTopSpanImage = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Skin"), Description("Up Arrow Graphic")> _
        Public Property ThumbBottomImage() As Image
            Get
                Return moThumbBottomImage
            End Get
            Set(ByVal value As Image)
                moThumbBottomImage = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Skin"), Description("Up Arrow Graphic")> _
        Public Property ThumbBottomSpanImage() As Image
            Get
                Return moThumbBottomSpanImage
            End Get
            Set(ByVal value As Image)
                moThumbBottomSpanImage = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), Browsable(True), DefaultValue(False), Category("Skin"), Description("Up Arrow Graphic")> _
        Public Property ThumbMiddleImage() As Image
            Get
                Return moThumbMiddleImage
            End Get
            Set(ByVal value As Image)
                moThumbMiddleImage = value
            End Set
        End Property

        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor

            If UpArrowImage IsNot Nothing Then
                e.Graphics.DrawImage(UpArrowImage, New Rectangle(New Point(0, 0), New Size(Me.Width, UpArrowImage.Height)))
            End If

            Dim oBrush As Brush = New SolidBrush(moChannelColor)
            Dim oWhiteBrush As Brush = New SolidBrush(Color.FromArgb(255, 255, 255))

            'draw channel left and right border colors
            e.Graphics.FillRectangle(oWhiteBrush, New Rectangle(0, UpArrowImage.Height, 1, (Me.Height - DownArrowImage.Height)))
            e.Graphics.FillRectangle(oWhiteBrush, New Rectangle(Me.Width - 1, UpArrowImage.Height, 1, (Me.Height - DownArrowImage.Height)))

            'draw channel
            e.Graphics.FillRectangle(oBrush, New Rectangle(1, UpArrowImage.Height, Me.Width - 2, (Me.Height - DownArrowImage.Height)))

            'draw thumb
            Dim nTrackHeight As Integer = (Me.Height - (UpArrowImage.Height + DownArrowImage.Height))
            Dim fThumbHeight As Single = (CSng(LargeChange) / CSng(Maximum)) * nTrackHeight
            Dim nThumbHeight As Integer = CInt(Math.Truncate(fThumbHeight))

            If nThumbHeight > nTrackHeight Then
                nThumbHeight = nTrackHeight
                fThumbHeight = nTrackHeight
            End If
            If nThumbHeight < 56 Then
                nThumbHeight = 56
                fThumbHeight = 56
            End If

            'Debug.WriteLine(nThumbHeight.ToString());

            Dim fSpanHeight As Single = (fThumbHeight - (ThumbMiddleImage.Height + ThumbTopImage.Height + ThumbBottomImage.Height)) / 2.0F
            Dim nSpanHeight As Integer = CInt(Math.Truncate(fSpanHeight))

            Dim nTop As Integer = moThumbTop
            nTop += UpArrowImage.Height

            'draw top
            e.Graphics.DrawImage(ThumbTopImage, New Rectangle(1, nTop, Me.Width - 2, ThumbTopImage.Height))

            nTop += ThumbTopImage.Height
            'draw top span
            Dim rect As New Rectangle(1, nTop, Me.Width - 2, nSpanHeight)


            e.Graphics.DrawImage(ThumbTopSpanImage, 1.0F, CSng(nTop), CSng(Me.Width) - 2.0F, CSng(fSpanHeight) * 2)

            nTop += nSpanHeight
            'draw middle
            e.Graphics.DrawImage(ThumbMiddleImage, New Rectangle(1, nTop, Me.Width - 2, ThumbMiddleImage.Height))


            nTop += ThumbMiddleImage.Height
            'draw top span
            rect = New Rectangle(1, nTop, Me.Width - 2, nSpanHeight * 2)
            e.Graphics.DrawImage(ThumbBottomSpanImage, rect)

            nTop += nSpanHeight
            'draw bottom
            e.Graphics.DrawImage(ThumbBottomImage, New Rectangle(1, nTop, Me.Width - 2, nSpanHeight))

            If DownArrowImage IsNot Nothing Then
                e.Graphics.DrawImage(DownArrowImage, New Rectangle(New Point(0, (Me.Height - DownArrowImage.Height)), New Size(Me.Width, DownArrowImage.Height)))
            End If

        End Sub

        Public Overrides Property AutoSize() As Boolean
            Get
                Return MyBase.AutoSize
            End Get
            Set(ByVal value As Boolean)
                MyBase.AutoSize = value
                If MyBase.AutoSize Then
                    Me.Width = moUpArrowImage.Width
                End If
            End Set
        End Property

        Private Sub InitializeComponent()
            Me.SuspendLayout()
            ' 
            ' CustomScrollbar
            ' 
            Me.Name = "CustomScrollbar"
            AddHandler Me.Load, New System.EventHandler(AddressOf Me.CustomScrollbar_Load)
            AddHandler Me.MouseDown, New System.Windows.Forms.MouseEventHandler(AddressOf Me.CustomScrollbar_MouseDown)
            AddHandler Me.MouseMove, New System.Windows.Forms.MouseEventHandler(AddressOf Me.CustomScrollbar_MouseMove)
            AddHandler Me.MouseUp, New System.Windows.Forms.MouseEventHandler(AddressOf Me.CustomScrollbar_MouseUp)
            Me.ResumeLayout(False)

        End Sub

        Private Sub CustomScrollbar_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
            Dim ptPoint As Point = Me.PointToClient(Cursor.Position)
            Dim nTrackHeight As Integer = (Me.Height - (UpArrowImage.Height + DownArrowImage.Height))
            Dim fThumbHeight As Single = (CSng(LargeChange) / CSng(Maximum)) * nTrackHeight
            Dim nThumbHeight As Integer = CInt(Math.Truncate(fThumbHeight))

            If nThumbHeight > nTrackHeight Then
                nThumbHeight = nTrackHeight
                fThumbHeight = nTrackHeight
            End If
            If nThumbHeight < 56 Then
                nThumbHeight = 56
                fThumbHeight = 56
            End If

            Dim nTop As Integer = moThumbTop
            nTop += UpArrowImage.Height


            Dim thumbrect As New Rectangle(New Point(1, nTop), New Size(ThumbMiddleImage.Width, nThumbHeight))
            If thumbrect.Contains(ptPoint) Then

                'hit the thumb
                nClickPoint = (ptPoint.Y - nTop)
                'MessageBox.Show(Convert.ToString((ptPoint.Y - nTop)));
                Me.moThumbDown = True
            End If

            Dim uparrowrect As New Rectangle(New Point(1, 0), New Size(UpArrowImage.Width, UpArrowImage.Height))
            If uparrowrect.Contains(ptPoint) Then

                Dim nRealRange As Integer = (Maximum - Minimum) - LargeChange
                Dim nPixelRange As Integer = (nTrackHeight - nThumbHeight)
                If nRealRange > 0 Then
                    If nPixelRange > 0 Then
                        If (moThumbTop - SmallChange) < 0 Then
                            moThumbTop = 0
                        Else
                            moThumbTop -= SmallChange
                        End If

                        'figure out value
                        Dim fPerc As Single = CSng(moThumbTop) / CSng(nPixelRange)
                        Dim fValue As Single = fPerc * (Maximum - LargeChange)

                        moValue = CInt(Math.Truncate(fValue))
                        Debug.WriteLine(moValue.ToString())

                        RaiseEvent ValueChanged(Me, New EventArgs())

                        RaiseEvent Scroll(Me, New EventArgs())

                        Invalidate()
                    End If
                End If
            End If

            Dim downarrowrect As New Rectangle(New Point(1, UpArrowImage.Height + nTrackHeight), New Size(UpArrowImage.Width, UpArrowImage.Height))
            If downarrowrect.Contains(ptPoint) Then
                Dim nRealRange As Integer = (Maximum - Minimum) - LargeChange
                Dim nPixelRange As Integer = (nTrackHeight - nThumbHeight)
                If nRealRange > 0 Then
                    If nPixelRange > 0 Then
                        If (moThumbTop + SmallChange) > nPixelRange Then
                            moThumbTop = nPixelRange
                        Else
                            moThumbTop += SmallChange
                        End If

                        'figure out value
                        Dim fPerc As Single = CSng(moThumbTop) / CSng(nPixelRange)
                        Dim fValue As Single = fPerc * (Maximum - LargeChange)

                        moValue = CInt(Math.Truncate(fValue))
                        Debug.WriteLine(moValue.ToString())

                        RaiseEvent ValueChanged(Me, New EventArgs())

                        RaiseEvent Scroll(Me, New EventArgs())

                        Invalidate()
                    End If
                End If
            End If
        End Sub

        Private Sub CustomScrollbar_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
            Me.moThumbDown = False
            Me.moThumbDragging = False
        End Sub

        Private Sub MoveThumb(ByVal y As Integer)
            Dim nRealRange As Integer = Maximum - Minimum
            Dim nTrackHeight As Integer = (Me.Height - (UpArrowImage.Height + DownArrowImage.Height))
            Dim fThumbHeight As Single = (CSng(LargeChange) / CSng(Maximum)) * nTrackHeight
            Dim nThumbHeight As Integer = CInt(Math.Truncate(fThumbHeight))

            If nThumbHeight > nTrackHeight Then
                nThumbHeight = nTrackHeight
                fThumbHeight = nTrackHeight
            End If
            If nThumbHeight < 56 Then
                nThumbHeight = 56
                fThumbHeight = 56
            End If

            Dim nSpot As Integer = nClickPoint

            Dim nPixelRange As Integer = (nTrackHeight - nThumbHeight)
            If moThumbDown AndAlso nRealRange > 0 Then
                If nPixelRange > 0 Then
                    Dim nNewThumbTop As Integer = y - (UpArrowImage.Height + nSpot)

                    If nNewThumbTop < 0 Then
                        moThumbTop = InlineAssignHelper(nNewThumbTop, 0)
                    ElseIf nNewThumbTop > nPixelRange Then
                        moThumbTop = InlineAssignHelper(nNewThumbTop, nPixelRange)
                    Else
                        moThumbTop = y - (UpArrowImage.Height + nSpot)
                    End If

                    'figure out value
                    Dim fPerc As Single = CSng(moThumbTop) / CSng(nPixelRange)
                    Dim fValue As Single = fPerc * (Maximum - LargeChange)
                    moValue = CInt(Math.Truncate(fValue))
                    Debug.WriteLine(moValue.ToString())

                    Application.DoEvents()

                    Invalidate()
                End If
            End If
        End Sub

        Private Sub CustomScrollbar_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
            If moThumbDown = True Then
                Me.moThumbDragging = True
            End If

            If Me.moThumbDragging Then

                MoveThumb(e.Y)
            End If

            RaiseEvent ValueChanged(Me, New EventArgs())

            RaiseEvent Scroll(Me, New EventArgs())
        End Sub

        Private Sub CustomScrollbar_Load(ByVal sender As Object, ByVal e As EventArgs)

        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function

    End Class

    Friend Class ScrollbarControlDesigner
        Inherits System.Windows.Forms.Design.ControlDesigner



        Public Overrides ReadOnly Property SelectionRules() As SelectionRules
            Get
                Dim selectionRules__1 As SelectionRules = MyBase.SelectionRules
                Dim propDescriptor As PropertyDescriptor = TypeDescriptor.GetProperties(Me.Component)("AutoSize")
                If propDescriptor IsNot Nothing Then
                    Dim autoSize As Boolean = CBool(propDescriptor.GetValue(Me.Component))
                    If autoSize Then
                        selectionRules__1 = SelectionRules.Visible Or SelectionRules.Moveable Or SelectionRules.BottomSizeable Or SelectionRules.TopSizeable
                    Else
                        selectionRules__1 = SelectionRules.Visible Or SelectionRules.AllSizeable Or SelectionRules.Moveable
                    End If
                End If
                Return selectionRules__1
            End Get
        End Property
    End Class
End Namespace

