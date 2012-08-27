Follow the Salan Al-Ani's instructions!
The DatagridViewPrinter class is in the DatagridViewPrinter.vb file.
Here is the source code you must add to your project (I hope I have not forgotten something...):

    Private Function SetupThePrinting() As Boolean
        Dim MyPrintDialog As PrintDialog = New PrintDialog()

        MyPrintDialog.AllowCurrentPage = False
        MyPrintDialog.AllowPrintToFile = False
        MyPrintDialog.AllowSelection = False
        MyPrintDialog.AllowSomePages = True
        MyPrintDialog.PrintToFile = False
        MyPrintDialog.ShowHelp = False
        MyPrintDialog.ShowNetwork = False

        If MyPrintDialog.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then Return False

        MyPrintDocument.DocumentName = "Customers Report"
        MyPrintDocument.PrinterSettings = MyPrintDialog.PrinterSettings
        MyPrintDocument.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings
        MyPrintDocument.DefaultPageSettings.Margins = New Margins(40, 40, 40, 40)

       if MessageBox.Show("Do you want the report to be centered on the page", "InvoiceManager - Center on Page", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes then
           MyDataGridViewPrinter = new DataGridViewPrinter(MyDataGridView, MyPrintDocument, true, true, "Customers", new Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, true)
       else
           MyDataGridViewPrinter = new DataGridViewPrinter(MyDataGridView, MyPrintDocument, false, true, "Customers", new Font("Tahoma", 18, FontStyle.Bold, GraphicsUnit.Point), Color.Black, true)
       end if

       Return True
    End Function

    Private Sub MyPrintDocument_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles MyPrintDocument.PrintPage
        Dim more As Boolean

        Try
            more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics)
            If more Then e.HasMorePages = True
        Catch Ex As Exception
            MessageBox.Show(Ex.Message & vbCrLf & Ex.StackTrace, MyConstants.CaptionFehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' The Print Preview Button
    Private Sub btnPrintPreview_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintPreview.Click
        If SetupThePrinting() Then
            Dim MyPrintPreviewDialog As PrintPreviewDialog = New PrintPreviewDialog()
            MyPrintPreviewDialog.Document = MyPrintDocument
            MyPrintPreviewDialog.ShowDialog()
        End If
    End Sub

    ' The Print Button
    Private Sub btnPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrint.Click
	if SetupThePrinting() then MyPrintDocument.Print
    end SUb